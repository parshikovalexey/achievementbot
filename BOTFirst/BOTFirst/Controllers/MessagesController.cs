using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Diagnostics;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System.Linq;
using Autofac;

namespace BOTFirst
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity != null)
            {

                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                        break;
                    case ActivityTypes.ConversationUpdate:
                        IConversationUpdateActivity update = activity;
                        using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
                        {
                            var client = scope.Resolve<IConnectorClient>();
                            if (update.MembersAdded.Any())
                            {
                                var reply = activity.CreateReply();
                                foreach (var newMember in update.MembersAdded)
                                {
                                    if (newMember.Id != activity.Recipient.Id)
                                    {
                                        reply.Text = $"Привет, я Чат-бот! Как тебя зовут ? ";
                                    }

                                    await client.Conversations.ReplyToActivityAsync(reply);
                                }
                            }
                        }
                        break;
                    case ActivityTypes.ContactRelationUpdate:
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                    case ActivityTypes.Ping:
                    default:
                        Trace.TraceError($"Unknown activity type ignored: { activity.GetActivityType()}");
                        break;
                }
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }
    }
}