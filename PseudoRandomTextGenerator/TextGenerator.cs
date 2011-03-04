using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PseudoRandomTextGenerator
{
	public class TextGenerator
	{
		private readonly int _keySize;
		private Dictionary<string, List<string>> _chain;
        private Random _random;

		public TextGenerator(int keySize = 2)
        {
			if(keySize<1||keySize>5) throw new ArgumentOutOfRangeException("keySize","Can be 1-5");

			_keySize = keySize;
			_chain = new Dictionary<string, List<string>>();
            _random = new Random();
        }

        public void ReadTextFile(string filePath)
        {
			ReadStreamAndInitialize(new StreamReader(filePath).BaseStream);
        }

		public void ReadStream(Stream stream)
		{
			ReadStreamAndInitialize(stream);
		}

		public void ReadText(string thisIsMySeedData)
		{
			InitializeChain(thisIsMySeedData);
		}

		private void ReadStreamAndInitialize(Stream stream)
		{
			TextReader reader = new StreamReader(stream);
			var text = reader.ReadToEnd();
			reader.Close();

			InitializeChain(text);
		}

		private void InitializeChain(string text)
		{
			var tokens = Split(text);

			if(tokens.Count < _keySize) throw new ArgumentException("The text is smaller than the key size");

			AddTokensToChain(tokens);
		}

		static IList<string> Split(string subject)
		{
			var regex = new Regex(@"\s+");
			return regex.Split(subject).ToList();
		}

        private void AddTokensToChain(IList<string> tokens)
        {
            for (var i = _keySize; i < tokens.Count; i++)
            {
            	var nonModifiedClosurei = i;
            	var keyTokens = Enumerable.Range(0, _keySize).Select(k => tokens[nonModifiedClosurei - (_keySize - k)].Trim());

            	AddToken(keyTokens, tokens[nonModifiedClosurei].Trim());
            }
        }

        private void AddToken(IEnumerable<string> keyTokens, string value)
        {
        	var chainKey = string.Join(" ", keyTokens);

            if (_chain.ContainsKey(chainKey))
            {
                if (!_chain[chainKey].Contains(value)) _chain[chainKey].Add(value);
            }
            else _chain.Add(chainKey, new List<string>() { value  });
        }

		public string Write(int minLength, int maxLength)
		{
			// once we get a string longer than this we are done
			// NOTE: minLength is a hard barrier, while maxLength is soft.
			var length = _random.Next(minLength, maxLength);

			// get the first key
			var seedString = GetSeedString(_random.Next(_chain.Count));
			var currentKey = seedString;
			// keep track of the sentance length
			var sentenceLength = seedString.Length;
			// keep the words in a list, so we aren't doing more string manipulation than necessary
			var words = new List<string> {seedString};
			
			string nextWord;

			// while we haven't reached the length
			// get the next word
			// add it to the list
			// increment the sentence length, add one for the space we will add in the join
			// update the current key by getting the tail and appending the nextWord
			while (sentenceLength < length)
			{
				nextWord = GetNextWord(currentKey);
				words.Add(nextWord);
				sentenceLength += (nextWord.Length + 1);
				currentKey = string.Join(" ", currentKey.Split(Convert.ToChar(" ")).Skip(1)) + " " + nextWord;
			}

			return string.Join(" ",words);
		}

        private string GetNextWord(string key)
        {
			if (_chain.ContainsKey(key))
			{
				var index = _random.Next(_chain[key].Count - 1);
				return _chain[key][index];
			}

        	return GetSeedString(_random.Next(_chain.Count));
        }

        private string GetSeedString(int keyIndex)
        {
        	return _chain.Keys.ToArray()[keyIndex];
        }
	}
}