
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
                self.performSegue(withIdentifier: "toPlayer", sender: self)
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
    
    
    
   
}

