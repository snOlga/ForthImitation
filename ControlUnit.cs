public class ControlUnit
{
    private readonly Memory mainMemory;
    private readonly DataPath dataPath;
    private const int startProgrammIndex = 0;
    private const int indexForConst = 500;
    private int indexForVariable = 100;
    private readonly Decoder decoder;
    private (bool neg, bool zero, bool less) flags = (false, false, false);
    private int howManyPushConst = 0;
    private bool isPushedFromMemory = false;
    private int bufferOffset = 0;
    private Dictionary<string, string[]> mnemonicAndMicrocode = new Dictionary<string, string[]>();
    private readonly Dictionary<string, string[]> namedProcedures = new Dictionary<string, string[]>();
    #region metadata
    private static int microCount = 0;
    private static int programSize = 0;
    private static int instructionCount = 0;
    public string GetMetaData()
    {
        return $"Microcommands count: {microCount} | Program size in bit: {programSize} | Instruction count: {instructionCount}";
    }
    #endregion
    private class Decoder
    {
        private string[] mnemonics;
        private string dataToMemory = "";
        public Decoder(string fileName)
        {
            mnemonics = File.ReadAllLines(fileName);
        }
        public string DataToMemory
        {
            get { return dataToMemory; }
        }
        public (string[] microCode, LoadTypes loadType) DecodeInstruction(string name)
        {
            LoadTypes loadType = LoadTypes.Nothing;
            List<string> mnemonicProgramm = new List<string>();
            dataToMemory = "";

            if (int.TryParse(name, out int nevermind))
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
                mnemonicProgramm.AddRange(mnemonics[15..18]);
            }

            switch (name)
            {
                case ".":
                    mnemonicProgramm.AddRange(mnemonics[1..4]);
                    break;
                case "key":
                    mnemonicProgramm.AddRange(mnemonics[10..13]);
                    break;
                case "drop":
                    mnemonicProgramm.AddRange(mnemonics[6..8]);
                    break;
                case "dup":
                    mnemonicProgramm.AddRange(mnemonics[20..22]);
                    break;
                case "rot":
                    mnemonicProgramm.AddRange(mnemonics[114..126]);
                    break;
                case "+":
                    mnemonicProgramm.AddRange(mnemonics[30..34]);
                    break;
                case "and":
                    mnemonicProgramm.AddRange(mnemonics[42..46]);
                    break;
                case "or":
                    mnemonicProgramm.AddRange(mnemonics[48..52]);
                    break;
                case "<":
                    mnemonicProgramm.AddRange(mnemonics[24..28]);
                    break;
                case "-":
                    mnemonicProgramm.AddRange(mnemonics[36..40]);
                    break;
                case "if":
                    mnemonicProgramm.AddRange(mnemonics[54..59]);
                    break;
                case "else":
                    mnemonicProgramm.AddRange(mnemonics[61..66]);
                    break;
                case "do":
                    mnemonicProgramm.AddRange(mnemonics[68..80]);
                    break;
                case "loop":
                    mnemonicProgramm.AddRange(mnemonics[82..87]);
                    break;
                case "swap":
                    mnemonicProgramm.AddRange(mnemonics[93..107]);
                    break;
                case "!":
                    mnemonicProgramm.AddRange(mnemonics[89..91]);
                    break;
                case "?":
                    mnemonicProgramm.AddRange(mnemonics[109..112]);
                    break;
            }

            return (mnemonicProgramm.ToArray(), loadType);
        }
    }
    public ControlUnit(string fileNameMainProg, string fileNameCM, DataPath actualDataPath, string fileNameMnemonicDescription)
    {
        dataPath = actualDataPath;
        mainMemory = dataPath.MainMemory;

        string forthProgrammLine = File.ReadAllText(fileNameMainProg);
        string[] forthProgramm = forthProgrammLine.Split(' ');

        int indexForLoading = startProgrammIndex;

        string rememberLine = "";

        //prog to memory
        for (int instrIndex = 0; instrIndex < forthProgramm.Length; instrIndex++)
        {
            string instruction = forthProgramm[instrIndex];
            if (instruction.Contains('\"') && (instruction.Substring(0, 1) == "\"" && !instruction.EndsWith("\"") || instruction == "\""))
            {
                rememberLine += instruction + " ";
                instrIndex++;
                instruction = forthProgramm[instrIndex];
                while (!instruction.Contains('\"'))
                {
                    rememberLine += instruction + " ";
                    instrIndex++;
                    instruction = forthProgramm[instrIndex];
                }
                rememberLine += instruction;
                instruction = rememberLine;
                rememberLine = "";
            }
            if (instruction == ":")
            {
                instrIndex++;
                string procedureName = forthProgramm[instrIndex];
                List<string> procedure = new List<string>();
                instrIndex++;
                while (forthProgramm[instrIndex] != ";")
                {
                    procedure.Add(forthProgramm[instrIndex]);
                    instrIndex++;
                }
                namedProcedures.Add(procedureName, procedure.ToArray());
                indexForLoading--;
            }
            else if (namedProcedures.ContainsKey(instruction))
            {
                string[] currentProcedure = namedProcedures[instruction];
                foreach (string instrInProcedure in currentProcedure)
                {
                    mainMemory.LoadToMemory(indexForLoading, instrInProcedure);
                    indexForLoading++;
                }
                indexForLoading--;
            }
            else
            {
                mainMemory.LoadToMemory(indexForLoading, instruction);
            }

            indexForLoading++;
        }
        mainMemory.LoadToMemory(indexForLoading, "_"); //null pointer
        decoder = new Decoder(fileNameCM);

        //fill dict of microcode
        string[] mnemonicDescription = File.ReadAllLines(fileNameMnemonicDescription);

        string currentMnemonic = "";
        List<string> microcode = new List<string>();
        foreach(string mnemonic in mnemonicDescription)
        {
            if(mnemonic.StartsWith("1") || mnemonic.StartsWith("0"))
            {
                string code = mnemonic.Split(' ')[0];
                microcode.Add(code);
            }
            else if(mnemonic != "")
            {
                if(currentMnemonic != "")
                {
                    mnemonicAndMicrocode.Add(currentMnemonic, microcode.ToArray());
                }
                currentMnemonic = mnemonic;
                microcode.Clear();
            }
        }
    }
    public void Work()
    {
        int currentPointer = startProgrammIndex;
        while (mainMemory.GetData(currentPointer) != "_")
        {
            (string[] mnemonicProg, LoadTypes loadType) decodeResult = decoder.DecodeInstruction(mainMemory.GetData(currentPointer));

            string[] microCode = Preprocessing(decodeResult);

            try
            {
                ExecuteMicroProgramm(microCode);
            }
            catch (System.NullReferenceException)
            {
                return;
            }

            currentPointer = Postprocessing(mainMemory, currentPointer);

            currentPointer++;
            instructionCount++;
        }
    }
    private string[] Preprocessing((string[] mnemonicProg, LoadTypes loadType) decodeResult)
    {
        if (decodeResult.loadType != LoadTypes.Nothing)
        {
            LoadConsts(decoder.DataToMemory, decodeResult.loadType);
        }

        List<string> microProg = new List<string>();

        foreach (string mnemonic in decodeResult.mnemonicProg)
        {
            foreach (var microcommand in mnemonicAndMicrocode[mnemonic])
            {
                microProg.Add(microcommand);
            }
        }

        return microProg.ToArray();
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
            mainMemory.LoadToMemory(indexForConst, indexForVariable.ToString());
            isPushedFromMemory = true;
            bufferOffset = 0;
        }
        else if (loadType == LoadTypes.StringData)
        {
            char[] stringData = constant.ToCharArray();
            int currentIndex = indexForConst;
            for (int i = 0; i < stringData.Length; i++)
            {
                mainMemory.LoadToMemory(currentIndex, stringData[i].ToString());
                currentIndex++;
            }
            mainMemory.LoadToMemory(currentIndex, stringData.Length.ToString());
            howManyPushConst = stringData.Length + 1;
            isPushedFromMemory = true;
            bufferOffset = 0;
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
            microCount++;
            programSize += instruction.Length;
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