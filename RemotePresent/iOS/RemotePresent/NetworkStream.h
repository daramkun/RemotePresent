//
//  NetworkStream.h
//  RemotePresent
//
//  Created by Daramkun on 12. 12. 2..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import <Foundation/Foundation.h>

#import <unistd.h>
#import <sys/types.h>
#import <sys/socket.h>
#import <netinet/in.h>
#import <arpa/inet.h>

#import "Stream.h"

@interface NetworkStream : NSObject<Stream>
{
    int connectMode;
    
    int socketId;
    struct sockaddr_in socketEndPoint;
    
    struct sockaddr_in remoteEndPoint;
}

- (id) initWithConnection:(NSString*)ipAddress;
- (id) initWithBroadcasting;

@end
