public class Memory
{
    private string[] mainMemory = new string[10000];
    private string dataBeforeSnap = "";
    private int pointerBeforeSnap = 0;
    private int pointer = 0;
    public void LoadToMemory(string data, int index)
    {
        mainMemory[index] = data;
        pointer = index;
    }
    public void LoadToMemory(string data)
    {
        mainMemory[pointer] = data;
    }
    public string GetData(int index)
    {
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
        get { return mainMemory[pointer]; }
    }
    public int BufferPointer
    {
        set { pointerBeforeSnap = value; }
    }
    public void Snap()
    {
        mainMemory[pointerBeforeSnap] = dataBeforeSnap;
        pointer = pointerBeforeSnap;
    }
}