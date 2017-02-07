//
//  AppDelegate.m
//  RemotePackReceiver
//
//  Created by Daramkun on 12. 10. 8..
//  Copyright (c) 2012년 Daramkun. All rights reserved.
//

#import "AppDelegate.h"
#import <CoreServices/CoreServices.h>
#import "DNKeySender.h"
#import "DNGlobal.h"

CGEventSourceRef g_eventSource;

unsigned long GetTickCount()
{
    return TickCount();
    //return (unsigned long)[NSNumber numberWithLong:[[NSDate date]
    //                                                timeIntervalSince1970] * 1000];
}

#pragma mark - DeviceListItem

@implementation DeviceListItem

#pragma mark - Property synthesizes
@synthesize deviceName;
@synthesize deviceType;
@synthesize controllerType;
@synthesize networkIp;
@synthesize networkPort;
@synthesize lastCheckTime;

@end

#pragma mark AppDelegate

@implementation AppDelegate

#pragma mark - Property synthesizes

- (int) BROADCAST_PORT
{
    return 25567;
}

#pragma mark - Constructor and Destructor

- (void)dealloc
{
    [super dealloc];
}

#pragma mark - NSApplicationDelegate methods

- (void)applicationDidFinishLaunching:(NSNotification *)aNotification
{
    // Insert code here to initialize your application
    m_receivedApps = [[NSMutableArray alloc] init];
    
    m_listenSocket = [[DNSocket alloc] initWithProtocol:DNSocketTypeTcp];
    [m_listenSocket bindPort:self.BROADCAST_PORT];
    [m_listenSocket listen:1];
    
    m_receiveSocket = [[DNSocket alloc] initWithProtocol:DNSocketTypeUdp];
    [m_receiveSocket bindPort:self.BROADCAST_PORT];
    [m_receiveSocket broadcastOn];
    
    m_receiveThread = [[NSThread alloc] initWithTarget:self
                                              selector:@selector(receiveApps:)
                                                 object:nil];
    m_timeoutThread = [[NSThread alloc] initWithTarget:self
                                              selector:@selector(timeoutCheck:)
                                                object:nil];
    [m_receiveThread start];
    [m_timeoutThread start];
    
    [self startAccept:nil];
}

- (void)applicationWillTerminate:(NSNotification *)notification
{
    [m_timeoutThread cancel];
    [m_timeoutThread release];
    
    [m_receiveThread cancel];
    [m_receiveThread release];
    
    [m_receivedApps release];
}

#pragma mark - NSWindowdelegate methods

- (void)windowWillClose:(NSNotification *)notification
{
    [NSApp terminate:self];
}

#pragma mark - Private object methods

- (void) addToTable:(NSString*) deviceName
         deviceType:(ControllerType)deviceType
           platform:(DeviceType)platform
                 ip:(DNAddress*)ip
               port:(int)port
{
    DeviceListItem * item = [[DeviceListItem alloc] init];
    item.deviceName = deviceName;
    item.controllerType = deviceType;
    item.deviceType = platform;
    item.networkIp = ip;
    item.networkPort = port;
    item.lastCheckTime = GetTickCount();
    
    BOOL isAvailable = NO;
    for(int i = 0; i < m_receivedApps.count; i++)
    {
        DeviceListItem* item = [m_receivedApps objectAtIndex:i];
        if([item.networkIp isEqual:ip])
        {
            if(item.deviceName != deviceName)
            {
                item.deviceName = deviceName;
                [m_tableView reloadData];
            }
            else
                item.lastCheckTime = GetTickCount();
            item.networkPort = port;
            isAvailable = YES;
        }
    }
    
    if(isAvailable) return;
    
    [m_receivedApps addObject:item];
    [self performSelectorOnMainThread:@selector(reloadDataOfTableView:) withObject:nil waitUntilDone:YES];
}

- (void) changeConnectState:(BOOL)state
{
    switch(state)
    {
        case YES:
            m_tableView.enabled = NO;
            m_connButton.enabled = NO;
            m_noticeLabel.stringValue = @"Succeed connection to app.";
            break;
        case NO:
            m_tableView.enabled = YES;
            m_connButton.enabled = YES;
            
            m_noticeLabel.stringValue = @"Disconnected from app.";
            
            [self performSelectorOnMainThread:@selector(startAccept:)
                                   withObject:nil
                                waitUntilDone:NO];
            break;
    }
}

#pragma mark - IBActions and perform methods

