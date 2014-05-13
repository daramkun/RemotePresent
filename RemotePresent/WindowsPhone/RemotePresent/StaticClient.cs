using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Net.Sockets;

namespace RemotePresent
{
	public static class StaticClient
	{
		static Socket clientSocket;

		public static Socket ClientSocket
		{
			get { return clientSocket; }
			set { clientSocket = value; }
		}
	}
}
