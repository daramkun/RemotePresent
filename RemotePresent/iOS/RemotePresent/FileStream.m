//
//  FileStream.m
//  RemotePresent
//
//  Created by Daramkun on 12. 12. 4..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import "FileStream.h"

@implementation FileStream

- (id) initWithFilename:(NSString*)filename andAccessType:(FileAccess)access
{
    if(self = [super init])
    {
		NSArray * paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
		NSString * documentsDirectory = [paths objectAtIndex:0];
		NSString * fullFilename = [NSString stringWithFormat:@"%@/%@", documentsDirectory, filename];
		
		const char * file = fullFilename.UTF8String;
        fp = fopen(file,
                   (access == FileAccessRead) ? "rb" : ((access == FileAccessWrite) ? "wb" : "rwb"));
        
        if(!fp) return nil;
    }
    return self;
}

- (void)dealloc
{
    fclose(fp);
    [super dealloc];
}

- (bool) endOfFile
{
    return feof(fp);
}

- (char) readCharacter
{
    char buffer = 0;
    fread(&buffer, sizeof(buffer), 1, fp);
    return buffer;
}

- (short) readShort
{
    short buffer = 0;
    fread(&buffer, sizeof(buffer), 1, fp);
    return buffer;
}

- (int) readInteger
{
    int buffer = 0;
    fread(&buffer, sizeof(buffer), 1, fp);
    return buffer;
}

- (long long) readLong
{
    long long buffer = 0;
    fread(&buffer, sizeof(buffer), 1, fp);
    return buffer;
}

- (bool) readBoolean
{
    bool buffer = 0;
    fread(&buffer, sizeof(buffer), 1, fp);
    return buffer;
}

- (NSString*) readString
{
    char length = [self readCharacter];
    
    char * buffer = (char*)malloc(length + 1);
    fread(buffer, length, 1, fp);
    buffer[length] = 0;
    NSString * temp = [NSString stringWithCString:buffer encoding:NSStringEncodingConversionExternalRepresentation];
    free(buffer);
    
    return temp;
}

- (NSData*) readData
{
    int length = [self readInteger];
    
    char * buffer = (char*)malloc(length);
    fread(buffer, length, 1, fp);
    NSData * temp = [NSData dataWithBytes:buffer length:length];
    free(buffer);
    
    return temp;
}

- (void) writeCharacter:(char)value
{
    fwrite(&value, sizeof(value), 1, fp);
}

- (void) writeShort:(short)value
{
    fwrite(&value, sizeof(value), 1, fp);
}

- (void) writeInteger:(int)value
{
    fwrite(&value, sizeof(value), 1, fp);
}

- (void) writeLong:(long long)value
{
    fwrite(&value, sizeof(value), 1, fp);
}

- (void) writeBoolean:(bool)value
{
    fwrite(&value, sizeof(value), 1, fp);
}

- (void) writeString:(NSString*)value
{
    [self writeCharacter:(char)value.length];
    fwrite(value.UTF8String, value.length, 1, fp);
}

- (void) writeData:(NSData*)value
{
    [self writeInteger:value.length];
    fwrite(value.bytes, value.length, 1, fp);
}

@end
