import requests
url = 'http://localhost:8080/detect'
files = [('file', open('trainFile.csv', 'rb')), ('file', open('testFile.csv', 'rb'))]
params = {'type': 'hybrid'}

r = requests.post(url, params=params,  files=files)

r.text
print(r.text)
 
