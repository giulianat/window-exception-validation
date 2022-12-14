using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using WindowExceptionsValidation;
using CsvParser = WindowExceptionsValidation.CsvParser;

namespace WindowExceptionsGenerator;

public class Generator
{
    private readonly string _destinationPath;
    private readonly string _holiday;
    private readonly IEnumerable<CurrentDataRecord> _currentData;
    private readonly IEnumerable<OpsPlanRecord> _opsPlan;
    private readonly Dictionary<int, DateOnly> _holidayWeekMap;
    private readonly Dictionary<string, string> _marketCodeToNameMap;
    private readonly Dictionary<int, DayOfWeekMapRecord> _numericDayToDayOfWeekMap;

    public Generator(string sourceDirectory, string destinationPath, string holiday, string sundayOfHolidayWeek)
    {
        _destinationPath = destinationPath;
        _holiday = holiday;
        _currentData = ReadCurrentData(sourceDirectory);
        _opsPlan = ReadOpsPlan(sourceDirectory);

        var sunday = DateOnly.ParseExact(sundayOfHolidayWeek, "MM/dd/yyyy");
        _holidayWeekMap = new Dictionary<int, DateOnly>
        {
            { 0, sunday },
            { 1, sunday.AddDays(1) },
            { 2, sunday.AddDays(2) },
            { 3, sunday.AddDays(3) },
            { 4, sunday.AddDays(4) },
            { 5, sunday.AddDays(5) },
            { 6, sunday.AddDays(6) }
        };

        //TODO: Move these out of Validator? 
        _marketCodeToNameMap = CsvParser.GetMarketCodeToNameMap();
        _numericDayToDayOfWeekMap = CsvParser.GetDayOfWeekMap();
    }

    public void GenerateZones()
    {
        string FormatTimestamp(string time) => TimeOnly.ParseExact(time, "H:mm:ss").ToString("HH:mm");
        var doubleDeliveryZones = _opsPlan
            .Where(plan =>
            {
                var referenceZones = _currentData
                    .Where(d =>
                    {
                        var zoneName =
                            $"{plan.Zone_Code}: {_numericDayToDayOfWeekMap[plan.Exception_Delivery_Date].Day_Of_Week.ToUpper()}";
                        return d.Zone.Name.StartsWith(zoneName);
                    })
                    .ToList();

                return !plan.Is_Employee_Zone
                       && referenceZones.Count == 1
                       && _opsPlan.All(p => p.old_window_id != referenceZones.First().Window.WindowId);
            });

        var zonePairs = (
            from zone in doubleDeliveryZones
            let refZoneName =
                $"{zone.Zone_Code}: {_numericDayToDayOfWeekMap[zone.Exception_Delivery_Date].Day_Of_Week.ToUpper()}"
            let referenceZone = _currentData.First(d => d.Zone.Name.StartsWith(refZoneName))
            let movedZoneName =
                $"{zone.Zone_Code}: {_numericDayToDayOfWeekMap[zone.Original_Delivery_Date].Day_Of_Week.ToUpper()}"
            let movedZone = _currentData.First(d => d.Zone.Name.StartsWith(movedZoneName))
            select (movedZone, referenceZone)
        ).ToList();

        var zones = (from @group in zonePairs
            let movedZone = @group.movedZone.Zone
            let refZoneData = @group.referenceZone
            let name = GenerateDoubleDeliveryZoneName(movedZone.Name, refZoneData.Window.StartDay)
            select new
            {
                zoneId = Guid.NewGuid().ToString(),
                fulfillmentCenterId = refZoneData.Zone.FulfillmentCenterId,
                name,
                timezone = refZoneData.Zone.Timezone,
                zonePickupAddressId = refZoneData.Zone.ZonePickupAddressId,
                expectedServiceTimeInMinutes = refZoneData.Zone.ExpectedServiceTimeInMinutes,
                isLineHaul = refZoneData.Zone.IsLineHaul.ToString().ToUpper(),
                pickupTime = FormatTimestamp(refZoneData.Zone.PickupTime),
                marketCode = refZoneData.Zone.MarketCode,
                transitTime = FormatTimestamp(refZoneData.Zone.TransitTime)
            }).ToList();

        var lineHaulZones = zones.Where(z => z.isLineHaul == "TRUE").ToList();
        WriteToCsv("CSV Upload - Zones - Line Haul.csv", lineHaulZones);

        var localZones = zones.Where(z => z.isLineHaul == "FALSE").Select(z => new
        {
            z.zoneId,
            z.fulfillmentCenterId,
            z.name,
            z.timezone,
            z.zonePickupAddressId,
            z.expectedServiceTimeInMinutes,
            z.isLineHaul,
            z.pickupTime,
            z.marketCode
        }).ToList();
        WriteToCsv("CSV Upload - Zones - Local.csv", localZones);
    }

    private string GenerateDoubleDeliveryZoneName(string movedZoneName, int refZoneStartDay)
    {
        var captureCollection = Regex.Match(movedZoneName, "([A-Z]{3}: [^ ]+)(.*)").Groups.Values.ToList();
        var prefix = captureCollection[1];
        var refZoneDayOfWeek = _numericDayToDayOfWeekMap[refZoneStartDay].Day_Of_Week.ToUpper();
        var suffix = captureCollection[2];

        return $"{prefix} / {refZoneDayOfWeek}{suffix}";
    }

    private void WriteToCsv(string fileName, IEnumerable<object> data)
    {
        var path = $"{_destinationPath}/{fileName}";
        using var writer = File.CreateText(path);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(data);
    }

    private static IEnumerable<OpsPlanRecord> ReadOpsPlan(string sourceDirectory)
    {
        var path = $"{sourceDirectory}/Ops Plan.csv";
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<OpsPlanRecordMap>();

        return csv.GetRecords<OpsPlanRecord>().ToList();
    }

    private static IEnumerable<CurrentDataRecord> ReadCurrentData(string sourceDirectory)
    {
        var path = $"{sourceDirectory}/Current Data.csv";
        using var currentDataReader = new StreamReader(path);
        using var currentDataCsv = new CsvReader(currentDataReader, CultureInfo.InvariantCulture);

        currentDataCsv.Context.RegisterClassMap<CurrentDataRecordMap>();

        return currentDataCsv.GetRecords<CurrentDataRecord>().ToList();
    }
}