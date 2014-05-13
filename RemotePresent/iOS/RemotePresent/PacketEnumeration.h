//
//  PacketEnumeration.h
//  RemotePresent
//
//  Created by Daramkun on 12. 12. 11..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

typedef enum _PacketType : unsigned char
{
    PacketTypeUnknown = 0,
    
	PacketTypeBroadcast = 1,
	PacketTypeConnectionRequest = 2,
	PacketTypeCommand = 3,
} PacketType;

typedef enum _DeviceType : unsigned char
{
	DeviceTypeUnknown = 0,
    
	DeviceTypeWindows = 1,
	DeviceTypeLinux = 2,
	DeviceTypeOSX = 3,
    
	DeviceTypeiPhone = 4,
	DeviceTypeAndroid = 5,
	DeviceTypeBada = 6, /* Not support in 2.0 */
	DeviceTypeWindowsPhone = 7,
    
    /* Remote Packet Version 2.0 */
    DeviceTypeWindowsRT = 8,
    DeviceTypeiPad = 9,
} DeviceType;

typedef enum _PacketVersion : unsigned char
{
    PacketVersion1 = 0x01,
    PacketVersion2 = 0x02,
} PacketVersion;

typedef enum _CommandType : unsigned char
{
	CommandTypeUnknwon = 0,
    
	CommandTypeSlideLeft = 1,
	CommandTypeSlideRight = 2,
	CommandTypeScreenLaser = 3,
	CommandTypeScreenLaserMove = 4,
	CommandTypePresentationStart = 5,
    
	CommandTypeMultimediaPrev = 6,
	CommandTypeMultimediaNext = 7,
	CommandTypeMultimediaPlayPause = 8,
	CommandTypeMultimediaStop = 9,
	CommandTypeSoundDown = 10,
	CommandTypeSoundMute = 11,
	CommandTypeSoundUp = 12,
} CommandType;

typedef enum _ControllerType : unsigned char
{
	ControllerTypeUnknown = 0,
    
	ControllerTypeReceiver = 1,
    
    /* Not support in 2.0 */
	ControllerTypePresent = 2,
	ControllerTypeMediaControl = 3,
	ControllerTypeGamePad = 4,
    /* Not support in 2.0 */
    
    ControllerTypeNesture = 5,  /* Added 2.0 */
} ControllerType;
