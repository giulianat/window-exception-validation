namespace WindowExceptionsGenerator.Entities;

public class Window
{
    public Window() { }
    
    public Window(Window copy)
    {
        windowId = copy.windowId;
        zoneId = copy.zoneId;
        customizationStartDay = copy.customizationStartDay;
        customizationStartTime = copy.customizationStartTime;
        customizationEndDay = copy.customizationEndDay;
        customizationEndTime = copy.customizationEndTime;
        dispatchDay = copy.dispatchDay;
        dispatchTime = copy.dispatchTime;
        startDay = copy.startDay;
        startTime = copy.startTime;
        endDay = copy.endDay;
        endTime = copy.endTime;
        fulfillmentCenterId = copy.fulfillmentCenterId;
        deliveryPrice = copy.deliveryPrice;
        subtotalMin = copy.subtotalMin;
        deliveryProvider = copy.deliveryProvider;
        packDateOffset = copy.packDateOffset;
        carrierDaysInTransit = copy.carrierDaysInTransit;
        messageToUser = copy.messageToUser;
    }

    public string windowId { get; set; }

    public string zoneId { get; set; }
    public string customizationStartDay { get; set; }
    public string customizationStartTime { get; set; }
    public string customizationEndDay { get; set; }
    public string customizationEndTime { get; set; }
    public string dispatchDay { get; set; }
    public string dispatchTime { get; set; }
    public string startDay { get; set; }
    public string startTime { get; set; }
    public string endDay { get; set; }
    public string endTime { get; set; }
    public string fulfillmentCenterId { get; set; }
    public string deliveryPrice { get; set; }
    public string subtotalMin { get; set; }
    public string deliveryProvider { get; set; }
    public int orderBy => 1000;
    public string isVisible => "FALSE";
    public string smsNotifications => "TRUE";
    public string packDateOffset { get; set; }
    public string carrierDaysInTransit { get; set; }
    public string messageToUser { get; set; }
}