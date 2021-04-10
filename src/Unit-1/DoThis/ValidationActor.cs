﻿using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace WinTail
{
    public class ValidationActor : UntypedActor
    {
        public IActorRef _consoleWriterActor { get; private set; }

        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            var msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                // signal that the user needs to supply an input
                _consoleWriterActor.Tell(new Messages.NullInputError("No input received."));
            }
            else
            {
                var valid = IsValid(msg);
                if (valid)
                {
                    // send success to console writer
                    _consoleWriterActor.Tell(new Messages.InputSuccess("Thank you! Message was valid."));
                }
                else
                {
                    // signal that input was bad
                    _consoleWriterActor.Tell(new Messages.ValidationError("Invalid: input had odd number of characters."));
                }
            }

            // tell sender to continue doing its thing
            // (whatever that may be, this actor doesn't care)
            Sender.Tell(new Messages.ContinueProcessing());
        }

        #region Internal methods
        /// <summary>
        /// Determines if the message received is valid.
        /// Checks if number of chars in message received is even.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool IsValid(string message)
        {
            var valid = message.Length % 2 == 0;
            return valid;
        }
        #endregion
    }
}