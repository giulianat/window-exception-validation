using CsvHelper.Configuration;

namespace WindowExceptionsValidation;

public record CurrentDataRecord
{
    public string QuickLookup { get; set; }
    public string DeliveryProviderLookup { get; set; }
    public Window Window { get; set; }
    public Zone Zone { get; set; }
    public string ToBeArchived { get; set; }
}

public record Zone
{
    public string ZoneId { get; set; }
    public string FulfillmentCenterId { get; set; }
    public string Name { get; set; }
    public string ExpectedServiceTimeInMinutes { get; set; }
    public string Archived { get; set; }
    public string ZonePickupAddressId { get; set; }
    public string Timezone { get; set; }
    public string Created { get; set; }
    public string Updated { get; set; }
    public string IsLineHaul { get; set; }
    public string PickupTime { get; set; }
    public string TransitTime { get; set; }
    public string MarketCode { get; set; }
}

public record Window
{
    public string WindowId { get; set; }
    public string FulfillmentCenterId { get; set; }
    public string StartDay { get; set; }
    public string StartTime { get; set; }
    public string EndDay { get; set; }
    public string EndTime { get; set; }
    public string DispatchDay { get; set; }
    public string DispatchTime { get; set; }
    public string CustomizationStartDay { get; set; }
    public string CustomizationStartTime { get; set; }
    public string CustomizationEndDay { get; set; }
    public string CustomizationEndTime { get; set; }
    public string DeliveryPrice { get; set; }
    public string OrderBy { get; set; }
    public string IsVisible { get; set; }
    public string MessageToUser { get; set; }
    public string DBPRouteId { get; set; }
    public string DBPWindowId { get; set; }
    public string Created { get; set; }
    public string Archived { get; set; }
    public string ArchivedDate { get; set; }
    public string SmsNotifications { get; set; }
    public string Updated { get; set; }
    public string PackDateOffset { get; set; }
    public string SubtotalMin { get; set; }
    public string DeliveryProvider { get; set; }
    public string CarrierDaysinTransit { get; set; }
}

public sealed class CurrentDataRecordMap : ClassMap<CurrentDataRecord>
{
    public CurrentDataRecordMap()
    {
        Map(m => m.QuickLookup).Name("Quick Lookup").NameIndex(0);
        Map(m => m.DeliveryProviderLookup).Name("Delivery Provider Lookup").NameIndex(0);
        Map(m => m.Window.WindowId).Name("Window Id").NameIndex(1);
        Map(m => m.Window.FulfillmentCenterId).Name("Fulfillment Center Id").NameIndex(1);
        Map(m => m.Window.StartDay).Name("Start Day").NameIndex(0);
        Map(m => m.Window.StartTime).Name("Start Time").NameIndex(0);
        Map(m => m.Window.EndDay).Name("End Day").NameIndex(0);
        Map(m => m.Window.EndTime).Name("End Time").NameIndex(0);
        Map(m => m.Window.DispatchDay).Name("Dispatch Day").NameIndex(0);
        Map(m => m.Window.DispatchTime).Name("Dispatch Time").NameIndex(0);
        Map(m => m.Window.CustomizationStartDay).Name("Customization Start Day").NameIndex(0);
        Map(m => m.Window.CustomizationStartTime).Name("Customization Start Time").NameIndex(0);
        Map(m => m.Window.CustomizationEndDay).Name("Customization End Day").NameIndex(0);
        Map(m => m.Window.CustomizationEndTime).Name("Customization End Time").NameIndex(0);
        Map(m => m.Window.DeliveryPrice).Name("Delivery Price").NameIndex(0);
        Map(m => m.Window.OrderBy).Name("Order By").NameIndex(0);
        Map(m => m.Window.IsVisible).Name("Is Visible").NameIndex(0);
        Map(m => m.Window.MessageToUser).Name("Message To User").NameIndex(0);
        Map(m => m.Window.DBPRouteId).Name("DBP Route Id").NameIndex(0);
        Map(m => m.Window.DBPWindowId).Name("DBP Window Id").NameIndex(0);
        Map(m => m.Window.Created).Name("Created").NameIndex(0);
        Map(m => m.Window.Archived).Name("Archived").NameIndex(0);
        Map(m => m.Window.ArchivedDate).Name("Archived Date").NameIndex(0);
        Map(m => m.Window.SmsNotifications).Name("Sms Notifications").NameIndex(0);
        Map(m => m.Window.Updated).Name("Updated").NameIndex(0);
        Map(m => m.Window.PackDateOffset).Name("Pack Date Offset").NameIndex(0);
        Map(m => m.Window.SubtotalMin).Name("Subtotal Min").NameIndex(0);
        Map(m => m.Window.DeliveryProvider).Name("Delivery Provider").NameIndex(1);
        Map(m => m.Window.CarrierDaysinTransit).Name("Carrier Days in Transit").NameIndex(0);
        Map(m => m.Zone.ZoneId).Name("Zone Id").NameIndex(1);
        Map(m => m.Zone.FulfillmentCenterId).Name("Fulfillment Center Id").NameIndex(2);
        Map(m => m.Zone.Name).Name("Name").NameIndex(1);
        Map(m => m.Zone.ExpectedServiceTimeInMinutes).Name("Expected Service Time In Minutes").NameIndex(0);
        Map(m => m.Zone.Archived).Name("Archived").NameIndex(1);
        Map(m => m.Zone.ZonePickupAddressId).Name("Zone Pickup Address Id").NameIndex(0);
        Map(m => m.Zone.Timezone).Name("Timezone").NameIndex(0);
        Map(m => m.Zone.Created).Name("Created").NameIndex(1);
        Map(m => m.Zone.Updated).Name("Updated").NameIndex(1);
        Map(m => m.Zone.IsLineHaul).Name("Is Line Haul").NameIndex(0);
        Map(m => m.Zone.PickupTime).Name("Pickup Time").NameIndex(0);
        Map(m => m.Zone.TransitTime).Name("Transit Time").NameIndex(0);
        Map(m => m.Zone.MarketCode).Name("Market Code").NameIndex(1);
        Map(m => m.ToBeArchived).Name("To Be Archived").NameIndex(0);
    }
}