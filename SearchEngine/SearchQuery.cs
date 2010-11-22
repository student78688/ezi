using System;
namespace SearchEngine
{
	public class SearchQuery : SearchElement
	{
		public SearchQuery (string header, string body) :
			base(header, body)
		{
		}
		
		public double[] TfIdf
		{
			get { return tfIdf; }
		}
		
		public double TfIdfWidth
		{
			get { return tfIdfWidth; }
		}
		
		public int[] BagOfWords
		{
			get { return bagOfWords; }
		}
	}
}

