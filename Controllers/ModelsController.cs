using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
////////
using WebApplication13.Models;

namespace WebApplication13.Controllers
{
   
    public class ModelsController : ApiController
    {
        private IModelsManager productsManager = new ModelsManager();

        // GET: api/Models
        public IEnumerable<Model> GetAllModels()
        {
            return productsManager.GetAllModels();
        }

        // GET: api/Models/5
        public Model Get(int model_id)
        {
            return productsManager.GetModelById(model_id);
            ///return "value";
        }

        // POST: api/Models
        [HttpPost]
        public Model AddModel(/*String t,*/Model p)
        {
            productsManager.AddModel(p);
            return p;
        }

      

        // PUT: api/Models/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Models/5
        public void Delete(int id)
        {

        }


    }
}

