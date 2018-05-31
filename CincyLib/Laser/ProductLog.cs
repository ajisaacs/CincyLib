using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CincyLib.Laser
{
    public class ProductLog
    {
        private ProductLog()
        {
            Records = new List<Record>();
        }

        public List<Record> Records { get; set; }

        public static ProductLog Load(string file)
        {
            var log = new ProductLog();
            var reader = new StreamReader(file);

            Record curRecord = null;
            string line = null;

            while ((line = reader.ReadLine()) != null)
            {
                int splitIndex = line.IndexOf("Program");

                if (splitIndex == -1)
                    continue;

				var dateString = line.Remove(splitIndex - 1);

				if (dateString.Contains("..."))
					continue;

				curRecord = new Record();
				curRecord.Date = DateTime.Parse(dateString);

                var programString = line.Substring(splitIndex + 8);
                curRecord.ProgramFile = ReadBetweenQuotes(programString);

                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    var ws = LeadingWhitespaceCount(line);

                    if (ws != 4)
                        break;

                    if (line[4] == '\'')
                    {
                        var libFile = ReadBetweenQuotes(line);

                        if (libFile != null)
                            curRecord.LibraryFiles.Add(libFile);
                    }
                    else
                    {
                        var cutRecord = new CutRecord();

                        var cycleString = line.Substring(15, 13);
                        cutRecord.CycleTime = TimeSpan.Parse(cycleString);

                        var totalString = line.Substring(34);
                        cutRecord.TotalTime = TimeSpan.Parse(totalString);

                        curRecord.CutRecords.Add(cutRecord);
                    }
                }

				log.Records.Add(curRecord);
			}

			return log;
        }

        private static int LeadingWhitespaceCount(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != ' ')
                    return i;
            }

            return text.Length;
        }

        private static string ReadBetweenQuotes(string text)
        {
            var startIndex = text.IndexOf('\'');

            if (startIndex == -1)
                return null;

            var endIndex = text.IndexOf('\'', startIndex + 1);

            if (endIndex == -1)
                return null;

            return text.Substring(startIndex + 1, endIndex - startIndex - 1);
        }
    }

    public class Record
    {
        public Record()
        {
            CutRecords = new List<CutRecord>();
            LibraryFiles = new List<string>();
        }

        public DateTime Date { get; set; }
        public string ProgramFile { get; set; }
        public List<string> LibraryFiles { get; set; }
        public List<CutRecord> CutRecords { get; set; }

		public TimeSpan TotalCycleTime
		{
			get { return TimeSpan.FromTicks(CutRecords.Sum(r => r.CycleTime.Ticks)); }
		}
    }

    public class CutRecord
    {
        public TimeSpan CycleTime { get; set; }
        public TimeSpan TotalTime { get; set; }
    }
}
