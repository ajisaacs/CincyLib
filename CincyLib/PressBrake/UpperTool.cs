using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace CincyLib.PressBrake
{
    public class UpperTool
    {
        /// <summary>
        /// The name of the file
        /// </summary>
        public string Name;

        public string ToolName;
        public double Length;
        public double Angle;
        public double Radius;
        public double Height;
        public double Clearance;
        public double MaxLoad;
        public double Offset;
        public int Type;


        public static UpperTool Load(string xmlpath)
        {
            var upperTool = new UpperTool();
            var reader = XmlReader.Create(xmlpath);

            try
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "UpperTool":
                                upperTool.Name = Path.GetFileNameWithoutExtension(xmlpath);
                                upperTool.ToolName = reader.GetAttribute("ToolName");

                                int type;
                                int.TryParse(reader.GetAttribute("Type"), out type);
                                upperTool.Type = type;

                                double length;
                                double.TryParse(reader.GetAttribute("Length"), out length);
                                upperTool.Length = length;

                                double height;
                                double.TryParse(reader.GetAttribute("Height"), out height);
                                upperTool.Height = height;

                                double clearance;
                                double.TryParse(reader.GetAttribute("Clearance"), out clearance);
                                upperTool.Clearance = clearance;

                                double maxload;
                                double.TryParse(reader.GetAttribute("MaxLoad"), out maxload);
                                upperTool.MaxLoad = maxload;

                                double angle;
                                double.TryParse(reader.GetAttribute("Angle"), out angle);
                                upperTool.Angle = angle;

                                double radius;
                                double.TryParse(reader.GetAttribute("Radius"), out radius);
                                upperTool.Radius = radius;

                                double offset;
                                double.TryParse(reader.GetAttribute("Offset"), out offset);
                                upperTool.Offset = offset;

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

            return upperTool;
        }

        public void Print()
        {
            Console.WriteLine(Name);
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
}
