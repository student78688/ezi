using System;
using System.Drawing;
using System.Collections.Generic;

namespace SearchEngine
{
	public class SearchFrame : wx.Frame
	{
		enum Id
		{
			termsTC,
			documentsTC,
			searchTC,
			termsBtn,
			documentsBtn,
			searchBtn,
			resultsLC,
			kSC,
			iterSC,
			menuBar,
			quitFileMenu,
			aboutHelpMenu,
			showTermsOptionsMenu,
			showDocumentsOptionsMenu,
			showZerosOptionsMenu			
		}

		protected wx.TextCtrl termsTC;
		protected wx.TextCtrl documentsTC;
		protected wx.TextCtrl searchTC;
		protected wx.Button termsBtn;
		protected wx.Button documentsBtn;
		protected wx.Button searchBtn;
		protected wx.ListCtrl resultsLC;
		protected wx.SpinCtrl kSC;
		protected wx.SpinCtrl iterSC;
		protected wx.MenuBar menuBar;
		protected wx.Menu fileMenu;
		protected wx.Menu helpMenu;
		protected wx.Menu optionsMenu;
		protected wx.MenuItem showZerosMenuCheck;
		protected DetailsFrame detailsDocumentsFrame;
		protected DetailsFrame detailsTermsFrame;
		
		protected TfIdfCalc searcher;
		protected GroupEngine grouper;

		public SearchFrame (wx.Frame parent, int id, string title, Size size, Point pos, wx.WindowStyles style) : base(parent, id, title, pos, size, style)
		{
			this.DoControls ();
			this.DoControlsProperties ();
			this.DoLayout ();
			
			this.SetSize (new Size (640, 480));
			
			EVT_MENU ((int)Id.quitFileMenu, new wx.EventListener (OnQuitMenuClick));
			EVT_MENU ((int) Id.aboutHelpMenu, new wx.EventListener(OnAboutMenu));
			EVT_MENU((int) Id.showTermsOptionsMenu, new wx.EventListener(OnShowTermsMenu));
			EVT_MENU((int) Id.showDocumentsOptionsMenu, new wx.EventListener(OnShowDocumentsMenu));
			EVT_BUTTON ((int)Id.termsBtn, new wx.EventListener (OnTermsBtnClick));
			EVT_BUTTON ((int)Id.documentsBtn, new wx.EventListener (OnDocumentsBtnClick));
			EVT_BUTTON((int) Id.searchBtn, new wx.EventListener(OnSearchBtnClick));
			EVT_SIZE(new wx.EventListener(OnSizeChanged));
			EVT_SPINCTRL((int) Id.kSC, new wx.EventListener(OnKSpinChanged));
			EVT_SPINCTRL((int) Id.iterSC, new wx.EventListener(OnIterSpinChanged));
						
			searcher = new TfIdfCalc(new StandardTextProcessor(new PorterStemmerAlgorithm.Stemmer2()));
						
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				this.BackgroundColour = wx.SystemSettings.GetColour(wx.SystemColour.wxSYS_COLOUR_BTNFACE);	
		}

		protected void DoControls ()
		{
			termsTC = new wx.TextCtrl (this, (int)Id.termsTC, string.Empty, wx.Window.wxDefaultPosition, wx.Window.wxDefaultSize, wx.WindowStyles.TE_READONLY);
			documentsTC = new wx.TextCtrl (this, (int)Id.documentsTC, string.Empty, wx.Window.wxDefaultPosition, wx.Window.wxDefaultSize, wx.WindowStyles.TE_READONLY);
			searchTC = new wx.TextCtrl (this, (int)Id.searchTC);
			termsBtn = new wx.Button (this, (int)Id.termsBtn, "Wczytaj termy");
			documentsBtn = new wx.Button (this, (int)Id.documentsBtn, "Wczytaj dokumenty");
			searchBtn = new wx.Button (this, (int)Id.searchBtn, "Szukaj");
			resultsLC = new wx.ListCtrl (this, (int)Id.resultsLC);
			kSC = new wx.SpinCtrl(this, (int)Id.kSC, "2");
			iterSC = new wx.SpinCtrl(this, (int)Id.iterSC, "5");
			
			menuBar = new wx.MenuBar ();
			this.MenuBar = menuBar;
			fileMenu = new wx.Menu ();
			fileMenu.Append ((int)Id.quitFileMenu, "Zakończ");
			helpMenu = new wx.Menu ();
			helpMenu.Append ((int)Id.aboutHelpMenu, "O programie");
			optionsMenu = new wx.Menu();
			optionsMenu.Append((int)Id.showDocumentsOptionsMenu, "Pokaż prztworzone dokumenty");
			optionsMenu.Append((int)Id.showTermsOptionsMenu, "Pokaż prztworzone termy");
			showZerosMenuCheck = optionsMenu.AppendCheckItem((int)Id.showZerosOptionsMenu, "Wyświetlaj dokumenty z zerową miarą");
			menuBar.Append (fileMenu, "Plik");
			menuBar.Append(optionsMenu, "Opcje");
			menuBar.Append (helpMenu, "Pomoc");
			
			this.CreateStatusBar (1, wx.WindowStyles.SB_RAISED);
			this.StatusText = "Wyszukiwarka...";
			
		}

