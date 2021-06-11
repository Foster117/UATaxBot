﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UATaxBot
{
    public static class TextManager
    {
        // Menu text
        public const string MenuCarFromUsaInaccurate = "~ Авто из США";
        public const string MenuCarFromKoreaInaccurate = "~ Авто из Кореи";
        public const string MenuCarFromEuropeInaccurate = "~ Авто из Европы";
        public const string MenuInformation = "Информация";
        public const string MenuCarFromUsaKoreaEuropeAccurate = "Точный расчёт на сегодня (США / Корея / Европа)";
        public const string MenuCarEurobadgeAccurate = "Евробляха: точный расчет";
        public const string MenuEurobadgeInformation = "Евробляха: информация";
        public const string MenuNBUCurrencyRates = "Курс валют НБУ";
        public const string MenuContacts = "Контакты";

        // Calculation process
        public const string TaxValidationErrorText = "Введены некорректные данные. Попробуйте еще раз.";

        // Console output text
        public const string ConsoleStartedBot = "started the bot";

        // Greeting text
        public const string StartText  = "Приветствую!\n\n" +
           "Я рассчитываю платежи для уплаты на таможню в качестве налогов и сборов, а также платеж " +
            "в пенсионный фонд при первой регистрации автомобиля в Украине\n\n" +
           "Вы можете приблизительно прикинуть таможенные платежи (требуется вводить меньше данных) " +
            "для автомобилей из разных стран, либо воспользоваться точным расчетом на сегодняшнюю дату " +
            "(требуется вводить больше данных).\n\n" +
           "Также могу рассчитать все налоги по Евробляхе на текущую дату\n\n" +
           "\U00002757 Расчёты верны только для б/у легковых автомобилей, предназначенных для перевозки людей.";

        // Contacts text
        public const string ContactsText  = "Растаможка авто\n\n\U0001F4DE +380635205050\n\U0001F4F1 @quvag\n\n";

        // Information text
        public const string InformationText = "\U00002757 Расчёты верны только для б/у легковых автомобилей, предназначенных для перевозки пассажиров;\n\n" +
                "\U00002757 Вычисления проводятся на текущую дату, поскольку курс доллара или евро к гривне меняется каждый банковский день. Имейте это ввиду, если планируете заранее переводить средства на расчётный счёт таможни. При условии ослабления гривны, этой суммы может быть недостаточно на день таможенной очистки (растаможки);\n\n" +
                "-------------\n\n" +
                "\U0001F537 Стоимостью транспортировки авто является сумма всех понесенных затрат на доставку автомобиля до таможенной границы Украины. Дополнительно можно добавить сюда комиссию аукциона и другие расходы;\n\n" +
                "\U0001F537 Стоимостью автомобиля является цена авто, указанная в счёт-фактуре или договоре купли-продажи;\n\n" +
                "\U0001F537 Стоимость транспортировки авто до таможенной границы Украины складывается со стоимостью самого автомобиля и является Таможенной Стоимостью (ТС);\n\n" +
                "\U0001F537 Налоги рассчитываются исходя из таможенной стоимости автомобиля\n\n" +
                "-------------\n\n" +
                "\U0001F4D6 Общая стоимость таможенных платежей рассчитывается по формуле: \nСУММА = АКЦИЗ + ПОШЛИНА + НДС;\n\n" +
                "\U0001F4D6 Акцизный сбор (акциз) рассчитывается исходя из нескольких параметров автомобиля: типа и объёма двигателя, года производства авто.\n\n" +
                "\U0001F4D6 Пошлина равна 10% от таможенной стоимости;\n\n" +
                "\U0001F4D6 НДС (ставка 20%) рассчитывается по формуле: \nНДС = (ТС + АКЦИЗ + ПОШЛИНА) * 0.2;\n\n" +
                "-------------\n\n" +
                "⚠️ Обратите внимание – для бензиновых (и/или газ) двигателей объёмом более 3000 см3, а также дизельных двигателей объёмом более 3500 см3, применяется двойная ставка акциза;\n\n" +
                "⚠️ Для точности расчёта акциза на бензиновые и дизельные автомобили рекомендую указывать точный объём двигателя в кубических сантиметрах, например 1596. Часто эта информация отображена в свидетельстве о регистрации или техническом паспорте авто.\n\n" +
                "⚠️ К сожалению, для целей налогообложения принято считать гибридными двигателями только те, у которых электродвигатель является основным, а ДВС – вспомогательным, работающим в качестве генератора, и не передающим крутящий момент на колёса. " +
                "Таким образом, привычные нам «гибриды», в том числе PLUG IN, рассчитываются исходя из объёма ДВС, и с каждого «кубика» придётся уплатить акциз. Поэтому для верного расчёта платежей на обычные «гибриды» выбирайте тип ДВС: бензин или дизель.";
    }
}