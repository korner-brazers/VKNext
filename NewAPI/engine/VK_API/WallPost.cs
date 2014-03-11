using System;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace NewAPI.engine.VK_API
{
    class WallPost
    {
        public void put(InfoClass.GroupList GroupList, InfoClass.GroupPost GroupPost, InfoClass.account account, string message, string attachments, string post_id, string source_id)
        {
            if (message == null && attachments == null || GroupList.AttachmentsNullBlock && attachments == null || GroupList.AttachmentsNullBlock && (!attachments.Contains("photo") && !attachments.Contains("video")))
            {
                Console.WriteLine("info: Новость https://vk.com/wall{0}_{1} забракована в WallPost.cs, UserID: {2}, ID: {3}, группа: https://vk.com/public{4}", source_id, post_id, account.UserID, account.id, GroupPost.GroupID.Replace("-", ""));
                GroupList = null; message = null; attachments = null; GroupPost = null; account = null; post_id = null; source_id = null;
            }
            else
            {
                Console.WriteLine("info: {5} https://vk.com/wall{0}_{1}, UserID: {3}, ID: {4}, группа https://vk.com/public{2}", source_id, post_id, GroupPost.GroupID.Replace("-", ""), account.UserID, account.id, (GroupList.ModerationNews ? "Отправляем в модерацию новость" : "Публикуем новость"));
                string tmp = new PostAPI().post("https://api.vk.com/method/wall.post", ("owner_id=" + GroupPost.GroupID + (message != null ? "&message=" + HttpUtility.UrlEncode(message) : "") + (attachments == null ? "" : "&attachments=" + attachments) + "&from_group=1&access_token=" + (GroupList.ModerationNews ? account.ModerationToken : account.token)), source_id, post_id, account.UserID, GroupList.GroupID);
                string s = new Regex("post_id\":([0-9]+)").Match(tmp).Groups[1].Value.Trim();

                //Ставим ключевые слова для фотографии
                if (account.KeyPhoto != 0 && s != "" && account.UploadPhoto)
                    new PhotoEdit().put(GroupPost.GroupID, s, account, post_id, source_id, GroupList.ModerationNews);
                s = null; tmp = null;

                //Чистим ресурсы
                GroupList = null; message = null; attachments = null; GroupPost = null; post_id = null; source_id = null;

                //Ждем между новостями
                int sleep = new Random().Next(account.RandomStart, account.RandomNext);
                System.Threading.Thread.Sleep(1000 * sleep);
                account = null;
            }
        }
    }
}
