/*
 Copyright 2019 StartApp Inc
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
 http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
 */



#import "STAUnityAdiOS.h"

static NSMutableDictionary<NSString*, STAUnityAdiOS*>* _sAds;

@interface STAUnityAdiOS ()

@property(nonatomic, nonnull) STAStartAppAd* startAppAd;
@property(nonatomic, copy, nonnull) NSString* delegateName;

@end

@implementation STAUnityAdiOS

+ (instancetype)createWithName:(NSString*)name {
    STAUnityAdiOS* ad = [[STAUnityAdiOS alloc] initWithDelegateName:name];

    if (_sAds == nil) {
        _sAds = [[NSMutableDictionary alloc] init];
    }

    _sAds[name] = ad;
    return ad;
}

- (instancetype)initWithDelegateName:(NSString*)name {
    if (self = [super init]) {
        self.startAppAd = [[STAStartAppAd alloc] init];
        self.delegateName = name;
    }
    return self;
}

- (void)loadAd {
    [self.startAppAd loadAdWithDelegate:self];
}

- (void)loadRewardedVideoAd {
    [self.startAppAd loadRewardedVideoAdWithDelegate:self];
}

- (void)loadVideoAd {
    [self.startAppAd loadVideoAdWithDelegate:self];
}

- (void)showAd {
    [self.startAppAd showAd];
}

- (void)showAdWithAdTag:(NSString*)tag {
    [self.startAppAd showAdWithAdTag:tag];
}

- (BOOL)shouldAutoRotate {
    return self.startAppAd.STAShouldAutoRotate;
}

- (BOOL)isReady {
    return self.startAppAd.isReady;
}

// ======================================================================

- (void)didLoadAd:(STAAbstractAd*)ad {
    UnitySendMessage(self.delegateName.UTF8String, "OnDidLoadAd", "");
}

- (void)failedLoadAd:(STAAbstractAd*)ad withError:(NSError*)error {
    UnitySendMessage(self.delegateName.UTF8String, "OnFailedLoadAd", error.localizedDescription.UTF8String);
}

- (void)didShowAd:(STAAbstractAd*)ad {
    UnitySendMessage(self.delegateName.UTF8String, "OnDidShowAd", "");
}

- (void)failedShowAd:(STAAbstractAd*)ad withError:(NSError*)error {
}

- (void)didCloseAd:(STAAbstractAd*)ad {
    UnitySendMessage(self.delegateName.UTF8String, "OnDidCloseAd", "");
}

- (void)didClickAd:(STAAbstractAd*)ad {
    UnitySendMessage(self.delegateName.UTF8String, "OnDidClickAd", "");
}

- (void)didCloseInAppStore:(STAAbstractAd*)ad {
}

- (void)didCompleteVideo:(STAAbstractAd*)ad {
    UnitySendMessage(self.delegateName.UTF8String, "OnDidCompleteVideo", "");
}

// =======================================================================

void sta_loadAd(const char* gameObjectName) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    STAUnityAdiOS* ad = [STAUnityAdiOS createWithName:key];
    [ad loadAd];
}

void sta_showAd(const char* gameObjectName) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    [_sAds[key] showAd];
}

void sta_showAdWithTag(const char* gameObjectName, const char* tag) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    NSString* adTag = [NSString stringWithUTF8String:tag];
    [_sAds[key] showAdWithAdTag:adTag];
}

void sta_loadRewardedVideoAd(const char* gameObjectName) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    STAUnityAdiOS* ad = [STAUnityAdiOS createWithName:key];
    [ad loadRewardedVideoAd];
}
    
void sta_loadVideoAd(const char* gameObjectName) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    STAUnityAdiOS* ad = [STAUnityAdiOS createWithName:key];
    [ad loadVideoAd];
}

BOOL sta_isReady(const char* gameObjectName) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    return _sAds[key].isReady;
}

void sta_removeAdObject(const char* gameObjectName) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    [_sAds removeObjectForKey:key];
}

@end
