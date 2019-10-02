using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using WeCantSpell.Hunspell;

namespace Bot.Models
{
	public class SpellCheckModel
	{
		private IWebHostEnvironment _appEnvironment;

		public SpellCheckModel(IWebHostEnvironment appEnvironment)
		{
			_appEnvironment = appEnvironment;
		}
		public string SpellCheck(string message)
		{
			var dictionary = WordList.CreateFromFiles(_appEnvironment.WebRootPath + "/ru_RU.dic");
			var words = message.Split(' ', '\n');
			var corrected = new List<string>();
			string fullResponse = null;

			foreach (var word in words)
			{
				var cleanWord = word.Trim(new char[]
				{
					'.', ',', '!', '?', ';', ':', '-', ' ', '*',
					'@', '#', '№', '$', '%', '^','&','"', '/', '`', '~',
					'(', ')', '[', ']', '{','}', '\n'
				});
				if (!string.IsNullOrEmpty(cleanWord))
				{
					bool isCorrect = dictionary.Check(cleanWord);
					if (!isCorrect)
					{
						var recommended = dictionary.Suggest(cleanWord);
						var checkerMessage = $"Неправильно: {cleanWord}. Варианты правильного написания: ";
						foreach (var recommendation in recommended)
						{
							checkerMessage = checkerMessage + recommendation + ", ";
						}
						checkerMessage = checkerMessage.TrimEnd(new char[] { ',', ' ' });
						corrected.Add(checkerMessage);
					}
				}
			}

			if (corrected.Count > 0)
			{
				foreach (var correction in corrected)
				{
					fullResponse += "- " + correction + "\n";
				}
			}

			return fullResponse;
		}
	}
}