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
        private IActorRef _consoleWriterActor;

        public ConsoleReaderActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
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
                if (GetAndValidateInput())
                {
                    ContinueProcessing();
                }
                else
                {
                    // Unblock MyActorSystem.AwaitTermination() call in the Main method.
                    Context.System.Shutdown();
                }
            }
        }

        private void PrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.Write("Some lines will appear as");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(" red ");
            Console.ResetColor();
            Console.Write(" and others will appear as");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" green! ");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }

        private void ContinueProcessing()
        {
            Self.Tell(new Messages.ContinueProcessing());
        }

        /// <returns><c>true</c> to continue processing.</returns>
        public bool GetAndValidateInput()
        {
            var read = Console.ReadLine();
            if (!string.IsNullOrEmpty(read) && String.Equals(read, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string validationErrorReason = "";
            if (string.IsNullOrEmpty(read))
            {
                // Empty input:
                _consoleWriterActor.Tell(new Messages.NullInputError("An empty string was entered."));
            }
            else if (!IsValid(read, out validationErrorReason))
            {
                // Invalid input:
                _consoleWriterActor.Tell(new Messages.InputError(validationErrorReason));
            }
            else
            {
                // Valid input:
                _consoleWriterActor.Tell(new Messages.InputSuccess(read));
            }

            return true;
        }

        private bool IsValid(string input, out string reasonWhyInvalid)
        {
            if (input == null)
            {
                reasonWhyInvalid = "NULL string.";
                return false;
            }
            else if (input.Length % 2 != 0)
            {
                reasonWhyInvalid = "The input has odd number of characters.";
                return false;
            }

            reasonWhyInvalid = "";
            return true;
        }
    }
}