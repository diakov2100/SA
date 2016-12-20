import os
import datetime
import spotipy.util as util
import spotipy
import spotify_requests
import json
import string
import filling_db
import io

if __name__ == '__main__':
    #with io.open('data.json', encoding='utf8') as data_file:    
        #testedtraks = json.load(data_file)['testedtracks']
    print(datetime.datetime.now())
    with io.open('keywords.json') as data_file:    
        preset = json.load(data_file)
    data = spotify_requests.SPsearch(preset)
    fulldata=spotify_requests.SPgetinfo(data)
    print(datetime.datetime.now())
    filling_db.fillDB(fulldata)
    print(datetime.datetime.now())
    #with io.open('data.json', 'w', encoding='utf8') as f:
        #json.dump(fulldata, f, ensure_ascii=False)
    #f.close()