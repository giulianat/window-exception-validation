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
    public void ShouldDispatchOnLatestDispatchDayForDoubleDeliveries()
    {
        var currentData = CsvParser.GetCurrentData().ToList();
        var combinedZones = CsvParser.GetZones().ToList();
        var doubleDeliveryWindows = _windows
            .GroupBy(w => w.zoneId, w => w)
            .Where(g => g.Count() > 2)
            .ToList();

        Assert.That(doubleDeliveryWindows.Count, Is.EqualTo(combinedZones.Count));
        
        Assert.Multiple(() =>
        {
            foreach (var windowGroup in doubleDeliveryWindows)
            {
                var oldZoneIds = windowGroup.Select(g => g.old_zone_id).ToList();
                var correspondingWindows = currentData
                    .Where(c => oldZoneIds.Contains(c.Zone.ZoneId));
                var windowWithLatestDispatch = correspondingWindows
                    .MaxBy(c => c.Window.StartDay)
                    !.Window;

                var windowsWithMaxDispatch = windowGroup.Count(w =>
                    w.dispatchDay == windowWithLatestDispatch.DispatchDay &&
                    w.dispatchTime == windowWithLatestDispatch.DispatchTime);
                Assert.That(windowsWithMaxDispatch, Is.EqualTo(windowGroup.Count()), $"Mismatch on {windowGroup.Key} expected dispatch on {windowWithLatestDispatch.DispatchDay} at {windowWithLatestDispatch.DispatchTime}");
            }
        });
    }
}