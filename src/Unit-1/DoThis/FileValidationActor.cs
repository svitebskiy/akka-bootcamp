using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace WinTail
{
    public class FileValidationActor : UntypedActor
    {
        private IActorRef _consoleWriterActor;
        public const string TailCoordinatorActorSelector = "akka://MyActorSystem/user/tailCoordinator";

        public FileValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            string inputData = message as string;
            Messages.InputError error;
            if (!IsValid(inputData, out error))
            {
                _consoleWriterActor.Tell(error);
            }
            else
            {
                _consoleWriterActor.Tell(new Messages.InputSuccess(inputData));
                Context.ActorSelection(TailCoordinatorActorSelector).Tell(new TailCoordinatorActor.StartTailMessage(inputData, _consoleWriterActor));
            }

            Sender.Tell(new Messages.ContinueProcessing());
        }

        private bool IsValid(string input, out Messages.InputError errorMsg)
        {
            if (input == null)
            {
                errorMsg = new Messages.NullInputError("NULL file path.");
                return false;
            }
            else if (input.Length == 0)
            {
                errorMsg = new Messages.ValidationError("Empty file path.");
                return false;
            }
            else if (!IsFilePath(input))
            {
                errorMsg = new Messages.ValidationError(string.Format("\"{0}\" is not a file path.", input));
                return false;
            }

            errorMsg = null;
            return true;
        }

        private bool IsFilePath(string path)
        {
            return System.IO.File.Exists(path);
        }
    }
}