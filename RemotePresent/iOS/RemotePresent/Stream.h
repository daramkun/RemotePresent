//
//  Stream.h
//  RemotePresent
//
//  Created by Daramkun on 12. 12. 4..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol Stream <NSObject>

- (char) readCharacter;
- (short) readShort;
- (int) readInteger;
- (long long) readLong;
- (bool) readBoolean;
- (NSString*) readString;
- (NSData*) readData;

- (void) writeCharacter:(char)value;
- (void) writeShort:(short)value;
- (void) writeInteger:(int)value;
- (void) writeLong:(long long)value;
- (void) writeBoolean:(bool)value;
- (void) writeString:(NSString*)value;
- (void) writeData:(NSData*)value;

@end
