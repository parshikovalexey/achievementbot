using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System.Text;

namespace BOTFirst.Dialogs {
    [Serializable]
    public class RootDialog : IDialog<object> {
        public Task StartAsync(IDialogContext context) {
            // Передаём управление методу, который спрашивает имя.
            context.Wait(NameFunc);
            //context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result) {
            var activity = await result;
            if (activity.Text.ToLower().Contains("да")) {
                await context.PostAsync("Получи пирожок");
                await context.PostAsync("Еще хочешь? ");
                context.Wait(MessageReceivedAsync);
            }
            else {
                await context.PostAsync("Не понял запроса");
                context.Wait(MessageReceivedAsync);
            }
            // calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;
            // return our reply to the user
            // тут пользователь должен ответить да, иначе бот просто непоймёт 
            // var reply = activity.Text.ToLower() == "да" ?
            // "Получи пирожочек" :
            //  "Я тебя не понимаю";
            // if (activity.Text == "Привет, Да")
            // await context.PostAsync(reply);
        }
        // метод, в котором бот будет приветствовать пользователя. 
        private async Task NameFunc(IDialogContext context, IAwaitable<IMessageActivity> result) {
            var activity = await result as Activity;
            if (!string.IsNullOrWhiteSpace(activity.Text)) {
                var text = new StringBuilder();
                text.AppendLine($"Рад познакомиться, {activity.Text}, Хочешь пироги ?");
                text.AppendLine("У меня самые вкусные пироги, 😉 Хочешь? Знаю, хочешь!");
                await context.PostAsync(text.ToString());
                // Передаём управление другому методу.
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}