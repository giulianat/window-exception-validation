using System.Globalization;
using CsvHelper;

namespace WindowExceptionsValidation.ValidationTests;

[TestFixture]
public class ZoneToDayMappingOutputValidationTest
{
    [SetUp]
    public void Setup()
    {
        using var zReader = new StreamReader(ZonesCsv);
        using var zCsv = new CsvReader(zReader, CultureInfo.InvariantCulture);
        _zones = zCsv.GetRecords<ZonesRecord>().ToList();

        using var wReader = new StreamReader(WindowsCsv);
        using var wCsv = new CsvReader(wReader, CultureInfo.InvariantCulture);
        _windows = wCsv.GetRecords<WindowsRecord>().ToList();
    }

    private const string WindowsCsv = @"./csv/Christmas and New Years Window Exceptions - Windows.csv";
    private const string ZonesCsv = @"./csv/Christmas and New Years Window Exceptions - Zones.csv";

    private const string ZoneToDayCsv =
        @"./csv/Christmas and New Years Window Exceptions - Zone to Day Mapping Output.csv";

    private static readonly string[] Holidays = { "Christmas", "New Years" };

    private static readonly Dictionary<int, DateOnly> ChristmasWeekMap = new()
    {
        { 0, new DateOnly(2022, 12, 25) },
        { 1, new DateOnly(2022, 12, 26) },
        { 2, new DateOnly(2022, 12, 27) },
        { 3, new DateOnly(2022, 12, 28) },
        { 4, new DateOnly(2022, 12, 29) },
        { 5, new DateOnly(2022, 12, 30) },
        { 6, new DateOnly(2022, 12, 31) }
    };

    private static readonly Dictionary<int, DateOnly> NewYearsWeekMap = new()
    {
        { 0, new DateOnly(2023, 1, 1) },
        { 1, new DateOnly(2023, 1, 2) },
        { 2, new DateOnly(2023, 1, 3) },
        { 3, new DateOnly(2023, 1, 4) },
        { 4, new DateOnly(2023, 1, 5) },
        { 5, new DateOnly(2023, 1, 6) },
        { 6, new DateOnly(2023, 1, 7) }
    };

    private IEnumerable<ZonesRecord> _zones = new List<ZonesRecord>();
    private IEnumerable<WindowsRecord> _windows = new List<WindowsRecord>();

