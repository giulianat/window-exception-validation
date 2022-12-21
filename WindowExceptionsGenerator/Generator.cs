using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using WindowExceptionsGenerator.Entities;
using WindowExceptionsValidation.Entities;
using Window = WindowExceptionsGenerator.Entities.Window;

namespace WindowExceptionsGenerator;

public class Generator
{
    public static readonly Dictionary<string, string> MarketCodeToNameMap = new()
    {
        { "ATM", "College Station" },
        { "AUS", "Austin" },
        { "BDN", "Bend" },
        { "BLI", "Bellingham" },
        { "BNA", "Nashville" },
        { "BOI", "Boise" },
        { "BOS", "Boston" },
        { "BRX", "Bronx" },
        { "BTR", "Baton Rouge" },
        { "BWI", "Severn" },
        { "CHI", "Chicago" },
        { "CLE", "Cleveland" },
        { "CRP", "Corpus Christi" },
        { "DAY", "Dayton" },
        { "DEN", "Denver" },
        { "DFW", "Dallas-Fort Worth" },
        { "DSM", "Des Moines" },
        { "DTW", "Detroit" },
        { "EBY", "Pittsburg CA" },
        { "EUG", "Eugene" },
        { "EWR", "Newark" },
        { "HAR", "Hartford" },
        { "IAH", "Houston" },
        { "IND", "Indianapolis" },
        { "KCI", "Kansas City" },
        { "LAS", "Las Vegas" },
        { "LAX", "Los Angeles" },
        { "LIM", "Long Island" },
        { "LOU", "Louisville" },
        { "MCE", "Merced" },
        { "MKE", "Milwaukee" },
        { "MOC", "Montgomery County" },
        { "MSN", "Madison WI" },
        { "MSP", "Minneapolis-Saint Paul" },
        { "NBY", "Santa Rosa" },
        { "NVA", "Northern Virginia" },
        { "NWI", "Northwest Indiana" },
        { "OKC", "Oklahoma City" },
        { "OLM", "Olympia" },
        { "PDX", "Portland" },
        { "PHL", "Philadelphia" },
        { "PHX", "Phoenix" },
        { "PIT", "Pittsburgh" },
        { "RDU", "Raleigh" },
        { "RIC", "Richmond" },
        { "RNO", "Reno" },
        { "SAC", "Sacramento" },
        { "SAN", "San Diego" },
        { "SAT", "San Antonio" },
        { "SBA", "Santa Barbara" },
        { "SBY", "San Jose" },
        { "SEA", "Seattle" },
        { "SFO", "San Francisco" },
        { "SLC", "Salt Lake City" },
        { "SNA", "Orange County" },
        { "SPK", "Spokane" },
        { "STL", "St Louis" },
        { "SWM", "Southwest Michigan" },
        { "TPL", "Temple/Waco" },
        { "TUS", "Tucson" }
    };

    public static readonly Dictionary<int, DayOfWeekMapRecord> NumericDayToDayOfWeekMap = new()
    {
        { 0, new DayOfWeekMapRecord { Abbreviation = "Sun", Day_Of_Week = "Sunday", Numeric_Day_Of_Week = 0 } },
        { 1, new DayOfWeekMapRecord { Abbreviation = "Mon", Day_Of_Week = "Monday", Numeric_Day_Of_Week = 1 } },
        { 2, new DayOfWeekMapRecord { Abbreviation = "Tues", Day_Of_Week = "Tuesday", Numeric_Day_Of_Week = 2 } },
        { 3, new DayOfWeekMapRecord { Abbreviation = "Wed", Day_Of_Week = "Wednesday", Numeric_Day_Of_Week = 3 } },
        { 4, new DayOfWeekMapRecord { Abbreviation = "Thurs", Day_Of_Week = "Thursday", Numeric_Day_Of_Week = 4 } },
        { 5, new DayOfWeekMapRecord { Abbreviation = "Fri", Day_Of_Week = "Friday", Numeric_Day_Of_Week = 5 } },
        { 6, new DayOfWeekMapRecord { Abbreviation = "Sat", Day_Of_Week = "Saturday", Numeric_Day_Of_Week = 6 } }
    };

    private readonly IEnumerable<CurrentDataRecord> _currentData;
    private readonly string _destinationPath;
    private readonly string _holiday;
    private readonly Dictionary<int, DateOnly> _holidayWeekMap;
    private readonly IEnumerable<OpsPlanRecord> _opsPlan;
    private readonly string _sourceDirectory;

    public Generator(string sourceDirectory, string destinationPath, string holiday, string sundayOfHolidayWeek)
    {
        _sourceDirectory = sourceDirectory;
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
    }

