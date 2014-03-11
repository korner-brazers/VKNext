using System.Collections.Generic;
using System.Net;
using System.IO;
using System;
using System.Threading;

namespace NewAPI.engine
{
    class CreateVideo
    {
        public List<InfoClass.Video> get(List<InfoClass.Video> v, InfoClass.account account, string GroupID)
        {
            List<InfoClass.Video> result = new List<InfoClass.Video> { };
            foreach (InfoClass.Video video in v)
            {
                //Сохраняем картинку в byte[]
                WebClient wc = new WebClient();
                byte[] ImageBig = wc.DownloadData(video.img);
                FileStream ImgStream = null;

                try
                {
                    //Сохраняем картинку на диск
                    ImgStream = new FileStream(conf.DirImgTmp + account.id + ".jpg", FileMode.Create, FileAccess.ReadWrite);
                    ImgStream.Write(ImageBig, 0, ImageBig.Length);
                }
                catch { }
                finally
                {
                    wc.Dispose(); wc = null; ImageBig = null;
                    if (ImgStream != null)
                    {
                        ImgStream.Close(); ImgStream.Dispose(); ImgStream = null;
                    }
                }


                //Если дубликата нету то добавляем картинку в список загружаемых фотографий
                if (!new CheckIMG().get(account) && video.media_id != null && video.owner_id != null)
                {
                    if (account.AddVideo)
                    {
                        string video_id = new engine.VK_API.VideoADD().get(video.media_id, video.owner_id, account.token, account.UserID, GroupID);
                        if (video_id != null && account.UserID != null)
                        {
                            //Редактируем Видео
                            if (account.CreateDescVideo)
                                new VK_API.VideoEdit().put(video_id, account, GroupID);

                            //Добовляем данные в лист
                            result.Add(new InfoClass.Video
                            {
                                img = video.img,
                                media_id = video_id,
                                owner_id = account.UserID
                            });
                        }
                        video_id = null;
                    }
                    else
                    {
                        //Добовляем данные оригинального видео в лист
                        result.Add(new InfoClass.Video
                        {
                            img = video.img,
                            media_id = video.media_id,
                            owner_id = video.owner_id
                        });
                    }
                }
                Thread.Sleep(500);
            }


            //Возвращаем результат
            v = null; account = null; GroupID = null;
            return result.Count == 0 ? (result =null) : result;
        }
    }
}
