//
//  DNPacket.h
//  RemotePresent
//
//  Created by Daramkun on 12. 9. 5..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import <Foundation/Foundation.h>

#pragma pack (push, 1)

typedef enum _PacketType
{
	PacketTypeUnknown,
	PacketTypeItsMe,
	PacketTypeIWantConnectToYou,
	PacketTypeCommand,
} PacketType;

typedef enum _DeviceType
{
	DeviceTypeUnknown,
	DeviceTypeWindows,
	DeviceTypeLinux,
	DeviceTypeOSX,
	DeviceTypeiOS,
	DeviceTypeAndroid,
	DeviceTypeBada,
	DeviceTypeWindowsPhone,
} DeviceType;

typedef enum _CommandType
{
	CommandTypeUnknwon,
	CommandTypeSlideLeft,
	CommandTypeSlideRight,
	CommandTypeScreenLaser,
	CommandTypeScreenLaserMove,
	CommandTypePresentationStart,
	CommandTypeMultimediaPrev,
	CommandTypeMultimediaNext,
	CommandTypeMultimediaPlayPause,
	CommandTypeMultimediaStop,
	CommandTypeSoundDown,
	CommandTypeSoundMute,
	CommandTypeSoundUp,
	CommandTypeGamePadJoystick,
	CommandTypeGamePadA,
	CommandTypeGamePadB,
	CommandTypeGamePadX,
	CommandTypeGamePadY,
	CommandTypeGamePadStart,
	CommandTypeGamePadSelect,
} CommandType;

typedef enum _ControllerType
{
	ControllerTypeUnknown,
	ControllerTypeReceiver,
	ControllerTypePresent,
	ControllerTypeMediaControl,
	ControllerTypeGamePad,
} ControllerType;

typedef struct _NativePacket
{
    unsigned char errHeader;
    
    unsigned char packetType;
    unsigned char deviceType;
    unsigned char version;
    
    union
    {
        struct
        {
            char deviceName[21];
            char unused[5];
        } itsMe;
        
        struct
        {
            char deviceName[21];
            char password[5];
        } iWantConnectToYou;
        
        struct
        {
            unsigned char command;
            char x;
            char y;
            char z;
            char unused[22];
        } command;
        
        unsigned char arrContent[26];
    } packetContent;
    
    unsigned char remoteType;
    unsigned char errFooter;
} NativePacket;

#pragma pack (pop)

@interface DNPacket : NSObject
{
	NativePacket nativePacket;
}

- (id) init;

- (void) organizeToReceiverPacket:(NSString*)deviceName;
- (void) organizeToAppPacket:(NSString*)deviceName otPassword:(NSString*)password;
- (void) organizeComandPacket:(CommandType)commandType x:(char)x y:(char)y z:(char)z;

@property (readonly) NativePacket nativePacket;

@end
