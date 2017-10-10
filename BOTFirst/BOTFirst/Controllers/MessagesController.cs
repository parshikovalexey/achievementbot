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
using EntityModel;
using BOTFirst.Factories;
using System;

namespace BOTFirst {
    [BotAuthentication]
    public class MessagesController : ApiController {
        /// <summary>
        /// POST: api/Messages.
        /// Receive a message from a user and reply to it.
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity) {
            if (activity != null) {
                switch (activity.GetActivityType()) {
                    case ActivityTypes.Message:
                        await Conversation.SendAsync(activity, () => new Dialogs.PhraseDialog());
                        break;
                    case ActivityTypes.ConversationUpdate:
                        IConversationUpdateActivity update = activity;
                        using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity)) {
                            var client = scope.Resolve<IConnectorClient>();
                            if (update.MembersAdded.Any()) {
                                var reply = activity.CreateReply();
                                try {
                                    foreach (var newMember in update.MembersAdded) {
                                        if (newMember.Id != activity.Recipient.Id) {
                                            reply.Text = string.Format("Привет, {0}! Я буду помогать тебе добиваться поставленных целей! Поделись своими последними достижениями!  \n", newMember.Name);
                                            bool userIsNew;
                                            string errors;
                                            User user = UsersFactory.CreateOrRetrieveUser(newMember.Name, newMember.Id, activity.ChannelId, out userIsNew, out errors);
                                            if (user == null) {
                                                reply.Text = "Пользователь не создан c ошибкой: " + errors;
                                            }
                                            //#if DEBUG 
                                            //                                        reply.Text += UsersFactory.UserAddingDebug(activity.ChannelId, userIsNew, user);
                                            //#endif
                                            
                                        }
                                    }
                                } catch (Exception ex) {
                                    reply.Text = "MesageController exception: " + ex.Message;
                                }

                                await client.Conversations.ReplyToActivityAsync(reply);
                            }
                        }
                        break;
                    case ActivityTypes.ContactRelationUpdate:
                        // Your bot should call this method when it receives an activity of type deleteUserData
                        // or an activity of type contactRelationUpdate that indicates the bot has been removed
                        // from the user's contact list (https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-state).
                        activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
                        break;
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                        // See comment for ActivityTypes.ContactRelationUpdate case.
                        activity.GetStateClient().BotState.DeleteStateForUser(activity.ChannelId, activity.From.Id);
                        break;
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
