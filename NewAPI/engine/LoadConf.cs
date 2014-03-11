using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace NewAPI.engine
{
    class LoadConf
    {
        public LoadConf()
        {
            //Загрузка ключей и другого мусора
            if(File.Exists(conf.DirConf))
            {
                string s = File.ReadAllText(conf.DirConf).Replace(" ", "");
                conf.KeyAntigate = new Regex("KeyAntigate:([^;]+);").Match(s).Groups[1].Value;
                s = null;
            }

            //Загрузка данных по аккаунтам и группам
            foreach (string AccountDir in Directory.GetFiles(conf.DirAccount))
            {
                string account = Regex.Replace(Regex.Replace(File.ReadAllText(AccountDir), @"/\*[^\*]+\*/", ""), "//[^\n\r]+\n?\r?", "").Replace("\r", "").Replace("\n", "").Replace(@"\", "");
                if (account.Replace(" ", "").Trim() == "")
                    continue;

                //Смотрим нужно ли грабить задание или нет
                string activ = new Regex("activ:([a-z]+);").Match(account.Replace(" ", "").ToLower()).Groups[1].Value.Trim();
                if (activ == "" || activ == "false")
                    continue;

                //Токены и ID
                string Token = new Regex("UsrerToken:([a-zA-Z0-9]+);").Match(account.Replace(" ", "")).Groups[1].Value;
                string ModToken = new Regex("ModerationToken:([a-zA-Z0-9]+);").Match(account.Replace(" ", "")).Groups[1].Value;
                string userID = new engine.VK_API.UserGet().get(Token);
                string ModID = new engine.VK_API.UserGet().get(ModToken);
                string ID = new Regex("id:([^;]+);").Match(account.Replace(" ", "")).Groups[1].Value.Trim();


                ////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Список групп куда грабить и список URL для загрузки изображения
                List<InfoClass.GroupPost> Gpost = new List<InfoClass.GroupPost> { };
                foreach (string gPost in new Regex("grouppost:{([-0-9,]+)};").Match(account.Replace(" ", "").ToLower()).Groups[1].Value.Split(','))
                {
                    //Добовляем в List
                    Gpost.Add(new InfoClass.GroupPost
                    {
                        GroupID = gPost.Trim(),
                        UploadURL = new engine.VK_API.UploadURL().get(Token, gPost),
                        ModerationUploadURL = new engine.VK_API.UploadURL().get(ModToken, gPost)
                    });
                }


                ////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Грузим ключи если они доступны
                List<string> key = new List<string> { };
                foreach (string s1 in new Regex("GroupKey:{([A-Za-z-0-9,_-]+)};").Match(account.Replace(" ", "")).Groups[1].Value.Split(','))
                {
                    if (File.Exists(conf.DirKey + s1))
                    {
                        foreach (string s2 in File.ReadLines(conf.DirKey + s1))
                        {
                            key.Add(s2.Trim());
                        }
                    }
                }


                ////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Грузим теги если они доступны
                List<string> tag = new List<string> { };
                if (File.Exists(conf.DirTag + ID))
                {
                    foreach (string s in File.ReadLines(conf.DirTag + ID))
                    {
                        tag.Add(s.ToLower().Trim());
                    }
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Загружаем данные для групп
                List<InfoClass.GroupList> Glist = new List<InfoClass.GroupList> { };
                foreach (string gListID in new Regex("GroupList:{([a-zA-Z0-9-_,]+)};").Match(account.Replace(" ", "")).Groups[1].Value.Split(','))
                {
                    if (File.Exists(conf.DirGroupList + gListID))
                    {
                        foreach (string gList in Regex.Replace(Regex.Replace(File.ReadAllText(conf.DirGroupList + gListID), @"/\*[^\*]+\*/", ""), "//[^\n\r]+\n?\r?", "").ToLower().Replace(" ", "").Replace("\r", "").Split('\n'))
                        {
                            if (gList.Trim() == "")
                                continue;

                            GroupCollection g = new Regex("group:'([-0-9]+)',{photo=([a-z]+),text=([a-z]+),poll=([a-z]+),audio=([a-z]+),video=([a-z]+),break=([a-z]+),attachmentsnullblock=([a-z]+),moderationnews=([a-z]+)};").Match(gList).Groups;
                            int lastTime = 0;

                            //Если нету ID откуда брать то и остальное не нужно
                            if (g[1].Value.Replace(" ", "").Trim() == "")
                                continue;

                            ////////////////////////////////////////////////////////////////////

                            if (File.Exists(conf.DirLastTime + ID))
                            {
                                foreach (string last in File.ReadAllLines(conf.DirLastTime + ID))
                                {
                                    if ((new Regex(g[1].Value + ":[0-9]+").Match(last)).Success)
                                    {
                                        lastTime = int.Parse(new Regex(g[1].Value + ":([0-9]+)").Match(last).Groups[1].Value);
                                        break;
                                    }
                                }
                            }

                            ////////////////////////////////////////////////////////////////////

                            Glist.Add(new InfoClass.GroupList
                            {
                                GroupID = g[1].Value,
                                photo = g[2].Value == "true" ? true : false,
                                text = g[3].Value == "true" ? true : false,
                                poll = g[4].Value == "true" ? true : false,
                                audio = g[5].Value == "true" ? true : false,
                                video = g[6].Value == "true" ? true : false,
                                BreakMedia = g[7].Value == "true" ? true : false,
                                AttachmentsNullBlock = g[8].Value == "true" ? true : false,
                                ModerationNews = g[9].Value == "true" ? true : false,
                                LastTime = lastTime
                            });
                            g = null;
                        }
                    }
                }


                ////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Загружаем Hash фотографий
                List<ulong> Hphoto = new List<ulong> { };
                if (File.Exists(conf.DirHashPhoto + ID))
                    foreach (string hash in File.ReadAllLines(conf.DirHashPhoto + ID))
                    {
                        try
                        {
                            Hphoto.Add(ulong.Parse(hash.Trim()));
                        }
                        catch { }
                    }


                ////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Стоп слова
                List<string> SWords = new List<string> { };
                if (File.Exists(conf.DirStopWords))
                {
                    foreach (string s1 in new Regex("StopWords:{([^)]+)};").Match(account.Replace(" ", "")).Groups[1].Value.Split(','))
                    {
                        foreach (string s2 in File.ReadLines(conf.DirStopWords))
                        {
                            if ((new Regex(@"id:" + s1.ToLower() + "={([^}]+)}").Match(s2.ToLower())).Success)
                                foreach (string s3 in new Regex("id:[a-zA-Z-0-9-_]+={([^}]+)}").Match(s2).Groups[1].Value.Split(','))
                                {
                                    SWords.Add(s3.Trim());
                                }
                        }
                    }
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Данные по ключам
                string Ktext = new Regex("keytext:{true,'count:([0-9]+)'};").Match(account.Replace(" ", "").ToLower()).Groups[1].Value.Trim();
                string Kwall = new Regex("keywall:{true,'count:([0-9]+)'};").Match(account.Replace(" ", "").ToLower()).Groups[1].Value.Trim();
                string Kphoto = new Regex("keyphoto:{true,'count:([0-9]+)'};").Match(account.Replace(" ", "").ToLower()).Groups[1].Value.Trim();
                string Kvideo = new Regex("keyvideo:{true,'count:([0-9]+)'};").Match(account.Replace(" ", "").ToLower()).Groups[1].Value.Trim();
                string Rstart = new Regex("random:([0-9]+),([0-9]+);").Match(account.Replace(" ", "").ToLower()).Groups[1].Value.Trim();
                string Rnext = new Regex("random:([0-9]+),([0-9]+);").Match(account.Replace(" ", "").ToLower()).Groups[2].Value.Trim();
                string Keyaes = new Regex("AES:{true,'([^']+)'};").Match(account.Replace(" ", "")).Groups[1].Value;
                GroupCollection gUtext = new Regex("Url:{[trueTRUE]+,'([^']+)'};").Match(account).Groups;


                ////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Добовляем все данные
                conf.account.Add(new InfoClass.account
                {
                    id = ID,
                    UserID = userID,
                    ModerationID = ModID,
                    token = Token,
                    ModerationToken = ModToken,
                    activ = new Regex("activ:([a-z]+);").Match(account.Replace(" ", "").ToLower()).Groups[1].Value.Trim() == "true" ? true : false,
                    GroupPost = Gpost,
                    GroupKey = key.Count == 0 ? null : key,
                    GroupList = Glist,
                    HashPhoto = Hphoto,
                    KeyText = int.Parse(Ktext == "" ? "0" : Ktext),
                    KeyWall = int.Parse(Kwall == "" ? "0" : Kwall),
                    KeyPhoto = int.Parse(Kphoto == "" ? "0" : Kphoto),
                    KeyVideo = int.Parse(Kvideo == "" ? "0" : Kvideo),
                    StopWords = SWords.Count == 0 ? null : SWords,
                    Tag = tag,
                    RandomStart = int.Parse(Rstart == "" ? "0" : Rstart),
                    RandomNext = int.Parse(Rnext == "" ? "0" : Rnext),
                    ReplaceTag = new Regex("replacetag:([trueTRUE]+);").Match(account.Replace(" ", "").ToLower()).Groups[1].Value.Trim() == "true" ? true : false,
                    UploadPhoto = new Regex("uploadphoto:([trueTRUE]+);").Match(account.Replace(" ", "").ToLower()).Groups[1].Value.Trim() == "true" ? true : false,
                    AddVideo = new Regex("addvideo:([trueTRUE]+);").Match(account.Replace(" ", "").ToLower()).Groups[1].Value.Trim() == "true" ? true : false,
                    CreateDescVideo = new Regex("createdescvideo:([trueTRUE]+);").Match(account.Replace(" ", "").ToLower()).Groups[1].Value.Trim() == "true" ? true : false,
                    KeyAES = Keyaes.Trim() == "" ? null : Keyaes.Trim(),
                    URL_Text = gUtext[1].Value.ToString().Trim() != "" ? gUtext[1].Value.ToString().Replace("<br>", "\n") : null
                });

                ////////////////////////////////////////////////////////////////////////////////////////////////////////

                //Чистим ресурсы 
                Gpost = null; key = null; Glist = null; Hphoto = null; SWords = null; gUtext = null; ModID = null; ID = null; Keyaes = null; userID = null;
                Kwall = null; Kphoto = null; Kvideo = null; Rstart = null; Rnext = null; ModToken = null; Token = null; tag = null; Ktext = null;
            }
        }
    }
}
