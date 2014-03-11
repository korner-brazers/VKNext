using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System;

namespace NewAPI.engine
{
    class CheckIMG
    {
        [DllImport(@"pHash.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int ph_dct_imagehash(string file, ref ulong hash);

        [DllImport(@"pHash.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int ph_hamming_distance(ulong hash1, ulong hash2);


        public bool get(InfoClass.account account)
        {
            //Получаем hash текущего изображения
            ulong hash = 0;
            ph_dct_imagehash(conf.DirImgTmp + account.id + ".jpg", ref hash);


            //Ищем дубликаты из сохраненных хешей
            foreach (ulong i in account.HashPhoto)
            {
                //Скажем так, разница фотографий до 6 процентов (хотя тут слегка другой принцип)
                if (ph_hamming_distance(i, hash) < 7)
                {
                    return true;
                }
            }

            //Если дубликатов нету то добавляем текущий hash и записываем в файл
            account.HashPhoto.Add(hash);
            Directory.CreateDirectory(conf.DirHashPhoto);
            File.AppendAllText(conf.DirHashPhoto + account.id, hash.ToString() + Environment.NewLine);

            //Возвращаем результат
            return false;
        }
    }
}
