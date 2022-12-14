using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using WindowExceptionsValidation;
using CsvParser = WindowExceptionsValidation.CsvParser;

namespace WindowExceptionsGenerator;

public class Generator
{
    private readonly IEnumerable<CurrentDataRecord> _currentData;
    private readonly string _destinationPath;
    private readonly string _holiday;
    private readonly Dictionary<int, DateOnly> _holidayWeekMap;
    private readonly Dictionary<string, string> _marketCodeToNameMap;
    private readonly Dictionary<int, DayOfWeekMapRecord> _numericDayToDayOfWeekMap;
    private readonly IEnumerable<OpsPlanRecord> _opsPlan;

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
        Func<CurrentDataRecord, bool> ZoneNameMatches(OpsPlanRecord plan)
        {
            return d =>
            {
                var marketCode = plan.Zone_Code;
                var day = _numericDayToDayOfWeekMap[plan.Exception_Delivery_Date].Day_Of_Week.ToUpper();
                var zoneName = $"{marketCode}: {day}";
                        
                return d.Zone.Name.StartsWith(zoneName);
            };
        }

        string FormatBooleanAsString(bool value)
        {
            return value.ToString().ToUpper();
        }

        var doubleDeliveryZones = _opsPlan
            .Where(plan => !plan.Is_Employee_Zone)
            .Where(plan =>
            {
                var refWindowId = _currentData
                    .FirstOrDefault(ZoneNameMatches(plan))
                    ?.Window
                    .WindowId;

                return _currentData.Any(ZoneNameMatches(plan))
                       && _opsPlan.All(p => p.old_window_id != refWindowId);
            });

        var zonePairs = from zone in doubleDeliveryZones
            let marketCode = zone.Zone_Code
            let newDay = _numericDayToDayOfWeekMap[zone.Exception_Delivery_Date].Day_Of_Week.ToUpper()
            let originalDay = _numericDayToDayOfWeekMap[zone.Original_Delivery_Date].Day_Of_Week.ToUpper()
            let refZoneName = $"{marketCode}: {newDay}"
            let movedZoneName = $"{marketCode}: {originalDay}"
            let referenceZone = _currentData.First(d => d.Zone.Name.StartsWith(refZoneName))
            let movedZone = _currentData.First(d => d.Zone.Name.StartsWith(movedZoneName))
            select (movedZone, referenceZone);

        var zones = (from @group in zonePairs
            let movedZone = @group.movedZone.Zone
            let refZone = @group.referenceZone.Zone
            let refWindow = @group.referenceZone.Window
            let name = GenerateDoubleDeliveryZoneName(movedZone.Name, refWindow.StartDay)
            select new LineHaulZone
            {
                ZoneId = Guid.NewGuid().ToString(),
                FulfillmentCenterId = refZone.FulfillmentCenterId,
                Name = name,
                Timezone = refZone.Timezone,
                ZonePickupAddressId = refZone.ZonePickupAddressId,
                ExpectedServiceTimeInMinutes = refZone.ExpectedServiceTimeInMinutes,
                IsLineHaul = refZone.IsLineHaul.ToString(),
                PickupTime = refZone.PickupTime,
                MarketCode = refZone.MarketCode,
                TransitTime = refZone.TransitTime
            }).ToList();

        var lineHaulZones = zones
            .Where(z => z.IsLineHaul == "TRUE")
            .ToList();
        WriteToCsv("CSV Upload - Zones - Line Haul.csv", lineHaulZones);

        var localZones = zones
            .Where(z => z.IsLineHaul == "FALSE")
            .Cast<LocalZone>()
            .ToList();
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

    private void WriteToCsv<T>(string fileName, IEnumerable<T> data)
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