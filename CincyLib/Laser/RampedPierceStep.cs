
namespace CincyLib.Laser
{
    public struct RampedPierceStep
    {
        public float Time;

        public int Power;

        public override string ToString()
        {
            return string.Format("[RampedPierceStep: Time:{0}, Power:{1}", Time, Power);
        }
    }
}
