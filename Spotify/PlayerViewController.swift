

import UIKit
import AVFoundation
import Alamofire
import WatchConnectivity

class PlayerViewController: UIViewController, SPTAudioStreamingDelegate {
    
    @IBOutlet weak var rhytm: UITextField!
    @IBOutlet weak var nameOfTrack: UILabel!
    
    fileprivate let session: WCSession? = WCSession.isSupported() ? WCSession.default() : nil
    
    required init?(coder aDecoder: NSCoder) {
        super.init(coder: aDecoder)
        
        configureWCSession()
    }
    
    override init(nibName nibNameOrNil: String?, bundle nibBundleOrNil: Bundle?) {
        super.init(nibName: nibNameOrNil, bundle: nibBundleOrNil)
        
        configureWCSession()
    }
    
    fileprivate func configureWCSession() {
        session?.delegate = self
        session?.activate()
    }
    
    
    let player = SPTAudioStreamingController.sharedInstance()
    var spAuth = SPTAuth.defaultInstance()
    var username = SPTAuth.defaultInstance().session?.canonicalUsername
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        player!.delegate = self
        player!.login(withAccessToken: spAuth?.session.accessToken)
        print("Username is \(spAuth?.session.canonicalUsername)")
        
        // Do any additional setup after loading the view.
    }
    
    @IBAction func nextTrack(_ sender: Any) {
        getNewTrack(self.rhytm.text!, "true") {() in
        }
    }
    
    @IBAction func playPause(_ sender: Any) {
        player!.setIsPlaying(!(player?.playbackState.isPlaying)!, callback: nil)
    }
    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
    
    func activateAudioSession() {
        try! AVAudioSession.sharedInstance().setCategory(AVAudioSessionCategoryPlayback)
        try! AVAudioSession.sharedInstance().setActive(true)
    }
    
    func deactivateAudioSession() {
        try! AVAudioSession.sharedInstance().setActive(false)
    }
    
    func getNewTrack(_ rhytm: String, _ now: String, completionHandler: () -> ()) {
        
        print("Link is http://scherbakov.top/hj/getTrack/?bpm=\(rhytm)&now=\(now)&user=\(username!)")
        
        Alamofire.request("http://scherbakov.top/hj/getTrack/?bpm=\(rhytm)&now=\(now)&user=\(username!)", method: .get, headers: nil).response { response in
            
            print("Request: \(response.request)")
            print("Response: \(response.response)")
            print("Error: \(response.error)")
            
            if let data = response.data, let utf8Text = String(data: data, encoding: .utf8) {
                let result = self.convertStringToDictionary(text: utf8Text)! as [String:AnyObject]
                if let id = result["id"] as? String {
                    self.nameOfTrack.text = result["name"] as? String
                    self.playTrack(id: id)
                }
                
            }
        }
        
    }
    
    func convertStringToDictionary(text: String) -> [String:AnyObject]? {
        if let data = text.data(using: String.Encoding.utf8) {
            do {
                return try JSONSerialization.jsonObject(with: data, options: []) as? [String:AnyObject]
            } catch let error as NSError {
                print(error)
            }
        }
        return nil
    }
    
    func playTrack(id: String) {
        activateAudioSession()
        print("[playTrack], Id is \(id)")
        self.player?.playSpotifyURI("spotify:track:\(id)", startingWith: 0, startingWithPosition: 10, callback: {(error: Error?) -> Void in
            if (error != nil) {
                print("I'm failed to play: \(error)")
                return
            } else {
                print("I'm playing")
            }
        })
    }
    
    
    /*
     // MARK: - Navigation
     
     // In a storyboard-based application, you will often want to do a little preparation before navigation
     override func prepare(for segue: UIStoryboardSegue, sender: Any?) {
     // Get the new view controller using segue.destinationViewController.
     // Pass the selected object to the new view controller.
     }
     */
    
}

extension PlayerViewController: WCSessionDelegate {
    /** Called when all delegate callbacks for the previously selected watch has occurred. The session can be re-activated for the now selected watch using activateSession. */
    @available(iOS 9.3, *)
    public func sessionDidDeactivate(_ session: WCSession) {
        //Dummy Implementation
    }
    
    /** Called when the session can no longer be used to modify or add any new transfers and, all interactive messages will be cancelled, but delegate callbacks for background transfers can still occur. This will happen when the selected watch is being changed. */
    @available(iOS 9.3, *)
    public func sessionDidBecomeInactive(_ session: WCSession) {
        //Dummy Implementation
    }
    
    /** Called when the session has completed activation. If session state is WCSessionActivationStateNotActivated there will be an error with more details. */
    @available(iOS 9.3, *)
    public func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        //Dummy Implementation
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any], replyHandler: @escaping ([String : Any]) -> Void) {
        
        //Use this to update the UI instantaneously (otherwise, takes a little while)
        DispatchQueue.main.async {
            if let pulseValue = message["pulseValue"] as? String {
                self.getNewTrack(pulseValue, "true") {() in
                    
                }
            }
            
            if message["next"] != nil {
                self.getNewTrack(self.rhytm.text!, "true") {() in
                }
            }
            
            if message["play"] != nil {
                self.player!.setIsPlaying(!(self.player?.playbackState.isPlaying)!, callback: nil)
            }
        }
    }
}
