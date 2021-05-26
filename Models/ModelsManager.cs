using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
  /// <summary>
  /// contain all the anomalis info
  /// </summary>
    public class ModelsManager : IModelsManager
    {

        private static List<Model> myModels = new List<Model>()
        {
             new Model { Id = 1,Description= "example", Time ="100" } ,
          };

        public void AddModel(Model p)
        {
            myModels.Add(p);
            
        }

        public void DeleteModel(int id)
        {
            Model p = myModels.Where(x => x.Id == id).FirstOrDefault();
            if (p == null)
                throw new Exception("model not found");
            myModels.Remove(p);
        }

        public IEnumerable<Model> GetAllModels()
        {
            return myModels;
        }

        public Model GetModelById(int id)
        {
            Model p = myModels.Where(x => x.Id == id).FirstOrDefault();
            return p;
        }

        public void UpdateModel(Model p)
        {
            Model prod = myModels.Where(x => x.Id == p.Id).FirstOrDefault();
            prod.Time = p.Time;
            prod.Description = p.Description;
        }

       
    }
}