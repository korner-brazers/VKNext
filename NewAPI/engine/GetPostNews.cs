using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NewAPI.engine
{
    class GetPostNews
    {
        public GetPostNews()
        {
            foreach (InfoClass.account account in conf.account)
            {
                if (!account.activ)
                    continue;

                Console.WriteLine("info: Старт потока, UserID: {0}, ID: {1}", account.UserID, account.id);
                Task.Run(() =>
                {
                    while (true)
                    {
                        //Записываем время старта этого круга
                        DateTime timer = DateTime.Now;

                        //Загружаем список групп
                        for (int i = 0; i < account.GroupList.Count; i++)
                        {
                            InfoClass.GroupList GroupList = (account.GroupList[i] as InfoClass.GroupList);
                            if (GroupList == null)
                            {
                                GroupList = null;
                                continue;
                            }

                            //Получаем и публикуем новости
                            foreach (InfoClass.news news in new engine.VK_API.news().get(account, GroupList))
                            {
                                //Создаем если нужно папку для проверки картинок на дубликаты
                                if (!Directory.Exists(conf.DirImgTmp))
                                    Directory.CreateDirectory(conf.DirImgTmp);

                                //Проверяем на дубликаты фотографии и видео, после чего возвращаем новые данные
                                List<InfoClass.Image> ImgUrl = new CreateIMG().get(news.img, account);
                                List<InfoClass.Video> video = new CreateVideo().get(news.Video, account, GroupList.GroupID);

                                foreach (InfoClass.GroupPost GroupPost in account.GroupPost)
                                {
                                    //Получаем фотографии, опросы, аудио, видио и обрабатываем текст
                                    string MessageURL = account.URL_Text != null ? account.URL_Text: "";
                                    string KeyText = account.KeyText != 0 ? "\n" + ProcessingText.get(account.GroupKey, account.KeyText, account).Replace("\n\n", "\n") : MessageURL;
                                    string NewsText = GroupList.text ? news.Text : null;
                                    string message = account.KeyWall != 0 ? (NewsText == null ? ProcessingText.get(account.GroupKey, account.KeyWall, account).Replace("\n\n", "\n") : (new TextReplace().get(NewsText, account.ReplaceTag) + KeyText)) : (NewsText == null ? KeyText : (new TextReplace().get(NewsText, account.ReplaceTag) + KeyText));
                                    string attachments = new Attachments().get(ImgUrl, news, video, GroupList, GroupPost, account);

                                    //Публикуем новость и чистим ресурсы
                                    new engine.VK_API.WallPost().put(GroupList, GroupPost, account, (message == null || message.Replace(" ", "").Trim() == "" ? null : Regex.Replace(Regex.Replace(message, @"\n +\n", "\n\n"), "[\n]+#", "\n#")), attachments, news.post_id, news.source_id);
                                    attachments = null; message = null; NewsText = null; MessageURL = null; KeyText = null;
                                }
                                ImgUrl = null;
                            }
                            GroupList = null;
                        }

                        //Ждем если надо перед новым кругом 3 минуты
                        int time = (int)(DateTime.Now - timer).TotalSeconds;
                        if (time < (60 * 3))
                        {
                            Console.WriteLine("Поток UserID: {1}, ID: {2}, ждет {0}сек, перед новым кругом.", (60 * 3) - time, account.UserID, account.id);
                            System.Threading.Thread.Sleep(1000 * ((60 * 3) - time));
                        }
                    }
                });
            }

            //Ждем себе хз чего
            while (true)
                Console.ReadLine();
        }
    }
}
