using System;
﻿using Akka.Actor;

namespace WinTail
{
    #region Program
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            // initialize MyActorSystem
            MyActorSystem = ActorSystem.Create("MyActorSystem");

            // time to make your first actors!
            // make consoleWriterActor using these props: Props.Create(() => new ConsoleWriterActor())
            // make consoleReaderActor using these props: Props.Create(() => new ConsoleReaderActor(consoleWriterActor))
            // SV: IActoveRef does not necessary represent a specific actor C# object:
            //     This allows for stateless singletons, load-balanced "farms", etc.
            var writerActorRef = MyActorSystem.ActorOf(Props.Create(() => new ConsoleWriterActor()));
            var readerActorRef = MyActorSystem.ActorOf(Props.Create(() => new ConsoleReaderActor(writerActorRef)));

            // tell console reader to begin
            // SV: An actor will not do anything until it receives a message.
            readerActorRef.Tell(ConsoleReaderActor.StartCommand);

            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.AwaitTermination();
        }
    }
    #endregion
}
