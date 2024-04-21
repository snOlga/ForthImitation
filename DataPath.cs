public class DataPath
{
    private Memory mainMemory;
    private string[] mainStack = new string[500];
    private int mainStackPointer = 0;
    private string mainTOS = "0";
    private string[] bufferStack = new string[500];
    private int bufferStackPointer = 0;
    private string bufferTOS = "0";
    private (int leftData, int rightData) alu = (0, 0);
    private string IOData = "0";

    public DataPath(Memory memory)
    {
        this.mainMemory = memory;
    }

    private string stackBeforeSnap = "0";
    private int mainPointerBeforeSnap = 0;
    private string mainTOSBeforeSnap = "0";
    private string bufStackBeforeSnap = "0";
    private int bufPointerBeforeSnap = 0;
    private string bufTOSBeforeSnap = "0";
    private (int leftData, int rightData) aluBeforeSnap = (0, 0);
    private string IODataBeforeSnap = "0";

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
        mainStack[mainStackPointer] = stackBeforeSnap;
        mainTOSBeforeSnap = stackBeforeSnap;
        bufStackBeforeSnap = stackBeforeSnap;
        aluBeforeSnap.rightData = int.Parse(stackBeforeSnap);
        mainMemory.BufferData = stackBeforeSnap;
    }
    public void SnapBufferStack()
    {
        bufferStack[bufferStackPointer] = bufStackBeforeSnap;
        bufTOSBeforeSnap = bufStackBeforeSnap;
    }
    public void SnapMainTOS()
    {
        mainTOS = mainTOSBeforeSnap;
        IODataBeforeSnap = mainTOSBeforeSnap;
        stackBeforeSnap = mainTOSBeforeSnap;
        bufTOSBeforeSnap = mainTOSBeforeSnap;
        aluBeforeSnap.rightData = int.Parse(mainTOSBeforeSnap);
        mainMemory.BufferPointer = int.Parse(mainTOSBeforeSnap);
    }
    public void SnapBufferTOS()
    {
        bufferTOS = bufTOSBeforeSnap;
        bufStackBeforeSnap = bufTOSBeforeSnap;
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

    #endregion snaps

    public void DoOperation(string type)
    {
        int result = 0;
        switch (type)
        {
            case "+":
                result = alu.leftData + alu.rightData;
                break;
            case "-":
                result = alu.leftData + alu.rightData;
                break;
            case "<":
                result = alu.leftData < alu.rightData ? 1 : 0;
                break;
            case "and":
                result = alu.leftData & alu.rightData;
                break;
            case "or":
                result = alu.leftData | alu.rightData;
                break;
            case "inc":
                result = alu.rightData + 1;
                break;
            case "dec":
                result = alu.rightData - 1;
                break;
        }

        stackBeforeSnap = "" + result;
        mainPointerBeforeSnap = result;
        mainTOSBeforeSnap = "" + result;
        bufStackBeforeSnap = "" + result;
        bufPointerBeforeSnap = result;
        bufTOSBeforeSnap = "" + result;
        IODataBeforeSnap = "" + result;
    }

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
        aluBeforeSnap.leftData = int.Parse(mainStack[mainStackPointer]);
        mainMemory.BufferData = mainStack[mainStackPointer];
    }
    public void ReloadBufferStack()
    {
        bufTOSBeforeSnap = bufferStack[bufferStackPointer];
    }
    public void ReloadMainTOS()
    {
        IODataBeforeSnap = mainTOS;
        stackBeforeSnap = mainTOS;
        bufTOSBeforeSnap = mainTOS;
        aluBeforeSnap.rightData = int.Parse(mainTOS);
        mainMemory.BufferPointer = int.Parse(mainTOS);
    }
    public void ReloadBufferTOS()
    {
        bufStackBeforeSnap = bufferTOS;
        aluBeforeSnap.rightData = int.Parse(bufferTOS);
    }
    public void ReloadMemory()
    {
        mainTOSBeforeSnap = mainMemory.Data;
    }

    #endregion reloads


}

public class Memory
{
    private string[] mainMemory = new string[10000];
    private string dataBeforeSnap = "";
    private int pointerBeforeSnap = 0;

    private int currentPointer = 0;
    public void LoadToMemory(string data, int pointer)
    {
        mainMemory[pointer] = data;
        currentPointer = pointer;
    }
    public void LoadToMemory(string data)
    {
        mainMemory[currentPointer] =  data;
    }

    public string GetData(int pointer)
    {
        currentPointer = pointer;
        return mainMemory[pointer];
    }

    public int Pointer
    {
        get { return currentPointer; }
        set { currentPointer = value; }
    }

    public string BufferData
    {
        set { dataBeforeSnap = value; }
    }

    public string Data
    {
        get { return mainMemory[currentPointer]; }
    }
    public int BufferPointer
    {
        set { pointerBeforeSnap = value; }
    }

    public void Snap()
    {
        mainMemory[currentPointer] = dataBeforeSnap;
    }
}