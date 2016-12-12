import pymongo

client = pymongo.MongoClient("mongodb://max:123456@46.101.198.18/SA_db")
db = client.SA_db

def fillDB(fulldata):   
    collection= db["spotify_db"]
    for track in fulldata:
        collection.insert(track)