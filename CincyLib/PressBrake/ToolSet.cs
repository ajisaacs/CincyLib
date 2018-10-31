using System;

namespace CincyLib.PressBrake
{
    public class ToolSet
    {
        private LowerTool lowerTool;
        private UpperTool upperTool;

        public ToolSet(LowerTool lt, UpperTool ut)
        {
            lowerTool = lt;
            upperTool = ut;
        }

        public bool IsValid()
        {
            if (upperTool.Angle > lowerTool.Angle)
                return false;

            return true;
        }

        public double BendRadius()
        {
            double r1 = lowerTool.DevelopedRadius();
            double r2 = upperTool.Radius;
            return r1 > r2 ? r1 : r2;
        }

        public double BendLength()
        {
            double r1 = lowerTool.Length;
            double r2 = upperTool.Length;
            return r1 > r2 ? r1 : r2;
        }

        public double MinFlangeLength()
        {
            var thickness = BendRadius();
            var radius = thickness;
            var outsideRadius = radius + thickness;
            var length2bend = lowerTool.VOpening * 0.5 + 0.0625;
            var bendlength = SheetMetal.BendLength(thickness, radius, 90.0, 0.42);

            return length2bend - (bendlength * 0.5) + outsideRadius;
        }

        public void Print()
        {
            Console.WriteLine("{0} / {1}", lowerTool.ToolName, upperTool.ToolName);
            Console.WriteLine("  Generates an inside radius of:             {0}", BendRadius().ToString("n3") + "\"");
            Console.WriteLine("  Usable on materials less than or equal to: {0}", BendRadius().ToString("n3") + "\"");
            Console.WriteLine("  Capable of bends greater than or equal to: {0}", lowerTool.Angle.ToString("n0") + " degrees");
            Console.WriteLine("  Capable of bends up to:                    {0} long", BendLength().ToString("n3") + "\"");
            Console.WriteLine("  Minimum 90 degree flange length:           {0}", MinFlangeLength().ToString("n3") + "\"");
        }
    }
}
