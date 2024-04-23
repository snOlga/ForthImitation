using NUnit.Framework;

public class Test
{
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_if.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_if.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_loop.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_loop.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_math.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_math.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_swap.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_swap.txt")]
    [TestCase("D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\test_string_const.txt", "D:\\ITMO\\2_year\\csa\\ForthImitation\\tests\\result_string_const.txt")]
    public void DoTest(string testPath, string resultPath)
    {
        Memory memory = new Memory();
        DataPath dataPath = new DataPath(memory, "D:\\ITMO\\2_year\\csa\\ForthImitation\\input.txt");
        ControlUnit controlUnit = new ControlUnit(testPath,
                                                "D:\\ITMO\\2_year\\csa\\ForthImitation\\microcode.txt", dataPath);

        controlUnit.Work();

        dataPath.OutputFile.Close();

        string testResult = File.ReadAllText("D:\\ITMO\\2_year\\csa\\ForthImitation\\output.txt");
        string resultNeeded = File.ReadAllText(resultPath);

        if(testResult != resultNeeded)
            Assert.Fail("oops");
    }
}