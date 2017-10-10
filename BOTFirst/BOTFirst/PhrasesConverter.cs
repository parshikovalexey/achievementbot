using EntityModel;
using System;
using System.Diagnostics;
using System.Linq;

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
                var action = db.Actions.Where(a => a.Text == phrase.Action).FirstOrDefault();
                if (action != null) {
                    modelPhrase.ActionId = action.Id;
                    Debug.WriteLine("there already is such action in DB.");
                }
                else
                    modelPhrase.Action = (phrase.Action != null) ? new EntityModel.Action() { Text = phrase.Action } : null;

                var measureUnit = db.MeasureUnits.Where(mu => mu.Text == phrase.Units).FirstOrDefault();
                if (measureUnit != null) {
                    modelPhrase.MeasureUnitId = measureUnit.Id;
                    Debug.WriteLine("there already is such MeasureUnit in DB.");
                }
                else
                    modelPhrase.MeasureUnit = (phrase.Units != null) ? new EntityModel.MeasureUnit() { Text = phrase.Units } : null;

                var additionalText = db.AdditionalTexts.Where(at => at.Text == phrase.AdditionalText).FirstOrDefault();
                if (additionalText != null) {
                    modelPhrase.AdditionalTextId = additionalText.Id;
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
                var action = db.Actions.Where(a => a.Text == phrase.Action).FirstOrDefault();
                if (action != null) {
                    // We assign a navigation property here.
                    // But correspond Id is not defined and furthermore the property will be null after getting this record from DB.
                    // Also a duplicate action will be created. This happens when we assign an object that already exists in DB.
                    // Maybe the thing about this is that I assign navigation property improperly.
                    modelPhrase.Action = action;
                    Debug.WriteLine("there already is such action in DB.");
                }
                else
                    modelPhrase.Action = (phrase.Action != null) ? new EntityModel.Action() { Text = phrase.Action } : null;

                var measureUnit = db.MeasureUnits.Where(mu => mu.Text == phrase.Units).FirstOrDefault();
                if (measureUnit != null) {
                    // We assign a navigation property here.
                    // But correspond Id is not defined and furthermore the property will be null after getting this record from DB.
                    // Also a duplicate MeasureUnit will be created.
                    modelPhrase.MeasureUnit = measureUnit;
                    Debug.WriteLine("there already is such MeasureUnit in DB.");
                }
                else
                    modelPhrase.MeasureUnit = (phrase.Units != null) ? new EntityModel.MeasureUnit() { Text = phrase.Units } : null;

                var additionalText = db.AdditionalTexts.Where(at => at.Text == phrase.AdditionalText).FirstOrDefault();
                if (additionalText != null) {
                    // We assign a navigation property here.
                    // But correspond Id is not defined and furthermore the property will be null after getting this record from DB.
                    // Also a duplicate AdditionalText will be created.
                    modelPhrase.AdditionalText = additionalText;
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
