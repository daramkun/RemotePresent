//
//  AppDelegate.h
//  RemotePresent
//
//  Created by Daramkun on 12. 11. 29..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "PresentActionDelegate.h"

@interface AppDelegate : UIResponder <UIApplicationDelegate, PresentActionDelegate>
{   
    UITabBarController * tabBarController;
    
    NSString* deviceName;
    NSString* ipAddress;
    bool nonePasswordMode;
    bool unconnectionMode;
    bool screenLaserEnabled;
    bool realSwipeMode;
}

@property (strong, nonatomic) UIWindow * window;
@property (nonatomic, readonly) UITabBarController * tabBarController;

@end