		private const int similarityColumnSize = 60;

		protected void DoControlsProperties ()
		{
			termsTC.Enabled = false;
			documentsTC.Enabled = false;
			searchBtn.SetDefault ();
			resultsLC.MinSize = new Size (280, 170);
			resultsLC.StyleFlags = wx.WindowStyles.LC_REPORT;
			resultsLC.InsertColumn (0, "Dokument");
			resultsLC.InsertColumn (1, "Grupa");
			resultsLC.InsertColumn (2, "Miara");
			resultsLC.SetColumnWidth(2, similarityColumnSize);
//			kSC.set
//			iterSC.Min = 1;
		}

		protected void DoLayout ()
		{
			wx.BoxSizer topSizer = new wx.BoxSizer (wx.Orientation.wxVERTICAL);
			{
				wx.BoxSizer boxSizer = new wx.BoxSizer (wx.Orientation.wxVERTICAL);
				{
					wx.BoxSizer detailsSizer = new wx.BoxSizer(wx.Orientation.wxHORIZONTAL);
					{
						wx.StaticBoxSizer dataSizer = new wx.StaticBoxSizer (new wx.StaticBox (this, "Dane"), wx.Orientation.wxHORIZONTAL);
						{
							wx.FlexGridSizer gridSizer = new wx.FlexGridSizer (2, 2, 5, 10);
							{
								gridSizer.Add (documentsTC, 0, wx.SizerFlag.wxEXPAND);
								gridSizer.Add (documentsBtn, 0, wx.SizerFlag.wxNo_FLAG);
								gridSizer.Add (termsTC, 0, wx.SizerFlag.wxEXPAND);
								gridSizer.Add (termsBtn, 0, wx.SizerFlag.wxEXPAND);
								
							}
							gridSizer.AddGrowableCol (0);
							dataSizer.Add (gridSizer, 1, wx.SizerFlag.wxALL | wx.SizerFlag.wxEXPAND, 5);
						}
						detailsSizer.Add (dataSizer, 1, wx.SizerFlag.wxEXPAND);
						
						detailsSizer.Add(5, 10, 0, wx.SizerFlag.wxNo_FLAG, 0);
						
						wx.StaticBoxSizer configSizer = new wx.StaticBoxSizer(new wx.StaticBox(this, "Konfiguracja"),
						                                                      wx.Orientation.wxHORIZONTAL);
						{
							wx.FlexGridSizer gridSizer2 = new wx.FlexGridSizer(2, 2, 5, 5);
							{
								gridSizer2.Add(new wx.StaticText(this, "k: "), 0, 
								               wx.SizerFlag.wxALIGN_CENTRE_VERTICAL|wx.SizerFlag.wxALIGN_RIGHT);
								gridSizer2.Add(kSC, 0, wx.SizerFlag.wxNo_FLAG);
								gridSizer2.Add(new wx.StaticText(this, "iteracje: "), 0, 
								               wx.SizerFlag.wxALIGN_CENTRE_VERTICAL|wx.SizerFlag.wxALIGN_RIGHT);
								gridSizer2.Add(iterSC, 0, wx.SizerFlag.wxNo_FLAG);
							}
							configSizer.Add(gridSizer2, 0, wx.SizerFlag.wxALL, 5);
						}
						detailsSizer.Add(configSizer, 0, wx.SizerFlag.wxEXPAND, 0);
						
					}
					boxSizer.Add (detailsSizer, 0, wx.SizerFlag.wxEXPAND);
									
					boxSizer.Add (20, 5, 0, wx.SizerFlag.wxEXPAND, 0);
					
					wx.StaticBoxSizer searchSizer = new wx.StaticBoxSizer (new wx.StaticBox (this, "Wyszukiwanie"), wx.Orientation.wxVERTICAL);
					{
						wx.BoxSizer inputSizer = new wx.BoxSizer (wx.Orientation.wxHORIZONTAL);
						{
							inputSizer.Add (searchTC, 1, wx.SizerFlag.wxEXPAND);
							inputSizer.Add (10, 20, 0, wx.SizerFlag.wxEXPAND, 0);
							inputSizer.Add (searchBtn, 0, wx.SizerFlag.wxNo_FLAG);
						}
						searchSizer.Add (inputSizer, 0, wx.SizerFlag.wxALL | wx.SizerFlag.wxEXPAND, 5);
						
						searchSizer.Add (resultsLC, 1, wx.SizerFlag.wxEXPAND | wx.SizerFlag.wxALL, 5);
					}
					boxSizer.Add (searchSizer, 1, wx.SizerFlag.wxEXPAND);
					
				}
				topSizer.Add (boxSizer, 1, wx.SizerFlag.wxALL | wx.SizerFlag.wxEXPAND, 5);
			}
			topSizer.SetSizeHints (this);
			this.SetSizer (topSizer);
			
		}

