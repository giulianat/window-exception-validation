using CsvHelper.Configuration;

namespace WindowExceptionsValidation;

public record MarketRecord
{
    public string Code { get; set; }
    public string Name { get; set; }
}

public sealed class MarketRecordMap : ClassMap<MarketRecord>
{
    public MarketRecordMap()
    {
        Map(m => m.Code).Name("Market Code").NameIndex(0);
        Map(m => m.Name).Name("Market Name").NameIndex(0);
    }
}