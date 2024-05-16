using NUnit.Framework;
using Serilog;

public class Test
{
    [TestCase("\\test_if.txt",
                "\\results\\result_if.txt",
                "\\logs\\if.log",
                "\\inputs\\input.txt")]
    [TestCase("\\test_loop.txt",
                "\\results\\result_loop.txt",
                "\\logs\\loop.log",
                "\\inputs\\input.txt")]
    [TestCase("\\test_math.txt",
                "\\results\\result_math.txt",
                "\\logs\\math.log",
                "\\inputs\\input.txt")]
    [TestCase("\\test_swap.txt",
                "\\results\\result_swap.txt",
                "\\logs\\swap.log",
                "\\inputs\\input.txt")]
    [TestCase("\\test_string_const.txt",
                "\\results\\result_string_const.txt",
                "\\logs\\string_const.log",
                "\\inputs\\input.txt")]
    [TestCase("\\test_variables.txt",
                "\\results\\result_variables.txt",
                "\\logs\\variables.log",
                "\\inputs\\input.txt")]
    [TestCase("\\test_dup.txt",
                "\\results\\result_dup.txt",
                "\\logs\\dup.log",
                "\\inputs\\input.txt")]
    [TestCase("\\test_rot.txt",
                "\\results\\result_rot.txt",
                "\\logs\\rot.log",
                "\\inputs\\input.txt")]
    [TestCase("\\test_simple_procedure.txt",
                "\\results\\result_simple_procedure.txt",
                "\\logs\\simple_procedure.log",
                "\\inputs\\input.txt")]
    [TestCase("\\test_fibonacci.txt",
                "\\results\\result_fibonacci.txt",
                "\\logs\\fibonacci.log",
                "\\inputs\\input.txt")]
    [TestCase("\\test_fibonacci_procedure.txt",
                "\\results\\result_fibonacci_procedure.txt",
                "\\logs\\fibonacci_procedure.log",
                "\\inputs\\input.txt")]
    [TestCase("\\test_hello_world.txt",
                "\\results\\result_hello_world.txt",
                "\\logs\\hello_world.log",
                "\\inputs\\input.txt")]
    [TestCase("\\test_hello_user.txt",
                "\\results\\result_hello_user.txt",
                "\\logs\\hello_user.log",
                "\\inputs\\input_user.txt")]
    [TestCase("\\test_cat.txt",
                "\\results\\result_cat.txt",
                "\\logs\\cat.log",
                "\\inputs\\input_cat.txt")]
    public void TestsNoInput(string testPath, string resultPath, string logging, string input)
    {
        testPath = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().IndexOf("\\bin\\Debug\\net8.0")) + "\\tests" + testPath;
        resultPath = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().IndexOf("\\bin\\Debug\\net8.0")) + "\\tests" + resultPath;
        logging = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().IndexOf("\\bin\\Debug\\net8.0")) + "\\tests" + logging;
        input = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().IndexOf("\\bin\\Debug\\net8.0")) + "\\tests" + input;
        File.WriteAllText(logging, string.Empty);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogFiles", $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}", logging),
                        rollingInterval: RollingInterval.Infinite,
                        outputTemplate: "[{Timestamp:HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
            .CreateLogger();
        Log.Information("Imitation started");

        string outputFile = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().IndexOf("\\bin\\Debug\\net8.0")) + "\\tests" + "\\output.txt";
        string microcodeFile = Directory.GetCurrentDirectory().Remove(Directory.GetCurrentDirectory().IndexOf("\\bin\\Debug\\net8.0")) + "\\forth_to_mnem.txt";

        Memory memory = new Memory();
        DataPath dataPath = new DataPath(memory, input, outputFile);
        ControlUnit controlUnit = new ControlUnit(testPath, microcodeFile, dataPath, "D:\\ITMO\\2_year\\csa\\ForthImitation\\mnemonic_description.txt");

        controlUnit.Work();

        dataPath.OutputFile.Close();
        Log.CloseAndFlush();

        string testResult = File.ReadAllText(outputFile);
        string resultNeeded = File.ReadAllText(resultPath);

        if (testResult != resultNeeded)
            Assert.Fail("oops");
    }
}