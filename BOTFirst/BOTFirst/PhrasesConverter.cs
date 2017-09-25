using EntityModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BotPhrase {
    public static class PhrasesConverter {
        public static Phrase ModelPhraseToPhrase(EntityModel.Phrase modelPhrase) {
            return new Phrase() {
                Action = (modelPhrase.Action != null) ? modelPhrase.Action.Text : "действие не указано",
                Amount = (modelPhrase.Amount != null) ? modelPhrase.Amount.ToString() : "количество отсутствует",
                Units = (modelPhrase.MeasureUnit != null) ? modelPhrase.MeasureUnit.Text : "единицы измерения не указаны",
                AdditionalText = (modelPhrase.AdditionalText != null) ? modelPhrase.AdditionalText.Text : "объект либо состояние не были указаны",
                Date = (modelPhrase.Date != "") ? modelPhrase.Date : "дата не была указана",
                Time = (modelPhrase.Date != "") ? modelPhrase.Time : "время не было указано",
            };
        }

        public static EntityModel.Phrase PhraseToModelPhrase(Phrase phrase) {
            var modelPhrase = new EntityModel.Phrase {
                WasRecognized = phrase.WasRecognized,
                OriginalMessage = phrase.OriginalMessage,
                Date = phrase.Date,
                Time = phrase.Time,
                CorrectedMessage = "Коррекция правописания еще не реализована",
            };
            using (EDModelContainer db = new EDModelContainer()) {
                Debug.Indent();
                if (db.Actions.Any(t => t.Text == phrase.Action)) {
                    var existingActionId = db.Actions.Where(a => a.Text == phrase.Action).Select(a => a.Id).First();
                    modelPhrase.ActionId = existingActionId;
                    Debug.WriteLine("there already is such action in DB.");
                }
                else
                    modelPhrase.Action = (phrase.Action != null) ? new EntityModel.Action() { Text = phrase.Action } : null;

                if (db.MeasureUnits.Any(t => t.Text == phrase.Units)) {
                    var existingMeasureUnitId = db.MeasureUnits.Where(mu => mu.Text == phrase.Units).Select(mu => mu.Id).First();
                    modelPhrase.MeasureUnitId = existingMeasureUnitId;
                    Debug.WriteLine("there already is such MeasureUnit in DB.");
                }
                else
                    modelPhrase.MeasureUnit = (phrase.Units != null) ? new EntityModel.MeasureUnit() { Text = phrase.Units } : null;

                if (db.AdditionalTexts.Any(t => t.Text == phrase.AdditionalText)) {
                    var existingAdditionalTextId = db.AdditionalTexts.Where(at => at.Text == phrase.AdditionalText).Select(at => at.Id).First();
                    modelPhrase.AdditionalTextId = existingAdditionalTextId;
                    Debug.WriteLine("there already is such AdditionalText in DB.");
                }
                else
                    modelPhrase.AdditionalText = (phrase.AdditionalText != null) ? new EntityModel.AdditionalText() { Text = phrase.AdditionalText } : null;
                Debug.Unindent();

                if (phrase.Amount != null)
                    try {
                        modelPhrase.Amount = decimal.Parse(phrase.Amount);
                    }
                    catch (Exception) {
                        throw;
                    }
                return modelPhrase;
            }
        }
        public static EntityModel.Phrase ShowingErrorPhraseToModelPhrase(Phrase phrase) {
            var modelPhrase = new EntityModel.Phrase {
                WasRecognized = phrase.WasRecognized,
                OriginalMessage = phrase.OriginalMessage,
                Date = phrase.Date,
                Time = phrase.Time,
                CorrectedMessage = "Коррекция правописания еще не реализована",
            };
            using (EDModelContainer db = new EDModelContainer()) {
                Debug.Indent();
                if (db.Actions.Any(t => t.Text == phrase.Action)) {
                    var existingAction = db.Actions.Where(a => a.Text == phrase.Action).First();
                    // We assign a navigation property here.
                    // But correspond Id is not defined and furthermore the property will be null after getting this record from DB.
                    // Also a duplicate action will be created. This happens when we assign an object that already exists in DB.
                    // Maybe the thing about this is that I assign navigation property improperly.
                    modelPhrase.Action = existingAction;
                    Debug.WriteLine("there already is such action in DB.");
                }
                else
                    modelPhrase.Action = (phrase.Action != null) ? new EntityModel.Action() { Text = phrase.Action } : null;

                if (db.MeasureUnits.Any(t => t.Text == phrase.Units)) {
                    // We assign a navigation property here.
                    // But correspond Id is not defined and furthermore the property will be null after getting this record from DB.
                    // Also a duplicate MeasureUnit will be created.
                    var existingMeasureUnit = db.MeasureUnits.Where(mu => mu.Text == phrase.Units).First();
                    modelPhrase.MeasureUnit = existingMeasureUnit;
                    Debug.WriteLine("there already is such MeasureUnit in DB.");
                }
                else
                    modelPhrase.MeasureUnit = (phrase.Units != null) ? new EntityModel.MeasureUnit() { Text = phrase.Units } : null;

                if (db.AdditionalTexts.Any(t => t.Text == phrase.AdditionalText)) {
                    var existingAdditionalText = db.AdditionalTexts.Where(at => at.Text == phrase.AdditionalText).First();
                    // We assign a navigation property here.
                    // But correspond Id is not defined and furthermore the property will be null after getting this record from DB.
                    // Also a duplicate AdditionalText will be created.
                    modelPhrase.AdditionalText = existingAdditionalText;
                    Debug.WriteLine("there already is such AdditionalText in DB.");
                }
                else
                    modelPhrase.AdditionalText = (phrase.AdditionalText != null) ? new EntityModel.AdditionalText() { Text = phrase.AdditionalText } : null;
                Debug.Unindent();

                if (phrase.Amount != "")
                    try {
                        modelPhrase.Amount = decimal.Parse(phrase.Amount);
                    }
                    catch (Exception) {
                        throw;
                    }
                return modelPhrase;
            }
        }
    }
}

