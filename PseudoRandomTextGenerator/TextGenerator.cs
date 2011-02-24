using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PseudoRandomTextGenerator
{
	public class TextGenerator
	{
        private Dictionary<string, Dictionary<string, int>> _chain;
        private Random _random;

		public TextGenerator()
        {
            _chain = new Dictionary<string, Dictionary<string, int>>();
            _random = new Random();
        }

        public void ReadTextFile(string filePath)
        {
            TextReader reader = new StreamReader(filePath);
            var text = reader.ReadToEnd();
            reader.Close();

            var tokens = Split(text);

            AddTokensToChain(tokens);
        }

        private void AddTokensToChain(IList<string> tokens)
        {
            for (var i = 2; i < tokens.Count; i++)
            {
                AddToken(tokens[i - 2], tokens[i - 1], tokens[i]);
            }
        }

        private void AddToken(string key1, string key2, string value)
        {
            var chainKey = string.Format("{0} {1}", key1, key2);

            if (_chain.ContainsKey(chainKey))
            {
                if (!_chain[chainKey].ContainsKey(value)) _chain[chainKey].Add(value, 0);
            }
            else _chain.Add(chainKey, new Dictionary<string, int>() { { value, 0 } });
        }

        static IList<string> Split(string subject)
        {
            var tokens = new List<string>();
            var regex = new Regex(@"\s+");
            tokens.AddRange(regex.Split(subject));

            return tokens;
        }

        public string Write(int minLength, int maxLength)
        {
            var length = _random.Next(minLength, maxLength);

            var memoir = GetSeedString(_random.Next(_chain.Count)); ;
            var parts = Split(memoir);
            var key1 = parts[0];
            var key2 = parts[1];
            var nextWord = string.Empty;

            while (memoir.Length < length)
            {
                nextWord = GetNextWord(key1, key2);

                memoir += " " + nextWord;

                key1 = key2;
                key2 = nextWord;
            }

            return memoir;
        }

        private string GetNextWord(string key1, string key2)
        {
            var key = string.Format("{0} {1}", key1, key2);
			if (_chain.ContainsKey(key))
			{
				var possibleWords = _chain[key];

				var index = _random.Next(possibleWords.Count - 1);

				var nextWord = string.Empty;
				var count = 0;
				foreach (var possibleWord in possibleWords)
				{
					nextWord = possibleWord.Key;
					count++;
					if (count == index) break;
				}

				return nextWord;
			}

        	return GetSeedString(_random.Next(_chain.Count));
        }

        private string GetSeedString(int keyIndex)
        {
            var chainKeys = new string[_chain.Count];
            _chain.Keys.CopyTo(chainKeys, 0);
            return chainKeys[keyIndex];
        }
	}
}