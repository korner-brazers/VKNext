using System;
using System.Net;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO;
using System.Threading;

namespace NewAPI.engine.VK_API
{
    class PhotoEdit
    {
        public void put(string GroupID, string MediaID, InfoClass.account account, string post_id, string source_id, bool moderation)
        {
            if (GroupID == null || MediaID == null)
                return;

            //Загружаем данные по новости
            List<MediaDB> media = get(GroupID, MediaID, (moderation ? account.ModerationToken : account.token));
            if (media == null)
                return;

            if (media.Count == 0)
            {
                media = null;
                return;
            }

            //Меняем описание для фотографии 
            foreach (MediaDB m in media)
            {
                string aes = account.KeyAES != null ? "private:" + new engine.AES256(account.KeyAES).Encrypt(string.Format("{0}_{1}", source_id, post_id)) + "\n" : "";
                new PostAPI().post("https://api.vk.com/method/photos.edit", ("owner_id=" + m.owner_id + "&photo_id=" + m.pid + "&caption=" + HttpUtility.UrlEncode(aes + ProcessingText.get(account.GroupKey, account.KeyPhoto, account)) + "&access_token=" + (moderation ? account.ModerationToken : account.token)), null, null, (moderation ? account.ModerationID : account.UserID), GroupID);
                aes = null;
                Thread.Sleep(500);
            }

            //Чистим ресурсы
            media = null; GroupID = null; MediaID = null; account = null; source_id = null; post_id = null;
        }



        private List<MediaDB> get(string GroupID, string MediaID, string token)
        {
            List<MediaDB> media = new List<MediaDB> { };
            XDocument xml = null;
            try
            {
                string captcha = "";
                Refresh: xml = XDocument.Load("https://api.vk.com/method/wall.getById.xml?posts=" + GroupID + "_" + MediaID + captcha + "&access_token=" + token);
                foreach (XElement el in xml.Root.Elements())
                {
                    if (el.Name.ToString() == "captcha_img")
                    {
                        Console.WriteLine("Разгадываем капчу в PhotoEdit.cs");
                        captcha = new Antigate().get(el.Value);
                        goto Refresh;
                    }
                    else if (el.Name.ToString() == "post")
                    {
                        foreach (XElement el_post in el.Elements())
                        {
                            if (el_post.Name == "attachments")
                            {
                                foreach (XElement el_attachments in el_post.Elements())
                                {
                                    foreach (XElement el_attachment in el_attachments.Elements())
                                    {
                                        if (el_attachment.Name == "photo")
                                        {
                                            MediaDB tmp = new MediaDB();
                                            foreach (XElement el_photo in el_attachment.Elements())
                                            {
                                                switch (el_photo.Name.ToString())
                                                {
                                                    case "pid": tmp.pid = el_photo.Value; break;
                                                    case "owner_id": tmp.owner_id = el_photo.Value; break;
                                                }
                                            }
                                            media.Add(tmp); tmp = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                captcha = null;
            }
            catch (Exception e) 
            {
                if (e != null)
                    Console.WriteLine("Ошибка в PhotoEdit.cs, код ошибки: {1}{0}{1}{1}", e, Environment.NewLine);
            }



            //Воврат результата
            GroupID = null; MediaID = null; xml = null;
            return media;
        }


        class MediaDB
        {
            public string pid { get; set; }
            public string owner_id { get; set; }
        }
    }
}
