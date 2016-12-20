import UIKit
import AVFoundation
import Alamofire
import WatchConnectivity

class PlayerViewController: UIViewController, SPTAudioStreamingDelegate {
    var localbpm: Double = 120.0
    var playingtime: Int = 11
    var isplaying: Bool = false
    var firststart: Bool = true
    @IBOutlet weak var rhytm: UILabel!
    @IBOutlet weak var nameOfTrack: UILabel!
    
    let us = UserDefaults.standard
    
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
        getNewTrackNext("120.0", "", completionHandler: { response in
        })
        
        // Do any additional setup after loading the view.
    }
    
    @IBAction func nextTrack(_ sender: Any) {
        playingtime=0
        getNewTrackNext(self.rhytm.text!, "true") {() in
        }
    }
    
    @IBAction func playPause(_ sender: Any) {
        if  isplaying == true {
            isplaying=false
            player?.setIsPlaying(false, callback: nil)
        } else {
            isplaying=true
            player?.setIsPlaying(true, callback: nil)
            if firststart == true {
                playTrack(id: self.nameOfTrack.text!)
                firststart = false
            }
        }
    }


    @IBAction func stop(_ sender: Any) {
        playingtime=0
        isplaying=false
        player?.setIsPlaying(false, callback: nil)
        self.dismiss(animated: true) {
            let params = [
                "bpm":self.localbpm,
                "action": 1,
                "username": "qwz",
                "style": 1
                ] as [String : Any]
            Alamofire.request("http://46.101.198.18/api/sa/qwz", method: .delete, parameters: params,encoding: JSONEncoding.default)
        }
    }

    @IBAction func minusBPM(_ sender: Any) {
        playingtime=0
        let params = [
            "bpm":localbpm - 10.0,
            "action": 1,
            "username": "qwz",
            "style": 1
            ] as [String : Any]

        Alamofire.request("http://46.101.198.18/api/sa/", method: .post, parameters: params,encoding: JSONEncoding.default).responseString { (response) in
            self.playTrack(id: response.result.value!)//передает нормально id
                        self.nameOfTrack.text = response.result.value!
        }
        localbpm-=10
        self.rhytm.text! = String(localbpm)
    }
    
    @IBAction func plusBPM(_ sender: Any) {
        playingtime=0
        let params = [
            "bpm": localbpm + 10.0,
            "action": 1,
            "username": "qwz",
            "style": 1
            ] as [String : Any]

        Alamofire.request("http://46.101.198.18/api/sa/", method: .post, parameters: params,encoding: JSONEncoding.default).responseString { (response) in
            self.playTrack(id: response.result.value!)//передает нормально id
                        self.nameOfTrack.text = response.result.value!
        }
        localbpm+=10
        self.rhytm.text! = String(localbpm)
        print(localbpm)
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
        if playingtime>2
        {
            if (abs(Double(rhytm)! - Double(localbpm)) > 5)
        {
        let params = [
            "bpm":Double(rhytm) ?? 0,
            "action": 1,
            "username": "qwz",
            "style": 1
        ] as [String : Any]

        Alamofire.request("http://46.101.198.18/api/sa/", method: .post, parameters: params,encoding: JSONEncoding.default).responseString { (response) in
            self.playTrack(id: response.result.value!)//передает нормально id
            self.nameOfTrack.text = response.result.value!
            }
        localbpm = Double(rhytm)!
            }
        playingtime = 0
        }
        else{
            playingtime+=1}
    }
    func getNewTrackNext(_ rhytm: String, _ now: String, completionHandler: () -> ()) {
        
            let params = [
                "bpm":Double(rhytm) ?? 0,
                "action": 1,
                "username": "qwz",
                "style": 1
                ] as [String : Any]
            
            Alamofire.request("http://46.101.198.18/api/sa/", method: .post, parameters: params,encoding: JSONEncoding.default).responseString { (response) in
                self.playTrack(id: response.result.value!)//передает нормально id
                self.nameOfTrack.text = response.result.value!
            }
            localbpm = Double(rhytm)!
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
        self.player?.playSpotifyURI("spotify:track:"+id, startingWith: 0, startingWithPosition: 10, callback: {(error: Error?) -> Void in
            if (error != nil) {
                print("I'm failed to play: \(error)")
                return
            } else {
                print("I'm playing")
                self.isplaying = true
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
        print(message["pulseValue"] as? Double ?? 0.0)
        //Use this to update the UI instantaneously (otherwise, takes a little while)
        
        DispatchQueue.main.async {
            self.rhytm.text = message["pulseValue"] as! String?
            
            self.getNewTrack(message["pulseValue"] as! String, "", completionHandler: { response in
            })
            
//            if let pulseValue = message["pulseValue"] as? Double {
//                if self.us.value(forKey: "pulse") != nil{
//                if abs((self.us.value(forKey: "pulse") as! Double) - pulseValue) > 5.0 {
//                self.getNewTrack("\(pulseValue)", "lihj") {() in
//                    
//                }
//                self.us.set(pulseValue, forKey: "pulse")
//                }
//                } else {
//                    self.getNewTrack("\(pulseValue)", "lihj") {() in                        
//                    }
//
//                }
//            }
            
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
