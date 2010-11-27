using System;
using System.Collections.Generic;

namespace SearchEngine
{
	public class CentroidGroup : IComparable<CentroidGroup>
	{
		// srednia reprezentacja termow w TF-TDF dla centroidu
		protected double[] centroidTfIdf;
		// dlugosc wektora sredniej reprezentacji termow
		protected double centroidTfIdfWidth;
		// podobienstwo centroidu do kazdego z dokumentow kolekcji
		protected Dictionary<SearchDocument, double> centroidSimilarity;
		// lista wszystkich dokumentow kolekcji
		protected List<SearchDocument> allDocuments;
		// dokumenty nalezace do grupy centroidu
		protected List<SearchDocument> centroidGroup;
		// dokumenty poprzednio nalezace do grupy centroidu
		protected List<SearchDocument> oldCentroidGroup;
		
		public CentroidGroup (SearchDocument root, List<SearchDocument> allDocuments)             
		{
			this.centroidTfIdf = root.TfIdf;
			this.centroidTfIdfWidth = root.TfIdfWidth;
			this.centroidSimilarity = new Dictionary<SearchDocument, double>();
			this.allDocuments = allDocuments;	
			this.centroidGroup = new List<SearchDocument>();
			
			// wyliczenie podobienstwa centroidu do wszystkich dokumentow kolekcji
			for (int i = 0; i < allDocuments.Count; i++)
			{
				double sim = 0;
				for (int j = 0; j < centroidTfIdf.Length; j++)
				{
					sim += centroidTfIdf[j] * allDocuments[i].TfIdf[j];
				}
				sim /= (centroidTfIdfWidth*allDocuments[i].TfIdfWidth);
				centroidSimilarity.Add(allDocuments[i], sim);
			}
		}
		
		public void CalcuateCentroidToDocumentsSimilarity()
		{
			double termTfIdf;
			centroidTfIdfWidth = 0;
			
			#region wyliczenie sredniej reprezentacji termow dla centroidu
			// iteracja po wszystkich termach
			for (int i=0; i < centroidTfIdf.Length; i++)
			{
				termTfIdf = 0;
				// iteracja po wszytkich dokumentach w grupie
				for (int j = 0; j < centroidGroup.Count; j++)
				{
					termTfIdf += centroidGroup[j].TfIdf[i];
				}
				centroidTfIdf[i] = termTfIdf / centroidGroup.Count;	
				centroidTfIdfWidth += Math.Pow(centroidTfIdf[i], 2.0);
			}
			centroidTfIdfWidth = Math.Sqrt(centroidTfIdfWidth);
			#endregion
			
			#region wyliczenie podobienstwa dokumentow do centroidu
			for (int i = 0; i < allDocuments.Count; i++)
			{
				double sim = 0;
				for (int j = 0; j < centroidTfIdf.Length; j++)
				{
					sim += centroidTfIdf[j] * allDocuments[i].TfIdf[j];
				}
				sim /= (centroidTfIdfWidth*allDocuments[i].TfIdfWidth);
				centroidSimilarity[allDocuments[i]] = sim;
			}
			#endregion
		}
		
		public void ResetGroup()
		{
			oldCentroidGroup = centroidGroup;
			centroidGroup = new List<SearchDocument>();
		}
		
		public void AddToGroup(SearchDocument document)
		{
			if (!centroidGroup.Contains(document))
				centroidGroup.Add(document);
		}
		
		public List<SearchDocument> GroupDocuments
		{
			get { return centroidGroup; }
		}
		
		public List<SearchDocument> OldGroupDocuments
		{
			get { return oldCentroidGroup; }
		}
		
		public double GetDocumentSimilarity(SearchDocument doc)
		{
			return centroidSimilarity[doc];
		}
		
		public double GroupQuerySimilarity
		{
			get 
			{
				double sum = 0;
				for (int i = 0; i < centroidGroup.Count; i++)
					sum += centroidGroup[i].TfIdfSim;
				
				return sum;
			}
		}
		
		public int CompareTo (CentroidGroup other)
		{
			return this.GroupQuerySimilarity.CompareTo(other.GroupQuerySimilarity);
		}
	}
}

