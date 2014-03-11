using System.Collections.Generic;
using System.Net;
using System.IO;
using System;


namespace NewAPI.engine
{
    class CreateIMG
    {
        public List<InfoClass.Image> get(List<InfoClass.Image> Image, InfoClass.account account)
        {
            List<InfoClass.Image> tmp = new List<InfoClass.Image> { };
            foreach (InfoClass.Image img in Image)
            {
                if (img.src_big != null)
                {
                    //Сохраняем картинку в byte[]
                    byte[] ImageBig = new WebClient().DownloadData(img.src_big);
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
                        ImageBig = null;
                        if (ImgStream != null)
                        {
                            ImgStream.Close(); ImgStream.Dispose(); ImgStream = null;
                        }
                    }


                    //Если дубликата нету то добавляем картинку в список загружаемых фотографий
                    if (!new CheckIMG().get(account))
                    {
                        //Выбираем самую большую фотографию
                        string BigImg = null;
                        if (img.src_xxbig != null)
                            BigImg = img.src_xxbig;
                        else if (img.src_xbig != null)
                            BigImg = img.src_xbig;
                        else if (img.src_big != null)
                            BigImg = img.src_big;


                        //Добовляем данные в массив
                        tmp.Add(new InfoClass.Image 
                        {
                            owner_id = img.owner_id,
                            pid = img.pid,
                            src_big = BigImg,
                            text = img.text
                        });

                        //Чистим ресурсы
                        BigImg = null;
                    }
                }
            }

            //Возвращаем результат
            Image = null; account = null;
            return tmp.Count == 0 ? (tmp = null) : tmp;
        }
    }
}
