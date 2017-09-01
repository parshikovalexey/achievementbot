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
        // Ты сделал.
        public string Action { get; set; }
        // Сколько.
        public string Amount { get; set; }
        // Чего.
        public string Units { get; set; }
        // Что ? Кому ? Каким ? Куда ?
        public string AdditionalText { get; set; }
        // Когда, дата.
        public string Date { get; set; }
        // Когда, время.
        public string Time { get; set; }

        public DateTime PhraseDateTime { get; set; }

        public string RecognitionResult { get; set; }

        public Boolean WasRecognized { get; set; }

        public string OriginalMessage { get; set; }

       /* public override string ToString()
        {
            var messsage = new StringBuilder();
            messsage.AppendLine(YouDid + " " + HowMuch + " " + Units);
            messsage.AppendLine(WhatWhere);
            messsage.AppendLine(Date);
            messsage.AppendLine(Time);
            return messsage.ToString();
           
        }*/
        

    }
}