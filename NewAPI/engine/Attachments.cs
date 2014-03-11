using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NewAPI.engine
{
    class Attachments
    {
        public string get(List<InfoClass.Image> ImgUrl, InfoClass.news news, List<InfoClass.Video> Video, InfoClass.GroupList GroupList, InfoClass.GroupPost GroupPost, InfoClass.account account)
        {
            StringBuilder result = new StringBuilder();

            //Браковщик
            if (GroupList.BreakMedia)
            {
                if (ImgUrl != null && !GroupList.photo || news.Audio != null && !GroupList.audio || news.Poll != null && !GroupList.poll || Video != null && !GroupList.video)
                {
                    ImgUrl = null; news = null; GroupList = null; GroupPost = null; account = null; result = null; Video = null;
                    return null;
                }
            }

            //Загружаем фотографию
            if (ImgUrl != null && GroupList.photo)
            {
                if (account.UploadPhoto)
                {
                    string ResultImg = new engine.VK_API.Upload().get(ImgUrl, GroupPost, GroupList, account);
                    if (ResultImg != null)
                    {
                        result.Append(ResultImg);
                        ResultImg = null;
                    }
                }
                else
                {
                    foreach (InfoClass.Image img in ImgUrl)
                    {
                        if (img.owner_id != null && img.pid != null)
                        {
                            result.Append("photo" + img.owner_id + "_" + img.pid + ",");
                        }
                    }
                }
            }

            //Обработка аудио
            if (news.Audio != null && GroupList.audio)
                result.Append(news.Audio);

            //Обработка голосования
            if (news.Poll != null && GroupList.poll)
                result.Append(news.Poll);

            //Обработка видео
            if (Video != null && GroupList.video)
            {
                foreach (InfoClass.Video video in Video)
                {
                    if (video.media_id != null && video.owner_id != null)
                        result.Append("video" + video.owner_id + "_" + video.media_id + ",");
                }
            }

            //Очистка ресурсов и возврат данных
            ImgUrl = null; news = null; GroupList = null; GroupPost = null; account = null; Video = null;
            return result.ToString().Replace(",", "").Replace(" ", "").Trim() == "" ? null : Regex.Replace(result.ToString(), ",$", "");
        }
    }
}
