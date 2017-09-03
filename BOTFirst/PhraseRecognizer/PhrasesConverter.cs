using EntityModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotPhrase
{
    public static class PhrasesConverter
    {
        public static Phrase ModelPhraseToPhrase(EntityModel.Phrase modelPhrase)
        {
            return new Phrase()
            {
                Action = (modelPhrase.Action != null) ? modelPhrase.Action.Text : "действие не указано",
                Amount = (modelPhrase.Amount != null) ? modelPhrase.Amount.ToString() : "количество отсутствует",
                Units = (modelPhrase.MeasureUnit != null) ? modelPhrase.MeasureUnit.Text : "единицы измерения не указаны",
                AdditionalText = (modelPhrase.AdditionalText != null) ? modelPhrase.AdditionalText.Text : "объект либо состояние не были указаны",
                Date = (modelPhrase.Date != "") ? modelPhrase.Date : "дата не была указана",
                Time = (modelPhrase.Date != "") ? modelPhrase.Time : "время не было указано",
            };
        }

        public static EntityModel.Phrase PhraseToModelPhrase(Phrase phrase)
        {
            var modelPhrase = new EntityModel.Phrase
            {
                WasRecognized = phrase.WasRecognized,
                OriginalMessage = phrase.OriginalMessage,
                Date = phrase.Date,
                Time = phrase.Time,
                CorrectedMessage = "Коррекция правописания еще не реализована",
            };
            using (EDModelContainer db = new EDModelContainer())
            {
                Debug.Indent();
                if (db.Actions.Any() && db.Actions.Where((t) => t.Text == phrase.Action).Count() > 0)
                {
                    modelPhrase.ActionFId = db.Actions.Where((t) => t.Text == phrase.Action).First().Id;
                    Debug.WriteLine("there already is such action.");
                }
                else
                    modelPhrase.Action = (phrase.Action != null) ? new EntityModel.Action() { Text = phrase.Action } : null;

                if (db.MeasureUnits.Any() && db.MeasureUnits.Where((t) => t.Text == phrase.Units).Count() > 0)
                {
                    modelPhrase.MeasureUnitFId = db.MeasureUnits.Where((t) => t.Text == phrase.Units).First().Id;
                    Debug.WriteLine("there already is such MeasureUnit.");
                }
                else
                    modelPhrase.MeasureUnit = (phrase.Units != null) ? new EntityModel.MeasureUnit() { Text = phrase.Units } : null;

                if (db.AdditionalTexts.Any() && db.AdditionalTexts.Where((t) => t.Text == phrase.AdditionalText).Count() > 0)
                {
                    modelPhrase.AdditionalTextFId = db.AdditionalTexts.Where((t) => t.Text == phrase.AdditionalText).First().Id;
                    Debug.WriteLine("there already is such AdditionalText."); 
                }
                else
                    modelPhrase.AdditionalText = (phrase.AdditionalText != null) ? new EntityModel.AdditionalText() { Text = phrase.AdditionalText } : null;

                Debug.Unindent();
                

                if (phrase.Amount != null)
                    try
                    {
                        decimal.Parse(phrase.Amount);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                // TODO Implement checking of result phrase existence in DB and returning the existing phrase.
                return modelPhrase;
            }
        }

    }
}

