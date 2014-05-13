//
//  NetworkStream.m
//  RemotePresent
//
//  Created by Daramkun on 12. 12. 2..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import "NetworkStream.h"

@implementation NetworkStream

- (id) initWithConnection:(NSString*)ipAddress
{
    if(self = [super init])
    {
        socketId = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);
        
        socketEndPoint.sin_family = AF_INET;
        socketEndPoint.sin_addr.s_addr = htonl(INADDR_LOOPBACK);
        socketEndPoint.sin_port = htons(25567);
        
        remoteEndPoint.sin_family = AF_INET;
        remoteEndPoint.sin_addr.s_addr = inet_addr(ipAddress.UTF8String);
        remoteEndPoint.sin_port = htons(25567);
        
        if(connect(socketId, (struct sockaddr*)&remoteEndPoint, sizeof(remoteEndPoint)) < 0)
            return nil;
        
        connectMode = 1;
    }
    return self;
}

- (id) initWithBroadcasting
{
    if(self = [super init])
    {
        socketId = socket(PF_INET, SOCK_DGRAM, IPPROTO_UDP);
        
        socketEndPoint.sin_family = AF_INET;
        socketEndPoint.sin_addr.s_addr = htonl(INADDR_LOOPBACK);
        socketEndPoint.sin_port = htons(25567);
        
        if(bind(socketId, (struct sockaddr*)&socketEndPoint,
                sizeof(socketEndPoint)) < 0)
            return nil;
        
        int broadcastEnabled = 1;
        if(setsockopt(socketId, SOL_SOCKET, SO_BROADCAST,
                      &broadcastEnabled, sizeof(broadcastEnabled)) != 0)
            return nil;
        
        remoteEndPoint.sin_family = AF_INET;
        remoteEndPoint.sin_addr.s_addr = htonl(INADDR_BROADCAST);
        remoteEndPoint.sin_port = htons(25567);
        
        connectMode = 2;
    }
    return self;
}

- (void)dealloc
{
    close(socketId);
    [super dealloc];
}

- (char) readCharacter
{
    char buffer = 0;
    if(connectMode == 1)
        recv(socketId, &buffer, sizeof(buffer), 0);
    else if(connectMode == 2)
    {
        socklen_t remoteEndPointLength = sizeof(remoteEndPoint);
        recvfrom(socketId, &buffer, sizeof(buffer), 0,
                 (struct sockaddr*)&remoteEndPoint, &remoteEndPointLength);
    }
    return buffer;
}

- (short) readShort
{
    short buffer = 0;
    if(connectMode == 1)
        recv(socketId, &buffer, sizeof(buffer), 0);
    else if(connectMode == 2)
    {
        socklen_t remoteEndPointLength = sizeof(remoteEndPoint);
        recvfrom(socketId, &buffer, sizeof(buffer), 0,
                 (struct sockaddr*)&remoteEndPoint, &remoteEndPointLength);
    }
    return buffer;
}

- (int) readInteger
{
    int buffer = 0;
    if(connectMode == 1)
        recv(socketId, &buffer, sizeof(buffer), 0);
    else if(connectMode == 2)
    {
        socklen_t remoteEndPointLength = sizeof(remoteEndPoint);
        recvfrom(socketId, &buffer, sizeof(buffer), 0,
                 (struct sockaddr*)&remoteEndPoint, &remoteEndPointLength);
    }
    return buffer;
}

- (long long) readLong
{
    long long buffer = 0;
    if(connectMode == 1)
        recv(socketId, &buffer, sizeof(buffer), 0);
    else if(connectMode == 2)
    {
        socklen_t remoteEndPointLength = sizeof(remoteEndPoint);
        recvfrom(socketId, &buffer, sizeof(buffer), 0,
                 (struct sockaddr*)&remoteEndPoint, &remoteEndPointLength);
    }
    return buffer;
}

