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
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using RemotePresentReceiver.Properties;
using System.Windows.Forms;

namespace RemotePresentReceiver
{
	/// <summary>b
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		[DllImport ( "user32.dll" )]
		static extern short keybd_event ( byte vk, byte bScan, int flag, int extra );

		// Socket for Controller Information Receive
		Socket receiveSocket = new Socket ( AddressFamily.InterNetwork, SocketType.Dgram, 
			ProtocolType.Udp );
		// Socket for Listening a client
		Socket listenSocket = new Socket ( AddressFamily.InterNetwork, SocketType.Stream,
			ProtocolType.Tcp );
		// Socket for Communication to Controller
		Socket clientSocket = null;

		Thread receiveThread, timeoutRemover;
		bool isClose = false;

		// Communication Port
		//readonly int Port = 25567;
		private int Port
		{
			get 
			{
				return ( Settings.Default.UseTelnetPort ) ? 23 : 25567;
			}
		}

		ScreenLaser screenLaser;

		public MainWindow ()
		{
			InitializeComponent ();
		}

		void SetNotice ( string notice )
		{
			Dispatcher.BeginInvoke ( new Action ( () => { lblNotice.Content = notice; } ) );
		}

		void PressKey ( Keys key, bool state )
		{
			keybd_event ( ( byte ) key, 0, ( state ) ? 0 : 2, 0 );
		}

		void PressKeyOnce ( Keys key )
		{
			keybd_event ( ( byte ) key, 0, 0, 0 );
			keybd_event ( ( byte ) key, 0, 2, 0 );
		}

		void StartAccept ()
		{
			listenSocket.BeginAccept ( ( IAsyncResult ar ) =>
			{
				clientSocket = ( ar.AsyncState as Socket ).EndAccept ( ar );
				Dispatcher.BeginInvoke ( new Action ( () =>
				{
					btnConnect.IsEnabled = lstControllers.IsEnabled = false;
					StartReceiveCommands ();
					lblNotice.Content = AppResources.stateConnected;
				} ) );
			}, listenSocket );
		}

