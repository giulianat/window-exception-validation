using CsvHelper.Configuration;

namespace WindowExceptionsValidation;

public struct OpsPlanRecord : IEquatable<OpsPlanRecord>
{
    public string old_window_id { get; set; }
    public string FC_Name { get; set; }
    public string Zone_Code { get; set; }
    public string City_Name { get; set; }
    public DayOfWeek Original_Delivery_Day { get; set; }
    public DayOfWeek Exception_Delivery_Day { get; set; }

    public int Original_Delivery_Date { get; set; }

    public int Exception_Delivery_Date { get; set; }

    public bool Is_Employee_Zone { get; set; }

    public bool Equals(OpsPlanRecord other)
    {
        return City_Name == other.City_Name
               && Original_Delivery_Day == other.Original_Delivery_Day
               && Exception_Delivery_Day == other.Exception_Delivery_Day
               && Original_Delivery_Date == other.Original_Delivery_Date
               && Exception_Delivery_Date == other.Exception_Delivery_Date
               && Is_Employee_Zone == other.Is_Employee_Zone;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(City_Name, (int)Original_Delivery_Day, Original_Delivery_Date,
            (int)Exception_Delivery_Day, Exception_Delivery_Date);
    }

    public override string ToString()
    {
        return
            $"{City_Name},{Original_Delivery_Day},{Exception_Delivery_Day},{Original_Delivery_Date},{Exception_Delivery_Date}";
    }
}

public sealed class OpsPlanRecordMap : ClassMap<OpsPlanRecord>
{
    public OpsPlanRecordMap()
    {
        Map(m => m.old_window_id).Name("old_window_id");
        Map(m => m.FC_Name).Name("FC Name");
        Map(m => m.Zone_Code).Name("Zone Code");
        Map(m => m.City_Name).Name("City Name");
        Map(m => m.Original_Delivery_Day).Name("Original Delivery Day");
        Map(m => m.Exception_Delivery_Day).Name("Exception Delivery Day");
        Map(m => m.Original_Delivery_Date).Name("Original Delivery Date");
        Map(m => m.Exception_Delivery_Date).Name("Exception Delivery Date");
        Map(m => m.Is_Employee_Zone).Name("Is Employee Zone");
    }
}