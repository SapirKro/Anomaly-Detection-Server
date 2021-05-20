
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace WebApplication13.Controllers
{

    public class FileUploadingController : ApiController
    {
        private static List<String> mydocfiles = new List<string>();

        /// <summary>
        /// fucntions to upload csv files thought post method
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/FileUploading/UploadFile/Hybrid")]
        public async Task<string> UploadFileHybrid()
        {
           
            var type= "Hybrid";
            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/App_Data");
            var provider =
                new MultipartFormDataStreamProvider(root);

            try
            {
                await Request.Content
                    .ReadAsMultipartAsync(provider);
                numberOfModels.num++;

                foreach (var file in provider.FileData)
                {
                    var name = file.Headers
                        .ContentDisposition
                        .FileName;

                    // remove double quotes from string.
                    name = name.Trim('"');
                    name = type + numberOfModels.num+ name;
                    var localFileName = file.LocalFileName;
                    var filePath = Path.Combine(root, name);

                    File.Move(localFileName, filePath);
                    mydocfiles.Add(filePath);

                }

            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }

            return "File uploaded!";
        }

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
                    mydocfiles.Add(filePath);

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
            
          
           //// var root ="C:\\Users\\speed\\source\\repos\\WebApplication13\\App_Data" ;
            String[] files =
       Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            int size = files.Length;
            for (int i = 0; i < size; i++)
            {
                if (!(mydocfiles.Contains(files[i])))
                {
                    mydocfiles.Add(files[i]);
                }
            }
            
            return mydocfiles;
        }

    }
}
