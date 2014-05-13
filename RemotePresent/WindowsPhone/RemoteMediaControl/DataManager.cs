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
using System.IO.IsolatedStorage;
using System.IO;

namespace RemoteMediaControl
{
	public static class DataManager
	{
		static string deviceName, ip;

		public static string DeviceName
		{
			get { return deviceName; }
			set { deviceName = value; }
		}

		public static string IP
		{
			get { return ip; }
			set { ip = value; }
		}

		public static bool LoadDatas ()
		{
			try
			{
				using ( IsolatedStorageFileStream fs = new IsolatedStorageFileStream ( "remoteMediaControlConfigures.cfg",
					FileMode.Open, IsolatedStorageFile.GetUserStoreForApplication () ) )
				{
					BinaryReader br = new BinaryReader ( fs );
					deviceName = br.ReadString ();
					ip = br.ReadString ();
				}
			}
			catch { return false; }
			return true;
		}

		public static bool SaveDatas ()
		{
			try
			{
				using ( IsolatedStorageFileStream fs = new IsolatedStorageFileStream ( "remoteMediaControlConfigures.cfg",
					FileMode.Create, IsolatedStorageFile.GetUserStoreForApplication () ) )
				{
					BinaryWriter bw = new BinaryWriter ( fs );
					bw.Write ( deviceName );
					bw.Write ( ip );
				}
			}
			catch { return false; }
			return true;
		}
	}
}
