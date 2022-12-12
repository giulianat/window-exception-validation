using CsvHelper.Configuration;

namespace WindowExceptionsValidation;

public record EmpZoneRecord
{
    public string Nickname { get; set; }
    public string FcId { get; set; }
    public string ZoneName { get; set; }
    public string MarketCode { get; set; }
    public string ZoneId { get; set; }
    public string WindowId { get; set; }
    public int StartDay { get; set; }
    public string IsInPlan { get; set; }

    public string FC_Name { get; set; }
    public string Zone_Code { get; set; }
    public string City_Name { get; set; }
    public DayOfWeek Original_Delivery_Day { get; set; }
    public DayOfWeek Exception_Delivery_Day { get; set; }
    public int Original_Delivery_Date { get; set; }
    public int Exception_Delivery_Date { get; set; }
}

public sealed class EmpZoneRecordMap : ClassMap<EmpZoneRecord>
{
    public EmpZoneRecordMap()
    {
        Map(m => m.Nickname).Name("Address Nickname");
        Map(m => m.FcId).Name("FC Id");
        Map(m => m.ZoneName).Name("Zone Name");
        Map(m => m.MarketCode).Name("Market Code");
        Map(m => m.ZoneId).Name("Zone Id");
        Map(m => m.WindowId).Name("Window Id");
        Map(m => m.StartDay).Name("Start Day");
        Map(m => m.IsInPlan).Name("Is In Plan");
        Map(m => m.FC_Name).Name("FC_Name");
        Map(m => m.Zone_Code).Name("Zone_Code");
        Map(m => m.City_Name).Name("City_Name");
        Map(m => m.Original_Delivery_Day).Name("Original_Delivery_Day");
        Map(m => m.Exception_Delivery_Day).Name("Exception_Delivery_Day");
        Map(m => m.Original_Delivery_Date).Name("Original_Delivery_Date");
        Map(m => m.Exception_Delivery_Date).Name("Exception_Delivery_Date");
    }
}