    public void GenerateZones()
    {
        var doubleDeliveryZonePairs = GetDoubleDeliveryZones();

        var zones = (from @group in doubleDeliveryZonePairs
            let movedZone = @group.movedZone.Zone
            let refZone = @group.referenceZone.Zone
            let refWindow = @group.referenceZone.Window
            let name = GenerateDoubleDeliveryZoneName(@group)
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

    public void GenerateWindows()
    {
        string FormatTime(string time) => TimeOnly.Parse(time).ToString("HH:mm:ss");
        var doubleDeliveryZones = GetDoubleDeliveryZones();
        var zoneNameToGeneratedIdMap = ReadZones(_sourceDirectory).ToDictionary(z => z.Name, z=> z.ZoneId);
        var plansByWindowType = _opsPlan
            .GroupBy(p => doubleDeliveryZones.Any(pair => pair.movedZone.Window.WindowId == p.old_window_id))
            .ToList();
        var changedSingleWindows = plansByWindowType.First(g => !g.Key);
        var changedDoubleDeliveryWindows = plansByWindowType.First(g => g.Key);
        
        var changedWindows = from opsPlan in changedSingleWindows
            let windowId = opsPlan.old_window_id
            let refData = _currentData.First(d => d.Window.WindowId == windowId)
            let refWindow = refData.Window
            let newDeliveryDate = opsPlan.Exception_Delivery_Date.ToString()
            select new Window
            {
                windowId = Guid.NewGuid().ToString(),
                zoneId = refData.Zone.ZoneId,
                customizationStartDay = refWindow.CustomizationStartDay,
                customizationStartTime = FormatTime(refWindow.CustomizationStartTime),
                customizationEndDay = refWindow.CustomizationEndDay,
                customizationEndTime = FormatTime(refWindow.CustomizationEndTime),
                dispatchDay = refWindow.DispatchDay.ToString(),
                dispatchTime = FormatTime(refWindow.DispatchTime),
                startDay = newDeliveryDate,
                startTime = FormatTime(refWindow.StartTime),
                endDay = newDeliveryDate,
                endTime = FormatTime(refWindow.EndTime),
                fulfillmentCenterId = refWindow.FulfillmentCenterId,
                deliveryPrice = refWindow.DeliveryPrice,
                subtotalMin = refWindow.SubtotalMin,
                deliveryProvider = refWindow.DeliveryProvider,
                packDateOffset = refWindow.PackDateOffset,
                carrierDaysInTransit = refWindow.CarrierDaysinTransit,
                messageToUser = $"{_holiday.ToLower()}-window-exceptions-{_holidayWeekMap[0].Year}",
            };

        var movedDoubleDeliveryWindows = from opsPlan in changedDoubleDeliveryWindows
            let windowId = opsPlan.old_window_id
            let doubleDeliveryZone = doubleDeliveryZones.First(pair => pair.movedZone.Window.WindowId == windowId)
            let deliveryZoneName = GenerateDoubleDeliveryZoneName(doubleDeliveryZone)
            let refData = _currentData.First(d => d.Window.WindowId == windowId)
            let zoneId = zoneNameToGeneratedIdMap[deliveryZoneName]
            let refWindow = refData.Window
            let newDeliveryDate = opsPlan.Exception_Delivery_Date.ToString()
            let movedDispatchDateTime = GetDispatchDateTime(doubleDeliveryZone.movedZone)
            let refDispatchDateTime = GetDispatchDateTime(doubleDeliveryZone.referenceZone)
            let maxDispatchDateTime = movedDispatchDateTime > refDispatchDateTime ? movedDispatchDateTime : refDispatchDateTime
            select new Window
            {
                windowId = Guid.NewGuid().ToString(),
                zoneId = zoneId,
                customizationStartDay = refWindow.CustomizationStartDay,
                customizationStartTime = FormatTime(refWindow.CustomizationStartTime),
                customizationEndDay = refWindow.CustomizationEndDay,
                customizationEndTime = FormatTime(refWindow.CustomizationEndTime),
                dispatchDay = doubleDeliveryZone.referenceZone.Window.DispatchDay.ToString(),
                dispatchTime = doubleDeliveryZone.referenceZone.Window.DispatchTime,
                startDay = newDeliveryDate,
                startTime = FormatTime(refWindow.StartTime),
                endDay = newDeliveryDate,
                endTime = FormatTime(refWindow.EndTime),
                fulfillmentCenterId = refWindow.FulfillmentCenterId,
                deliveryPrice = refWindow.DeliveryPrice,
                subtotalMin = refWindow.SubtotalMin,
                deliveryProvider = refWindow.DeliveryProvider,
                packDateOffset = refWindow.PackDateOffset,
                carrierDaysInTransit = refWindow.CarrierDaysinTransit,
                messageToUser = $"{_holiday.ToLower()}-window-exceptions-{_holidayWeekMap[0].Year}",
            };

        var doubleDeliveryUnchangedWindows = from doubleDeliveryZone in doubleDeliveryZones
            let windowId = doubleDeliveryZone.referenceZone.Window.WindowId
            let refData = _currentData.First(d => d.Window.WindowId == windowId)
            let doubleDeliveryZoneName = GenerateDoubleDeliveryZoneName(doubleDeliveryZone)
            let newZoneId = zoneNameToGeneratedIdMap[doubleDeliveryZoneName]
            let refWindow = refData.Window
            select new Window
            {
                windowId = Guid.NewGuid().ToString(),
                zoneId = newZoneId,
                customizationStartDay = refWindow.CustomizationStartDay,
                customizationStartTime = FormatTime(refWindow.CustomizationStartTime),
                customizationEndDay = refWindow.CustomizationEndDay,
                customizationEndTime = FormatTime(refWindow.CustomizationEndTime),
                dispatchDay = refWindow.DispatchDay.ToString(),
                dispatchTime = refWindow.DispatchTime,
                startDay = refWindow.StartDay.ToString(),
                startTime = FormatTime(refWindow.StartTime),
                endDay = refWindow.EndDay,
                endTime = FormatTime(refWindow.EndTime),
                fulfillmentCenterId = refWindow.FulfillmentCenterId,
                deliveryPrice = refWindow.DeliveryPrice,
                subtotalMin = refWindow.SubtotalMin,
                deliveryProvider = refWindow.DeliveryProvider,
                packDateOffset = refWindow.PackDateOffset,
                carrierDaysInTransit = refWindow.CarrierDaysinTransit,
                messageToUser = $"{_holiday.ToLower()}-window-exceptions-{_holidayWeekMap[0].Year}",
            };

        var windows = changedWindows.Union(movedDoubleDeliveryWindows).Union(doubleDeliveryUnchangedWindows);
        WriteToCsv("CSV Upload - Windows.csv", windows);
    }

    private DateTime GetDispatchDateTime(CurrentDataRecord record)
    {
        var window = record.Window;
        var custoStartDayOfWeek = int.Parse(window.CustomizationStartDay);
        var custoClosedDayOfWeek = int.Parse(window.CustomizationEndDay);
        var weekOffset = custoStartDayOfWeek > custoClosedDayOfWeek ? 0 : 7;

        return _holidayWeekMap[window.DispatchDay]
            .AddDays(weekOffset)
            .ToDateTime(TimeOnly.Parse(window.DispatchTime));
    }

    private IEnumerable<(CurrentDataRecord movedZone, CurrentDataRecord referenceZone)> GetDoubleDeliveryZones()
    {
        var doubleDeliveryZones = _opsPlan
            .Where(plan => !plan.Is_Employee_Zone)
            .Where(plan =>
            {
                bool MatchingZoneName(CurrentDataRecord d)
                {
                    return ZoneNameMatchesPredicate(d, plan);
                }

                var refWindowId = _currentData
                    .FirstOrDefault(MatchingZoneName)
                    ?.Window
                    .WindowId;

                return _currentData.Any(MatchingZoneName)
                       && _opsPlan.All(p => p.old_window_id != refWindowId);
            });

        var doubleDeliveryZonePairs = from zone in doubleDeliveryZones
            let marketCode = zone.Zone_Code
            let newDay = NumericDayToDayOfWeekMap[zone.Exception_Delivery_Date].Day_Of_Week.ToUpper()
            let originalDay = NumericDayToDayOfWeekMap[zone.Original_Delivery_Date].Day_Of_Week.ToUpper()
            let refZoneName = $"{marketCode}: {newDay}"
            let movedZoneName = $"{marketCode}: {originalDay}"
            let referenceZone = _currentData.First(d => d.Zone.Name.StartsWith(refZoneName))
            let movedZone = _currentData.First(d => d.Zone.Name.StartsWith(movedZoneName))
            select (movedZone, referenceZone);

        return doubleDeliveryZonePairs;
    }


    private static bool ZoneNameMatchesPredicate(CurrentDataRecord data, OpsPlanRecord plan)
    {
        var marketCode = plan.Zone_Code;
        var day = NumericDayToDayOfWeekMap[plan.Exception_Delivery_Date].Day_Of_Week.ToUpper();
        var zoneName = $"{marketCode}: {day}";

        return data.Zone.Name.StartsWith(zoneName);
    }

    private static string GenerateDoubleDeliveryZoneName((CurrentDataRecord movedZone, CurrentDataRecord referenceZone) doubleDeliveryZonePair)
    {
        var movedZoneName = doubleDeliveryZonePair.movedZone.Zone.Name;
        var refZoneStartDay = doubleDeliveryZonePair.referenceZone.Window.StartDay;
        var captureCollection = Regex.Match(movedZoneName, "([A-Z]{3}: [^ ]+)(.*)").Groups.Values.ToList();
        var prefix = captureCollection[1];
        var refZoneDayOfWeek = NumericDayToDayOfWeekMap[refZoneStartDay].Day_Of_Week.ToUpper();
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

    private static IEnumerable<LocalZone> ReadZones(string sourceDirectory)
    {
        var pathForLineHaul = $"{sourceDirectory}/CSV Upload - Zones - Line Haul.csv";
        using var lineHaulReader = new StreamReader(pathForLineHaul);
        using var lineHaulCsv = new CsvReader(lineHaulReader, CultureInfo.InvariantCulture);
        var lineHaulZones = lineHaulCsv.GetRecords<LineHaulZone>();

        var pathForLocal = $"{sourceDirectory}/CSV Upload - Zones - Local.csv";
        using var localReader = new StreamReader(pathForLocal);
        using var localCsv = new CsvReader(localReader, CultureInfo.InvariantCulture);
        var localZones = localCsv.GetRecords<LocalZone>();

        return lineHaulZones.Union(localZones).ToList();
    }
}