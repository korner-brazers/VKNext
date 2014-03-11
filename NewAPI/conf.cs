using System;
using System.Collections.Generic;

namespace NewAPI
{
    class conf
    {
        //Директории
        public static string s = Environment.OSVersion.Platform.ToString().ToLower().Trim() == "unix" ? "/" : @"\";
        public static string dir = Environment.CurrentDirectory + s + "Engine" + s;
        public static string DirHashPhoto = dir + "cache" + s + "HashPhoto" + s;
        public static string DirLastTime = dir + "cache" + s + "LastTime" + s;
        public static string DirAccount = dir + "Account" + s;
        public static string DirKey = dir + "key" + s;
        public static string DirConf = dir + "conf";
        public static string DirStopWords = dir + "StopWords";
        public static string DirImgTmp = dir + "cache" + s + "img" + s;
        public static string DirGroupList = dir + "GroupList" + s;
        public static string DirTag = dir + "cache" + s + "HashTag" + s;


        //Аккаунты
        public static List<InfoClass.account> account = new List<InfoClass.account> { };
        public static string KeyAntigate = "not";
    }
}
