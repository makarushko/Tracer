using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace TracerLib.Test
{
    [TestClass]
    
    public class TracerLibTest
    {
        private Tracer _tracer;
        private TraceResult _traceResult;

        [TestInitialize]

        public void Setup()
        {
            _tracer = new Tracer();
            _traceResult = new TraceResult();
        }
        
        [TestMethod]
        public void TestThreadId()
        {
           _tracer.StartTrace();
           _tracer.StopTrace();
            _traceResult = _tracer.GetTraceResult();
            
            Assert.AreEqual(Thread.CurrentThread.ManagedThreadId,_traceResult.Threads[0].IdThread);
        }
        
        [TestMethod]
        public void TestThreadAndSingleMethodTime()
        {
            Foo f = new Foo(_tracer);
            
            f.MyMethod();
            _traceResult = _tracer.GetTraceResult();
            
            Assert.AreEqual(_traceResult.Threads[0].Time,_traceResult.Threads[0].Method[0].Time);
        }
        
        [TestMethod]
        public void GetTraceResult_MyMethod_returnMyMethodInnerMethod()
        {
            Foo f = new Foo(_tracer);
            f.MyMethod();
            _traceResult = _tracer.GetTraceResult();
            
            Assert.AreEqual("MyMethod",_traceResult.Threads[0].Method[0].Name);
            Assert.AreEqual("InnerMethod", _traceResult.Threads[0].Method[0].ChildMethod[0].Name);
        }
        
        [TestMethod]
        public void StopTrace_ThreadCount_return2()
        {
            Foo f = new Foo(_tracer);
            f.MyMethod();
            C c = new C(_tracer);
            Thread thread = new Thread(c.M0);
            thread.Start();
            thread.Join();
            _traceResult = _tracer.GetTraceResult();
            
            Assert.AreEqual(2, _traceResult.Threads.Count);
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
}