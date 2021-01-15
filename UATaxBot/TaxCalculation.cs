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

namespace UATaxBot
{
    class TaxCalculation
    {
        public static async void TaxCalculationProcess(TelegramBotClient Bot, Dictionary<string, TaxForm> calcTaxData, TaxForm findedForm, string messageText, Message message)
        {
            bool validation = findedForm.SetCalcTaxParam(messageText);
            if (!validation)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, Messages.TaxValidationErrorText);
            }
            var stageText = findedForm.GetCalcTaxStageText();
            switch (stageText.Item2)
            {
                case 1:
                    var inlineKeyboard1 = new InlineKeyboardMarkup(new[] {
                            new[]{ InlineKeyboardButton.WithCallbackData("USD", "USD"),
                                   InlineKeyboardButton.WithCallbackData("EUR", "EUR")}
                            });
                    await Bot.SendTextMessageAsync(message.Chat.Id, stageText.Item1, replyMarkup: inlineKeyboard1);
                    return;

                case 3:
                    var inlineKeyboard4 = new InlineKeyboardMarkup(new[] {
                            new[]{ InlineKeyboardButton.WithCallbackData("Бензин и/или газ", "petrol"),
                                   InlineKeyboardButton.WithCallbackData("Дизель", "diesel")},
                            new[]{ InlineKeyboardButton.WithCallbackData("Гибрид", "hybrid"),
                                   InlineKeyboardButton.WithCallbackData("Электро", "electro")}
                            });
                    await Bot.SendTextMessageAsync(message.Chat.Id, stageText.Item1, replyMarkup: inlineKeyboard4);
                    return;

                case 6:
                    var inlineKeyboard6 = new InlineKeyboardMarkup(new[] {
                            new[]{ InlineKeyboardButton.WithCallbackData("USD", "USD"),
                                   InlineKeyboardButton.WithCallbackData("EUR", "EUR")}
                            });
                    await Bot.SendTextMessageAsync(message.Chat.Id, stageText.Item1, replyMarkup: inlineKeyboard6);
                    return;

                case -1:
                    await Bot.SendTextMessageAsync(message.Chat.Id, stageText.Item1);
                    calcTaxData.Remove(message.From.Id.ToString());
                    return;

                default:
                    await Bot.SendTextMessageAsync(message.Chat.Id, stageText.Item1);
                    return;
            }
        }

        private static decimal GetCP(TaxForm form, decimal rateUSD, decimal rateEUR)
        {
            decimal carPrice = 0;
            decimal transportationUAH = 0;
            switch (form.CarPriceCurrency)
            {
                case CurrencyType.USD:
                    carPrice = form.CarPrice * rateUSD;
                    break;
                case CurrencyType.EUR:
                    carPrice = form.CarPrice * rateEUR;
                    break;
            }
            switch (form.TransportToUABorderCurrency)
            {
                case CurrencyType.USD:
                    transportationUAH = form.TransportToUABorderCost * rateUSD;
                    break;
                case CurrencyType.EUR:
                    transportationUAH = form.TransportToUABorderCost * rateEUR;
                    break;
            }
            return carPrice + transportationUAH;
        }
        private static decimal GetKY(int yearOfManufacture)
        {
            decimal ky = DateTime.Now.Year - (yearOfManufacture - 1);
            if (ky > 15)
            {
                return 15;
            }
            if (ky < 1)
            {
                return 1;
            }
            return ky;
        }
        private static decimal GetKE(TaxForm form, decimal rateEUR)
        {
            switch (form.CarEngineType)
            {
                case EngineType.Petrol:
                    if (form.EngineVolume <= 3000)
                    {
                        return (50m * form.EngineVolume / 1000m) * rateEUR;
                    }
                    else
                    {
                        return (100m * form.EngineVolume / 1000m) * rateEUR;
                    }
                case EngineType.Diesel:
                    if (form.EngineVolume <= 3500)
                    {
                        return (75m * form.EngineVolume / 1000m) * rateEUR;
                    }
                    else
                    {
                        return (150m * form.EngineVolume / 1000m) * rateEUR;
                    }
                case EngineType.Hybrid:
                    return 100m * rateEUR;
                case EngineType.Electro:
                    return form.EngineVolume * rateEUR;
                default:
                    return 0;
            }
        }
        private static decimal GetTF(TaxForm form, decimal CP)
        {
            if (form.CarEngineType == EngineType.Electro)
            {
                return 0;
            }
            else
            {
                return CP * 0.1m;
            }
        }
        private static decimal GetEXC(TaxForm form, decimal KY, decimal KE)
        {
            if (form.CarEngineType == EngineType.Petrol || form.CarEngineType == EngineType.Diesel)
            {
                return KY * KE;
            }
            else
            {
                return KE;
            }
        }
        private static decimal GetVAT(TaxForm form, decimal CP, decimal TF, decimal EXC)
        {
            if (form.CarEngineType == EngineType.Electro)
            {
                return 0;
            }
            else
            {
               return (CP + TF + EXC) * 0.2m;
            }
        }

        public static string CalculateTax(TaxForm form)
        {
            decimal rateUSD = 0, rateEUR = 0;
            decimal TAX, VAT, TF, EXC, CP, KY, KE;
            List<Currency> currencies = GetExchangeRate();
            foreach (Currency currency in currencies)
            {
                switch (currency.cc)
                {
                    case "USD":
                        rateUSD = currency.rate;
                        break;
                    case "EUR":
                        rateEUR = currency.rate;
                        break;
                }
            }

            CP = GetCP(form, rateUSD, rateEUR);
            KY = GetKY(form.YearOfManufacture);
            KE = GetKE(form, rateEUR);
            TF = GetTF(form, CP);
            EXC = GetEXC(form, KY, KE);
            VAT = GetVAT(form, CP, TF, EXC);

            /////// TAX
            TAX = VAT + TF + EXC;

            VAT = Math.Round(VAT, 2);
            TF = Math.Round(TF, 2);
            EXC = Math.Round(EXC, 2);
            TAX = Math.Round(TAX, 2);

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
                case EngineType.Electro:
                    fuelToOutput = "Электро";
                    break;
            }

            string engVolToOutput = "";
            if (form.CarEngineType == EngineType.Petrol || form.CarEngineType == EngineType.Diesel)
                engVolToOutput = $"{form.EngineVolume} куб.см";
            if (form.CarEngineType == EngineType.Electro)
                engVolToOutput = $"{form.EngineVolume} кВт/ч";

            string yearToOutput = "";
            if (form.CarEngineType == EngineType.Petrol || form.CarEngineType == EngineType.Diesel)
                yearToOutput = $"Год выпуска: {form.YearOfManufacture}\n";

            decimal rateToOutput = 0;
            switch (form.CarPriceCurrency)  
            {
                case CurrencyType.USD:
                    rateToOutput = rateUSD;
                    break;
                case CurrencyType.EUR:
                    rateToOutput = rateEUR;
                    break;
            }

            string result = $"Расчёт на {DateTime.Now.Day:d2}/{DateTime.Now.Month:d2}/{DateTime.Now.Year}г.\n\n" +
                $"ИТОГО: {GetFormattedPrice(TAX)} грн.\n\n" +
                $"В том числе\n" +
                $"Акцизный сбор: {GetFormattedPrice(EXC)} грн.\n" +
                $"Пошлина: {GetFormattedPrice(TF)} грн.\n" +
                $"НДС: {GetFormattedPrice(VAT)} грн.\n\n" +
                "-------------\n\n" +
                $"ИТОГО: {GetFormattedPrice(TAX / rateToOutput)} {form.CarPriceCurrency}\n\n" +
                $"В том числе\n" +
                $"Акцизный сбор: {GetFormattedPrice(EXC / rateToOutput)} {form.CarPriceCurrency}\n" +
                $"Пошлина: {GetFormattedPrice(TF / rateToOutput)} {form.CarPriceCurrency}\n" +
                $"НДС: {GetFormattedPrice(VAT / rateToOutput)} {form.CarPriceCurrency}\n\n" +
                "-------------\n\n" +
                $"Рассчитано на основании введенных данных:\n" +
                $"Цена автомобиля: {form.CarPrice} {form.CarPriceCurrency}\n" +
                $"{fuelToOutput} {engVolToOutput}\n" +
                yearToOutput +
                $"Стоимость транспортировки: {form.TransportToUABorderCost} {form.TransportToUABorderCurrency}";
            return result;
        }

        private static List<Currency> GetExchangeRate()
        {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString("D2");
            string day = DateTime.Now.Day.ToString("D2");
            string responseFromServer;
            string requestString = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date=" + year + month + day + "&json";

            WebRequest request = WebRequest.Create(requestString);
            using (Stream dataStream = request.GetResponse().GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
            }
            List<Currency> allRates = JsonSerializer.Deserialize<List<Currency>>(responseFromServer);
            List<Currency> rates = new List<Currency>();
            foreach (Currency currency in allRates)
            {
                if (currency.cc.ToUpper() == "USD" || currency.cc.ToUpper() == "EUR")
                {
                    currency.cc = currency.cc.ToUpper();
                    rates.Add(currency);
                }
            }
            return rates;
        }

        private static string GetFormattedPrice(decimal price)
        {
            price = Math.Round(price, 2);
            string formattedPrice = price.ToString("N2", CultureInfo.GetCultureInfo("ru-RU"));
            return formattedPrice;
        }
    }
}
