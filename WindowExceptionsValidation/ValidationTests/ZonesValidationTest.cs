using System.Globalization;
using CsvHelper;

namespace WindowExceptionsValidation.ValidationTests;

[TestFixture]
public class ZonesValidationTest
{
    [SetUp]
    public void SetUp()
    {
        using var currentDataReader = new StreamReader(CurrentDataCSV);
        using var currentDataCsv = new CsvReader(currentDataReader, CultureInfo.InvariantCulture);

        currentDataCsv.Context.RegisterClassMap<CurrentDataRecordMap>();
        _currentData = currentDataCsv.GetRecords<CurrentDataRecord>().ToList();


        using var mappingsReader = new StreamReader(MappingsCSV);
        using var mappingsCsv = new CsvReader(mappingsReader, CultureInfo.InvariantCulture);

        mappingsCsv.Context.RegisterClassMap<MarketRecordMap>();
        _xrefs = mappingsCsv.GetRecords<Market>().ToDictionary(m => m.Name, m => m.Code);

        using var opsPlanReader = new StreamReader(OuputCSV);
        using var opsPlanCsv = new CsvReader(opsPlanReader, CultureInfo.InvariantCulture);

        _plan = opsPlanCsv.GetRecords<OpsPlanRecord>().ToList();
    }

    private List<CurrentDataRecord> _currentData;
    private Dictionary<string, string> _xrefs;
    private List<OpsPlanRecord> _plan;
    private const string CurrentDataCSV = @"./csv/Christmas and New Years Window Exceptions - Current Data.csv";
    private const string MappingsCSV = @"./csv/Christmas and New Years Window Exceptions - Mappings.csv";
    private const string ZonesCSV = @"./csv/Christmas and New Years Window Exceptions - Zones.csv";

    private const string OuputCSV =
        @"./csv/Patrick's Copy of Xmas and NY Holiday Market_Zone Exception Tool - Output corrected.csv";

    [Test]
    public void Should()
    {
        using var reader = new StreamReader(ZonesCSV);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var expectedRecords = csv.GetRecords<ZonesRecord>();
        /*
         * - City
         * - New delivery day
         * - old delivery days short
         * - zone id for the zone that moved
         * - zone id for the zone that was merged into
         * - zone id (generated GUUID)
         * - FC ID - FC for either zone - should match
         * - 
         */

        var actualRecords = new List<ZonesRecord>();

        // Assert.That(expectedRecords, Is.EqualTo(actualRecords));
    }
}