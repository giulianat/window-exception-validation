using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using CsvHelper;
using NUnit.Framework;

namespace WindowExceptionsValidation.ValidationTests;

[TestFixture]
public class EmpZoneValidationTest
{
    private List<EmpZoneRecord> _empZones = new();

    private static Dictionary<string, string> _cityNameExceptions = new()
    {
        { "SEA", "Seattle" },
        { "HAR", "Hartford" },
        { "NVA", "Northern Virginia" },
        { "EWR", "Newark" },
        { "MOC", "Montgomery County" },
        { "STL", "St. Louis" },
        { "BOS", "Boston" },
        { "SBA", "Santa Barbara" },
    };

    [SetUp]
    public void Setup()
    {
        using var reader = new StreamReader(EmpZoneCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<EmpZoneRecordMap>();
        
        _empZones = csv.GetRecords<EmpZoneRecord>()
            .Select(z =>
            {
                if (_cityNameExceptions.ContainsKey(z.MarketCode))
                {
                    z.Nickname = _cityNameExceptions[z.MarketCode];
                }

                return z;
            })
            .ToList();
    }
    
    private const string EmpZoneCsv = @"./csv/Xmas and NY Holiday Market_Zone Exception Tool - EMP Zone - GB.csv";
    private const string ImportZoneCsv = @"./csv/Xmas and NY Holiday Market_Zone Exception Tool - Import Zone.csv";

    [Test]
    public void ShouldPlanForAllEmpRecords()
    {
        using var reader = new StreamReader(EmpZoneCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        
        var empOpsPlan = csv.GetRecords<OpsPlanRecord>().ToList();

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
        using var reader = new StreamReader(EmpZoneCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<EmpZoneRecordMap>();
        
        var empOpsPlan = csv.GetRecords<OpsPlanRecord>().ToList();
        
        Assert.Multiple(() =>
        {
            foreach (var record in empOpsPlan)
            {
                var correspondingZone = _empZones.Find(z => z.MarketCode == record.Zone_Code);
                
                Assert.That(record.FC_Name, Is.EqualTo(correspondingZone?.FcId));
                Assert.That(record.Zone_Code, Is.EqualTo(correspondingZone?.MarketCode));
                Assert.That(record.City_Name, Is.EqualTo(correspondingZone?.Nickname), $"Mismatched market {correspondingZone?.MarketCode}");
            }
        });
    }

    [Test]
    public void ShouldContainOriginalDeliveryInformation()
    {
        using var reader = new StreamReader(EmpZoneCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<EmpZoneRecordMap>();
        
        var empOpsPlan = csv.GetRecords<OpsPlanRecord>().ToList();
        
        Assert.Multiple(() =>
        {
            foreach (var record in empOpsPlan)
            {
                var correspondingZone = _empZones.Find(z => z.MarketCode == record.Zone_Code);
                
                Assert.That(record.Original_Delivery_Day, Is.EqualTo((DayOfWeek)correspondingZone?.StartDay));
                Assert.That(record.Original_Delivery_Date, Is.EqualTo(correspondingZone?.StartDay));
            }
        });
    }

    [Test]
    public void ShouldSummarizeMismatchedInformation()
    {
        using var reader = new StreamReader(EmpZoneCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<EmpZoneRecordMap>();
        var empOpsPlan = csv.GetRecords<OpsPlanRecord>().ToList();
        var importZones = Parser.ParseImportZoneEmpRecords(ImportZoneCsv).Distinct().ToList();
        var plannedCities = empOpsPlan.Select(p => p.City_Name).ToList();
        var importZoneCities = importZones.Select(i => i.City_Name).ToList();
        var plansNotInImportZone = plannedCities.Except(importZoneCities);
        var importZonesNotInPlan = importZoneCities.Except(plannedCities);
        
        Console.WriteLine($"EMP Zone.csv that aren't in Import Zone.csv: {JsonSerializer.Serialize(plansNotInImportZone)}");
        Console.WriteLine($"Import Zone.csv that aren't in EMP Zone.csv: {JsonSerializer.Serialize(importZonesNotInPlan)}");

        Assert.Pass();
    }
    
    [Test]
    public void ShouldContainExceptionDeliveryInformation()
    {
        using var reader = new StreamReader(EmpZoneCsv);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<EmpZoneRecordMap>();
        
        var importZones = Parser.ParseImportZoneEmpRecords(ImportZoneCsv).Distinct().ToList();
        var empOpsPlansWithImportZones = csv.GetRecords<OpsPlanRecord>()
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
                Assert.That(record.Exception_Delivery_Day, Is.EqualTo(correspondingZone.Exception_Delivery_Day), correspondingZone.City_Name);
                Assert.That(record.Exception_Delivery_Date, Is.EqualTo(correspondingZone.Exception_Delivery_Date), correspondingZone.City_Name);
            }
        });
        
    }
}