		protected void OnQuitMenuClick (object sender, wx.Event evt)
		{
			this.Close ();
		}

		protected void OnTermsBtnClick (object sender, wx.Event evt)
		{
			wx.FileDialog openDlg = new wx.FileDialog (this, "Otwórz plik", "data", "", "Pliki tekstowe *.txt|*.txt", wx.WindowStyles.FD_OPEN);
			
			if (openDlg.ShowModal() == wx.ShowModalResult.OK)
			{
				if (searcher.IsReady)
				{
					documentsTC.Value = string.Empty;
					searcher.ResetData();
				}
				
				termsTC.Value = openDlg.Path;	
				searcher.ReadTerms(openDlg.Path);
			}
			else
				return;
			
			this.PrepareToSearch();
		}

		protected void OnDocumentsBtnClick (object sender, wx.Event evt)
		{
			wx.FileDialog openDlg = new wx.FileDialog (this, "Otwórz plik", "data", "", "Pliki tekstowe *.txt|*.txt", wx.WindowStyles.FD_OPEN);
			
			if (openDlg.ShowModal() == wx.ShowModalResult.OK)
			{
				if (searcher.IsReady)
				{
					searcher.ResetData();
					termsTC.Value = string.Empty;
				}
				
				documentsTC.Value = openDlg.Path;	
				searcher.ReadDocuments(openDlg.Path);
			}
			else
				return;
			
			this.PrepareToSearch();
		}
		
		protected void PrepareToSearch()
		{
			if (!searcher.IsReady)
				return;
			
			try
			{
				searcher.CalucateTfIdfFactors();
			}
			catch (CalculationException ex)
			{
				wx.MessageDialog.ShowModal(ex.Message, "Błąd w obliczeniach", 
				                           wx.WindowStyles.ICON_ERROR|wx.WindowStyles.DIALOG_OK);
				searcher.ResetData();
				wx.MessageDialog.ShowModal("Wczytane dane zostały zresetowane. Wczytaj dane ponownie", "Informacja", 
				                           wx.WindowStyles.ICON_INFORMATION|wx.WindowStyles.DIALOG_OK);
				termsTC.Value = documentsTC.Value = string.Empty;
				return;
			}
			
			// utworzenie nowego silnika grupowania ;)
			grouper = new GroupEngine(searcher.Documents, this.kSC.Value, this.iterSC.Value);
		}
		
