Фронт-офис банка

Для разворачивания приложения, необходимо:
  1. Web-сервер с интерпретатором PHP 5.3 и выше;
  2. Microsoft SQL Server;
  3. На клиенте должна быть установлена .NET Runtime 6.0.

Подготовка к запуску:
  1. На сервере БД создать БД и запустить в ней скрипт Init.sql из каталога SQL;
  2. Файлы с каталога PHP разместить на Web-сервере;
  3. В файле config.xml внести данные для подключения к серверу БД;
  4. В клиентской части перед сборкой поменять переменную API_HOST на свою.
