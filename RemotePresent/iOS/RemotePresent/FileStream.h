//
//  FileStream.h
//  RemotePresent
//
//  Created by Daramkun on 12. 12. 4..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <stdio.h>
#import "Stream.h"

typedef enum _FileAccess
{
    FileAccessRead,
    FileAccessWrite,
    FileAccessReadWrite,
} FileAccess;

@interface FileStream : NSObject<Stream>
{
    FILE * fp;
}

- (id) initWithFilename:(NSString*)filename andAccessType:(FileAccess)access;

- (bool) endOfFile;

@end
