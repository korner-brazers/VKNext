using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;

namespace NewAPI.engine
{
    class Antigate
    {
        public string get(string s)
        {
            if (s == null || s.Trim() == "")
                return "";

            string id = UploadCAPCHA(s), key = "";
            if (id == null || id.Trim() == "")
                return "";

            //Примерно 50 секунд на ожидание капчи
            for (int i = 0; i < 5; i++)
            {
                //Ждем 10 секунд и получаем значение
                Thread.Sleep(1000 * 10);
                string tmp = HttpGet(id);

                //Капча не готова
                if (tmp == null || tmp.ToUpper() == "CAPCHA_NOT_READY")
                {
                    tmp = null;
                    continue;
                }

                //Получили капчу
                if (new Regex(@"^[okOK]{2}\|([0-9a-zA-Z]+)$").Match(tmp).Success)
                {
                    key = new Regex(@"^[okOK]{2}\|([0-9a-zA-Z]+)$").Match(tmp).Groups[1].Value;
                    break;
                }
                tmp = null;
            }

            id = null;
            return string.Format("&captcha_sid={0}&captcha_key={1}", new Regex("sid=([0-9]+)").Match(s).Groups[1].Value.Trim(), key.Trim()).Trim();
        }


        private string UploadCAPCHA(string url)
        {
            try
            {
                //Типа разделитель и данные для работы 
                string str = DateTime.Now.Ticks.ToString("x");
                byte[] boundary = Encoding.UTF8.GetBytes("\n--" + str + "\n");
                HttpWebRequest request = null;
                Stream stream = null, StreamHTML = null;
                StreamReader Reader = null;
                WebClient wc = new WebClient();
                WebResponse response = null;
                string result = null;

                try
                {
                    //Создаем подключение с URL для загрузки картинок
                    request = (HttpWebRequest)WebRequest.Create("http://antigate.com/in.php");
                    request.ContentType = "multipart/form-data; boundary=" + str;
                    request.Method = "POST";
                    request.KeepAlive = true;
                    request.MaximumAutomaticRedirections = 3;
                    request.ReadWriteTimeout = 1000 * 30;
                    request.Timeout = 1000 * 30;


                    //Хедеры
                    byte[] image = wc.DownloadData(url);
                    byte[] header = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"method\"\n\npost\n--{0}\n", str) +
                                                           string.Format("Content-Disposition: form-data; name=\"key\"\n\n{0}\n--{1}\n", conf.KeyAntigate, str) +
                                                           "Content-Disposition: form-data; name=\"file\"; filename=\"image.jpg\"\nContent-Type: \"image/jpg\"\n\n");


                    //Записываем в поток все данные
                    stream = request.GetRequestStream();
                    stream.Write(boundary, 0, boundary.Length);
                    stream.Write(header, 0, header.Length);
                    stream.Write(image, 0, image.Length);
                    stream.Write(boundary, 0, boundary.Length);
                    image = null; header = null;


                    //Возвращаем данные 
                    response = request.GetResponse();
                    StreamHTML = response.GetResponseStream();
                    Reader = new StreamReader(StreamHTML);
                    result = new Regex(@"ok\|([0-9]+)").Match(Reader.ReadToEnd().ToLower()).Groups[1].Value;
                }
                catch { }
                finally
                {
                    //Чистим ресурсы
                    url = null; wc.Dispose(); wc = null; str = null; boundary = null;
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

                //Возвращаем результат 
                return result != null ? (result.Trim() == "" ? null : result.Trim()) : null;
            }
            catch
            {
                return (url = null);
            }
        }


        private string HttpGet(string CaptchaId)
        {
            HttpWebRequest Request = null;
            Stream stream = null;
            StreamReader Reader = null;
            WebResponse Response = null;
            string result = null;

            try
            {
                Request = (HttpWebRequest)WebRequest.Create("http://antigate.com/res.php?key=" + conf.KeyAntigate + "&action=get&id=" + CaptchaId);
                Request.ContentType = "application/x-www-form-urlencoded";
                Request.KeepAlive = true;
                Request.MaximumAutomaticRedirections = 3;
                Request.ReadWriteTimeout = 1000 * 30;
                Request.Timeout = 1000 * 30;
                Response = Request.GetResponse();
                stream = Response.GetResponseStream();
                Reader = new StreamReader(stream);
                result = Reader.ReadToEnd();
            }
            catch { }
            finally
            {
                //Чистим ресурсы
                CaptchaId = null;
                if (Reader != null)
                {
                    Reader.Close(); Reader.Dispose(); Reader = null;
                }
                if (Response != null)
                {
                    Response.Close(); Response.Dispose(); Response = null;
                }
                if (stream != null)
                {
                    stream.Close(); stream.Dispose(); stream = null;
                }
                if (Request != null)
                {
                    Request.Abort(); Request = null;
                }
            }

            //Возвращаем результат 
            return result;
        }
    }
}
