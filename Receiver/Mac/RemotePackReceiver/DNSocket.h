//
//  DNSocket.h
//  RemotePresent
//
//  Created by Daramkun on 12. 9. 5..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef enum _DNSocketType
{
	DNSocketTypeTcp,
	DNSocketTypeUdp,
}DNSocketType;

@interface DNAddress : NSObject
{
	in_addr_t addr;
}

- (id) initWithString:(const char*)addrStr;
- (id) initWithLong:(in_addr_t)addrLong;

@property in_addr_t address;

+ (id) anyAddress;
+ (id) broadcastAddress;

@end

@interface DNSocket : NSObject
{
	int m_socket;
}

- (id) initWithProtocol:(DNSocketType)socketType;

- (BOOL) connectToServer:(DNAddress*)addr andPort:(int)port;
- (BOOL) bindPort:(int)port;
- (void) listen:(int)backlog;
- (DNSocket*) accept;
- (void) closeSocket;

- (id) broadcastOn;

- (int) sendToSocket:(void*)buffer andLength:(int)length;
- (int) recvFromSocket:(void*)buffer andLength:(int)length;

- (int) sendToEndPoint:(void*)buffer andLength:(int)length
			andAddress:(DNAddress*)addr andPort:(int)port;
- (int) recvFromEndPoint:(void*)buffer andLength:(int)length
			  andAddress:(DNAddress*)addr andPort:(int*)port;

@end