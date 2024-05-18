using Serilog;

/*
args:
0 - main programm
1 - file for input
2 - file for output
3 - forth to mnemonic file
4 - mnemonic to microcode file
5 - logging file
*/

string mainProg = Directory.GetCurrentDirectory() + "\\" + args[0];
string inputFile = Directory.GetCurrentDirectory() + "\\" + args[1];
string outputFile = Directory.GetCurrentDirectory() + "\\" + args[2];
string mnemonic = Directory.GetCurrentDirectory() + "\\" + args[3];
string microcode = Directory.GetCurrentDirectory() + "\\" + args[4];
string logging = Directory.GetCurrentDirectory() + "\\" + args[5];

File.WriteAllText(logging, string.Empty);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogFiles", $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}", logging),
                rollingInterval: RollingInterval.Infinite,
                outputTemplate: "[{Timestamp:HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
    .CreateLogger();

Log.Information("Imitation started");
Memory memory = new Memory();
DataPath dataPath = new DataPath(memory, inputFile, outputFile);
ControlUnit controlUnit = new ControlUnit(mainProg, mnemonic, microcode, dataPath);

controlUnit.Work();

dataPath.OutputFile.Close();
Log.CloseAndFlush();

Console.WriteLine("Tick count: " + DataPath.TickCounter);
Console.WriteLine(controlUnit.GetMetaData());

