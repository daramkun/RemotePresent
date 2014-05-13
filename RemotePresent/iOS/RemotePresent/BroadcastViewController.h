//
//  BroadcastViewController.h
//  RemotePresent
//
//  Created by Daramkun on 12. 11. 29..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "NetworkStream.h"
#import "PresentActionDelegate.h"

@interface BroadcastViewController : UIViewController<UITextFieldDelegate>
{
#pragma mark - Member variables
    id<PresentActionDelegate> delegate;
    
    NetworkStream * networkStream;
    
    NSThread * recvThread;
    NSTimer * sendTimer;
    
#pragma mark - IBOutlets
    IBOutlet UITextField * textFieldDeviceName;
    IBOutlet UITextField * textFieldOneTimePassword;
    IBOutlet UISwitch * switchPasswordEnabled;
}

#pragma mark - Properties
@property (nonatomic, readonly) id<PresentActionDelegate> delegate;

#pragma mark - Methods
- (id) initWithDelegate:(id<PresentActionDelegate>)delegate;

#pragma mark - IBActions

@end
