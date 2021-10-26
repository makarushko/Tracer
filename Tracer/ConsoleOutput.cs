using System;

namespace Tracer
{
    public class ConsoleOutput : IOutput
    {
        public void Output(string String)
        {
            Console.WriteLine(String);
        }
    }
}