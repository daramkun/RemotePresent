using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Reflection;
using System.IO;
using System.Text;

namespace RemotePresent
{
	public partial class HelpPage : PhoneApplicationPage
	{
		string ConvertExtendedASCII ( string HTML )
		{
			string retVal = "";
			char [] s = HTML.ToCharArray ();

			foreach ( char c in s )
			{
				if ( Convert.ToInt32 ( c ) > 127 )
					retVal += "&#" + Convert.ToInt32 ( c ) + ";";
				else
					retVal += c;
			}

			return retVal;
		}

		string GetPage ( string file )
		{
			Stream stream = Assembly.GetExecutingAssembly ()
				 .GetManifestResourceStream ( "RemotePresent." + file );
			string page;
			using ( StreamReader sr = new StreamReader ( stream, Encoding.UTF8 ) )
				page = sr.ReadToEnd ();
			return ConvertExtendedASCII ( page );
		}

		public HelpPage ()
		{
			InitializeComponent ();

			connectionBrowser.NavigateToString ( GetPage ( "Help_Connection.html" ) );
			commandBrowser.NavigateToString ( GetPage ( "Help_Commands.html" ) );
		}
	}
}