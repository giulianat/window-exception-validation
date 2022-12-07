using System.Globalization;
using CsvHelper;

namespace WindowExceptionsValidation;

public static class Parser
{
    public static Dictionary<string, string> GetMarketCodeToNameMap()
    {
        using var mappingsReader = new StreamReader(@"./csv/Mappings/MarketMap.csv");
        using var mappingsCsv = new CsvReader(mappingsReader, CultureInfo.InvariantCulture);

        return mappingsCsv
            .GetRecords<MarketMapRecord>()
            .ToDictionary(m => m.Market_Code, m => m.Market_Name);
    }

    public static Dictionary<string, DayOfWeekMapRecord> GetDayOfWeekMap()
    {
        using var mappingsReader = new StreamReader(@"./csv/Mappings/DayOfWeekMap.csv");
        using var mappingsCsv = new CsvReader(mappingsReader, CultureInfo.InvariantCulture);

        return mappingsCsv
            .GetRecords<DayOfWeekMapRecord>()
            .ToDictionary(m => m.Numeric_Day_Of_Week, m => m);
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