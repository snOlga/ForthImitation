using NUnit.Framework;
using Serilog;

public class Test
{
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_if.txt", 
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_if.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\if.log")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_loop.txt", 
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_loop.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\loop.log")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_math.txt", 
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_math.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\math.log")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_swap.txt", 
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_swap.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\swap.log")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_string_const.txt", 
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_string_const.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\string_const.log")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_variables.txt", 
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_variables.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\variables.log")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_dup.txt", 
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_dup.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\dup.log")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_rot.txt", 
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_rot.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\rot.log")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_simple_procedure.txt", 
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_simple_procedure.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\simple_procedure.log")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_fibonacci.txt", 
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_fibonacci.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\fibonacci.log")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_fibonacci_procedure.txt", 
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_fibonacci_procedure.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\fibonacci_procedure.log")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_hello_world.txt", 
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_hello_world.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\hello_world.log")]
    public void TestsNoInput(string testPath, string resultPath, string logging)
    {
        File.WriteAllText(logging, string.Empty);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogFiles", $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}", logging),
                        rollingInterval: RollingInterval.Infinite,
                        outputTemplate: "[{Timestamp:HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
            .CreateLogger();
        Log.Information("Imitation started");
        
        Memory memory = new Memory();
        DataPath dataPath = new DataPath(memory, "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\input.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\output.txt");
        ControlUnit controlUnit = new ControlUnit(testPath,
                                                "D:\\ITMO\\2_year\\csa\\ForthImitation\\microcode.txt", dataPath);

        controlUnit.Work();

        dataPath.OutputFile.Close();
        Log.CloseAndFlush();

        string testResult = File.ReadAllText("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\output.txt");
        string resultNeeded = File.ReadAllText(resultPath);

        if (testResult != resultNeeded)
            Assert.Fail("oops");
    }

    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_hello_user.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_hello_user.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\input_user.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\hello_user.log")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_cat.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_cat.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\input_cat.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\cat.log")]
    public void TestsInput(string testPath, string resultPath, string inputPath, string logging)
    {
        File.WriteAllText(logging, string.Empty);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogFiles", $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}", logging),
                        rollingInterval: RollingInterval.Infinite,
                        outputTemplate: "[{Timestamp:HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
            .CreateLogger();
        Log.Information("Imitation started");

        Memory memory = new Memory();
        DataPath dataPath = new DataPath(memory, inputPath, "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\output.txt");
        ControlUnit controlUnit = new ControlUnit(testPath,
                                                "D:\\ITMO\\2_year\\csa\\ForthImitation\\microcode.txt", dataPath);

        controlUnit.Work();

        dataPath.OutputFile.Close();

        string testResult = File.ReadAllText("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\output.txt");
        string resultNeeded = File.ReadAllText(resultPath);

        if (testResult != resultNeeded)
            Assert.Fail("oops");
    }
}