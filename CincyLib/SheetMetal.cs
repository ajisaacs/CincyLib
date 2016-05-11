using System;

namespace CincyLib
{
    public static class SheetMetal
    {
        public static double BendLength(double thickness, double radius, double angle, double kfactor)
        {
            return (radius + thickness * kfactor) * 2.0 * Math.PI * angle / 360.0;
        }
    }
}
