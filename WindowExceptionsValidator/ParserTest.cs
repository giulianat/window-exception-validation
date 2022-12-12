using System.Text.RegularExpressions;

namespace WindowExceptionsValidation;

[TestFixture]
public class ParserTests
{
    private const string FilePath = @"./csv/Xmas and NY Holiday Market_Zone Exception Tool - Import Zone.csv";
    private readonly string _source = File.ReadAllText(FilePath);

    [Test]
    public void ShouldMapToOpsPlanRecord()
    {
        var numberOfDashes = Regex.Matches(_source, " - ").Count;
        var numberOfEmp = Regex.Matches(_source, "EMP").Count;
        var expectedOpsPlans = numberOfDashes - numberOfEmp;

        var opsPlan = Parser.ParseImportZone(FilePath).ToList();

        var failMessage =
            $"Expected = Total records {numberOfDashes} - EMP Records {numberOfEmp}";
        Assert.That(opsPlan, Has.Count.EqualTo(expectedOpsPlans), failMessage);
    }

    [Test]
    public void ShouldNotIncludeUnchangedWindows()
    {
        var opsPlan = Parser.ParseImportZoneWithChangesToDeliveryDay(FilePath)
            .Where(p => p.Exception_Delivery_Date == p.Original_Delivery_Date);

        Assert.That(opsPlan, Is.Empty);
    }
}