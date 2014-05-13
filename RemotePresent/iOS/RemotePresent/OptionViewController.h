//
//  OptionViewController.h
//  RemotePresent
//
//  Created by Daramkun on 12. 11. 29..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "PresentActionDelegate.h"

@interface OptionViewController : UIViewController
{
    id<PresentActionDelegate> delegate;
    
    IBOutlet UISwitch * switchUnconnectMode;
    IBOutlet UISwitch * switchScreenLaserMode;
    IBOutlet UISwitch * switchRealSwipeMode;
}

@property (nonatomic, readonly) id<PresentActionDelegate> delegate;

- (id) initWithDelegate:(id<PresentActionDelegate>)delegate;

@end
