import spotipy
import sys
import spotipy.util as util
import os

os.environ['SPOTIPY_CLIENT_ID'] = "94a636ebaa3f46d1af9c8dcf66858daf"
os.environ['SPOTIPY_CLIENT_SECRET'] = "8b057cb6a70641969d3a5b706dc501d9"
os.environ['SPOTIPY_REDIRECT_URI'] = "https://65d0c0ea.ngrok.io/auth/"


username = 'diakov111'
def SPsearch():
    token = util.prompt_for_user_token(username)

    if token:
        data = []
        search_str = 'sport'

        sp = spotipy.Spotify(auth=token)
        result = sp.search(search_str, 15, 0, 'playlist')

        for playlist in result['playlists']['items']:
          print(playlist['name'].encode("utf-8"))
          results = sp.user_playlist(playlist['owner']['id'], playlist['id'], fields="tracks,next")
          tracks = results['tracks']
          data.append(tracks['items'])
        return data
    else:
        print("Can't get token for", username)

def SPgetinfo(data):
    token = util.prompt_for_user_token(username)
    fulldata = []
    
    if token:
        sp = spotipy.Spotify(auth=token)
        for namelist in data:
            for track in namelist:
                if track['track']['id'] not in fulldata:
                    trackdata = dict()
                    trackdata['trackid'] = track['track']['id']
                    trackdata['name'] = track['track']['name']
                    trackdata['duration']= track['track']['duration_ms']
                    trackdata['played']=0
                    trackdata['skiped']=0
                    trackdata['artists'] = []
                    addartist = dict()
                    for artist in track['track']['artists']:
                        addartist['type'] = artist['type']
                        addartist['name'] = artist['name']
                        addartist['uri'] = artist['uri']
                        addartist['artistid'] = artist['id']
                        addartist['href'] = artist['href']
                        trackdata['artists'].append(addartist)  
                    trackdata['tempo'] = sp.audio_features([track['track']['id']])[0]['tempo']
                    fulldata.append(trackdata)
    return fulldata