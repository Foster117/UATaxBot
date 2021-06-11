using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using UATaxBot.Entities;
using UATaxBot.Services;

namespace UATaxBot.Actions
{
    static class StartMenuAction
    {
        public static TelegramBotClient Bot => Program.Bot;
        public static Dictionary<string, Customer> ActiveCustomersCollection => Program.ActiveCustomersCollection;
        public static async void Go(Customer customer)
        {
            ReplyKeyboardMarkup replyKeyboard = new ReplyKeyboardMarkup(new[]{
                        // Row 1
                        new[] { 
                            new KeyboardButton(TextManager.MenuCarFromUsaInaccurate), 
                            new KeyboardButton(TextManager.MenuCarFromKoreaInaccurate) 
                        },

                        // Row 2
                        new[] { 
                            new KeyboardButton(TextManager.MenuCarFromEuropeInaccurate), 
                            new KeyboardButton(TextManager.MenuInformation) 
                        },

                        // Row 3
                        new[] { new KeyboardButton(TextManager.MenuCarFromUsaKoreaEuropeAccurate) },

                        // Row 4
                        new[] { 
                            new KeyboardButton(TextManager.MenuCarEurobadgeAccurate), 
                            new KeyboardButton(TextManager.MenuEurobadgeInformation) 
                        },

                        // Row 5
                        new[] { 
                            new KeyboardButton(TextManager.MenuNBUCurrencyRates), 
                            new KeyboardButton(TextManager.MenuContacts) 
                        }});

            await Bot.SendTextMessageAsync(customer.ChatId, TextManager.StartText, replyMarkup: replyKeyboard);
            LogService.PrintLogText($"{customer.FirstName} {customer.LastName}", TextManager.ConsoleStartedBot);
            ActiveCustomersCollection.Remove(customer.ChatId);
        }
    }
}
