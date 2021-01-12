using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Globalization;

namespace UATaxBot
{
    class Model
    {
        public static string CalculateTax(TaxForm form)
        {
            decimal rateUSD = 0, rateEUR = 0;
            decimal TAX, VAT, TF, EXC, CP, KY, KE;
            List<Currency> currencies = GetExchangeRate();
            foreach (Currency item in currencies)
            {
                if (item.cc.ToUpper() == "USD")
                {
                    rateUSD = item.rate;
                }
                else
                {
                    rateEUR = item.rate;
                }
            }

            /////// CP
            decimal invoiceUAH;
            decimal transportationUAH;
            invoiceUAH = (form.Currency == "USD") ? form.InvoicePrice * rateUSD : form.InvoicePrice * rateEUR;
            transportationUAH = (form.TransportationToUABorderCurrency == "USD") ? form.TransportationToUABorderCost * rateUSD : form.TransportationToUABorderCost * rateEUR;
            CP = invoiceUAH + transportationUAH;
            /////// 

            /////// KY
            KY = DateTime.Now.Year - form.YearOfManufacture - 1;
            if (KY > 15)
            {
                KY = 15;
            }
            if (KY < 1)
            {
                KY = 1;
            }
            ///////

            /////// KE
            switch (form.EngineType)
            {
                case "petrol":
                    if (form.EngineVolume <= 3000)
                    {
                        KE = (50 * form.EngineVolume / 1000) * rateEUR;
                    }
                    else
                    {
                        KE = (100 * form.EngineVolume / 1000) * rateEUR;
                    }
                    break;
                case "diesel":
                    if (form.EngineVolume <= 3500)
                    {
                        KE = (75 * form.EngineVolume / 1000) * rateEUR;
                    }
                    else
                    {
                        KE = (150 * form.EngineVolume / 1000) * rateEUR;
                    }
                    break;
                case "hybrid":
                    KE = 100 * rateEUR;
                    break;
                case "electro":
                    KE = form.EngineVolume * rateEUR;
                    break;
                default:
                    KE = 0;
                    break;
            }
            ///////
            /////// TF
            if (form.EngineType == "electro")
            {
                TF = 0;
            }
            else
            {
                TF = CP * 0.1m;
            }
            ///////
            /////// EXC
            if (form.EngineType == "petrol" || form.EngineType == "diesel")
            {
                EXC = KY * KE;
            }
            else
            {
                EXC = KE;
            }
            ///////
            /////// VAT
            if (form.EngineType == "electro")
            {
                VAT = 0;
            }
            else
            {
                VAT = (CP + TF + EXC) * 0.2m;
            }
            ///////
            /////// TAX
            TAX = VAT + TF + EXC;
            ///////
            VAT = Math.Round(VAT, 2);
            TF = Math.Round(TF, 2);
            EXC = Math.Round(EXC, 2);
            TAX = Math.Round(TAX, 2);

            string fuelToOutput = null;
            if (form.EngineType == "petrol")
                fuelToOutput = "Бензин";
            if (form.EngineType == "diesel")
                fuelToOutput = "Дизель";
            if (form.EngineType == "hybrid")
                fuelToOutput = "Гибрид";
            if (form.EngineType == "electro")
                fuelToOutput = "Электро";

            string engVolToOutput = "";
            if (form.EngineType == "petrol" || form.EngineType == "diesel")
                engVolToOutput = $"{form.EngineVolume.ToString()} куб.см";
            if (form.EngineType == "electro")
                engVolToOutput = $"{form.EngineVolume.ToString()} кВт/ч";

            string yearToOutput = "";
            if (form.EngineType == "petrol" || form.EngineType == "diesel")
                yearToOutput = $"Год выпуска: {form.YearOfManufacture.ToString()}\n";

            decimal rateToOutput = (form.Currency == "USD") ? rateUSD : rateEUR;

            string result = $"ИТОГО: {GetFormattedPrice(TAX)} грн.\n\n" +
                $"В том числе\n" +
                $"Акцизный сбор: {GetFormattedPrice(EXC)} грн.\n" +
                $"Пошлина: {GetFormattedPrice(TF)} грн.\n" +
                $"НДС: {GetFormattedPrice(VAT)} грн.\n\n" +
                "-------------\n\n" +
                $"ИТОГО: {GetFormattedPrice(TAX / rateToOutput)} {form.Currency}\n\n" +
                $"В том числе\n" +
                $"Акцизный сбор: {GetFormattedPrice(EXC / rateToOutput)} {form.Currency}\n" +
                $"Пошлина: {GetFormattedPrice(TF / rateToOutput)} {form.Currency}\n" +
                $"НДС: {GetFormattedPrice(VAT / rateToOutput)} {form.Currency}\n\n" +
                "-------------\n\n" +
                $"Рассчитано на основании введенных данных:\n" +
                $"Цена автомобиля: {form.InvoicePrice} {form.Currency}\n" +
                $"{fuelToOutput} {engVolToOutput}\n" +
                yearToOutput +
                $"Стоимость транспортировки: {form.TransportationToUABorderCost} {form.TransportationToUABorderCurrency}";
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
            List<Currency> allCurr = JsonSerializer.Deserialize<List<Currency>>(responseFromServer);
            List<Currency> cur = new List<Currency>();
            foreach (Currency item in allCurr)
            {
                if (item.cc.ToUpper() == "USD" || item.cc.ToUpper() == "EUR")
                {
                    cur.Add(item);
                }
            }
            return cur;
        }

        private static string GetFormattedPrice(decimal price)
        {
            price = Math.Round(price, 2);
            string formattedPrice = price.ToString("N2", CultureInfo.GetCultureInfo("ru-RU"));
            return formattedPrice;
        }
    }
}
