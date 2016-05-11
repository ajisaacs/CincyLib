using System.Linq;

namespace CincyLib.Laser
{
    public class MaterialLib
    {
        public MaterialLib()
        {
            const int MaxSteps = 20;
            RampedPierceSteps = new RampedPierceStep[MaxSteps];
        }

        public string Name { get; set; }

        public string Machine { get; set; }

        #region Pierce

        public PierceType PierceType { get; set; }

        public float PierceDwell { get; set; }

        public int PiercePower { get; set; }

        public int PierceFrequency { get; set; }

        public int PierceDutyCycle { get; set; }

        public float PierceZHoldDistance { get; set; }

        public float PierceNozzleStandoffRampFrom { get; set; }

        //public float PierceNozzleStandoffRampTo { get; set; }

        public AssistGasType PierceAssistGas { get; set; }

        public int PierceAssistGasPressure { get; set; }

        public bool UsePartCoolantOnPierce { get; set; }

        public int RampedPierceStartPower { get; set; }

        public float RampedPierceCoolingTime { get; set; }

        public RampedPierceStep[] RampedPierceSteps;

        public float PierceTime()
        {
            switch (PierceType)
            {
                case PierceType.NoPierce:
                    return 0;

                case PierceType.FixedDwellTime:
                    return PierceDwell;

                case PierceType.RampedPower:
                    return RampedPierceSteps.Sum(step => step.Time) + RampedPierceCoolingTime;
            }

            return 0;
        }

        #endregion

        #region Cut

        public int Power { get; set; }

        public int Frequency { get; set; }

        public int DutyCycle { get; set; }

        public int Feedrate { get; set; }

        //public float PreCutDwell { get; set; } // seconds

        public float KerfWidth { get; set; } // inches

        //public float PowerBurstTime { get; set; } // seconds

        public bool DynamicPowerControl { get; set; }

        public bool DynamicAssistGas { get; set; }

        public bool UsePartCoolantOnCut { get; set; }

        public float NozzleStandoff1 { get; set; }

        /// <summary>
        /// Optional standoff (M45)
        /// </summary>
        public float NozzleStandoff2 { get; set; }

        public AssistGasType AssistGas { get; set; }

        public int AssistGasPressure1 { get; set; }

        /// <summary>
        /// Optional assist gas pressure (M67)
        /// </summary>
        public int AssistGasPressure2 { get; set; }

        #endregion

        /// <summary>
        /// Dynamic power control feedrate.
        /// </summary>
        public int DPCFeedrate { get; set; }

        /// <summary>
        /// Dynamic power control minimum power.
        /// </summary>
        public int DPCMinPower { get; set; }

        //public FocusingLensType FocusingLens { get; set; }

        //public NozzleTipType NozzleTip { get; set; }

        public string Notes { get; set; }

        public static MaterialLib Load(string libfile)
        {
            var reader = new MaterialLibReader();
            return reader.Read(libfile) ? reader.MaterialLib : null;
        }
    }
}
