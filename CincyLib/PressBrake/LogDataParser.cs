using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace CincyLib.PressBrake
{
    public class LogDataParser
    {
        public List<LogEvent> GetEvents(string filepath)
        {
            var xml = File.ReadAllText(filepath);

            xml = xml.Insert(0, "<LogFile>");
            xml += "</LogFile>";

            var doc = XDocument.Parse(xml);
            var elem = doc.Element("LogFile");

            var events = new List<LogEvent>();

            foreach (var e in elem.Elements())
            {
                Console.WriteLine(e.Name);

                var name = e.Name.ToString();

                switch (name)
                {
                    case "ProgramStart":
                        events.Add(ReadProgramStart(e));
                        break;

                    case "ProgramStop":
                        events.Add(ReadProgramStop(e));
                        break;

                    case "Fault":
                        events.Add(ReadFault(e));
                        break;
                }
            }

            return events;
        }

        static ProgramStart ReadProgramStart(XElement e)
        {
            var programStart = new ProgramStart();
            programStart.DateTime = e.Attribute("DateTime").ToDateTime();
            programStart.ProgramName = e.FirstNode.NodeType == System.Xml.XmlNodeType.Text ? e.FirstNode.ToString().Trim() : null;
            programStart.RamGageMode = e.Element("RamGageMode")?.Value;
            programStart.UpperTool = e.Element("UpperTool")?.Value;
            programStart.LowerTool = e.Element("LowerTool")?.Value;

            return programStart;
        }

        static ProgramStop ReadProgramStop(XElement e)
        {
            var programStop = new ProgramStop();
            programStop.DateTime = e.Attribute("DateTime").ToDateTime();
            programStop.ProgramName = e.FirstNode.NodeType == System.Xml.XmlNodeType.Text ? e.FirstNode.ToString().Trim() : null;
            programStop.TotalPartCounter = e.Element("TotalPartCounter").ToIntOrNull();
            programStop.CurrentPartCounter = e.Element("CurrentPartCounter").ToIntOrNull();
            programStop.BatchCounter = e.Element("BatchCounter").ToIntOrNull();
            programStop.TotalMachineStrokes = e.Element("TotalMachineStrokes").ToIntOrNull();
            programStop.CurrentMachineStrokes = e.Element("CurrentMachineStrokes").ToIntOrNull();
            programStop.RunTime = e.Element("RunTime").ToTimeSpan();
            programStop.TotalCycleTime = e.Element("TotalCycleTime").ToDoubleOrNull();
            programStop.CurrentCycleTime = e.Element("CurrentCycleTime").ToDoubleOrNull();
            programStop.CurrentCycleTimeSec = e.Element("CurrentCycleTimeSec").ToDoubleOrNull();
            programStop.MainDriveOnTime = e.Element("MainDriveOnTime").ToDoubleOrNull();
            programStop.PowerOnTime = e.Element("PowerOnTime").ToDoubleOrNull();

            return programStop;
        }

        static Fault ReadFault(XElement e)
        {
            var fault = new Fault();
            fault.DateTime = e.Attribute("DateTime").ToDateTime();
            fault.Message = e.Value;

            return fault;
        }
    }

    public class LogEvent
    {
        public DateTime? DateTime { get; set; }
    }

    public class ProgramStart : LogEvent
    {
        public string ProgramName { get; set; }
        public string RamGageMode { get; set; }
        public string LowerTool { get; internal set; }
        public string UpperTool { get; internal set; }
    }

    public class ProgramStop : LogEvent
    {
        public string ProgramName { get; set; }
        public int? TotalPartCounter { get; set; }
        public int? CurrentPartCounter { get; set; }
        public int? BatchCounter { get; set; }
        public int? TotalMachineStrokes { get; set; }
        public int? CurrentMachineStrokes { get; internal set; }
        public TimeSpan? RunTime { get; set; }
        public double? TotalCycleTime { get; internal set; }
        public double? CurrentCycleTime { get; internal set; }
        public double? PowerOnTime { get; internal set; }
        public double? MainDriveOnTime { get; internal set; }
        public double? CurrentCycleTimeSec { get; internal set; }
    }

    public class Fault : LogEvent
    {
        public string Message { get; set; }
    }
}
