using NUnit.Framework;
using Serilog;

public class Test
{
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_if.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_if.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\if.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_loop.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_loop.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\loop.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_math.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_math.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\math.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_swap.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_swap.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\swap.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_string_const.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_string_const.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\string_const.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_variables.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_variables.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\variables.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_dup.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_dup.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\dup.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_rot.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_rot.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\rot.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_simple_procedure.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_simple_procedure.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\simple_procedure.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_fibonacci.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_fibonacci.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\fibonacci.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_fibonacci_procedure.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_fibonacci_procedure.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\fibonacci_procedure.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_hello_world.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_hello_world.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\hello_world.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_hello_user.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_hello_user.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\hello_user.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input_user.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_cat.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\results\\result_cat.txt",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\logs\\cat.log",
                "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\inputs\\input_cat.txt")]
    public void TestsNoInput(string testPath, string resultPath, string logging, string input)
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
        DataPath dataPath = new DataPath(memory, input, "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\output.txt");
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
}