public class ControlUnit
{
    Memory mainMemory;
    DataPath dataPath;
    private const int startProgrammIndex = 0;
    private const int indexForConst = 500;
    private int indexForVariable = 100;
    public static int constIndexForVariable = 100;
    private LoadTypes loadType;
    private Decoder decoder;
    private (bool neg, bool zero, bool less) flags = (false, false, false);
    private int howManyPushConst = 0;
    private bool isPushedFromMemory = false;
    private int bufferOffset = 0;
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
        public (string[] microCode, LoadTypes loadType) DecodeInstruction(string name)
        {
            LoadTypes loadType = LoadTypes.Nothing;
            List<string> microProgramm = new List<string>();
            dataToMemory = "";

            if (name.All(char.IsDigit))
            {
                loadType = LoadTypes.NumericData;
            }
            if (name.StartsWith('"'))
            {
                loadType = LoadTypes.StringData;
                dataToMemory = name.Substring(1, name.Length - 2);
            }
            if (name == "variable")
            {
                loadType = LoadTypes.Variable;
            }
            if (loadType != LoadTypes.Nothing)
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
                case "dup":
                    microProgramm.AddRange(microcommands[11..18]);
                    microProgramm.Add(microcommands[22]);
                    break;
                case "rot":
                    microProgramm.AddRange(microcommands[130..171]);
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
                case "swap":
                    microProgramm.AddRange(microcommands[75..119]);
                    break;
                case "!":
                    microProgramm.AddRange(microcommands[70..74]);
                    break;
                case "?":
                    microProgramm.AddRange(microcommands[120..129]);
                    break;
            }

            return (microProgramm.ToArray(), loadType);
        }
    }
    public ControlUnit(string fileNameMainProg, string fileNameCM, DataPath actualDataPath)
    {
        dataPath = actualDataPath;
        mainMemory = dataPath.MainMemory;

        string[] forthProgramm = File.ReadAllLines(fileNameMainProg);

        for (int i = startProgrammIndex; i < forthProgramm.Length; i++)
        {
            mainMemory.LoadToMemory(i, forthProgramm[i]);
        }
        mainMemory.LoadToMemory(forthProgramm.Length + startProgrammIndex, " "); //null pointer
        decoder = new Decoder(fileNameCM);
    }
    public void Work()
    {
        int currentPointer = startProgrammIndex;
        while (mainMemory.GetData(currentPointer) != " ")
        {
            (string[] microCode, LoadTypes loadType) decodeResult = decoder.DecodeInstruction(mainMemory.GetData(currentPointer));

            Preprocessing(decodeResult);

            ExecuteMicroProgramm(decodeResult.microCode);

            currentPointer = Postprocessing(mainMemory, currentPointer);

            currentPointer++;
        }
    }
    private void Preprocessing((string[] microCode, LoadTypes loadType) decodeResult)
    {
        if (decodeResult.loadType != LoadTypes.Nothing)
        {
            LoadConsts(decoder.DataToMemory, decodeResult.loadType);
        }
    }
    private int Postprocessing(Memory mainMemory, int currentPointer)
    {
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
        if (mainMemory.GetData(currentPointer) == "loop")
        {
            if (flags.less)
            {
                while (mainMemory.GetData(currentPointer) != "do")
                {
                    currentPointer--;
                }
            }
        }
        if (mainMemory.GetData(currentPointer) == "!")
        {
            indexForVariable++;
        }
        return currentPointer;
    }
    private void LoadConsts(string constant, LoadTypes loadType)
    {
        if (loadType == LoadTypes.NumericData)
        {
            //nothing
        }
        else if (loadType == LoadTypes.Variable)
        {
            mainMemory.LoadToMemory(indexForConst, indexForVariable + "");
            isPushedFromMemory = true;
            bufferOffset = 0;
        }
        else if (loadType == LoadTypes.StringData)
        {
            char[] stringData = constant.ToCharArray();
            mainMemory.LoadToMemory(indexForConst, stringData.Length + "");
            int currentIndex = indexForConst + 1;
            for (int i = 0; i < stringData.Length; i++)
            {
                mainMemory.LoadToMemory(currentIndex, stringData[i] + "");
                currentIndex++;
            }
            howManyPushConst = stringData.Length;
            isPushedFromMemory = true;
            bufferOffset = 1;
        }
    }
    private void ExecuteMicroProgramm(string[] microProg)
    {
        int rememberPointer = mainMemory.Pointer;
        if (isPushedFromMemory)
        {
            mainMemory.Pointer = indexForConst + bufferOffset;
            howManyPushConst--;
            bufferOffset++;
            isPushedFromMemory = howManyPushConst > 0;
        }
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
        if (isPushedFromMemory)
            ExecuteMicroProgramm(microProg);
        mainMemory.Pointer = rememberPointer;
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
            {
                if (controlCommad[13] == '0')
                    dataPath.ReloadMemory();
                if (controlCommad[13] == '1')
                    dataPath.ReloadReadMemory();
            }
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
            dataPath.DoOperation(MathOperation.Add);
        }
        if (operativeCommad[2] == '1')
        {
            dataPath.DoOperation(MathOperation.Inc);
        }
        if (operativeCommad[3] == '1')
        {
            dataPath.DoOperation(MathOperation.And);
        }
        if (operativeCommad[4] == '1')
        {
            dataPath.DoOperation(MathOperation.Or);
        }
        if (operativeCommad[5] == '1')
        {
            dataPath.DoOperation(MathOperation.Less);
        }
        if (operativeCommad[6] == '1')
        {
            dataPath.DoOperation(MathOperation.Subtract);
        }
        if (operativeCommad[7] == '1')
        {
            dataPath.DoOperation(MathOperation.Dec);
        }
    }
}
public enum LoadTypes
{
    StringData,
    NumericData,
    Variable,
    Nothing
}
public enum MathOperation
{
    Add,
    Subtract,
    Or,
    And,
    Inc,
    Dec,
    Less
}