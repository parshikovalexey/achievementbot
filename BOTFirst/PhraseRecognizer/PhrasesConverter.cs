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
                YouDid = (modelPhrase.Action != null) ? "Ты сделал — " + modelPhrase.Action.Text : "действие не указано",
                HowMuch = (modelPhrase.Amount != null) ? "Сколько? — " + modelPhrase.Amount.ToString() : "количество отсутствует",
                Units = (modelPhrase.MeasureUnit != null) ? "Чего? — " + modelPhrase.MeasureUnit.Text : "единицы измерения не указаны",
                WhatWhere = (modelPhrase.AdditionalText != null) ? "Что?/Кому?/Каким?/Куда? — " + modelPhrase.AdditionalText.Text : "объект либо состояние не были указаны",
                Date = (modelPhrase.Date != "") ? "Когда? — Дата: " + modelPhrase.Date : "дата не была указана",
                Time = (modelPhrase.Date != "") ? "Когда? — Время: " + modelPhrase.Time : "время не было указано",
            };
        }

    }
}
