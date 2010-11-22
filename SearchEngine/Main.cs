using System;
using System.Drawing;

namespace SearchEngine
{
	
	class MainClass
	{
		[STAThread]
		public static void Main (string[] args)
		{
			SearchEngineApp searchApp = new SearchEngineApp();
			searchApp.Run();
		}
	}
	
	class SearchEngineApp : wx.App
	{
		public override bool OnInit ()
		{
			SearchFrame mainFrame = new SearchFrame(null, wx.Window.wxID_ANY, 
			                                        "Wyszukiwarka - Jacek Trubłajewicz, 78688",
			                                        new Size(800, 600),
			                                        wx.Window.wxDefaultPosition,
			                                        wx.WindowStyles.FRAME_DEFAULT_STYLE);
			mainFrame.CenterOnScreen();
			mainFrame.Show(true);
			
			return true;
		}
	}
}

