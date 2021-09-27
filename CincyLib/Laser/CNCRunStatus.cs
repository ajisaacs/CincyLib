using System.ComponentModel;

namespace CincyLib.Laser
{
    public enum CNCRunStatus
    {
        [Description("No Program Loaded")]
        NoProgramLoaded,
        Stopped,
        [Description("Ready To Run")]
        ReadyToRun,
        Running,
        Finished,
        Unknown
    }
}
