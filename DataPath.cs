using Serilog;

public class DataPath
{
    public static int TickCounter = 0;
    StreamReader streamReader;
    StreamWriter streamWriter;
    private Memory mainMemory;
    #region real_registers
    private string[] mainStack = new string[500];
    private int mainStackPointer = -1;
    private string mainTOS = "0";
    private string[] bufferStack = new string[500];
    private int bufferStackPointer = -1;
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
    public DataPath(Memory memory, string inputArg, string outArg)
    {
        this.mainMemory = memory;
        streamReader = new StreamReader(inputArg);
        streamWriter = new StreamWriter(outArg);
        streamWriter.AutoFlush = true;
    }
    public StreamWriter OutputFile
    {
        get { return streamWriter; }
    }
    private string GetState()
    {
        string line = $"Main stack pointer: {mainStackPointer} | Main TOS: {mainTOS} | Main stack: ";
        foreach (string node in mainStack)
        {
            if (node != null)
                line += node + " ";
            else
                break;
        }
        line += $"| Buffer stack pointer: {bufferStackPointer} | Buffer TOS: {bufferTOS} | Buffer stack: ";
        foreach (string node in bufferStack)
        {
            if (node != null)
                line += node + " ";
            else
                break;
        }
        line += $"| IO: {IOData} | Flags: {flags}";
        return line;
    }
    #region snaps
    public void SnapMainPointer()
    {
        TickCounter++;
        mainStackPointer = mainPointerBeforeSnap;
        aluBeforeSnap.rightData = mainPointerBeforeSnap;
        Log.Information($"Snapped main pointer {mainStackPointer}");
        Log.Warning(GetState());
    }
    public void SnapBufferPointer()
    {
        TickCounter++;
        bufferStackPointer = bufPointerBeforeSnap;
        aluBeforeSnap.rightData = bufPointerBeforeSnap;
        Log.Information($"Snapped buffer pointer {bufferStackPointer}");
        Log.Warning(GetState());
    }
    public void SnapMainStack()
    {
        TickCounter++;
        mainStack[mainStackPointer] = mainStackBeforeSnap;
        mainTOSBeforeSnap = mainStackBeforeSnap;
        bufStackBeforeSnap = mainStackBeforeSnap;
        mainMemory.BufferData = mainStackBeforeSnap;
        if (mainStackBeforeSnap.All(Char.IsDigit))
            aluBeforeSnap.rightData = int.Parse(mainStackBeforeSnap);
        Log.Information($"Snapped main stack {mainStack[mainStackPointer]}");
        Log.Warning(GetState());
    }
    public void SnapBufferStack()
    {
        TickCounter++;
        bufferStack[bufferStackPointer] = bufStackBeforeSnap;
        bufTOSBeforeSnap = bufStackBeforeSnap;
        if (bufStackBeforeSnap.All(Char.IsDigit))
            aluBeforeSnap.leftData = int.Parse(bufStackBeforeSnap);
        Log.Information($"Snapped buffer stack {bufferStack[bufferStackPointer]}");
        Log.Warning(GetState());
    }
    public void SnapMainTOS()
    {
        TickCounter++;
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
        Log.Warning(GetState());
    }
    public void SnapBufferTOS()
    {
        TickCounter++;
        bufferTOS = bufTOSBeforeSnap;
        bufStackBeforeSnap = bufTOSBeforeSnap;
        mainTOSBeforeSnap = bufTOSBeforeSnap;
        if (bufTOSBeforeSnap.All(Char.IsDigit))
            aluBeforeSnap.rightData = int.Parse(bufTOSBeforeSnap);
        Log.Information($"Snapped buffer TOS {bufferTOS}");
        Log.Warning(GetState());
    }
    public void SnapMemory()
    {
        mainMemory.Snap();
        mainTOSBeforeSnap = mainMemory.Data;
        Log.Warning(GetState());
    }
    public void SnapAlu()
    {
        TickCounter++;
        alu = aluBeforeSnap;
        Log.Information($"Snapped alu {alu}");
        Log.Warning(GetState());
    }
    public void SnapNull()
    {
        TickCounter++;
        aluBeforeSnap.leftData = 0;
        Log.Information($"Snapped null");
        Log.Warning(GetState());
    }
    public void SnapIO(string code)
    {
        TickCounter++;
        IOData = IODataBeforeSnap;
        if (code == "in")
        {
            char input = (char)streamReader.Read();
            IODataBeforeSnap = input.ToString();
            if (IODataBeforeSnap == "\uFFFF")
            {
                Log.Error("Null input");
                throw new NullReferenceException("Null from input!");
            }
        }
        else if (code == "out")
        {
            if (IOData != "\uFFFF")
                streamWriter.Write(IOData);
        }
        IOData = IODataBeforeSnap;
        mainTOSBeforeSnap = IOData;
        Log.Information($"Snapped IO {IOData}");
        Log.Warning(GetState());
    }
    public (bool neg, bool zero, bool less) SnapFlags()
    {
        TickCounter++;
        flags = flagsBeforeSnap;
        Log.Information($"Snapped flags {flags}");
        Log.Warning(GetState());
        return flags;
    }
    #endregion snaps

