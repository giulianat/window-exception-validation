using System.Globalization;
using CsvHelper;

namespace WindowExceptionsValidation;

public static class CsvParser
{
    public static Dictionary<string, string> GetMarketCodeToNameMap()
    {
        using var mappingsReader = new StreamReader(@"./csv/Mappings/MarketMap.csv");
        using var mappingsCsv = new CsvReader(mappingsReader, CultureInfo.InvariantCulture);

        return mappingsCsv
            .GetRecords<MarketMapRecord>()
            .ToDictionary(m => m.Market_Code, m => m.Market_Name);
    }

    public static Dictionary<int, DayOfWeekMapRecord> GetDayOfWeekMap()
    {
        using var mappingsReader = new StreamReader(@"./csv/Mappings/DayOfWeekMap.csv");
        using var mappingsCsv = new CsvReader(mappingsReader, CultureInfo.InvariantCulture);

        return mappingsCsv
            .GetRecords<DayOfWeekMapRecord>()
            .ToDictionary(m => m.Numeric_Day_Of_Week, m => m);
    }

    public static IEnumerable<ZonesRecord> GetZones()
    {
        using var reader = new StreamReader(@"./csv/Christmas and New Years Window Exceptions - Zones.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<ZonesRecord>().ToList();
    }

    public static IEnumerable<CurrentDataRecord> GetCurrentData()
    {
        const string path = @"./csv/Christmas and New Years Window Exceptions - Current Data.csv";
        using var currentDataReader = new StreamReader(path);
        using var currentDataCsv = new CsvReader(currentDataReader, CultureInfo.InvariantCulture);

        currentDataCsv.Context.RegisterClassMap<CurrentDataRecordMap>();

        return currentDataCsv.GetRecords<CurrentDataRecord>().ToList();
    }

    public static IEnumerable<EmpZoneRecord> GetEmpZones()
    {
        const string path = @"./csv/Xmas and NY Holiday Market_Zone Exception Tool - EMP Zone - GB.csv";
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<EmpZoneRecordMap>();

        return csv.GetRecords<EmpZoneRecord>().ToList();
    }

    public static IEnumerable<ExceptionRecord> GetExceptions()
    {
        using var reader = new StreamReader(@"./csv/Christmas and New Years Window Exceptions - Exceptions.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<ExceptionRecord>().ToList();
    }

    public static IEnumerable<OpsPlanRecord> GetOpsPlans()
    {
        const string path = @"./csv/Christmas and New Years Window Exceptions - Ops Plan.csv";
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<OpsPlanRecordMap>();

        return csv.GetRecords<OpsPlanRecord>().ToList();
        
    }

    public static IEnumerable<WindowsRecord> GetWindows()
    {
        using var reader = new StreamReader(@"./csv/Christmas and New Years Window Exceptions - Windows.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<WindowsRecord>().ToList();
    }

    public static IEnumerable<ZoneToDayMappingOutputRecord> GetZoneToDayMappingOutput()
    {
        const string path = @"./csv/Christmas and New Years Window Exceptions - Zone to Day Mapping Output.csv";
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<ZoneToDayMappingOutputRecordMap>();
        
        return csv.GetRecords<ZoneToDayMappingOutputRecord>().ToList();
    }

    public static IEnumerable<ImportZoneRecord> ParseImportZone(string[] allLines)
    {
        var dataLines = allLines.Skip(2).ToArray();

        return dataLines
            .Select(r =>
            {
                var (s, m, t, w, h, f) = r.Split(",") switch
                {
                    var a => (a[1], a[2], a[3], a[4], a[5], a[6])
                };

                return new ImportZoneRecord
                {
                    Sunday = s,
                    Monday = m,
                    Tuesday = t,
                    Wednesday = w,
                    Thursday = h,
                    Friday = f
                };
            });
    }

    public static IEnumerable<OpsPlanRecord> ParseImportZone(string filePath)
    {
        using var reader = new StreamReader(filePath);
        var unused = reader.ReadLine();
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<ImportZoneRecord>()
            .SelectMany(r =>
            {
                var records = new List<OpsPlanRecord?>
                {
                    CreateOpsPlanRecord(r.Sunday, DayOfWeek.Sunday),
                    CreateOpsPlanRecord(r.Monday, DayOfWeek.Monday),
                    CreateOpsPlanRecord(r.Tuesday, DayOfWeek.Tuesday),
                    CreateOpsPlanRecord(r.Wednesday, DayOfWeek.Wednesday),
                    CreateOpsPlanRecord(r.Thursday, DayOfWeek.Thursday),
                    CreateOpsPlanRecord(r.Friday, DayOfWeek.Friday)
                };
                return records;
            }).OfType<OpsPlanRecord>()
            .ToList();
    }

    public static IEnumerable<OpsPlanRecord> ParseImportZoneEmpRecords(string filePath)
    {
        using var reader = new StreamReader(filePath);
        var unused = reader.ReadLine();
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<ImportZoneRecord>()
            .SelectMany(r =>
            {
                var records = new List<OpsPlanRecord?>
                {
                    CreateEmpRecord(r.Sunday, DayOfWeek.Sunday),
                    CreateEmpRecord(r.Monday, DayOfWeek.Monday),
                    CreateEmpRecord(r.Tuesday, DayOfWeek.Tuesday),
                    CreateEmpRecord(r.Wednesday, DayOfWeek.Wednesday),
                    CreateEmpRecord(r.Thursday, DayOfWeek.Thursday),
                    CreateEmpRecord(r.Friday, DayOfWeek.Friday)
                };
                return records;
            }).OfType<OpsPlanRecord>()
            .ToList();
    }

    public static IEnumerable<OpsPlanRecord> ParseImportZoneWithChangesToDeliveryDay(string filePath)
    {
        return ParseImportZone(filePath).Where(r => r.Exception_Delivery_Date != r.Original_Delivery_Date);
    }

    private static OpsPlanRecord? CreateOpsPlanRecord(string record, DayOfWeek exceptionDeliveryDay)
    {
        if (string.IsNullOrEmpty(record)) return null;
        var (city, originalDeliveryDay) = record.Split(" - ") switch
        {
            var a => (a[0], a[1])
        };

        if (originalDeliveryDay == "EMP") return null;

        var originalDay = Enum.Parse<DayOfWeek>(originalDeliveryDay);
        return new OpsPlanRecord
        {
            City_Name = city,
            Original_Delivery_Day = originalDay,
            Original_Delivery_Date = (int)originalDay,
            Exception_Delivery_Day = exceptionDeliveryDay,
            Exception_Delivery_Date = (int)exceptionDeliveryDay
        };
    }

    private static OpsPlanRecord? CreateEmpRecord(string record, DayOfWeek exceptionDeliveryDay)
    {
        if (string.IsNullOrEmpty(record)) return null;
        var (city, originalDeliveryDay) = record.Split(" - ") switch
        {
            var a => (a[0], a[1])
        };

        if (originalDeliveryDay != "EMP") return null;

        return new OpsPlanRecord
        {
            City_Name = city,
            Exception_Delivery_Day = exceptionDeliveryDay,
            Exception_Delivery_Date = (int)exceptionDeliveryDay
        };
    }
}