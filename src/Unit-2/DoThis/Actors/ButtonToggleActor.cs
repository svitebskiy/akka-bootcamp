using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Akka.Actor;

namespace ChartApp.Actors
{
    public class ButtonToggleActor : ReceiveActor
    {
        #region Message types

        public class Toggle { }

        #endregion

        private readonly ChartingActor.CounterType _counterType;
        private readonly Button _button;
        private readonly IActorRef _coordinatorActor;

        public ButtonToggleActor(IActorRef coordinatorActor, Button button, ChartingActor.CounterType counterType, bool isToggledOn)
        {
            _coordinatorActor = coordinatorActor;
            _button = button;
            _counterType = counterType;

            if (isToggledOn)
            {
                OnState();
            }
            else
            {
                OffState();
            }
        }

        private void OffState()
        {
            Receive<Toggle>(_ =>
            {
                _button.Text = FormatButtonName(_counterType, true);
                _coordinatorActor.Tell(new PerformaceCounterCoordinatorActor.Watch(_counterType));
                Become(OnState);
            });
        }

        private void OnState()
        {
            Receive<Toggle>(_ =>
            {
                _button.Text = FormatButtonName(_counterType, false);
                _coordinatorActor.Tell(new PerformaceCounterCoordinatorActor.Unwatch(_counterType));
                Become(OffState);
            });
        }

        private static string FormatButtonName(ChartingActor.CounterType counterType, bool isOn)
        {
            return string.Format("{0} ({1})", counterType.ToString().ToUpperInvariant(), isOn ? "ON" : "OFF");
        }
    }
}
