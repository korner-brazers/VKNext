using System.Collections.Generic;

namespace NewAPI.InfoClass
{
    class account
    {
        public string id { get; set; } //ID для хеша фоток и другого мусора
        public string UserID { get; set; } //ID юсера
        public string ModerationID { get; set; }
        public string token { get; set; }
        public string ModerationToken { get; set; }
        public bool activ { get; set; }
        public List<GroupPost> GroupPost { get; set; }
        public List<string> GroupKey { get; set; }
        public List<GroupList> GroupList { get; set; }
        public List<ulong> HashPhoto { get; set; }
        public int KeyText { get; set; }
        public int KeyWall { get; set; }
        public int KeyPhoto { get; set; }
        public int KeyVideo { get; set; }
        public List<string> StopWords { get; set; }
        public List<string> Tag { get; set; }
        public int RandomStart { get; set; }
        public int RandomNext { get; set; }
        public bool ReplaceTag { get; set; }
        public bool UploadPhoto { get; set; }
        public bool AddVideo { get; set; }
        public bool CreateDescVideo { get; set; }
        public string KeyAES { get; set; }
        public string URL_Text { get; set; }
    }
}
