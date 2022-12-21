using System.Text.Json;
using WindowExceptionsValidation.Entities;

namespace WindowExceptionsValidation.ValidationTests;

[TestFixture]
public class ExceptionsValidationTest
{
    [SetUp]
    public void Setup()
    {
        _exceptions = CsvParser.GetExceptions().ToList();
    }

    private static readonly DateOnly[] Holidays =
    {
        new(2022, 12, 24),
        new(2022, 12, 25),
        new(2022, 12, 26),
        new(2023, 1, 1),
        new(2023, 1, 2)
    };

    private List<ExceptionRecord> _exceptions = new();

    [Test]
    public void ShouldHaveReplacementDeliveryDay()
    {
        var hasEmptyStartOrEndDays = _exceptions
            .Any(w => string.IsNullOrEmpty(w.replacementDeliveryDay));

        Assert.That(hasEmptyStartOrEndDays, Is.False);
    }

    [Test]
    public void ShouldNotDeliverOnHoliday()
    {
        var numberOfStartDaysOnAHoliday = _exceptions
            .Where(e => !string.IsNullOrEmpty(e.replacementDeliveryDay))
            .Select(w => Enum.Parse<DayOfWeek>(w.replacementDeliveryDay))
            .Count(dow => Holidays.Select(h => h.DayOfWeek).Contains(dow));

        Assert.That(numberOfStartDaysOnAHoliday, Is.Zero);
    }

    [Test]
    public void ShouldSummarizeUnchangedWindows()
    {
        var unchangedWindows = _exceptions
            .Where(e => e.originalDeliveryDay == e.replacementDeliveryDay)
            .Select(e => e.originalWindowId)
            .ToList();

        Console.WriteLine(JsonSerializer.Serialize(unchangedWindows));
        Assert.Pass();
    }
}