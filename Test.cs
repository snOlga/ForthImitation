using NUnit.Framework;

public class Test
{
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_if.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_if.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_loop.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_loop.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_math.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_math.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_swap.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_swap.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_string_const.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_string_const.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_variables.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_variables.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_dup.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_dup.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_rot.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_rot.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_simple_procedure.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_simple_procedure.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_fibonacci.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_fibonacci.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_fibonacci_procedure.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_fibonacci_procedure.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_hello_world.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_hello_world.txt")]
    public void TestsNoInput(string testPath, string resultPath)
    {
        Memory memory = new Memory();
        DataPath dataPath = new DataPath(memory, "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\input.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\output.txt");
        ControlUnit controlUnit = new ControlUnit(testPath,
                                                "D:\\ITMO\\2_year\\csa\\ForthImitation\\microcode.txt", dataPath);

        controlUnit.Work();

        dataPath.OutputFile.Close();

        string testResult = File.ReadAllText("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\output.txt");
        string resultNeeded = File.ReadAllText(resultPath);

        if (testResult != resultNeeded)
            Assert.Fail("oops");
    }

    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_hello_user.txt",
    "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_hello_user.txt",
    "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\input_user.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_cat.txt",
    "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_cat.txt",
    "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\input_cat.txt")]
    public void TestsInput(string testPath, string resultPath, string inputPath)
    {
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