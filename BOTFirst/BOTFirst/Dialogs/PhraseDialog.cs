using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using BotPhrase;
using EntityModel;
using System.Diagnostics;
using System.Data.Entity.Validation;

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

            //Adding data to DB example.It is necessary to implement checking of existence related objects such as actions, measureUnits and additionalTexts.
            var phrase = Recognizer.RecognizePhrase(activity.Text);
            var modelPhrase = PhrasesConverter.PhraseToModelPhrase(phrase);
            Debug.Indent();
            try
            {
                using (EDModelContainer db = new EDModelContainer())
                {
                    db.Phrases.Add(modelPhrase);
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            Debug.Unindent();

            await context.PostAsync(Recognizer.RecognizePhrase(activity.Text).RecognitionResult);
            await context.PostAsync("Отлично! Есть какие-либо другие новые достижения?");
            context.Wait(MessageReceivedAsync);
        }
       
    }
}