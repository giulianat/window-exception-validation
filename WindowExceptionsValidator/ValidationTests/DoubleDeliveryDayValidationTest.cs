using Newtonsoft.Json;

namespace WindowExceptionsValidation.ValidationTests;

[TestFixture]
public class DoubleDeliveryDayValidationTest
{
    private const string ImportZoneCsv = @"./csv/Xmas and NY Holiday Market_Zone Exception Tool - Import Zone.csv";

    [Test]
    public void ShouldIdentifyDoubleDeliveryDaysGroupedByDay()
    {
        var contentSplitByLine = File.ReadAllLines(ImportZoneCsv);
        var contentSplitByColumn = contentSplitByLine.Select(l => l.Split(",")).Skip(2).ToList();
        var dupeSundays = GetDuplicateCitiesForDate(contentSplitByColumn, 1).ToList();
        var dupeMondays = GetDuplicateCitiesForDate(contentSplitByColumn, 2).ToList();
        var dupeTuesdays = GetDuplicateCitiesForDate(contentSplitByColumn, 3).ToList();
        var dupeWednesdays = GetDuplicateCitiesForDate(contentSplitByColumn, 4).ToList();
        var dupeThursdays = GetDuplicateCitiesForDate(contentSplitByColumn, 5).ToList();
        var dupeFridays = GetDuplicateCitiesForDate(contentSplitByColumn, 6).ToList();

        Console.WriteLine("Double Deliveries on...");
        Console.WriteLine($"Sunday {dupeSundays.Count()} [{string.Join(", ", dupeSundays)}]");
        Console.WriteLine($"Monday {dupeMondays.Count()} [{string.Join(", ", dupeMondays)}]");
        Console.WriteLine($"Tuesday {dupeTuesdays.Count()} [{string.Join(", ", dupeTuesdays)}]");
        Console.WriteLine($"Wednesday {dupeWednesdays.Count()} [{string.Join(", ", dupeWednesdays)}]");
        Console.WriteLine($"Thursday {dupeThursdays.Count()} [{string.Join(", ", dupeThursdays)}]");
        Console.WriteLine($"Friday {dupeFridays.Count()} [{string.Join(", ", dupeFridays)}]");

        Assert.Pass();
    }

    [Test]
    public void ShouldIdentifyDoubleDeliveryDaysGroupedByOriginalZone()
    {
        var opsPlan = CsvParser.GetOpsPlans()
            .GroupBy(p => $"{p.City_Name} on {p.Exception_Delivery_Day}")
            .Where(g => g.Count() > 1)
            .ToDictionary(g => g.Key,
                g => g.Select(p => p.Original_Delivery_Day).OrderBy(d => d).Select(d => d.ToString()));

        Console.WriteLine(JsonConvert.SerializeObject(opsPlan, Formatting.Indented));

        Assert.Pass();
    }

    private static IEnumerable<string> GetDuplicateCitiesForDate(IEnumerable<string[]> splitByColumn, int columnIndex)
    {
        return splitByColumn
            .Select(l => l[columnIndex].Split(" - "))
            .Where(l => l.Length == 2)
            .Where(l => l[1] != "EMP")
            .Select(l => l[0])
            .GroupBy(c => c)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key);
    }
}