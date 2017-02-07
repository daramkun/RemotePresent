//
//  DNPacket.m
//  RemotePresent
//
//  Created by Daramkun on 12. 9. 5..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import "DNPacket.h"

void strcpy_s(char * dest, const char * src, int length);
void strcpy_s(char * dest, const char * src, int length)
{
    int i = 0;
    for(i = 0; (i < length) && (src[i] != 0); i++)
        dest[i] = src[i];
    dest[i] = 0;
}

@implementation DNPacket

@synthesize nativePacket;

- (id) init
{
	if(self = [super init])
	{
		nativePacket.errHeader = nativePacket.errFooter = 255;
		nativePacket.version = 1;
		nativePacket.deviceType = DeviceTypeOSX;
		nativePacket.remoteType = ControllerTypeReceiver;
	}
	
	return self;
}

- (void) organizeToReceiverPacket:(NSString*)deviceName
{
	nativePacket.packetType = PacketTypeItsMe;
	strcpy_s(nativePacket.packetContent.itsMe.deviceName, deviceName.UTF8String, 19);
}

- (void) organizeToAppPacket:(NSString*)deviceName otPassword:(NSString*)password
{
	nativePacket.packetType = PacketTypeIWantConnectToYou;
	strcpy_s(nativePacket.packetContent.iWantConnectToYou.deviceName, deviceName.UTF8String, 19);
	strcpy_s(nativePacket.packetContent.iWantConnectToYou.password, password.UTF8String, 4);
}

- (void) organizeComandPacket:(CommandType)commandType x:(char)x y:(char)y z:(char)z
{
	nativePacket.packetType = PacketTypeCommand;
	nativePacket.packetContent.command.command = commandType;
	nativePacket.packetContent.command.x = x;
	nativePacket.packetContent.command.y = y;
	nativePacket.packetContent.command.z = z;
}

@end
