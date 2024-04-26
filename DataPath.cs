public class DataPath
{
    StreamReader streamReader;
    StreamWriter streamWriter;
    private Memory mainMemory;
    #region real_registers
    private string[] mainStack = new string[500];
    private int mainStackPointer = 0;
    private string mainTOS = "0";
    private string[] bufferStack = new string[500];
    private int bufferStackPointer = 0;
    private string bufferTOS = "0";
    private (int leftData, int rightData) alu = (0, 0);
    private string IOData = "0";
    private (bool neg, bool zero, bool less) flags = (false, false, false);
    #endregion real_registers

    #region registers_before_snap
    private string mainStackBeforeSnap = "0";
    private int mainPointerBeforeSnap = 0;
    private string mainTOSBeforeSnap = "0";
    private string bufStackBeforeSnap = "0";
    private int bufPointerBeforeSnap = 0;
    private string bufTOSBeforeSnap = "0";
    private (int leftData, int rightData) aluBeforeSnap = (0, 0);
    private string IODataBeforeSnap = "0";
    private (bool neg, bool zero, bool less) flagsBeforeSnap = (false, false, false);
    #endregion  registers_before_snap
    public DataPath(Memory memory, string inputArg)
    {
        this.mainMemory = memory;
        streamReader = new StreamReader(inputArg);
        streamWriter = new StreamWriter("D:\\ITMO\\2_year\\csa\\ForthImitation\\output.txt");
        streamWriter.AutoFlush = true;
    }
    public StreamWriter OutputFile
    {
        get { return streamWriter; }
    }
    #region snaps
    public void SnapMainPointer()
    {
        mainStackPointer = mainPointerBeforeSnap;
        aluBeforeSnap.rightData = mainPointerBeforeSnap;
    }
    public void SnapBufferPointer()
    {
        bufferStackPointer = bufPointerBeforeSnap;
        aluBeforeSnap.rightData = bufPointerBeforeSnap;
    }
    public void SnapMainStack()
    {
        mainStack[mainStackPointer] = mainStackBeforeSnap;
        mainTOSBeforeSnap = mainStackBeforeSnap;
        bufStackBeforeSnap = mainStackBeforeSnap;
        mainMemory.BufferData = mainStackBeforeSnap;
        if (mainStackBeforeSnap.All(Char.IsDigit))
            aluBeforeSnap.rightData = int.Parse(mainStackBeforeSnap);
    }
    public void SnapBufferStack()
    {
        bufferStack[bufferStackPointer] = bufStackBeforeSnap;
        bufTOSBeforeSnap = bufStackBeforeSnap;
        if (bufStackBeforeSnap.All(Char.IsDigit))
            aluBeforeSnap.leftData = int.Parse(bufStackBeforeSnap);
    }
    public void SnapMainTOS()
    {
        mainTOS = mainTOSBeforeSnap;
        IODataBeforeSnap = mainTOSBeforeSnap;
        mainStackBeforeSnap = mainTOSBeforeSnap;
        bufTOSBeforeSnap = mainTOSBeforeSnap;
        if (mainTOSBeforeSnap.All(Char.IsDigit))
        {
            aluBeforeSnap.rightData = int.Parse(mainTOSBeforeSnap);
            mainMemory.BufferPointer = int.Parse(mainTOSBeforeSnap);
        }
    }
    public void SnapBufferTOS()
    {
        bufferTOS = bufTOSBeforeSnap;
        bufStackBeforeSnap = bufTOSBeforeSnap;
        mainTOSBeforeSnap = bufTOSBeforeSnap;
        if (bufTOSBeforeSnap.All(Char.IsDigit))
            aluBeforeSnap.rightData = int.Parse(bufTOSBeforeSnap);
    }
    public void SnapMemory()
    {
        mainMemory.Snap();
        mainTOSBeforeSnap = mainMemory.Data;
    }
    public void SnapAlu()
    {
        alu = aluBeforeSnap;
    }
    public void SnapNull()
    {
        aluBeforeSnap.leftData = 0;
    }
    public void SnapIO(string code)
    {
        IOData = IODataBeforeSnap;
        if (code == "in")
        {
            char input = (char)streamReader.Read();
            IODataBeforeSnap = "" + input;
        }
        else if (code == "out")
        {
            if (IOData != "\uFFFF")
                streamWriter.Write(IOData);
        }
        IOData = IODataBeforeSnap;
        mainTOSBeforeSnap = IOData;
    }
    public (bool neg, bool zero, bool less) SnapFlags()
    {
        flags = flagsBeforeSnap;
        return flags;
    }

    #endregion snaps

    #region reloads
    public void ReloadMainPointer()
    {
        aluBeforeSnap.rightData = mainStackPointer;
    }
    public void ReloadBufferPointer()
    {
        aluBeforeSnap.rightData = bufferStackPointer;
    }
    public void ReloadMainStack()
    {
        mainTOSBeforeSnap = mainStack[mainStackPointer];
        bufStackBeforeSnap = mainStack[mainStackPointer];
        mainMemory.BufferData = mainStack[mainStackPointer];
        if (mainStack[mainStackPointer].All(Char.IsDigit))
            aluBeforeSnap.leftData = int.Parse(mainStack[mainStackPointer]);
    }
    public void ReloadBufferStack()
    {
        bufTOSBeforeSnap = bufferStack[bufferStackPointer];
        mainStackBeforeSnap = bufferStack[bufferStackPointer];
        if (bufferStack[bufferStackPointer].All(Char.IsDigit))
            aluBeforeSnap.leftData = int.Parse(bufferStack[bufferStackPointer]);
    }
    public void ReloadMainTOS()
    {
        IODataBeforeSnap = mainTOS;
        mainStackBeforeSnap = mainTOS;
        bufTOSBeforeSnap = mainTOS;
        if (mainTOS.All(Char.IsDigit))
        {
            aluBeforeSnap.rightData = int.Parse(mainTOS);
            mainMemory.BufferPointer = int.Parse(mainTOS);
        }
    }
    public void ReloadBufferTOS()
    {
        bufStackBeforeSnap = bufferTOS;
        if (bufferTOS.All(Char.IsDigit))
            aluBeforeSnap.rightData = int.Parse(bufferTOS);
    }
    public void ReloadMemory()
    {
        mainTOSBeforeSnap = mainMemory.Data;
    }
    public void ReloadReadMemory()
    {
        mainTOSBeforeSnap = mainMemory.GetData(int.Parse(mainTOS));
    }
    #endregion reloads

    public Memory MainMemory
    {
        get { return mainMemory; }
    }
    public void DoOperation(string type)
    {
        int result = 0;
        switch (type)
        {
            case "+":
                result = alu.leftData + alu.rightData;
                flagsBeforeSnap = (result < 0, result == 0, flagsBeforeSnap.less);
                break;
            case "-":
                result = alu.leftData - alu.rightData;
                flagsBeforeSnap = (result < 0, result == 0, flagsBeforeSnap.less);
                break;
            case "<":
                result = alu.rightData < alu.leftData ? 1 : 0;
                flagsBeforeSnap = (flagsBeforeSnap.neg, flagsBeforeSnap.zero, result == 1);
                break;
            case "and":
                result = alu.leftData & alu.rightData;
                flagsBeforeSnap = (flagsBeforeSnap.neg, result == 0, flagsBeforeSnap.less);
                break;
            case "or":
                result = alu.leftData | alu.rightData;
                flagsBeforeSnap = (flagsBeforeSnap.neg, result == 0, flagsBeforeSnap.less);
                break;
            case "inc":
                result = alu.rightData + 1;
                flagsBeforeSnap = (result < 0, result == 0, flagsBeforeSnap.less);
                break;
            case "dec":
                result = alu.rightData - 1;
                flagsBeforeSnap = (result < 0, result == 0, flagsBeforeSnap.less);
                break;
        }

        mainStackBeforeSnap = "" + result;
        mainPointerBeforeSnap = result;
        mainTOSBeforeSnap = "" + result;
        bufStackBeforeSnap = "" + result;
        bufPointerBeforeSnap = result;
        bufTOSBeforeSnap = "" + result;
        IODataBeforeSnap = "" + result;
    }
}