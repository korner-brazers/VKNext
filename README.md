VKNext 0.2
======
Программа для граббинга групп Вконтакте, консольный проект для Linux.
Скомпилированный проект находиться в папке: NewAPI\bin\Debug

======
Установка для windows 7/8: 
Установить .NET Framework 4.5: http://www.microsoft.com/ru-ru/download/details.aspx?id=40779
Установить vcredist_x86.exe который находиться в проекте или скачать с сайта "Распространяемые пакеты Visual C++ для Visual Studio 2013 x86 - http://www.microsoft.com/ru-ru/download/details.aspx?id=40784"

======

Установка для Linux:
echo "deb http://debian.meebey.net/experimental/mono /" >> /etc/apt/sources.list
apt-get update
apt-get install mono-complete libphash0 libphash0-dev

Сертификаты:
mozroots --import --machine --sync
mozroots --import --sync
certmgr -ssl https://go.microsoft.com
certmgr -ssl https://nugetgallery.blob.core.windows.net
certmgr -ssl https://nuget.org
certmgr -ssl https://vk.com
certmgr -ssl https://google.com
certmgr -ssl https://google.com.ua
certmgr -ssl https://yandex.com

Запуск
mono NewAPI.exe или mono --runtime=v4.0.30319 NewAPI.exe

