//
//  BroadcastViewController.m
//  RemotePresent
//
//  Created by Daramkun on 12. 11. 29..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import "BroadcastViewController.h"
#import "PacketEnumeration.h"

@interface BroadcastViewController ()

@end

@implementation BroadcastViewController

@synthesize delegate;

- (id) initWithDelegate:(id<PresentActionDelegate>)_delegate
{
    if(self = [super initWithNibName:@"BroadcastViewController" bundle:nil])
    {
        self.title = @"브로드캐스트";
        self.navigationItem.prompt = @"Remote Present";
        delegate = _delegate;
    }
    
    return self;
}

- (void) receiveConnection:(id)object
{
    @autoreleasepool
    {
        while(true)
        {
            unsigned char errorChecker = [networkStream readCharacter];
            if(errorChecker != 255)
                continue;
            PacketType packetType = [networkStream readCharacter];
            DeviceType deviceType = [networkStream readCharacter];
            PacketVersion packetVersion = [networkStream readCharacter];
            
            switch (packetVersion)
            {
                case PacketVersion1:
                    
                    break;
                    
                case PacketVersion2:
                    
                    break;
                    
                default:
                    break;
            }
        }
    }
}

- (void) sendBroadcast:(NSTimer*)timer
{
    
}

- (void)viewWillAppear:(BOOL)animated
{
    textFieldDeviceName.text = delegate.deviceName;
    switchPasswordEnabled.on = delegate.nonePasswordMode;
    
    recvThread = [[NSThread alloc] initWithTarget:self
                                         selector:@selector(receiveConnection:)
                                           object:nil];
    sendTimer = [NSTimer scheduledTimerWithTimeInterval:2.0f
                                                 target:self
                                               selector:@selector(sendBroadcast:)
                                               userInfo:nil
                                                repeats:YES];
    
    networkStream = [[NetworkStream alloc] initWithBroadcasting];
}

- (void)viewWillDisappear:(BOOL)animated
{
    delegate.deviceName = textFieldDeviceName.text;
    delegate.nonePasswordMode = switchPasswordEnabled.on;
    
    [recvThread release];
    [sendTimer release];
    
    [delegate saveOptionsToFile];
    
    [networkStream release];
}

- (void)touchesEnded:(NSSet *)touches withEvent:(UIEvent *)event
{
    [textFieldDeviceName resignFirstResponder];
    [textFieldOneTimePassword resignFirstResponder];
}

- (BOOL)textFieldShouldReturn:(UITextField *)textField
{
    [textField resignFirstResponder];
    return YES;
}

@end
