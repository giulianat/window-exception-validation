using System.Text.Json;

namespace WindowExceptionsValidation.ValidationTests;

[TestFixture]
public class EmpZoneValidationTest
{
    [SetUp]
    public void Setup()
    {
        _empZones = CsvParser.GetEmpZones()
            .Select(z =>
            {
                if (CityNameExceptions.ContainsKey(z.MarketCode)) z.Nickname = CityNameExceptions[z.MarketCode];

                return z;
            })
            .ToList();
    }

    private List<EmpZoneRecord> _empZones = new();

    private static readonly Dictionary<string, string> CityNameExceptions = new()
    {
        { "SEA", "Seattle" },
        { "HAR", "Hartford" },
        { "NVA", "Northern Virginia" },
        { "EWR", "Newark" },
        { "MOC", "Montgomery County" },
        { "STL", "St. Louis" },
        { "BOS", "Boston" },
        { "SBA", "Santa Barbara" }
    };

    private const string EmpZoneCsv = @"./csv/Xmas and NY Holiday Market_Zone Exception Tool - EMP Zone - GB.csv";
    private const string ImportZoneCsv = @"./csv/Xmas and NY Holiday Market_Zone Exception Tool - Import Zone.csv";

    [Test]
    public void ShouldPlanForAllEmpRecords()
    {
        var empOpsPlan = CsvParser.GetEmpZones().ToList();

        Assert.Multiple(() =>
        {
            Assert.That(empOpsPlan, Has.Count.EqualTo(_empZones.Count));

            foreach (var record in empOpsPlan)
            {
                var correspondingZone = _empZones.Find(z => z.MarketCode == record.Zone_Code);

                Assert.NotNull(correspondingZone, $"Could not find {record.City_Name}");
            }
        });
    }

    [Test]
    public void ShouldHaveCorrectLocationInformationForEachMarket()
    {
        var empOpsPlan = CsvParser.GetEmpZones().ToList();

        Assert.Multiple(() =>
        {
            foreach (var record in empOpsPlan)
            {
                var correspondingZone = _empZones.Find(z => z.MarketCode == record.Zone_Code);

                Assert.That(record.FC_Name, Is.EqualTo(correspondingZone?.FcId));
                Assert.That(record.Zone_Code, Is.EqualTo(correspondingZone?.MarketCode));
                Assert.That(record.City_Name, Is.EqualTo(correspondingZone?.Nickname),
                    $"Mismatched market {correspondingZone?.MarketCode}");
            }
        });
    }

    [Test]
    public void ShouldContainOriginalDeliveryInformation()
    {
        var empOpsPlan = CsvParser.GetEmpZones().ToList();

        Assert.Multiple(() =>
        {
            foreach (var record in empOpsPlan)
            {
                var correspondingZone = _empZones.Find(z => z.MarketCode == record.Zone_Code);

                Assert.That(record.Original_Delivery_Day, Is.EqualTo((DayOfWeek)correspondingZone?.StartDay));
                Assert.That(record.Original_Delivery_Date, Is.EqualTo(correspondingZone.StartDay));
            }
        });
    }

    [Test]
    public void ShouldSummarizeMismatchedInformation()
    {
        var empOpsPlan = CsvParser.GetEmpZones().ToList();
        var importZones = CsvParser.ParseImportZoneEmpRecords(ImportZoneCsv).Distinct().ToList();
        var plannedCities = empOpsPlan.Select(p => p.City_Name).ToList();
        var importZoneCities = importZones.Select(i => i.City_Name).ToList();
        var plansNotInImportZone = plannedCities.Except(importZoneCities);
        var importZonesNotInPlan = importZoneCities.Except(plannedCities);

        Console.WriteLine(
            $"EMP Zone.csv that aren't in Import Zone.csv: {JsonSerializer.Serialize(plansNotInImportZone)}");
        Console.WriteLine(
            $"Import Zone.csv that aren't in EMP Zone.csv: {JsonSerializer.Serialize(importZonesNotInPlan)}");

        Assert.Pass();
    }

    [Test]
    public void ShouldContainExceptionDeliveryInformation()
    {
        var importZones = CsvParser.ParseImportZoneEmpRecords(ImportZoneCsv).Distinct().ToList();
        var empOpsPlansWithImportZones = CsvParser.GetEmpZones()
            .Where(r => importZones.Select(z => z.City_Name).Contains(r.City_Name))
            .Where(r => r.City_Name != "Los Angeles") //TODO: Remove
            .ToList();

        Assert.Multiple(() =>
        {
            foreach (var record in empOpsPlansWithImportZones)
            {
                var correspondingZones = importZones.Where(i => i.City_Name == record.City_Name).ToList();
                Assert.That(correspondingZones, Has.Count.EqualTo(1));

                var correspondingZone = correspondingZones.First();
                Assert.That(record.Exception_Delivery_Day, Is.EqualTo(correspondingZone.Exception_Delivery_Day),
                    correspondingZone.City_Name);
                Assert.That(record.Exception_Delivery_Date, Is.EqualTo(correspondingZone.Exception_Delivery_Date),
                    correspondingZone.City_Name);
            }
        });
    }
}