using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger("Start Module");
//logger.LogInformation("Hello World! Logging is {Description}.", "fun");

Memory memory= new Memory();
DataPath dataPath= new DataPath(memory);
ControlUnit controlUnit = new ControlUnit("D:\\ITMO\\2_year\\csa\\ForthImitation\\program.txt", 
                                        "D:\\ITMO\\2_year\\csa\\ForthImitation\\microcode.txt", memory, dataPath);

controlUnit.Work();

