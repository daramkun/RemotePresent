//
//  OptionViewController.m
//  RemotePresent
//
//  Created by Daramkun on 12. 11. 29..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import "OptionViewController.h"
#import "AppDelegate.h"

@interface OptionViewController ()

@end

@implementation OptionViewController

@synthesize delegate;

- (id) initWithDelegate:(id<PresentActionDelegate>)_delegate
{
    if(self = [super initWithNibName:@"OptionViewController" bundle:nil])
    {
        self.title = @"환경설정";
        self.navigationItem.prompt = @"Remote Present";
        delegate = _delegate;
    }
    
    return self;
}

- (void)viewWillAppear:(BOOL)animated
{
    switchUnconnectMode.on = delegate.unconnectionMode;
    switchScreenLaserMode.on = delegate.screenLaserEnabled;
    switchRealSwipeMode.on = delegate.realSwipeMode;
}

- (void)viewWillDisappear:(BOOL)animated
{
    delegate.unconnectionMode = switchUnconnectMode.on;
    delegate.screenLaserEnabled = switchScreenLaserMode.on;
    delegate.realSwipeMode = switchRealSwipeMode.on;
    [delegate saveOptionsToFile];
}

@end
