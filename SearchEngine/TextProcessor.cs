using System;
using System.Text;
using System.Collections.Generic;
using PorterStemmerAlgorithm;
namespace SearchEngine
{
	
	public interface ITextProcessor
	{
		// returns text as a array of words
		string[] ProcessText(string inputText);
	}
	
	
	public class StandardTextProcessor : ITextProcessor
	{
		protected StemmerInterface stemmer;
		protected char[] splitMarks;
		protected string[] junkMarks;
		protected string[] stopWords;
		
		public StandardTextProcessor (StemmerInterface stemmer)
		{
			this.stemmer = stemmer;
//			splitMarks = new char[] {'.', ',', ';', ':', '?', ' '};
			splitMarks = new char[] {' '};
			junkMarks = new string[] {"\"", "/", "\\", "'", "(", ")", "`", "-", "_", "|", "©", "[", "]", "<", ">", ".", ",", ";", ":", "?", "+", "·" };
			stopWords = new string[] {"and", "a", "on", "of", "with", "in", "the", "etc"};
		}
		
		public string[] ProcessText (string inputText)
		{
			StringBuilder sb = new StringBuilder(inputText.ToLower());
			
			for (int i = 0; i < junkMarks.Length; i++)
			{
//				sb.Replace(junkMarks[i], "");
				sb.Replace(junkMarks[i], " ");
			}
		
//			for (int i = 0; i <stopWords.Length; i++)
//			{
//				sb.Replace()
//			}
			
			string[] words = sb.ToString().Split(splitMarks);
			List<string> stemmed = new List<string>();
			for (int i=0; i<words.Length; i++)
			{
				string word = words[i].Trim();
				if (word == string.Empty)
					continue;
				stemmed.Add(stemmer.stemTerm(word));
			}
			
			return stemmed.ToArray();
		}
		
//		public string ProcessText(string inputText)
//		{
//			string[] words = this.ProcessText(inputText);
//			StringBuilder sb = new StringBuilder();
//			for (int i =0; i < words.Length; i++)
//			{
//				if (words[i] != string.Empty)
//					sb.AppendFormat("{0} ", words[i]);						
//			}
//			
//			return sb.ToString().Trim();			
//		}
		
		public StemmerInterface Stemmer
		{
			get { return stemmer; }	
		}
	}
}

