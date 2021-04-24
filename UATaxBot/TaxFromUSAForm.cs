using System;
using UATaxBot.Enums;
using UATaxBot.Services;

namespace UATaxBot
{
    class TaxFromUSAForm
    {
        public decimal CarPrice { get; set; }
        public CurrencyType CarPriceCurrency { get; set; }
        public int YearOfManufacture { get; set; }
        public EngineType CarEngineType { get; set; }
        public int EngineVolume { get; set; }
        public CurrencyType TransportToUABorderCurrency { get; set; }
        public decimal TransportToUABorderCost { get; set; }

        private int _calcTaxStage;

        public TaxFromUSAForm()
        {
            _calcTaxStage = 1;
        }

        public (string, int) GetCalcTaxStageText()
        {
            switch (_calcTaxStage)
            {
                case 1:
                    return ("\U0001F4B5\U0001F4B6 Выберите валюту покупки автомобиля:", _calcTaxStage);
                case 2:
                    return ("\U0001F4C4 Введите стоимость автомобиля:", _calcTaxStage);
                case 3:
                    return ("\U000026FD Выберите тип двигателя:", _calcTaxStage);
                case 4:
                    return ((CarEngineType == EngineType.Electro) ? "\U0001F50B Введите ёмкость батареи (кВт/ч):" : "\U00002747 Введите объём двигателя (куб.см):", _calcTaxStage);
                case 5:
                    return ("\U0001F3AB Введите год выпуска автомобиля:", _calcTaxStage);
                case 6:
                    return ("\U0001F4B5\U0001F4B6 Выберите валюту транспортировки до границы Украины:", _calcTaxStage);
                case 7:
                    return ("\U0001F4C4 Введите цену транспортировки до границы Украины:", _calcTaxStage);
                default:
                    string tax = TaxCalculationUSA.CalculateTax(this);
                    LogService.PrintLogText($"UserName", "calculated customs tax");
                    return (tax, -1);
            }
        }

        public bool SetCalcTaxParam(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return false;
            }

            switch (_calcTaxStage)
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
                            _calcTaxStage += 2;
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
                        if (int.TryParse(param, out engineVolume) && engineVolume > 0 && engineVolume <= maxValue)
                        {
                            EngineVolume = engineVolume;
                            _calcTaxStage++;
                            break;
                        }
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
            _calcTaxStage++;
            return true;
        }
    }
}
