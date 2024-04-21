public class DataPath
{

}

public class Memory
{
    private List<string> mainMemory = new List<string>();

    private int currentPointer = 0;
    public void LoadToMemory(string data, int pointer)
    {
        mainMemory.Insert(pointer, data);
        currentPointer = pointer;
    }
    public void LoadToMemory(string data)
    {
        mainMemory.Insert(currentPointer, data);
    }

    public void Snap()
    {
        
    }
}