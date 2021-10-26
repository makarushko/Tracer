using System.Threading;
using TracerLib;

namespace Tracer
{
    class Program
    {
        static void Main(string[] args)
        {
            TracerLib.Tracer tracer = new TracerLib.Tracer();
            Foo f = new Foo(tracer);
            f.MyMethod();
            C c = new C(tracer);
            Thread thread = new Thread(new ThreadStart(c.M0));
            thread.Start();
            thread.Join();
            TraceResult traceResult = tracer.GetTraceResult();
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
            Thread.Sleep(30);
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