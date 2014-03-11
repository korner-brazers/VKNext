using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace NewAPI.engine.VK_API
{
    class PostAPI
    {
        public string post(string url, string data, string source_id, string post_id, string AccountID, string GroupID)
        {
            try
            {
                //Типа разделитель и данные для работы 
                string captcha = "";
                Refresh: string captcha_img = null;
                HttpWebRequest request = null;
                Stream stream = null, StreamHTML = null;
                StreamReader Reader = null;
                WebClient wc = new WebClient();
                WebResponse response = null;
                string result = null;

                try
                {
                    //Создаем подключение с URL для загрузки картинок
                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Method = "POST";
                    request.KeepAlive = true;
                    request.MaximumAutomaticRedirections = 3;
                    request.ReadWriteTimeout = 1000 * 30;
                    request.Timeout = 1000 * 30;

                    //параметры
                    byte[] info = Encoding.UTF8.GetBytes(data + captcha);

                    //Записываем в поток все данные
                    stream = request.GetRequestStream();
                    stream.Write(info, 0, info.Length);
                    info = null;

                    //Возвращаем данные 
                    response = request.GetResponse();
                    StreamHTML = response.GetResponseStream();
                    Reader = new StreamReader(StreamHTML);
                    result = Reader.ReadToEnd();
                    captcha_img = new Regex("captcha_img\":\"([^\"]+)\"").Match(result.Replace(" ", "").Replace(@"\", "").ToLower().Trim()).Groups[1].Value;
                }
                catch (Exception e)
                {
                    if ((source_id == null || post_id == null) && AccountID != null && GroupID != null)
                        Console.WriteLine("error: Ошибка во время запроса в PostAPI, ID аккаунта: {1}, группа: {2}", AccountID, GroupID);
                    else if (source_id != null && post_id != null && AccountID != null && GroupID != null)
                        Console.WriteLine("error: Ошибка во время публикации новости https://vk.com/wall{0}_{1} в PostAPI, ID аккаунта: {2}, группа: {3}", source_id, post_id, AccountID, GroupID);
                    else
                        Console.WriteLine("error: Ошибка во время запроса в PostAPI..");

                    if (e != null)
                        Console.WriteLine("Код ошибки: {1}{0}{1}{1}", e, Environment.NewLine);
                }
                finally
                {
                    //Чистим ресурсы
                    wc.Dispose(); wc = null;
                    if (Reader != null)
                    {
                        Reader.Close(); Reader.Dispose(); Reader = null;
                    }
                    if (StreamHTML != null)
                    {
                        StreamHTML.Close(); StreamHTML.Dispose(); StreamHTML = null;
                    }
                    if (response != null)
                    {
                        response.Close(); response.Dispose(); response = null;
                    }
                    if (stream != null)
                    {
                        stream.Close(); stream.Dispose(); stream = null;
                    }
                    if (request != null)
                    {
                        request.Abort(); request = null;
                    }
                }

                //Капча
                if (captcha_img != null && captcha_img.Replace(" ", "").Trim() != "")
                {
                    if (AccountID != null && GroupID != null)
                        Console.WriteLine("info: Разгадываем капчу в PostAPI.cs, ID аккаунта: {0}, группа {1}", AccountID, GroupID);
                    else
                        Console.WriteLine("info: Разгадываем капчу в PostAPI.cs..");
                    captcha = new Antigate().get(captcha_img);
                    captcha_img = null;
                    goto Refresh;
                }

                //Возвращаем результат 
                url = null; data = null; source_id = null; post_id = null; AccountID = null; GroupID = null; captcha = null; captcha_img = null;
                return result.Trim() == "" ? null : result.Trim();
            }
            catch (Exception e) 
            {
                if (e != null)
                    Console.WriteLine("Ошибка в PostAPI.cs, код ошибки: {1}{0}{1}{1}", e, Environment.NewLine);

                return (url = null);
            }
        }
    }
}
