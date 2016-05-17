using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace WinTail
{
    public class TailActor : UntypedActor
    {
        public class InitialReadMessage
        {
            public InitialReadMessage(string fileName, string text)
            {
                FileName = fileName;
                Text = text;
            }

            public string FileName { get; private set; }
            public string Text { get; private set; }
        }

        private readonly string _filePath;
        private readonly IActorRef _reporterActor;
        private FileObserver _observer;
        private StreamReader _fileStreamReader;

        public TailActor(IActorRef reporterActor, string filePath)
        {
            _reporterActor = reporterActor;
            _filePath = filePath;
            _observer = null;
            _fileStreamReader = null;
        }

        /// <summary>
        /// Called before the first message is delivered to the actor.
        /// </summary>
        protected override void PreStart()
        {
            base.PreStart();

            // start watching file for changes
            _observer = new FileObserver(Self, Path.GetFullPath(_filePath));
            _observer.Start();

            var fileStream = new FileStream(Path.GetFullPath(_filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _fileStreamReader = new StreamReader(fileStream, Encoding.UTF8);

            // read the initial contents of the file and send it to console as first msg
            var text = _fileStreamReader.ReadToEnd();
            Self.Tell(new InitialReadMessage(_filePath, text));
        }

        /// <summary>
        /// Called after the actor has stopped receiving messages.
        /// </summary>
        protected override void PostStop()
        {
            _observer.Dispose();
            _observer = null;
            _fileStreamReader.Close();
            _fileStreamReader = null;
            base.PostStop();
        }

        protected override void OnReceive(object message)
        {
            if (message is Messages.FileChange)
            {
                var text = _fileStreamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    _reporterActor.Tell(new Messages.InputSuccess(text));
                }
            }
            else if (message is Messages.FileError)
            {
                var fe = message as Messages.FileError;
                _reporterActor.Tell(new Messages.InputError(fe.ToString()));
            }
            else if (message is InitialReadMessage)
            {
                var ir = message as InitialReadMessage;
                _reporterActor.Tell(new Messages.InputSuccess(ir.Text));
            }
        }
    }
}
