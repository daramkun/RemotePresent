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
using System.Text;

namespace RemotePresent
{
	public enum PacketType
	{
		Unknown = 0,
		ItsMe = 1,
		IWantConnectToYou = 2,
		Command = 3,
	}

	public enum DeviceType
	{
		Unknown = 0,

		Windows = 1,
		Linux = 2,
		MacOSX = 3,

		iOS = 4,
		Android = 5,
		bada = 6,
		WindowsPhone = 7,
	}

	public enum CommandType
	{
		Unknown = 0,

		SlideLeft = 1,
		SlideRight = 2,
		ScreenLaser = 3,
		ScreenLaserMove = 4,
		PresentationStart = 5,

		MultimediaPrev = 6,
		MultimediaNext = 7,
		MultimediaPlayPause = 8,
		MultimediaStop = 9,
		SoundDown = 10,
		SoundMute = 11,
		SoundUp = 12,

		GamePadJoystick = 13,
		GamePadA = 14,
		GamePadB = 15,
		GamePadX = 16,
		GamePadY = 17,
		GamePadStart = 18,
		GamePadSelect = 19,
	}

	public enum RemoteControllerType
	{
		Unknown = 0,

		Receiver = 1,

		RemotePresent = 2,
		RemoteMediaControl = 3,
		RemoteGamePad = 4,
	}

	public interface Packet
	{
		PacketType PacketType { get; }
		DeviceType DeviceType { get; }
		RemoteControllerType ControllerType { get; }
	}

	public class UnknownPacket : Packet
	{
		public virtual PacketType PacketType { get { return PacketType.Unknown; } }
		protected DeviceType deviceType;
		protected RemoteControllerType controllerType;
		public DeviceType DeviceType { get { return deviceType; } }
		public RemoteControllerType ControllerType { get { return controllerType; } }

		internal UnknownPacket ( DeviceType dt, RemoteControllerType ct )
		{
			deviceType = dt;
			controllerType = ct;
		}
	}

	public class FromControllerPacket : UnknownPacket
	{
		public override PacketType PacketType { get { return PacketType.ItsMe; } }
		public string DeviceName { get { return deviceName; } set { deviceName = value; } }
		public IPAddress DeviceIP { get { return ep.Address; } }
		public int DevicePort { get { return ep.Port; } }

		string deviceName;
		IPEndPoint ep;

		internal FromControllerPacket ( DeviceType dt, RemoteControllerType ct, IPEndPoint ep, string deviceName )
			: base ( dt, ct )
		{
			this.deviceName = deviceName;
			this.ep = ep;
		}
	}

	public class FromReceiverPacket : UnknownPacket
	{
		public override PacketType PacketType { get { return PacketType.IWantConnectToYou; } }
		public string DeviceName { get { return deviceName; } set { deviceName = value; } }
		public string Password { get { return password; } set { password = value; } }
		public IPAddress DeviceIP { get { return ep.Address; } }
		public int DevicePort { get { return ep.Port; } }

		string deviceName;
		string password;
		IPEndPoint ep;

		internal FromReceiverPacket ( DeviceType dt, RemoteControllerType ct, IPEndPoint ep, string deviceName, string password )
			: base ( dt, ct )
		{
			this.deviceName = deviceName;
			this.password = password;
			this.ep = ep;
		}
	}

	public class CommandPacket : UnknownPacket
	{
		public override PacketType PacketType { get { return PacketType.Command; } }
		CommandType type;
		sbyte x, y, z;

		public CommandType CommandType { get { return type; } set { type = value; } }
		public sbyte X { get { return x; } set { x = value; } }
		public sbyte Y { get { return y; } set { y = value; } }
		public sbyte Z { get { return z; } set { z = value; } }

		internal CommandPacket ( DeviceType dt, RemoteControllerType ct, CommandType type,
			sbyte x, sbyte y, sbyte z )
			: base ( dt, ct )
		{
			this.type = type;
			this.x = x; this.y = y; this.z = z;
		}
	}