		public void StartReceiveCommands ()
		{
			byte [] buffer = new byte [ 32 ];
			clientSocket.BeginReceive ( buffer, 0, 32, SocketFlags.None,
				( IAsyncResult ar ) =>
				{
					try
					{
						if ( clientSocket.EndReceive ( ar ) != 0 )
						{
							Packet packet = PacketAnalyzer.AnalzingPacket ( ar.AsyncState as byte [],
									   clientSocket.RemoteEndPoint as IPEndPoint );
							if ( packet.PacketType == PacketType.Command )
							{
								CommandPacket cp = packet as CommandPacket;
								switch ( cp.CommandType )
								{
									case CommandType.PresentationStart:
										PressKeyOnce ( Keys.F5 );
										break;
									case CommandType.SlideLeft:
										PressKeyOnce ( Keys.Left );
										break;
									case CommandType.SlideRight:
										PressKeyOnce ( Keys.Right );
										break;
									case CommandType.ScreenLaser:
										if ( cp.X == 1 )
										{
											Dispatcher.BeginInvoke ( new Action ( () =>
											{
												if ( screenLaser == null )
													screenLaser = new ScreenLaser ();
												screenLaser.Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2 - 7;
												screenLaser.Top = System.Windows.SystemParameters.PrimaryScreenHeight / 2 - 7;
												screenLaser.EllipseColor = Settings.Default.ScreenLaserColor;
												screenLaser.Show ();
											} ) );
										}
										else
										{
											Dispatcher.BeginInvoke ( new Action ( () =>
											{
												screenLaser.Hide ();
											} ) );
										}
										break;
									case CommandType.ScreenLaserMove:
										{
											Dispatcher.BeginInvoke ( new Action ( () =>
											{
												screenLaser.Left += ( ( ( sbyte ) cp.X ) );
												screenLaser.Top += ( ( ( sbyte ) cp.Y ) );
												if ( screenLaser.Left < -7 ) screenLaser.Left = -7;
												if ( screenLaser.Top < -7 ) screenLaser.Top = -7;
												if ( screenLaser.Left > System.Windows.SystemParameters.PrimaryScreenWidth - 7 )
													screenLaser.Left = System.Windows.SystemParameters.PrimaryScreenWidth - 7;
												if ( screenLaser.Top > System.Windows.SystemParameters.PrimaryScreenHeight - 7 )
													screenLaser.Top = System.Windows.SystemParameters.PrimaryScreenHeight - 7;
											} ) );
										}
										break;

									case CommandType.MultimediaPrev:
										PressKeyOnce ( Keys.MediaPreviousTrack );
										break;
									case CommandType.MultimediaPlayPause:
										PressKeyOnce ( Keys.MediaPlayPause );
										break;
									case CommandType.MultimediaStop:
										PressKeyOnce ( Keys.MediaStop );
										break;
									case CommandType.MultimediaNext:
										PressKeyOnce ( Keys.MediaNextTrack );
										break;

									case CommandType.SoundDown:
										PressKeyOnce ( Keys.VolumeDown );
										break;
									case CommandType.SoundMute:
										PressKeyOnce ( Keys.VolumeMute );
										break;
									case CommandType.SoundUp:
										PressKeyOnce ( Keys.VolumeUp );
										break;

									case CommandType.GamePadJoystick:
										if ( cp.X == -1 ) PressKey ( Settings.Default.GamePadLeft, cp.Z == 1 );
										if ( cp.X == 1 ) PressKey ( Settings.Default.GamePadRight, cp.Z == 1 );
										if ( cp.Y == -1 ) PressKey ( Settings.Default.GamePadUp, cp.Z == 1 );
										if ( cp.Y == 1 ) PressKey ( Settings.Default.GamePadDown, cp.Z == 1 );
										break;
									case CommandType.GamePadA:
										PressKey ( Settings.Default.GamePadA, cp.Z == 1 );
										break;
									case CommandType.GamePadB:
										PressKey ( Settings.Default.GamePadB, cp.Z == 1 );
										break;
									case CommandType.GamePadX:
										PressKey ( Settings.Default.GamePadX, cp.Z == 1 );
										break;
									case CommandType.GamePadY:
										PressKey ( Settings.Default.GamePadY, cp.Z == 1 );
										break;
									case CommandType.GamePadStart:
										PressKey ( Settings.Default.GamePadStart, cp.Z == 1 );
										break;
									case CommandType.GamePadSelect:
										PressKey ( Settings.Default.GamePadSelect, cp.Z == 1 );
										break;
								}
							}
							StartReceiveCommands ();
						}
						else
						{
							clientSocket = null;
							Dispatcher.BeginInvoke ( new Action ( () =>
							{
								lstControllers.IsEnabled = btnConnect.IsEnabled = true;
								if ( screenLaser != null )
									screenLaser.Hide ();
								lblNotice.Content = AppResources.stateDisconnected;
							} ) );
							StartAccept ();
						}
					}
					catch (Exception ex)
					{
						if ( ex is SocketException )
						{
							clientSocket = null;
							Dispatcher.BeginInvoke ( new Action ( () =>
							{
								lstControllers.IsEnabled = btnConnect.IsEnabled = true;
								if ( screenLaser != null )
									screenLaser.Hide ();
								lblNotice.Content = AppResources.stateDisconnected;
							} ) );
							StartAccept ();
						}
					}
				}, buffer );
		}

		private void btnConnect_Click ( object sender, RoutedEventArgs e )
		{
			if ( lstControllers.SelectedItem != null )
			{
				ControllerListItem cli = lstControllers.SelectedItem as ControllerListItem;
				AskPassword pwAsk = new AskPassword ();
				if ( pwAsk.ShowDialog () == false ) return;
				string password = pwAsk.Password;
				Packet packet = new FromReceiverPacket ( DeviceType.Windows, RemoteControllerType.Receiver,
					new IPEndPoint ( IPAddress.Any, Port ), cli.Content as string, password );
				byte [] packetBuffer = PacketAnalyzer.PackingPacket ( packet );
				int a;
				if ( ( a = receiveSocket.SendTo ( packetBuffer, new IPEndPoint ( cli.DeviceIP, cli.DevicePort ) ) ) == -1 )
				{
					SetNotice ( AppResources.stateConnectionFailed );
				}
			}
			else
			{
				SetNotice ( AppResources.stateNotSelectedDevice );
			}
		}

