//
//  ConnectViewController.h
//  RemotePresent
//
//  Created by Daramkun on 12. 11. 29..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "PresentActionDelegate.h"

@interface ConnectViewController : UIViewController<UITextFieldDelegate>
{
#pragma mark - Member variables
    id<PresentActionDelegate> delegate;
    
#pragma mark - IBOutlets
    IBOutlet UITextField * textFieldIpAddress;
}

#pragma mark - Properties
@property (nonatomic, readonly) id<PresentActionDelegate> delegate;

#pragma mark - Methods
- (id) initWithDelegate:(id<PresentActionDelegate>)delegate;

#pragma mark - IBActions


@end
