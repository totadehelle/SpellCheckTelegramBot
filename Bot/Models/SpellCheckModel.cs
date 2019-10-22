using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using WeCantSpell.Hunspell;

namespace Bot.Models
{
	public class SpellCheckModel
	{
		private readonly IWebHostEnvironment _appEnvironment;
		private readonly WordList _dictionary;

		public SpellCheckModel(IWebHostEnvironment appEnvironment)
		{
			_appEnvironment = appEnvironment;
			_dictionary = WordList.CreateFromFiles(_appEnvironment.WebRootPath + "/ru_RU.dic");
		}
		public string SpellCheck(string message)
		{
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
					bool isCorrect = _dictionary.Check(cleanWord);
					if (!isCorrect)
					{
						var recommended = _dictionary.Suggest(cleanWord);
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