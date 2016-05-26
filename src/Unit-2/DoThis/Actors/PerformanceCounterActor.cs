using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class PerformanceCounterActor : ReceiveActor
    {
        private readonly string _seriesName;
        private readonly Func<PerformanceCounter> _performanceCounterGenerator;
        private PerformanceCounter _performanceCounter;

        private readonly HashSet<IActorRef> _subscriptions = new HashSet<IActorRef>();
        private readonly ICancelable _cancelPublishing = new Cancelable(Context.System.Scheduler);

        public PerformanceCounterActor(string seriesName, Func<PerformanceCounter> performanceCounterGenerator)
        {
            _seriesName = seriesName;
            _performanceCounterGenerator = performanceCounterGenerator;
            DefaultState();
        }

        protected override void PreStart()
        {
            base.PreStart();
            _performanceCounter = _performanceCounterGenerator();

            var interval = TimeSpan.FromMilliseconds(250);
            var messageToSchedule = new ChartingActor.GetMetrics();
            Context.System.Scheduler.ScheduleTellRepeatedly(interval, interval, Self, messageToSchedule, Self, _cancelPublishing);
        }

        protected override void PostStop()
        {
            try
            {
                _cancelPublishing.Cancel(false);
                _performanceCounter.Dispose();
                _performanceCounter = null;
            }
            finally
            {
                base.PostStop();
            }
        }

        private void DefaultState()
        {
            Receive<ChartingActor.GetMetrics>(gm =>
                {
                    HandleGetMetrics();
                });

            Receive<ChartingActor.SubscriptCounter>(sub =>
                {
                    _subscriptions.Add(sub.Subscriber);
                });

            Receive<ChartingActor.UnsubscriptCounter>(unsub =>
                {
                    _subscriptions.Remove(unsub.Subscriber);
                });
        }

        private void HandleGetMetrics()
        {
            var metric = new ChartingActor.Metric(_seriesName, _performanceCounter.NextValue());
            foreach (var sub in _subscriptions)
            {
                sub.Tell(metric);
            }
        }
    }
}