using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;


namespace TracerLib
{
    [XmlRoot("root")]
    public class TraceResult
    {
        [XmlElement(ElementName = "thread")]
        public List<ThreadInfo> Threads { get; internal set; }
    }

    public class ThreadInfo
    {
        [XmlAttribute("id")]
        public int IdThread { get; set; }
        
        [XmlAttribute("time")]
        public double Time { get; set; }
        
        [XmlElement(ElementName = "method")]
        public List<MethodInfo> Method { get; set; }
    }

    public class MethodInfo
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        
        [XmlAttribute("class")]
        public string Class { get; set; }
        
        [XmlAttribute("time")]
        public double Time { get; set; }
        
        [XmlElement(ElementName = "method")]
        public List<MethodInfo> ChildMethod { get; internal set; }
    }
    public class Tracer : ITracer
    {
        readonly TraceResult _trace = new TraceResult();
        readonly ConcurrentDictionary<int, Stack<(Stopwatch,MethodInfo)>> _threadInfos = new ConcurrentDictionary<int, Stack<(Stopwatch,MethodInfo)>>();

        public void StartTrace()
        {
            Stopwatch stopwatch = new Stopwatch();
            MethodInfo methodInfo = new MethodInfo();
            
            stopwatch.Start();
            int id = Thread.CurrentThread.ManagedThreadId;

            if (_trace.Threads == null)
            {
                _trace.Threads = new List<ThreadInfo>();
            }

            if (_threadInfos.TryAdd(id, new Stack<(Stopwatch, MethodInfo)>()))
            {
                _trace.Threads.Add(new ThreadInfo() {IdThread = id});
            }
            StackFrame stackFrame = new StackFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            methodInfo.Name =   methodBase.Name;
            methodInfo.Class = !(methodBase.DeclaringType == null) ? methodBase.DeclaringType.Name : "Unknown";
            _threadInfos[id].Push((stopwatch,methodInfo));
        }

        public void StopTrace()
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            Stopwatch stopwatch;
            MethodInfo methodInfo;
            (stopwatch, methodInfo) = _threadInfos[id].Pop();
            methodInfo.Time = stopwatch.ElapsedMilliseconds;
            stopwatch.Stop();
            if (_threadInfos[id].Count > 0)
            {
                MethodInfo localMethodInfo;
                (_, localMethodInfo) = _threadInfos[id].Peek();
                if (localMethodInfo.ChildMethod == null)
                {
                    localMethodInfo.ChildMethod = new List<MethodInfo>();
                }
                localMethodInfo.ChildMethod.Add(methodInfo);
            }
            else
            {
                int index = _trace.Threads.FindIndex(thread => thread.IdThread == id);
                if (_trace.Threads[index].Method == null)
                {
                    _trace.Threads[index].Method = new List<MethodInfo>();
                }
                _trace.Threads[index].Method.Add(methodInfo);
                _trace.Threads[index].Time += methodInfo.Time;
            }
        }
        public TraceResult GetTraceResult()
        {
            return _trace;
        }
    }
}