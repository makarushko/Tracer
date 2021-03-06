using System.Threading;
using TracerLib;

namespace Tracer
{
    class Program
    {
        static void Main()
        {
            TracerLib.Tracer tracer = new TracerLib.Tracer();
            Foo f = new Foo(tracer);
            
            f.MyMethod();
            
            C c = new C(tracer);
            Thread thread = new Thread(new ThreadStart(c.M0));
            thread.Start();
            thread.Join();
            
            TraceResult traceResult = tracer.GetTraceResult();
            ConsoleOutput consoleOutput = new ConsoleOutput();
            
            FileOutput fileOutputJson = new FileOutput("C:/Users/Anna/RiderProjects/Tracer/Tracer/Files/file.json");
            Json_Serializer json = new Json_Serializer();
            consoleOutput.Output(json.Serialize(traceResult));
            fileOutputJson.Output(json.Serialize(traceResult));
            
            FileOutput fileOutputXml = new FileOutput("C:/Users/Anna/RiderProjects/Tracer/Tracer/Files/file.xml");
            Xml_Serializer xml = new Xml_Serializer();
            consoleOutput.Output(xml.Serialize(traceResult));
            fileOutputXml.Output(xml.Serialize(traceResult));
        }
    }
    
    public class Foo
    {
        private Bar _bar;
        private ITracer _tracer;

        internal Foo(ITracer tracer)
        {
            _tracer = tracer;
            _bar = new Bar(_tracer);
        }
    
        public void MyMethod()
        {
            _tracer.StartTrace();
            Thread.Sleep(50);
            _bar.InnerMethod();
            _tracer.StopTrace();
        }
    }

    public class Bar
    {
        private ITracer _tracer;

        internal Bar(ITracer tracer)
        {
            _tracer = tracer;
        }
    
        public void InnerMethod()
        {
            _tracer.StartTrace();
            Thread.Sleep(50);
            _tracer.StopTrace();
        }
    }
    
    public class C
    {
        private ITracer _tracer;
    
        public C(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void M0()
        {
            M1();
            M2();
        }
    
        private void M1()
        {
            _tracer.StartTrace();
            Thread.Sleep(100);
            _tracer.StopTrace();
        }
    
        private void M2()
        {
            _tracer.StartTrace();
            Thread.Sleep(200);
            _tracer.StopTrace();
        }
    }
}