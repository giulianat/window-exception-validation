using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;

namespace WindowExceptionsValidation.ValidationTests;

[TestFixture]
public class ZonesValidationTest
{
    [SetUp]
    public void SetUp()
    {
        using var currentDataReader = new StreamReader(CurrentDataCSV);
        using var currentDataCsv = new CsvReader(currentDataReader, CultureInfo.InvariantCulture);

        currentDataCsv.Context.RegisterClassMap<CurrentDataRecordMap>();
        _currentData = currentDataCsv.GetRecords<CurrentDataRecord>().ToList();
        
        using var opsPlanReader = new StreamReader(OuputCSV);
        using var opsPlanCsv = new CsvReader(opsPlanReader, CultureInfo.InvariantCulture);

        _plan = opsPlanCsv.GetRecords<OpsPlanRecord>().ToList();
    }

    private List<CurrentDataRecord> _currentData;
    private Dictionary<string, string> _xrefs;
    private List<OpsPlanRecord> _plan;
    private const string CurrentDataCSV = @"./csv/Christmas and New Years Window Exceptions - Current Data.csv";
    private const string ZonesCSV = @"./csv/Christmas and New Years Window Exceptions - Zones.csv";

    private const string OuputCSV =
        @"./csv/Patrick's Copy of Xmas and NY Holiday Market_Zone Exception Tool - Output corrected.csv";

    [Test]
    public void ShouldHaveCorrectFulfillmentCenter()
    {
        using var reader = new StreamReader(ZonesCSV);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var actualRecords = csv.GetRecords<ZonesRecord>();

        foreach (var zone in actualRecords)
        {
            var refZone = _currentData.First(d => d.Zone.ZoneId == zone.referenceZoneId);
            
            Assert.That(zone.fulfillmentCenterId, Is.EqualTo(refZone.Window.FulfillmentCenterId));
        }
    }
    
    [Test]
    public void ShouldHaveDescriptionBasedOnMovedAndReferencedZoneIds()
    {
        var marketDictionary = Parser.GetMarketCodeToNameMap();
        var dayOfWeekDictionary = Parser.GetDayOfWeekMap();
        using var reader = new StreamReader(ZonesCSV);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var actualRecords = csv.GetRecords<ZonesRecord>();

        foreach (var zone in actualRecords)
        {
            var movedZone = _currentData.First(d => d.Zone.ZoneId == zone.movedZoneId);
            var refZone = _currentData.First(d => d.Zone.ZoneId == zone.referenceZoneId);
            var expectedDescription = $"{marketDictionary[zone.marketCode]} {dayOfWeekDictionary[movedZone.Window.StartDay].Abbreviation[..3]}/{dayOfWeekDictionary[refZone.Window.StartDay].Abbreviation[..3]} on {dayOfWeekDictionary[refZone.Window.StartDay].Day_Of_Week}";
            
            Assert.That(zone.description, Is.EqualTo(expectedDescription));
        }
    }
    
    [Test]
    public void ShouldHaveNameBasedOnMovedAndReferencedZoneIds()
    {
        var dayOfWeekDictionary = Parser.GetDayOfWeekMap();
        using var reader = new StreamReader(ZonesCSV);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var actualRecords = csv.GetRecords<ZonesRecord>();

        foreach (var zone in actualRecords)
        {
            var movedZone = _currentData.First(d => d.Zone.ZoneId == zone.movedZoneId);
            var refZone = _currentData.First(d => d.Zone.ZoneId == zone.referenceZoneId);
            
            var captureCollection = Regex.Match(movedZone.Zone.Name, "([A-Z]{3}: [^ ]+)(.*)").Groups.Values.ToList();
            var expectedName = $"{captureCollection[1]} / {dayOfWeekDictionary[refZone.Window.StartDay].Day_Of_Week.ToUpper()}{captureCollection[2]}";
            Assert.That(zone.name, Is.EqualTo(expectedName));
        }
    }
    
    [Test]
    public void ShouldHaveCorrectZoneInformation()
    {
        using var reader = new StreamReader(ZonesCSV);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var actualRecords = csv.GetRecords<ZonesRecord>();

        foreach (var zone in actualRecords)
        {
            var refZone = _currentData.First(d => d.Zone.ZoneId == zone.referenceZoneId);
            
            Assert.That(zone.timezone, Is.EqualTo(refZone.Zone.Timezone));
            Assert.That(zone.zonePickupAddressId, Is.EqualTo(refZone.Zone.ZonePickupAddressId));
            Assert.That(zone.expectedServiceTimeInMinutes, Is.EqualTo(refZone.Zone.ExpectedServiceTimeInMinutes));
            Assert.That(zone.isLineHaul, Is.EqualTo(refZone.Zone.IsLineHaul));
            Assert.That(zone.pickupTime, Is.EqualTo(refZone.Zone.PickupTime));
            Assert.That(zone.marketCode, Is.EqualTo(refZone.Zone.MarketCode));
        }
    }
}