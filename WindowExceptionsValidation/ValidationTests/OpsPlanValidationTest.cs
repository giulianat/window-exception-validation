using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;

namespace WindowExceptionsValidation.ValidationTests;

[TestFixture]
public class OpsPlanValidationTests
{
    private const string InputFilePath = @"./csv/Xmas and NY Holiday Market_Zone Exception Tool - Import Zone.csv";

    private const string FilePathToBeValidated =
        @"./csv/Patrick's Copy of Xmas and NY Holiday Market_Zone Exception Tool - Output corrected.csv";
        //@"./csv/[Dylan] Second Validation – New Output - Output.csv";
        //@"./csv/Christmas and New Years Window Exceptions - Ops Plan.csv";
        
    private readonly List<OpsPlanRecord> _expectedOpsPlan = Parser.ParseImportZone(InputFilePath).ToList();

    [Test]
    public void ShouldHaveExpectedNumberOfWindowExceptions()
    {
        using var reader = new StreamReader(FilePathToBeValidated);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var actualOpsPlan = csv.GetRecords<OpsPlanRecord>().ToList();

        Assert.That(actualOpsPlan, Has.Count.EqualTo(_expectedOpsPlan.Count));
        Assert.That(_expectedOpsPlan.Except(actualOpsPlan), Is.Empty);
        Assert.That(actualOpsPlan.Except(_expectedOpsPlan), Is.Empty);
    }

    [Test]
    public void ShouldContainMatchingOutputForEachInput()
    {
        using var reader = new StreamReader(FilePathToBeValidated);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var actualOpsPlan = csv.GetRecords<OpsPlanRecord>().ToList();

        foreach (var record in _expectedOpsPlan) Assert.That(actualOpsPlan, Contains.Item(record));
    }

    [Test]
    public void ShouldHaveContentInAllColumns()
    {
        using var reader = new StreamReader(FilePathToBeValidated);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var actualOpsPlan = csv.GetRecords<OpsPlanRecord>().ToList();

        foreach (var record in actualOpsPlan)
        {
            Assert.That(record.City_Name, Is.Not.Empty);
            Assert.That(record.Original_Delivery_Day.ToString(), Is.Not.Empty);
            Assert.That(record.Exception_Delivery_Day.ToString(), Is.Not.Empty);
            Assert.That(record.Original_Delivery_Date, Is.GreaterThanOrEqualTo(0));
            Assert.That(record.Exception_Delivery_Date, Is.GreaterThanOrEqualTo(0));
        }
    }

    [Test]
    public void ShouldMatchImportZoneTotals()
    {
        using var reader = new StreamReader(FilePathToBeValidated);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var opsPlan = csv.GetRecords<OpsPlanRecord>().ToList();
        var outputUnchanged = opsPlan.Where(r => r.Exception_Delivery_Day == r.Original_Delivery_Day);
        
        var content = File.ReadAllText(InputFilePath);
        var contentSplitByLine = File.ReadAllLines(InputFilePath);
        var numberOfDashes = Regex.Matches(content, " - ").Count;
        var numberOfEmp = Regex.Matches(content, "EMP").Count;
        var contentSplitByColumn = contentSplitByLine.Select(l => l.Split(",")).Skip(2).ToList();
        var unchangedSundays = contentSplitByColumn.Count(l => l[1].Contains("Sunday"));
        var unchangedMondays = contentSplitByColumn.Count(l => l[2].Contains("Monday"));
        var unchangedTuesdays = contentSplitByColumn.Count(l => l[3].Contains("Tuesday"));
        var unchangedWednesdays = contentSplitByColumn.Count(l => l[4].Contains("Wednesday"));
        var unchangedThursdays = contentSplitByColumn.Count(l => l[5].Contains("Thursday"));
        var unchangedFridays = contentSplitByColumn.Count(l => l[6].Contains("Friday"));
        var inputUnchanged = unchangedSundays + unchangedMondays + unchangedTuesdays + unchangedWednesdays + unchangedThursdays + unchangedFridays;

        Assert.That(opsPlan, Has.Count.EqualTo(numberOfDashes - numberOfEmp));
        Assert.That(outputUnchanged.Count(), Is.EqualTo(inputUnchanged));
    }
    
    [Test]
    public void ShouldSummarizeOpsPlan()
    {
        var contentSplitByLine = File.ReadAllLines(FilePathToBeValidated);
        using var reader = new StreamReader(FilePathToBeValidated);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var opsPlan = csv.GetRecords<OpsPlanRecord>().ToList();
        var unchanged = opsPlan.Where(r => r.Exception_Delivery_Day == r.Original_Delivery_Day);
        
        Console.WriteLine($"{contentSplitByLine.Length} lines");
        Console.WriteLine($"{opsPlan.Count} total window exceptions");
        Console.WriteLine($"{unchanged.Count()} unchanged windows");
        
        Assert.Pass();
    }
}