using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeeGraph.Core;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeeGraph.TelegramBot
{
    class Program
    {
        private const string Token = "";
        private static readonly TelegramBotClient Bot = new TelegramBotClient(Token);

        static void Main(string[] args)
        {
            var dialogService = IoC.IoC.Container.GetInstance<IDialogService>();
            var graph = dialogService.GetAll().First();

            SharedDialog = new UserDialog(new StatefulDialog(graph));

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;
            Bot.StartReceiving();
            Console.ReadLine();
        }

        private static async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            var query = e.CallbackQuery;

            (var response, var options) = SharedDialog.Talk(query.Data);

            await Bot.SendTextMessageAsync(query.From.Id, response);

            var buttons = options.Select(o => new InlineKeyboardCallbackButton(o.Key, o.Key)).ToArray();

            var keyboard = new InlineKeyboardMarkup(buttons);

            await Bot.SendTextMessageAsync(query.From.Id, "Choose",
                replyMarkup: keyboard);
        }

        static UserDialog SharedDialog;

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null || message.Type != MessageType.TextMessage) return;


        }
    }
}
