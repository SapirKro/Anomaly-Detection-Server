
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

    public class FileUploadingController : ApiController
    {
        private static List<String> myfiles = new List<string>();
		private static DetectorServer server;

		/// fucntions to upload csv files thought post method
	
		[HttpPost]
        [Route("detect")]
        public async Task<object> UploadFile()
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
			string temppp = provider.FormData.ToString().Trim('=').ToLower();
			int index = 0; 
			try
            {
                await Request.Content
                    .ReadAsMultipartAsync(provider);
                globalsModels.num++;



				/*string filename=provider.FileData[0].Headers.ContentDisposition.FileName;
				///chage to only 2 files
				if ( filename.Contains(".csv")==false )
				{
					return $"Error not CSV file";

				}*/

				
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
					if (File.Exists(filePath))
					{
						File.Delete(filePath);
					}
					////File.Move(@"c:\test\SomeFile.txt", @"c:\test\Test\SomeFile.txt");
					File.Move(localFileName, filePath);
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
		public object loadAndReturnMyJson(string jsonPath)
		{
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
