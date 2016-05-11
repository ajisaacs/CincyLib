using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace CincyLib.PressBrake
{
    public class LowerTool
    {
        /// <summary>
        /// The name of the file
        /// </summary>
        public string Name;

        public string ToolName;
        public int Type;
        public double Length;
        public double Height;
        public double Clearance;
        public double MaxLoad;
        public double Angle;
        public double Radius;
        public double VOpening;
        public double Offset;

        public double BendRadius()
        {
            return VOpening * 0.15625;
        }

        public static LowerTool Load(string xmlpath)
        {
            var lowerTool = new LowerTool();
            var reader = XmlReader.Create(xmlpath);

            try
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "LowerTool":
                                lowerTool.Name = Path.GetFileNameWithoutExtension(xmlpath);
                                lowerTool.ToolName = reader.GetAttribute("ToolName");

                                int type;
                                int.TryParse(reader.GetAttribute("Type"), out type);
                                lowerTool.Type = type;

                                double length;
                                double.TryParse(reader.GetAttribute("Length"), out length);
                                lowerTool.Length = length;

                                double height;
                                double.TryParse(reader.GetAttribute("Height"), out height);
                                lowerTool.Height = height;

                                double clearance;
                                double.TryParse(reader.GetAttribute("Clearance"), out clearance);
                                lowerTool.Clearance = clearance;

                                double maxload;
                                double.TryParse(reader.GetAttribute("MaxLoad"), out maxload);
                                lowerTool.MaxLoad = maxload;

                                double angle;
                                double.TryParse(reader.GetAttribute("Angle"), out angle);
                                lowerTool.Angle = angle;

                                double radius;
                                double.TryParse(reader.GetAttribute("Radius"), out radius);
                                lowerTool.Radius = radius;

                                double vopening;
                                double.TryParse(reader.GetAttribute("VeeOpening"), out vopening);
                                lowerTool.VOpening = vopening;

                                double offset;
                                double.TryParse(reader.GetAttribute("Offset"), out offset);
                                lowerTool.Offset = offset;

                                break;
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                Debug.WriteLine("Error loading: " + xmlpath);
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }

            return lowerTool;
        }

        public void Print()
        {
            Console.WriteLine(Name);
            Console.WriteLine("  Length:      {0}", Length);
            Console.WriteLine("  Angle:       {0}", Angle);
            Console.WriteLine("  Radius:      {0}", Radius);
            Console.WriteLine("  V-Opening:   {0}", VOpening);
            Console.WriteLine("  Height:      {0}", Height);
            Console.WriteLine("  Clearance:   {0}", Clearance);
            Console.WriteLine("  MaxLoad:     {0}", MaxLoad);
            Console.WriteLine("  Offset:      {0}", Offset);
            Console.WriteLine("  Type:        {0}", Type);
            Console.WriteLine("  Bend Radius: {0}", BendRadius());
        }
    }
}
