using System;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Globalization;
using UATaxBot.Enums;
using UATaxBot.Entities;

namespace UATaxBot
{
    class TaxAccurateCalculation
    {
        public static Dictionary<string, Customer> ActiveCustomersCollection => Program.ActiveCustomersCollection;
        public static async void TaxCalculationProcess(TelegramBotClient Bot, Customer customer)
        {
            bool validation = customer.TaxForm.SetCalcTaxParam(customer.MessageText);
            if (!validation)
            {
                await Bot.SendTextMessageAsync(customer.ChatId, TextManager.TaxValidationErrorText);
            }
            (string, int) stageText = customer.TaxForm.GetCalcTaxStageText();
            switch (stageText.Item2)
            {
                case 1:
                    var inlineKeyboard1 = new InlineKeyboardMarkup(new[] {
                            new[]{ InlineKeyboardButton.WithCallbackData("USD"),
                                   InlineKeyboardButton.WithCallbackData("EUR")}
                            });
                    await Bot.SendTextMessageAsync(customer.ChatId, stageText.Item1, replyMarkup: inlineKeyboard1);
                    return;
                case 3:
                    var inlineKeyboard3 = new InlineKeyboardMarkup(new[]
                    {
                        new[] { InlineKeyboardButton.WithCallbackData(TextManager.AuctionComission) },
                        new[] { InlineKeyboardButton.WithCallbackData(TextManager.InsuranceCost) },
                        new[] { InlineKeyboardButton.WithCallbackData(TextManager.TransportationByLand) },
                        new[] { InlineKeyboardButton.WithCallbackData(TextManager.TransportationByWater) },
                        new[] { InlineKeyboardButton.WithCallbackData(TextManager.OtherExpenses) },
                        new[] { InlineKeyboardButton.WithCallbackData(TextManager.NotAdd) }
                    });
                    await Bot.SendTextMessageAsync(customer.ChatId, stageText.Item1, replyMarkup: inlineKeyboard3);
                    return;


                case 5:
                    var inlineKeyboard5 = new InlineKeyboardMarkup(new[] {
                            new[]{ InlineKeyboardButton.WithCallbackData("Бензин и/или газ", "petrol"),
                                   InlineKeyboardButton.WithCallbackData("Дизель", "diesel")},
                            new[]{ InlineKeyboardButton.WithCallbackData("Гибрид", "hybrid"),
                                   InlineKeyboardButton.WithCallbackData("Электро", "electro")}
                            });
                    await Bot.SendTextMessageAsync(customer.ChatId, stageText.Item1, replyMarkup: inlineKeyboard5);
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

        private static decimal GetCP(TaxAccurateCalculationForm form, decimal rateUSD, decimal rateEUR)
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
            decimal ky = DateTime.Now.Year - yearOfManufacture - 1;
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
        private static decimal GetKE(TaxAccurateCalculationForm form, decimal rateEUR)
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
        private static decimal GetTF(TaxAccurateCalculationForm form, decimal CP)
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
        private static decimal GetEXC(TaxAccurateCalculationForm form, decimal KY, decimal KE)
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
        private static decimal GetVAT(TaxAccurateCalculationForm form, decimal CP, decimal TF, decimal EXC)
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
        private static decimal GetPF(decimal CP)
        {
            if (CP >= 0 && CP <= 374550)
            {
                return CP * 0.03m;
            }
            if (CP > 374550 && CP <= 658300)
            {
                return CP * 0.04m;
            }
            else
            {
                return CP * 0.05m;
            }
        }

        public static string CalculateTax(TaxAccurateCalculationForm form)
        {
            decimal rateUSD = 0, rateEUR = 0;
            decimal TAX, VAT, TF, EXC, CP, KY, KE, PF;
            List<Currency> currencies = CurrencyRates.GetExchangeRate();
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
            PF = GetPF(CP);

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
                $"\U000027A1 ИТОГО: {GetFormattedPrice(TAX)} грн.\n\n" +
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
                $"Платёж в пенсионный фонд:\n{GetFormattedPrice(PF)}грн. ({GetFormattedPrice(PF / rateUSD)} USD)\n\n" +
                "-------------\n\n" +
                $"Рассчитано на основании введенных данных:\n" +
                $"Цена автомобиля: {form.CarPrice} {form.CarPriceCurrency}\n" +
                $"{fuelToOutput} {engVolToOutput}\n" +
                yearToOutput +
                $"Стоимость транспортировки: {form.TransportToUABorderCost} {form.TransportToUABorderCurrency}";
            return result;
        }

        private static string GetFormattedPrice(decimal price)
        {
            price = Math.Round(price, 2);
            string formattedPrice = price.ToString("N2", CultureInfo.GetCultureInfo("ru-RU"));
            return formattedPrice;
        }
    }
}
