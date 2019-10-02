using System;

namespace Bot.Models
{
	public static class AppConfig
	{
		public static string URL { get; } = "https://zkgzkgbot.herokuapp.com/";
		public static string KEY { get; } = Environment.GetEnvironmentVariable("BOT_KEY");
	}
}