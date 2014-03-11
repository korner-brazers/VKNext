using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace NewAPI.engine.VK_API
{
    class UserGet
    {
        public string get(string token)
        {
            int CountRefresh = 0;
            Refresh: string tmp = new PostAPI().post("https://api.vk.com/method/users.get", ("&access_token=" + token), null, null, null, null).Replace("\\/", "/").Replace(" ", "").ToLower().Trim();
            string id = new Regex("\"uid\":([0-9]+),?\"").Match(tmp).Groups[1].Value;

            //Проверка на капчу и получение URL
            if ((new Regex("userauthorizationfailed:invalidaccess_token").Match(tmp)).Success || CountRefresh == 3)
            {
                Console.WriteLine("Error UserGet.cs: {0}\nToken: {1}\nНажмите любую клавишу для завершения программы.", new Regex("\"error_msg\":\"([^\"]+)\"").Match(tmp).Groups[1].Value, token);
                Console.ReadKey();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else if ((id == null || id.Trim() == "") && CountRefresh != 3)
            {
                id = null; tmp = null; CountRefresh++; goto Refresh;
            }

            //Чистим ресурсы
            tmp = null; token = null;
            return id.Trim();
        }
    }
}
