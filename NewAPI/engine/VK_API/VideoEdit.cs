using System;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Linq;


namespace NewAPI.engine.VK_API
{
    class VideoEdit
    {
        public void put(string video_id, InfoClass.account account, string GroupID)
        {
            //Получаем информацию по видео
            MediaDB media = get(video_id, account.UserID, account.token);

            //Работа с текстом
            string title = (media.title == null ? "Not Title" : new TextReplace().get(media.title, account.ReplaceTag));
            string description = media.description == null ? ProcessingText.get(account.GroupKey, account.KeyVideo, account) : (new TextReplace().get(media.description, account.ReplaceTag) + "\n\n" + ProcessingText.get(account.GroupKey, account.KeyText, account));

            //Меняем описание и название видио
            new PostAPI().post("https://api.vk.com/method/video.edit", ("owner_id=" + account.UserID + "&video_id=" + video_id + "&name=" + HttpUtility.UrlEncode(title) + "&desc=" + HttpUtility.UrlEncode(Regex.Replace(description, @"[\n\r]+", "\n")) + "&access_token=" + account.token), null, null, account.UserID, GroupID);

            //Чистим ресурсы
            video_id = null; account = null; GroupID = null; media = null; title = null; description = null;
        }



        private MediaDB get(string video_id, string owner_id, string token)
        {
            MediaDB media = new MediaDB();
            XDocument xml = null;
            try
            {
                string captcha = "";
                Refresh: xml = XDocument.Load("https://api.vk.com/method/video.get.xml?owner_id=" + owner_id + "&videos=" + owner_id + "_" + video_id + "&access_token=" + token);
                foreach (XElement el in xml.Root.Elements())
                {
                    if (el.Name.ToString() == "captcha_img")
                    {
                        Console.WriteLine("Разгадываем капчу в VideoEdit.cs");
                        captcha = new Antigate().get(el.Value);
                        goto Refresh;
                    }
                    else if (el.Name.ToString().ToLower().Trim() == "video")
                    {
                        foreach (XElement el_video in el.Elements())
                        {
                            switch (el_video.Name.ToString().ToLower().Trim())
                            {
                                case "title": media.title = (el_video.Value.Trim() == "" ? null : el_video.Value); break;
                                case "description": media.description = (el_video.Value.Trim() == "" ? null : el_video.Value); break;
                            }
                        }

                        //Проверка на спам
                        if (media.title != null && new CheckSpam().get(media.title, null))
                        {
                            media.title = null;
                        }
                        if (media.description != null && new CheckSpam().get(media.description, null))
                        {
                            media.description = null;
                        }
                    }
                }
                captcha = null;
            }
            catch { }

            //Воврат результата
            owner_id = null; video_id = null; xml = null;
            return media;
        }


        private class MediaDB
        {
            public string title { get; set; }
            public string description { get; set; }
        }
    }
}
