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

            // SV: IActoveRef does not necessary represent a specific actor C# object:
            //     This allows for stateless singletons, load-balanced "farms", etc.
            // SV: readerActorRef is a "top level" actor as it's created by ActorySystm.ActorOf().
            //     A child actor is created by Context.ActorOf() inside a parent actor's method.
            // NEVER call 'new MyActorClass()' outside the context of Props.Create.
            // NEVER call 'new Props' directly. Always use Props.Create().
            // SV: Actor names are optional, but recommended for logging and addressing.

            // Not recommended to use the typeof() version, but possible still.
            // The danger is in passing a non-actor class to typeof().
            // var writerProps = Props.Create(typeof(ConsoleWriterActor));
            // Safer way is use Create<T>() or Create(Func<T>)
            var writerProps = Props.Create<ConsoleWriterActor>();
            var writerRef = MyActorSystem.ActorOf(writerProps, "writer");

            var tailCoordinatorProps = Props.Create<TailCoordinatorActor>();
            var tailCoordinatorRef = MyActorSystem.ActorOf(tailCoordinatorProps, "tailCoordinator");

            var vaildationProps = Props.Create(() => new FileValidationActor(writerRef, tailCoordinatorRef));
            var vaidatoinRef = MyActorSystem.ActorOf(vaildationProps, "validation");

            var readerProps = Props.Create(() => new ConsoleReaderActor(vaidatoinRef));
            var readerRef = MyActorSystem.ActorOf(readerProps, "reader");

            // tell console reader to begin
            // SV: An actor will not do anything until it receives a message.
            readerRef.Tell(ConsoleReaderActor.StartCommand);

            // blocks the main thread from exiting until the actor system is shut down
            MyActorSystem.AwaitTermination();
        }
    }
    #endregion
}