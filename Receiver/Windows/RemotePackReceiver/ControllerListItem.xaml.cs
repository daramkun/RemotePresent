using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;

namespace RemotePresentReceiver
{
	/// <summary>
	/// RemoDListItem.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ControllerListItem : UserControl
	{
		IPAddress deviceIP;
		int devicePort;

		int lastTickCount;

		public new object Content { get { return label1.Content; } set { label1.Content = value; } }
		public object Platform
		{
			get { return label2.Content; }
			set
			{
				label2.Content = value;
				switch ( label2.Content as string )
				{
					case "iOS":
						imgPlatform.Source = new BitmapImage ( new Uri ( "/Resources/ios.png", UriKind.RelativeOrAbsolute ) );
						break;
					case "Android":
						imgPlatform.Source = new BitmapImage ( new Uri ( "/Resources/android.png", UriKind.RelativeOrAbsolute ) );
						break;
					case "bada":
						imgPlatform.Source = new BitmapImage ( new Uri ( "/Resources/bada.png", UriKind.RelativeOrAbsolute ) );
						break;
					case "WindowsPhone":
						imgPlatform.Source = new BitmapImage ( new Uri ( "/Resources/wp.png", UriKind.RelativeOrAbsolute ) );
						break;
				}
			}
		}
		public IPAddress DeviceIP { get { return deviceIP; } set { deviceIP = value; } }
		public int DevicePort { get { return devicePort; } set { devicePort = value; } }
		public int LastTickCount { get { return lastTickCount; } set { lastTickCount = value; } }
		public string ControllerType
		{
			get { return null; }
			set { label3.Content = value; }
		}

		public ControllerListItem ()
		{
			InitializeComponent ();
		}
	}
}
