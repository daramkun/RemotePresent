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
using Microsoft.Xna.Framework.Input.Touch;
using System.Windows.Threading;

namespace RemoteMediaControl
{
	public partial class GesturePage : PhoneApplicationPage
	{
		DispatcherTimer tapTimer;

		void ReceivePacket ()
		{
			SocketAsyncEventArgs asyncEvent = new SocketAsyncEventArgs ();
			asyncEvent.SetBuffer ( ( asyncEvent.UserToken = new byte [ 32 ] ) as byte [], 0, 32 );
			asyncEvent.Completed += ( object sender, SocketAsyncEventArgs e ) =>
			{
				if ( e.SocketError != SocketError.Success )
				{
					Dispatcher.BeginInvoke ( () => { if ( NavigationService.CanGoBack )NavigationService.GoBack (); } );
					return;
				}

				Packet packet = PacketAnalyzer.AnalzingPacket ( e.UserToken as byte [],
					StaticClient.ClientSocket.RemoteEndPoint as IPEndPoint );
				if ( packet is CommandPacket )
				{
					CommandPacket cp = packet as CommandPacket;
				}

				ReceivePacket ();
			};
			StaticClient.ClientSocket.ReceiveAsync ( asyncEvent );
		}

		public GesturePage ()
		{
			InitializeComponent ();
			{
				lblNotice.Text = "명령을 대기 중입니다...";
			}

			ReceivePacket ();

			TouchPanel.EnabledGestures = GestureType.Tap | GestureType.DoubleTap |
				GestureType.Hold | GestureType.Flick;
			Touch.FrameReported += ( object sender, TouchFrameEventArgs e ) =>
			{
				while ( TouchPanel.IsGestureAvailable )
				{
					GestureSample sample = TouchPanel.ReadGesture ();
					CommandPacket packet = new CommandPacket ( DeviceType.WindowsPhone,
						RemoteControllerType.RemotePresent, CommandType.Unknown, 0, 0, 0 );
					string message = "";
					switch ( sample.GestureType )
					{
						case GestureType.DoubleTap:
							{
								if ( tapTimer != null )
								{
									tapTimer.Stop ();
									tapTimer = null;
								}
								MessageBoxResult r = MessageBox.Show ( "연결을 종료하시겠습니까?", "종료",
									MessageBoxButton.OKCancel );
								switch ( r )
								{
									case MessageBoxResult.OK:
										StaticClient.ClientSocket.Close ();
										StaticClient.ClientSocket = null;
										NavigationService.GoBack ();
										return;
								}
								continue;
							}
						case GestureType.Tap:
							if ( tapTimer != null ) break;
							tapTimer = new DispatcherTimer ();
							tapTimer.Interval = TimeSpan.FromMilliseconds ( 400 );
							tapTimer.Tick += ( object s, EventArgs ee ) =>
							{
								packet.CommandType = CommandType.MultimediaPlayPause;
								message = "미디어 재생/일시정지";
								Dispatcher.BeginInvoke ( () => { lblNotice.Text = message; } );
								tapTimer.Stop ();
								tapTimer = null;

								byte [] ddata = PacketAnalyzer.PackingPacket ( packet );
								SocketAsyncEventArgs aasyncEvent = new SocketAsyncEventArgs ();
								aasyncEvent.SetBuffer ( ddata, 0, 32 );
								aasyncEvent.UserToken = message;
								aasyncEvent.Completed += ( object ss, SocketAsyncEventArgs a ) =>
								{
									if ( a.SocketError == SocketError.Success )
										Dispatcher.BeginInvoke ( () => { lblNotice.Text = a.UserToken as string; } );
									else
										Dispatcher.BeginInvoke ( () => { NavigationService.GoBack (); } );
								};
								StaticClient.ClientSocket.SendAsync ( aasyncEvent );
							};
							tapTimer.Start ();
							continue;
						case GestureType.Hold:
							packet.CommandType = CommandType.MultimediaStop;
							message = "미디어 정지";
							break;
						case GestureType.Flick:
							if ( sample.Delta.X < sample.Position.X && ( Math.Abs ( sample.Position.X - sample.Delta.X ) > Math.Abs ( sample.Position.Y - sample.Delta.Y ) ) )
							{
								packet.CommandType = CommandType.MultimediaPrev;
								message = "이전 미디어";
							}
							else if ( sample.Delta.X > sample.Position.X && ( Math.Abs ( sample.Position.X - sample.Delta.X ) > Math.Abs ( sample.Position.Y - sample.Delta.Y ) ) )
							{
								packet.CommandType = CommandType.MultimediaNext;
								message = "다음 미디어";
							}
							else if ( sample.Delta.Y < sample.Position.Y && ( Math.Abs ( sample.Position.X - sample.Delta.X ) < Math.Abs ( sample.Position.Y - sample.Delta.Y ) ) )
							{
								packet.CommandType = CommandType.SoundUp;
								message = "볼륨 상향 조절";
							}
							else if ( sample.Delta.Y > sample.Position.Y && ( Math.Abs ( sample.Position.X - sample.Delta.X ) < Math.Abs ( sample.Position.Y - sample.Delta.Y ) ) )
							{
								packet.CommandType = CommandType.SoundDown;
								message = "볼륨 하향 조절";
							}
							break;
						default:
							continue;
					}
					byte [] data = PacketAnalyzer.PackingPacket ( packet );
					SocketAsyncEventArgs asyncEvent = new SocketAsyncEventArgs ();
					asyncEvent.SetBuffer ( data, 0, 32 );
					asyncEvent.UserToken = message;
					asyncEvent.Completed += ( object s, SocketAsyncEventArgs a ) =>
					{
						if ( a.SocketError == SocketError.Success )
							Dispatcher.BeginInvoke ( () => { lblNotice.Text = a.UserToken as string; } );
						else
							Dispatcher.BeginInvoke ( () => { NavigationService.GoBack (); } );
					};
					if(StaticClient.ClientSocket != null)
						StaticClient.ClientSocket.SendAsync ( asyncEvent );
				}
			};
		}

		protected override void OnBackKeyPress ( System.ComponentModel.CancelEventArgs e )
		{
			if ( MessageBox.Show ( "연결을 종료하시려면 확인 버튼을 눌러주세요.",
				"안내", MessageBoxButton.OKCancel ) == MessageBoxResult.Cancel )
				e.Cancel = true;
			base.OnBackKeyPress ( e );
		}

		private void PhoneApplicationPage_Loaded ( object sender, RoutedEventArgs e )
		{
			PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
			phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Disabled;
		}

		private void PhoneApplicationPage_Unloaded ( object sender, RoutedEventArgs e )
		{
			PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
			phoneAppService.UserIdleDetectionMode = IdleDetectionMode.Enabled;
		}
	}
}