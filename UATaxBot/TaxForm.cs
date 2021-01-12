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
        public ActionType ActionType { get; set; }

        public int calcTaxStage;

        public TaxForm(string id, string chat_id, string name, ActionType actionType)
        {
            calcTaxStage = 1;
            Name = name;
            TargetId = id;
            ChatId = chat_id;
            ActionType = ActionType;
        }

        public (string, int) GetCalcTaxStageText()
        {
            if (calcTaxStage == 1)
                return ("Выберите валюту покупки автомобиля:", calcTaxStage);
            if (calcTaxStage == 2)
                return ("Введите инвойсную стоимость автомобиля:", calcTaxStage);
            if (calcTaxStage == 3)
                return ("Введите год выпуска автомобиля:", calcTaxStage);
            if (calcTaxStage == 4)
                return ("Выберите тип двигателя:", calcTaxStage);
            if (calcTaxStage == 5)
                return ("Введите объем двигателя/ёмкость батареи:", calcTaxStage);
            if (calcTaxStage == 6)
                return ("Выберите валюту транспортировки до границы Украины:", calcTaxStage);
            if (calcTaxStage == 7)
                return ("Введите цену транспортировки до границы Украины:", calcTaxStage);

            string tax = Model.CalculateTax(this);
            Visualizer.DrawLogText($"{Name} calculated customs tax     < {DateTime.Now} >");
            return (tax, -1);
        }

        public bool SetCalcTaxParam(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return false;
            }
            switch (calcTaxStage)
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
                            calcTaxStage++;
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
            calcTaxStage++;
            return true;
        }
    }
}
