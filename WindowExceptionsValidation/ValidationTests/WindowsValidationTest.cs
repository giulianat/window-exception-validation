using System.Globalization;
using CsvHelper;

namespace WindowExceptionsValidation.ValidationTests;

[TestFixture]
public class WindowsValidationTest
{
    [SetUp]
    public void Setup()
    {
        using var reader = new StreamReader(WindowsCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        _windows = csv.GetRecords<WindowsRecord>().ToList();
    }

    private const string WindowsCsv = @"./csv/Christmas and New Years Window Exceptions - Windows.csv";

    private static readonly DateOnly[] Holidays =
    {
        new(2022, 12, 24),
        new(2022, 12, 25),
        new(2022, 12, 26),
        new(2023, 1, 1),
        new(2023, 1, 2)
    };

    private List<WindowsRecord> _windows = new();

    [Test]
    public void ShouldHaveStartAndEndDays()
    {
        var hasEmptyStartOrEndDays = _windows
            .Any(w => w.startDay == null || w.endDay == null);

        Assert.That(hasEmptyStartOrEndDays, Is.False);
    }

    [Test]
    public void ShouldNotStartOnHoliday()
    {
        var numberOfStartDaysOnAHoliday = _windows
            .Select(w => (DayOfWeek)w.startDay)
            .Count(dow => Holidays.Select(h => h.DayOfWeek).Contains(dow));

        Assert.That(numberOfStartDaysOnAHoliday, Is.Zero);
    }

    [Test]
    public void ShouldNotEndOnHoliday()
    {
        var numberOfStartDaysOnAHoliday = _windows
            .Select(w => (DayOfWeek)w.endDay)
            .Count(dow => Holidays.Select(h => h.DayOfWeek).Contains(dow));

        Assert.That(numberOfStartDaysOnAHoliday, Is.Zero);
    }

    [Test]
    public void ShouldHaveAscendingDates()
    {
        Dictionary<int, DateOnly> weekMap = new()
        {
            { 0, new DateOnly(2022, 12, 25) },
            { 1, new DateOnly(2022, 12, 26) },
            { 2, new DateOnly(2022, 12, 27) },
            { 3, new DateOnly(2022, 12, 28) },
            { 4, new DateOnly(2022, 12, 29) },
            { 5, new DateOnly(2022, 12, 30) },
            { 6, new DateOnly(2022, 12, 31) }
        };
        
        Assert.Multiple(() =>
        {
            foreach (var window in _windows)
            {
                var custoEnd = weekMap[window.customizationEndDay!.Value]
                    .ToDateTime(TimeOnly.Parse(window.customizationEndTime));
                var dispatch = weekMap[window.dispatchDay!.Value]
                    .ToDateTime(TimeOnly.Parse(window.dispatchTime));
                var deliveryStart = weekMap[window.startDay!.Value]
                    .ToDateTime(TimeOnly.Parse(window.startTime));
                var deliveryEnd = weekMap[window.endDay!.Value]
                    .ToDateTime(TimeOnly.Parse(window.endTime));
                
                Assert.That((dispatch - custoEnd).TotalHours, Is.LessThanOrEqualTo(6), $"Dispatch Comparison for {window.name}");
                Assert.That(deliveryStart, Is.LessThan(deliveryEnd), $"Delivery Comparison for {window.name}");
            }
        });
    }
}