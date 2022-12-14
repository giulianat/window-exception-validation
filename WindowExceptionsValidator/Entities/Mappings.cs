using CsvHelper.Configuration.Attributes;

namespace WindowExceptionsValidation;

public record MarketMapRecord
{
    [Name("Market Code")]
    public string Market_Code { get; set; }
    [Name("Market Name")]
    public string Market_Name { get; set; }
}

public record FulfillmentCenterMapRecord
{
    public string FC_ID { get; set; }
    public string FC_Name { get; set; }
}

public record DayOfWeekMapRecord
{
    public string Day_Of_Week { get; set; }
    public int Numeric_Day_Of_Week { get; set; }
    public string Abbreviation { get; set; }
}