public class ControlUnit
{
    Memory mainMemory;
    DataPath dataPath;
    private const int startProgrammIndex = 0;
    private const int indexForConst = 300;
    private Decoder decoder;
    private class Decoder
    {
        private string[] microcommands;
        private string dataToMemory = "";
        public Decoder(string fileName)
        {
            microcommands = File.ReadAllLines(fileName);
        }
        public string DataToMemory
        {
            get { return dataToMemory; }
        }
        public (bool willLoad, string[] microCode) DecodeInstruction(string name)
        {
            List<string> microProgramm = new List<string>();
            dataToMemory = "";
            bool isNeedToLoad = false;

            if (name.All(char.IsDigit))
            {
                isNeedToLoad = true;
            }
            if (name.StartsWith('"'))
            {
                dataToMemory = name.Substring(1, name.Length - 1);
                isNeedToLoad = true;
            }
            if (name == "variable")
            {
                dataToMemory = "addr1000"; //TODO: clean of hard code
                isNeedToLoad = true;
            }
            if (isNeedToLoad)
            {
                microProgramm.AddRange(microcommands[11..18]);
                microProgramm.AddRange(microcommands[21..23]);
            }

            switch (name)
            {
                case ".":
                    microProgramm.AddRange(microcommands[0..10]);
                    break;
                case "key":
                    microProgramm.AddRange(microcommands[11..20]);
                    break;
                case "drop":
                    microProgramm.AddRange(microcommands[2..10]);
                    break;
                case "+":
                    microProgramm.AddRange(microcommands[24..27]);
                    microProgramm.Add(microcommands[27]);
                    microProgramm.AddRange(microcommands[32..39]);
                    break;
                case "and":
                    microProgramm.AddRange(microcommands[24..27]);
                    microProgramm.Add(microcommands[28]);
                    microProgramm.AddRange(microcommands[32..39]);
                    break;
                case "or":
                    microProgramm.AddRange(microcommands[24..27]);
                    microProgramm.Add(microcommands[29]);
                    microProgramm.AddRange(microcommands[32..39]);
                    break;
                case "<":
                    microProgramm.AddRange(microcommands[24..27]);
                    microProgramm.Add(microcommands[30]);
                    microProgramm.AddRange(microcommands[32..39]);
                    break;
                case "-":
                    microProgramm.AddRange(microcommands[24..27]);
                    microProgramm.Add(microcommands[31]);
                    microProgramm.AddRange(microcommands[32..39]);
                    break;
                case "if":
                    microProgramm.AddRange(microcommands[40..45]);
                    break;
                case "do":
                    microProgramm.AddRange(microcommands[46..55]);
                    microProgramm.AddRange(microcommands[2..10]);
                    microProgramm.AddRange(microcommands[2..10]);
                    microProgramm.AddRange(microcommands[56..63]);
                    break;
                case "loop":
                    microProgramm.AddRange(microcommands[64..69]);
                    microProgramm.AddRange(microcommands[56..63]);
                    break;
                case "!":
                    microProgramm.AddRange(microcommands[70..74]);
                    break;
                case "swap":
                    microProgramm.AddRange(microcommands[75..117]);
                    break;
                case "?":
                    microProgramm.AddRange(microcommands[118..127]);
                    break;
            }

            return (isNeedToLoad, microProgramm.ToArray());
        }
    }
    public ControlUnit(string fileNameMainProg, string fileNameCM, DataPath actualDataPath)
    {
        dataPath = actualDataPath;
        mainMemory = dataPath.MainMemory;

        string[] forthProgramm = File.ReadAllLines(fileNameMainProg);

        for (int i = startProgrammIndex; i < forthProgramm.Length; i++)
        {
            mainMemory.LoadToMemory(forthProgramm[i], i);
        }
        mainMemory.LoadToMemory(" ", forthProgramm.Length); //null pointer
        decoder = new Decoder(fileNameCM);
    }
    private (bool neg, bool zero, bool less) flags = (false, false, false);
    public void Work()
    {
        int currentPointer = startProgrammIndex;
        while (mainMemory.GetData(currentPointer) != " ")
        {
            (bool willLoad, string[] microCode) decodeResult = decoder.DecodeInstruction(mainMemory.GetData(currentPointer));

            if (decodeResult.willLoad)
            {
                LoadConsts(decoder.DataToMemory, mainMemory.Pointer);
            }

            ExecuteMicroProgramm(decodeResult.microCode);


            //jumping here
            if (mainMemory.GetData(currentPointer) == "else")
            {
                while (mainMemory.GetData(currentPointer) != "then")
                {
                    currentPointer++;
                }
            }
            if (mainMemory.GetData(currentPointer) == "if")
            {
                if (flags.zero)
                {
                    while (mainMemory.GetData(currentPointer) != "else")
                    {
                        currentPointer++;
                    }
                }
            }
            if(mainMemory.GetData(currentPointer) == "loop")
            {
                if(flags.less)
                {
                    while (mainMemory.GetData(currentPointer) != "do")
                    {
                        currentPointer--;
                    }
                }
            }
            currentPointer++;
        }
    }
    private void LoadConsts(string constant, int pointer)
    {
        // if (constant.All(Char.IsDigit))
        // {
        //     mainMemory.LoadToMemory(constant, indexForConst);
        //     mainMemory.Pointer = indexForConst;
        // }
        // if (constant == "addr1000") //TODO: hard code here
        // {
        //     mainMemory.LoadToMemory("1000", indexForConst);
        //     mainMemory.Pointer = indexForConst;
        // }
        // else
        // {
        //     char[] stringData = constant.ToCharArray();
        //     mainMemory.LoadToMemory(stringData.Length + "", indexForConst);
        //     int currentIndex = indexForConst + 1;
        //     for (int i = 0; i < stringData.Length; i++)
        //     {
        //         mainMemory.LoadToMemory(stringData[i] + "", currentIndex);
        //         currentIndex++;
        //     }
        //     mainMemory.Pointer = 1000;
        // }
    }
    private void ExecuteMicroProgramm(string[] microProg)
    {
        foreach (var instruction in microProg)
        {
            char[] bytes = instruction.ToCharArray();

            if (bytes[0] == '1')
            {
                ExecuteControlCommand(bytes);
            }
            else if (bytes[0] == '0')
            {
                ExecuteOperativeCommand(bytes);
            }
        }
    }
    private void ExecuteControlCommand(char[] controlCommad)
    {
        if (controlCommad[1] == '1') //stack pointer
        {
            if (controlCommad[9] == '1') //snap
            {
                if (controlCommad[12] == '1')
                    dataPath.SnapMainPointer();
                if (controlCommad[11] == '1')
                    dataPath.SnapBufferPointer();
            }
            if (controlCommad[10] == '1') //reload
            {
                if (controlCommad[12] == '1')
                    dataPath.ReloadMainPointer();
                if (controlCommad[11] == '1')
                    dataPath.ReloadBufferPointer();
            }
        }
        else if (controlCommad[2] == '1') // stack
        {
            if (controlCommad[9] == '1') //snap
            {
                if (controlCommad[12] == '1')
                    dataPath.SnapMainStack();
                if (controlCommad[11] == '1')
                    dataPath.SnapBufferStack();
            }
            if (controlCommad[10] == '1') //reload
            {
                if (controlCommad[12] == '1')
                    dataPath.ReloadMainStack();
                if (controlCommad[11] == '1')
                    dataPath.ReloadBufferStack();
            }
        }
        else if (controlCommad[3] == '1') //memory
        {
            if (controlCommad[9] == '1') //snap
                dataPath.SnapMemory();
            if (controlCommad[10] == '1') //reload
                dataPath.ReloadMemory();
        }
        else if (controlCommad[4] == '1') //tos
        {
            if (controlCommad[9] == '1') //snap
            {
                if (controlCommad[12] == '1')
                    dataPath.SnapMainTOS();
                if (controlCommad[11] == '1')
                    dataPath.SnapBufferTOS();
            }
            if (controlCommad[10] == '1') //reload
            {
                if (controlCommad[12] == '1')
                    dataPath.ReloadMainTOS();
                if (controlCommad[11] == '1')
                    dataPath.ReloadBufferTOS();
            }
        }
        else if (controlCommad[5] == '1')
        {
            dataPath.SnapAlu();
        }
        else if (controlCommad[6] == '1') //io
        {
            if (controlCommad[9] == '1')
                dataPath.SnapIO("in");
            if (controlCommad[10] == '1')
                dataPath.SnapIO("out");
        }
        else if (controlCommad[7] == '1')
        {
            //nothing here :(
        }
        else if (controlCommad[8] == '1') //flags
        {
            flags = dataPath.SnapFlags();
        }
        else
        {
            dataPath.SnapNull();
        }
    }
    private void ExecuteOperativeCommand(char[] operativeCommad)
    {
        if (operativeCommad[1] == '1')
        {
            dataPath.DoOperation("+");
        }
        if (operativeCommad[2] == '1')
        {
            dataPath.DoOperation("inc");
        }
        if (operativeCommad[3] == '1')
        {
            dataPath.DoOperation("and");
        }
        if (operativeCommad[4] == '1')
        {
            dataPath.DoOperation("or");
        }
        if (operativeCommad[5] == '1')
        {
            dataPath.DoOperation("<");
        }
        if (operativeCommad[6] == '1')
        {
            dataPath.DoOperation("-");
        }
        if (operativeCommad[7] == '1')
        {
            dataPath.DoOperation("dec");
        }
    }
}