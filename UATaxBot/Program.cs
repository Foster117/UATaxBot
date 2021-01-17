using System;
using UATaxBot.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;

// bot.imex@ukr.net
// qweasdzxc1234

// http://www.freenom.com/
// bot.imex@ukr.net
// qweasdzxc1234

//"1509689010:AAE7TSS0mDGnLS1x-SFsXR2k-L_IM1X-cq0"    TRUE bot
//"1560358205:AAG4thqkHip7fBv2XabKntdZeErGFHM_290"    Test bot

namespace UATaxBot
{
    class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("1509689010:AAE7TSS0mDGnLS1x-SFsXR2k-L_IM1X-cq0");
        static Dictionary<string, TaxForm> calcTaxData = new Dictionary<string, TaxForm>();
        static void Main(string[] args)
        {
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnReceiveError += BotOnReceiveError;
            Bot.StartReceiving(Array.Empty<UpdateType>());

            Visualizer.DrawStartText(Bot.GetMeAsync().Result);

            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            Visualizer.DrawErrorMessage(e.ApiRequestException.ErrorCode, e.ApiRequestException.Message);
        }

        private static void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            MessageHandler(null, e);
        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            MessageHandler(e, null);
        }

        private static async void MessageHandler(MessageEventArgs messageArgs, CallbackQueryEventArgs callbackArgs)
        {
            Message message;
            string messageText;
            string chatId;
            TaxForm userForm;

            if (messageArgs != null)
            {
                message = messageArgs.Message;
                messageText = messageArgs.Message.Text;
                chatId = messageArgs.Message.From.Id.ToString();
            }
            else
            {
                message = callbackArgs.CallbackQuery.Message;
                messageText = callbackArgs.CallbackQuery.Data;
                chatId = callbackArgs.CallbackQuery.From.Id.ToString();
            }

            switch (messageText)
            {
                case "/start":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]{
                        new[] { new KeyboardButton("Рассчитать стоимость растаможки") },
                        new[] { new KeyboardButton("Контакты"), new KeyboardButton("Информация") },
                        new[] { new KeyboardButton("Показать курсы НБУ") },
                    });
                    await Bot.SendTextMessageAsync(chatId, Messages.StartText, replyMarkup: replyKeyboard);
                    Visualizer.DrawLogText($"{message.From.FirstName} {message.From.LastName}", "started bot");
                    return;
                /////--------------------------////
                case "Рассчитать стоимость растаможки":
                    TaxForm form = new TaxForm(message.From.Id.ToString(), message.Chat.Id.ToString(), $"{message.From.FirstName} {message.From.LastName}", ActionType.TaxCalculation);
                    calcTaxData.Remove(message.From.Id.ToString());
                    calcTaxData.Add(message.From.Id.ToString(), form);
                    (string, int) firstStageText = form.GetCalcTaxStageText();
                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] {
                            new[]{ InlineKeyboardButton.WithCallbackData("USD", "USD"),
                                   InlineKeyboardButton.WithCallbackData("EUR", "EUR")}
                            });
                    await Bot.SendTextMessageAsync(message.Chat.Id, firstStageText.Item1, replyMarkup: inlineKeyboard);
                    return;
                /////--------------------------////
                case "Контакты":
                    calcTaxData.Remove(message.From.Id.ToString());
                    await Bot.SendTextMessageAsync(message.Chat.Id, Messages.ContactsText);
                    Visualizer.DrawLogText($"{message.From.FirstName} {message.From.LastName}", "checked contacts");
                    return;
                /////--------------------------////
                case "Информация":
                    calcTaxData.Remove(message.From.Id.ToString());
                    await Bot.SendTextMessageAsync(message.Chat.Id, Messages.InformationText);
                    Visualizer.DrawLogText($"{message.From.FirstName} {message.From.LastName}", "checked information");
                    return;
                /////--------------------------////
                case "Показать курсы НБУ":
                    calcTaxData.Remove(message.From.Id.ToString());
                    await Bot.SendTextMessageAsync(message.Chat.Id, CurrencyRates.ShowCurrencyRates());
                    Visualizer.DrawLogText($"{message.From.FirstName} {message.From.LastName}", "checked currency rates");
                    return;
                /////--------------------------////
                default:
                    calcTaxData.TryGetValue(chatId, out userForm);
                    if (userForm == null)
                    {
                        return;
                    }
                    switch (userForm.ActionType)
                    {
                        case ActionType.TaxCalculation:
                            TaxCalculation.TaxCalculationProcess(Bot, calcTaxData, userForm, messageText, message);
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }
    }
}