using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ChartingActor : ReceiveActor
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

        #endregion

        private readonly Chart _chart;
        private Dictionary<string, Series> _seriesIndex;

        public ChartingActor(Chart chart) : this(chart, new Dictionary<string, Series>())
        {
            Receive<InitializeChart>(ic => HandleInitialize(ic));
            Receive<AddSeries>(addSerz => HandleAddSeries(addSerz));
        }

        public ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex)
        {
            _chart = chart;
            _seriesIndex = seriesIndex;
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
        }

        private void HandleAddSeries(AddSeries addSerz)
        {
            var name = addSerz.Series.Name;

            if (!string.IsNullOrEmpty(name) && !_seriesIndex.ContainsKey(name))
            {
                _seriesIndex.Add(name, addSerz.Series);
                _chart.Series.Add(addSerz.Series);
            }
        }

        #endregion
    }
}
