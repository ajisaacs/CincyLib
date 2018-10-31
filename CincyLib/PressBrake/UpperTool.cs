using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CincyLib.PressBrake
{
    public class UpperTool
    {
        public UpperTool()
        {
            Segments = new List<ToolSegment>();
        }

        public List<ToolSegment> Segments { get; set; }

        public string FilePath { get; set; }
        public int Version { get; set; }
        public string ToolName { get; set; }
        public double Length { get; set; }
        public double Angle { get; set; }
        public double Radius { get; set; }
        public double Height { get; set; }
        public double Clearance { get; set; }
        public double MaxLoad { get; set; }
        public double Offset { get; set; }
        public int Type { get; set; }

        public bool IsSegmented
        {
            get { return Segments.Count > 0; }
        }

        public static UpperTool Load(string xmlpath)
        {
            var stream = File.OpenRead(xmlpath);
            var tool = Load(stream);
            tool.FilePath = xmlpath;
            return tool;
        }

        public static UpperTool Load(Stream stream)
        {
            var upperTool = new UpperTool();

            var xml = XDocument.Load(stream);
            var data = xml.Root.Element("UpperTool");

            upperTool.Version = data.Attribute("Version").ToInt();
            upperTool.ToolName = data.Attribute("ToolName")?.Value;
            upperTool.Type = data.Attribute("Type").ToInt();
            upperTool.Length = data.Attribute("Length").ToDouble();
            upperTool.Height = data.Attribute("Height").ToDouble();
            upperTool.Clearance = data.Attribute("Clearance").ToDouble();
            upperTool.MaxLoad = data.Attribute("MaxLoad").ToDouble();
            upperTool.Angle = data.Attribute("Angle").ToDouble();
            upperTool.Radius = data.Attribute("Radius").ToDouble();
            upperTool.Offset = data.Attribute("Radius").ToDouble();

            foreach (var item in data.Element("SegmentList").Descendants("ToolSeg"))
            {
                var seg = new ToolSegment();
                seg.Length = item.Attribute("Length").ToDouble();
                seg.Quantity = item.Attribute("Quantity").ToInt(1);

                upperTool.Segments.Add(seg);
            }

            var totalSegLength = upperTool.Segments.Sum(i => i.Length * i.Quantity);

            if (totalSegLength > 0)
                upperTool.Length = totalSegLength;

            return upperTool;
        }

        public void Print()
        {
            Console.WriteLine(ToolName);
            Console.WriteLine("  Length:    {0}", Length);
            Console.WriteLine("  Angle:     {0}", Angle);
            Console.WriteLine("  Radius:    {0}", Radius);
            Console.WriteLine("  Height:    {0}", Height);
            Console.WriteLine("  Clearance: {0}", Clearance);
            Console.WriteLine("  MaxLoad:   {0}", MaxLoad);
            Console.WriteLine("  Offset:    {0}", Offset);
            Console.WriteLine("  Type:      {0}", Type);
        }
    }

    public class ToolSegment
    {
        public double Length { get; set; }
        public int Quantity { get; set; }
    }
}
