using WindowExceptionsGenerator;

const int errorFileNotFound = 2;

Console.WriteLine("Welcome to the Wonderful World of Window Exceptions!");

Console.WriteLine("Where are your CSVs located?");
var sourcePath = Console.ReadLine();
var opsPlanExists = File.Exists($"{sourcePath}/Ops Plan.csv");
var currentDataExists = File.Exists($"{sourcePath}/Current Data.csv");
if (!opsPlanExists || !currentDataExists)
{
    Console.WriteLine("I need both `Ops Plan.csv` and `Current Data.csv` in that directory.");
    Environment.Exit(errorFileNotFound);
}

Console.WriteLine("Where would you like the files to be generated?");
var destinationPath = Console.ReadLine();

Console.WriteLine("What is the name of the holiday?");
var holiday = Console.ReadLine();

Console.WriteLine("On which Sunday does your holiday week begin? (mm/dd/yyyy)");
var date = Console.ReadLine();

Console.WriteLine("Hold please, processing... ~ cue elevator music ~");
var generator = new Generator(sourcePath, destinationPath, holiday, date);

generator.GenerateZones();
