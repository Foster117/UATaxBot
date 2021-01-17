using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace UATaxBot
{
    class CurrencyRates
    {
        public static List<Currency> GetExchangeRate()
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

        public static string ShowCurrencyRates()
        {
            decimal rateUSD = 0, rateEUR = 0;
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

            StringBuilder result = new StringBuilder();
            result.Append($"\U0001F4E2 Курс НБУ на {DateTime.Now.Day:d2}/{DateTime.Now.Month:d2}/{DateTime.Now.Year}г.:\n\n");
            result.Append($"{rateUSD} грн / USD 1.00\n");
            result.Append($"{rateEUR} грн / EUR 1.00\n\n");
            result.Append($"Кросс-курс:\nUSD {Math.Round(rateEUR / rateUSD, 2)} / EUR 1,00");

            return result.ToString();
        }
    }
}
