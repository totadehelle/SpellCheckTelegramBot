using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using WeCantSpell.Hunspell;

namespace Bot.Models
{
	public class SpellCheckModel
	{
		private readonly IWebHostEnvironment _appEnvironment;
		private readonly WordList _dictionaryEn;
		private readonly WordList _dictionaryRu;
		private readonly Regex _notAlphabetical = new Regex(@"[^A-ZА-ЯЁ\-\n ]", RegexOptions.IgnoreCase);
		private readonly Regex _isLatinAndCyrillic = new Regex(@"\b(?=\w*[A-Z])(?=\w*[А-ЯЁ]).+\b", RegexOptions.IgnoreCase);
		private readonly Regex _isLatin = new Regex(@"[A-Z]+", RegexOptions.IgnoreCase);
		private readonly Regex _isNewLine = new Regex(@"[\n]+");

		public SpellCheckModel(IWebHostEnvironment appEnvironment)
		{
			_appEnvironment = appEnvironment;
			_dictionaryRu = WordList.CreateFromFiles(_appEnvironment.WebRootPath + "/ru_RU.dic");
			_dictionaryEn = WordList.CreateFromFiles(_appEnvironment.WebRootPath + "/en_GB.dic");
		}
		public string SpellCheck(string message)
		{
			var cleanMessage = _notAlphabetical.Replace(message, "");
			cleanMessage = _isNewLine.Replace(cleanMessage, " ");
			var words = cleanMessage.Split(' ');
			var corrected = new List<string>();
			string fullResponse = null;

			foreach (var word in words)
			{
				//delete all not alphabetical characters
				if (!string.IsNullOrEmpty(word))
				{
					//if the word contains both latin and cyrillic characters, it should be ignored
					if (_isLatinAndCyrillic.IsMatch(word))
					{
						continue;
					}
					
					var dictionary = _isLatin.IsMatch(word)
						? _dictionaryEn 
						: _dictionaryRu;
					
					bool isCorrect = dictionary.Check(word);
					if (!isCorrect)
					{
						var recommended = dictionary.Suggest(word);
						var checkerMessage = $"Неправильно: {word}. Варианты правильного написания: ";
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