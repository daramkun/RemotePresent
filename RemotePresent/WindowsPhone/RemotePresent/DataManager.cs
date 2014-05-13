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

namespace RemotePresent
{
	public static class DataManager
	{
		static bool isRealSwipe, useScreenLaser;
		static string deviceName, ip;

		public static bool IsRealSwipeMode
		{
			get { return isRealSwipe; }
			set { isRealSwipe = value; }
		}

		public static bool UseScreenLaser
		{
			get { return useScreenLaser; }
			set { useScreenLaser = value; }
		}

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
				using ( IsolatedStorageFileStream fs = new IsolatedStorageFileStream ( "remotePresentConfigures.cfg",
					FileMode.Open, IsolatedStorageFile.GetUserStoreForApplication () ) )
				{
					BinaryReader br = new BinaryReader ( fs );
					isRealSwipe = br.ReadBoolean ();
					useScreenLaser = br.ReadBoolean ();
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
				using ( IsolatedStorageFileStream fs = new IsolatedStorageFileStream ( "remotePresentConfigures.cfg",
					FileMode.Create, IsolatedStorageFile.GetUserStoreForApplication () ) )
				{
					BinaryWriter bw = new BinaryWriter ( fs );
					bw.Write ( isRealSwipe );
					bw.Write ( useScreenLaser );
					bw.Write ( deviceName );
					bw.Write ( ip );
				}
			}
			catch { return false; }
			return true;
		}
	}
}
