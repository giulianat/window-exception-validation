namespace WindowExceptionsValidation.Entities;

[TestFixture]
public class CurrentDataRecordMapTest
{
    [SetUp]
    public void SetUp()
    {
        _currentData = CsvParser.GetCurrentData().ToList();
    }

    private List<CurrentDataRecord> _currentData = new();

    [Test]
    public void ShouldSafelyIgnoreDuplicateColumns()
    {
        var firstRow = _currentData.First();
        
        Assert.Multiple(() =>
        {
            Assert.That(firstRow.QuickLookup, Is.EqualTo("CHI|1"));
            Assert.That(firstRow.DeliveryProviderLookup, Is.EqualTo("CHI|1|Bringg"));
            Assert.That(firstRow.Window.WindowId, Is.EqualTo("863f927e-b0da-4a4c-b2df-43bd6327f392"));
            Assert.That(firstRow.Window.FulfillmentCenterId, Is.EqualTo("c059b389-ff65-4283-8edb-06ffc96193af"));
            Assert.That(firstRow.Window.StartDay, Is.EqualTo(1));
            Assert.That(firstRow.Window.StartTime, Is.EqualTo("5:00:00"));
            Assert.That(firstRow.Window.EndDay, Is.EqualTo("1"));
            Assert.That(firstRow.Window.EndTime, Is.EqualTo("21:00:00"));
            Assert.That(firstRow.Window.DispatchDay, Is.EqualTo(6));
            Assert.That(firstRow.Window.DispatchTime, Is.EqualTo("18:00:00"));
            Assert.That(firstRow.Window.CustomizationStartDay, Is.EqualTo("4"));
            Assert.That(firstRow.Window.CustomizationStartTime, Is.EqualTo("15:00:00"));
            Assert.That(firstRow.Window.CustomizationEndDay, Is.EqualTo("6"));
            Assert.That(firstRow.Window.CustomizationEndTime, Is.EqualTo("12:00:00"));
            Assert.That(firstRow.Window.DeliveryPrice, Is.EqualTo("5.99"));
            Assert.That(firstRow.Window.OrderBy, Is.EqualTo("0"));
            Assert.That(firstRow.Window.IsVisible, Is.EqualTo("TRUE"));
            Assert.That(firstRow.Window.MessageToUser,
                Is.EqualTo("Hooray!  We deliver to your neighborhood on Monday afternoon/evening."));
            Assert.That(firstRow.Window.DBPRouteId, Is.EqualTo("155"));
            Assert.That(firstRow.Window.DBPWindowId, Is.EqualTo("61"));
            Assert.That(firstRow.Window.Created, Is.EqualTo("2018-05-31 17:40:53.713 -0500"));
            Assert.That(firstRow.Window.Archived, Is.EqualTo("FALSE"));
            Assert.That(firstRow.Window.ArchivedDate, Is.EqualTo(""));
            Assert.That(firstRow.Window.SmsNotifications, Is.EqualTo("TRUE"));
            Assert.That(firstRow.Window.Updated, Is.EqualTo("2022-11-04 11:45:22.770 -0500"));
            Assert.That(firstRow.Window.PackDateOffset, Is.EqualTo("1"));
            Assert.That(firstRow.Window.SubtotalMin, Is.EqualTo(""));
            Assert.That(firstRow.Window.DeliveryProvider, Is.EqualTo("Bringg"));
            Assert.That(firstRow.Window.CarrierDaysinTransit, Is.EqualTo(""));
            Assert.That(firstRow.Zone.ZoneId, Is.EqualTo("52fa935c-7062-4d05-8b7e-8ff5614e539c"));
            Assert.That(firstRow.Zone.FulfillmentCenterId, Is.EqualTo("c059b389-ff65-4283-8edb-06ffc96193af"));
            Assert.That(firstRow.Zone.Name, Is.EqualTo("CHI: MONDAY PM"));
            Assert.That(firstRow.Zone.ExpectedServiceTimeInMinutes, Is.EqualTo("5"));
            Assert.That(firstRow.Zone.Archived, Is.EqualTo("FALSE"));
            Assert.That(firstRow.Zone.ZonePickupAddressId, Is.EqualTo("cf716172-2162-11e8-b467-0ed5f89f718b"));
            Assert.That(firstRow.Zone.Timezone, Is.EqualTo("America/Chicago"));
            Assert.That(firstRow.Zone.Created, Is.EqualTo("2018-08-07 14:17:21.817 -0500"));
            Assert.That(firstRow.Zone.Updated, Is.EqualTo("2021-08-02 11:25:03.657 -0500"));
            Assert.That(firstRow.Zone.IsLineHaul, Is.EqualTo(false));
            Assert.That(firstRow.Zone.PickupTime, Is.EqualTo("8:30:00"));
            Assert.That(firstRow.Zone.TransitTime, Is.EqualTo("0:00:00"));
            Assert.That(firstRow.Zone.MarketCode, Is.EqualTo("CHI"));
            Assert.That(firstRow.ToBeArchived, Is.EqualTo("FALSE"));
        });
    }
}