//
//  DNSocket.m
//  RemotePresent
//
//  Created by Daramkun on 12. 9. 5..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import "DNSocket.h"
#import <unistd.h>
#import <sys/types.h>
#import <sys/socket.h>
#import <netinet/in.h>
#import <arpa/inet.h>

@implementation DNAddress

@synthesize address = addr;

- (id) initWithString:(const char*)addrStr
{
	if(self = [super init])
	{
		self.address = inet_addr(addrStr);
	}
	return self;
}

- (id) initWithLong:(in_addr_t)addrLong
{
	if(self = [super init])
	{
		self.address = htonl(addrLong);
	}
	return self;
}

+ (id) anyAddress
{
	return [[[DNAddress alloc] initWithLong:INADDR_ANY] autorelease];
}

+ (id) broadcastAddress
{
	return [[[DNAddress alloc] initWithLong:INADDR_BROADCAST] autorelease];
}

- (BOOL) isEqual:(id)object
{
    if([object isKindOfClass:[DNAddress class]])
        return (self.address == ((DNAddress*)object).address);
    else return NO;
}

@end

@interface DNSocket()

@property int mainSocket;

@end

@implementation DNSocket

@synthesize mainSocket = m_socket;

- (id) initWithProtocol:(DNSocketType)socketType
{
	if(self = [super init])
	{
		switch(socketType)
		{
			case DNSocketTypeTcp:
				self.mainSocket = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);
				break;
			case DNSocketTypeUdp:
				self.mainSocket = socket(PF_INET, SOCK_DGRAM, IPPROTO_UDP);
				break;
			default:
				[self release];
				return nil;
		}
		
		int nosigpipe = 1;
		setsockopt(self.mainSocket, SOL_SOCKET, SO_NOSIGPIPE, &nosigpipe, sizeof(nosigpipe));
	}
	
	return self;
}

- (BOOL) connectToServer:(DNAddress*)addr andPort:(int)port
{
	struct sockaddr_in connAddr = {0, };
	connAddr.sin_family = AF_INET;
	connAddr.sin_addr.s_addr = addr.address;
	connAddr.sin_port = htons(port);
	
	if(connect(m_socket, (struct sockaddr*)&connAddr, sizeof(connAddr)) < 0)
		return NO;
	
	return YES;
}

- (BOOL) bindPort:(int)port
{
	struct sockaddr_in bindAddr = {0, };
	bindAddr.sin_family = AF_INET;
	bindAddr.sin_addr.s_addr = htonl(INADDR_ANY);
	bindAddr.sin_port = htons(port);
	
	if(bind(self.mainSocket, (struct sockaddr*)&bindAddr, sizeof(bindAddr)) < 0)
		return NO;
	
	return YES;
}

- (void) listen:(int)backlog
{
    listen(self.mainSocket, backlog);
}

- (DNSocket*) accept
{
	DNSocket * socket = [[DNSocket alloc] init];
	struct sockaddr addr;
	int len = sizeof(addr);
	
	socket.mainSocket = accept(self.mainSocket, &addr, (socklen_t*)&len);
	return [socket autorelease];
}

- (void) closeSocket
{
	close(m_socket);
}

- (id) broadcastOn
{
	int be = 1;
	if(setsockopt(self.mainSocket, SOL_SOCKET, SO_BROADCAST, &be, sizeof(be)) != 0)
		return nil;
	return self;
}

- (int) sendToSocket:(void*)buffer andLength:(int)length
{
	return (int)send(self.mainSocket, buffer, length, 0);
}

- (int) recvFromSocket:(void*)buffer andLength:(int)length
{
	return (int)recv(self.mainSocket, buffer, length, 0);
}

- (int) sendToEndPoint:(void*)buffer andLength:(int)length
			andAddress:(DNAddress*)addr andPort:(int)port
{
	if(addr == nil) return -1;
	
	struct sockaddr_in stAddr = {0, };
	stAddr.sin_family = AF_INET;
	stAddr.sin_addr.s_addr = addr.address;
	stAddr.sin_port = htons(port);
	
	return (int)sendto(self.mainSocket, buffer, length, 0, (struct sockaddr*)&stAddr, sizeof(stAddr));
}

- (int) recvFromEndPoint:(void*)buffer andLength:(int)length
			  andAddress:(DNAddress*)addr andPort:(int*)port
{
	if(addr == nil) return -1;
	if(port == nil) return -1;
	
	struct sockaddr_in rfAddr = {0, };
	rfAddr.sin_family = AF_INET;
	rfAddr.sin_addr.s_addr = addr.address;
	rfAddr.sin_port = htons(*port);
	
	socklen_t rfLength = sizeof(rfAddr);
	
	int len = (int)recvfrom(self.mainSocket, buffer, length, 0,
                            (struct sockaddr*)&rfAddr, &rfLength);
	addr.address = rfAddr.sin_addr.s_addr;
    *port = ntohs(rfAddr.sin_port);
	
	return len;
}

@end
