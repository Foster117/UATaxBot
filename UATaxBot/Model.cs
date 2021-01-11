using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.Json;

namespace UATaxBot
{
    class Model
    {
        public static string CalculateTax(TaxForm form)
        {
            decimal rateUSD = 0,  rateEUR = 0;
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



            CP = form.InvoicePrice + form.TransportationToUABorderCost;

            KY = DateTime.Now.Year - form.YearOfManufacture - 1;
            if (KY > 15)
            {
                KY = 15;
            }
            if (KY < 1)
            {
                KY = 1;
            }

      




            return $"{rateUSD}  {rateEUR}  {KY}";
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
    }
}
