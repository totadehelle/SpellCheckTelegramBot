using System.Collections.Generic;
using System.Linq;
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
		private readonly Regex _isLeadingHyphen = new Regex(@"^[-]+");

		public SpellCheckModel(IWebHostEnvironment appEnvironment)
		{
			_appEnvironment = appEnvironment;
			_dictionaryRu = WordList.CreateFromFiles(_appEnvironment.WebRootPath + "/ru_RU.dic");
			_dictionaryEn = WordList.CreateFromFiles(_appEnvironment.WebRootPath + "/en_GB.dic");
		}
		public string SpellCheck(string message)
		{
			string fullResponse = null;
			if (message == null)
				return fullResponse;
			var cleanMessage = _notAlphabetical.Replace(message, "");
			cleanMessage = _isNewLine.Replace(cleanMessage, " ");
			var words = cleanMessage.Split(' ');
			var corrected = new List<string>();

			foreach (var word in words)
			{
				var cleanWord = _isLeadingHyphen.Replace(word, "");
				if (!string.IsNullOrEmpty(cleanWord))
				{
					//if the word contains both latin and cyrillic characters, it should be ignored
					if (_isLatinAndCyrillic.IsMatch(cleanWord))
					{
						continue;
					}
					
					var dictionary = _isLatin.IsMatch(cleanWord)
						? _dictionaryEn 
						: _dictionaryRu;
					
					bool isCorrect = dictionary.Check(cleanWord);
					if (!isCorrect)
					{
						var recommended = dictionary.Suggest(cleanWord);
						var checkerMessage = $"Неправильно: {cleanWord}. Правильно: ";
						if (recommended.Count() == 0)
							checkerMessage += "нет вариантов";
						else
						{
							foreach (var recommendation in recommended)
							{
								checkerMessage = checkerMessage + recommendation + ", ";
							}
							checkerMessage = checkerMessage.TrimEnd(new char[] { ',', ' ' });
						}
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