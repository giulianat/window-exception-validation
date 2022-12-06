using CsvHelper.Configuration;

namespace WindowExceptionsValidation;

public record ZoneToDayMappingOutputRecord
{
    public string ZoneId { get; set; }
    public string ZoneName { get; set; }
    public string Holiday { get; set; }
    public string PackDate { get; set; }
    public string DeliveryDate { get; set; }
    public string CustoOpenDateTime { get; set; }
    public string CustoCloseDateTime { get; set; }
}

public sealed class ZoneToDayMappingOutputRecordMap : ClassMap<ZoneToDayMappingOutputRecord>
{
    public ZoneToDayMappingOutputRecordMap()
    {
        Map(m => m.ZoneId).Name("Zone ID");
        Map(m => m.ZoneName).Name("Zone Name");
        Map(m => m.Holiday).Name("Christmas or New Years");
        Map(m => m.PackDate).Name("Pack Date");
        Map(m => m.DeliveryDate).Name("Delivery Date");
        Map(m => m.CustoOpenDateTime).Name("Custo Open Date / Time");
        Map(m => m.CustoCloseDateTime).Name("Custo Close Date / Time");
    }
}