public class ControlUnit
{
    Memory mainMemory;
    private const int startProgrammIndex = 0;
    private Decoder decoder;
    private class Decoder
    {
        private string[] microcommands;
        private string needLoadToMemory = "";
        public Decoder(string fileName)
        {
            microcommands = File.ReadAllLines(fileName);
        }

        public (bool, string[]) DecodeInstruction(string name)
        {
            List<string> microProgramm = new List<string>();
            needLoadToMemory = "";
            bool isNeedToLoad = false;

            if (name.Any(char.IsDigit))
            {
                needLoadToMemory = name;
                isNeedToLoad = true;
            }
            if (name.StartsWith('"'))
            {
                needLoadToMemory = name.Substring(1, name.Length - 1);
                isNeedToLoad = true;
            }
            if (name == "variable")
            {
                needLoadToMemory = "500"; //TODO: clean of hard code
                isNeedToLoad = true;
            }
            if (isNeedToLoad)
            {
                microProgramm.AddRange(microcommands[11..17]);
                microProgramm.AddRange(microcommands[21..22]);
            }

            switch (name)
            {
                case ".":
                    microProgramm.AddRange(microcommands[0..9]);
                    break;
                case "drop":
                    microProgramm.AddRange(microcommands[2..9]);
                    break;
                case "+":
                    microProgramm.AddRange(microcommands[24..26]);
                    microProgramm.Add(microcommands[27]);
                    microProgramm.AddRange(microcommands[32..38]);
                    break;
                case "and":
                    microProgramm.AddRange(microcommands[24..26]);
                    microProgramm.Add(microcommands[28]);
                    microProgramm.AddRange(microcommands[32..38]);
                    break;
                case "or":
                    microProgramm.AddRange(microcommands[24..26]);
                    microProgramm.Add(microcommands[29]);
                    microProgramm.AddRange(microcommands[32..38]);
                    break;
                case "<":
                    microProgramm.AddRange(microcommands[24..26]);
                    microProgramm.Add(microcommands[30]);
                    microProgramm.AddRange(microcommands[32..38]);
                    break;
                case "-":
                    microProgramm.AddRange(microcommands[24..26]);
                    microProgramm.Add(microcommands[31]);
                    microProgramm.AddRange(microcommands[32..38]);
                    break;
                case "if":
                    microProgramm.AddRange(microcommands[40..44]);
                    break;
                case "do":
                    microProgramm.AddRange(microcommands[46..54]);
                    microProgramm.AddRange(microcommands[2..9]);
                    microProgramm.AddRange(microcommands[2..9]);
                    microProgramm.AddRange(microcommands[56..60]);
                    break;
                case "loop":
                    microProgramm.AddRange(microcommands[62..66]);
                    microProgramm.AddRange(microcommands[57..60]);
                    break;
                case "!":
                    microProgramm.AddRange(microcommands[68..72]);
                    break;
                case "swap":
                    microProgramm.AddRange(microcommands[73..111]);
                    break;
                case "?":
                    microProgramm.AddRange(microcommands[113..121]);
                    break;
            }

            return (isNeedToLoad, microProgramm.ToArray());
        }
    }

    public ControlUnit(string fileName, Memory actualMemory)
    {
        mainMemory = actualMemory;

        string[] forthProgramm = File.ReadAllLines(fileName);

        for (int i = startProgrammIndex; i < forthProgramm.Length; i++)
        {
            mainMemory.LoadToMemory(forthProgramm[i], i);
        }
        mainMemory.LoadToMemory(" "); //null pointer
    }

    public void Work()
    {


    }
}