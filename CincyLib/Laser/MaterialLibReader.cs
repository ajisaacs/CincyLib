using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CincyLib.Laser
{
    internal sealed class MaterialLibReader
    {
        public readonly MaterialLib MaterialLib;

        public MaterialLibReader()
        {
            MaterialLib = new MaterialLib();
        }

        public MaterialLibReader(MaterialLib materiallib)
        {
            MaterialLib = materiallib;
        }

        public void Read(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                MaterialLib.Path = file;
                Read(stream);
            }
        }

        public void Read(Stream stream)
        {
            var reader = new BinaryReader(stream);

            MaterialLib.Machine = reader.ReadString();
            MaterialLib.PierceDwell = Math.Round(reader.ReadSingle(), 4);

            reader.BaseStream.Seek(2, SeekOrigin.Current); // Unknown 2 bytes
            MaterialLib.PiercePower = reader.ReadInt16();
            MaterialLib.PierceFrequency = reader.ReadInt16();
            MaterialLib.PierceDutyCycle = reader.ReadInt16();

            var pierceType = reader.ReadByte();

            switch (pierceType)
            {
                case 0:
                    MaterialLib.PierceType = PierceType.NoPierce;
                    break;

                case 1:
                    MaterialLib.PierceType = PierceType.RampedPower;
                    break;
            }

            reader.BaseStream.Seek(1, SeekOrigin.Current); // Unknown 1 byte

            MaterialLib.PierceNozzleStandoffRampFrom = Math.Round(reader.ReadSingle(), 4);

            MaterialLib.RampedPierceCoolingTime = Math.Round(reader.ReadSingle(), 4);
            MaterialLib.UsePartCoolantOnPierce = Convert.ToBoolean(reader.ReadInt16());

            MaterialLib.PierceAssistGas = (AssistGasType)reader.ReadInt16();
            MaterialLib.PierceAssistGasPressure = reader.ReadInt16();
            MaterialLib.PreCutDwellSeconds = Math.Round(reader.ReadSingle(), 4);

            reader.BaseStream.Seek(2, SeekOrigin.Current); // Unknown 2 bytes
            MaterialLib.Power = reader.ReadInt16();
            MaterialLib.Frequency = reader.ReadInt16();
            MaterialLib.DutyCycle = reader.ReadInt16();
            MaterialLib.DynamicPowerControl = Convert.ToBoolean(reader.ReadInt16());
            MaterialLib.DPCFeedrate = reader.ReadInt16();
            MaterialLib.DPCMinPower = reader.ReadInt16();

            reader.BaseStream.Seek(4, SeekOrigin.Current); // Unknown 4 bytes
            MaterialLib.NozzleStandoff1 = Math.Round(reader.ReadSingle(), 4);
            MaterialLib.KerfWidth = Math.Round(reader.ReadSingle(), 4);

            reader.BaseStream.Seek(2, SeekOrigin.Current); // Unknown 2 bytes
            MaterialLib.AssistGas = (AssistGasType)reader.ReadInt16();
            MaterialLib.AssistGasPressure1 = reader.ReadInt16();
            MaterialLib.AssistGasPressure2 = reader.ReadInt16();
            MaterialLib.DynamicAssistGas = Convert.ToBoolean(reader.ReadInt16());

            reader.BaseStream.Seek(4, SeekOrigin.Current); // Unknown 4 bytes
            var steps = reader.ReadInt16();

            reader.BaseStream.Seek(4, SeekOrigin.Current); // Unknown 4 bytes
            MaterialLib.RampedPierceStartPower = reader.ReadInt16();

            MaterialLib.RampedPierceSteps = new RampedPierceStep[steps];


            for (int i = 0; i < steps; ++i)
            {
                MaterialLib.RampedPierceSteps[i] = new RampedPierceStep();
                MaterialLib.RampedPierceSteps[i].Time = Math.Round(reader.ReadSingle(), 4);
                MaterialLib.RampedPierceSteps[i].Power = reader.ReadInt16();
            }

            const int MAX_STEPS = 20;

            var remaining = MAX_STEPS - steps;
            reader.BaseStream.Seek(6 * remaining, SeekOrigin.Current);

            int length = reader.ReadByte();

            if (length == 0xFF)
            {
                var second = reader.ReadByte();
                var third = reader.ReadByte();

                length = length * third + second + third;
            }
            MaterialLib.Notes = Encoding.ASCII.GetString(reader.ReadBytes(length));

            reader.BaseStream.Seek(4, SeekOrigin.Current); // Unknown 4 bytes
            MaterialLib.Feedrate = reader.ReadInt16();

            reader.BaseStream.Seek(424, SeekOrigin.Current);
            MaterialLib.NozzleStandoff2 = Math.Round(reader.ReadSingle(), 4);
            MaterialLib.CutFocusNearField = Math.Round(reader.ReadSingle(), 4);
            MaterialLib.CutFocusFarField = Math.Round(reader.ReadSingle(), 4);
            MaterialLib.PierceFocusNearField = Math.Round(reader.ReadSingle(), 4);
            MaterialLib.PierceFocusFarField = Math.Round(reader.ReadSingle(), 4);

            // older files wont go this far...
            const int seek = 76;

            var hasExtendedInfo = reader.BaseStream.Length > reader.BaseStream.Position + seek;

            if (hasExtendedInfo)
            {
                reader.BaseStream.Seek(seek, SeekOrigin.Current);
                MaterialLib.Lens = (LensType)reader.ReadByte();

                reader.BaseStream.Seek(1, SeekOrigin.Current);
                MaterialLib.Nozzle = reader.ReadString();

                reader.BaseStream.Seek(4, SeekOrigin.Current);
                MaterialLib.PierceType = (PierceType)reader.ReadByte();
            }
        }
    }

    public enum LensType
    {
        /// <summary>
        /// 5.0" Lens
        /// </summary>
        _050 = 0,

        /// <summary>
        /// 7.5" Lens
        /// </summary>
        _075 = 1,

        /// <summary>
        /// 10.0" Lens
        /// </summary>
        _100 = 2,

        Any = 3
    }
}
