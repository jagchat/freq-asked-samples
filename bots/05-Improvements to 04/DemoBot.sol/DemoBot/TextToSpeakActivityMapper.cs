﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace DemoBot
{
    public sealed class TextToSpeakActivityMapper : IMessageActivityMapper
    {
        public IMessageActivity Map(IMessageActivity message)
        {
            // only set the speak if it is not set by the developer.
            var channelCapability = new ChannelCapability(Address.FromActivity(message));

            if (channelCapability.SupportsSpeak() && string.IsNullOrEmpty(message.Speak))
            {
                message.Speak = message.Text;

                // set InputHint to ExpectingInput if text is a question
                var isQuestion = message.Text?.EndsWith("?");
                if (isQuestion.GetValueOrDefault())
                {
                    message.InputHint = InputHints.ExpectingInput;
                }
            }

            return message;
        }
    }
}