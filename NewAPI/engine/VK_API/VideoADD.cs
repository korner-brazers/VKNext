using System;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace NewAPI.engine.VK_API
{
    class VideoADD
    {
        public string get(string video_id, string owner_id, string token, string AccountID, string GroupID)
        {
            string tmp = new PostAPI().post("https://api.vk.com/method/video.add", ("owner_id=" + owner_id + "&video_id=" + video_id + "&access_token=" + token), null, null, AccountID, GroupID);
            string s = new Regex("{\"response\":([0-9]+)}").Match(tmp).Groups[1].Value;

            //Возврат результатов и очистка ресурсов
            AccountID = null; GroupID = null; video_id = null; owner_id = null; token = null; tmp = null;
            return (s != null && s.Trim() != "" ? s : s = null);
        }
    }
}
