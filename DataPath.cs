using Serilog;

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
        Log.Information($"Snapped main pointer {mainStackPointer}");
    }
    public void SnapBufferPointer()
    {
        bufferStackPointer = bufPointerBeforeSnap;
        aluBeforeSnap.rightData = bufPointerBeforeSnap;
        Log.Information($"Snapped buffer pointer {bufferStackPointer}");
    }
    public void SnapMainStack()
    {
        mainStack[mainStackPointer] = mainStackBeforeSnap;
        mainTOSBeforeSnap = mainStackBeforeSnap;
        bufStackBeforeSnap = mainStackBeforeSnap;
        mainMemory.BufferData = mainStackBeforeSnap;
        if (mainStackBeforeSnap.All(Char.IsDigit))
            aluBeforeSnap.rightData = int.Parse(mainStackBeforeSnap);
        Log.Information($"Snapped main stack {mainStack[mainStackPointer]}");
    }
    public void SnapBufferStack()
    {
        bufferStack[bufferStackPointer] = bufStackBeforeSnap;
        bufTOSBeforeSnap = bufStackBeforeSnap;
        if (bufStackBeforeSnap.All(Char.IsDigit))
            aluBeforeSnap.leftData = int.Parse(bufStackBeforeSnap);
        Log.Information($"Snapped buffer stack {bufferStack[bufferStackPointer]}");
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
        Log.Information($"Snapped main TOS {mainTOS}");
    }
    public void SnapBufferTOS()
    {
        bufferTOS = bufTOSBeforeSnap;
        bufStackBeforeSnap = bufTOSBeforeSnap;
        mainTOSBeforeSnap = bufTOSBeforeSnap;
        if (bufTOSBeforeSnap.All(Char.IsDigit))
            aluBeforeSnap.rightData = int.Parse(bufTOSBeforeSnap);
        Log.Information($"Snapped buffer TOS {bufferTOS}");
    }
    public void SnapMemory()
    {
        mainMemory.Snap();
        mainTOSBeforeSnap = mainMemory.Data;
    }
    public void SnapAlu()
    {
        alu = aluBeforeSnap;
        Log.Information($"Snapped alu {alu}");
    }
    public void SnapNull()
    {
        aluBeforeSnap.leftData = 0;
        Log.Information($"Snapped null");
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
        Log.Information($"Snapped IO {IOData}");
    }
    public (bool neg, bool zero, bool less) SnapFlags()
    {
        flags = flagsBeforeSnap;
        Log.Information($"Snapped flags {flags}");
        return flags;
    }

    #endregion snaps

    #region reloads
    public void ReloadMainPointer()
    {
        aluBeforeSnap.rightData = mainStackPointer;
        Log.Information($"Reload main pointer {mainStackPointer}");
    }
    public void ReloadBufferPointer()
    {
        aluBeforeSnap.rightData = bufferStackPointer;
        Log.Information($"Reload buffer pointer {bufferStackPointer}");
    }
    public void ReloadMainStack()
    {
        mainTOSBeforeSnap = mainStack[mainStackPointer];
        bufStackBeforeSnap = mainStack[mainStackPointer];
        mainMemory.BufferData = mainStack[mainStackPointer];
        if (mainStack[mainStackPointer].All(Char.IsDigit))
            aluBeforeSnap.leftData = int.Parse(mainStack[mainStackPointer]);
        Log.Information($"Reload main stack {mainStack[mainStackPointer]}");
    }
    public void ReloadBufferStack()
    {
        bufTOSBeforeSnap = bufferStack[bufferStackPointer];
        mainStackBeforeSnap = bufferStack[bufferStackPointer];
        if (bufferStack[bufferStackPointer].All(Char.IsDigit))
            aluBeforeSnap.leftData = int.Parse(bufferStack[bufferStackPointer]);
        Log.Information($"Reload buffer stack {bufferStack[bufferStackPointer]}");
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
        Log.Information($"Reload main TOS {mainTOS}");
    }
    public void ReloadBufferTOS()
    {
        bufStackBeforeSnap = bufferTOS;
        mainTOSBeforeSnap = bufferTOS;
        if (bufferTOS.All(Char.IsDigit))
            aluBeforeSnap.rightData = int.Parse(bufferTOS);
        Log.Information($"Reload buffer TOS {bufferTOS}");
    }
    public void ReloadMemory()
    {
        mainTOSBeforeSnap = mainMemory.Data;
        Log.Information($"Reload memory {mainTOSBeforeSnap}");
    }
    public void ReloadReadMemory()
    {
        mainTOSBeforeSnap = mainMemory.GetData(int.Parse(mainTOS));
        Log.Information($"Reload memory {mainTOSBeforeSnap}");
    }
    #endregion reloads

    public Memory MainMemory
    {
        get { return mainMemory; }
    }
    public void DoOperation(MathOperation type)
    {
        int result = 0;
        switch (type)
        {
            case MathOperation.Add:
                result = alu.leftData + alu.rightData;
                flagsBeforeSnap = (result < 0, result == 0, flagsBeforeSnap.less);
                break;
            case MathOperation.Subtract:
                result = alu.leftData - alu.rightData;
                flagsBeforeSnap = (result < 0, result == 0, flagsBeforeSnap.less);
                break;
            case MathOperation.Less:
                result = alu.rightData < alu.leftData ? 1 : 0;
                flagsBeforeSnap = (flagsBeforeSnap.neg, flagsBeforeSnap.zero, result == 1);
                break;
            case MathOperation.And:
                result = alu.leftData & alu.rightData;
                flagsBeforeSnap = (flagsBeforeSnap.neg, result == 0, flagsBeforeSnap.less);
                break;
            case MathOperation.Or:
                result = alu.leftData | alu.rightData;
                flagsBeforeSnap = (flagsBeforeSnap.neg, result == 0, flagsBeforeSnap.less);
                break;
            case MathOperation.Inc:
                result = alu.rightData + 1;
                flagsBeforeSnap = (result < 0, result == 0, flagsBeforeSnap.less);
                break;
            case MathOperation.Dec:
                result = alu.rightData - 1;
                flagsBeforeSnap = (result < 0, result == 0, flagsBeforeSnap.less);
                break;
        }

        Log.Information($"Alu operation: {type}");

        mainStackBeforeSnap = result.ToString();
        mainPointerBeforeSnap = result;
        mainTOSBeforeSnap = result.ToString();
        bufStackBeforeSnap = result.ToString();
        bufPointerBeforeSnap = result;
        bufTOSBeforeSnap = result.ToString();
        IODataBeforeSnap = result.ToString();
    }
}