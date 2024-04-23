using Microsoft.Extensions.Logging;

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger("Start Module");
//logger.LogInformation("Hello World! Logging is {Description}.", "fun");

//args[0] for program, arg[1] for input.txt, args[2] for microcode

Memory memory = new Memory();
DataPath dataPath = new DataPath(memory, "D:\\ITMO\\2_year\\csa\\ForthImitation\\input.txt");
ControlUnit controlUnit = new ControlUnit("D:\\ITMO\\2_year\\csa\\ForthImitation\\program.txt",
                                        "D:\\ITMO\\2_year\\csa\\ForthImitation\\microcode.txt", dataPath);

controlUnit.Work();

dataPath.OutputFile.Close();

