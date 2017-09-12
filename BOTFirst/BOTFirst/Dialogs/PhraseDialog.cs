using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using BotPhrase;
using EntityModel;
using System.Diagnostics;
using System.Data.Entity.Validation;

namespace BOTFirst.Dialogs {
    [Serializable]
    public class PhraseDialog : IDialog<object> {
        public Task StartAsync(IDialogContext context) {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result) {
            var activity = await result;
            await context.PostAsync("Результаты разбора вашей фразы:  \n" + PhrasesFactory.CreateOrRetreavePhrase(activity.Text).RecognitionResult);
            await context.PostAsync("Отлично! Есть какие-либо другие новые достижения?");
            context.Wait(MessageReceivedAsync);
        }
    }
}