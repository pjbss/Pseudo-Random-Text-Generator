using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using PseudoRandomTextGenerator;

namespace UnitTests
{
	/// <summary>
	/// if an exception is not thrown then the contents were read successfully
	/// </summary>
	[TestFixture]
	public class text_generator_read_tests
	{
		[Test]
		[ExpectedException(typeof(FileNotFoundException))]
		public void invalid_path_throws_exception()
		{
			_textGenerator.ReadTextFile("this file path");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void text_less_than_keySize_throws_exception()
		{
			_textGenerator.ReadText("one");
		}

		[Test]
		public void can_read_file()
		{
			_textGenerator.ReadTextFile(_filePath);
		}

		[Test]
		public void can_read_stream()
		{
			var stream = new StreamReader(_filePath).BaseStream;
			_textGenerator.ReadStream(stream);
		}

		[Test]
		public void can_read_text()
		{
			_textGenerator.ReadText("this is my seed data");
		}

		private TextGenerator _textGenerator;
		private string _filePath;

		[SetUp]
		public void SetUp()
		{
			_textGenerator = new TextGenerator();
			_filePath = string.Format(@"{0}\AllsWellThatEndsWell.txt", Assembly.GetExecutingAssembly().Directory());
		}
	}
}