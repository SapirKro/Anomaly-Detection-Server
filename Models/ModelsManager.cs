using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication13.Models
{
  
    public class ModelsManager : IModelsManager
    {

        private static List<Model> products = new List<Model>()
        {
             new Model { Id = 1,Description= "ready", Time =":30:55+02.00" },
              new Model { Id = 2,Description= "pending", Time ="20210430T21:30:55+02.00", },
          };

        public void AddModel(Model p)
        {
            products.Add(p);
            
        }

        public void DeleteModel(int id)
        {
            Model p = products.Where(x => x.Id == id).FirstOrDefault();
            if (p == null)
                throw new Exception("model not found");
            products.Remove(p);
        }

        public IEnumerable<Model> GetAllModels()
        {
            return products;
        }

        public Model GetModelById(int id)
        {
            Model p = products.Where(x => x.Id == id).FirstOrDefault();
            return p;
        }

        public void UpdateModel(Model p)
        {
            Model prod = products.Where(x => x.Id == p.Id).FirstOrDefault();
            prod.Time = p.Time;
            prod.Description = p.Description;
        }

       
    }
}