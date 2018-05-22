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
            
            if(message is InputError)
            {
                var msgRec = message as InputError;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msgRec.Reason);
            }
            else if (message is InputSuccess)
            {
                var msgRec = message as InputSuccess;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(msgRec.Reason);
            }
            else
            {
                Console.WriteLine(message);
            }

            Console.ResetColor();

        }
    }
}
