using Serilog;

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
    private JumpSignal jumpSignal = JumpSignal.Nothing;
    private CheckSignal checkSignal = CheckSignal.Nothing;
    private Stack<int> returnAddresses = new Stack<int>();
    #region metadata
    private static int microCount = 0;
    private static int programSize = 0;
    private static int instructionCount = 0;
    private static int programLength = 0;
    public string GetMetaData()
    {
        return $"Microcommands count: {microCount} | Program size in bit: {programSize} | Instruction count: {instructionCount} | Program length: {programLength}";
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
        public (string[] decodedMnemonics, LoadTypes loadType) DecodeInstruction(string name)
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
                    mnemonicProgramm.AddRange(mnemonics[116..128]);
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
                    mnemonicProgramm.AddRange(mnemonics[68..82]);
                    break;
                case "loop":
                    mnemonicProgramm.AddRange(mnemonics[84..89]);
                    break;
                case "swap":
                    mnemonicProgramm.AddRange(mnemonics[95..109]);
                    break;
                case "!":
                    mnemonicProgramm.AddRange(mnemonics[91..93]);
                    break;
                case "?":
                    mnemonicProgramm.AddRange(mnemonics[111..114]);
                    break;
            }

            return (mnemonicProgramm.ToArray(), loadType);
        }
    }
    public ControlUnit(string fileNameMainProg, string fileNameCM, string fileNameMnemonicDescription, DataPath actualDataPath)
    {
        decoder = new Decoder(fileNameCM);
        dataPath = actualDataPath;
        mainMemory = dataPath.MainMemory;

        string forthProgrammLine = File.ReadAllText(fileNameMainProg);
        string[] forthProgramm = forthProgrammLine.Split(' ');

        int indexForLoading = startProgrammIndex;

        string rememberLine = "";

        //prog to memory
        for (int instrIndex = 0; instrIndex < forthProgramm.Length; instrIndex++)
        {
            //parsing string const
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

                    string[] translatedForLogging = decoder.DecodeInstruction(instrInProcedure).decodedMnemonics;
                    Log.Information(instrInProcedure + "\n" + String.Join("\n", translatedForLogging));
                }
                indexForLoading--;
            }
            else
            {
                mainMemory.LoadToMemory(indexForLoading, instruction);

                string[] translatedForLogging = decoder.DecodeInstruction(instruction).decodedMnemonics;
                Log.Information(instruction + "\n" + String.Join("\n", translatedForLogging));
            }

            indexForLoading++;
        }
        mainMemory.LoadToMemory(indexForLoading, "_"); //null pointer

        //fill dict of mnemonics
        string[] mnemonicDescription = File.ReadAllLines(fileNameMnemonicDescription);

        string currentMnemonic = "";
        List<string> microcode = new List<string>();
        foreach (string mnemonic in mnemonicDescription)
        {
            if (mnemonic.StartsWith("1") || mnemonic.StartsWith("0"))
            {
                string code = mnemonic.Split(' ')[0];
                microcode.Add(code);
            }
            else if (mnemonic != "")
            {
                if (currentMnemonic != "")
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
        string mnemProgResultString = "Mnemonics that done:\n";
        int currentPointer = startProgrammIndex;
        string currentInst = mainMemory.GetData(currentPointer);
        while (currentInst != "_")
        {
            (string[] mnemonicProg, LoadTypes loadType) decodeResult = decoder.DecodeInstruction(currentInst);

            mnemProgResultString += "\n--- " + currentInst + "\n" + String.Join("\n", decodeResult.mnemonicProg);
            programLength += decodeResult.mnemonicProg.Length;

            string[] microCode = Preprocessing(decodeResult);

            try
            {
                ExecuteMicroProgramm(microCode);
            }
            catch (System.NullReferenceException)
            {
                return;
            }

            currentPointer = Postprocessing(currentPointer);

            currentPointer++;
            instructionCount++;
            currentInst = mainMemory.GetData(currentPointer);
        }

        Log.Information("---\n" + mnemProgResultString);
    }
    private string[] Preprocessing((string[] mnemonicProg, LoadTypes loadType) decodeResult)
    {
        if (decodeResult.loadType != LoadTypes.Nothing)
        {
            LoadConsts(decoder.DataToMemory, decodeResult.loadType);
        }

        List<string> microProg = new List<string>();

        //mnemonic prog to microcode
        foreach (string mnemonic in decodeResult.mnemonicProg)
        {
            if (mnemonic == "")
                continue;
            if (mnemonic == "jump do")
                jumpSignal = JumpSignal.Do;
            else if (mnemonic == "jump loop")
                jumpSignal = JumpSignal.Loop;
            else if (mnemonic == "jump else")
                jumpSignal = JumpSignal.Else;
            else if (mnemonic == "jump then")
                jumpSignal = JumpSignal.Then;
            else if (mnemonic == "check zero")
                checkSignal = CheckSignal.Zero;
            else if (mnemonic == "check not zero")
                checkSignal = CheckSignal.NotZero;
            else if (mnemonic == "check negative")
                checkSignal = CheckSignal.Negative;
            else if (mnemonic == "check not negative")
                checkSignal = CheckSignal.NotNegative;
            else if (mnemonic == "check less")
                checkSignal = CheckSignal.Less;
            else if (mnemonic == "check not less")
                checkSignal = CheckSignal.NotLess;
            else if (mnemonic == "save address")
                returnAddresses.Push(mainMemory.Pointer);
            else
            {
                foreach (var microcommand in mnemonicAndMicrocode[mnemonic])
                {
                    microProg.Add(microcommand);
                }
            }
        }

        return microProg.ToArray();
    }
    private int Postprocessing(int currentPointer)
    {
        bool needToJump = false;
        if (checkSignal == CheckSignal.Zero)
            needToJump = flags.zero;
        else if (checkSignal == CheckSignal.NotZero)
            needToJump = !flags.zero;
        else if (checkSignal == CheckSignal.Less)
            needToJump = flags.less;
        else if (checkSignal == CheckSignal.NotLess)
            needToJump = !flags.less;
        else if (checkSignal == CheckSignal.Negative)
            needToJump = flags.neg;
        else if (checkSignal == CheckSignal.NotNegative)
            needToJump = !flags.neg;

        if (!needToJump)
        {
            checkSignal = CheckSignal.Nothing;
            jumpSignal = JumpSignal.Nothing;
            return currentPointer;
        }

        if (jumpSignal == JumpSignal.Else)
        {
            while (mainMemory.GetData(currentPointer) != "else")
                currentPointer++;
        }
        else if (jumpSignal == JumpSignal.Then)
        {
            while (mainMemory.GetData(currentPointer) != "then")
                currentPointer++;
        }
        else if (jumpSignal == JumpSignal.Loop)
        {
            returnAddresses.Pop();
            while (mainMemory.GetData(currentPointer) != "loop")
                currentPointer++;
        }
        else if (jumpSignal == JumpSignal.Do)
        {
            currentPointer = returnAddresses.Peek();
        }

        if (mainMemory.GetData(currentPointer) == "!")
        {
            indexForVariable++;
        }
        checkSignal = CheckSignal.Nothing;
        jumpSignal = JumpSignal.Nothing;
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
public enum JumpSignal
{
    Else,
    Then,
    Loop,
    Do,
    Nothing
}
public enum CheckSignal
{
    Zero,
    NotZero,
    Less,
    NotLess,
    Negative,
    NotNegative,
    Nothing
}