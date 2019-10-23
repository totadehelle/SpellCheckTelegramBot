using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using WeCantSpell.Hunspell;

namespace Bot.Models
{
	public class SpellCheckModel
	{
		private readonly IWebHostEnvironment _appEnvironment;
		private readonly WordList _dictionaryEn;
		private readonly WordList _dictionaryRu;
		private readonly Regex _notAlphaRegex = new Regex(@"[^A-ZА-ЯЁ -]", RegexOptions.IgnoreCase);
		private readonly Regex _isLatinAndCyrillic = new Regex(@"\b(?=\w*[A-Z])(?=\w*[А-ЯЁ]).+\b", RegexOptions.IgnoreCase);
		private readonly Regex _isLatin = new Regex(@"[A-Z]+", RegexOptions.IgnoreCase);

		public SpellCheckModel(IWebHostEnvironment appEnvironment)
		{
			_appEnvironment = appEnvironment;
			_dictionaryRu = WordList.CreateFromFiles(_appEnvironment.WebRootPath + "/ru_RU.dic");
			_dictionaryEn = WordList.CreateFromFiles(_appEnvironment.WebRootPath + "/en_GB.dic");
		}
		public string SpellCheck(string message)
		{
			var words = message.Split(' ', '\n');
			var corrected = new List<string>();
			string fullResponse = null;

			foreach (var word in words)
			{
				//delete all not alphabetical characters
				var cleanWord = _notAlphaRegex.Replace(word, "");

				if (!string.IsNullOrEmpty(cleanWord))
				{
					//if the word contains both latin and cyrillic characters, it should be ignored
					if (_isLatinAndCyrillic.Match(cleanWord).Success)
					{
						continue;
					}
					
					var dictionary = _isLatin.Match(cleanWord).Success 
						? _dictionaryEn 
						: _dictionaryRu;
					
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