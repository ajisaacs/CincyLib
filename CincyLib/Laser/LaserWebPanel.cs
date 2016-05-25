using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace CincyLib.Laser
{
    public class LaserWebPanel
    {
        public string CurrentProgram { get; set; }

        public CNCMode CurrentMode { get; set; }

        public CNCRunStatus CurrentStatus { get; set; }

        public LaserMains LaserMains { get; set; }

        public HighVoltage HighVoltage { get; set; }

        public string[] SystemAlarms { get; set; }

        public string[] LaserAlarms { get; set; }

        public string[] FYIMessages { get; set; }

        public bool Update(Stream stream)
        {
            try
            {
                var reader = new LaserWebPanelReader(this);
                return reader.Read(stream);
            }
            catch { }

            return false;
        }

        public bool Update(Uri uri)
        {
            try
            {
                var reader = new LaserWebPanelReader(this);
                return reader.Read(uri);
            }
            catch { }

            return false;
        }

        public bool Update(string url)
        {
            try
            {
                var reader = new LaserWebPanelReader(this);
                return reader.Read(new Uri(url));
            }
            catch { }

            return false;
        }
    }

    internal class LaserWebPanelReader
    {
        public readonly LaserWebPanel LaserWebPanel;

        public LaserWebPanelReader()
        {
            LaserWebPanel = new LaserWebPanel();
        }

        public LaserWebPanelReader(LaserWebPanel lwp)
        {
            LaserWebPanel = lwp;
        }

        public bool Read(Stream stream)
        {
            try
            {
                var reader = new StreamReader(stream);
                var responseString = reader.ReadToEnd();
                reader.Close();

                var doc = new XmlDocument();
                doc.LoadXml(responseString);

                LaserWebPanel.CurrentProgram = doc.DocumentElement.SelectSingleNode("/Refresh/ProgramName").InnerText;
                LaserWebPanel.SystemAlarms = doc.DocumentElement.SelectSingleNode("/Refresh/SystemAlarms").InnerText.Replace("\r", "").Split('\n');
                LaserWebPanel.LaserAlarms = doc.DocumentElement.SelectSingleNode("/Refresh/LaserAlarms").InnerText.Replace("\r", "").Split('\n');
                LaserWebPanel.FYIMessages = doc.DocumentElement.SelectSingleNode("/Refresh/FYIMessages").InnerText.Replace("\r", "").Split('\n');

                int mode;
                var cncModeString = doc.DocumentElement.SelectSingleNode("/Refresh/CNCMode").InnerText;

                if (int.TryParse(cncModeString, out mode))
                    LaserWebPanel.CurrentMode = GetCNCMode(mode);

                int status;
                var cncRunStatusString = doc.DocumentElement.SelectSingleNode("/Refresh/CNCRunStatus").InnerText;

                if (int.TryParse(cncRunStatusString, out status))
                    LaserWebPanel.CurrentStatus = GetCNCRunStatus(status);

                int mains;
                var laserMainsString = doc.DocumentElement.SelectSingleNode("/Refresh/LaserMains").InnerText;

                if (int.TryParse(laserMainsString, out mains))
                    LaserWebPanel.LaserMains = GetLaserMains(mains);

                int hv;
                var highVoltageString = doc.DocumentElement.SelectSingleNode("/Refresh/HighVoltage").InnerText;

                if (int.TryParse(highVoltageString, out hv))
                    LaserWebPanel.HighVoltage = GetHighVoltage(hv);

                ApplyCorrections(LaserWebPanel);

                return true;
            }
            catch { }

            return false;
            
        }

        private void ApplyCorrections(LaserWebPanel lwp)
        {
            foreach (var msg in lwp.FYIMessages)
            {
                if (msg.ToLower().Contains("laser high voltage is off"))
                {
                    lwp.HighVoltage = HighVoltage.Off;
                    break;
                }
            }
        }

        public bool Read(Uri uri)
        {
            try
            {
                var refreshUri = new Uri(uri, "refresh.aspx");
                var request = WebRequest.Create(refreshUri.ToString());
                request.Method = "POST";

                using (var stream = request.GetRequestStream())
                {
                    var postData = "<Refresh><CNCMode/><CNCRunStatus/><ProgramName/><SystemAlarms/><LaserAlarms/><FYIMessages/><LaserMains/><HighVoltage/></Refresh>";
                    var byteData = Encoding.ASCII.GetBytes(postData);
                    stream.Write(byteData, 0, byteData.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                
                return Read(response.GetResponseStream());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        private static CNCMode GetCNCMode(int i)
        {
            switch (i)
            {
                case 0: return CNCMode.Auto;
                case 1: return CNCMode.Jog;
                case 2: return CNCMode.Home;
                default: return CNCMode.Auto;
            }
        }

        private static CNCRunStatus GetCNCRunStatus(int i)
        {
            switch (i)
            {
                case 0: return CNCRunStatus.NoProgramLoaded;
                case 1: return CNCRunStatus.Stopped;
                case 2: return CNCRunStatus.ReadyToRun;
                case 3: return CNCRunStatus.Running;
                case 4: return CNCRunStatus.Finished;
                case 5: return CNCRunStatus.Unknown;
                default: return CNCRunStatus.NoProgramLoaded;
            }
        }

        private static LaserMains GetLaserMains(int i)
        {
            switch (i)
            {
                case 0: return LaserMains.Locked;
                case 1: return LaserMains.Off;
                case 2: return LaserMains.On;
                default: return LaserMains.Off;
            }
        }

        private static HighVoltage GetHighVoltage(int i)
        {
            switch (i)
            {
                case 0: return HighVoltage.Locked;
                case 1: return HighVoltage.Off;
                case 2: return HighVoltage.On;
                default: return HighVoltage.Off;
            }
        }
    }

    public enum CNCMode
    {
        Auto,
        Jog,
        Home
    }

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

    public enum LaserMains
    {
        Locked,
        Off,
        On
    }

    public enum HighVoltage
    {
        Locked,
        Off,
        On
    }
}
