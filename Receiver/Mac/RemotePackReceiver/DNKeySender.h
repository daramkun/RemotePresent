//
//  DNKeySender.h
//  RemotePackReceiver
//
//  Created by Daramkun on 12. 10. 15..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import <Foundation/Foundation.h>
//#import "DNVirtualKey.h"
#import <Carbon/Carbon.h>

@interface DNKeySender : NSObject
{
    BOOL isCommandDown, isOptionDown,
        isShiftDown, isFunctionDown,
        isCtrlDown;
    CGEventFlags eventFlag;
    
    CGEventSourceRef eventRef;
}

- (void) sendModifyKey:(CGKeyCode)keyCode isKeyDown:(BOOL)isKeyDown;

- (void) sendKey:(CGKeyCode)keyCode isKeyDown:(BOOL)isKeyDown;
- (void) sendKeyOnce:(CGKeyCode)keyCode;
- (void) sendCharacterKey:(NSString*)chr isKeyDown:(BOOL)isKeyDown;
- (void) sendCharacterKeyOnce:(NSString*)chr;

@end
