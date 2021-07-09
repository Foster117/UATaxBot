using System;
using UATaxBot.Enums;
using UATaxBot.Services;

namespace UATaxBot.Entities
{
    class TaxAccurateCalculationForm
    {
        private Customer _Customer { get; set; }
        public decimal CarPrice { get; set; }
        public CurrencyType CarPriceCurrency { get; set; }
        public int YearOfManufacture { get; set; }
        public EngineType CarEngineType { get; set; }
        public int EngineVolume { get; set; }
        public CurrencyType TransportToUABorderCurrency { get; set; }
        public decimal TransportToUABorderCost { get; set; }
        private int _calcTaxStage = 0;

        public TaxAccurateCalculationForm(Customer customer)
        {
            _Customer = customer;
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
                    return ("Какие затраты добавить?", _calcTaxStage);
                case 4:
                    return ("Введите сумму:", _calcTaxStage);
                case 5:
                    return ("\U000026FD Выберите тип двигателя:", _calcTaxStage);
                case 6:
                    return ((CarEngineType == EngineType.Electro) ? "\U0001F50B Введите ёмкость батареи (кВт/ч):" : "\U00002747 Введите объём двигателя (куб.см):", _calcTaxStage);
                case 7:
                    return ("\U0001F3AB Введите год выпуска автомобиля:", _calcTaxStage);
                default:
                    string tax = TaxAccurateCalculation.CalculateTax(this);
                    LogService.PrintLogText($"{_Customer.FirstName} {_Customer.LastName}", 
                        TextManager.ConsoleCustomerCalculatesTaxAccurate);
                    return (tax, -1);
            }
        }

        public bool SetCalcTaxParam(string parameter)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                return false;
            }

            switch (_calcTaxStage)
            {
                case 1:
                    switch (parameter.ToUpper())
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
                    decimal enteredCarPrice;
                    parameter = parameter.Replace(',', '.');
                    if (decimal.TryParse(parameter, out enteredCarPrice) && enteredCarPrice > 0)
                    {
                        CarPrice = enteredCarPrice;
                        break;
                    }
                    return false;
                case 3:
                    switch (parameter)
                    {
                        case TextManager.AuctionComission:
                            break;
                        case TextManager.InsuranceCost:
                            break;
                        case TextManager.TransportationByLand:
                            break;
                        case TextManager.TransportationByWater:
                            break;
                        case TextManager.OtherExpenses:
                            break;
                        case TextManager.NotAdd:
                            break;   
                    }
                    return false;

                case 4:
                    decimal enteredPrice;
                    parameter = parameter.Replace(',', '.');
                    if (decimal.TryParse(parameter, out enteredPrice) && enteredPrice >= 0)
                    {
                        ///////// TODO: Implement filtering
                        break;
                    }
                    return false;
                case 5:
                    switch (parameter.ToLower())
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
                case 6:
                    int engineVolume;
                    int maxValue = 9999;
                    if (CarEngineType == EngineType.Electro)
                    {
                        maxValue = 400;
                        YearOfManufacture = 0;
                        if (int.TryParse(parameter, out engineVolume) && engineVolume > 0 && engineVolume <= maxValue)
                        {
                            EngineVolume = engineVolume;
                            _calcTaxStage++;
                            break;
                        }
                    }
                    if (int.TryParse(parameter, out engineVolume) && engineVolume > 0 && engineVolume <= maxValue)
                    {
                        EngineVolume = engineVolume;
                        break;
                    }
                    return false;
                case 7:
                    int year;
                    if (int.TryParse(parameter, out year) && year > 1920 && year <= DateTime.Now.Year)
                    {
                        YearOfManufacture = year;
                        break;
                    }
                    return false;
            }
            _calcTaxStage++;
            return true;
        }
    }
}
