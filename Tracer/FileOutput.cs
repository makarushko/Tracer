using System.IO;

namespace Tracer
{
    public class FileOutput : IOutput
    {
        public string PathToSave
        {
            get;
            private set;
        }
        public void Output(string serializedResult)
        {
             File.WriteAllText(PathToSave, serializedResult);
        }

        public FileOutput(string path)
        {
            PathToSave = path;
        }
    }
}