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
        public static TelegramBotClient Bot => Program.Bot;
        public static Dictionary<string, Customer> ActiveCustomersCollection => Program.ActiveCustomersCollection;

        public async void SelectAction(Customer customer)
        {
            switch (customer.MessageText)
            {
                case "/start":
                    StartAction.Go(customer);
                    return;
                /////--------------------------////
                case "Контакты":
                    ContactAction.Go(customer);
                    return;
                /////--------------------------////
                case "Информация":
                    InformationAction.Go(customer);
                    return;
                /////--------------------------///
                case "Показать курсы НБУ":
                    ExchangeRatesAction.Go(customer);
                    return;
                /////--------------------------////
                case "Рассчитать стоимость растаможки (USA)":
                    customer.ActionType = ActionType.TaxFromUSACalculation;
                    customer.TaxFromUSAForm = new TaxFromUSAForm();
                    (string, int) firstStageText = customer.TaxFromUSAForm.GetCalcTaxStageText();
                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] {
                                new[]{ InlineKeyboardButton.WithCallbackData("USD", "USD"),
                                       InlineKeyboardButton.WithCallbackData("EUR", "EUR")}
                                });
                    await Bot.SendTextMessageAsync(customer.ChatId, firstStageText.Item1, replyMarkup: inlineKeyboard);
                    return;

                ///////--------------------------////
                ///
                default:
                    ActiveCustomersCollection.TryGetValue(customer.ChatId, out customer);
                    if (customer == null)
                    {
                        return;
                    }
                    switch (customer.ActionType)
                    {
                        case ActionType.TaxFromUSACalculation:
                            TaxCalculationUSA.TaxCalculationProcess(Bot, customer);
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }
    }
}
