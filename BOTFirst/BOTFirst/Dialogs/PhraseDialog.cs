using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using BotPhrase;

namespace BOTFirst.Dialogs
{
    [Serializable]
    public class PhraseDialog : IDialog<object>
    {

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            
            await context.PostAsync(Recognizer.GetPhraseFromMessage(activity.Text));

            await context.PostAsync("Есть еще какие-либо новые достижения?");
            context.Wait(MessageReceivedAsync);
        }
       
    }
}