using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEngine
{
	public abstract class SearchElement
	{
		protected string header;
		protected string body;
		protected string[] fullStemmed;
		protected int[] bagOfWords;
		protected double[] tf;
		protected double tfWidth;
		protected double[] tfIdf;
		protected double tfIdfWidth;
				
		
		public SearchElement (string header, string body)
		{
			this.header = header.ToLower();
			this.body = body.ToLower();
		}
		
		public virtual void ProcessElement(ITextProcessor processor)
		{
			string [] words = processor.ProcessText(header+ " " +body);
			if (words.Length < 1)
				return;
			this.fullStemmed = words;
		}
		
		public virtual int[] CalcualteBagOfWords(List<string> terms)
		{
			if (terms.Count == 0)
				throw new ArgumentException("Przekazane kolekcje terms ma zerową długość");
			
			bagOfWords = new int[terms.Count];
			
			for (int i = 0; i < bagOfWords.Length; i++)
				bagOfWords[i] = 0;
		
			for (int i = 0; i < terms.Count; i++)
			{
				for (int j = 0; j < fullStemmed.Length; j++)
				{
					if (fullStemmed[j].Equals(terms[i]))
						bagOfWords[i]++;
				}
			}
			
			return bagOfWords;
		}
		
		public virtual void CalculateTfIdf(List<string> terms, List<double> idf)
		{		
			if (terms.Count != idf.Count)
				throw new ArgumentException("Przekazane kolekcje terms i idf muszą mieć tę samą długość");
			if (bagOfWords == null)
				throw new InvalidOperationException("Reprezentacja bag-of-words nie została jeszcze wyliczona");
			if (terms.Count != bagOfWords.Length)
				throw new ArgumentException("Przekazana kolekcja terms nie pasuje do listy bagOfWords");
			
			int maxTerm = bagOfWords[0];
			for (int i = 1; i<bagOfWords.Length; i++)
				if (maxTerm < bagOfWords[i])
					maxTerm = bagOfWords[i];
			
			
			tf = new double[bagOfWords.Length];
			tfIdf = new double[bagOfWords.Length];
			for (int i = 0; i < tf.Length; i++)
			{
				if (maxTerm == 0)
					tf[i] = 0;
				else
					tf[i] = (double)bagOfWords[i] / maxTerm;
				tfIdf[i] = tf[i] * idf[i];
				tfWidth += Math.Pow(tf[i], 2.0);
				tfIdfWidth += Math.Pow(tfIdf[i], 2.0);
			}
			
			tfWidth = Math.Sqrt(tfWidth);			
			tfIdfWidth = Math.Sqrt(tfIdfWidth);
		}
				
		public string Header
		{
			get { return header; }
		}
		
		public string Body
		{
			get {return body; }
		}
		
		// header + body po stemmingu
		public string FullTextProcessed
		{
			get 
			{
				if (fullStemmed == null)
					throw new InvalidOperationException("Dokument nie został jeszcze przetworzony");
				StringBuilder sb = new StringBuilder();
				for(int i =0; i<fullStemmed.Length; i++)
				{
					sb.AppendFormat("{0} ", fullStemmed[i]);
				}
				return sb.ToString().Trim();
			}
		}
	}
}

