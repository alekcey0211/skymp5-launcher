using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using UpdatesClient.Modules.Downloader.Models;

namespace UpdatesClient.Modules.Downloader.UI
{
    /// <summary>
    /// Логика взаимодействия для ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : UserControl
    {
        private readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        private ProgressBarModel model;

        public double Value { get; set; }

        //Bytes
        public long Size { get; set; }
        public long Downloaded { get; set; }
        public bool Started { get; private set; } = false;

        //KB/sec
        private long Speed;
        //Sec
        private long NeedTime;

        private Stopwatch stopwatch;
        private readonly MovingAverage movingAverage = new MovingAverage(16);
        private readonly MovingAverage movingAverageTime = new MovingAverage(16);

        public ProgressBar()
        {
            InitializeComponent();
            Init(false);
        }

        public void Init(bool isIndeterminate, string text = null)
        {
            model = new ProgressBarModel
            {
                IsIndeterminate = isIndeterminate
            };
            if (!string.IsNullOrEmpty(text) && isIndeterminate) model.Progress = text;

            grid.DataContext = model;
        }
        public void Start()
        {
            stopwatch = new Stopwatch();
            Started = true;
            stopwatch.Start();
        }
        public void Update(long downloaded)
        {
            if (Started && Size != 0)
            {
                long timeChange = stopwatch.ElapsedMilliseconds; //ms
                Value = downloaded / (Size / 100.0);
                if (timeChange < 50) return;
                stopwatch.Restart();

                if (timeChange != 0)
                {
                    movingAverage.ComputeAverage((downloaded - Downloaded) / timeChange);
                    Speed = (long)movingAverage.Average * 1000; // kb/ms

                    if (Speed != 0) movingAverageTime.ComputeAverage((Size - Downloaded) / Speed);
                    NeedTime = (long)movingAverageTime.Average; //Sec
                }
                Downloaded = downloaded;


                model.FProgress = (float)Value;
                model.Speed = $"({SizeSuffix(Speed, 0)}/s)";
                model.Time = $"{TimeSuffix(NeedTime)}";
            }
        }
        public void Stop()
        {
            stopwatch.Reset();
            Started = false;
            Value = 100;
            Size = 0;
            Downloaded = 0;
            Speed = 0;
            NeedTime = 0;

            model.FProgress = 100;
            model.Speed = $"(0 MB/s)";
            model.Time = $"00:00:00";
        }

        private string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
        private string TimeSuffix(long value)
        {
            if (value < 0) { return "-" + TimeSuffix(-value); }
            if (value == 0) return "00:00:00";

            long m = value / 60;
            long h = m / 60;

            long s = value % 60;
            m %= 60;

            return $"{h:00}:{m:00}:{s:00}";
        }
    }

    public class MovingAverage
    {
        private readonly Queue<decimal> samples = new Queue<decimal>();
        private readonly int windowSize = 32;
        private decimal sampleAccumulator;
        public decimal Average { get; private set; }

        public MovingAverage(int size = 32)
        {
            windowSize = size;
        }

        /// <summary>
        /// Computes a new windowed average each time a new sample arrives
        /// </summary>
        /// <param name="newSample"></param>
        public void ComputeAverage(decimal newSample)
        {
            sampleAccumulator += newSample;
            samples.Enqueue(newSample);

            if (samples.Count > windowSize)
            {
                sampleAccumulator -= samples.Dequeue();
            }

            Average = sampleAccumulator / samples.Count;
        }
    }
}
