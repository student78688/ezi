﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PorterStemmerAlgorithm;

namespace SearchEngine
{
	public class CalculationException : Exception
	{
		public CalculationException(string message):
			base(message)
		{
		}
	}
	
	
	public class TfIdfCalc
	{
		protected List<string> terms;
		protected List<double> idf;
		protected List<SearchDocument> documents;
		protected ITextProcessor textProcessor;
		
		public TfIdfCalc (ITextProcessor textProcessor)
		{
			this.textProcessor = textProcessor;
		}

		public bool ReadTerms (string dataFilePath)
		{
			List<string> tmpTerms = new List<string>();
			
			try
			{
				using (FileStream fs = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read))
				{
					using (StreamReader sr = new StreamReader(fs))
					{
						while (!sr.EndOfStream)	
						{
							string term = ((StandardTextProcessor)textProcessor).Stemmer.stemTerm(sr.ReadLine().ToLower());
							if (!tmpTerms.Contains(term))
								tmpTerms.Add(term);
						}
					}
				}
			}
			catch (FileNotFoundException ex)
			{
				throw new ArgumentException(String.Format("Plik nie został znaleziony. {0}", ex.Message));
			}
			catch (IOException ex)
			{
				throw new ArgumentException(String.Format("Błąd podczas odczytu pliku. {0}", ex.Message));
			}
			
			if (tmpTerms.Count > 0)
			{
				terms = tmpTerms;
				return true;
			}
			else
				return false;
		}
		
		public bool ReadDocuments(string dataFilePath)
		{
			List<SearchDocument> tmpDocuments = new List<SearchDocument>();
			StringBuilder sb = new StringBuilder();
			string header = string.Empty;
			string group = string.Empty;
			
			try
			{
				using (FileStream fs = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read))
				{
					using (StreamReader sr = new StreamReader(fs))
					{
						while (!sr.EndOfStream)	
						{
							sb.Remove(0, sb.Length);
							string line;
							int counter = 0;
							header = string.Empty;	
							group = string.Empty;
						
							while(((line = sr.ReadLine()) != string.Empty))
							{
								if (counter == 0)
									group = (string.Format("{0}", line)).Trim();
								if (counter == 1)
									header = (string.Format("{0} ", line)).Trim();
								else
									sb.Append(string.Format("{0} ", line));
								counter++;
								
								if (sr.EndOfStream)
									break;
							}
							
							SearchDocument document = new SearchDocument(header, sb.ToString(), group);
							tmpDocuments.Add(document);
						}
					}
				}
			}
			catch (FileNotFoundException ex)
			{
				throw new ArgumentException(String.Format("Plik nie został znaleziony. {0}", ex.Message));
			}
			catch (IOException ex)
			{
				throw new ArgumentException(String.Format("Błąd podczas odczytu pliku", ex.Message));
			}	
			
			
			// czyszczenie i stemming wczytanych dokumentów
			if (tmpDocuments.Count > 0)
			{
				for(int i=0; i < tmpDocuments.Count; i++)
					tmpDocuments[i].ProcessElement(textProcessor);
				
				documents = tmpDocuments;
				return true;
			}
			else
				return false;
		}
		
		
		public void CalucateTfIdfFactors()
		{
			if (!this.IsReady)
				return;
			
			#region wyliczenie IDF
			List<int[]> bagOfWords = new List<int[]>();
			idf = new List<double>();
			
			for (int i = 0; i < documents.Count; i++)
			{
				bagOfWords.Add(documents[i].CalcualteBagOfWords(terms));	
			}
			
			for (int i = 0; i < terms.Count; i++)
			{
				int termOccurents = 0;
				for (int j = 0; j < bagOfWords.Count; j++)
				{
					if (bagOfWords[j][i] > 0)
						termOccurents++;
				}
				
				// dopuszczenie termow, ktore nie wystepuja w zadnym z dokumentow
				if (termOccurents == 0)
				{
//					throw new CalculationException(string.Format("Term: \"{0}\" nie występuje w żadnym dokumencie. " +
//						"Wczytane dokumenty i termy nie pasują do siebie", terms[i]));
					idf.Add(0.0);
					System.Console.WriteLine(String.Format("UWAGA Term: \"{0}\" nie występuje w żadnym dokumencie. " +
						"Wczytane dokumenty i termy nie pasują do siebie", terms[i]));
				}
				else
				{
					idf.Add(Math.Log10((double)documents.Count / (double)termOccurents));
				}

//				System.Console.WriteLine("i={0} termoccurents = {1}, idf = {2}", i, termOccurents, idf[i]);
			}
			#endregion
			
			// wyliczenie TF-IDF
			for(int i = 0; i < documents.Count; i++)
			{
				documents[i].CalculateTfIdf(terms, idf);
			}
		}
					
		
		public List<SearchDocument> PerformSearch(string query)
		{		
			SearchQuery q = new SearchQuery(string.Empty, query);
			q.ProcessElement(textProcessor);
			q.CalcualteBagOfWords(terms);
			q.CalculateTfIdf(terms, idf);
			
			for (int i = 0; i < documents.Count; i++)
			{
				documents[i].CalculateTfIdfSimilarity(q);
			}
			
			List<SearchDocument> sortedDocuments = new List<SearchDocument>(documents);
			sortedDocuments.Sort();
			sortedDocuments.Reverse();
			
			return sortedDocuments;			
		}
		
		public bool IsReady 
		{
			get 
			{
				if (terms != null && documents != null)
				{
					return true;
				}
				return false;
			}
		}
		
		public List<SearchDocument> Documents
		{
			get { return documents; }
		}
		
		public void ResetData()
		{
			terms = null;
			documents = null;
			idf = null;
		}
		
		public string ProcessedDocuments
		{
			get 
			{
				if (documents == null)
					return string.Empty;
				if (documents.Count == 0)
					return string.Empty;
				
				StringBuilder sb = new StringBuilder();
				foreach (SearchDocument document in documents)
				{
					sb.AppendLine(document.Header);
					sb.Append(document.FullTextProcessed);
					sb.AppendLine("\n");
				}
				
				return sb.ToString();
			}
		}
		
		public string ProcessedTerms
		{
			
			get 
			{
				if (terms == null)
					return string.Empty;
				if (terms.Count == 0)
					return string.Empty;
				
				StringBuilder sb = new StringBuilder();
				foreach (string term in terms)
				{
					sb.AppendLine(term);
				}
				
				return sb.ToString();
			}
		}
		
	}
}

