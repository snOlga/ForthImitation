using Serilog;


File.WriteAllText("D:\\ITMO\\2_year\\csa\\ForthImitation\\Log.txt", string.Empty);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogFiles", $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}", "D:\\ITMO\\2_year\\csa\\ForthImitation\\Log.txt"),
                rollingInterval: RollingInterval.Infinite,
                outputTemplate: "[{Timestamp:HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
    .CreateLogger();

Log.Information("Imitation started");
Memory memory = new Memory();
DataPath dataPath = new DataPath(memory, "D:\\ITMO\\2_year\\csa\\ForthImitation\\input.txt");
ControlUnit controlUnit = new ControlUnit("D:\\ITMO\\2_year\\csa\\ForthImitation\\program.txt",
                                        "D:\\ITMO\\2_year\\csa\\ForthImitation\\microcode.txt", dataPath);

controlUnit.Work();

dataPath.OutputFile.Close();

Log.CloseAndFlush();

Console.WriteLine("Snaps count: " + File.ReadAllLines("D:\\ITMO\\2_year\\csa\\ForthImitation\\Log.txt").Length);

