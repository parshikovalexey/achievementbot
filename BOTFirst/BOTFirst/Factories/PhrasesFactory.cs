using BotPhrase;
using EntityModel;
using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace BOTFirst.Factories {
    public static class PhrasesFactory {
        /// <summary>
        /// This method creates a new <see cref="BotPhrase.Phrase"/> and <see cref="EntityModel.Phrase"/>.
        /// If such phrase already exists in DB then the method would not add the phrase, otherwise the method would add the phrase into DB.
        /// </summary>
        /// <param name="userRawPhrase">Input phrase as a string.</param>
        /// <param name="outModelPhrase">Created or existing model phrase.</param>
        /// <returns>New <see cref="BotPhrase.Phrase"> object (NOT <see cref="EntityModel.Phrase"> object!).</returns>
        public static BotPhrase.Phrase CreateOrRetreavePhrase(string userRawPhrase, out EntityModel.Phrase outModelPhrase) {
            BotPhrase.Phrase phrase = Recognizer.RecognizePhrase(userRawPhrase);
            // NOTE Here we can use the alternative PhrasesConverter.ShowingErrorPhraseToModelPhrase method too see problems when using navigation properties.
            // Also the problems are mentioned at that alternative method comments.
            var modelPhrase = PhrasesConverter.PhraseToModelPhrase(phrase);
            try {
                using (EDModelContainer db = new EDModelContainer()) {
                    // Maybe I should use an exposed IEqualityComparer class and .Contains(modelPhrase), but I tried and failed to make this way working.
                    if (!db.Phrases.Any(p => p.ActionId == modelPhrase.ActionId
                            && p.AdditionalTextId == modelPhrase.AdditionalTextId
                            && p.Amount == modelPhrase.Amount
                            && p.CorrectedMessage == modelPhrase.CorrectedMessage
                            && p.Date == modelPhrase.Date
                            && p.MeasureUnitId == modelPhrase.MeasureUnitId
                            && p.OriginalMessage == modelPhrase.OriginalMessage
                            && p.Time == modelPhrase.Time
                            && p.WasRecognized == modelPhrase.WasRecognized)) {
                        db.Phrases.Add(modelPhrase);
                        db.SaveChanges();
                    }
                    else {
                        Debug.Indent();
                        Debug.WriteLine("There already is such Phrase in DB.");
                        Debug.Unindent();
                    }

                    // TODO Create some method to remove code repeating or find a way to get id of new added model phrase.
                    outModelPhrase = db.Phrases.Where(p => p.ActionId == modelPhrase.ActionId
                        && p.AdditionalTextId == modelPhrase.AdditionalTextId
                        && p.Amount == modelPhrase.Amount
                        && p.CorrectedMessage == modelPhrase.CorrectedMessage
                        && p.Date == modelPhrase.Date
                        && p.MeasureUnitId == modelPhrase.MeasureUnitId
                        && p.OriginalMessage == modelPhrase.OriginalMessage
                        && p.Time == modelPhrase.Time
                        && p.WasRecognized == modelPhrase.WasRecognized).First();
                    return phrase;
                }
            }
            catch (DbEntityValidationException e) {
                EntityModel.EntitiesErrorsCatcher.CatchError(e);
                throw;
            }
        }
        public static EntityModel.Phrase GetExistingPhraseById(int modelPhraseId) {
            using (EDModelContainer db = new EDModelContainer()) {
                try {
                    return db.Phrases.Where(p => p.Id == modelPhraseId).First();
                }
                // Debugging.
                catch (DbEntityValidationException e) {
                    EntityModel.EntitiesErrorsCatcher.CatchError(e);
                    throw;
                }
            }
        }
        public static EntityModel.Action GetActionOfExistingPhraseById(int modelPhraseId) {
            using (EDModelContainer db = new EDModelContainer()) {
                try {
                    return db.Actions.Where(a => a.Id == db.Phrases.Where(p => p.Id == modelPhraseId).FirstOrDefault().ActionId).First();
                }
                catch (DbEntityValidationException e) {
                    EntityModel.EntitiesErrorsCatcher.CatchError(e);
                    throw;
                }
            }
        }
    }
}
