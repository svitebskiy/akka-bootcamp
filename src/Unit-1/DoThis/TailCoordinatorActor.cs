using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Creates TailActor to monitor a file for changes.
    /// </summary>
    public class TailCoordinatorActor : UntypedActor
    {
        public class StartTailMessage
        {
            public StartTailMessage(string filePath, IActorRef reporterActor)
            {
                this.FilePath = filePath;
                this.ReporterActor = reporterActor;
            }

            public string FilePath { get; private set; }
            public IActorRef ReporterActor { get; private set; }
        }

        public class StopTailMessage
        {
            public StopTailMessage(string filePath)
            {
                this.FilePath = filePath;
            }

            public string FilePath { get; private set; }
        }

        protected override void OnReceive(object message)
        {
            if (message is StartTailMessage)
            {
                var stt = message as StartTailMessage;
                Context.ActorOf(Props.Create(() => new TailActor(stt.ReporterActor, stt.FilePath)));
            }
            else if (message is StopTailMessage)
            {
                ////### TODOD:
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10, TimeSpan.FromSeconds(30), // Allow up to 10 errors within 30 sec
                ex =>
                {
                    if (ex is ArithmeticException) return Directive.Resume;
                    if (ex is NotSupportedException) return Directive.Stop;
                    return Directive.Restart;
                });
        }
    }
}