		protected void OnSearchBtnClick(object sender, wx.Event evt)
		{
			resultsLC.DeleteAllItems();
		
			if (!searcher.IsReady)
			{
				wx.MessageDialog.ShowModal("Wyszukiwarka nie jest gotowa, wpierw wczytaj dane", "Błąd", 
				                           wx.WindowStyles.ICON_ERROR|wx.WindowStyles.DIALOG_OK);
			}
							
			string query = searchTC.Value.Trim();
			if (query == string.Empty)
				return;
									
			List<SearchDocument> results = null; 
			try
			{
				results = searcher.PerformSearch(query);
			}
			catch (CalculationException ex)
			{
				wx.MessageDialog.ShowModal(ex.Message, "Błąd w obliczeniach", 
				                           wx.WindowStyles.ICON_ERROR|wx.WindowStyles.DIALOG_OK);
				return;
			}
			
			List<CentroidGroup> groups = grouper.GroupDocuments();
			// sortowanie dokumentow w grupach
			foreach (CentroidGroup group in groups)
			{
				group.GroupDocuments.Sort();
				group.GroupDocuments.Reverse();
			}
			// sortowanie grup pod wzgledem lacznego podobienstwa dokumentow w grupie do zapytania
			groups.Sort();
			groups.Reverse();
			
			
			// wyswietlenie pogrupowanych wynikow
			for (int i = 0; i < groups.Count; i++)
			{
				wx.ListItem[] row = new wx.ListItem[1];
				row[0] = new wx.ListItem("Grupa #"+i);
				resultsLC.AppendItemRow(row);
				
				for (int j = 0; j < groups[i].GroupDocuments.Count; j++)
				{
					if (!showZerosMenuCheck.Checked)
					{
						if (groups[i].GroupDocuments[j].TfIdfSim == 0)
							break;
					}
					row = new wx.ListItem[3];
					row[0] = new wx.ListItem(groups[i].GroupDocuments[j].Header, wx.ListColumnFormat.LEFT);
					row[1] = new wx.ListItem(groups[i].GroupDocuments[j].Group);
					row[2] = new wx.ListItem(groups[i].GroupDocuments[j].TfIdfSim.ToString());
					resultsLC.AppendItemRow(row);
				}
				
				// pusta linia
				row = new wx.ListItem[1];
				row[0] = new wx.ListItem(" ");
				resultsLC.AppendItemRow(row);
			}	
		}
		
		protected void OnSizeChanged(object sender, wx.Event evt)
		{
			resultsLC.SetColumnWidth(0, (int)(0.8 * (resultsLC.ClientRect.Width - similarityColumnSize)));
			resultsLC.SetColumnWidth(1, (int)(0.2 * (resultsLC.ClientRect.Width - similarityColumnSize)));
			resultsLC.SetColumnWidth(2, similarityColumnSize);
			evt.Skip();
		}
		
		protected void OnAboutMenu (object sender, wx.Event evt)
		{
			string msg = "Eksperymentalna wyszukiwarka v2\n\nJacek Trubłajewicz <gothic@os.pl>";
			wx.MessageDialog.MessageBox (msg, "O programie");
		}
		
		protected void OnShowTermsMenu(object sender, wx.Event evt)
		{
			if (detailsTermsFrame == null)
				detailsTermsFrame = new DetailsFrame(this, "Dokumenty", searcher.ProcessedTerms);
			else
				detailsTermsFrame.Text = searcher.ProcessedTerms;
			
			detailsTermsFrame.Show();
		}
		
		protected void OnShowDocumentsMenu(object sender, wx.Event evt)
		{
			if (detailsDocumentsFrame == null)
				detailsDocumentsFrame = new DetailsFrame(this, "Dokumenty", searcher.ProcessedDocuments);
			else
				detailsDocumentsFrame.Text = searcher.ProcessedDocuments;
			
			detailsDocumentsFrame.Show();
		}
		
		protected void OnKSpinChanged(object sender, wx.Event evt)
		{
			if (this.kSC.Value < 1)
				this.kSC.Value = 1;
			
			if (this.grouper != null)
				this.grouper.K = this.kSC.Value;
			
		}
		
		protected void OnIterSpinChanged(object sender, wx.Event evt)
		{
			if (this.iterSC.Value < 1)
				this.iterSC.Value = 1;
			
			if (this.grouper != null)
				this.grouper.Iter = this.iterSC.Value;
		}	
	}
	
}

