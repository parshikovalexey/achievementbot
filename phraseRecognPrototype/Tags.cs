using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordClassTagger
{
    public static class Tags
    {
        public enum TagsEnum
        {
            A, ADV, ADVPRO, ANUM, APRO, COM, CONJ, INTJ, NUM, PART, PR, S, SPRO, V, UNDEFINED, UNRECOGNIZED
        }

        // Converting from int of enum to string
        public static List<string> StringTagsForConverting = new List<string>
        {
            "<A>", "<ADV>", "<ADVPRO>", "<ANUM>", "<APRO>", "<COM>", "<CONJ>",
            "<INTJ>", "<NUM>", "<PART>", "<PR>", "<S>", "<SPRO>", "<V>", "<ЕщеНеЗадан>", "<НеРаспознан>"
        };
        
        public static int StringTagToEnumTag(string tag)
        {
            return StringTagsForConverting.FindIndex((element) => element == tag);
        }

        public static string EnumTagToStringTag(int tag)
        {
            return StringTagsForConverting[tag];
        }

        /*
         * A - ПРИЛАГАТЕЛЬНОЕ
         * ADV - НАРЕЧИЕ
         * ADVPRO - МЕСТОИМЕННОЕ НАРЕЧИЕ (ГДЕ-ТО, ДОСЕЛЕ, ЗАЧЕМ-ТО, КОЕ-КАК)
         * ANUM - ЧИСЛИТЕЛЬНОЕ-ПРИЛАГАТЕЛЬНОЕ (ВОСЬМАЯ, ВТОРАЯ)
         * APRO - МЕСТОИМЕНИЕ-ПРИЛАГАТЕЛЬНОЕ (КОЕ, КОТОРАЯ, МОЙ, НЕКОТОРЫЙ, НИКАКАЯ, САМ)
         * COM - ЧАСТЬ КОМПОЗИТА - СЛОЖНОГО СЛОВА (АНТИ)
         * CONJ - СОЮЗ
         * INTJ - МЕЖДОМЕТИЕ
         * NUM - ЧИСЛИТЕЛЬНОЕ
         * PART - ЧАСТИЦА
         * PR - ПРЕДЛОГ
         * S - СУЩЕСТВИТЕЛЬНОЕ
         * SPRO - МЕСТОИМЕНИЕ-СУЩЕСТВИТЕЛЬНОЕ
         * V - ГЛАГОЛ
         */
    }
}
