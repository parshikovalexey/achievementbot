using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BotPhrase
{
    [Serializable]
    public class Phrase
    {
        //ты сделал
        public string YouDid { get; set; }
        //Сколько
        public string HowMuch { get; set; }
        //Чего
        public string Units { get; set; }
        //Что?/Кому?/Каким?/Куда?
        public string WhatWhere { get; set; }
        //Когда
        public string Date { get; set; }

        public string Time { get; set; }

        DateTime phraseDateTime { get; set; }

        public override string ToString()
        {
            var messsage = new StringBuilder();
            messsage.AppendLine(YouDid + " " + HowMuch + " " + Units);
            messsage.AppendLine(WhatWhere);
            messsage.AppendLine(Date);
            messsage.AppendLine(Time);
            return messsage.ToString();
           
        }
        //Вот это я добавил пока думал
        public Phrase PhraseToModul(EntityModel.Phrase modelPhrase)
        {
            return new Phrase
            {
                YouDid = modelPhrase.Action.Text,
                HowMuch = modelPhrase.Amount.ToString(),
                Units = modelPhrase.MeasureUnit.Text,
                WhatWhere = modelPhrase.AdditionalText.Text,
                Date = modelPhrase.Date,
                Time = modelPhrase.Time,
                phraseDateTime = DateTime.Parse(modelPhrase.Date+ modelPhrase.Time)
            };
        }

    }
}