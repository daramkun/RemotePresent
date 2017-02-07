//
//  AppDelegate.h
//  RemotePackReceiver
//
//  Created by Daramkun on 12. 10. 8..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import <Cocoa/Cocoa.h>

#import "DNPacket.h"
#import "DNSocket.h"

@interface DeviceListItem : NSObject
{
    NSString* deviceName;
    DeviceType deviceType;
    ControllerType controllerType;
    DNAddress * networkIp;
    int networkPort;
    
    unsigned long lastCheckTime;
}

@property (assign) NSString* deviceName;
@property (assign) DeviceType deviceType;
@property (assign) ControllerType controllerType;
@property (retain) DNAddress* networkIp;
@property (assign) int networkPort;
@property (assign) unsigned long lastCheckTime;

@end

@interface AppDelegate : NSObject <NSApplicationDelegate, NSTableViewDataSource,
    NSWindowDelegate>
{
    /// Main Window's Outlets ///
    IBOutlet NSTableView *m_tableView;
    IBOutlet NSButton * m_connButton;
    IBOutlet NSPopUpButton * m_targetButton;
    IBOutlet NSTextField * m_noticeLabel;
    
    /// Main Window's Device list Columns ///
    IBOutlet NSTableColumn * m_deviceNameColumn;
    IBOutlet NSTableColumn * m_deviceTypeColumn;
    IBOutlet NSTableColumn * m_osColumn;
    
    /// Panel Windows ///
    IBOutlet NSPanel *m_optionWindow;
    IBOutlet NSPanel *m_passwordWindow;
    IBOutlet NSPanel *m_screenLaserWindow;
    
    /// Password Window Outlets ///
    IBOutlet NSSecureTextField *m_passwordInput;
    
    /// Preference Window Outlets ///
    IBOutlet NSButton * m_telnetPort;
    
    /// Connection Sockets ///
    DNSocket * m_receiveSocket, * m_listenSocket, * m_commandSocket;
    /// Connection threads ///
    NSThread * m_receiveThread, * m_timeoutThread, * m_acceptThread;
    
    /// Received Devices ///
    NSMutableArray * m_receivedApps;
}

@property (assign) IBOutlet NSWindow *window;

- (IBAction) connectStart:(NSButton *)sender;
- (IBAction) passwordInputted:(id)sender;
- (IBAction) showPreferences:(id)sender;
- (IBAction) donePreferences:(id)sender;
- (IBAction) checkPreferences:(id)sender;

@end
