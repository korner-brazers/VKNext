VKNext 0.2
======
Программа для граббинга групп Вконтакте, консольный проект для Linux.<br>
Скомпилированный проект находиться в папке: \NewAPI\bin\Debug\ <br>

======
Установка для windows 7/8: <br>
Установить .NET Framework 4.5: http://www.microsoft.com/ru-ru/download/details.aspx?id=40779<br>
Установить <b>vcredist_x86.exe</b> который находиться в проекте или скачать с сайта "Распространяемые пакеты Visual C++ для Visual Studio 2013 x86 - http://www.microsoft.com/ru-ru/download/details.aspx?id=40784"<br>

======
Установка для Linux:<br>
echo "deb http://debian.meebey.net/experimental/mono /" >> /etc/apt/sources.list<br>
apt-get update<br>
apt-get install mono-complete libphash0 libphash0-dev<br><br>

Сертификаты:<br>
mozroots --import --machine --sync<br>
mozroots --import --sync<br>
certmgr -ssl https://go.microsoft.com<br>
certmgr -ssl https://nugetgallery.blob.core.windows.net<br>
certmgr -ssl https://nuget.org<br>
certmgr -ssl https://vk.com<br>
certmgr -ssl https://google.com<br>
certmgr -ssl https://google.com.ua<br>
certmgr -ssl https://yandex.com<br>

Запуск<br>
mono NewAPI.exe или mono --runtime=v4.0.30319 NewAPI.exe<br>