    #region reloads
    public void ReloadMainPointer()
    {
        TickCounter++;
        aluBeforeSnap.rightData = mainStackPointer;
        Log.Information($"Reload main pointer {mainStackPointer}");
        Log.Warning(GetState());
    }
    public void ReloadBufferPointer()
    {
        TickCounter++;
        aluBeforeSnap.rightData = bufferStackPointer;
        Log.Information($"Reload buffer pointer {bufferStackPointer}");
        Log.Warning(GetState());
    }
    public void ReloadMainStack()
    {
        TickCounter++;
        mainTOSBeforeSnap = mainStack[mainStackPointer];
        bufStackBeforeSnap = mainStack[mainStackPointer];
        mainMemory.BufferData = mainStack[mainStackPointer];
        if (mainStack[mainStackPointer].All(Char.IsDigit))
            aluBeforeSnap.leftData = int.Parse(mainStack[mainStackPointer]);
        Log.Information($"Reload main stack {mainStack[mainStackPointer]}");
        Log.Warning(GetState());
    }
    public void ReloadBufferStack()
    {
        TickCounter++;
        bufTOSBeforeSnap = bufferStack[bufferStackPointer];
        mainStackBeforeSnap = bufferStack[bufferStackPointer];
        if (bufferStack[bufferStackPointer].All(Char.IsDigit))
            aluBeforeSnap.leftData = int.Parse(bufferStack[bufferStackPointer]);
        Log.Information($"Reload buffer stack {bufferStack[bufferStackPointer]}");
        Log.Warning(GetState());
    }
    public void ReloadMainTOS()
    {
        TickCounter++;
        IODataBeforeSnap = mainTOS;
        mainStackBeforeSnap = mainTOS;
        bufTOSBeforeSnap = mainTOS;
        if (mainTOS.All(Char.IsDigit))
        {
            aluBeforeSnap.rightData = int.Parse(mainTOS);
            mainMemory.BufferPointer = int.Parse(mainTOS);
        }
        Log.Information($"Reload main TOS {mainTOS}");
        Log.Warning(GetState());
    }
    public void ReloadBufferTOS()
    {
        TickCounter++;
        bufStackBeforeSnap = bufferTOS;
        mainTOSBeforeSnap = bufferTOS;
        if (bufferTOS.All(Char.IsDigit))
            aluBeforeSnap.rightData = int.Parse(bufferTOS);
        Log.Information($"Reload buffer TOS {bufferTOS}");
        Log.Warning(GetState());
    }
    public void ReloadMemory()
    {
        mainTOSBeforeSnap = mainMemory.Data;
        Log.Information($"Reload memory {mainTOSBeforeSnap}");
        Log.Warning(GetState());
    }
    public void ReloadReadMemory()
    {
        mainTOSBeforeSnap = mainMemory.GetData(int.Parse(mainTOS));
        Log.Information($"Reload memory {mainTOSBeforeSnap}");
        Log.Warning(GetState());
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
        Log.Warning(GetState());

        mainStackBeforeSnap = result.ToString();
        mainPointerBeforeSnap = result;
        mainTOSBeforeSnap = result.ToString();
        bufStackBeforeSnap = result.ToString();
        bufPointerBeforeSnap = result;
        bufTOSBeforeSnap = result.ToString();
        IODataBeforeSnap = result.ToString();
    }
}