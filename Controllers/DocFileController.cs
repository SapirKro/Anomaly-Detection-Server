using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication13.myDetectorServer;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.IO;
namespace WebApplication13.Controllers
{
    public struct numberOfModels
    {
        public static int num;
        
    }
  
    public class DocFileController : ApiController
    {
        private List<String> mydocfiles = new List<string>();
        DetectorServer server;
      

        /// GET csv files from the home page.the users upload the files though the homepage and the function save them in 
        ///the project location.
        ////the full path for the files saved in mydocfiles

        public HttpResponseMessage Post()
        {
            
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                System.Console.WriteLine("You received the call!");
                string trainFileString = "trainFile";
                string testFileDString = "testFile";
                string name;
                string AlgoType = HttpContext.Current.Request.Form["cars"];
                numberOfModels.num++;
                foreach (string file in httpRequest.Files)
                {
                   
                    var postedFile = httpRequest.Files[file];
                    name = postedFile.FileName;
                    if (file == trainFileString)
                    {
                        name =trainFileString + AlgoType + numberOfModels.num + name;
                       
                    }
                    if (file == testFileDString)
                    {
                        name = testFileDString + AlgoType + numberOfModels.num+ name;

                    }
                    var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + name);
                        postedFile.SaveAs(filePath);
                        mydocfiles.Add(filePath);
                }
                string jsonName = "Model_" + numberOfModels.num + ".json";
                var serverUploadPath = HttpContext.Current.Server.MapPath("~/App_Data/"+ jsonName);
                String trainFilePath = mydocfiles[0];
                String testFilePath = mydocfiles[1];
                AlgoType = AlgoType.ToLower();
                result = Request.CreateResponse(HttpStatusCode.Created, mydocfiles);
                var response = Request.CreateResponse(HttpStatusCode.Moved);
                response.Headers.Location = new Uri("http://localhost:9876/");

                server = new DetectorServer(trainFilePath, testFilePath, AlgoType, serverUploadPath);
             server.Serialize();
                Console.WriteLine("done");
                return response;
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return result;
        }

    }
}

