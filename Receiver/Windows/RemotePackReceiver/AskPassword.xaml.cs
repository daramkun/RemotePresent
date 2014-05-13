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

namespace RemotePresentReceiver
{
	/// <summary>
	/// AskPassword.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class AskPassword : Window
	{
		string password;

		public string Password { get { return password; } }

		public AskPassword ()
		{
			InitializeComponent ();
		}

		private void btnConnect_Click ( object sender, RoutedEventArgs e )
		{
			password = pwBox.Password;
			DialogResult = true;
		}

		private void Window_Loaded ( object sender, RoutedEventArgs e )
		{
			pwBox.Focus ();
		}
	}
}
