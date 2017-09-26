using EntityModel;
using System;
using System.Data.Entity.Validation;
using System.Linq;

namespace BOTFirst.Factories {
    public static class UsersFactory {
        /// <summary>
        /// The method creates and adds to DB some new user or only returns some existing user from DB.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="messengerUserIdentifier">User id, that was defined by some messenger (channel).</param>
        /// <param name="inputMessengerName">Name of using messenger (channel).</param>
        /// <param name="thisUserIsNew">Output flag indicating about if new user was created.</param>
        public static User CreateOrRetrieveUser(string userName, string messengerUserIdentifier, string inputMessengerName, out bool thisUserIsNew) {
            using (EDModelContainer db = new EDModelContainer()) {
                try {
                    User returningUser;
                    // There is no such user yet.
                    if (!db.Users.Any() && !db.Users.Any(user => user.Name == userName)) {
                        // Then we create and add the user.
                        // TODO Implement getting user phone number.
                        returningUser = new User() { Name = userName };
                        db.Users.Add(returningUser);
                        db.SaveChanges();
                        thisUserIsNew = true;
                    }
                    else {
                        returningUser = db.Users.Where(u => u.Name == userName).First();
                        thisUserIsNew = false;
                    }
                    // Temporary variables.
                    Messenger messenger = db.Messengers.Where(m => m.Name == inputMessengerName).FirstOrDefault();
                    UserMessenger userMessenger = null;
                    // If there is such messenger in DB.
                    if (messenger != null) {
                        userMessenger = db.UserMessengers.Where(um => um.MessengerId == messenger.Id && um.UserId == returningUser.Id).FirstOrDefault();
                        if (userMessenger == null) {
                            userMessenger = new UserMessenger() {
                                MessengerUserIdentifier = messengerUserIdentifier,
                                MessengerId = messenger.Id,
                                UserId = returningUser.Id
                            };
                            db.UserMessengers.Add(userMessenger);
                            db.SaveChanges();
                        }
                    }
                    else {
                        // Since we created and add new messenger, so there is no userMessenger entries yet.
                        userMessenger = new UserMessenger() {
                            MessengerUserIdentifier = messengerUserIdentifier,
                            Messenger = new Messenger() { Name = inputMessengerName },
                            UserId = returningUser.Id
                        };
                        db.UserMessengers.Add(userMessenger);
                        db.SaveChanges();
                    }
                    return returningUser;
                }
                // Debugging.
                catch (DbEntityValidationException e) {
                    EntityModel.EntitiesErrorsCatcher.CatchError(e);
                    throw;
                }
            }
        }
        public static User GetExistingUserById(int modelUserId) {
            using (EDModelContainer db = new EDModelContainer()) {
                try {
                    return db.Users.Where(u => u.Id == modelUserId).First();
                }
                // Debugging.
                catch (DbEntityValidationException e) {
                    EntityModel.EntitiesErrorsCatcher.CatchError(e);
                    throw;
                }
            }
        }
        /// <summary>
        /// Method for checking if a user has been added to DB and showing some debug info about messenger name and userId in the messenger.
        /// This method should be called after a call of <see cref="UsersFactory.CreateOrRetrieveUser"/> method.
        /// </summary>
        /// <param name="channelId">Id of current channel. It is used as messenger name.</param>
        /// <param name="userIsNew">Whether the user was added into DB.</param>
        /// <param name="user">Some adding (or just retrieving from DB) user.</param>
        /// <returns>Debug info about previously added user.</returns>
        public static string UserAddingDebug(string channelId, bool userIsNew, User user) {
            // Testing. Maybe it should be moved to a new method of UsersFactory class (or to some new class).
            Messenger currentMessenger = null;
            UserMessenger currentUserMessenger = null;
            using (EDModelContainer db = new EDModelContainer()) {
                currentMessenger = db.Messengers.Where(m => m.Name == channelId).FirstOrDefault();
                if (currentMessenger != null) {
                    currentUserMessenger = db.UserMessengers.Where(um => um.UserId == user.Id && um.MessengerId == currentMessenger.Id).FirstOrDefault();
                    if (currentUserMessenger != null)
                        currentUserMessenger = db.UserMessengers.Where((um) => um.UserId == user.Id && um.MessengerId == currentMessenger.Id).First();
                }
            }
            if (userIsNew)
                return string.Format("Debug info: добавлен новый пользователь: имя - {0}, мессенджер - {1}, id пользователя в данном мессенджере - {2}", user.Name, currentMessenger.Name, currentUserMessenger.MessengerUserIdentifier);
            else
                return string.Format("Debug info: такой пользователь уже есть, поэтому используются уже существующие в БД записи: имя - {0}, мессенджер - {1}, id пользователя в данном мессенджере - {2}", user.Name, currentMessenger.Name, currentUserMessenger.MessengerUserIdentifier);
        }
    }
}
