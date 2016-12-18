import UIKit
import SafariServices

class ViewController: UIViewController, SFSafariViewControllerDelegate, SPTAudioStreamingPlaybackDelegate, SPTAudioStreamingDelegate {

    let notificationName = Notification.Name("sessionUpdated")
    var authViewController = UIViewController()
//    let kTokenSwapURL = "http://localhost:1234/swap"
//    let kTokenRefreshURL = "http://localhost:1234/refresh"
    
    var spAuth = SPTAuth.defaultInstance()
    
    @IBAction func loginWithSpotify(sender: AnyObject) {
        
        if (spAuth?.session == nil) {
            self.openLoginPage()
        } else {
            if (spAuth?.session.isValid())! {
                print("I'm work \(spAuth?.session.isValid())")
                self.performSegue(withIdentifier: "toNew", sender: self)
            } else {
               self.openLoginPage()
            }
        }
    }
    
    override func viewDidAppear(_ animated: Bool) {

        NotificationCenter.default.addObserver(self, selector: #selector(ViewController.setUpPlayer), name: notificationName, object: nil)
    }
    
    override func viewDidDisappear(_ animated: Bool) {
        NotificationCenter.default.removeObserver(self, name: notificationName, object: nil)
    }
    
    func setUpPlayer() {
        let player = SPTAudioStreamingController.sharedInstance()
        try! player!.start(withClientId: spAuth?.clientID, audioController: nil, allowCaching: true)
        player!.playbackDelegate = self
        player!.diskCache = SPTDiskCache(capacity: 1028 * 1024 * 64)
        self.performSegue(withIdentifier: "toNew", sender: self)
    }
    
    func openLoginPage() {
        let auth = SPTAuth.defaultInstance()
        UIApplication.shared.open((auth?.spotifyWebAuthenticationURL())!, options: [:], completionHandler: nil)
    }
}

