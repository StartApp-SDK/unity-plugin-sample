//
//  STAAbstractAd.h
//  StartAppAdSDK
//
//  Copyright (c) 2013 StartApp. All rights reserved.
//  SDK version 3.11.0

#import <Foundation/Foundation.h>

@class STAAbstractAd;
@protocol STADelegateProtocol <NSObject>
@optional

- (void) didLoadAd:(STAAbstractAd*)ad;
- (void) failedLoadAd:(STAAbstractAd*)ad withError:(NSError *)error;
- (void) didShowAd:(STAAbstractAd*)ad;
- (void) failedShowAd:(STAAbstractAd*)ad withError:(NSError *)error;
- (void) didCloseAd:(STAAbstractAd*)ad;
- (void) didClickAd:(STAAbstractAd*)ad;
- (void) didCloseInAppStore:(STAAbstractAd*)ad;
- (void) didCompleteVideo:(STAAbstractAd*)ad;

@end

@interface STAAbstractAd : NSObject

- (BOOL) isReady;

@end
