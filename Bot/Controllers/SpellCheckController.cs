using Bot.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static System.String;

namespace Bot.Controllers
{
	[Route("")]
	[Route("bot")]
    [ApiController]
    public class SpellCheckController : ControllerBase
    {
	    private IWebHostEnvironment _appEnvironment;
	    private readonly SpellCheckModel model;

	    public SpellCheckController(IWebHostEnvironment appEnvironment)
	    {
		    _appEnvironment = appEnvironment;
			model = new SpellCheckModel(_appEnvironment);
	    }

		[Route("")]
		[Route("bot")]
		[Route("bot/update")]
		[HttpPost]
	    public async void UpdateAsync([FromBody] Update update)
	    {
		    var client = GetClient();
		    if (update == null) return;
		    var message = update.Message;
		    if (message?.Type == MessageType.Text)
		    {
			    var response = model.SpellCheck(message.Text);
			    if (!IsNullOrEmpty(response))
			    {
				    await client.SendTextMessageAsync(message.Chat.Id, response);
				}
		    }
	    }

	    private TelegramBotClient GetClient()
	    {
		    TelegramBotClient client = new TelegramBotClient(AppConfig.KEY);
		    var info = client.GetWebhookInfoAsync().Result;
		    if (info.Url != AppConfig.URL)
		    {
			    client.SetWebhookAsync(AppConfig.URL).Wait();
		    }
		    return client;
	    }
	}
}