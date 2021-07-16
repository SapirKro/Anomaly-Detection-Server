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
using System.Text;

namespace WebApp.Controllers
{


    /// global struct so we can get to the models from other Controllers
      public struct globalsModels
    {

		public static ModelsManager allmodels = new ModelsManager();
		public static int num= allmodels.GetAllModels().Count();

	}

	/// <summary>
	/// this controller has post method to upload files from the homepage
	/// </summary>
	public class UploadHomepageController : ApiController
    {
        private  static List<String> myfiles = new List<string>();
         DetectorServer server;

      
        /// load json file and add every AnomalyObject to the models.(so we can see the anomalys in the homepage's table)
     
        public void LoadJson(string jsonPath)
        {
			///loading all the info to List<AnomalyObject> all 
			List<AnomalyObject> all = new List<AnomalyObject>();
            using (StreamReader sr = File.OpenText(jsonPath))
            {
                all = JsonConvert.DeserializeObject<List<AnomalyObject>>(sr.ReadToEnd());
                
                
            }
			///creating the anomalies in the model
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

		/// <summary>
		/// AnomalyObject as in the server
		/// </summary>
		public class AnomalyObject
        {
            public string description { get; set; }
            public string timeStep { get; set; }
        }


        /// post method. the users upload the csv files though the homepage and the function save them in 
        ///the folder "App_Data" in the project location.
        ////the full path for the files saved in mydocfiles

        public void PostFromHomePage()
        {
            
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
			int index = 0;
	
			///upload the files
			if (httpRequest.Files.Count > 0)
            {
                System.Console.WriteLine("You received the call!");
                string trainFileString = "train";
                string testFileDString = "test";
                string name;
                string AlgoType = HttpContext.Current.Request.Form["AlgorithemType"];
			
					/////if submit is pressed without any upload file
					if (httpRequest.Files[0].FileName == "" || httpRequest.Files[1].FileName == "")
					{
						HttpResponseMessage response22 =
		this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "one of the files is missing");
						throw new HttpResponseException(response22);


						///////>>>>>>>>only for testing .delete later<<<<<<<<<
						var serverUploadPath1 = HttpContext.Current.Server.MapPath("~/App_Data/" + "Model_1.json");
						LoadJson(serverUploadPath1);
						var response1 = Request.CreateResponse(HttpStatusCode.Moved);
						response1.Headers.Location = new Uri("http://localhost:8080/");
						///////>>>>>>>>if loop ^^^^^^^^^^ to only for testing .delete later<<<<<<<<<*/
						return;
					}
			


				///
				String trainFilePath="NULL";
				String testFilePath = "NULL";
				globalsModels.num++;
				foreach (string file in httpRequest.Files)
                {
					index++;
					if (index > 2)
					{
						break;
					}
				

					var postedFile = httpRequest.Files[file];

                    name = postedFile.FileName;
					
						if (name.Contains(".csv") == false)
						{

							HttpResponseMessage response22 =
	this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "one of the file isnt CSV file");
							throw new HttpResponseException(response22);


						}
					
					if (file.Contains(trainFileString))
                    {

                        name =trainFileString + globalsModels.num+ AlgoType + name;
						var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + name);
						postedFile.SaveAs(filePath);
						bool isEq1=FilesAreEqual(myfiles, filePath);
						if (isEq1 == false)
						{

							trainFilePath = filePath;
						///postedFile.SaveAs(filePath);
							myfiles.Add(filePath);
						}
						if (isEq1 == true)
						{
							trainFilePath = "NULL";
						}
					}

                    if (file.Contains(testFileDString))
                    {
                        name = testFileDString + globalsModels.num+ AlgoType + name;
						var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + name);
						postedFile.SaveAs(filePath);
						
						bool isEq2 = FilesAreEqual(myfiles, filePath);
						if (isEq2 == false)
						{
							testFilePath = filePath;
						///	postedFile.SaveAs(filePath);
							myfiles.Add(filePath);
						}
						if (isEq2 == true)
						{
							testFilePath = "NULL";
						}

					}
                 
                }

			
				///if we didnt get the test and train file,we decrease the number of model and throw error messege
				if ( (testFilePath.Equals("NULL"))|| (trainFilePath.Equals("NULL")  ))
				{
					
						globalsModels.num--;
					HttpResponseMessage response22 =
this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "one of the files is missing or use already upload this files");
					throw new HttpResponseException(response22);


				}
                ////sending the files to the server
                string jsonName = "Model_" + globalsModels.num + ".json";
                var serverUploadPath = HttpContext.Current.Server.MapPath("~/App_Data/"+ jsonName);
       
                AlgoType = AlgoType.ToLower();
				
					server = new DetectorServer(trainFilePath, testFilePath, AlgoType, serverUploadPath);
					server.Serialize();
					Console.WriteLine("done");
					LoadJson(serverUploadPath);
				
              /*  result = Request.CreateResponse(HttpStatusCode.Created, myfiles);
                ///redirecting back to the homepage
                var response = Request.CreateResponse(HttpStatusCode.Moved);
                response.Headers.Location = new Uri("http://localhost:8080/");
				///return response;*/
				return;
			}
			else
            {
				//there isnt any file
				HttpResponseMessage response22 =
this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "there isnt any file to upload");
				throw new HttpResponseException(response22);

			
            }
    
			return;
		}



		const int BYTES_TO_READ = sizeof(Int64);

		 bool FilesAreEqual(List<String> myfiles, string path)
		{
			int i = 0;
			while (i< myfiles.Count()) {
					byte[] bytes1 = Encoding.Convert(Encoding.GetEncoding(1252), Encoding.ASCII, Encoding.GetEncoding(1252).GetBytes(File.ReadAllText(path)));
					byte[] bytes2 = Encoding.Convert(Encoding.GetEncoding(1252), Encoding.ASCII, Encoding.GetEncoding(1252).GetBytes(File.ReadAllText(myfiles.ElementAt(i))));

					if (Encoding.ASCII.GetChars(bytes1).SequenceEqual(Encoding.ASCII.GetChars(bytes2)))
					{
						//matched! 
						Console.WriteLine("match");
						return true;
					}
					else
					{
						Console.WriteLine("not match");

					}
				i++;
				}
			return false;
		}
	}
}

