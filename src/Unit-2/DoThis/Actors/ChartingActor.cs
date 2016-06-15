using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ChartingActor : ReceiveActor, IWithUnboundedStash
    {
        #region Messages

        public class InitializeChart
        {
            public InitializeChart(Dictionary<string, Series> initialSeries)
            {
                InitialSeries = initialSeries;
            }

            public Dictionary<string, Series> InitialSeries { get; private set; }
        }

        public class AddSeries
        {
            public AddSeries(Series serz)
            {
                this.Series = serz;
            }

            public Series Series { get; private set; }
        }

        public class RemoveSeries
        {
            public RemoveSeries(string serzName)
            {
                this.SeriesName = serzName;
            }

            public string SeriesName { get; private set; }
        }

        public class GetMetrics
        {
        }

        public class Metric
        {
            public Metric(string series, float counterValue)
            {
                this.Series = series;
                this.CounterValue = counterValue;
            }

            public string Series { get; private set; }
            public float CounterValue { get; private set; }
        }

        #endregion

        #region Performance Counter Management

        public enum CounterType
        {
            Cpu,
            Memory,
            Disk
        }

        public class SubscriptCounter
        {
            public SubscriptCounter(CounterType counter, IActorRef subscriber)
            {
                this.Counter = counter;
                this.Subscriber = subscriber;
            }

            public CounterType Counter { get; private set; }
            public IActorRef Subscriber { get; private set; }
        }

        public class UnsubscriptCounter
        {
            public UnsubscriptCounter(CounterType counter, IActorRef subscriber)
            {
                this.Counter = counter;
                this.Subscriber = subscriber;
            }

            public CounterType Counter { get; private set; }
            public IActorRef Subscriber { get; private set; }
        }

        public class TogglePause { }

        #endregion

        private readonly Chart _chart;
        private Dictionary<string, Series> _seriesIndex;
        private Button _pauseResumeBtn;

        public const int MaxPoints = 250;
        private int _xPosCounter = 0;

        public ChartingActor(Chart chart, Button pauseResumeBtn) : this(chart, pauseResumeBtn, new Dictionary<string, Series>())
        {
            ChartingState();
        }

        private ChartingActor(Chart chart, Button pauseResumeBtn, Dictionary<string, Series> seriesIndex)
        {
            _chart = chart;
            _seriesIndex = seriesIndex;
            _pauseResumeBtn = pauseResumeBtn;
        }

        private void ChartingState()
        {
            Receive<InitializeChart>(ic => HandleInitialize(ic));
            Receive<AddSeries>(addSerz => HandleAddSeries(addSerz));
            Receive<RemoveSeries>(rmSerz => HandleRemoveSeries(rmSerz));
            Receive<Metric>(metric => HandleMetrics(metric));

            Receive<TogglePause>(_ =>
            {
                SetPauseButtonText(true);
                BecomeStacked(PausedState);
            });
        }

        private void PausedState()
        {
            Receive<AddSeries>(addSerz => Stash.Stash());
            Receive<RemoveSeries>(rmSerz => Stash.Stash());
            Receive<Metric>(metric => { });

            Receive<TogglePause>(_ =>
            {
                SetPauseButtonText(false);
                UnbecomeStacked();
                Stash.UnstashAll();
            });
        }

        #region Individual Message Type Handlers

        private void HandleInitialize(InitializeChart ic)
        {
            if (ic.InitialSeries != null)
            {
                //swap the two series out
                _seriesIndex = ic.InitialSeries;
            }

            //delete any existing series
            _chart.Series.Clear();

            // set the axes up - Configure the chart's 2D area
            var area = _chart.ChartAreas[0];
            area.AxisX.IntervalType = DateTimeIntervalType.Number;
            area.AxisY.IntervalType = DateTimeIntervalType.Number;
            SetChartBoundaries();

            //attempt to render the initial chart
            if (_seriesIndex.Any())
            {
                foreach (var series in _seriesIndex)
                {
                    //force both the chart and the internal index to use the same names
                    series.Value.Name = series.Key;
                    _chart.Series.Add(series.Value);
                }
            }

            SetChartBoundaries();
        }

        private void HandleAddSeries(AddSeries addSerz)
        {
            var name = addSerz.Series.Name;

            if (!string.IsNullOrEmpty(name) && !_seriesIndex.ContainsKey(name))
            {
                _seriesIndex.Add(name, addSerz.Series);
                _chart.Series.Add(addSerz.Series);
                SetChartBoundaries();
            }
        }

        private void HandleRemoveSeries(RemoveSeries rmSerz)
        {
            var name = rmSerz.SeriesName;

            if (_seriesIndex.ContainsKey(name))
            {
                _seriesIndex.Remove(name);

                int seriesIndex = _chart.Series.IndexOf(name);
                if (seriesIndex >= 0)
                {
                    _chart.Series.RemoveAt(seriesIndex);
                }

                SetChartBoundaries();
            }
        }

        private void HandleMetrics(Metric metric)
        {
            if (string.IsNullOrEmpty(metric.Series))
            {
                return;
            }

            Series serz;
            if (!_seriesIndex.TryGetValue(metric.Series, out serz))
            {
                return;
            }

            // Add new XY point
            serz.Points.AddXY(_xPosCounter, metric.CounterValue);
            _xPosCounter++;

            // Remove the exess old XY points to make the chart scroll horizontally once it fills the whole X dimension
            while (serz.Points.Count > MaxPoints)
            {
                serz.Points.RemoveAt(0);
            }

            SetChartBoundaries();
        }

        #endregion

        private void SetChartBoundaries()
        {
            // List of all X,Y coordinate pairs
            var allPoints = _seriesIndex.Values.SelectMany(serz => serz.Points).ToList();

            if (allPoints.Count > 2)
            {
                var yValues = allPoints.SelectMany(point => point.YValues).ToList();

                double minAxisX = _xPosCounter - MaxPoints;
                double maxAxisX = _xPosCounter;
                double minAxisY = yValues.Count > 0 ? Math.Floor(yValues.Min()) : 0.0;
                double maxAxisY = yValues.Count > 0 ? Math.Ceiling(yValues.Max()) : 1.0;

                var area = _chart.ChartAreas[0];
                area.AxisX.Minimum = minAxisX;
                area.AxisX.Maximum = maxAxisX;
                area.AxisY.Minimum = minAxisY;
                area.AxisY.Maximum = maxAxisY;
            }
        }

        private void SetPauseButtonText(bool isPaused)
        {
            _pauseResumeBtn.Text = isPaused ? "Resume ->" : "Pause ||";
        }

        public IStash Stash
        {
            get;
            set;
        }
    }
}