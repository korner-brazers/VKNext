using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace NewAPI.engine.VK_API
{
    class UploadURL
    {
        public string get(string token, string gPost)
        {
            int CountRefresh = 0;
            Refresh: string tmp = new PostAPI().post("https://api.vkontakte.ru/method/photos.getWallUploadServer", ("group_id=" + gPost.Replace("-", "").Trim() + "&access_token=" + token), null, null, null, null).Replace("\\/", "/").Trim();
            string url = new Regex("\"upload_url\":\"([^\"]+)\"").Match(tmp).Groups[1].Value;

            //Проверка на капчу и получение URL
            if ((new Regex("userauthorizationfailed:invalidaccess_token").Match(tmp.ToLower().Replace(" ", ""))).Success || CountRefresh == 3)
            {
                Console.WriteLine("Error UploadURL.cs: {0}\nToken: {1}\nНажмите любую клавишу для завершения программы.", new Regex("\"error_msg\":\"([^\"]+)\"").Match(tmp).Groups[1].Value, token);
                Console.ReadKey();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else if ((url == null || url.Trim() == "") && CountRefresh != 3)
            {
                url = null; tmp = null; CountRefresh++; goto Refresh;
            }

            //Чистим ресурсы
            tmp = null; token = null; gPost = null;
            return url;
        }
    }
}
