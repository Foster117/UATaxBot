using System;
using UATaxBot.Enums;

namespace UATaxBot
{
    class TaxForm
    {
        public string Name { get; set; }
        public string ChatId { get; set; }
        public string TargetId { get; set; }
        public decimal CarPrice { get; set; }
        public CurrencyType CarPriceCurrency { get; set; }
        public int YearOfManufacture { get; set; }
        public EngineType CarEngineType { get; set; }
        public int EngineVolume { get; set; }
        public CurrencyType TransportToUABorderCurrency { get; set; }
        public decimal TransportToUABorderCost { get; set; }
        public ActionType ActionType { get; set; }

        private int calcTaxStage;

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
            if (ActionType == ActionType.TaxCalculation)
            {
                switch (calcTaxStage)
                {
                    case 1:
                        return ("Выберите валюту покупки автомобиля:", calcTaxStage);
                    case 2:
                        return ("Введите стоимость автомобиля:", calcTaxStage);
                    case 3:
                        return ("Выберите тип двигателя:", calcTaxStage);
                    case 4:
                        return ((CarEngineType == EngineType.Electro) ? "Введите ёмкость батареи (кВт/ч):" : "Введите объём двигателя (куб.см):", calcTaxStage);
                    case 5:
                        return ("Введите год выпуска автомобиля:", calcTaxStage);
                    case 6:
                        return ("Выберите валюту транспортировки до границы Украины:", calcTaxStage);
                    case 7:
                        return ("Введите цену транспортировки до границы Украины:", calcTaxStage);
                    default:
                        string tax = TaxCalculation.CalculateTax(this);
                        Visualizer.DrawLogText($"{Name}", "calculated customs tax");
                        return (tax, -1);
                }
            }
            return (null, 0);
        }

        public bool SetCalcTaxParam(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return false;
            }
            if (ActionType == ActionType.TaxCalculation)
            {
                switch (calcTaxStage)
                {
                    case 1:
                        switch (param.ToUpper())
                        {
                            case "USD":
                                CarPriceCurrency = CurrencyType.USD;
                                break;
                            case "EUR":
                                CarPriceCurrency = CurrencyType.EUR;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case 2:
                        decimal price;
                        param = param.Replace(',', '.');
                        if (decimal.TryParse(param, out price) && price > 0)
                        {
                            CarPrice = price;
                            break;
                        }
                        return false;
                    case 3:
                        switch (param.ToLower())
                        {
                            case "petrol":
                                CarEngineType = EngineType.Petrol;
                                break;
                            case "diesel":
                                CarEngineType = EngineType.Diesel;
                                break;
                            case "hybrid":
                                CarEngineType = EngineType.Hybrid;
                                EngineVolume = 0;
                                YearOfManufacture = 0;
                                calcTaxStage += 2;
                                break;
                            case "electro":
                                CarEngineType = EngineType.Electro;
                                YearOfManufacture = 0;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case 4:
                        int engineVolume;
                        int maxValue = 9999;
                        if (CarEngineType == EngineType.Electro)
                        {
                            maxValue = 400;
                            YearOfManufacture = 0;
                            calcTaxStage++;
                        }
                        if (int.TryParse(param, out engineVolume) && engineVolume > 0 && engineVolume <= maxValue)
                        {
                            EngineVolume = engineVolume;
                            break;
                        }
                        return false;
                    case 5:
                        int year;
                        if (int.TryParse(param, out year) && year > 1920 && year <= DateTime.Now.Year)
                        {
                            YearOfManufacture = year;
                            break;
                        }
                        return false;
                    case 6:
                        switch (param.ToUpper())
                        {
                            case "USD":
                                TransportToUABorderCurrency = CurrencyType.USD;
                                break;
                            case "EUR":
                                TransportToUABorderCurrency = CurrencyType.EUR;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case 7:
                        decimal priceToBorder;
                        param = param.Replace(',', '.');
                        if (decimal.TryParse(param, out priceToBorder) && priceToBorder >= 0)
                        {
                            TransportToUABorderCost = priceToBorder;
                            break;
                        }
                        return false;
                }
                calcTaxStage++;
                return true;
            }
            return false;
        }
    }
}
