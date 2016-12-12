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
    with io.open('data.json', encoding='utf8') as data_file:    
        testedtraks = json.load(data_file)
    print(datetime.datetime.now())
    data = auth.SPsearch()
    fulldata=auth.SPgetinfo(data)
    print(datetime.datetime.now())
    database.fillDB(fulldata)
    print(datetime.datetime.now())
    with io.open('data.json', 'w', encoding='utf8') as f:
        json.dump(fulldata, f, ensure_ascii=False)
    f.close()