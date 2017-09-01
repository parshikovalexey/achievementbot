using System;
using System.Collections.Generic;
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
                Action = (modelPhrase.Action != null) ?  modelPhrase.Action.Text : "действие не указано",
                Amount = (modelPhrase.Amount != null) ? modelPhrase.Amount.ToString() : "количество отсутствует",
                Units = (modelPhrase.MeasureUnit != null) ? modelPhrase.MeasureUnit.Text : "единицы измерения не указаны",
                AdditionalText = (modelPhrase.AdditionalText != null) ? modelPhrase.AdditionalText.Text : "объект либо состояние не были указаны",
                Date = (modelPhrase.Date != "") ? modelPhrase.Date : "дата не была указана",
                Time = (modelPhrase.Date != "") ? modelPhrase.Time : "время не было указано",
            };
        }
        public static EntityModel.Phrase PhraseToModelPhrase(Phrase phrase)
         {
             return new EntityModel.Phrase
             {
                 Action = new EntityModel.Action() { Text = phrase.Action },
                 WasRecognized = phrase.WasRecognized,
                 Date = phrase.Date,
                 Time = phrase.Time,
                 Amount = decimal.Parse(phrase.Amount),
                 MeasureUnit = new EntityModel.MeasureUnit() { Text = phrase.Units },
                 CorrectedMessage = "Коррекция правописания еще не реализована",
                 AdditionalText = new EntityModel.AdditionalText() { Text = phrase.AdditionalText }
             };
         }
        

    }
}