	/* =========================================================================== *
	 *                         = All of Packet Structures =
	 * [ff000000] 00000000 00000000 00000000 00000000 00000000 00000000 000000[ff]
	 *  - [0] and [31] must has 255(for founding errors).
	 *  - [1] is packet type. 0 : unknown, 1 : device info, 2 : conn request, 3 : command
	 *  - [2] is device type. 0 : unknown, 1 ~ 3 : perscom, 4 ~ 7 : mobile
	 *  - [3] is version.
	 * =========================================================================== */
	/// <summary>
	/// 
	/// </summary>
	public static class PacketAnalyzer
	{
		public static Packet AnalzingPacket ( byte [] buffer, IPEndPoint ep )
		{
			Packet packet = null;
			if ( buffer [ 0 ] == 255 && buffer [ 31 ] == 255 )
			{
				switch ( buffer [ 3 ] )
				{
					case 1:
						switch ( ( PacketType ) buffer [ 1 ] )
						{
							case PacketType.Unknown:
								packet = new UnknownPacket ( ( DeviceType ) buffer [ 2 ], ( RemoteControllerType ) buffer [ 30 ] );
								break;
							case PacketType.ItsMe:
								{
									string deviceName = Encoding.UTF8.GetString ( buffer, 4, 21 );
									deviceName = deviceName.Remove ( deviceName.IndexOf ( '\0' ) );
									packet = new FromControllerPacket ( ( DeviceType ) buffer [ 2 ], ( RemoteControllerType ) buffer [ 30 ], ep, deviceName );
								}
								break;
							case PacketType.IWantConnectToYou:
								{
									string deviceName = Encoding.UTF8.GetString ( buffer, 4, 21 );
									deviceName = deviceName.Remove ( deviceName.IndexOf ( '\0' ) );
									string password = Encoding.UTF8.GetString ( buffer, 25, 5 );
									password = password.Remove ( password.IndexOf ( '\0' ) );
									packet = new FromReceiverPacket ( ( DeviceType ) buffer [ 2 ], ( RemoteControllerType ) buffer [ 30 ], ep, deviceName, password );
								}
								break;
							case PacketType.Command:
								{
									packet = new CommandPacket ( ( DeviceType ) buffer [ 2 ],
										( RemoteControllerType ) buffer [ 30 ],
										( CommandType ) buffer [ 4 ],
										( sbyte ) buffer [ 5 ],
										( sbyte ) buffer [ 6 ],
										( sbyte ) buffer [ 7 ] );
								}
								break;
						}
						break;
				}
			}

			return packet;
		}

		public static byte [] PackingPacket ( Packet packet )
		{
			byte [] buffer = new byte [ 32 ];

			switch ( packet.PacketType )
			{
				case PacketType.ItsMe:
					{
						FromControllerPacket p = packet as FromControllerPacket;
						Encoding.UTF8.GetBytes ( p.DeviceName, 0, p.DeviceName.Length, buffer, 4 );
						buffer [ 24 ] = 0;
					}
					break;
				case PacketType.IWantConnectToYou:
					{
						FromReceiverPacket p = packet as FromReceiverPacket;
						Encoding.UTF8.GetBytes ( p.DeviceName, 0, p.DeviceName.Length, buffer, 4 );
						buffer [ 24 ] = 0;
						Encoding.UTF8.GetBytes ( p.Password, 0, p.Password.Length, buffer, 25 );
						buffer [ 29 ] = 0;
					}
					break;
				case PacketType.Command:
					{
						CommandPacket p = packet as CommandPacket;
						buffer [ 4 ] = ( byte ) p.CommandType;
						buffer [ 5 ] = ( byte ) p.X;
						buffer [ 6 ] = ( byte ) p.Y;
						buffer [ 7 ] = ( byte ) p.Z;
					}
					break;
			}

			buffer [ 0 ] = buffer [ 31 ] = 255;
			buffer [ 1 ] = ( byte ) packet.PacketType;
			buffer [ 2 ] = ( byte ) packet.DeviceType;
			buffer [ 3 ] = 1;
			buffer [ 30 ] = ( byte ) packet.ControllerType;

			return buffer;
		}
	}
}
