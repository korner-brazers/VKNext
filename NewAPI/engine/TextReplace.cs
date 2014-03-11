using System.Text.RegularExpressions;

namespace NewAPI.engine
{
    class TextReplace
    {
        public string get(string s, bool ReplaceTag)
        {
            if (s == null)
                return null;

            //Меняем текст
            s = Regex.Replace(s, "@[^ ]+", "");  //@группа
            s = s.Replace("<br>", "\n");


            //Вырезаем теги
            if (ReplaceTag)
                s = Regex.Replace(s, "#[^ ]+", "");  //#тег

            return s;
        }
    }
}