    [Test]
    public void ShouldContainExactlyTwoOfEachZone()
    {
        using var reader = new StreamReader(ZoneToDayCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<ZoneToDayMappingOutputRecordMap>();
        var expectedRecords = csv.GetRecords<ZoneToDayMappingOutputRecord>().ToList();

        foreach (var zonesRecord in _zones)
        {
            var correspondingZones = expectedRecords
                .Where(r => r.ZoneId == zonesRecord.zoneId)
                .ToList();

            Assert.That(correspondingZones.Count, Is.EqualTo(2));
            Assert.That(correspondingZones[0].ZoneName, Is.EqualTo(zonesRecord.name));
            Assert.That(correspondingZones[1].ZoneName, Is.EqualTo(zonesRecord.name));
        }
    }

    [Test]
    public void ShouldContainAnEntryForEachHoliday()
    {
        using var reader = new StreamReader(ZoneToDayCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<ZoneToDayMappingOutputRecordMap>();
        var expectedRecords = csv.GetRecords<ZoneToDayMappingOutputRecord>().ToList();

        foreach (var zonesRecord in _zones)
        {
            var correspondingZones = expectedRecords
                .Where(r => r.ZoneId == zonesRecord.zoneId)
                .ToList();

            Assert.That(correspondingZones.Count, Is.EqualTo(2));
            Assert.That(correspondingZones[0].Holiday, Is.EqualTo("Christmas"));
            Assert.That(correspondingZones[1].Holiday, Is.EqualTo("New Years"));
        }
    }

    [Test]
    public void ShouldHavePackDate()
    {
        using var reader = new StreamReader(ZoneToDayCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<ZoneToDayMappingOutputRecordMap>();
        var expectedRecords = csv.GetRecords<ZoneToDayMappingOutputRecord>().ToList();

        foreach (var zonesRecord in _zones)
        {
            var correspondingZones = expectedRecords
                .Where(r => r.ZoneId == zonesRecord.zoneId)
                .ToList();
            var correspondingWindow = _windows.First(w => w.zoneId == zonesRecord.zoneId);
            var packDayOfWeek = (correspondingWindow.startDay - correspondingWindow.packDateOffset)!.Value;
            var christmasPackDate = ChristmasWeekMap[packDayOfWeek].ToString();
            var newYearsPackDate = NewYearsWeekMap[packDayOfWeek].ToString();

            Assert.That(correspondingZones.Count, Is.EqualTo(2));
            Assert.That(correspondingZones[0].Holiday, Is.EqualTo("Christmas"));
            Assert.That(correspondingZones[0].PackDate, Is.EqualTo(christmasPackDate));
            Assert.That(correspondingZones[1].Holiday, Is.EqualTo("New Years"));
            Assert.That(correspondingZones[1].PackDate, Is.EqualTo(newYearsPackDate));
        }
    }

    [Test]
    public void ShouldHaveDeliveryDate()
    {
        using var reader = new StreamReader(ZoneToDayCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<ZoneToDayMappingOutputRecordMap>();
        var expectedRecords = csv.GetRecords<ZoneToDayMappingOutputRecord>().ToList();

        foreach (var zonesRecord in _zones)
        {
            var correspondingZones = expectedRecords
                .Where(r => r.ZoneId == zonesRecord.zoneId)
                .ToList();
            var correspondingWindow = _windows.First(w => w.zoneId == zonesRecord.zoneId);
            var deliveryDayOfWeek = correspondingWindow.endDay!.Value;
            var christmasDeliveryDate = ChristmasWeekMap[deliveryDayOfWeek].ToString();
            var newYearsDeliveryDate = NewYearsWeekMap[deliveryDayOfWeek].ToString();

            Assert.That(correspondingZones.Count, Is.EqualTo(2));
            Assert.That(correspondingZones[0].Holiday, Is.EqualTo("Christmas"));
            Assert.That(correspondingZones[0].DeliveryDate, Is.EqualTo(christmasDeliveryDate));
            Assert.That(correspondingZones[1].Holiday, Is.EqualTo("New Years"));
            Assert.That(correspondingZones[1].DeliveryDate, Is.EqualTo(newYearsDeliveryDate));
        }
    }

    [Test]
    public void ShouldHaveCustoOpenDateTime()
    {
        using var reader = new StreamReader(ZoneToDayCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<ZoneToDayMappingOutputRecordMap>();
        var expectedRecords = csv.GetRecords<ZoneToDayMappingOutputRecord>().ToList();

        foreach (var zonesRecord in _zones)
        {
            var correspondingZones = expectedRecords
                .Where(r => r.ZoneId == zonesRecord.zoneId)
                .ToList();
            var correspondingWindow = _windows.First(w => w.zoneId == zonesRecord.zoneId);
            var custoOpenDayOfWeek = correspondingWindow.customizationStartDay!.Value;
            var custoOpenTime = correspondingWindow.customizationStartTime;
            var christmasCustoOpenDate = ChristmasWeekMap[custoOpenDayOfWeek].AddDays(-7).ToString();
            var newYearsCustoOpenDate = NewYearsWeekMap[custoOpenDayOfWeek].AddDays(-7).ToString();
            var christmasCustoOpenDateTime = $"{christmasCustoOpenDate} {custoOpenTime}";
            var newYearsCustoOpenDateTime = $"{newYearsCustoOpenDate} {custoOpenTime}";

            Assert.That(correspondingZones.Count, Is.EqualTo(2));
            Assert.That(correspondingZones[0].Holiday, Is.EqualTo("Christmas"));
            Assert.That(correspondingZones[0].CustoOpenDateTime, Is.EqualTo(christmasCustoOpenDateTime));
            Assert.That(correspondingZones[1].Holiday, Is.EqualTo("New Years"));
            Assert.That(correspondingZones[1].CustoOpenDateTime, Is.EqualTo(newYearsCustoOpenDateTime));
        }
    }

    [Test]
    public void ShouldHaveCustoClosedDateTime()
    {
        using var reader = new StreamReader(ZoneToDayCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<ZoneToDayMappingOutputRecordMap>();
        var expectedRecords = csv.GetRecords<ZoneToDayMappingOutputRecord>().ToList();

        foreach (var zonesRecord in _zones)
        {
            var correspondingZones = expectedRecords
                .Where(r => r.ZoneId == zonesRecord.zoneId)
                .ToList();
            var correspondingWindow = _windows.First(w => w.zoneId == zonesRecord.zoneId);
            var custoClosedDayOfWeek = correspondingWindow.customizationEndDay!.Value;
            var custoClosedTime = correspondingWindow.customizationEndTime;
            var christmasCustoClosedDate = ChristmasWeekMap[custoClosedDayOfWeek].AddDays(-7).ToString();
            var newYearsCustoClosedDate = NewYearsWeekMap[custoClosedDayOfWeek].AddDays(-7).ToString();
            var christmasCustoClosedDateTime = $"{christmasCustoClosedDate} {custoClosedTime}";
            var newYearsCustoClosedDateTime = $"{newYearsCustoClosedDate} {custoClosedTime}";

            Assert.That(correspondingZones.Count, Is.EqualTo(2));
            Assert.That(correspondingZones[0].Holiday, Is.EqualTo("Christmas"));
            Assert.That(correspondingZones[0].CustoOpenDateTime, Is.EqualTo(christmasCustoClosedDateTime));
            Assert.That(correspondingZones[1].Holiday, Is.EqualTo("New Years"));
            Assert.That(correspondingZones[1].CustoOpenDateTime, Is.EqualTo(newYearsCustoClosedDateTime));
        }
    }

    [Test]
    public void ShouldGenerateExpectedCsv()
    {
        List<ZoneToDayMappingOutputRecord> records = new();

        foreach (var zonesRecord in _zones)
        {
            var correspondingWindow = _windows.First(w => w.zoneId == zonesRecord.zoneId);
            var packDayOfWeek = (correspondingWindow.startDay - correspondingWindow.packDateOffset)!.Value;
            var deliveryDayOfWeek = correspondingWindow.endDay!.Value;
            var custoOpenDayOfWeek = correspondingWindow.customizationStartDay!.Value;
            var custoOpenTime = correspondingWindow.customizationStartTime;
            var christmasCustoOpenDate = ChristmasWeekMap[custoOpenDayOfWeek].AddDays(-7).ToString();
            var newYearsCustoOpenDate = NewYearsWeekMap[custoOpenDayOfWeek].AddDays(-7).ToString();
            var custoClosedDayOfWeek = correspondingWindow.customizationEndDay!.Value;
            var custoClosedTime = correspondingWindow.customizationEndTime;
            var christmasCustoClosedDate = ChristmasWeekMap[custoClosedDayOfWeek].AddDays(-7).ToString();
            var newYearsCustoClosedDate = NewYearsWeekMap[custoClosedDayOfWeek].AddDays(-7).ToString();

            var christmas = new ZoneToDayMappingOutputRecord
            {
                ZoneId = zonesRecord.zoneId,
                ZoneName = zonesRecord.name,
                Holiday = "Christmas",
                PackDate = ChristmasWeekMap[packDayOfWeek].ToString(),
                DeliveryDate = ChristmasWeekMap[deliveryDayOfWeek].ToString(),
                CustoOpenDateTime = $"{christmasCustoOpenDate} {custoOpenTime}",
                CustoCloseDateTime = $"{christmasCustoClosedDate} {custoClosedTime}"
            };
            var newYears = new ZoneToDayMappingOutputRecord
            {
                ZoneId = zonesRecord.zoneId,
                ZoneName = zonesRecord.name,
                Holiday = "New Years",
                PackDate = NewYearsWeekMap[packDayOfWeek].ToString(),
                DeliveryDate = NewYearsWeekMap[deliveryDayOfWeek].ToString(),
                CustoOpenDateTime = $"{newYearsCustoOpenDate} {custoOpenTime}",
                CustoCloseDateTime = $"{newYearsCustoClosedDate} {custoClosedTime}"
            };

            records.Add(christmas);
            records.Add(newYears);
        }

        const string path = @"./csv/generated/Zone to Day Mapping Output.csv";
        using var writer = File.Exists(path) ? File.AppendText(path) : File.CreateText(path);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(records);
    }
}