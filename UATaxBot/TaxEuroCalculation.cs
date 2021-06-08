using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.IO;
using System.Text.Json;
using UATaxBot.Enums;
using UATaxBot.Entities;

namespace UATaxBot
{
    class TaxEuroCalculation
    {
        public static Dictionary<string, Customer> ActiveCustomersCollection => Program.ActiveCustomersCollection;
        public static async void TaxCalculationProcess(TelegramBotClient Bot, Customer customer)
        {
            bool validation = customer.TaxEuroForm.SetCalcTaxParam(customer.MessageText);
            if (!validation)
            {
                await Bot.SendTextMessageAsync(customer.ChatId, Messages.TaxValidationErrorText);
            }
            (string, int) stageText = customer.TaxEuroForm.GetCalcTaxStageText();
            switch (stageText.Item2)
            {
                case 1:
                    await Bot.SendTextMessageAsync(customer.ChatId, stageText.Item1);
                    return;

                case 2:
                    var inlineKeyboard4 = new InlineKeyboardMarkup(new[] {
                            new[]{ InlineKeyboardButton.WithCallbackData("Бензин/Газ", "petrol"),
                                   InlineKeyboardButton.WithCallbackData("Дизель", "diesel"),
                                   InlineKeyboardButton.WithCallbackData("Гибрид", "hybrid") }});
                    await Bot.SendTextMessageAsync(customer.ChatId, stageText.Item1, replyMarkup: inlineKeyboard4);
                    return;

                case 3:
                    await Bot.SendTextMessageAsync(customer.ChatId, stageText.Item1);
                    return;

                case -1:
                    await Bot.SendTextMessageAsync(customer.ChatId, stageText.Item1);
                    ActiveCustomersCollection.Remove(customer.ChatId);
                    return;

                default:
                    await Bot.SendTextMessageAsync(customer.ChatId, stageText.Item1);
                    return;
            }
        }

        public static string CalculateTax(TaxEuroForm form)
        {
            if (!form.isValidYear)
            {
                return "!!!Поплава!!!";
            }

            decimal rateEUR = 0;
            List<Currency> currencies = CurrencyRates.GetExchangeRate();
            foreach (Currency currency in currencies)
            {
                if (currency.cc == "EUR")
                {
                    rateEUR = currency.rate;
                }
            }

            decimal excise, vat, tax, PF;
            decimal fineUAH = 8500;
            decimal fineEUR = fineUAH / rateEUR;
            decimal SB = GetSB(form.YearOfManufacture);
            decimal VE = GetVE(form.EngineVolume);
            decimal FE = GetFE(form.CarEngineType);

            excise = SB + VE + FE;
            PF = GetPF(excise);
            vat = excise * 0.2m;
            tax = excise + vat;

            string fuelToOutput = null;
            switch (form.CarEngineType)
            {
                case EngineType.Petrol:
                    fuelToOutput = "Бензин";
                    break;
                case EngineType.Diesel:
                    fuelToOutput = "Дизель";
                    break;
                case EngineType.Hybrid:
                    fuelToOutput = "Гибрид";
                    break;
            }

            string result = "\U00002757 Евробляха\n\n" +
                $"Расчёт на {DateTime.Now.Day:d2}/{DateTime.Now.Month:d2}/{DateTime.Now.Year}г.\n\n" +
                $"\U000027A1 ИТОГО: {GetFormattedPrice((tax * rateEUR) + fineUAH)} грн.\n\n" +
                $"В том числе\n" +
                $"Акцизный сбор: {GetFormattedPrice(excise * rateEUR)} грн.\n" +
                $"НДС: {GetFormattedPrice(vat * rateEUR)} грн.\n\n" +
                $"Штраф: {GetFormattedPrice(fineUAH)} грн.\n\n" +
                "-------------\n\n" +
                $"ИТОГО: {GetFormattedPrice(tax + fineEUR)} EUR.\n\n" +
                $"В том числе\n" +
                $"Акцизный сбор: {GetFormattedPrice(excise)} EUR.\n" +
                $"НДС: {GetFormattedPrice(vat)} EUR.\n\n" +
                $"Штраф: {GetFormattedPrice(fineEUR)} EUR.\n\n" +
                "-------------\n\n" +
                $"Платёж в пенсионный фонд:\n{GetFormattedPrice(PF * rateEUR)}грн. ({GetFormattedPrice(PF)} EUR)\n\n" +
                "-------------\n\n" +
                $"Рассчитано на основании введенных данных:\n" +
                $"Год выпуска: {form.YearOfManufacture}\n" +
                $"{fuelToOutput} {form.EngineVolume} куб.см\n";
            return result;
        }

        private static decimal GetSB(int yearOfManufacture)
        {
            decimal SB = -1;
            int fullYears = DateTime.Now.Year - yearOfManufacture - 1;
            if (fullYears >= 5 && fullYears <= 9)
            {
                SB = 0;
            }
            else if (fullYears >= 15)
            {
                SB = 150;
            }
            else
            {
                switch (fullYears)
                {
                    case 10:
                        SB = 25;
                        break;
                    case 11:
                        SB = 50;
                        break;
                    case 12:
                        SB = 75;
                        break;
                    case 13:
                        SB = 100;
                        break;
                    case 14:
                        SB = 125;
                        break;
                }
            }
            return SB;
        }

        private static decimal GetVE(int engineVolume)
        {
            decimal VE = -1;
            if (engineVolume > 0 && engineVolume <= 2000)
            {
                VE = engineVolume * 0.25m;
            }
            if (engineVolume > 2000 && engineVolume <= 3000)
            {
                VE = engineVolume * 0.2m;
            }
            if (engineVolume > 3000 && engineVolume <= 4000)
            {
                VE = engineVolume * 0.25m;
            }
            if (engineVolume > 4000 && engineVolume <= 5000)
            {
                VE = engineVolume * 0.35m;
            }
            if(engineVolume > 5000)
            {
                VE = engineVolume * 0.5m;
            }
            return VE;
        }

        private static decimal GetFE(EngineType engineType)
        {
            decimal FE;
            switch (engineType)
            {
                case EngineType.Diesel:
                    FE = 100;
                    break;
                default:
                    FE = 0;
                    break;
            }
            return FE;
        }

        private static decimal GetPF(decimal excise)
        {
            if (excise >= 0 && excise <= 374550)
            {
                return excise * 0.03m;
            }
            if (excise > 374550 && excise <= 658300)
            {
                return excise * 0.04m;
            }
            else
            {
                return excise * 0.05m;
            }
        }


        private static string GetFormattedPrice(decimal price)
        {
            price = Math.Round(price, 2);
            string formattedPrice = price.ToString("N2", CultureInfo.GetCultureInfo("ru-RU"));
            return formattedPrice;
        }
    }
}
