using System.Globalization;
using CsvHelper;

namespace WindowExceptionsValidation.ValidationTests;

[TestFixture]
public class ExceptionsValidationTest
{
    [SetUp]
    public void Setup()
    {
        using var reader = new StreamReader(ExceptionsCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        _exceptions = csv.GetRecords<ExceptionRecord>().ToList();
    }

    private const string ExceptionsCsv = @"./csv/Christmas and New Years Window Exceptions - Exceptions.csv";

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
}