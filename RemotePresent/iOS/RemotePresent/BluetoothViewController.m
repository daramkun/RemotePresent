//
//  BluetoothViewController.m
//  RemotePresent
//
//  Created by Daramkun on 12. 11. 29..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import "BluetoothViewController.h"

@interface BluetoothViewController ()

@end

@implementation BluetoothViewController

@synthesize delegate;

- (id) initWithDelegate:(id<PresentActionDelegate>)_delegate
{
    if(self = [super initWithNibName:@"BluetoothViewController" bundle:nil])
    {
        self.title = @"블루투스";
        self.navigationItem.prompt = @"Remote Present";
        delegate = _delegate;
    }
    
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    // Do any additional setup after loading the view from its nib.
    
}

@end
