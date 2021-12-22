using System.IO;

namespace Tracer
{
    public class FileOutput : IOutput
    {
        private string PathToSave { get; }
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