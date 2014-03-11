using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Security.Cryptography;

namespace NewAPI.engine.VK_API
{
    class Upload
    {
        public string get(List<InfoClass.Image> ImgUrl, InfoClass.GroupPost GroupPost, InfoClass.GroupList GroupList, InfoClass.account account)
        {
            try
            {
                //Наш результат
                StringBuilder result = new StringBuilder();

                //Если картинок нету
                if (ImgUrl == null)
                {
                    ImgUrl = null; GroupPost = null; account = null; result = null;
                    return null;
                }

                //Загружаем все картинки
                foreach (InfoClass.Image s in ImgUrl)
                {
                    //Загружаем картинку и получаем данные с хешем
                    string photo = HttpUploadPhoto(s.src_big, (GroupList.ModerationNews ? GroupPost.ModerationUploadURL : GroupPost.UploadURL));
                    if (photo == null)
                        continue; //Если была ошибка во время загрузки картинки, то переходим на следующею картинку 

                    //Работа с данными
                    GroupCollection g = new Regex("\"server\":([0-9]+),\"photo\":\"(.*)\",\"hash\":\"([a-zA-Z0-9]+)\"").Match(photo.Replace("\\", "").Replace(" ", "")).Groups;
                    string tmp = new PostAPI().post("https://api.vk.com/method/photos.saveWallPhoto", ("group_id=" + GroupPost.GroupID.Replace("-", "").Trim() + "&server=" + g[1].Value + "&photo=" + HttpUtility.UrlEncode(g[2].Value) + new Regex("(&gid=[0-9]+)&").Match(GroupList.ModerationNews ? GroupPost.ModerationUploadURL : GroupPost.UploadURL).Groups[1].Value + "&hash=" + g[3].Value + "&access_token=" + (GroupList.ModerationNews ? account.ModerationToken : account.token)), null, null, account.UserID, GroupList.GroupID);
                    string res = new Regex("id\":\"([a-zA-Z-_0-9]+)").Match(tmp).Groups[1].Value + ",";

                    //Добовляем результат
                    if (res.Replace(",", "").Replace(" ", "").Trim() != "")
                        result.Append(new Regex("id\":\"([a-zA-Z-_0-9]+)").Match(tmp).Groups[1].Value + ",");

                    //Чистим ресурсы
                    photo = null; g = null; tmp = null; res = null;
                }

                //Возвращаем результат всех фотографий 
                ImgUrl = null; GroupPost = null; GroupList = null; account = null;
                return result.ToString().Replace(",", "").Replace(" ", "").Trim() == "" ? null : result.ToString();
            }
            catch (Exception e) 
            {
                Console.WriteLine("error: Ошибка во время сохранения фотографий на сервере загрузки, UserID: {0}, ID: {1}, группа: {2}", account.UserID, account.id, GroupList.GroupID);
                if (e != null)
                    Console.WriteLine("Код ошибки: {1}{0}{1}{1}", e, Environment.NewLine);

                return null;
            }
        }



        private string HttpUploadPhoto(string img, string UrlUpload)
        {
            try
            {
                //Типа разделитель и данные для работы 
                byte[] boundary = Encoding.UTF8.GetBytes("\n--hfteu74ir5\n");
                HttpWebRequest request = null;
                Stream stream = null, StreamHTML = null;
                StreamReader Reader = null;
                WebClient wc = new WebClient();
                WebResponse Response = null;
                string result = null;

                try
                {
                    //Создаем подключение с URL для загрузки картинок
                    request = (HttpWebRequest)WebRequest.Create(UrlUpload);
                    request.ContentType = "multipart/form-data; boundary=hfteu74ir5";
                    request.Method = "POST";
                    request.KeepAlive = true;
                    request.MaximumAutomaticRedirections = 3;
                    request.ReadWriteTimeout = 1000 * 30;
                    request.Timeout = 1000 * 30;
                    stream = request.GetRequestStream();

                    //Записываем в поток все нужные данные для загрузки картинки в контакт
                    byte[] image = wc.DownloadData(img);
                    if (image != null)
                    {
                        byte[] header = Encoding.UTF8.GetBytes("Content-Disposition: form-data; name=\"photo\"; filename=\"" + img + "\"\nContent-Type: image/jpeg\n\n");
                        stream.Write(boundary, 0, boundary.Length);
                        stream.Write(header, 0, header.Length);
                        stream.Write(image, 0, image.Length);
                        stream.Write(boundary, 0, boundary.Length);

                        //Возвращаем данные куда была загружена картинка
                        Response = request.GetResponse();
                        StreamHTML = Response.GetResponseStream();
                        Reader = new StreamReader(StreamHTML);
                        result = Reader.ReadToEnd();
                        image = null; header = null;
                    }
                }
                catch (Exception e) 
                { 
                    Console.WriteLine("info: Не удалось загрузить картинку: " + img);
                    if (e != null)
                        Console.WriteLine("Код ошибки: {1}{0}{1}{1}", e, Environment.NewLine);
                }


                //Чистим ресурсы
                boundary = null; img = null; UrlUpload = null; wc.Dispose(); wc = null;
                if (Reader != null)
                {
                    Reader.Close(); Reader.Dispose(); Reader = null;
                }
                if (StreamHTML != null)
                {
                    StreamHTML.Close(); StreamHTML.Dispose(); StreamHTML = null;
                }
                if (Response != null)
                {
                    Response.Close(); Response.Dispose(); Response = null;
                }
                if (stream != null)
                {
                    stream.Close(); stream.Dispose(); stream = null;
                }
                if (request != null)
                {
                    request.Abort(); request = null;
                }

                //Возвращаем результат 
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("error: Ошибка при загрузке фотографий на сервер");
                if (e != null)
                    Console.WriteLine("Код ошибки: {1}{0}{1}{1}", e, Environment.NewLine);

                img = null; UrlUpload = null;
                return null;
            }
        }
    }
}
