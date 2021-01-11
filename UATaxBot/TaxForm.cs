using System;
using System.Collections.Generic;
using System.Text;

namespace UATaxBot
{
    class TaxForm
    {
        public string Name { get; set; }
        public string ChatId { get; set; }
        public string TargetId { get; set; }
        public decimal InvoicePrice { get; set; }
        public string Currency { get; set; }
        public int YearOfManufacture { get; set; }
        public string EngineType { get; set; }
        public int EngineVolume { get; set; }
        public string TransportationToUABorderCurrency { get; set; }
        public decimal TransportationToUABorderCost { get; set; }

        public int stage;

        public TaxForm(string id, string chat_id, string name)
        {
            stage = 1;
            Name = name;
            TargetId = id;
            ChatId = chat_id;
        }

        public (string, int) StageText()
        {
            if (stage == 1)
                return ("Выберите валюту покупки автомобиля:", stage);
            if (stage == 2)
                return ("Введите инвойсную стоимость автомобиля:", stage);
            if (stage == 3)
                return ("Введите год выпуска автомобиля:", stage);
            if (stage == 4)
                return ("Выберите тип двигателя:", stage);
            if (stage == 5)
                return ("Введите объем двигателя/ёмкость батареи:", stage);
            if (stage == 6)
                return ("Выберите валюту транспортировки до границы Украины:", stage);
            if (stage == 7)
                return ("Введите цену транспортировки до границы Украины:", stage);

            string tax = Model.CalculateTax(this);
            Visualizer.DrawLogText($"{Name} calculated customs tax     < {DateTime.Now} >");
            return (tax, 0);
        }

        public bool SetParam(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return false;
            }
            switch (stage)
            {
                case 1:
                    if (param.ToUpper() == "USD" || param.ToUpper() == "EUR")
                    {
                        Currency = param;
                        break;
                    }
                    return false;
                case 2:
                    decimal price;
                    param = param.Replace(',', '.');
                    if (decimal.TryParse(param, out price) && price > 0)
                    {
                        InvoicePrice = price;
                        break;
                    }
                    return false;
                case 3:
                    int year;
                    if (int.TryParse(param, out year) && year > 1920 && year <= DateTime.Now.Year)
                    {
                        YearOfManufacture = year;
                        break;
                    }
                    return false;
                case 4:
                    param = param.ToLower();
                    if (param == "petrol" || param == "diesel" || param == "gybrid" || param == "electro")
                    {
                        EngineType = param;
                        if (param == "gybrid")
                        {
                            EngineVolume = 1;
                            stage++;
                        }
                        break;
                    }
                    return false;
                case 5:
                    int engineVolume;
                    int maxValue = 9999;
                    if (EngineType == "electro")
                    {
                        maxValue = 400;
                    }
                    if (int.TryParse(param, out engineVolume) && engineVolume > 0 && engineVolume <= maxValue)
                    {
                        EngineVolume = engineVolume;
                        break;
                    }
                    return false;
                case 6:
                    if (param.ToUpper() == "USD" || param.ToUpper() == "EUR")
                    {
                        TransportationToUABorderCurrency = param;
                        break;
                    }
                    return false;
                case 7:
                    decimal priceToBorder;
                    param = param.Replace(',', '.');
                    if (decimal.TryParse(param, out priceToBorder) && priceToBorder > 0)
                    {
                        TransportationToUABorderCost = priceToBorder;
                        break;
                    }
                    return false;
            }
            stage++;
            return true;
        }
    }
}
