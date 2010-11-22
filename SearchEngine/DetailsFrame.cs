using System;
namespace SearchEngine
{
	public class DetailsFrame : wx.Frame
	{
		protected wx.TextCtrl textTC;
		
		public DetailsFrame (wx.Window parent, string title, string text):
			base(parent, title)
		{
			textTC = new wx.TextCtrl(this, wx.Window.wxID_ANY, text, 
			                         wx.Window.wxDefaultPosition, wx.Window.wxDefaultSize,
			                         wx.WindowStyles.TE_MULTILINE);
			wx.BoxSizer topSizer = new wx.BoxSizer(wx.Orientation.wxVERTICAL);
			{
				wx.BoxSizer boxSizer = new wx.BoxSizer(wx.Orientation.wxVERTICAL);
				{
					boxSizer.Add(textTC, 1, wx.SizerFlag.wxEXPAND);
				}
				topSizer.Add(boxSizer, 1, wx.SizerFlag.wxEXPAND|wx.SizerFlag.wxALL, 5);
			}
			this.Sizer = topSizer ;
			topSizer.SetSizeHints(this);
			this.Size = new System.Drawing.Size(640, 480);
			this.CenterOnParent();
			
			EVT_CLOSE(new wx.EventListener(OnClose));
		}
		
		public string Text
		{
			set 
			{
				textTC.Value = value;
			}
		}
		
		protected void OnClose(object sender, wx.Event evt)
		{
			this.Hide();
		}
	}
}

