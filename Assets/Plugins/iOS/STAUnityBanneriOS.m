//  Unity Plugin 2.0
//  Created by StartApp on 04/11/18.
//  Copyright (c) 2018 StartApp. All rights reserved.



#import "STAUnityBanneriOS.h"
#import "STABannerSize.h"

static NSMutableDictionary<NSString*, STAUnityBanneriOS*>* _sAds;

@interface STAUnityBanneriOS ()

@property(nonatomic, nonnull) STABannerView* startAppBanner;
@property(nonatomic, copy, nonnull) NSString* delegateName;
@property(nonatomic) STAAdOrigin position;

@end

@implementation STAUnityBanneriOS

+ (void)updateWithName:(NSString*)name position:(STAAdOrigin)pos tag:(NSString*)tag {
    if (_sAds == nil) {
        _sAds = [[NSMutableDictionary alloc] init];
    }
    
    STAUnityBanneriOS* banner = _sAds[name];
    if (banner == nil) {
        STAUnityBanneriOS* ad = [[STAUnityBanneriOS alloc] initWithDelegateName:name position:pos tag:tag];
        _sAds[name] = ad;
        return;
    }
    
    [banner setPosition:pos];
    
    if (tag != nil) {
        [banner setTag:tag];
    }
    
    [banner show];
}

- (instancetype)initWithDelegateName:(NSString*)name position:(STAAdOrigin)pos tag:(NSString*)tag {
    if (self = [super init]) {
        self.position = pos;
        UIView* rootView = [self.class topViewControllerWithRootViewController:[UIApplication sharedApplication].keyWindow.rootViewController].view;
        
        if (tag == nil) {
            self.startAppBanner = [[STABannerView alloc] initWithSize:STA_AutoAdSize autoOrigin:pos withView:rootView withDelegate:self];
        } else {
            self.startAppBanner = [[STABannerView alloc] initWithSize:STA_AutoAdSize autoOrigin:pos withView:rootView withDelegate:self withAdTag:tag];
        }
        
        [rootView addSubview:self.startAppBanner];
        self.delegateName = name;
    }
    return self;
}

- (void)setTag:(NSString*)tag {
    [self.startAppBanner setSTABannerAdTag:tag];
}

- (void)setPosition:(STAAdOrigin)pos {
    _position = pos;
    [self.startAppBanner setSTAAutoOrigin:pos];
}

- (void)loadAd {
    [self.startAppBanner loadAd];
}

- (void)hide {
    [self.startAppBanner hideBanner];
}

- (void)show {
    [self.startAppBanner showBanner];
}

- (BOOL)isVisibleInPosition:(STAAdOrigin)pos {
    return [self.startAppBanner isVisible] && self.position == pos;
}

- (void)removeFromViewTree {
    [self.startAppBanner removeFromSuperview];
}

+ (void)removeObjectByName:(NSString*)name {
    [_sAds[name] removeFromViewTree];
    [_sAds removeObjectForKey:name];
}

+ (UIViewController*)topViewControllerWithRootViewController:(UIViewController*)rootViewController {
    UIViewController* presentedVC = rootViewController.presentedViewController;
    NSString* presentedVCClassName = NSStringFromClass(presentedVC.class);
    if ([rootViewController isKindOfClass:[UITabBarController class]]) {
        UITabBarController* tabBarController = (UITabBarController*)rootViewController;
        return [self topViewControllerWithRootViewController:tabBarController.selectedViewController];
    } else if ([rootViewController isKindOfClass:[UINavigationController class]]) {
        UINavigationController* navigationController = (UINavigationController*)rootViewController;
        return [self topViewControllerWithRootViewController:navigationController.visibleViewController];
    } else if (presentedVC && ![presentedVCClassName hasPrefix:@"STA"]) {
        return [self topViewControllerWithRootViewController:presentedVC];
    } else {
        return rootViewController;
    }
}

// ======================================================================

- (void)didDisplayBannerAd:(STABannerView*)banner {
    UnitySendMessage(self.delegateName.UTF8String, "OnDidShowBanner", "");
}

- (void)failedLoadBannerAd:(STABannerView*)banner withError:(NSError*)error {
    UnitySendMessage(self.delegateName.UTF8String, "OnFailedLoadBanner", error.localizedDescription.UTF8String);
}

- (void)didClickBannerAd:(STABannerView*)banner {
    UnitySendMessage(self.delegateName.UTF8String, "OnDidClickBanner", "");
}

// =======================================================================

void sta_addBanner(const char* gameObjectName, int position) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    [STAUnityBanneriOS updateWithName:key position:position tag:nil];
}

void sta_addBannerWithTag(const char* gameObjectName, int position, const char* tag) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    NSString* adTag = [NSString stringWithUTF8String:tag];
    [STAUnityBanneriOS updateWithName:key position:position tag:adTag];
}

void sta_preloadBanner(const char* gameObjectName) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    [_sAds[key] loadAd];
}

void sta_hideBanner(const char* gameObjectName) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    [_sAds[key] hide];
}

BOOL sta_isShownInPosition(const char* gameObjectName, int pos) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    return [_sAds[key] isVisibleInPosition:(STAAdOrigin)pos];
}

void sta_removeBannerObject(const char* gameObjectName) {
    NSString* key = [NSString stringWithUTF8String:gameObjectName];
    [STAUnityBanneriOS removeObjectByName:key];
}

@end
