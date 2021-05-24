import requests
url = 'http://localhost:8080/api/FileUploading/UploadFile/Hybrid'
files = [('file', open('Hybrid3trainFile.csv', 'rb')), ('file', open('testFile.csv', 'rb'))]
params = {'type': 'hybrid'}

r = requests.post(url, params=params,  files=files)

r.text
print(r.text)
 
