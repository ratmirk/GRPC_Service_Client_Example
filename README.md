Weather Simulator

Дано:

Сервис-эмулятор погодных датчиков. Сервис генерирует события от минимум двух погодных датчиков уличного и датчика внутри
помещения В событии от датчиков данные по текущей температуре, влажности и содержанию CO2. Сервис реализует потоковом
режиме возвращает события из датчиков.

1. Для данного сервиса требуется:
    1. Реализовывать GRPC-метод, который возвращает текущие параметры конкретного датчика. Например, сейчас нужно прямо
       узнать какие последние данные были на датчике.

Требуется разработать:

2. Сервис-клиент обработки событий от сервиса-эмулятора погодных датчиков.
    1. Сервис должен подписывается на получение данных от конкретного датчика или группы датчиков. Датчики, на которые
       необходимо подписаться должны указываться в конфигурационном файле. Также необходимо чтобы сервис умел
       реагировать если в конфигурационный файл во время работы приложения изменили.
    2. Сервис должен взаимодействовать с сервисом-эмулятором через полнодуплексный grpc stream.
    3. Должен уметь переподнимать поток, если вдруг происходит разрыв связи. Например, если сервис-эмулятор остановлен,
       то необходимо пробовать подключаться с нему, до победного. Плюсом будет использование более сложного алгоритма
       ожидания, чем простой Delay.
    4. Сервис должен сохранять информацию по датчикам в памяти.
    5. Сервис должен иметь HTTP ручку для получения сохранённых данных для конкретного датчика
    6. Сервис должен иметь HTTP ручку, которая вызывает GRPC ручку сервиса эмулятора из пункта 1.1.

Обратить внимание на:

1. Использование стандартных статусов ответов для grpc/http ручек

Дополнительное задание на бриллиант:

1. Интегрировать RateLimiter из задания на неделе 2 в сервис клиент. Ограничить количество запросов в HTTP ручки.
   Ограничения должны работать для конкретного пользователя и метода.
    1. Можно использовать RateLimiter из сторонней библиотеки, но он должен удовлетворять требованиям ДЗ 2.
    2. Готовые решения для http и grpc не принимаются.