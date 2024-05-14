using Serilog;

public class Memory
{
    private string[] mainMemory = new string[10000];
    private string dataBeforeSnap = "";
    private int pointerBeforeSnap = 0;
    private int pointer = 0;
    public void LoadToMemory(int index, string data)
    {
        DataPath.TickCounter += 2;
        mainMemory[index] = data;
        Log.Information($"{data} loaded to memory at {index}");
    }
    public void LoadToMemory(string data)
    {
        mainMemory[pointer] = data;
        Log.Information($"{data} loaded to memory at {pointer}");
    }
    public string GetData(int index)
    {
        DataPath.TickCounter += 2;
        pointer = index;
        return mainMemory[index];
    }
    public int Pointer
    {
        get { return pointer; }
        set { pointer = value; }
    }
    public string BufferData
    {
        set { dataBeforeSnap = value; }
    }
    public string Data
    {
        get
        {
            DataPath.TickCounter++;
            return mainMemory[pointer];
        }
    }
    public int BufferPointer
    {
        set { pointerBeforeSnap = value; }
    }
    public void Snap()
    {
        DataPath.TickCounter += 2;
        mainMemory[pointerBeforeSnap] = dataBeforeSnap;
        pointer = pointerBeforeSnap;
        Log.Information($"Memory snapped: data {dataBeforeSnap} at {pointerBeforeSnap}");
    }
}