﻿using System.Net;
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

namespace BOTFirst {
    [BotAuthentication]
    public class MessagesController : ApiController {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
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
                                foreach (var newMember in update.MembersAdded) {
                                    if (newMember.Id != activity.Recipient.Id) {
                                        reply.Text = string.Format("Привет, {0}! Я буду помогать тебе добиваться поставленных целей! Поделись своими последними достижениями!", newMember.Name);
                                        bool userIsNew;
                                        User user = UsersFactory.CreateOrRetrieveUser(newMember.Name, newMember.Id, activity.ChannelId, out userIsNew);

                                        #region Debug
                                        // Testing. Maybe it should be moved to a new method of UsersFactory class (or to some new class).
                                        Messenger currentMessenger = null;
                                        UserMessenger currentUserMessenger = null;
                                        using (EDModelContainer db = new EDModelContainer()) {
                                            if (db.Messengers.Where((m) => m.Name == activity.ChannelId).Count() > 0) {
                                                currentMessenger = db.Messengers.Where((m) => m.Name == activity.ChannelId).First();
                                            if (db.UserMessengers.Where((um) => um.UserId == user.Id && um.MessengerId == currentMessenger.Id).Count() > 0)
                                                currentUserMessenger = db.UserMessengers.Where((um) => um.UserId == user.Id && um.MessengerId == currentMessenger.Id).First();
                                            }
                                        }
                                        if (userIsNew)
                                            reply.Text += string.Format("  \nDebug info: добавлен новый пользователь: имя - {0}, мессенджер - {1}, id пользователя в данном мессенджере - {2}", user.Name, currentMessenger.Name, currentUserMessenger.MessengerUserIdentifier);
                                        else
                                            reply.Text += string.Format("  \nDebug info: такой пользователь уже есть, поэтому используются уже существующие в БД записи: имя - {0}, мессенджер - {1}, id пользователя в данном мессенджере - {2}", user.Name, currentMessenger.Name, currentUserMessenger.MessengerUserIdentifier);
                                        #endregion

                                        await client.Conversations.ReplyToActivityAsync(reply);
                                    }
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