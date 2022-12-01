using System.Globalization;
using CsvHelper;

namespace WindowExceptionsValidation;

public record ImportZone
{
    public IEnumerable<ImportZoneHeader> ColumnHeaders;
    public IEnumerable<ImportZoneRecord> Records;
}

public record ImportZoneHeader
{
    public DayOfWeek Day;
    public int Index;
}

public static class Parser
{
    public static ImportZone ParseImportZone(string[] allLines)
    {
        var deliveryIndices = allLines[0].Split(",").Where(i => !string.IsNullOrEmpty(i)).ToList();
        var deliveryDaysOfTheWeek = allLines[1].Split(",").Where(i => !string.IsNullOrEmpty(i)).ToList();
        var headers = deliveryIndices.Select((index, i) => new ImportZoneHeader
        {
            Index = int.Parse(index),
            Day = Enum.Parse<DayOfWeek>(deliveryDaysOfTheWeek[i])
        });
        var dataLines = allLines.Skip(2).ToArray();
        var records = dataLines
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

        return new ImportZone
        {
            ColumnHeaders = headers,
            Records = records
        };
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
}