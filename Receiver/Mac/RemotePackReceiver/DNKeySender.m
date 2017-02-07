//
//  DNKeySender.m
//  RemotePackReceiver
//
//  Created by Daramkun on 12. 10. 15..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import "DNKeySender.h"

@implementation DNKeySender

- (id) init
{
    if(self = [super init])
    {
        eventRef = CGEventSourceCreate(kCGEventSourceStateCombinedSessionState);
    }
    return self;
}

- (void) dealloc
{
    if(eventRef) CFRelease(eventRef);
    [super dealloc];
}

- (CGEventFlags) makeFlag
{
    eventFlag = 0;
    if(isShiftDown) eventFlag |= kCGEventFlagMaskShift;
    if(isCommandDown) eventFlag |= kCGEventFlagMaskCommand;
    if(isOptionDown) eventFlag |= kCGEventFlagMaskAlternate;
    if(isCtrlDown) eventFlag |= kCGEventFlagMaskControl;
    if(isFunctionDown) eventFlag |= kCGEventFlagMaskSecondaryFn;
    return eventFlag;
}

- (void) sendModifyKey:(CGKeyCode)keyCode isKeyDown:(BOOL)isKeyDown
{
    switch (keyCode)
    {
        case kVK_Shift: isShiftDown = isKeyDown; break;
        case kVK_Command: isCommandDown = isKeyDown; break;
        case kVK_Option: isOptionDown = isKeyDown; break;
        case kVK_Control: isCtrlDown = isKeyDown; break;
        case kVK_Function: isFunctionDown = isKeyDown; break;
            
        default: return;
    }
}

- (void) sendKey:(CGKeyCode)keyCode isKeyDown:(BOOL)isKeyDown
{
    switch (keyCode)
    {
        case kVK_Shift:
        case kVK_Command:
        case kVK_Option:
        case kVK_Control:
        case kVK_Function:
            return;
            
        default:
            {
                CGEventRef event = CGEventCreateKeyboardEvent(eventRef, keyCode, isKeyDown);
                CGEventSetFlags(event, [self makeFlag]);
                CGEventPost(kCGAnnotatedSessionEventTap, event);
                usleep(50);
                CFRelease(event);
            }
            break;
    }
}

- (void) sendKeyOnce:(CGKeyCode)keyCode
{
    switch (keyCode)
    {
        case kVK_Shift:
        case kVK_Command:
        case kVK_Option:
        case kVK_Control:
        case kVK_Function:
            return;
            
        default:
            {
                CGEventRef event = CGEventCreateKeyboardEvent(eventRef, keyCode, YES);
                CGEventRef event2 = CGEventCreateKeyboardEvent(eventRef, keyCode, NO);
                
                CGEventSetFlags(event, [self makeFlag]);
                CGEventSetFlags(event2, eventFlag);
                
                CGEventPost(kCGAnnotatedSessionEventTap, event);
                usleep(50);
                CGEventPost(kCGAnnotatedSessionEventTap, event2);
                usleep(50);
                
                CFRelease(event2);
                CFRelease(event);
            }
            break;
    }
}

- (void) sendCharacterKey:(NSString*)chr isKeyDown:(BOOL)isKeyDown
{
    if(chr.length < 1) return;
    
    CGEventRef event = CGEventCreateKeyboardEvent(eventRef, kVK_ANSI_A, isKeyDown);
    unichar symbol = [chr characterAtIndex:0];
    
    CGEventKeyboardSetUnicodeString(event, 1, &symbol);
    CGEventSetFlags(event, CGEventGetFlags(event) | [self makeFlag]);
    CGEventPost(kCGAnnotatedSessionEventTap, event);
    usleep(50);
    
    CFRelease(event);
}

- (void) sendCharacterKeyOnce:(NSString*)chr
{
    if(chr.length < 1) return;
    
    CGEventRef event = CGEventCreateKeyboardEvent(eventRef, kVK_ANSI_A, YES);
    CGEventRef event2 = CGEventCreateKeyboardEvent(eventRef, kVK_ANSI_A, NO);
    unichar symbol = [chr characterAtIndex:0];
    
    CGEventKeyboardSetUnicodeString(event, 1, &symbol);
    CGEventKeyboardSetUnicodeString(event2, 1, &symbol);
    
    CGEventSetFlags(event, CGEventGetFlags(event) | [self makeFlag]);
    CGEventSetFlags(event2, CGEventGetFlags(event2) | eventFlag);
    
    CGEventPost(kCGAnnotatedSessionEventTap, event);
    usleep(50);
    CGEventPost(kCGAnnotatedSessionEventTap, event2);
    usleep(50);
    
    CFRelease(event2);
    CFRelease(event);
}

@end
