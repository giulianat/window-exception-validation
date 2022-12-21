using CsvHelper.Configuration.Attributes;

namespace WindowExceptionsGenerator.Entities;

public class LocalZone
{
    private string _pickupTime;
    private string _isLineHaul;
    [Name("zoneId"), Index(0)] public string ZoneId { get; set; }

    [Name("fulfillmentCenterId"), Index(1)] public string FulfillmentCenterId { get; set; }

    [Name("name"), Index(2)] public string Name { get; set; }

    [Name("timezone"), Index(3)] public string Timezone { get; set; }

    [Name("zonePickupAddressId"), Index(4)] public string ZonePickupAddressId { get; set; }

    [Name("expectedServiceTimeInMinutes"), Index(5)] public string ExpectedServiceTimeInMinutes { get; set; }

    [Name("isLineHaul"), Index(6)]
    public string IsLineHaul
    {
        get => _isLineHaul.ToUpper();
        set => _isLineHaul = value;
    }

    [Name("pickupTime"), Index(7)]
    public string PickupTime
    {
        get => _pickupTime;
        set => _pickupTime = TimeOnly.Parse(value).ToString("HH:mm");
    }

    [Name("marketCode"), Index(8)] public string MarketCode { get; set; }
}

public class LineHaulZone : LocalZone
{
    private string _transitTime;

    [Name("transitTime"), Index(9)]
    public string TransitTime
    {
        get => _transitTime;
        set => _transitTime = TimeOnly.Parse(value).ToString("HH:mm");
    }
}