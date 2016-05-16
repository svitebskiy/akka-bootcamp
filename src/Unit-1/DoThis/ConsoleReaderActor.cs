using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Shutdown"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        public static readonly Messages.ContinueProcessing StartCommand = new Messages.ContinueProcessing();
        public const string ExitCommand = "exit";
        public const string ValidationActorSelector = "akka://MyActorSystem/user/validation";

        public ConsoleReaderActor()
        {
        }

        protected override void OnReceive(object message)
        {
            if (StartCommand.Equals(message))
            {
                PrintInstructions();
                ContinueProcessing();
            }
            else if (message is Messages.ContinueProcessing)
            {
                GetAndValidateInput();
            }
        }

        private void PrintInstructions()
        {
            Console.WriteLine("Please provide the URI of a log file on disk.");
        }

        private void ContinueProcessing()
        {
            Self.Tell(new Messages.ContinueProcessing());
        }

        public void GetAndValidateInput()
        {
            var read = Console.ReadLine();
            if (!string.IsNullOrEmpty(read) && String.Equals(read, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // Unblock MyActorSystem.AwaitTermination() call in the Main method.
                Context.System.Shutdown();
            }
            else
            {
                Context.ActorSelection(ValidationActorSelector).Tell(read);
            }
        }
    }
}