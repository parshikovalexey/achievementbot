using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using BotPhrase;
using EntityModel;

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
            var modelPhrase = Recognizer.GetModelPhraseFromMessage(activity.Text);
            var phrase = PhrasesConverter.ModelPhraseToPhrase(modelPhrase);

            //Adding data to DB example. It is necessary to implement checking of existence related objects such as actions, measureUnits and additionalTexts.
            /*using (EDModelContainer db = new EDModelContainer()) {
               db.Phrases.Add(modelPhrase);
               db.SaveChanges();
            }*/

            await context.PostAsync(phrase.ToString());
            await context.PostAsync("Отлично! Есть какие-либо другие новые достижения?");
            context.Wait(MessageReceivedAsync);
        }
       
    }
}