		private void Window_Loaded ( object sender, RoutedEventArgs e )
		{
			receiveSocket.EnableBroadcast = true;
			receiveSocket.Bind ( new IPEndPoint ( IPAddress.Any, Port ) );

			receiveThread = new Thread ( () =>
			{
				while ( !isClose )
				{
					byte [] buffer = new byte [ 32 ];
					EndPoint endPoint = new IPEndPoint ( IPAddress.Broadcast, Port );
					receiveSocket.ReceiveFrom ( buffer, ref endPoint );

					Packet packet = PacketAnalyzer.AnalzingPacket ( buffer, endPoint as IPEndPoint );
					if ( packet is FromControllerPacket )
					{
						FromControllerPacket fcp = packet as FromControllerPacket;
						bool hasDevice = false;
						foreach ( ControllerListItem cli in lstControllers.Items )
						{
							if ( cli.DeviceIP.ToString () == fcp.DeviceIP.ToString () )
							{
								hasDevice = true;
								Dispatcher.BeginInvoke ( new Action ( () =>
								{
									//cli.Content = fcp.DeviceName;
									cli.ControllerType = fcp.ControllerType.ToString ();
									cli.LastTickCount = Environment.TickCount;
								} ) );
							}
						}
						if ( !hasDevice )
						{
							Dispatcher.BeginInvoke ( new Action ( () =>
								{
									ControllerListItem cli = new ControllerListItem ();
									cli.Content = fcp.DeviceName;
									cli.Platform = fcp.DeviceType.ToString ();
									cli.ControllerType = fcp.ControllerType.ToString ();
									cli.DeviceIP = fcp.DeviceIP;
									cli.DevicePort = fcp.DevicePort;
									cli.LastTickCount = Environment.TickCount;
									cli.MouseDoubleClick += ( object sd, MouseButtonEventArgs ee ) =>
									{
										btnConnect_Click ( btnConnect, new RoutedEventArgs () );
									};
									lock ( lstControllers )
									{
										lstControllers.Items.Add ( cli );
									}
								} ) );
						}
					}

					Thread.Sleep ( 1 );
				}
			} );
			receiveThread.Start ();

			timeoutRemover = new Thread ( () =>
			{
				while ( !isClose )
				{
					List<ControllerListItem> remover = new List<ControllerListItem> ();
					int tick = Environment.TickCount;
					foreach ( ControllerListItem cli in lstControllers.Items )
					{
						if ( tick - cli.LastTickCount >= 5000 )
						{
							remover.Add ( cli );
						}
					}

					Dispatcher.BeginInvoke ( new Action ( () =>
					{
						lock ( lstControllers.Items )
						{
							foreach ( ControllerListItem cli in remover )
								lstControllers.Items.Remove ( cli );
						}
					} ) );
					Thread.Sleep ( 1000 );
				}
			} );
			timeoutRemover.Start ();

			listenSocket.Bind ( new IPEndPoint ( IPAddress.Any, Port ) );
			listenSocket.Listen ( 1 );

			StartAccept ();
		}

		private void Window_Closing ( object sender, System.ComponentModel.CancelEventArgs e )
		{
			if ( screenLaser != null )
			{
				screenLaser.Close ();
				screenLaser = null;
			}
			if ( timeoutRemover != null )
			{
				timeoutRemover.Abort ();
				timeoutRemover = null;
			}
			if ( receiveThread != null )
			{
				isClose = true;
				receiveThread.Abort ();
				receiveThread = null;
				receiveSocket.Close ();
			}
		}

		private void lstControllers_KeyUp ( object sender, System.Windows.Input.KeyEventArgs e )
		{
			if ( lstControllers.SelectedItem != null )
				if ( e.Key == Key.Return )
					btnConnect_Click ( btnConnect, new RoutedEventArgs () );
		}

		private void mnuOptions_Click ( object sender, RoutedEventArgs e )
		{
			new OptionWindow ().ShowDialog ();
		}

		private void mnuExit_Click ( object sender, RoutedEventArgs e )
		{
			this.Close ();
		}

		private void mnuAbout_Click ( object sender, RoutedEventArgs e )
		{
			new AboutWindow ().ShowDialog ();
		}
	}
}