- (void) startAccept:(id)object
{
    m_acceptThread = [[NSThread alloc] initWithTarget:self selector:@selector(acceptAction:) object:nil];
    [m_acceptThread start];
}

- (void) acceptAction:(id)object
{
    NSAutoreleasePool * pool = [[NSAutoreleasePool alloc] init];
    NativePacket nativePacket;
    
    m_commandSocket = [m_listenSocket accept];
    if(m_commandSocket != nil)
    {
        [self changeConnectState:YES];
        
        while(![m_acceptThread isCancelled])
        {
            if([m_commandSocket recvFromSocket:&nativePacket
                                     andLength:sizeof(nativePacket)] == 0)
            {
                [self changeConnectState:NO];
                break;
            }
            else
            {
                DNKeySender * keySender = [[DNKeySender alloc] init];
                switch(nativePacket.packetType)
                {
                    case PacketTypeCommand:
                        switch (nativePacket.packetContent.command.command)
                        {
                            case CommandTypePresentationStart:
                                if([m_targetButton.titleOfSelectedItem isEqualToString:@"Microsoft Office"])
                                {
                                    [keySender sendModifyKey:kVK_Command isKeyDown:YES];
                                    [keySender sendKeyOnce:kVK_Return];
                                    [keySender sendModifyKey:kVK_Command isKeyDown:NO];
                                }
                                else if([m_targetButton.titleOfSelectedItem isEqualToString:@"Keynote"])
                                {
                                    [keySender sendModifyKey:kVK_Command isKeyDown:YES];
                                    [keySender sendModifyKey:kVK_Option isKeyDown:YES];
                                    [keySender sendCharacterKeyOnce:@"p"];
                                    [keySender sendModifyKey:kVK_Option isKeyDown:NO];
                                    [keySender sendModifyKey:kVK_Command isKeyDown:NO];
                                }
                                else if([m_targetButton.titleOfSelectedItem isEqualToString:@"Preview"])
                                {
                                    [keySender sendModifyKey:kVK_Command isKeyDown:YES];
                                    [keySender sendModifyKey:kVK_Shift isKeyDown:YES];
                                    [keySender sendCharacterKeyOnce:@"f"];
                                    [keySender sendModifyKey:kVK_Shift isKeyDown:NO];
                                    [keySender sendModifyKey:kVK_Command isKeyDown:NO];
                                }
                                break;
                            case CommandTypeSlideLeft:
                                [keySender sendKeyOnce:kVK_LeftArrow];
                                break;
                            case CommandTypeSlideRight:
                                [keySender sendKeyOnce:kVK_RightArrow];
                                break;
                                
                            case CommandTypeScreenLaser:
                                if(nativePacket.packetContent.command.x)
                                {
                                    m_screenLaserWindow.isVisible = YES;
                                    m_screenLaserWindow.level = NSFloatingWindowLevel;
                                }
                                else
                                    m_screenLaserWindow.isVisible = NO;
                                break;
                            case CommandTypeScreenLaserMove:
                                {
                                    
                                }
                                break;
                                
                            default:
                                m_noticeLabel.stringValue = @"Cannot run arrived command.";
                                break;
                        }
                        break;
                    default:
                        break;
                }
                [keySender release];
            }
        }
    }
    else { [self changeConnectState:NO]; }
        
    [m_acceptThread release];
    
    [pool release];
}

- (void) receiveApps:(id)object
{
    NSAutoreleasePool * pool = [[NSAutoreleasePool alloc] init];
    
    NativePacket nativePacket;
    
    while (![m_receiveThread isCancelled])
    {
        DNAddress * addr = [DNAddress broadcastAddress];
        int broadcastPort = self.BROADCAST_PORT;
        [m_receiveSocket recvFromEndPoint:&nativePacket
                                andLength:sizeof(nativePacket)
                               andAddress:addr
                                  andPort:&broadcastPort];
        
        if(nativePacket.errHeader == 255 && nativePacket.errFooter == 255)
        {
            if(nativePacket.packetType == PacketTypeItsMe)
            {
                [self addToTable:[NSString stringWithUTF8String:nativePacket.packetContent.itsMe.deviceName]
                      deviceType:nativePacket.remoteType
                        platform:nativePacket.deviceType
                              ip:addr
                            port:broadcastPort];
            }
        }
        
        [addr release];
        
        usleep(100);
    }
    
    [pool release];
}

- (void) reloadDataOfTableView:(id)sender
{
    [m_tableView reloadData];
}

