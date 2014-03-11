using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace NewAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Энкодим консоль в UTF8 если это linux
            if (Environment.OSVersion.Platform.ToString().ToLower().Trim() == "unix")
                Console.OutputEncoding = Encoding.UTF8;


            //Проверка файлов и загрузка данных
            if (!Directory.Exists(conf.DirAccount))
            {
                Console.WriteLine("Нету папки '{0}'", conf.DirAccount);
                Console.ReadKey();
                return;
            }
            else
            {
                new engine.LoadConf();
            }


            //Проверка списка заданий
            if (conf.account.Count == 0)
            {
                Console.WriteLine("Нету заданий");
                Console.ReadKey();
                return;
            }

            //Запускаем граббер
            new engine.GetPostNews();
        }
    }
}
