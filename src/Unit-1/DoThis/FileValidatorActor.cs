﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTail
{
    // FileValidatorActor.cs
    using System.IO;
    using Akka.Actor;

    namespace WinTail
    {
        /// <summary>
        /// Actor that validates user input and signals result to others.
        /// </summary>
        public class FileValidatorActor : UntypedActor
        {
            private readonly IActorRef _consoleWriterActor;

            public FileValidatorActor(IActorRef consoleWriterActor)
            {
                _consoleWriterActor = consoleWriterActor;
            }

            protected override void OnReceive(object message)
            {
                var msg = message as string;
                if (string.IsNullOrEmpty(msg))
                {
                    // signal that the user needs to supply an input
                    _consoleWriterActor.Tell(new NullInputError("Input was blank. Please try again.\n"));

                    // tell sender to continue doing its thing (whatever that may be,
                    // this actor doesn't care)
                    Sender.Tell(new ContinueProcessing());
                }
                else
                {
                    var valid = IsFileUri(msg);
                    if (valid)
                    {
                        // signal successful input
                        _consoleWriterActor.Tell(new InputSuccess($"Starting processing for {msg}"));

                        // start coordinator
                        Context.ActorSelection("akka://MyFirstActorSystem/user/tailCoordinatorActor").Tell(
                            new TailCoordinatorActor.StartTail(msg, _consoleWriterActor));

                    }
                    else
                    {
                        // signal that input was bad
                        _consoleWriterActor.Tell(new ValidationError($"{msg} is not an existing URI on disk."));

                        // tell sender to continue doing its thing (whatever that
                        // may be, this actor doesn't care)
                        Sender.Tell(new ContinueProcessing());
                    }
                }
            }

            /// <summary>
            /// Checks if file exists at path provided by user.
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            private static bool IsFileUri(string path)
            {
                return File.Exists(path);
            }
        }
    }
}
