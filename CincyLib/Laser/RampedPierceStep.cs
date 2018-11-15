
namespace CincyLib.Laser
{
    public struct RampedPierceStep
    {
        public double Time { get; set; }

        public int Power { get; set; }

        public override string ToString()
        {
            return string.Format("[RampedPierceStep: Time:{0}, Power:{1}", Time, Power);
        }
    }
}
