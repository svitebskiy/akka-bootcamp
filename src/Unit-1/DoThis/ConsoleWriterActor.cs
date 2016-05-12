using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for serializing message writes to the console.
    /// (write one message at a time, champ :)
    /// </summary>
    class ConsoleWriterActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            try
            {
                if (message is Messages.InputSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(((Messages.InputSuccess) message).Data);
                }
                else if (message is Messages.NullInputError)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Please provide an input.");
                }
                else if (message is Messages.InputError)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("An error was reported. The reason is:");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(((Messages.InputError) message).Reason);
                }
            }
            finally
            {
                Console.ResetColor();
                Console.WriteLine();
            }
        }
    }
}