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

        public bool Read(string file)
        {
            Stream stream = null;
            MaterialLib.Name = Path.GetFileNameWithoutExtension(file);

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
            var success = false;

            try
            {
                var reader = new BinaryReader(stream);

                MaterialLib.Machine = reader.ReadString();
                MaterialLib.PierceDwell = reader.ReadSingle();

                reader.BaseStream.Seek(2, SeekOrigin.Current);
                MaterialLib.PiercePower = reader.ReadInt16();
                MaterialLib.PierceFrequency = reader.ReadInt16();
                MaterialLib.PierceDutyCycle = reader.ReadInt16();

                if (reader.ReadInt16() == 1)
                    MaterialLib.PierceType = PierceType.RampedPower;

                MaterialLib.PierceNozzleStandoffRampFrom = reader.ReadSingle();

                MaterialLib.RampedPierceCoolingTime = reader.ReadSingle();
                MaterialLib.UsePartCoolantOnPierce = Convert.ToBoolean(reader.ReadInt16());

                MaterialLib.PierceAssistGas = (AssistGasType)reader.ReadInt16();
                MaterialLib.PierceAssistGasPressure = reader.ReadInt16();

                reader.BaseStream.Seek(6, SeekOrigin.Current);
                MaterialLib.Power = reader.ReadInt16();
                MaterialLib.Frequency = reader.ReadInt16();
                MaterialLib.DutyCycle = reader.ReadInt16();
                MaterialLib.DynamicPowerControl = Convert.ToBoolean(reader.ReadInt16());
                MaterialLib.DPCFeedrate = reader.ReadInt16();
                MaterialLib.DPCMinPower = reader.ReadInt16();

                reader.BaseStream.Seek(4, SeekOrigin.Current);
                MaterialLib.NozzleStandoff1 = reader.ReadSingle();
                MaterialLib.KerfWidth = reader.ReadSingle();

                reader.BaseStream.Seek(2, SeekOrigin.Current);
                MaterialLib.AssistGas = (AssistGasType)reader.ReadInt16();
                MaterialLib.AssistGasPressure1 = reader.ReadInt16();
                MaterialLib.AssistGasPressure2 = reader.ReadInt16();
                MaterialLib.DynamicAssistGas = Convert.ToBoolean(reader.ReadInt16());

                reader.BaseStream.Seek(4, SeekOrigin.Current);
                var steps = reader.ReadInt16();

                reader.BaseStream.Seek(4, SeekOrigin.Current);
                MaterialLib.RampedPierceStartPower = reader.ReadInt16();

                MaterialLib.RampedPierceSteps = new RampedPierceStep[20];

                for (int i = 0; i < steps; ++i)
                {
                    MaterialLib.RampedPierceSteps[i] = new RampedPierceStep();
                    MaterialLib.RampedPierceSteps[i].Time = reader.ReadSingle();
                    MaterialLib.RampedPierceSteps[i].Power = reader.ReadInt16();
                }

                var remaining = MaterialLib.RampedPierceSteps.Length - steps;
                reader.BaseStream.Seek(6 * remaining, SeekOrigin.Current);

                int length = reader.ReadByte();

                if (length == 0xFF)
                {
                    var second = reader.ReadByte();
                    var third = reader.ReadByte();

                    length = length * third + second + third;
                }
                MaterialLib.Notes = Encoding.ASCII.GetString(reader.ReadBytes(length));

                reader.BaseStream.Seek(4, SeekOrigin.Current);
                MaterialLib.Feedrate = reader.ReadInt16();

                if (MaterialLib.PierceType != PierceType.RampedPower)
                {
                    if (reader.BaseStream.Length < reader.BaseStream.Position + 527)
                    {
                        MaterialLib.PierceType = MaterialLib.PierceDwell == 0
                            ? PierceType.NoPierce
                            : PierceType.FixedDwellTime;
                    }
                    else
                    {
                        reader.BaseStream.Seek(527, SeekOrigin.Current);
                        MaterialLib.PierceType = (PierceType)reader.ReadInt16();
                    }
                }

                success = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return success;
        }
    }
}
