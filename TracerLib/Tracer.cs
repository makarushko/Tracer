using System.Collections.Generic;
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
        
    }

    public class MethodInfo
    {
        
    }
    public class Tracer : ITracer
    {
        readonly TraceResult _trace = new TraceResult();

        public void StartTrace()
        {
            
        }

        public void StopTrace()
        {
            
        }
        public TraceResult GetTraceResult()
        {
            return _trace;
        }
    }
}