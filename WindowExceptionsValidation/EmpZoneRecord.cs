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
    }
}