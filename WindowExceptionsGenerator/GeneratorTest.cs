using NUnit.Framework;

namespace WindowExceptionsGenerator;

[TestFixture]
public class GeneratorTest
{
    [Test] public void ShouldGenerateLineHaulZones()
    {
        const string expectedFile = @"./TestCsv/Expected - Zones - Line Haul.csv";
        const string actualFile = @"./TestCsv/CSV Upload - Zones - Line Haul.csv";
        
        AssertFileContentsAreEqual(expectedFile, actualFile);
    }
    
    [Test]
    public void ShouldGenerateLocalZones()
    {
        const string expectedFile = @"./TestCsv/Expected - Zones - Local.csv";
        const string actualFile = @"./TestCsv/CSV Upload - Zones - Local.csv";
        
        AssertFileContentsAreEqual(expectedFile, actualFile);
    }
    
    private static void AssertFileContentsAreEqual(string pathToExpectedResult, string pathToActualResult)
    {
        var actualContents = File.ReadLines(pathToActualResult).Select(line => line.Split(",").Skip(1).Aggregate((s1, s2) => $"{s1},{s2}")).ToList();
        var actualHeader = actualContents.First();
        var expectedContents = File.ReadLines(pathToExpectedResult).Select(line => line.Split(",").Skip(1).Aggregate((s1, s2) => $"{s1},{s2}")).ToList();
        var expectedHeader = expectedContents.First();
        var dataInExpectedAndActual = expectedContents.Intersect(actualContents);             

        Assert.That(actualContents, Has.Count.EqualTo(expectedContents.Count));
        Assert.That(actualHeader, Is.EqualTo(expectedHeader));
        Assert.That(dataInExpectedAndActual.Count(), Is.EqualTo(expectedContents.Count));
    }
}