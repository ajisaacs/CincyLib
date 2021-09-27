using System;

namespace CincyLib.Laser
{
    public class LaserStatus
    {
        public DateTime DateTimeUTC { get; internal set; }

        public string Program { get; internal set; }

        public CNCMode CNCMode { get; internal set; }

        public CNCRunStatus RunStatus { get; internal set; }

        public LaserMains LaserMains { get; internal set; }

        public HighVoltage HighVoltage { get; internal set; }

        public string SystemAlarms { get; internal set; }

        public string LaserAlarms { get; internal set; }

        public string FYIMessages { get; internal set; }

        public bool IsDifferentThan(LaserStatus laserStatus)
        {
            if (laserStatus == null)
                return true;

            if (Program != laserStatus.Program)
                return true;

            if (CNCMode != laserStatus.CNCMode)
                return true;

            if (RunStatus != laserStatus.RunStatus)
                return true;

            if (LaserMains != laserStatus.LaserMains)
                return true;

            if (HighVoltage != laserStatus.HighVoltage)
                return true;

            if (SystemAlarms != laserStatus.SystemAlarms)
                return true;

            if (LaserAlarms != laserStatus.LaserAlarms)
                return true;

            if (FYIMessages != laserStatus.FYIMessages)
                return true;

            return false;
        }
    }
}
