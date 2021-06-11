using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using UATaxBot.Entities;
using UATaxBot.Services;
using UATaxBot.Actions;
using UATaxBot.Enums;

namespace UATaxBot
{
    class ActionManager
    {
        public static Dictionary<string, Customer> ActiveCustomersCollection => Program.ActiveCustomersCollection;

        public void SelectAction(Customer customer)
        {
            switch (customer.MessageText)
            {
                case "/start":
                    StartMenuAction.Go(customer);
                    break;

                case TextManager.MenuContacts:
                    ContactMenuAction.Go(customer);
                    break;

                case TextManager.MenuInformation:
                    InformationMenuAction.Go(customer);
                    break;

                case TextManager.MenuNBUCurrencyRates:
                    ExchangeRatesMenuAction.Go(customer);
                    break;

                case TextManager.MenuCarFromUsaKoreaEuropeAccurate:
                    CalculateTaxMenuAction.Go(customer);
                    break;

                case TextManager.MenuCarEurobadgeAccurate:
                    CalculateTaxEuroMenuAction.Go(customer);
                    break;

                default:
                    RouteAction.Go(customer);
                    break;
            }
        }
    }
}
