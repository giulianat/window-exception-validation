namespace WindowExceptionsValidation;

public struct OpsPlanRecord : IEquatable<OpsPlanRecord>
{
    public string FC_Name	 { get; set; }
    public string Zone_Code { get; set; }
    public string City_Name { get; set; }
    public DayOfWeek Original_Delivery_Day { get; set; }
    public DayOfWeek Exception_Delivery_Day { get; set; }

    public int Original_Delivery_Date { get; set; }

    public int Exception_Delivery_Date { get; set; }

    public bool Equals(OpsPlanRecord other)
    {
        return City_Name == other.City_Name
               && Original_Delivery_Day == other.Original_Delivery_Day
               && Exception_Delivery_Day == other.Exception_Delivery_Day
               && Original_Delivery_Date == other.Original_Delivery_Date
               && Exception_Delivery_Date == other.Exception_Delivery_Date;
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