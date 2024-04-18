public class Memory
{
    private List<string> currentMemory = new List<string>();
    private int memoryIndex = 0;

    public void PushToMemory(string value)
    {
        currentMemory.Add(value);
    }

    public void PushToMemory(List<string> value)
    {
        // value.Select(x => currentMemory.Add(x));
        currentMemory.AddRange(value);
    }

    public string GetNode()
    {
        int bufferIndex = memoryIndex;
        memoryIndex++;
        if(bufferIndex < currentMemory.Count-1)
        {
            return currentMemory[bufferIndex];
        }
        return "";
    }

    public int MemoryIndex
    {
        get { return memoryIndex; }
        set { memoryIndex = value; }
    }
}