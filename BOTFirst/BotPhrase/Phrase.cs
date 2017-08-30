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
        public string What { get; set; }
        //Что?/Кому?/Каким?/Куда?
        public string WhatWhere { get; set; }
        //Когда
        public string Data { get; set; }

        public override string ToString()
        {
            var messsag = new StringBuilder();
            messsag.AppendLine(YouDid + " " + HowMuch + " " + What);
            messsag.AppendLine(WhatWhere);
            messsag.AppendLine(Data);
            return messsag.ToString();
           
        }

    }
}