using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace CincyLib.PressBrake
{
    public class WebPanel
    {
        public string CurrentUser { get; set; }

        public string ProgramName { get; set; }

        public string ProgramPath { get; set; }

        public StrokeMode StrokeMode { get; set; }

        public string MaterialType { get; set; }

        public double Thickness { get; set; }

        public string[] SetupNotes { get; set; }

        public string[] CurrentFaults { get; set; }

        public int PartsCounter { get; set; }

        public int BatchSize { get; set; }

        public int BatchRemaining { get; set; }

        public int MachineStrokes { get; set; }

        public TimeSpan PowerOnTime { get; set; }

        public TimeSpan DriveOnTime { get; set; }

        public TimeSpan TotalCycleTime { get; set; }

        public TimeSpan LastCycleTime { get; set; }

        public TimeSpan AverageCycleTime { get; set; }

        public int SampleSize { get; set; }

        public double AveragePPM { get; set; }

        public double LastPPM { get; set; }

        public bool Update(Stream stream)
        {
            try
            {
                var reader = new WebPanelReader(this);
                return reader.Read(stream);
            }
            catch { }

            return false;
        }

        public bool Update(Uri uri)
        {
            try
            {
                var reader = new WebPanelReader(this);
                return reader.Read(uri);
            }
            catch { }

            return false;
        }

        public bool Update(string url)
        {
            try
            {
                var reader = new WebPanelReader(this);
                return reader.Read(new Uri(url));
            }
            catch { }

            return false;
        }
    }

    internal class WebPanelReader
    {
        public readonly WebPanel WebPanel;

        public WebPanelReader()
        {
            WebPanel = new WebPanel();
        }

        public WebPanelReader(WebPanel wp)
        {
            WebPanel = wp;
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

                WebPanel.CurrentUser = doc.DocumentElement.SelectSingleNode("/Refresh/CurrentUser").InnerText;
                WebPanel.ProgramName = doc.DocumentElement.SelectSingleNode("/Refresh/ProgramName").InnerText;
                WebPanel.ProgramPath = doc.DocumentElement.SelectSingleNode("/Refresh/PathName").InnerText;

                var mode = ReadInt(doc.DocumentElement.SelectSingleNode("/Refresh/StrokeMode").InnerText);
                WebPanel.StrokeMode = GetStrokeMode(mode);

                WebPanel.CurrentFaults = doc.DocumentElement.SelectSingleNode("/Refresh/CurrentFaults").InnerText.Replace("\r", "").Split('\n');
                WebPanel.MaterialType = doc.DocumentElement.SelectSingleNode("/Refresh/MatType").InnerText;
                WebPanel.Thickness = ReadDouble(doc.DocumentElement.SelectSingleNode("/Refresh/MatThickness").InnerText);
                
                WebPanel.SetupNotes = doc.DocumentElement.SelectSingleNode("/Refresh/SetupNotes").InnerText.Replace("\r", "").Split('\n');
                
                WebPanel.PartsCounter = ReadInt(doc.DocumentElement.SelectSingleNode("/Refresh/PartsCounter").InnerText);
                WebPanel.BatchSize = ReadInt(doc.DocumentElement.SelectSingleNode("/Refresh/BatchSize").InnerText);
                WebPanel.BatchRemaining = ReadInt(doc.DocumentElement.SelectSingleNode("/Refresh/BatchRemaining").InnerText);

                WebPanel.MachineStrokes = ReadInt(doc.DocumentElement.SelectSingleNode("/Refresh/MachineStrokes").InnerText);
                WebPanel.PowerOnTime = TimeSpan.FromSeconds(ReadDouble(doc.DocumentElement.SelectSingleNode("/Refresh/PowerOnTime").InnerText));
                WebPanel.DriveOnTime = TimeSpan.FromSeconds(ReadDouble(doc.DocumentElement.SelectSingleNode("/Refresh/DriveOnTime").InnerText));
                WebPanel.TotalCycleTime = TimeSpan.FromSeconds(ReadDouble(doc.DocumentElement.SelectSingleNode("/Refresh/TotalCycleTime").InnerText));

                WebPanel.AveragePPM = ReadDouble(doc.DocumentElement.SelectSingleNode("/Refresh/AveragePPM").InnerText);
                WebPanel.LastCycleTime = TimeSpan.FromSeconds(ReadDouble(doc.DocumentElement.SelectSingleNode("/Refresh/LastCycleTime").InnerText));
                WebPanel.AverageCycleTime = TimeSpan.FromSeconds(ReadDouble(doc.DocumentElement.SelectSingleNode("/Refresh/AverageCycleTime").InnerText));
                WebPanel.LastPPM = ReadDouble(doc.DocumentElement.SelectSingleNode("/Refresh/LastPPM").InnerText);
                WebPanel.SampleSize = ReadInt(doc.DocumentElement.SelectSingleNode("/Refresh/SampleSize").InnerText);

                return true;
            }
            catch { }

            return false;
        }

        public bool Read(Uri uri)
        {
            try
            {
                var refreshUri = new Uri(uri, "refresh.aspx");

                var refreshRequest = WebRequest.Create(refreshUri.ToString());
                refreshRequest.Method = "POST";

                using (var stream = refreshRequest.GetRequestStream())
                {
                    var postData = "<Refresh>" +
                        "<CurrentUser/><ProgramName/><PathName/><StrokeMode/><CurrentFaults/><MatType/><MatThickness/><SetupNotes/>" +
                        "<PartsCounter/><BatchSize/><BatchRemaining/>" +
                        "<MachineStrokes/><PowerOnTime/><DriveOnTime/><TotalCycleTime/>" +
                        "<AveragePPM/><LastCycleTime/><AverageCycleTime/><LastPPM/><SampleSize/>" +
                        "</Refresh>";

                    var byteData = Encoding.ASCII.GetBytes(postData);
                    stream.Write(byteData, 0, byteData.Length);
                }

                var refreshResponse = (HttpWebResponse)refreshRequest.GetResponse();

                return Read(refreshResponse.GetResponseStream());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        private static StrokeMode GetStrokeMode(int i)
        {
            switch (i)
            {
                case 0: return StrokeMode.Off;
                case 1: return StrokeMode.Setup;
                case 2: return StrokeMode.SingleStroke;
                default: return StrokeMode.ContinuousRun;
            }
        }

        private static int ReadInt(string s)
        {
            int i;
            return int.TryParse(s, out i) ? i : 0;
        }

        private static double ReadDouble(string s)
        {
            double d;
            return double.TryParse(s, out d) ? d : 0;
        }
    }

    public enum StrokeMode
    {
        Off,
        Setup,
        SingleStroke,
        ContinuousRun
    }
}
