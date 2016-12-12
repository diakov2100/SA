from flask import Flask

app = Flask(__name__)

@app.route("/auth/", methods=['GET', 'POST'])
def handle_auth():
    return "Success"

app.run(port=8071, debug=True)
app.config['SERVER_NAME'] = ''