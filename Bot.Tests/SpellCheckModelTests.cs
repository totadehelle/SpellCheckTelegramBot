using Bot.Models;
using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.Framework;

namespace Bot.Tests
{
	public class SpellCheckModelTests
	{
		//directory where the dictionaries are
		const string LocalStorage = "C:/Users/ext_elenas/source/repos/Bot/Bot/wwwroot";
		
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void SpellCheck_EnglishMessageIsCorrect_ReturnsNull()
		{
			var message = "Hello";
			Mock<IWebHostEnvironment> environment = new Mock<IWebHostEnvironment>();
			environment.Setup(e => e.WebRootPath).Returns(LocalStorage);
			var sut = new SpellCheckModel(environment.Object);

			var response = sut.SpellCheck(message);

			Assert.That(response, Is.Null);
		}

		[Test]
		public void SpellCheck_EnglishMessageWithNewlineCharacterIsCorrect_ReturnsNull()
		{
			var message = "Hello\nthere";
			Mock<IWebHostEnvironment> environment = new Mock<IWebHostEnvironment>();
			environment.Setup(e => e.WebRootPath).Returns(LocalStorage);
			var sut = new SpellCheckModel(environment.Object);

			var response = sut.SpellCheck(message);

			Assert.That(response, Is.Null);
		}

		[Test]
		public void SpellCheck_RussianMessageIsCorrect_ReturnsNull()
		{
			var message = "Привет";
			Mock<IWebHostEnvironment> environment = new Mock<IWebHostEnvironment>();
			environment.Setup(e => e.WebRootPath).Returns(LocalStorage);
			var sut = new SpellCheckModel(environment.Object);

			var response = sut.SpellCheck(message);

			Assert.That(response, Is.Null);
		}

		[Test]
		public void SpellCheck_MixedMessage_ReturnsNull()
		{
			var message = "бббbbb";
			Mock<IWebHostEnvironment> environment = new Mock<IWebHostEnvironment>();
			environment.Setup(e => e.WebRootPath).Returns(LocalStorage);
			var sut = new SpellCheckModel(environment.Object);

			var response = sut.SpellCheck(message);

			Assert.That(response, Is.Null);
		}

		[Test]
		public void SpellCheck_EnglishMessageIsIncorrect_ReturnsNotEmptyString()
		{
			var message = "Helo";
			Mock<IWebHostEnvironment> environment = new Mock<IWebHostEnvironment>();
			environment.Setup(e => e.WebRootPath).Returns(LocalStorage);
			var sut = new SpellCheckModel(environment.Object);

			var response = sut.SpellCheck(message);

			Assert.That(response, Is.Not.Null.Or.Empty);
		}

		[Test]
		public void SpellCheck_RussianMessageIsIncorrect_ReturnsNotEmptyString()
		{
			var message = "Првиет";
			Mock<IWebHostEnvironment> environment = new Mock<IWebHostEnvironment>();
			environment.Setup(e => e.WebRootPath).Returns(LocalStorage);
			var sut = new SpellCheckModel(environment.Object);

			var response = sut.SpellCheck(message);

			Assert.That(response, Is.Not.Null.Or.Empty);
		}

		[Test]
		public void SpellCheck_MessageIsNull_ReturnsNull()
		{
			string message = null;
			Mock<IWebHostEnvironment> environment = new Mock<IWebHostEnvironment>();
			environment.Setup(e => e.WebRootPath).Returns(LocalStorage);
			var sut = new SpellCheckModel(environment.Object);

			var response = sut.SpellCheck(message);

			Assert.That(response, Is.Null);
		}

		[Test]
		public void SpellCheck_MessageIsEmptyString_ReturnsNull()
		{
			var message = "";
			Mock<IWebHostEnvironment> environment = new Mock<IWebHostEnvironment>();
			environment.Setup(e => e.WebRootPath).Returns(LocalStorage);
			var sut = new SpellCheckModel(environment.Object);

			var response = sut.SpellCheck(message);

			Assert.That(response, Is.Null);
		}

		[Test]
		public void SpellCheck_MessageIsOneWhitespace_ReturnsNull()
		{
			var message = " ";
			Mock<IWebHostEnvironment> environment = new Mock<IWebHostEnvironment>();
			environment.Setup(e => e.WebRootPath).Returns(LocalStorage);
			var sut = new SpellCheckModel(environment.Object);

			var response = sut.SpellCheck(message);

			Assert.That(response, Is.Null);
		}

		[Test]
		public void SpellCheck_MessageIsMultipleWhitespaces_ReturnsNull()
		{
			var message = "     ";
			Mock<IWebHostEnvironment> environment = new Mock<IWebHostEnvironment>();
			environment.Setup(e => e.WebRootPath).Returns(LocalStorage);
			var sut = new SpellCheckModel(environment.Object);

			var response = sut.SpellCheck(message);

			Assert.That(response, Is.Null);
		}
		
		[Test]
		public void SpellCheck_MessageIsHyphen_ReturnsNull()
		{
			var message = "-";
			Mock<IWebHostEnvironment> environment = new Mock<IWebHostEnvironment>();
			environment.Setup(e => e.WebRootPath).Returns(LocalStorage);
			var sut = new SpellCheckModel(environment.Object);

			var response = sut.SpellCheck(message);

			Assert.That(response, Is.Null);
		}
	}
}