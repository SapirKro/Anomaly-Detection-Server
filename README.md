# Anomaly Detection Server
This project is a web-based interface for Anomaly Detection Algorithms that we implemented in the previous semester, for that various of consumers can access it.

This project is composed  from 2 parts:
   a. RESTful API that is accessible in the web through a simple HTTP protocol and delivering anomaly-computable functions to a varius of clients.
   b.Web-App that is a web page which is accessible from the browser, tht from him the user canupload a data file and find the anomalies.

The main directories in this project:
App_Data:
   This directory contains test and train CSV files and 2 json files of models.
App_Start:
   This directory contains the configuration files:Bundle, Filter, Route and WebApi.
Content:
   This directory contains CSS bootstrap files.
Controllers:
   This directory contains the controllers.
Models:
   This directory contains the models.

## How to use
Developer that want to work on our app need to have frameworks 4.7.2 and .NET 5.0 SDK or later.  
In addition,make sure the "System.Web.Http.HttpResponseException" doesn't handled by the debugger:  
1.open Exceptions Setting (ctrl + alt + E)   
2.Right click on "Common languge Runtime Exceptions" and add "System.Web.Http.HttpResponseException".  
3.Right click on "System.Web.Http.HttpResponseException"--> add condition: make sure you write "WebApplication13.dll" in the text box and click on "Not Equals".  


UML diagram:  
 [UML.pdf](https://github.com/SapirKro/WebApplication13/blob/master/UML.pdf)  
video of user story 1:  
 [userstory1](https://github.com/SapirKro/Anomaly-Detection-Server/blob/master/userstory1.mp4)  
 video of user story 2:  
[userstory2](https://github.com/SapirKro/Anomaly-Detection-Server/blob/master/userstory2.mp4)
