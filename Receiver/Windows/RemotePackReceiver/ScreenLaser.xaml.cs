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
using System.Windows.Shapes;
using RemotePresentReceiver.Properties;

namespace RemotePresentReceiver
{
	/// <summary>
	/// ScreenLaser.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class ScreenLaser : Window
	{
		public Color EllipseColor
		{
			get { return ( ellipse.Fill as SolidColorBrush ).Color; }
			set { ( ellipse.Fill as SolidColorBrush ).Color = value; }
		}

		public ScreenLaser ()
		{
			InitializeComponent ();
			ellipse.Fill = new SolidColorBrush ();
		}

		private void Window_Loaded ( object sender, RoutedEventArgs e )
		{
			( ellipse.Fill as SolidColorBrush ).Color = Settings.Default.ScreenLaserColor;
		}
	}
}
