﻿
namespace UATaxBot
{
    class Messages
    {
        public static string StartText { get; } = "Привет, здесь можно посчитать платежи для уплаты всех налогов на таможне.\nРасчёты верны только для б/у легковых автомобилей, предназначенных для перевозки пассажиров.\nПриступай или прочитай Информацию.";
        public static string ContactsText { get; } = "Растаможка авто\n\nтел: +380635205050\nTelegram: @quvag\n\n";
        public static string InformationText { get; } = "! Расчёты верны только для б/у легковых автомобилей, предназначенных для перевозки пассажиров;\n\n" +
                        "! Вычисления проводятся на текущую дату, поскольку курс доллара или евро к гривне меняется каждый банковский день. Имей это ввиду, если планируешь заранее переводить средства на расчётный счёт таможни. При условии ослабления гривны, этой суммы может быть недостаточно на день таможенной очистки (растаможки);\n\n" +
                        "- Стоимостью транспортировки авто является сумма всех понесенных тобой затрат на доставку автомобиля до таможенной границы Украины. Ты также можешь добавить сюда комиссию аукциона, и другие расходы;\n\n" +
                        "- Стоимостью автомобиля является цена авто, указанная в счёт-фактуре или договоре купли-продажи;\n\n" +
                        "- Стоимость транспортировки авто до таможенной границы Украины складывается со стоимостью самого автомобиля и является таможенной стоимостью (ТС);\n\n" +
                        "- Налоги рассчитываются исходя из таможенной стоимости автомобиля\n\n" +
                        "- Общая стоимость таможенных платежей рассчитывается по формуле: \nСУММА = АКЦИЗ + ПОШЛИНА + НДС;\n\n" +
                        "- Акцизный сбор (акциз) рассчитывается исходя из нескольких параметров автомобиля: типа и объёма двигателя, года производства авто.\n\n" +
                        "- Пошлина равна 10% от таможенной стоимости;\n\n" +
                        "- НДС (ставка 20%) рассчитывается по формуле: \nНДС = (ТС + АКЦИЗ + ПОШЛИНА) * 0.2;\n\n" +
                        "- Для точности расчёта акциза на бензиновые и дизельные автомобили рекомендую указывать точный объём двигателя в кубических сантиметрах, например 1596. Часто эта информация отображена в свидетельстве о регистрации или техническом паспорте авто.";
        public static string TaxValidationErrorText { get; } = "Введены некорректные данные. Попробуйте еще раз.";


    }
}