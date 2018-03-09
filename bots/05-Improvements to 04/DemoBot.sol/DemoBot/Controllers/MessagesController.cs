using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DemoBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using NLog;

namespace DemoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                logger.Trace("Invoking Luis Dialog.  Text:" + activity.Text);
                await Conversation.SendAsync(activity, MakeLuisDialog);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        internal static IDialog<object> MakeLuisDialog()
        {
            return Chain.From(() => new Dialogs.SampleLuisDialog(EmployeeCreation.BuildForm));
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels

                //// Note: Add introduction here:
                //IConversationUpdateActivity update = message;
                //var client = new ConnectorClient(new Uri(message.ServiceUrl), new MicrosoftAppCredentials());
                //if (update.MembersAdded != null && update.MembersAdded.Any())
                //{
                //    foreach (var newMember in update.MembersAdded)
                //    {
                //        if (newMember.Id != message.Recipient.Id)
                //        {
                //            var reply = message.CreateReply();
                //            reply.Text = $"Hi, I am Demo Bot.  Welcome! May I have your name please?";
                //            client.Conversations.ReplyToActivityAsync(reply);
                //        }
                //    }
                //}
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}