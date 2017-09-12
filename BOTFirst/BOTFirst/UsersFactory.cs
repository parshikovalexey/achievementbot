using EntityModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace BOTFirst {
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
                    if (!db.Users.Any() && db.Users.Where((user) => user.Name == userName).Count() == 0) {
                        // Then we create and add the user. 
                        // TODO Implement getting user phone number.
                        returningUser = new User() { Name = userName };
                        db.Users.Add(returningUser);
                        db.SaveChanges();
                        thisUserIsNew = true;
                    }
                    else {
                        returningUser = db.Users.Where((u) => u.Name == userName).First();
                        thisUserIsNew = false;
                    }
                    // Temporary variables.
                    Messenger messenger = null;
                    UserMessenger userMessenger = null;
                    // If there is such messenger in DB.
                    if (db.Messengers.Any() && db.Messengers.Where((m) => m.Name == inputMessengerName).Count() > 0) {
                        messenger = db.Messengers.Where((m) => m.Name == inputMessengerName).First();
                        // There is such userMessenger in DB.
                        if (db.UserMessengers.Any() && db.UserMessengers.Where((um) => um.MessengerId == messenger.Id && um.UserId == returningUser.Id).Count() > 0) {
                            userMessenger = db.UserMessengers.Where((um) => um.MessengerId == messenger.Id && um.UserId == returningUser.Id).First();
                        }
                        // There is no such userMessenger in DB.
                        else {
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
                    Debug.Indent();
                    foreach (var eve in e.EntityValidationErrors) {
                        Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors) {
                            Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    Debug.Unindent();
                    throw;
                }
            }
        }
    }
}