using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace WinTail
{
    public class FileObserver : IDisposable
    {
        private readonly IActorRef _tailActor;
        private readonly string _absoluteFilePath;
        private FileSystemWatcher _watcher;
        private readonly string _fileDir;
        private readonly string _fileNameOnly;

        public FileObserver(IActorRef tailActor, string absoluteFilePath)
        {
            _tailActor = tailActor;
            _absoluteFilePath = absoluteFilePath;
            _fileDir = Path.GetDirectoryName(absoluteFilePath);
            _fileNameOnly = Path.GetFileName(absoluteFilePath);
        }

        public void Start()
        {
            _watcher = new FileSystemWatcher(_fileDir, _fileNameOnly);
            _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            _watcher.Changed += OnFileChange;
            _watcher.Error += OnFileError;
            _watcher.EnableRaisingEvents = true; // This will start monitoring for file changes
        }

        private void OnFileChange(object sender, FileSystemEventArgs e)
        {
            if ((e.ChangeType & WatcherChangeTypes.Changed) != 0) // Only file data change, not necessarily (create/delete/rename)
            {
                // * ActorRefs.NoSender tells that the message was not sent by an actor, so it cannot be replied to.
                _tailActor.Tell(new Messages.FileChange(e.Name, e.FullPath), ActorRefs.NoSender);
            }
        }

        void OnFileError(object sender, ErrorEventArgs e)
        {
            _tailActor.Tell(new Messages.FileError(_fileNameOnly, Path.Combine(_fileDir, _fileNameOnly), e.GetException()), ActorRefs.NoSender);
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}
