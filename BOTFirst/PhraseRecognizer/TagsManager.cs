using System;

namespace WordClassTagger {
    public static class TagsManager {
        private static TagsEnum res;
        public enum TagsEnum {
            A, ADV, ADVPRO, ANUM, APRO, COM, CONJ, INTJ, NUM, PART, PR, S, SPRO, V, UNDEFINED, UNRECOGNIZED
        }

        /* A - ПРИЛАГАТЕЛЬНОЕ
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
         * V - ГЛАГОЛ */

        public static TagsEnum StringTagToEnumTag(string tag) {
            Enum.TryParse<TagsEnum>(tag.Substring(1, tag.Length - 2), out res);
            return res;
        }

        public static string EnumTagToStringTag(TagsManager.TagsEnum tag) {
            return "<" + Enum.GetName(typeof(TagsEnum), tag) + ">";
        }
    }
}
