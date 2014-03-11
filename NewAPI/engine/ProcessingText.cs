using System;
using System.Collections.Generic;
using System.Text;

namespace NewAPI.engine
{
    class ProcessingText
    {
        public static string get(List<string> key, int count, InfoClass.account account)
        {
            if (key == null || key.Count == 0)
            {
                key = null; account = null;
                return "";
            }

            //Если надо вернуть все ключи
            if (key.Count <= count)
            {
                StringBuilder res = new StringBuilder();
                foreach (string s in key)
                    res.Append(s + " ");

                key = null; account = null;
                return res.ToString().Replace("<br>", "\n").Trim();
            }

            //Возвращаем рандомовские ключи
            string result = "";
            Random rng = new Random();
            for (int i = 0, x = 0; i < count; x++)
            {
                string s = key[rng.Next(0, key.Count)].Trim();
                if (!result.Contains(s.Replace("#", "").Trim()))
                {
                    result += s.Trim() + " ";
                    i++;
                }
                else if (x > 1000)
                    break;

                s = null;
            }

            //Возвращаем результат
            string MessageURL = account.URL_Text != null ? account.URL_Text : "";
            key = null; rng = null; account = null;
            return result.Replace("<br>", "\n").Trim() + MessageURL;
        }
    }
}
