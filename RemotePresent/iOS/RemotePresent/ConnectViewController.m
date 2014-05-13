//
//  ConnectViewController.m
//  RemotePresent
//
//  Created by Daramkun on 12. 11. 29..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import "ConnectViewController.h"
#import "AppDelegate.h"

@interface ConnectViewController ()

@end

@implementation ConnectViewController

@synthesize delegate;

- (void) connectionStart:(UIBarButtonItem*)sender
{
    
}

- (id) initWithDelegate:(id<PresentActionDelegate>)_delegate
{
    if(self = [super initWithNibName:@"ConnectViewController" bundle:nil])
    {
        self.title = @"수동연결";
        self.navigationItem.prompt = @"Remote Present";
        delegate = _delegate;
        
        self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithTitle:@"연결" style:UIBarButtonItemStyleDone target:self action:@selector(connectionStart:)];
    }
    
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    // Do any additional setup after loading the view from its nib.
}

- (void)viewWillAppear:(BOOL)animated
{
    textFieldIpAddress.text = delegate.ipAddress;
}

- (void)viewWillDisappear:(BOOL)animated
{
    delegate.ipAddress = textFieldIpAddress.text;
    [delegate saveOptionsToFile];
}

- (void)touchesEnded:(NSSet *)touches withEvent:(UIEvent *)event
{
    [textFieldIpAddress resignFirstResponder];
}

- (BOOL)textFieldShouldReturn:(UITextField *)textField
{
    [textField resignFirstResponder];
    return YES;
}

@end
