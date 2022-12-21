using System.Text.RegularExpressions;
using WindowExceptionsValidation.Entities;

namespace WindowExceptionsValidation.ValidationTests;

[TestFixture]
public class ImportZoneValidationTest
{
    [SetUp]
    public void Setup()
    {
        var contentSplitByLine = File.ReadAllLines(FilePath);
        _deliveryIndices = contentSplitByLine[0].Split(",").Where(i => !string.IsNullOrEmpty(i)).ToList();
        _deliveryDaysOfTheWeek = contentSplitByLine[1].Split(",").Where(i => !string.IsNullOrEmpty(i)).ToList();
        _records = CsvParser.ParseImportZone(contentSplitByLine).ToList();
    }

    private const string FilePath = @"./csv/Xmas and NY Holiday Market_Zone Exception Tool - Import Zone.csv";

    private List<string> _deliveryDaysOfTheWeek = new();
    private List<string> _deliveryIndices = new();
    private List<ImportZoneRecord> _records = new();

    [Test]
    public void ShouldSummarizeImportZone()
    {
        var content = File.ReadAllText(FilePath);
        var contentSplitByLine = File.ReadAllLines(FilePath);
        var numberOfDashes = Regex.Matches(content, " - ").Count;
        var numberOfEmp = Regex.Matches(content, "EMP").Count;
        var contentSplitByColumn = contentSplitByLine.Select(l => l.Split(",")).Skip(2).ToList();
        var unchangedSundays = contentSplitByColumn.Count(l => l[1].Contains("Sunday"));
        var unchangedMondays = contentSplitByColumn.Count(l => l[2].Contains("Monday"));
        var unchangedTuesdays = contentSplitByColumn.Count(l => l[3].Contains("Tuesday"));
        var unchangedWednesdays = contentSplitByColumn.Count(l => l[4].Contains("Wednesday"));
        var unchangedThursdays = contentSplitByColumn.Count(l => l[5].Contains("Thursday"));
        var unchangedFridays = contentSplitByColumn.Count(l => l[6].Contains("Friday"));
        var numberOfUnchangedWindows = unchangedSundays + unchangedMondays + unchangedTuesdays + unchangedWednesdays +
                                       unchangedThursdays + unchangedFridays;

        Console.WriteLine($"{contentSplitByLine.Length} lines");
        Console.WriteLine($"{numberOfDashes} total window exceptions");
        Console.WriteLine($"{numberOfEmp} EMP window exceptions");
        Console.WriteLine($"{numberOfUnchangedWindows} unchanged windows");

        Assert.Pass();
    }

    [Test]
    public void ShouldContainValidHeaderRows()
    {
        Assert.That(_deliveryIndices, Has.Count.EqualTo(_deliveryDaysOfTheWeek.Count));
        foreach (var deliveryIndex in _deliveryIndices) Assert.DoesNotThrow(() => int.Parse(deliveryIndex));

        foreach (var dayOfWeek in _deliveryDaysOfTheWeek)
        {
            var i = _deliveryDaysOfTheWeek.ToList().IndexOf(dayOfWeek);
            Assert.Multiple(() =>
            {
                Assert.That(Enum.TryParse(dayOfWeek, out DayOfWeek val), Is.True);
                Assert.That((int)val, Is.EqualTo(int.Parse(_deliveryIndices[i])));
            });
        }
    }

    [Test]
    public void ShouldContainMarketAndZoneInEachRecord()
    {
        Assert.That(_records, Has.Count.EqualTo(93));
        foreach (var importZoneRecord in _records)
        {
            var index = _records.IndexOf(importZoneRecord);

            Assert.Multiple(() =>
            {
                AssertThatDeliveryEntryIsValid(importZoneRecord.Sunday, index);
                AssertThatDeliveryEntryIsValid(importZoneRecord.Monday, index);
                AssertThatDeliveryEntryIsValid(importZoneRecord.Tuesday, index);
                AssertThatDeliveryEntryIsValid(importZoneRecord.Wednesday, index);
                AssertThatDeliveryEntryIsValid(importZoneRecord.Thursday, index);
                AssertThatDeliveryEntryIsValid(importZoneRecord.Friday, index);
            });
        }
    }

    private static void AssertThatDeliveryEntryIsValid(string deliveryEntry, int index)
    {
        if (deliveryEntry == string.Empty) return;

        Assert.That(deliveryEntry, Does.Contain(" - "));

        var delivery = deliveryEntry.Split(" - ");
        Assert.That(delivery, Has.Length.EqualTo(2), $"{deliveryEntry} on line {index}");

        var dayOfWeek = delivery[1];
        if (dayOfWeek != "EMP")
            Assert.That(Enum.TryParse(dayOfWeek, out DayOfWeek _), Is.True);
    }
}