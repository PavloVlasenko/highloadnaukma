В якості бд MSSQL, в якості черги Azure Service Bus. Для кешу використовується просто IMemoryCache.
Є дві черги, одна хендлить і додає запис у бд, інша потрібна для синхронізації  кешу. Черги не налаштовував, але при кількох інстансах повідомлення з бд-черги має хендлити один конкретний інстансб а повідомлення на кеш-чергу кожен інстанс у якого є кеш.
CI/CD налаштований трошки криво - CI базується на докері, CD на чомусь хмарному на вінді для .net. Ну і це окремі пайплайни, кожеш з яких білдить проект з сирців окремо (але послідовно, CD запуститься лише якщо CI успіщно пройдк), не дуже ефективно.
Найбільше довбався з налаштуванням secrets, щоб конекшн стрінги не були у репозиторії і були зашифровані.

Мені було лінь створювати окремі бд для тестів, тому у деплоя і тестів одна бд, яка очищується перед прогонкою тестів. Це легко виправити, просто створивши нову бд і замінивши конекшн стрінг в налаштуваннях ажуру.
