using System.Text.RegularExpressions;
using WindowExceptionsValidation.Entities;

namespace WindowExceptionsValidation.ValidationTests;

[TestFixture]
public class ZonesValidationTest
{
    [SetUp]
    public void SetUp()
    {
        _currentData = CsvParser.GetCurrentData().ToList();
    }

    private List<CurrentDataRecord> _currentData = new();

    [Test]
    public void ShouldHaveCorrectFulfillmentCenter()
    {
        var actualRecords = CsvParser.GetZones();

        foreach (var zone in actualRecords)
        {
            var refZone = _currentData.First(d => d.Zone.ZoneId == zone.referenceZoneId);

            Assert.That(zone.fulfillmentCenterId, Is.EqualTo(refZone.Window.FulfillmentCenterId));
        }
    }

    [Test]
    public void ShouldHaveDescriptionBasedOnMovedAndReferencedZoneIds()
    {
        var marketDictionary = CsvParser.GetMarketCodeToNameMap();
        var dayOfWeekDictionary = CsvParser.GetDayOfWeekMap();
        
        var actualRecords = CsvParser.GetZones();

        foreach (var zone in actualRecords)
        {
            var movedZone = _currentData.First(d => d.Zone.ZoneId == zone.movedZoneId);
            var refZone = _currentData.First(d => d.Zone.ZoneId == zone.referenceZoneId);
            var expectedDescription =
                $"{marketDictionary[zone.marketCode]} {dayOfWeekDictionary[movedZone.Window.StartDay].Abbreviation[..3]}/{dayOfWeekDictionary[refZone.Window.StartDay].Abbreviation[..3]} on {dayOfWeekDictionary[refZone.Window.StartDay].Day_Of_Week}";

            Assert.That(zone.description, Is.EqualTo(expectedDescription));
        }
    }

    [Test]
    public void ShouldHaveNameBasedOnMovedAndReferencedZoneIds()
    {
        var dayOfWeekDictionary = CsvParser.GetDayOfWeekMap();
        
        var actualRecords = CsvParser.GetZones();

        foreach (var zone in actualRecords)
        {
            var movedZone = _currentData.First(d => d.Zone.ZoneId == zone.movedZoneId);
            var refZone = _currentData.First(d => d.Zone.ZoneId == zone.referenceZoneId);

            var captureCollection = Regex.Match(movedZone.Zone.Name, "([A-Z]{3}: [^ ]+)(.*)").Groups.Values.ToList();
            var expectedName =
                $"{captureCollection[1]} / {dayOfWeekDictionary[refZone.Window.StartDay].Day_Of_Week.ToUpper()}{captureCollection[2]}";
            Assert.That(zone.name, Is.EqualTo(expectedName));
        }
    }

    [Test]
    public void ShouldHaveCorrectZoneInformation()
    {
        var actualRecords = CsvParser.GetZones();

        foreach (var zone in actualRecords)
        {
            var refZone = _currentData.First(d => d.Zone.ZoneId == zone.referenceZoneId);

            Assert.That(zone.timezone, Is.EqualTo(refZone.Zone.Timezone));
            Assert.That(zone.zonePickupAddressId, Is.EqualTo(refZone.Zone.ZonePickupAddressId));
            Assert.That(zone.expectedServiceTimeInMinutes, Is.EqualTo(refZone.Zone.ExpectedServiceTimeInMinutes));
            Assert.That(zone.isLineHaul, Is.EqualTo(refZone.Zone.IsLineHaul));
            Assert.That(zone.pickupTime,
                Is.EqualTo(TimeOnly.ParseExact(refZone.Zone.PickupTime, "H:mm:ss").ToString("HH:mm")));
            Assert.That(zone.marketCode, Is.EqualTo(refZone.Zone.MarketCode));
            Assert.That(zone.transitTime,
                Is.EqualTo(TimeOnly.ParseExact(refZone.Zone.TransitTime, "H:mm:ss").ToString("HH:mm")));
        }
    }
}