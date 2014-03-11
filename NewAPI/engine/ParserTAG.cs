using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace NewAPI.engine
{
    class ParserTAG
    {
        public void get(List<string> TempTag, InfoClass.account account)
        {
            List<string> tmp = new List<string> { };
            if (TempTag.Count == 0)
            {
                TempTag = null; account = null;
                return;
            }


            //Вытаскиваем теги
            foreach (string s in TempTag)
                tmp.AddRange(GetTag(s));


            //Создаем папку если нужно
            if (!Directory.Exists(conf.DirTag))
                Directory.CreateDirectory(conf.DirTag);


            //Записываем новые теги
            foreach (string s1 in tmp)
            {
                //Проверяем по хеш тегам
                bool dublicat = false;
                foreach(string s2 in account.Tag)
                {
                    if (s1.ToLower().Trim() == s2.Trim())
                        dublicat = true;
                }

                //Проверяем по нашим хеш тегам
                if (!dublicat)
                {
                    foreach (string s2 in account.GroupKey)
                    {
                        if (s1.ToLower().Replace("#", "").Trim() == s2.Replace("#", "").ToLower().Trim())
                            dublicat = true;
                    }
                }

                //Добовляем новые хеш теги
                if (!dublicat)
                {
                    account.Tag.Add(s1.ToLower().Trim());
                    File.AppendAllText(conf.DirTag + account.id, s1.Trim() + Environment.NewLine);
                }
            }

            //Чистим ресурсы
            tmp = null; TempTag = null; account = null;
        }


        private List<string> GetTag(string s)
        {
            List<string> tmp = new List<string> { };
            Match matc = new Regex(@"#[a-zA-Z0-9_]+").Match(s);
            while (matc.Success) 
            {
                tmp.Add(matc.Value); 
                matc = matc.NextMatch(); 
            }
            s = null; matc = null;
            return tmp;
        }
    }
}
