using System;
using System.Collections.Generic;

namespace SearchEngine
{
	public class GroupEngine
	{
		// k centroidow i zwiazanych z nimi grupami dokumentow
		protected List<CentroidGroup> centroids;
		// liczba grup
		protected int k;
		// liczba iteracji grupowania
		protected int iter;
		List<SearchDocument> documents;
		
		public GroupEngine (List<SearchDocument> documents, int k, int iter)
		{
			this.k = k;
			this.iter = iter;
			this.documents = documents;
		}
		
		public List<CentroidGroup> GroupDocuments()
		{
			centroids = new List<CentroidGroup>();
			#region losowanie k dokumentow stanowiacych poczatkowe centroidy
			Random rand = new Random();
			List<SearchDocument> selectedDocs = new List<SearchDocument>();
			int x = 0;
			while (x < k)
			{
				int sel = rand.Next(documents.Count-1);
				if (selectedDocs.Contains(documents[sel]))
					continue;
				selectedDocs.Add(documents[sel]);
				
				CentroidGroup centroid = new CentroidGroup(documents[sel], documents);
				centroids.Add(centroid);				
				x++;
			}
			#endregion
			
			#region grupowanie dokumentów
			// kolejne iteracje grupowania
			for (int i = 0; i < iter; i++)
			{
				// wyczyszczenie przydzialu do grup
				for(int centr = 0; centr < centroids.Count; centr++)
					centroids[centr].ResetGroup();
							
				// iteracja po wszystkich dokumentach kolekcji
				for (int doc = 0; doc < documents.Count; doc++)
				{
					SearchDocument currentDoc = documents[doc];
					// indeks najlepiej pasujacego centroidu
					int best = 0;
					// iteracja po centroidach
					for (int centr = 1; centr < centroids.Count; centr++)
					{
						if (centroids[centr].GetDocumentSimilarity(currentDoc) >
						    					centroids[best].GetDocumentSimilarity(currentDoc))
						{
							best = centr;
						}	
					}
					// przydzielenie dokumentu do najbardziej odpowiadajcej grupy
					centroids[best].AddToGroup(currentDoc);					
				}
				
				// sprawdzenie czy byla zmiana w przydziale do grup
				bool change = false;
				// iteracja po centroidach
				for (int centr = 0; centr < centroids.Count; centr++)
				{
					CentroidGroup currentCentroid = centroids[centr];
					//  sprawdzenie rozmiarow kolecji dokumentow
					if (currentCentroid.OldGroupDocuments.Count != currentCentroid.GroupDocuments.Count)
					{
						change = true;
						break; // wyjscie z petli sprawdzania zmiany
					}
					
					// jezeli rozmiary sa takie same sprawdzamy kolejno dokumenty w grupie
					for (int doc = 0; doc < currentCentroid.GroupDocuments.Count; doc++)
					{
						if (!currentCentroid.OldGroupDocuments.Contains(currentCentroid.GroupDocuments[doc]))
						{
							change = true;
							break; // wyjscie z petli przegladania kolejnych dokumentow
						}
					}
					
					if (change == false)
						break; // wyjscie z petli sprawdzania zmiany
				}
				
				if (change == false)
					return centroids;	// wyjscie z petli grupowania
			}
			#endregion
			
			return centroids;
		}
		
		public int K
		{
			get { return k; }
			set { k = value < 1 ? 1 : value;	}
		}
		
		public int Iter
		{
			get { return iter; }
			set { iter = value < 0 ? 0 : value; }
		}
	}
}

