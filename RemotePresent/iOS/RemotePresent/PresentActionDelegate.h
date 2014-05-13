//
//  NetworkActionDelegate.h
//  RemotePresent
//
//  Created by Daramkun on 12. 11. 29..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol PresentActionDelegate <NSObject>

@property (nonatomic, copy) NSString* deviceName;
@property (nonatomic, copy) NSString* ipAddress;
@property bool nonePasswordMode;
@property bool unconnectionMode;
@property bool screenLaserEnabled;
@property bool realSwipeMode;

- (void) networkAction:(BOOL)actionState;
- (void) showGestureView;
- (void) saveOptionsToFile;
- (void) loadOptionsFromFile;

@end
