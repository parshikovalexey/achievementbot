using BotPhrase;
using EntityModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace BOTFirst {
    public static class PhrasesFactory {
        /// <summary>
        /// Method for creating new <see cref="EntityModel.Phrase"/>s and adding them to DB.
        /// If such phrase already exists in DB, then the method only return the phrase without repeated adding it.
        /// </summary>
        /// <param name="userRawPhrase">Input phrase as a string.</param>
        public static BotPhrase.Phrase CreateOrRetreavePhrase(string userRawPhrase) {
            var phrase = Recognizer.RecognizePhrase(userRawPhrase);
            var modelPhrase = PhrasesConverter.PhraseToModelPhrase(phrase);
            try {
                using (EDModelContainer db = new EDModelContainer()) {
                    // Maybe I should use an exposed IEqualityComparer class and .Contains(modelPhrase), but I failed to make this way working.
                    if (db.Phrases.Where((p) => p.ActionId == modelPhrase.ActionId
                            && p.AdditionalTextId == modelPhrase.AdditionalTextId
                            && p.Amount == modelPhrase.Amount
                            && p.CorrectedMessage == modelPhrase.CorrectedMessage
                            && p.Date == modelPhrase.Date
                            && p.MeasureUnitId == modelPhrase.MeasureUnitId
                            && p.OriginalMessage == modelPhrase.OriginalMessage
                            && p.Time == modelPhrase.Time
                            && p.WasRecognized == modelPhrase.WasRecognized).Count() == 0) {
                        db.Phrases.Add(modelPhrase);
                        db.SaveChanges();
                        return phrase;
                    }
                    else {
                        Debug.Indent();
                        Debug.WriteLine("there already is such Phrase in DB.");
                        Debug.Unindent();
                        return phrase;
                    }
                }
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