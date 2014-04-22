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
        private static List<ActivTask> activTask = new List<ActivTask> { };

        private class ActivTask
        {
            public Task task { get; set; }
            public int id { get; set; }
            public bool status { get; set; }
        }

        public GetPostNews()
        {
            //Добовляем задания в Task
            for (int x = 0; x < conf.account.Count; x++)
            {
                InfoClass.account account = conf.account[x];
                if (!account.activ)
                    continue;

                //Добовляем данные
                activTask.Add(new ActivTask()
                {
                    task = get(account),
                    id = x
                });
                account = null;
            }


            //Обновляем потоки
            while (true)
            {
                foreach (var work in activTask.ToArray())
                {
                    if (work.task.IsCompleted || work.task.IsCanceled || work.task.IsFaulted)
                    {
                        //Записываем ID
                        int x = work.id;

                        //Удаляем старое задание
                        activTask.Remove(work);

                        //Добовляем новое задание
                        activTask.Add(new ActivTask()
                        {
                            task = get(conf.account[x]),
                            id = x
                        });
                    }
                    else if (!work.status)
                    {
                        work.task.Start();
                        work.status = true;
                    }
                }

                //Ждем 3 минуты
                System.Threading.Thread.Sleep(1000 * 60 * 3);
            }
        }



        private Task get(InfoClass.account account)
        {
            return new Task(() =>
            {
                Console.WriteLine("info: Старт потока, UserID: {0}, ID: {1}", account.UserID, account.id);

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
                            string MessageURL = account.URL_Text != null ? account.URL_Text : "";
                            string KeyText = account.KeyText != 0 ? "\n" + ProcessingText.get(account.GroupKey, account.KeyText, account) : MessageURL;
                            string NewsText = GroupList.text ? news.Text : null;
                            string message = account.KeyWall != 0 ? (NewsText == null ? ProcessingText.get(account.GroupKey, account.KeyWall, account) : (new TextReplace().get(NewsText, account.ReplaceTag) + KeyText)) : (NewsText == null ? KeyText : (new TextReplace().get(NewsText, account.ReplaceTag) + KeyText));
                            string attachments = new Attachments().get(ImgUrl, news, video, GroupList, GroupPost, account);

                            //Публикуем новость и чистим ресурсы
                            new engine.VK_API.WallPost().put(GroupList, GroupPost, account, (message == null || message.Replace(" ", "").Trim() == "" ? null : Regex.Replace(Regex.Replace(message, "[\n]+#", "\n#"), "\n[\n ]+", "\n\n")), attachments, news.post_id, news.source_id);
                            attachments = null; message = null; NewsText = null; MessageURL = null; KeyText = null;
                        }
                        ImgUrl = null;
                    }
                    GroupList = null;
                }

                //Завершаем поток
                Console.WriteLine("info: Поток завершен, UserID: {0}, ID: {1}", account.UserID, account.id);
                account = null;
            });
        }
    }
}
