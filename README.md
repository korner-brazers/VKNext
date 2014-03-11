VKNext 0.2
======
Программа для граббинга групп Вконтакте, консольный проект для Linux.<br>
Скомпилированный проект находиться в папке: \NewAPI\bin\Debug\ <br>

======
<b>Функционал:</b>
<b>1)</b> Грабинг, аудио, видео, фото, опросы, текст
<b>2)</b> Рандомовские hash теги #
<b>3)</b> Отдельные настройки для каждой группы откуда берутся новости
<b>4)</b> Публикация в несколько групп
<b>5)</b> Использование нескольких аккаунтов
<b>6)</b> Проверка дубликатов видео и фото
<b>7)</b> Проверка на спам и ссылки

======
<b>Установка для windows 7/8:</b> <br>
Установить .NET Framework 4.5: http://www.microsoft.com/ru-ru/download/details.aspx?id=40779<br>
Установить <b>vcredist_x86.exe</b> который находиться в проекте или скачать с сайта "Распространяемые пакеты Visual C++ для Visual Studio 2013 x86 - http://www.microsoft.com/ru-ru/download/details.aspx?id=40784"<br>

======
<b>Установка для Linux:</b><br>
echo "deb http://debian.meebey.net/experimental/mono /" >> /etc/apt/sources.list<br>
apt-get update<br>
apt-get install mono-complete libphash0 libphash0-dev<br>

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

Запуск:<br>
<b>mono NewAPI.exe</b> или <b>mono --runtime=v4.0.30319 NewAPI.exe</b><br>

