using System;
using Akka.Actor;
using WinTail.WinTail;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Terminate"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        public const string StartCommand = "start";
        public const string ExitCommand = "exit";
        private readonly IActorRef _consoleWriterActor;
        private readonly IActorRef _validationActor;

        public ConsoleReaderActor(IActorRef consoleWriterActor, IActorRef validationActor = null)
        {
            _consoleWriterActor = consoleWriterActor;
            if (validationActor == null)
            {

                _validationActor = Context.ActorOf(Props.Create(() => new ValidationActor(_consoleWriterActor)), "consoleReaderActor_validationActor");
            }
            else
            {
                _validationActor = validationActor;
            }
        }

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                DoPrintInstructions();
            }
            else if (message is InputError)
            {
                _consoleWriterActor.Tell(message as InputError);
            }

            _validationActor.Tell(Console.ReadLine());
        }
        #region Internal methods
        private void DoPrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.WriteLine("Some entries will pass validation, and some won't...\n\n");


            Console.WriteLine("**Lesson 4** - Please provide the URI of a log file on disk.\n");

            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }

        #endregion

    }
}