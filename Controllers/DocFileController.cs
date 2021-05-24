using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.myDetectorServer;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApp.Models;
using System.IO;
using Newtonsoft.Json;

namespace WebApp.Controllers
{

    /// <summary>
    /// global struct so we can get to the models from other Controllers
    /// </summary>
    public struct globalsModels
    {

		public static ModelsManager allmodels = new ModelsManager();
		public static int num= allmodels.GetAllModels().Count();

	}
	

	public class DocFileController : ApiController
    {
        private List<String> mydocfiles = new List<string>();
         DetectorServer server;

        /// <summary>
        /// load json file and add every AnomalyObject to the models.(so we can see the anomalys in the homepage's table)
        /// </summary>
        /// <param name="jsonPath"></param>
        public void LoadJson(string jsonPath)
        {
            List<AnomalyObject> all = new List<AnomalyObject>();
            using (StreamReader sr = File.OpenText(jsonPath))
            {
                all = JsonConvert.DeserializeObject<List<AnomalyObject>>(sr.ReadToEnd());
                
                
            }
            for (int i = 0; i < all.Count(); i++)
            {
                string des = all[i].description;
                string time = all[i].timeStep;
                int modelid = globalsModels.num;
                Model a = new Model { Id = modelid, Description = des, Time = time, };

                globalsModels.allmodels.AddModel(a);
            }
            Console.WriteLine("done");
        }

        public class AnomalyObject
        {
            public string description { get; set; }
            public string timeStep { get; set; }
        }


        /// post method. the users upload the csv files though the homepage and the function save them in 
        ///the folder "App_Data" in the project location.
        ////the full path for the files saved in mydocfiles

        public HttpResponseMessage Post()
        {
            
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;


            /*
        var serverUploadPath1 = HttpContext.Current.Server.MapPath("~/App_Data/" +"Model_1.json");
                LoadJson(serverUploadPath1);
            var response1 = Request.CreateResponse(HttpStatusCode.Moved);
            response1.Headers.Location = new Uri("http://localhost:8080/");
            return response1;*/


            ///upload the files
            if (httpRequest.Files.Count > 0)
            {
                System.Console.WriteLine("You received the call!");
                string trainFileString = "trainFile";
                string testFileDString = "testFile";
                string name;
                string AlgoType = HttpContext.Current.Request.Form["AlgorithemType"];
                
            
             
                /////if submit is pressed without any upload file
                if (httpRequest.Files[0].FileName == ""|| httpRequest.Files[1].FileName == "")
                {
					HttpResponseMessage response22 =
	this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "you forget to upload one of the files");
					throw new HttpResponseException(response22);

					
					///////>>>>>>>>only for testing .delete later<<<<<<<<<
					var serverUploadPath1 = HttpContext.Current.Server.MapPath("~/App_Data/" + "Model_1.json");
                    LoadJson(serverUploadPath1);
                    var response1 = Request.CreateResponse(HttpStatusCode.Moved);
                    response1.Headers.Location = new Uri("http://localhost:8080/");
                    return response1;
                }
				globalsModels.num++;
				///////>>>>>>>>if loop ^^^^^^^^^^ to only for testing .delete later<<<<<<<<<*/
				///
				String trainFilePath="NULL";
				String testFilePath = "NULL";
				foreach (string file in httpRequest.Files)
                {
                   
                    var postedFile = httpRequest.Files[file];

                    name = postedFile.FileName;
					
					if (file.Contains(trainFileString))
                    {
                        name =trainFileString + AlgoType + globalsModels.num + name;
						var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + name);
						trainFilePath = filePath;
						postedFile.SaveAs(filePath);
						mydocfiles.Add(filePath);

					}

                    if (file.Contains(testFileDString))
                    {
                        name = testFileDString + AlgoType + globalsModels.num+ name;
						var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + name);
						testFilePath = filePath;
						postedFile.SaveAs(filePath);
						mydocfiles.Add(filePath);

					}
                 
                }
                ////sending the files to the server
                string jsonName = "Model_" + globalsModels.num + ".json";
                var serverUploadPath = HttpContext.Current.Server.MapPath("~/App_Data/"+ jsonName);
       
                AlgoType = AlgoType.ToLower();
				
					server = new DetectorServer(trainFilePath, testFilePath, AlgoType, serverUploadPath);
					server.Serialize();
					Console.WriteLine("done");
					LoadJson(serverUploadPath);
				
                result = Request.CreateResponse(HttpStatusCode.Created, mydocfiles);
                ///redirecting back to the homepage
                var response = Request.CreateResponse(HttpStatusCode.Moved);
                response.Headers.Location = new Uri("http://localhost:8080/");
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

