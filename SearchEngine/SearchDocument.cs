using System;
namespace SearchEngine
{
	public class SearchDocument : SearchElement, IComparable<SearchDocument>
	{
		// przechowywane do sprawdzania czy zapytanie się nie powtarza
		protected int[] lastQueryBagOfWords;
		protected double tfIdfSim;
		protected string group;
		
		
		public SearchDocument (string header, string body, string group) :
			base(header, body)
		{
			this.group = group;
		}
		
		
		public double CalculateTfIdfSimilarity(SearchQuery query)
		{
			if (query.BagOfWords.Length != this.bagOfWords.Length)
				throw new ArgumentException("Przekazane zapytanie ma nieprawidłową długość reprezentacji bag-of-words");
			
			if (this.lastQueryBagOfWords == query.BagOfWords)
				return tfIdfSim;
			this.lastQueryBagOfWords = query.BagOfWords;
			
			double tfIdfMul = 0;
			double tfIdfWidthMul = this.tfIdfWidth * query.TfIdfWidth;
			if (tfIdfWidthMul == 0)
			{
				tfIdfSim = 0;
				return tfIdfSim;
			}
						
			for (int i = 0; i < this.tfIdf.Length; i++)
			{
				tfIdfMul += this.tfIdf[i] * query.TfIdf[i];	
			}
			
			tfIdfSim = tfIdfMul / (this.tfIdfWidth * query.TfIdfWidth);

			return tfIdfSim;
		}
		
		public int CompareTo (SearchDocument other)
		{
			return this.tfIdfSim.CompareTo(other.tfIdfSim);
		}
		
		public double TfIdfSim
		{
			get { return tfIdfSim; }
		}
		
		public double[] TfIdf
		{
			get { return tfIdf; }
		}
		
		public double TfIdfWidth
		{
			get { return tfIdfWidth; }
		}
		
		public string Group
		{
			get { return group; }
		}
		
	}
}

