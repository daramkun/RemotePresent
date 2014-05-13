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
using System.Windows.Forms;

namespace RemotePresentReceiver
{
	/// <summary>
	/// OptionWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class OptionWindow : Window
	{
		System.Windows.Controls.Button gamepadButton = null;

		public OptionWindow ()
		{
			InitializeComponent ();

			elsScreenLaser.Fill = new SolidColorBrush ( Settings.Default.ScreenLaserColor );
			sldRed.Value = ( elsScreenLaser.Fill as SolidColorBrush ).Color.R;
			sldGreen.Value = ( elsScreenLaser.Fill as SolidColorBrush ).Color.G;
			sldBlue.Value = ( elsScreenLaser.Fill as SolidColorBrush ).Color.B;

			chkUseTelnetPort.IsChecked = Settings.Default.UseTelnetPort;
		}

		private void Window_Loaded ( object sender, RoutedEventArgs e )
		{
		}

		private void sldRed_ValueChanged ( object sender, RoutedPropertyChangedEventArgs<double> e )
		{
			if ( elsScreenLaser.Fill == null ) return;
			Color color = ( elsScreenLaser.Fill as SolidColorBrush ).Color;
			color.R = ( byte ) sldRed.Value;
			( elsScreenLaser.Fill as SolidColorBrush ).Color = color;
			Settings.Default.ScreenLaserColor = color;
			Settings.Default.Save ();
		}

		private void sldGreen_ValueChanged ( object sender, RoutedPropertyChangedEventArgs<double> e )
		{
			if ( elsScreenLaser.Fill == null ) return;
			Color color = ( elsScreenLaser.Fill as SolidColorBrush ).Color;
			color.G = ( byte ) sldGreen.Value;
			( elsScreenLaser.Fill as SolidColorBrush ).Color = color;
			Settings.Default.ScreenLaserColor = color;
			Settings.Default.Save ();
		}

		private void sldBlue_ValueChanged ( object sender, RoutedPropertyChangedEventArgs<double> e )
		{
			if ( elsScreenLaser.Fill == null ) return;
			Color color = ( elsScreenLaser.Fill as SolidColorBrush ).Color;
			color.B = ( byte ) sldBlue.Value;
			( elsScreenLaser.Fill as SolidColorBrush ).Color = color;
			Settings.Default.ScreenLaserColor = color;
			Settings.Default.Save ();
		}

		private void button7_Click ( object sender, RoutedEventArgs e )
		{
			gamepadButton = sender as System.Windows.Controls.Button;
		}

		private void button7_KeyUp ( object sender, System.Windows.Input.KeyEventArgs e )
		{
			Keys k = ( Keys ) KeyInterop.VirtualKeyFromKey ( e.Key );
			switch ( gamepadButton.Content as string )
			{
				case "▲":
					Settings.Default.GamePadUp = k;
					break;
				case "◀":
					Settings.Default.GamePadLeft = k;
					break;
				case "▶":
					Settings.Default.GamePadRight = k;
					break;
				case "▼":
					Settings.Default.GamePadDown = k;
					break;
				case "A":
					Settings.Default.GamePadA = k;
					break;
				case "B":
					Settings.Default.GamePadB = k;
					break;
				case "X":
					Settings.Default.GamePadX = k;
					break;
				case "Y":
					Settings.Default.GamePadY = k;
					break;
				case "Start":
					Settings.Default.GamePadStart = k;
					break;
				case "Select":
					Settings.Default.GamePadSelect = k;
					break;
				default:
					return;
			}
			Settings.Default.Save ();
		}

		private void chkUseTelnetPort_Checked ( object sender, RoutedEventArgs e )
		{
			Settings.Default.UseTelnetPort = chkUseTelnetPort.IsChecked ?? false;
			Settings.Default.Save ();
		}
	}
}
