

import WatchKit
import Foundation
import WatchConnectivity


class PlayerInterfaceController: WKInterfaceController, WCSessionDelegate {

    fileprivate let session : WCSession? = WCSession.isSupported() ? WCSession.default() : nil
    
    override init() {
        super.init()
        
        session?.delegate = self
        session?.activate()
    }
    
    @available(watchOS 2.2, *)
    public func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {    }
    
    override func awake(withContext context: Any?) {
        super.awake(withContext: context)
    }

    override func willActivate() {
        // This method is called when watch view controller is about to be visible to user
        super.willActivate()
    }

    override func didDeactivate() {
        // This method is called when watch view controller is no longer visible
        super.didDeactivate()
    }
    
    @IBAction func nextTrack() {
       passValue(pulse: "next", key: "next")
    }
    
    @IBAction func playPause() {
       passValue(pulse: "play", key: "play")
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {
        DispatchQueue.main.sync {
            if let pulseValue = message["pulseValue"] as? Int {
                print(pulseValue)
            }
        }
    }
    
    func passValue(pulse: String, key: String) {
        let applicationData = [key : pulse]
        
        if let session = session, session.isReachable {
            session.sendMessage(applicationData, replyHandler: { (replyData) in
                print(replyData)
            }, errorHandler: { (error) in
                print(error)
            })
        } else {
            print("iPhone is not connected")
        }
    }
}
