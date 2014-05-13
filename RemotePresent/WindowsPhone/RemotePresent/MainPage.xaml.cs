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
using Microsoft.Phone.Shell;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Marketplace;

namespace RemotePresent
{
	public partial class MainPage : PhoneApplicationPage
	{
		Socket broadcastSocket;

		// Communication Port
		readonly int Port = 25567;

		Thread sendThread, recvThread;
		bool isSendFirst = false;

		bool isThreadAlive = false;

		public MainPage ()
		{
			InitializeComponent ();

			if ( new LicenseInformation ().IsTrial () )
			{
				tgsRealSwipe.IsEnabled = false;
				tgsScreenLaser.IsEnabled = false;
			}

			BroadcastSocketInitialize ();

			sendThread = new Thread ( BroadcastTo );
			recvThread = new Thread ( BroadcastFrom );
		}

		private void BroadcastSocketInitialize ()
		{
			broadcastSocket = new Socket ( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
			broadcastSocket.SendBufferSize = 32;
			broadcastSocket.ReceiveBufferSize = 32;
		}

		private void MakeOtPassword ()
		{
			byte [] otpw = new byte [ 4 ];
			Random r = new Random ( Environment.TickCount );
			for ( int i = 0; i < 4; i++ )
			{
				switch ( r.Next ( 0, 3 ) )
				{
					case 0:
						otpw [ i ] = ( byte ) ( 'A' + r.Next ( 0, 26 ) );
						break;
					case 1:
						otpw [ i ] = ( byte ) ( 'a' + r.Next ( 0, 26 ) );
						break;
					case 2:
						otpw [ i ] = ( byte ) ( '0' + r.Next ( 0, 10 ) );
						break;
				}
			}
			txtPassword.Text = Encoding.UTF8.GetString ( otpw, 0, 4 );
		}

		private bool CheckNetworkState ()
		{
			if ( !NetworkInterface.GetIsNetworkAvailable () )
			{
				MessageBox.Show ( "네트워크가 연결되지 않아 브로드캐스트를 시작할 수 없었습니다. 네트워크 상태를 확인해주세요.",
					"RemotePresent", MessageBoxButton.OK );
				return false;
			}
			else
			{
				if ( NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
					NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet )
				{
					if ( MessageBox.Show ( "Wi-Fi 환경이 아닐 경우 브로드캐스트에 실패할 수 있습니다. 브로드캐스트를 계속 진행할까요?",
						"RemotePresent", MessageBoxButton.OKCancel ) == MessageBoxResult.No )
						return false;
				}
			}

			return true;
		}

		private void Broadcast ( bool state )
		{
			if ( state )
			{
				if ( isThreadAlive ) return;

				isThreadAlive = true;

				MakeOtPassword ();

				recvThread.Start ();
				sendThread.Start ();

				txtDeviceName.IsReadOnly = true;

				SystemTray.ProgressIndicator.Text = "브로드캐스트 중입니다...";
				SystemTray.ProgressIndicator.IsVisible = true;
			}
			else
			{
				if ( !isThreadAlive ) return;

				isThreadAlive = false;
				sendThread = new Thread ( BroadcastTo );
				recvThread = new Thread ( BroadcastFrom );

				broadcastSocket.Close ();
				BroadcastSocketInitialize ();

				isSendFirst = false;

				txtPassword.Text = "";

				txtDeviceName.IsReadOnly = false;

				SystemTray.ProgressIndicator.Text = "";
				SystemTray.ProgressIndicator.IsVisible = false;
			}
		}

		private void Connect ( bool state )
		{
			if ( state )
			{
				if ( txtIP.Text.Length < 7 )
				{
					MessageBox.Show ( "IP 주소가 잘못되었습니다." );
					return;
				}

				if ( isThreadAlive ) return;

				txtIP.IsReadOnly = true;

				StaticClient.ClientSocket = new Socket ( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
				SocketAsyncEventArgs connAsyncEvent = new SocketAsyncEventArgs ();
				connAsyncEvent.RemoteEndPoint = new IPEndPoint ( IPAddress.Parse ( txtIP.Text ), Port );
				connAsyncEvent.Completed += ( object ss, SocketAsyncEventArgs eee ) =>
				{
					if ( eee.SocketError == SocketError.Success )
					{
						Dispatcher.BeginInvoke ( () =>
						{
							NavigationService.Navigate ( new Uri ( "/GesturePage.xaml?ip=" + txtIP.Text + "&port" + Port.ToString (),
								UriKind.RelativeOrAbsolute ) );
							Connect ( false );
						} );
					}
					else
					{
						Dispatcher.BeginInvoke ( () =>
						{
							Connect ( false );
							MessageBox.Show ( "연결에 실패했습니다." );
						} );
					}
				};
				StaticClient.ClientSocket.ConnectAsync ( connAsyncEvent );

				SystemTray.ProgressIndicator.Text = "연결 중입니다...";
				SystemTray.ProgressIndicator.IsVisible = true;
			}
			else
			{
				txtIP.IsReadOnly = false;

				isSendFirst = false;

				SystemTray.ProgressIndicator.Text = "";
				SystemTray.ProgressIndicator.IsVisible = false;
			}
		}

		private void BroadcastTo ()
		{
			while ( isThreadAlive )
			{
				ManualResetEvent resetEvent = new ManualResetEvent ( false );
				FromControllerPacket messagePacket = null;
				resetEvent.Reset ();
				Dispatcher.BeginInvoke ( () =>
				{
					messagePacket = new FromControllerPacket ( DeviceType.WindowsPhone,
					   RemoteControllerType.RemotePresent, null, txtDeviceName.Text );
					resetEvent.Set ();
				} );
				resetEvent.WaitOne ();
				SocketAsyncEventArgs asyncEvent = new SocketAsyncEventArgs ();
				asyncEvent.SetBuffer ( PacketAnalyzer.PackingPacket ( messagePacket ), 0, 32 );
				asyncEvent.RemoteEndPoint = new IPEndPoint ( IPAddress.Broadcast, Port );
				asyncEvent.Completed += ( object a, SocketAsyncEventArgs b ) =>
				{
					if ( b.SocketError == SocketError.Success ) isSendFirst = true;
				};
				broadcastSocket.SendToAsync ( asyncEvent );

				Thread.Sleep ( 3000 );
			}
		}

		private void BroadcastFrom ()
		{
			while ( isThreadAlive )
			{
				bool isTransferEnd = false;
				if ( !isSendFirst ) continue;
				SocketAsyncEventArgs asyncEvent = new SocketAsyncEventArgs ();
				asyncEvent.SetBuffer ( new byte [ 32 ], 0, 32 );
				asyncEvent.RemoteEndPoint = new IPEndPoint ( IPAddress.Any, Port );
				asyncEvent.Completed += ( object a, SocketAsyncEventArgs b ) =>
				{
					if ( !isThreadAlive ) return;
					Packet packet = PacketAnalyzer.AnalzingPacket ( b.Buffer, b.RemoteEndPoint as IPEndPoint );
					if ( packet == null ) return;
					switch ( packet.PacketType )
					{
						case PacketType.IWantConnectToYou:
							{
								Dispatcher.BeginInvoke ( () =>
								{
									FromReceiverPacket p = packet as FromReceiverPacket;
									if ( p.DeviceName == txtDeviceName.Text )
									{
										if ( p.Password == txtPassword.Text )
										{
											StaticClient.ClientSocket = new Socket ( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
											SocketAsyncEventArgs connAsyncEvent = new SocketAsyncEventArgs ();
											connAsyncEvent.RemoteEndPoint = new IPEndPoint ( p.DeviceIP, p.DevicePort );
											connAsyncEvent.Completed += ( object ss, SocketAsyncEventArgs eee ) =>
											{
												if ( eee.SocketError == SocketError.Success )
												{
													Dispatcher.BeginInvoke ( () =>
													{
														Broadcast ( false );
														NavigationService.Navigate ( new Uri ( "/GesturePage.xaml?ip=" + p.DeviceIP.ToString () + "&port" + p.DevicePort,
															UriKind.RelativeOrAbsolute ) );
													} );
												}
												else
												{
													StaticClient.ClientSocket = null;
													Dispatcher.BeginInvoke ( () =>
													{
														MessageBox.Show ( "연결에 실패했습니다." );
														Broadcast ( false );
													} );
													
												}
											};
											StaticClient.ClientSocket.ConnectAsync ( connAsyncEvent );
										}
										else
										{
											Dispatcher.BeginInvoke ( () =>
											{
												MessageBox.Show ( "잘못된 패스워드입니다." );
												Broadcast ( false );
											} );
										}
									}
								} );
							}
							break;
					}
					isTransferEnd = true;
				};
				try
				{
					broadcastSocket.ReceiveFromAsync ( asyncEvent );
				}
				catch { }
				while ( !isTransferEnd && isThreadAlive ) Thread.Sleep ( 1 );
				Thread.Sleep ( 1 );
			}
		}

		private void mainPivot_SelectionChanged ( object sender, SelectionChangedEventArgs e )
		{
			int index = mainPivot.SelectedIndex;

			switch ( index )
			{
				case 0:
					ApplicationBar = Resources [ "broadcaster" ] as ApplicationBar;
					break;
				case 1:
					ApplicationBar = Resources [ "connector" ] as ApplicationBar;
					break;
				case 2:
					ApplicationBar = Resources [ "configure" ] as ApplicationBar;
					break;
			}
		}

		private void mnuHelp_Click ( object sender, EventArgs e )
		{
			NavigationService.Navigate ( new Uri ( "/HelpPage.xaml", UriKind.RelativeOrAbsolute ) );
		}

		private void btnBroadcast_Click ( object sender, EventArgs e )
		{
			if ( !CheckNetworkState () ) return;

			Broadcast ( true );
		}

		private void btnConnect_Click ( object sender, EventArgs e )
		{
			if ( !CheckNetworkState () ) return;

			Connect ( true );
		}

		private void tgsRealSwipe_Checked ( object sender, RoutedEventArgs e )
		{
			DataManager.IsRealSwipeMode = true;
		}

		private void tgsRealSwipe_Unchecked ( object sender, RoutedEventArgs e )
		{
			DataManager.IsRealSwipeMode = false;
		}

		private void tgsScreenLaser_Checked ( object sender, RoutedEventArgs e )
		{
			DataManager.UseScreenLaser = true;
		}

		private void tgsScreenLaser_Unchecked ( object sender, RoutedEventArgs e )
		{
			DataManager.UseScreenLaser = false;
		}

		private void btnBroadcastCancel_Click ( object sender, EventArgs e )
		{
			Broadcast ( false );
		}

		private void btnConnectCancel_Click ( object sender, EventArgs e )
		{
			Connect ( false );

			if ( StaticClient.ClientSocket != null )
			{
				StaticClient.ClientSocket.Close ();
				StaticClient.ClientSocket = null;
			}
		}

		private void PhoneApplicationPage_BackKeyPress ( object sender, System.ComponentModel.CancelEventArgs e )
		{
			DataManager.SaveDatas ();
			isThreadAlive = false;
			broadcastSocket.Close ();
		}

		private void PhoneApplicationPage_Loaded ( object sender, RoutedEventArgs e )
		{
			if ( DataManager.LoadDatas () )
			{
				txtDeviceName.Text = DataManager.DeviceName;
				txtIP.Text = DataManager.IP;

				tgsRealSwipe.IsChecked = DataManager.IsRealSwipeMode;
				tgsScreenLaser.IsChecked = DataManager.UseScreenLaser;
			}
		}

		private void txtDeviceName_TextChanged ( object sender, TextChangedEventArgs e )
		{
			DataManager.DeviceName = txtDeviceName.Text;
		}

		private void txtIP_TextChanged ( object sender, TextChangedEventArgs e )
		{
			DataManager.IP = txtIP.Text;
		}
	}
}