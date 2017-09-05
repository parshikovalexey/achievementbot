﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordClassTagger {
    public static class ConstantValues {
        // Markdown is used at bot framework, "\r\n" doesn't work.
        public const string NewLine = "  \n";

        public enum DateUnits {
            DAYS, WEEKS, MONTHS, YEARS
        }

        // It is used for date recognition.
        // Or to use "readonly string[]"?
        public static readonly ReadOnlyCollection<string> Months = new ReadOnlyCollection<string>(new List<string> { "(января)", "(февраля)", "(марта)", "(апреля)", "(мая)", "(июня)", "(июля)", "(августа)", "(сентября)", "(октября)", "(ноября)", "(декабря)" });
        // Examples: осенью 2012 года, летом 12 года.
        public static readonly ReadOnlyCollection<string> Seasons = new ReadOnlyCollection<string>(new List<string> { "(летом?)", "(осенью?)", "(по осени)", "(зима)", "(зимой)", "(по зиме)", "(весна)", "(весной)", "(по весне)" });
        // NOTE Bot would ask the user how they define and understand the "fuzzy" terms as such as "" or maybe better we should refuse the logic as unnecessary.
        // NOTE "в" preposition, eg. "в этом году" or "во вторник" is being accounting in Tokenizer.CommonDatePattern.
        public static readonly ReadOnlyCollection<string> AlphabeticDates = new ReadOnlyCollection<string>(new List<string> { "(сегодня)", "(вчера)", "(позавчера)", "(на (этой)? неделе)", "(на прошлой неделе)", "(на позапрошлой неделе)", "(этом месяце)", "(прошлом месяце)", "(позапрошлом месяце)", "(этом году)", "(прошлом году)", "(позапрошлом году)", "(на днях)", "(понедельник)", "(вторник)", "(сред(а|у))", "(четверг)", "(пятниц(а|у))", "(суббот(а|у))", "(воскресенье)" });

        // It is used for amount recognition.
        // 1/2, 1/10 etc. will be recognized by tagger, type is List in order to use its Contains method.
        public static readonly List<string> AmountByWords = new List<string> { "половину", "треть", "четверть", "много", "немного", "уйму", "кучу", "тучу" };

        // It is used for units of measure recognition.
        public static readonly List<string> Units = new List<string> { "(раза?)", "(км/ч)", "(кг)", "(килограмм((а)|(ов))?)", "(м)", "(метр((а)|(ов))?)", "(километр((а)|(ов))?)", "(грамм((а)|(ов))?)", "(литр((а)|(ов))?)", "(страницу?)", "(главу?)", "(книг?)" };
    }
}
