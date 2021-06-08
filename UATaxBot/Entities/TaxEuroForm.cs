using System;
using UATaxBot.Enums;
using UATaxBot.Services;

namespace UATaxBot.Entities
{
    class TaxEuroForm
    {
        private Customer _Customer { get; set; }
        public int YearOfManufacture { get; set; }
        public EngineType CarEngineType { get; set; }
        public int EngineVolume { get; set; }
        public bool isValidYear { get; set; } = true;
        private int _calcTaxStage = 0;

        public TaxEuroForm(Customer customer)
        {
            _Customer = customer;
        }

        public (string, int) GetCalcTaxStageText()
        {
            switch (_calcTaxStage)
            {
                case 1:
                    return ("\U0001F3AB Введите год выпуска автомобиля:", _calcTaxStage);
                case 2:
                    return ("\U000026FD Выберите тип двигателя:", _calcTaxStage);
                case 3:
                    return ("\U00002747 Введите объём двигателя (куб.см):", _calcTaxStage);
                default:
                    string tax = TaxEuroCalculation.CalculateTax(this);
                    LogService.PrintLogText($"{_Customer.FirstName} {_Customer.LastName}", "calculated customs tax");
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
                    int year;
                    if (int.TryParse(param, out year) && year > 1920 && year <= DateTime.Now.Year)
                    {
                        if ((DateTime.Now.Year - year - 1) < 5)
                        {
                            isValidYear = false;
                            _calcTaxStage += 2; 
                        }
                        YearOfManufacture = year;
                        break;
                    }
                    return false;
                case 2:
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
                            break;
                        default:
                            return false;
                    }
                    break;
                case 3:
                    int engineVolume;
                    int maxValue = 9999;
                    if (int.TryParse(param, out engineVolume) && engineVolume > 0 && engineVolume <= maxValue)
                    {
                        EngineVolume = engineVolume;
                        break;
                    }
                    return false;
            }
            _calcTaxStage++;
            return true;
        }
    }
}
