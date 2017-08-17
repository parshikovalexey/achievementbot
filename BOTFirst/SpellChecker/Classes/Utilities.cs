using System;
using Newtonsoft.Json.Linq;

namespace BOTFirst.Spellchecker
{
    /// <summary>
    /// Включает в себя полезные методы используемые в проекте
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Включает в себя раздел касающийся обработки JSON
        /// </summary>
        public static class JSON
        {
            /// <summary>
            /// Выбирает объект класса JToken по ключу
            /// </summary>
            /// <param name="token">Токен из которого необходимо выбрать объект</param>
            /// <param name="key">Ключ по которому нужно выбрать объект</param>
            /// <param name="error">Возможная ошибка(передается по ссылке)</param>
            public static JToken GetObjectByKey(JToken token, string key, ref Exception error)
            {
                JToken result = token.SelectToken(key); // не выбрасывает исключения в любом случае
                if (result == null)
                {
                    error = new InvalidOperationException("Объект не содержит поля с таким ключом");
                }
                return result;
            }
            /// <summary>
            /// Возвращает значение выбранное из объекта JToken по ключу
            /// </summary>
            /// <param name="token">Токен из которого необходимо получить значение</param>
            /// <param name="key">Ключ по которому нужно получить значение</param>
            /// <param name="error">Возможная ошибка(передается по ссылке)</param>
            public static T GetValueByKey<T>(JToken token, string key, ref Exception error)
            {
                JToken takenObject = GetObjectByKey(token, key, ref error);
                if (takenObject != null)
                {
                    return takenObject.Value<T>(); // возвращаем значение
                }
                else
                {
                    return default(T); // или дефолтное значение выбранного типа
                }
            }
        }
    }
}
