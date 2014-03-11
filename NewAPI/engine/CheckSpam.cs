using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace NewAPI.engine
{
    class CheckSpam
    {
        public bool get(string text, List<string> StopWords)
        {
            //Переводим в нижний регистр
            text = text.ToLower();

            //Ссылка на внешний ресурс или ссылка внутри контакта
            if (new Regex(@"https?://").Match(text.Replace(" ", "")).Success || new Regex(@"<a[^\|]+\|[^<]+</a>").Match(text.Replace("[", "<a").Replace("]", "</a>")).Success)
            {
                text = null; StopWords = null;
                return true;
            }

            //Возможно ссылка но текстом и с пробелами
            if (new Regex(@"[a-z0-9-]+\.[a-z]{2,4}").Match(text.Replace(" ", "").Replace(",", "")).Success)
            {
                text = null; StopWords = null;
                return true;
            }

            //Почта
            if (new Regex(@"[a-z0-9-]+@[a-z0-9-]+\.[a-z]{2,4}").Match(text).Success)
            {
                text = null; StopWords = null;
                return true;
            }

            //Список пользователя
            if (StopWords != null)
            {
                foreach (string s in StopWords)
                    if (text.ToLower().Replace(" ", "").Trim().Contains(s.ToLower().Replace(" ", "").Trim()))
                    {
                        text = null; StopWords = null;
                        return true;
                    }
            }

            //Если спама нету
            text = null; StopWords = null;
            return false;
        }
    }
}
