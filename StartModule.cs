using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger("Start Module");
//logger.LogInformation("Hello World! Logging is {Description}.", "fun");

Memory memory= new Memory();
ControlUnit controlUnit = new ControlUnit("D:\\ITMO\\2_year\\csa\\ForthImitation\\microcode.txt", memory);

string progStr = File.ReadAllText("D:\\ITMO\\2_year\\csa\\ForthImitation\\program.txt");

List<string> comandsStr = progStr.Split(" ").ToList();

