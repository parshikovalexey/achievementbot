using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using BOTFirst.Factories;

namespace BOTFirst.Dialogs {
    [Serializable]
    public class PhraseDialog : IDialog<object> {
        public Task StartAsync(IDialogContext context) {
            // This variable is used nowhere(.
            bool userIsNew;
            EntityModel.User currentUser = UsersFactory.CreateOrRetrieveUser(context.Activity.From.Name, context.Activity.From.Id, context.Activity.ChannelId, out userIsNew);
            context.UserData.SetValue("currentModelUserId", currentUser.Id);
            context.Wait(AchivementsAddingAsync);
            return Task.CompletedTask;
        }

        private async Task AchivementsAddingAsync(IDialogContext context, IAwaitable<IMessageActivity> result) {
            // Wait for user to write a new achievement here.
            var activity = await result;
            var modelPhrase = new EntityModel.Phrase();

            BotPhrase.Phrase newNotModelPhrase = PhrasesFactory.CreateOrRetreavePhrase(activity.Text, out modelPhrase);
            await context.PostAsync("Результаты разбора вашей фразы:  \n" + newNotModelPhrase.RecognitionResult);

            // Create new UserAchievement, Achievement, AchievementForm if the phrase was recognized.
            if (newNotModelPhrase.WasRecognized) {
                context.UserData.SetValue("currentPhraseId", modelPhrase.Id);
                context.UserData.SetValue("currentPhraseDateTime", newNotModelPhrase.PhraseDateTime);

                EntityModel.Phrase addedPhrase = PhrasesFactory.GetExistingPhraseById(modelPhrase.Id);
                if (!AchievementsFactory.SuchInputActionAchievementExists(PhrasesFactory.GetActionOfExistingPhraseById(addedPhrase.Id))) {
                    // NOTE If we would not show "Результаты разбора вашей фразы" info, then it is worth to add "Я вас понял" before "пожалуйста, введите..." here.
                    await context.PostAsync("Пожалуйста, введите соответствующее достижению существительное (пример: прочитал(а) – чтение), которое будет служить наименованием достижения. Если не получается подобрать существительное, то используйте несколько слов, отражающих только действие (например – «Поправил(ся/лась) на 10 кг – набор веса»:");
                    // Getting achievement from user input.
                    context.Wait(UserEntersAchievement);
                }
                else {
                    context.UserData.SetValue("currentPhraseId", modelPhrase.Id);
                    // Getting achievement that already exists in DB by means of phrase action.
                    var achievement = AchievementsFactory.GetAchievementThroughAction(PhrasesFactory.GetActionOfExistingPhraseById(addedPhrase.Id));
                    EntityModel.User currentUser = UsersFactory.GetExistingUserById(context.UserData.GetValue<int>("currentModelUserId"));
                    AchievementsFactory.CreateOrRetreaveUserAchievement(addedPhrase, newNotModelPhrase.PhraseDateTime, currentUser, achievement.Name, true);
                    await context.PostAsync("Отлично! Ваше достижение добавлено в систему, можете ввести следующее достижение:");
                }
            }
            else {
                await context.PostAsync("Не могу распознать фразу, пожалуйста, перефразируйте свое достижение:");
                context.Wait(AchivementsAddingAsync);
            }
        }

        private async Task UserEntersAchievement(IDialogContext context, IAwaitable<IMessageActivity> result) {
            var activity = await result;
            // Maybe does some user input check need here?
            string achievement = activity.Text;
            EntityModel.User currentUser = UsersFactory.GetExistingUserById(context.UserData.GetValue<int>("currentModelUserId"));
            EntityModel.Phrase addedPhrase = PhrasesFactory.GetExistingPhraseById(context.UserData.GetValue<int>("currentPhraseId"));
            AchievementsFactory.CreateOrRetreaveUserAchievement(addedPhrase, context.UserData.GetValue<DateTime>("currentPhraseDateTime"), currentUser, achievement, false);
            await context.PostAsync("Отлично! Ваше достижение добавлено в систему, можете ввести следующее достижение:");
            context.Wait(AchivementsAddingAsync);
        }
    }
}
