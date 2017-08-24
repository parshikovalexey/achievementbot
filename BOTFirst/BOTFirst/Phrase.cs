using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BOTFirst
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

        public string MoveTo(string a, string b, string c, string d, string f)
        {
            var messsag = new StringBuilder();
            messsag.AppendLine(YouDid+" "+ HowMuch+" "+ What);
            messsag.AppendLine(WhatWhere);
            messsag.AppendLine(Data);
            return messsag.ToString();
        }

        
    }
}