- (void) timeoutCheck:(id)object
{
    NSAutoreleasePool * pool = [[NSAutoreleasePool alloc] init];
    
    NSMutableArray * remover = [[NSMutableArray alloc] init];
    unsigned long nowTickCount;
    
    while(![m_timeoutThread isCancelled])
    {
        nowTickCount = GetTickCount();
        
        for(int i = 0; i < m_receivedApps.count; i++)
        {
            DeviceListItem * item = [m_receivedApps objectAtIndex:i];
            if((nowTickCount - item.lastCheckTime) >= 4000)
                [remover addObject:item];
        }
        for(int i = 0; i < remover.count; i++)
        {
            DeviceListItem * item = [remover objectAtIndex:i];
            if(item == nil)
            {
                continue;
            }
            [m_receivedApps removeObject:item];
            [item release];
        }
         
        [remover removeAllObjects];
        
        [self performSelectorOnMainThread:@selector(reloadDataOfTableView:) withObject:nil waitUntilDone:NO];
        
        usleep(100);
    }
    
    [remover release];
    
    [pool release];
}

- (IBAction) connectStart:(NSButton *)sender
{
    if(m_tableView.selectedRow < 0)
    {
        NSAlert * alert = [[NSAlert alloc] init];
        [alert addButtonWithTitle:@"OK"];
        alert.messageText = @"Please select the one device.";
        [alert beginSheetModalForWindow:self.window modalDelegate:nil didEndSelector:nil contextInfo:nil];
        alert.alertStyle = NSInformationalAlertStyle;
        [alert autorelease];
        
        return;
    }

    [NSApp runModalForWindow:m_passwordWindow];
}

- (void) connectionFail
{
    NSAlert * alert = [[NSAlert alloc] init];
    [alert addButtonWithTitle:@"OK"];
    alert.messageText = @"Connection failed.";
    [alert beginSheetModalForWindow:self.window modalDelegate:nil didEndSelector:nil contextInfo:nil];
    alert.alertStyle = NSInformationalAlertStyle;
    [alert autorelease];
}

- (IBAction) passwordInputted:(id)sender
{
    DeviceListItem * item = [m_receivedApps objectAtIndex:m_tableView.selectedRow];
    
    DNPacket * packet = [[DNPacket alloc] init];
    [packet organizeToAppPacket:item.deviceName otPassword:m_passwordInput.stringValue];
    NativePacket nativePacket = packet.nativePacket;
    [packet release];
    
    [m_receiveSocket sendToEndPoint:&nativePacket
                          andLength:sizeof(nativePacket)
                         andAddress:item.networkIp//[DNAddress broadcastAddress]
                            andPort:item.networkPort//BROADCAST_PORT
     ];
    
    m_passwordInput.stringValue = @"";
    [m_passwordWindow close];
    [NSApp abortModal];
}

- (IBAction) showPreferences:(id)sender
{
    [NSApp runModalForWindow:m_optionWindow];
}

- (IBAction) donePreferences:(id)sender
{
    [m_optionWindow close];
    [NSApp abortModal];
}

- (IBAction) checkPreferences:(id)sender
{
    if(sender == m_telnetPort)
    {
        
    }
}

#pragma mark - NSTableViewDataSource methods

- (NSInteger)numberOfRowsInTableView:(NSTableView *)tableView
{
    return [m_receivedApps count];
}

- (id)tableView:(NSTableView *)tableView objectValueForTableColumn:(NSTableColumn *)tableColumn row:(NSInteger)row
{
    if(tableColumn == m_deviceNameColumn)
        return ((DeviceListItem*)[m_receivedApps objectAtIndex:row]).deviceName;
    else if(tableColumn == m_osColumn)
        switch(((DeviceListItem*)[m_receivedApps objectAtIndex:row]).deviceType)
        {
            case DeviceTypeiOS: return @"iPhone";
            case DeviceTypeWindowsPhone: return @"Windows Phone";
            case DeviceTypeAndroid: return @"Android";
            case DeviceTypeBada: return @"bada";
            default: return @"Unknown";
        }
    else if(tableColumn == m_deviceTypeColumn)
        switch(((DeviceListItem*)[m_receivedApps objectAtIndex:row]).controllerType)
        {
            case ControllerTypePresent: return @"RemotePresent";
            case ControllerTypeMediaControl: return @"RemoteMediaControl";
            case ControllerTypeGamePad: return @"RemoteGamePad";
            default: return @"Unknown";
        }
    else
        return nil;
}

@end
