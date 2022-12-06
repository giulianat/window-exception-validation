namespace WindowExceptionsValidation;

public record ZonesRecord
{
    public string description { get; set; }
    public string movedZoneId { get; set; }
    public string referenceZoneId { get; set; }
    public string zoneId { get; set; }
    public string fulfillmentCenterId { get; set; }
    public string name { get; set; }
    public string timezone { get; set; }
    public string zonePickupAddressId { get; set; }
    public string expectedServiceTimeInMinutes { get; set; }
    public string isLineHaul { get; set; }
    public string pickupTime { get; set; }
    public string marketCode { get; set; }
}