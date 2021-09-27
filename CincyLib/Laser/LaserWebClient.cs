using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace CincyLib.Laser
{
    public class LaserWebClient
    {
        public readonly WebClient WebClient;

        private void ApplyCorrections(LaserStatus status)
        {
            if (status.FYIMessages.ToLower().Contains("laser high voltage is off"))
            {
                status.HighVoltage = HighVoltage.Off;
            }
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

        private string URL = null;

        public LaserStatus GetStatus(Stream stream)
        {
            try
            {
                var reader = new StreamReader(stream);
                var responseString = reader.ReadToEnd();
                reader.Close();

                var status = new LaserStatus();
                status.DateTimeUTC = DateTime.UtcNow.RoundDown(TimeSpan.FromSeconds(1));

                var doc = new XmlDocument();
                doc.LoadXml(responseString);

                if (doc.InnerText.Contains("Access is denied"))
                    return null;

                status.Program = doc.DocumentElement.SelectSingleNode("/Refresh/ProgramName").InnerText;
                status.SystemAlarms = doc.DocumentElement.SelectSingleNode("/Refresh/SystemAlarms").InnerText;
                status.LaserAlarms = doc.DocumentElement.SelectSingleNode("/Refresh/LaserAlarms").InnerText;
                status.FYIMessages = doc.DocumentElement.SelectSingleNode("/Refresh/FYIMessages").InnerText;

                int mode;
                var cncModeString = doc.DocumentElement.SelectSingleNode("/Refresh/CNCMode").InnerText;

                if (int.TryParse(cncModeString, out mode))
                    status.CNCMode = GetCNCMode(mode);

                int runStatus;
                var cncRunStatusString = doc.DocumentElement.SelectSingleNode("/Refresh/CNCRunStatus").InnerText;

                if (int.TryParse(cncRunStatusString, out runStatus))
                    status.RunStatus = GetCNCRunStatus(runStatus);

                int mains;
                var laserMainsString = doc.DocumentElement.SelectSingleNode("/Refresh/LaserMains").InnerText;

                if (int.TryParse(laserMainsString, out mains))
                    status.LaserMains = GetLaserMains(mains);

                int hv;
                var highVoltageString = doc.DocumentElement.SelectSingleNode("/Refresh/HighVoltage").InnerText;

                if (int.TryParse(highVoltageString, out hv))
                    status.HighVoltage = GetHighVoltage(hv);

                ApplyCorrections(status);

                return status;
            }
            catch { }

            return null;
        }

        public LaserStatus GetStatus(Uri uri)
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

                return GetStatus(response.GetResponseStream());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            return null;
        }

        public LaserStatus GetStatus(string url)
        {
            URL = url;
            try
            {
                var uri = new Uri(url);
                return GetStatus(uri);
            }
            catch { }

            return null;
        }
    }
}
