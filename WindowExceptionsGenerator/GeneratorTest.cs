using NUnit.Framework;

namespace WindowExceptionsGenerator;

[TestFixture]
public class GeneratorTest
{
    [Test]
    public void ShouldGenerateLineHaulZones()
    {
        const string expectedFile = @"./TestCsv/Expected - Zones - Line Haul.csv";
        const string actualFile = @"./TestCsv/CSV Upload - Zones - Line Haul.csv";
        const int zoneIdColumnIndex = 0;
        int[] generatedGuidColumnsToSkip = { zoneIdColumnIndex };

        AssertFileContentsAreEqual(expectedFile, actualFile, generatedGuidColumnsToSkip);
    }

    [Test]
    public void ShouldGenerateLocalZones()
    {
        const string expectedFile = @"./TestCsv/Expected - Zones - Local.csv";
        const string actualFile = @"./TestCsv/CSV Upload - Zones - Local.csv";
        const int zoneIdColumnIndex = 0;
        int[] generatedGuidColumnsToSkip = { zoneIdColumnIndex };

        AssertFileContentsAreEqual(expectedFile, actualFile, generatedGuidColumnsToSkip);
    }

    [Test]
    public void ShouldGenerateWindows()
    {
        const string expectedFile = @"./TestCsv/Expected - Windows.csv";
        const string actualFile = @"./TestCsv/CSV Upload - Windows.csv";
        const int windowIdColumnIndex = 0;
        const int zoneIdColumnIndex = 1;
        int[] generatedGuidColumnsToSkip = { windowIdColumnIndex, zoneIdColumnIndex };

        AssertFileContentsAreEqual(expectedFile, actualFile, generatedGuidColumnsToSkip);
    }

    private static void AssertFileContentsAreEqual(string pathToExpectedResult, string pathToActualResult,
        IEnumerable<int> columnIndicesToSkip)
    {
        var actualContents = File.ReadLines(pathToActualResult)
            .Select(line =>
            {
                var columns = line.Split(",").ToList();
                columns.RemoveAll(col => columnIndicesToSkip.Contains(columns.IndexOf(col)));
                return columns.Aggregate((s1, s2) => $"{s1},{s2}");
            }).ToList();
        var actualHeader = actualContents.First();
        var expectedContents = File.ReadLines(pathToExpectedResult)
            .Select(line =>
            {
                var columns = line.Split(",").ToList();
                columns.RemoveAll(col => columnIndicesToSkip.Contains(columns.IndexOf(col)));
                return columns.Aggregate((s1, s2) => $"{s1},{s2}");
            }).ToList();
        var expectedHeader = expectedContents.First();
        var dataInExpectedNotInActual = expectedContents.Except(actualContents);
        var dataInActualNotInExpected = actualContents.Except(expectedContents);

        Assert.That(actualContents, Has.Count.EqualTo(expectedContents.Count));
        Assert.That(actualHeader, Is.EqualTo(expectedHeader));
        Assert.That(dataInExpectedNotInActual.Count(), Is.EqualTo(0));
        Assert.That(dataInActualNotInExpected.Count(), Is.EqualTo(0));
    }
}