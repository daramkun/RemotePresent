//
//  AppDelegate.m
//  RemotePresent
//
//  Created by Daramkun on 12. 11. 29..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import "AppDelegate.h"
#import "BroadcastViewController.h"
#import "ConnectViewController.h"
#import "BluetoothViewController.h"
#import "OptionViewController.h"

#import "FileStream.h"

@implementation AppDelegate

@synthesize deviceName;
@synthesize ipAddress;
@synthesize nonePasswordMode;
@synthesize unconnectionMode;
@synthesize screenLaserEnabled;
@synthesize realSwipeMode;

@synthesize tabBarController;

- (void)dealloc
{
    [_window release];
    [super dealloc];
}

- (void) networkAction:(BOOL)actionState
{
    tabBarController.tabBar.userInteractionEnabled = actionState;
    [UIApplication sharedApplication].networkActivityIndicatorVisible = !actionState;
}

- (void) showGestureView
{
    
}

- (void) saveOptionsToFile
{
	FileStream * fs = [[FileStream alloc] initWithFilename:@"savedData.dat"
                                             andAccessType:FileAccessWrite];
    
    if(fs)
    {
        [fs writeString:deviceName];
        [fs writeString:ipAddress];
        [fs writeBoolean:nonePasswordMode];
    
        [fs writeBoolean:unconnectionMode];
        [fs writeBoolean:screenLaserEnabled];
        [fs writeBoolean:realSwipeMode];
        
        [fs release];
    }
}

- (void) loadOptionsFromFile
{
	FileStream * fs = [[FileStream alloc] initWithFilename:@"savedData.dat"
                                             andAccessType:FileAccessRead];
    
    if(fs && ![fs endOfFile])
    {
        self.deviceName = [fs readString];
        self.ipAddress = [fs readString];
        self.nonePasswordMode = [fs readBoolean];
    
        self.unconnectionMode = [fs readBoolean];
        self.screenLaserEnabled = [fs readBoolean];
        self.realSwipeMode = [fs readBoolean];
        
        [fs release];
    }
    else
    {
        self.deviceName = @"Remote Present";
        self.ipAddress = @"192.168.0.3";
        self.nonePasswordMode = false;
        
        self.unconnectionMode = false;
        self.screenLaserEnabled = true;
        self.realSwipeMode = false;
    }
}

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    self.window = [[[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]] autorelease];
    self.window.backgroundColor = [UIColor whiteColor];
    
    [self loadOptionsFromFile];
    
    UIColor * navTintColor = [UIColor colorWithRed:82 / 255.0f
                                             green:140 / 255.0f
                                              blue:189 / 255.0f
                                             alpha:1.0f
                              ];
    UIColor * tintColor = [UIColor colorWithRed:62 / 255.0f
                                          green:120 / 255.0f
                                           blue:169 / 255.0f
                                          alpha:1.0f
                           ];
    
    UINavigationController * broadcastNavigator = [[UINavigationController alloc]
                                                   initWithRootViewController:[[BroadcastViewController alloc] initWithDelegate:self]];
    UINavigationController * connectNavigator = [[UINavigationController alloc]
                                                 initWithRootViewController:[[ConnectViewController alloc] initWithDelegate:self]];
    UINavigationController * bluetoothNavigator = [[UINavigationController alloc]
                                                   initWithRootViewController:[[BluetoothViewController alloc] initWithDelegate:self]];
    UINavigationController * optionNavigator = [[UINavigationController alloc]
                                                initWithRootViewController:[[OptionViewController alloc] initWithDelegate:self]];
    
    broadcastNavigator.navigationBar.tintColor =
    connectNavigator.navigationBar.tintColor =
    bluetoothNavigator.navigationBar.tintColor =
    optionNavigator.navigationBar.tintColor = navTintColor;
    
    self.window.rootViewController = tabBarController = [[[UITabBarController alloc] init] autorelease];
    tabBarController.tabBar.tintColor = tintColor;
    tabBarController.viewControllers = [NSArray arrayWithObjects:broadcastNavigator, connectNavigator, bluetoothNavigator, optionNavigator, nil];
    
    [self.window makeKeyAndVisible];
    return YES;
}

- (void)applicationDidEnterBackground:(UIApplication *)application
{
    [self saveOptionsToFile];
}

- (void)applicationWillTerminate:(UIApplication *)application
{
    [self saveOptionsToFile];
}

@end
