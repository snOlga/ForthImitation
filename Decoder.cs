public class Decoder
{
    private Dictionary<string, string> microCodeBase = new Dictionary<string, string>();

    public void Initialize(string file)
    {
        string[] allMC = File.ReadAllLines(file);

        //string[] mnemonicAndCode = allMC.Split(" ");

        for (int i = 0; i < allMC.Length; i ++)
        {
            string[] currentMC = allMC[i].Split(' ');
            microCodeBase.Add(currentMC[0], currentMC[1]);
        }
    }

    public void DecodeAll(Memory memory)
    {
        string currentData = memory.GetNode();
        while (currentData != "~")
        {
            if (!ifSnapped && (currentData != "else" || currentData != "then"))
                continue;

            int bufferIndex = memory.MemoryIndex;
            List<string> mnemonicProg = new List<string>();
            if (currentData.Any(char.IsDigit))
            {
                memory.PushToMemory(currentData);
                mnemonicProg.Add("rts");
                mnemonicProg.Add("sst");
                mnemonicProg.Add("ssp");
                mnemonicProg.Add("sal");
                mnemonicProg.Add("inc");
                mnemonicProg.Add("sst");
                mnemonicProg.Add("sme");
                mnemonicProg.Add("sts");
            }

            switch (currentData)
            {
                case ".":
                    mnemonicProg.Add("rts");
                    mnemonicProg.Add("sou"); // next is "pop" code, but c#'s switch case doesnt work without break
                    mnemonicProg.Add("rsp");
                    mnemonicProg.Add("sal");
                    mnemonicProg.Add("dec");
                    mnemonicProg.Add("ssp");
                    mnemonicProg.Add("sst");
                    mnemonicProg.Add("sts");
                    break;
                case "pop":
                    mnemonicProg.Add("rsp");
                    mnemonicProg.Add("sal");
                    mnemonicProg.Add("dec");
                    mnemonicProg.Add("ssp");
                    mnemonicProg.Add("sst");
                    mnemonicProg.Add("sts");
                    break;
                case "key":
                    mnemonicProg.Add("rsp");
                    mnemonicProg.Add("inc");
                    mnemonicProg.Add("ssp");
                    mnemonicProg.Add("sts");
                    mnemonicProg.Add("sst");
                    mnemonicProg.Add("sin");
                    mnemonicProg.Add("sts");
                    break;
                case "<":
                    mnemonicProg.Add("rts");
                    mnemonicProg.Add("rst");
                    mnemonicProg.Add("les");
                    mnemonicProg.Add("sts");
                    mnemonicProg.Add("rsp");
                    mnemonicProg.Add("dec");
                    mnemonicProg.Add("ssp");
                    break;
                case "+":
                    mnemonicProg.Add("rts");
                    mnemonicProg.Add("rst");
                    mnemonicProg.Add("add");
                    mnemonicProg.Add("sts");
                    mnemonicProg.Add("rsp");
                    mnemonicProg.Add("dec");
                    mnemonicProg.Add("ssp");
                    break;
                case "-":
                    mnemonicProg.Add("rts");
                    mnemonicProg.Add("rst");
                    mnemonicProg.Add("min");
                    mnemonicProg.Add("sts");
                    mnemonicProg.Add("rsp");
                    mnemonicProg.Add("dec");
                    mnemonicProg.Add("ssp");
                    break;
                case "and":
                    mnemonicProg.Add("rts");
                    mnemonicProg.Add("rst");
                    mnemonicProg.Add("and");
                    mnemonicProg.Add("sts");
                    mnemonicProg.Add("rsp");
                    mnemonicProg.Add("dec");
                    mnemonicProg.Add("ssp");
                    break;
                case "or":
                    mnemonicProg.Add("rts");
                    mnemonicProg.Add("rst");
                    mnemonicProg.Add("orr");
                    mnemonicProg.Add("sts");
                    mnemonicProg.Add("rsp");
                    mnemonicProg.Add("dec");
                    mnemonicProg.Add("ssp");
                    break;
                case "if":
                    mnemonicProg.Add("rts");
                    mnemonicProg.Add("snl");
                    mnemonicProg.Add("sal");
                    mnemonicProg.Add("orr");
                    break;
                case "else":
                    ifSnapped = !ifSnapped;
                    break;
                case "then":
                    ifSnapped = true;
                    break;
                case "for": // < if pop
                    mnemonicProg.Add("rts");
                    mnemonicProg.Add("rst");
                    mnemonicProg.Add("les");
                    mnemonicProg.Add("sts");
                    mnemonicProg.Add("rsp");
                    mnemonicProg.Add("dec");
                    mnemonicProg.Add("ssp");
                    mnemonicProg.Add("rts");
                    mnemonicProg.Add("snl");
                    mnemonicProg.Add("sal");
                    mnemonicProg.Add("orr");
                    mnemonicProg.Add("rsp");
                    mnemonicProg.Add("sal");
                    mnemonicProg.Add("dec");
                    mnemonicProg.Add("ssp");
                    mnemonicProg.Add("sst");
                    mnemonicProg.Add("sts");
                    comandForIP = memory.MemoryIndex;
                    break;
                case "loop": // inc tos goto for 
                    comandForIP--;
                    mnemonicProg.Add("rts");
                    mnemonicProg.Add("sal");
                    mnemonicProg.Add("inc");
                    mnemonicProg.Add("sts");
                    memory.MemoryIndex = comandForIP;
                    break;
            }

            List<string> microProg = DecodeMnemonic(mnemonicProg);
            Do(microProg);
            memory.MemoryIndex = bufferIndex;
        }
    }

    private bool ifSnapped = true;

    public void SnapIf(bool value)
    {
        ifSnapped = value;
    }

    private int comandForIP = 0;


    private List<string> DecodeMnemonic(List<string> mnemonicProg)
    {
        List<string> microProg = new List<string>();

        foreach (var comand in mnemonicProg)
        {
            microProg.Add(microCodeBase[comand]);
        }

        return microProg;
    }

    private void Do(List<string> microProg)
    {

    }
}