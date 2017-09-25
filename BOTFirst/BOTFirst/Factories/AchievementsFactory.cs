using EntityModel;
using System;
using System.Data.Entity.Validation;
using System.Linq;

namespace BOTFirst.Factories {
    public static class AchievementsFactory {
        /// <summary>
        /// This method creates <see cref="Achievement"/>, <see cref="AchievementForm"/>, <see cref="UserAchievement"/> classes and adds them into DB if it is necessary (i.e. if they do not exist).
        /// </summary>
        /// <returns>New <see cref="UserAchievement"> object.</returns>
        public static UserAchievement CreateOrRetreaveUserAchievement(Phrase phrase, DateTime achievementDateTime, User user, string inputUserAchievement, bool suchAchivementForActionExists) {
            try {
                using (EDModelContainer db = new EDModelContainer()) {
                    Achievement achievement;
                    if (suchAchivementForActionExists) {
                        achievement = db.Achievements.Where(a => a.Name == inputUserAchievement).First();
                    }
                    else {
                        achievement = new Achievement() { Name = inputUserAchievement };
                    }

                    var userAchievement = new UserAchievement() {
                        Achievement = achievement,
                        DateAndTime = achievementDateTime
                    };
                    userAchievement.UserId = user.Id;
                    userAchievement.PhraseId = phrase.Id;
                    db.UserAchievements.Add(userAchievement);
                    db.SaveChanges();

                    // Define achievement foreign key for action.
                    achievement = db.Achievements.Where(a => a.Name == inputUserAchievement).First();
                    (from a in db.Actions where a.Id == phrase.ActionId select a).Single().AchievementId = achievement.Id;
                    db.SaveChanges();
                    return userAchievement;
                }
            }
            // Debugging.
            catch (DbEntityValidationException e) {
                EntityModel.EntitiesErrorsCatcher.CatchError(e);
                throw;
            }
        }

        public static bool SuchInputActionAchievementExists(EntityModel.Action action) {
            using (EDModelContainer db = new EDModelContainer()) {
                if (db.Achievements.Any(a => a.Id == action.Id))
                    return true;
                else
                    return false;
            }
        }

        public static Achievement GetAchievementThroughAction(EntityModel.Action action) {
            using (EDModelContainer db = new EDModelContainer()) {
                if (db.Achievements.Any(a => a.Id == action.AchievementId)) {
                    return db.Achievements.Where(a => a.Id == action.AchievementId).First();
                }
                else
                    throw new System.Exception("There is no achievement that related with such action.");
            }
        }
    }
}
