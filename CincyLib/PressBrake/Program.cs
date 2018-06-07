using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CincyLib.PressBrake
{
    public class Program
    {
        public Program()
        {
            UpperToolSets = new List<ToolSetup>();
            LowerToolSets = new List<ToolSetup>();
            Steps = new List<Step>();
        }

        public int Version { get; set; }

        public string Name { get; set; }

        public double MatThick { get; set; }

		public MatType MatType { get; set; }

		public double KFactor { get; set; }

        public string TeachName { get; set; }

        public string PartName { get; set; }

        public string SetupNotes { get; set; }

        public string ProgNotes { get; set; }

        public bool RZEnabled { get; set; }

        public List<ToolSetup> UpperToolSets { get; set; }

        public List<ToolSetup> LowerToolSets { get; set; }

        public List<Step> Steps { get; set; }

        public static Program Read(string file)
        {
            var reader = new ProgramReader();
            var success=  reader.Read(file);
            return reader.Program;
        }

        public static Program Read(Stream stream)
        {
            var reader = new ProgramReader();
            var success = reader.Read(stream);
            return reader.Program;
        }
    }

	public enum MatType
	{
		MildSteel,
		HighStrengthSteel,
		Stainless,
		SoftAluminum,
		HardAluminum
	}

    public class ProgramReader
    {
        public Program Program { get; set; }

        public ProgramReader()
        {
            Program = new Program();
        }

        public bool Read(string file)
        {
            Stream stream = null;
            this.Program.Name = Path.GetFileNameWithoutExtension(file);

            var success = false;

            try
            {
                stream = File.OpenRead(file);
                success = Read(stream);
            }
            catch (SystemException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return success;
        }

        public bool Read(Stream stream)
        {
            var xml = XDocument.Load(stream);

            var data = xml.Root.Element("PressBrakeProgram");

			var version = data.Attribute("Version")?.Value;
			var matthick = data.Attribute("MatThick")?.Value;
			var mattype = GetMaterialType(data.Attribute("MatType")?.Value);
			var kfactor = data.Attribute("KFactor")?.Value;

			if (version != null)
			{
				int v;

				if (int.TryParse(version, out v))
					Program.Version = v;
			}

			if (matthick != null)
			{
				double t;

				if (double.TryParse(matthick, out t))
					Program.MatThick = t;
			}

			if (kfactor != null)
			{
				double k;

				if (double.TryParse(kfactor, out k))
					Program.KFactor = k;
			}

            Program.TeachName = data.Attribute("TeachName")?.Value;
            Program.PartName = data.Attribute("PartName")?.Value;
            Program.SetupNotes = data.Attribute("SetupNotes")?.Value;
            Program.ProgNotes = data.Attribute("ProgNotes")?.Value;
            var RZEnabled = int.Parse(data.Attribute("RZEnabled").Value);
            Program.RZEnabled = Convert.ToBoolean(RZEnabled);

            foreach (var item in data.Element("UpperToolSets").Descendants("ToolSetup"))
            {
                var setup = ReadToolSetup(item);
                Program.UpperToolSets.Add(setup);
            }

            foreach (var item in data.Element("LowerToolSets").Descendants("ToolSetup"))
            {
                var setup = ReadToolSetup(item);
                Program.LowerToolSets.Add(setup);
            }

            foreach (var item in data.Element("StepData").Descendants("Step"))
            {
                var step = ReadStep(item);
                Program.Steps.Add(step);
            }

            return true;
        }

        private ToolSetup ReadToolSetup(XElement x)
        {
            var setup = new ToolSetup();

            setup.Name = x.Attribute("Name").Value;
            setup.Id = x.Attribute("ID").ToInt();
            setup.Length = x.Attribute("Length").ToDouble();
			setup.StackedHolderType = x.Attribute("StackedHolderType").ToInt();
			setup.HolderHeight = x.Attribute("HolderHeight").ToDouble();

			foreach (var item in x.Descendants("SegEntry"))
            {
                var entry = new SegEntry();
                entry.SegValue = item.Attribute("SegValue").ToDouble();
                setup.Segments.Add(entry);
            }

            return setup;
        }

        private Step ReadStep(XElement x)
        {
            var step = new Step();

            step.RevMode = x.Attribute("RevMode").ToInt();
            step.RevTons = x.Attribute("RevTons").ToDouble();
            step.MaxTons = x.Attribute("MaxTons").ToDouble();
            step.RevAbsPos = x.Attribute("RevAbsPos").ToDouble();
            step.ActualAng = x.Attribute("ActualAng").ToDouble();
            step.BendLen = x.Attribute("BendLen").ToDouble();
            step.StrokeLen = x.Attribute("StrokeLen").ToDouble();
            step.UpperID = x.Attribute("UpperID").ToInt();
            step.LowerID = x.Attribute("LowerID").ToInt();
            step.SpdChgDwn = x.Attribute("SpdChgDwn").ToDouble();
            step.SpdChgUp = x.Attribute("SpdChgUp").ToDouble();
            step.FormSpeed = x.Attribute("FormSpeed").ToDouble();
            step.XLeft = x.Attribute("XLeft").ToDouble();
            step.XRight = x.Attribute("XRight").ToDouble();
            step.RLeft = x.Attribute("RLeft").ToDouble();
            step.RRight = x.Attribute("RRight").ToDouble();
            step.ZLeft = x.Attribute("ZLeft").ToDouble();
            step.ZRight = x.Attribute("ZRight").ToDouble();
            step.FLeft = x.Attribute("FLeft").ToDouble();
            step.FRight = x.Attribute("FRight").ToDouble();
            step.SSLeft = x.Attribute("SSLeft").ToDouble();
            step.SSRight = x.Attribute("SSRight").ToDouble();
            step.ReturnSpd = x.Attribute("ReturnSpd").ToDouble();
            step.SideFlgHeight = x.Attribute("SideFlgHeight").ToDouble();
            return step;
        }

		private MatType GetMaterialType(string value)
		{
			if (value == null)
				return MatType.MildSteel;

			int i;

			if (!int.TryParse(value, out i))
				return MatType.MildSteel;

			switch (i)
			{
				case 0:
					return MatType.MildSteel;
				case 1:
					return MatType.HighStrengthSteel;
				case 2:
					return MatType.Stainless;
				case 3:
					return MatType.SoftAluminum;
				case 4:
					return MatType.HardAluminum;
			}

			return MatType.MildSteel;
		}
    }

    static class Extensions
    {
        public static bool ToBool(this XAttribute a, bool defaultValue = false)
        {
            if (a == null || string.IsNullOrWhiteSpace(a.Value))
                return defaultValue;

            int intValue;

            if (!int.TryParse(a.Value, out intValue))
                return defaultValue;

            return Convert.ToBoolean(intValue);
        }

        public static int ToInt(this XAttribute a, int defaultValue = 0)
        {
            if (a == null || string.IsNullOrWhiteSpace(a.Value))
                return defaultValue;

            int intValue;

            if (!int.TryParse(a.Value, out intValue))
                return defaultValue;

            return intValue;
        }

        public static double ToDouble(this XAttribute a, double defaultValue = 0)
        {
            if (a == null || string.IsNullOrWhiteSpace(a.Value))
                return defaultValue;

            double d;

            if (!double.TryParse(a.Value, out d))
                return defaultValue;

            return d;
        }
    }

    public class ToolSetup
    {
        public ToolSetup()
        {
            Segments = new List<SegEntry>();
        }

        public string Name { get; set; }

        public int Id { get; set; }

        public double Length { get; set; }

		public int StackedHolderType { get; set; }

		public double HolderHeight { get; set; }

		public List<SegEntry> Segments { get; set; }
    }

    public class SegEntry
    {
        public double SegValue { get; set; }
    }

    public class Step
    {
        public int RevMode { get; set; }
        public double RevTons { get; set; }
        public double MaxTons { get; set; }
        public double RevAbsPos { get; set; }
        public double ActualAng { get; set; }
        public double BendLen { get; set; }
        public double StrokeLen { get; set; }
        public int UpperID { get; set; }
        public int LowerID { get; set; }
        public double SpdChgDwn { get; set; }
        public double SpdChgUp { get; set; }
        public double FormSpeed { get; set; }
        public double XLeft { get; set; }
        public double XRight { get; set; }
        public double RLeft { get; set; }
        public double RRight { get; set; }
        public double ZLeft { get; set; }
        public double ZRight { get; set; }
        public double FLeft { get; set; }
        public double FRight { get; set; }

        public double SSLeft { get; set; }
        public double SSRight { get; set; }
        public double ReturnSpd { get; set; }
        public double SideFlgHeight { get; set; }
    }
}
