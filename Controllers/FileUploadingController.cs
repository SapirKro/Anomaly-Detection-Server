
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebApp.Models;
using Newtonsoft.Json;
using WebApp.myDetectorServer;

namespace WebApp.Controllers
{

	/// <summary>
	/// FileUploadingController get the files from outside,client send post reqest.
	///FileUploadingController send the file to the server and return json
	/// </summary>
	public class FileUploadingController : ApiController
    {
        private static List<String> myfiles = new List<string>();
		private static DetectorServer server;

		/// fucntions to upload csv files thought post method
	
		[HttpPost]
        [Route("detect")]
        public async Task<object> UploadFiles()
        {
           
           
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/App_Data");
            var provider =
                new MultipartFormDataStreamProvider(root);
			string trainFileString = "trainFile";
			string testFileDString = "testFile";
			String trainFilePath = "NULL";
			String testFilePath = "NULL";
			////string AlgoType11 = HttpContext.Current.Request["type"];
			string AlgoType12 = HttpContext.Current.Request.QueryString["type"];
			//temp for testing
			string temppp = provider.FormData.ToString().Trim('=').ToLower();
			int index = 0; 
			try
            {
                await Request.Content
                    .ReadAsMultipartAsync(provider);
                globalsModels.num++;


				
				foreach (var file in provider.FileData)
                {
					index++;
					if (index > 2)
					{
						break;
					}
                    var name = file.Headers
                        .ContentDisposition
                        .FileName;
					if (name.Contains(".csv") == false)
					{
						return $"Error not CSV file";

					}
					// remove double quotes from string.
					name = name.Trim('"');
                    name = AlgoType12 + globalsModels.num+ name;
					
					var localFileName = file.LocalFileName;
                    var filePath = Path.Combine(root, name);
					///if we got file with the same name,we delete the old file and insert the new one
					if (File.Exists(filePath))
					{
						File.Delete(filePath);
					}
					
					File.Move(localFileName, filePath);
					///saving the path to the train and test file for the server
					if (name.Contains(trainFileString))
					{
						trainFilePath = filePath;

					}
					if (name.Contains(testFileDString))
					{
						testFilePath = filePath;

					}
					myfiles.Add(filePath);

                }

            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }


			string jsonName = "Model_" + globalsModels.num + ".json";
			var serverUploadPath = HttpContext.Current.Server.MapPath("~/App_Data/" + jsonName);
			string AlgoType;
			if (AlgoType12 != null)
			{
				AlgoType = AlgoType12.ToLower();
			}
			else
			{
				AlgoType= temppp;
			}

			server = new DetectorServer(trainFilePath, testFilePath, AlgoType, serverUploadPath);
			server.Serialize();
			Console.WriteLine(index);
			Console.WriteLine("done");
			object jsonObject= loadAndReturnMyJson(serverUploadPath);

		
			return jsonObject;
        }


		/*
        [HttpPost]
        [Route("api/FileUploading/UploadFile/Regression")]
        public async Task<string> UploadFileRegression()
        {
            var type = "Regression";
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/App_Data");
            var provider =
                new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content
                    .ReadAsMultipartAsync(provider);

                foreach (var file in provider.FileData)
                {
                    var name = file.Headers
                        .ContentDisposition
                        .FileName;

                    // remove double quotes from string.
                    name = name.Trim('"');
                    name = type + name;
                    var localFileName = file.LocalFileName;
                    var filePath = Path.Combine(root, name);

                    File.Move(localFileName, filePath);
                    myfiles.Add(filePath);

                }
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }

            return "File uploaded!";
        }
        [Route("api/FileUploading/all")]
        public IEnumerable<String> GetAllModels()
        {
            ///path of all saved files
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/App_Data");
            
          
           //// var root ="C:\\Users\\speed\\source\\repos\\WebApp\\App_Data" ;
            String[] files =
			Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            int size = files.Length;
            for (int i = 0; i < size; i++)
            {
                if (!(myfiles.Contains(files[i])))
                {
                    myfiles.Add(files[i]);
                }
            }
            
            return myfiles;
        }

		*/
		///loading all the info ,saving it in the model and returning json
		public object loadAndReturnMyJson(string jsonPath)
		{
			///loading all the info ,saving it in the model and returning json
			string allText = System.IO.File.ReadAllText(jsonPath);

			object jsonObject = JsonConvert.DeserializeObject(allText);
			
			List<UploadHomepageController.AnomalyObject> all = new List<UploadHomepageController.AnomalyObject>();
			using (StreamReader sr = File.OpenText(jsonPath))
			{
				all = JsonConvert.DeserializeObject<List<UploadHomepageController.AnomalyObject>>(sr.ReadToEnd());


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
			return jsonObject;
			
		}


	}
}
