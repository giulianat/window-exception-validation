namespace WindowExceptionsValidation;

public record WindowsRecord
{
    public string old_window_id { get; set; }
    public string old_zone_id { get; set; }
    public string name { get; set; }
    public string windowId { get; set; }
    public string zoneId { get; set; }
    public int? customizationStartDay { get; set; }
    public string customizationStartTime { get; set; }
    public int? customizationEndDay { get; set; }
    public string customizationEndTime { get; set; }
    public string dispatchDay { get; set; }
    public string dispatchTime { get; set; }
    public int? startDay { get; set; }
    public string startTime { get; set; }
    public int? endDay { get; set; }
    public string endTime { get; set; }
    public string fulfillmentCenterId { get; set; }
    public string deliveryPrice { get; set; }
    public string subtotalMin { get; set; }
    public string deliveryProvider { get; set; }
    public string orderBy { get; set; }
    public string isVisible { get; set; }
    public string smsNotifications { get; set; }
    public int packDateOffset { get; set; }
    public string messageToUser { get; set; }
}