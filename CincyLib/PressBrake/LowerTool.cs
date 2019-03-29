using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CincyLib.PressBrake
{
    public class LowerTool
    {
        public LowerTool()
        {
            Segments = new List<ToolSegment>();
        }

        public string FilePath { get; set; }
        public List<ToolSegment> Segments { get; set; }
        public int Version { get; set; }
        public string ToolName { get; set; }
        public int Type { get; set; }
        public double Length { get; set; }
        public double Height { get; set; }
        public double Clearance { get; set; }
        public double MaxLoad { get; set; }
        public double Angle { get; set; }
        public double Radius { get; set; }
        public double VOpening { get; set; }
        public double Offset { get; set; }

        public double DevelopedRadius()
        {
            return VOpening * 0.15625;
        }

        public bool IsSegmented
        {
            get { return Segments.Count > 0; }
        }

        public static LowerTool Load(string xmlpath)
        {
            var stream = File.OpenRead(xmlpath);
            var tool = Load(stream);
            tool.FilePath = xmlpath;
            return tool;
        }

        public static LowerTool Load(Stream stream)
        {
            var t = new LowerTool();

            var xml = XDocument.Load(stream);
            var data = xml.Root.Element("LowerTool");

            t.Version = data.Attribute("Version").ToInt();
            t.ToolName = data.Attribute("ToolName")?.Value;
            t.Length = data.Attribute("Length").ToDouble();
            t.Type = data.Attribute("Type").ToInt();
            t.Height = data.Attribute("Height").ToDouble();
            t.Clearance = data.Attribute("Clearance").ToDouble();
            t.MaxLoad = data.Attribute("MaxLoad").ToDouble();
            t.Angle = data.Attribute("Angle").ToDouble();
            t.Radius = data.Attribute("Radius").ToDouble();
            t.VOpening = data.Attribute("VeeOpening").ToDouble();

            foreach (var item in data.Element("SegmentList").Descendants("ToolSeg"))
            {
                var seg = new ToolSegment();
                seg.Length = item.Attribute("Length").ToDouble();
                seg.Quantity = item.Attribute("Quantity").ToInt(1);

                t.Segments.Add(seg);
            }

            var totalSegLength = t.Segments.Sum(i => i.Length * i.Quantity);

            if (totalSegLength > 0)
                t.Length = totalSegLength;

            return t;
        }

        public void Print()
        {
            Console.WriteLine(ToolName);
            Console.WriteLine("  Length:      {0}", Length);
            Console.WriteLine("  Angle:       {0}", Angle);
            Console.WriteLine("  Radius:      {0}", Radius);
            Console.WriteLine("  V-Opening:   {0}", VOpening);
            Console.WriteLine("  Height:      {0}", Height);
            Console.WriteLine("  Clearance:   {0}", Clearance);
            Console.WriteLine("  MaxLoad:     {0}", MaxLoad);
            Console.WriteLine("  Offset:      {0}", Offset);
            Console.WriteLine("  Type:        {0}", Type);
            Console.WriteLine("  Bend Radius: {0}", DevelopedRadius());
        }

        public override string ToString()
        {
            return $"{ToolName}, {Length} LG";
        }
    }
}
