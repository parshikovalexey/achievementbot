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

    }
}