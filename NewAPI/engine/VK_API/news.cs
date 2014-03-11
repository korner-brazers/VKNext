using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace NewAPI.engine.VK_API
{
    class news
    {
        public List<InfoClass.news> get(InfoClass.account account, InfoClass.GroupList GroupList)
        {
            List<string> TempTag = new List<string> { };
            List<InfoClass.news> News = new List<InfoClass.news> { };
            XDocument xml = null;

            try
            {
                string captcha = "";
                Refresh: xml = XDocument.Load("https://api.vk.com/method/wall.get.xml?owner_id=" + GroupList.GroupID + captcha + "&count=100&filter=owner&access_token=" + account.token);
                foreach (XElement el in xml.Root.Elements())
                {
                    if (el.Name.ToString().ToLower() == "error_msg" && el.Value.ToLower().Replace(" ", "") == "userauthorizationfailed:invalidaccess_token.")
                    {
                        Console.WriteLine("error: XML news.cs вернул ошибку: {0}, UserID: {1}, ID: {2}, группа: {3}", el.Value, account.UserID, account.id, GroupList.GroupID);
                        break;
                    }
                    else if (el.Name.ToString().ToLower() == "captcha_img")
                    {
                        Console.WriteLine("info: Разгадываем капчу в news.cs, ID аккаунта: {0}, группа: {1}", account.UserID, GroupList.GroupID);
                        captcha = new Antigate().get(el.Value);
                        goto Refresh;
                    }
                    else if (el.Name.ToString().ToLower() == "post")
                    {
                        InfoClass.news tmp = new InfoClass.news();
                        foreach (XElement el_post in el.Elements())
                        {
                            if (el_post.Name.ToString().ToLower() == "from_id" || el_post.Name.ToString().ToLower() == "to_id")
                            {
                                if (int.Parse(el_post.Value) >= 0)
                                {
                                    tmp = null;
                                    break; //Переходим в следующий item
                                }
                                else
                                    tmp.source_id = el_post.Value;
                            }
                            if (el_post.Name.ToString().ToLower() == "post_id" || el_post.Name.ToString().ToLower() == "id")
                            {
                                tmp.post_id = el_post.Value;
                            }
                            else if (el_post.Name.ToString().ToLower() == "date")
                            {
                                tmp.Date = int.Parse(el_post.Value);
                                if (GroupList.LastTime > tmp.Date)
                                {
                                    tmp = null;
                                    break;  //Переходим в следующий item
                                }
                            }
                            else if (el_post.Name.ToString().ToLower() == "post_type" && el_post.Value.ToString().ToLower() != "post")
                            {
                                tmp = null;
                                break; //Переходим в следующий item
                            }
                            else if (el_post.Name.ToString().ToLower() == "message" || el_post.Name.ToString().ToLower() == "text")
                            {
                                //Проверяем текст
                                if (el_post.Value.Trim() != "")
                                    if (!new CheckSpam().get(el_post.Value, account.StopWords))
                                    {
                                        TempTag.Add(tmp.Text = el_post.Value.Replace("<br>", "\n").Replace("\r", "").Replace("&amp;", ""));
                                    }
                                    else
                                    {
                                        tmp = null;
                                        break; //Переходим в следующий item
                                    }
                            }
                            else if (el_post.Name.ToString().ToLower() == "attachments")
                            {
                                foreach (XElement el_attachments in el_post.Elements())
                                {
                                    InfoClass.Image img = new InfoClass.Image();
                                    foreach (XElement el_attachment in el_attachments.Elements())
                                    {
                                        if (el_attachment.Name.ToString().ToLower() == "photo")
                                        {
                                            foreach (XElement el_photo in el_attachment.Elements())
                                            {
                                                switch (el_photo.Name.ToString().ToLower())
                                                {
                                                    case "pid": img.pid = el_photo.Value; break;
                                                    case "owner_id": img.owner_id = el_photo.Value; break;
                                                    case "src": img.src = el_photo.Value; break;
                                                    case "src_big": img.src_big = el_photo.Value; break;
                                                    case "src_small": img.src_small = el_photo.Value; break;
                                                    case "src_xbig": img.src_xbig = el_photo.Value; break;
                                                    case "src_xxbig": img.src_xxbig = el_photo.Value; break;
                                                    case "text": TempTag.Add(img.text = el_photo.Value); break;
                                                }
                                            }
                                        }
                                        else if (el_attachment.Name.ToString().ToLower() == "poll")
                                        {
                                            foreach (XElement el_poll in el_attachment.Elements())
                                            {
                                                if (el_poll.Name.ToString().ToLower() == "poll_id" && tmp.source_id != null)
                                                {
                                                    tmp.Poll = "poll" + tmp.source_id + "_" + el_poll.Value + ",";
                                                }
                                            }
                                        }
                                        else if (el_attachment.Name.ToString().ToLower() == "video")
                                        {
                                            InfoClass.Video video = new InfoClass.Video();
                                            foreach (XElement el_video in el_attachment.Elements())
                                                switch (el_video.Name.ToString().ToLower())
                                                {
                                                    case "vid": video.media_id = el_video.Value; break;
                                                    case "owner_id": video.owner_id = el_video.Value; break;
                                                    case "image": video.img = el_video.Value; break;
                                                    case "description": TempTag.Add(el_video.Value); break;
                                                }

                                            //Добовляем видео
                                            if (video.img != null && video.media_id != null && video.owner_id != null)
                                            {
                                                tmp.Video.Add(new InfoClass.Video
                                                {
                                                    img = video.img,
                                                    media_id = video.media_id,
                                                    owner_id = video.owner_id
                                                });
                                            }
                                            video = null;
                                        }
                                        else if (el_attachment.Name.ToString().ToLower() == "audio")
                                        {
                                            string owner_id = null, media_id = null;
                                            foreach (XElement el_audio in el_attachment.Elements())
                                                switch (el_audio.Name.ToString().ToLower())
                                                {
                                                    case "aid": media_id = el_audio.Value; break;
                                                    case "owner_id": owner_id = el_audio.Value; break;
                                                }

                                            if (owner_id != null && media_id != null)
                                                tmp.Audio = (tmp.Audio == null ? "" : tmp.Audio) + "audio" + owner_id + "_" + media_id + ",";
                                            owner_id = null; media_id = null;
                                        }
                                        else if (el_attachment.Name.ToString().ToLower() != "type")
                                        {
                                            img = null; goto Break; //Бракуем новость если в ней есть неизвестная хрень
                                        }
                                    }
                                    tmp.img.Add(img);
                                    img = null;
                                }
                            }
                        }
                        if (tmp != null)
                            News.Add(tmp);
                    Break: tmp = null;
                    }
                }
                new ParserTAG().get(TempTag, account);
                TempTag = null; captcha = null;
            }
            catch (Exception e) 
            { 
                Console.WriteLine("error: Ошибка получения новостей, UsreID: {0}, ID: {1}, группа: {2}", account.UserID, account.id, GroupList.GroupID); 
                if (e != null)
                    Console.WriteLine("Код ошибки: {1}{0}{1}{1}", e, Environment.NewLine);
            }


            //Записываем LastTime
            string dir = conf.DirLastTime + account.id;
            if (News.Count != 0 && File.Exists(dir))
            {
                string s = File.ReadAllText(dir).Replace(" ", "");
                GroupList.LastTime = (News[0] as InfoClass.news).Date + 1;
                if (new Regex(GroupList.GroupID + ":[0-9]+").Match(s).Success)
                    File.WriteAllText(dir, Regex.Replace(s, (GroupList.GroupID + ":[0-9]+"), string.Format("{0}:{1}", GroupList.GroupID, GroupList.LastTime)));
                else
                    File.AppendAllText(dir, string.Format("{0}:{1}{2}", GroupList.GroupID, GroupList.LastTime, Environment.NewLine)); //Дописываем новые данные
                s = null;
            }
            else if (News.Count != 0 && !File.Exists(dir))
            {
                //Создаем новый файл и пишем в него данные
                Directory.CreateDirectory(conf.DirLastTime);
                GroupList.LastTime = (News[0] as InfoClass.news).Date + 1;
                File.WriteAllText(dir, string.Format("{0}:{1}", GroupList.GroupID, GroupList.LastTime) + Environment.NewLine);
            }

            //Чистим ресурсы 
            account = null; GroupList = null; xml = null; dir = null;
            return News;
        }
    }
}