- (bool) readBoolean
{
    bool buffer = 0;
    if(connectMode == 1)
        recv(socketId, &buffer, sizeof(buffer), 0);
    else if(connectMode == 2)
    {
        socklen_t remoteEndPointLength = sizeof(remoteEndPoint);
        recvfrom(socketId, &buffer, sizeof(buffer), 0,
                 (struct sockaddr*)&remoteEndPoint, &remoteEndPointLength);
    }
    return buffer;
}

- (NSString*) readString
{
    char length = [self readCharacter];
    
    char * buffer = (char*)malloc(length);
    if(connectMode == 1)
        recv(socketId, &buffer, length, 0);
    else if(connectMode == 2)
    {
        socklen_t remoteEndPointLength = sizeof(remoteEndPoint);
        recvfrom(socketId, &buffer, length, 0,
                 (struct sockaddr*)&remoteEndPoint, &remoteEndPointLength);
    }
    NSString * temp = [NSString stringWithCString:buffer encoding:NSStringEncodingConversionExternalRepresentation];
    free(buffer);
    
    return temp;
}

- (NSData*) readData
{
    int length = [self readInteger];
    
    char * buffer = (char*)malloc(length);
    if(connectMode == 1)
        recv(socketId, &buffer, length, 0);
    else if(connectMode == 2)
    {
        socklen_t remoteEndPointLength = sizeof(remoteEndPoint);
        recvfrom(socketId, &buffer, length, 0,
                 (struct sockaddr*)&remoteEndPoint, &remoteEndPointLength);
    }
    NSData * temp = [NSData dataWithBytes:buffer length:length];
    free(buffer);
    
    return temp;
}

- (void) writeCharacter:(char)value
{
    if(connectMode == 1)
        send(socketId, &value, sizeof(value), 0);
    else if(connectMode == 2)
    {
        sendto(socketId, &value, sizeof(value), 0,
                 (struct sockaddr*)&remoteEndPoint, sizeof(remoteEndPoint));
    }
}

- (void) writeShort:(short)value
{
    if(connectMode == 1)
        send(socketId, &value, sizeof(value), 0);
    else if(connectMode == 2)
    {
        sendto(socketId, &value, sizeof(value), 0,
               (struct sockaddr*)&remoteEndPoint, sizeof(remoteEndPoint));
    }
}

- (void) writeInteger:(int)value
{
    if(connectMode == 1)
        send(socketId, &value, sizeof(value), 0);
    else if(connectMode == 2)
    {
        sendto(socketId, &value, sizeof(value), 0,
               (struct sockaddr*)&remoteEndPoint, sizeof(remoteEndPoint));
    }
}

- (void) writeLong:(long long)value
{
    if(connectMode == 1)
        send(socketId, &value, sizeof(value), 0);
    else if(connectMode == 2)
    {
        sendto(socketId, &value, sizeof(value), 0,
               (struct sockaddr*)&remoteEndPoint, sizeof(remoteEndPoint));
    }
}

- (void) writeBoolean:(bool)value
{
    if(connectMode == 1)
        send(socketId, &value, sizeof(value), 0);
    else if(connectMode == 2)
    {
        sendto(socketId, &value, sizeof(value), 0,
               (struct sockaddr*)&remoteEndPoint, sizeof(remoteEndPoint));
    }
}

- (void) writeString:(NSString*)value
{
    [self writeCharacter:value.length];
    
    if(connectMode == 1)
        send(socketId, value.UTF8String, value.length, 0);
    else if(connectMode == 2)
    {
        sendto(socketId, value.UTF8String, value.length, 0,
               (struct sockaddr*)&remoteEndPoint, sizeof(remoteEndPoint));
    }
}

- (void) writeData:(NSData*)value
{
    [self writeInteger:value.length];
    
    if(connectMode == 1)
        send(socketId, value.bytes, value.length, 0);
    else if(connectMode == 2)
    {
        sendto(socketId, value.bytes, value.length, 0,
               (struct sockaddr*)&remoteEndPoint, sizeof(remoteEndPoint));
    }
}

@end
