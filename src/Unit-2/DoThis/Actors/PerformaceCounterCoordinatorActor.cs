using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class PerformaceCounterCoordinatorActor : ReceiveActor
    {
        #region Message Types

        public class Watch
        {
            public Watch(ChartingActor.CounterType counter)
            {
                this.Counter = counter;
            }

            public ChartingActor.CounterType Counter { get; private set; }
        }

        public class Unwatch
        {
            public Unwatch(ChartingActor.CounterType counter)
            {
                this.Counter = counter;
            }

            public ChartingActor.CounterType Counter { get; private set; }
        }

        #endregion

        private Dictionary<ChartingActor.CounterType, Func<PerformanceCounter>> CounterGenerators =
            new Dictionary<ChartingActor.CounterType, Func<PerformanceCounter>>()
            {
                {
                    ChartingActor.CounterType.Cpu,
                    () => new PerformanceCounter("Processor", "% Processor Time", "_Total", true)
                },
                {
                    ChartingActor.CounterType.Memory,
                    () => new PerformanceCounter("Memory", "% Committed Bytes In Use", true)
                },
                {
                    ChartingActor.CounterType.Disk,
                    () => new PerformanceCounter("LogicalDisk", "% Disk Time", "_Total", true)
                }
            };

        private Dictionary<ChartingActor.CounterType, Func<Series>> CounterSeriesGenerators =
            new Dictionary<ChartingActor.CounterType, Func<Series>>()
            {
                {
                    ChartingActor.CounterType.Cpu,
                    () => new Series("Cpu") { ChartType = SeriesChartType.SplineArea, Color = System.Drawing.Color.DarkGreen }
                },
                {
                    ChartingActor.CounterType.Memory,
                    () => new Series("Memory") { ChartType = SeriesChartType.FastLine, Color = System.Drawing.Color.MediumBlue }
                },
                {
                    ChartingActor.CounterType.Disk,
                    () => new Series("Disk") { ChartType = SeriesChartType.SplineArea, Color = System.Drawing.Color.DarkRed }
                }
            };

        private IActorRef _chartingActor;
        private Dictionary<ChartingActor.CounterType, IActorRef> _counterActors;

        public PerformaceCounterCoordinatorActor(IActorRef chartingActor)
        {
            _chartingActor = chartingActor;
            _counterActors = new Dictionary<ChartingActor.CounterType, IActorRef>();

            Receive<Watch>(watch => HandleWatch(watch));
            Receive<Unwatch>(unwatch => HandleUnwatch(unwatch));
        }

        private void HandleWatch(Watch watch)
        {
            IActorRef counterActor;
            if (!_counterActors.TryGetValue(watch.Counter, out counterActor))
            {
                var counterName = watch.Counter.ToString();
                var counterGenerator = CounterGenerators[watch.Counter];
                counterActor = Context.ActorOf(Props.Create(() => new PerformanceCounterActor(counterName, counterGenerator)));
                _counterActors.Add(watch.Counter, counterActor);
            }

            _chartingActor.Tell(new ChartingActor.AddSeries(CounterSeriesGenerators[watch.Counter]()));
            counterActor.Tell(new ChartingActor.SubscriptCounter(watch.Counter, _chartingActor));
        }

        private void HandleUnwatch(Unwatch unwatch)
        {
            IActorRef counterActor;
            if (!_counterActors.TryGetValue(unwatch.Counter, out counterActor))
            {
                return;
            }

            counterActor.Tell(new ChartingActor.UnsubscriptCounter(unwatch.Counter, _chartingActor));

            _chartingActor.Tell(new ChartingActor.RemoveSeries(unwatch.Counter.ToString()));
        }
    }
}
