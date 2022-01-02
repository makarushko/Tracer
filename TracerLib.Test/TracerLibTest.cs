using System;
using System.Diagnostics;
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
        public void TestDoubleMethodTime()
        {
            Tracer tracer = new Tracer();
            Foo f = new Foo(tracer);
            f.MyMethod();
            f.MyMethod();
            TraceResult traceResult = tracer.GetTraceResult();
            double time = traceResult.Threads[0].Method[0].Time - traceResult.Threads[0].Method[1].Time;
            
            Assert.IsTrue(traceResult.Threads[0].Method[0].Time > traceResult.Threads[0].Method[1].Time);
            Assert.IsTrue(time > 10);
        }
        
        [TestMethod]
        public void GetTraceResult_MyMethod_returnMyMethodInnerMethod()
        {
            Foo f = new Foo(_tracer);
            f.MyMethod();
            C c = new C(_tracer);
            Thread thread = new Thread(new ThreadStart(c.M0));
            thread.Start();
            thread.Join();
            _traceResult = _tracer.GetTraceResult();
            
            Assert.AreEqual("MyMethod",_traceResult.Threads[0].Method[0].Name);
            Assert.AreEqual("InnerMethod", _traceResult.Threads[0].Method[0].ChildMethod[0].Name);
            Assert.AreEqual("M1",_traceResult.Threads[1].Method[0].Name);
            Assert.AreEqual("M2",_traceResult.Threads[1].Method[1].Name);
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
        
        [TestMethod]
        public void Test_ExecutionTime()
        {
            Stopwatch stopwatch = new Stopwatch();
            Tracer tracer = new Tracer();

            stopwatch.Start();
            tracer.StartTrace();

            Thread.Sleep(1000);

            stopwatch.Stop();
            tracer.StopTrace();

            Assert.IsTrue(Math.Abs(stopwatch.ElapsedMilliseconds - tracer.GetTraceResult().Threads[0].Time) < 10);
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
                    _bar.InnerMethod();
                    Thread.Sleep(50);
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