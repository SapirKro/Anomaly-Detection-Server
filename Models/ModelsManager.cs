using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication13.Models
{
    public class ModelsManager
    {

        private static List<Model> products = new List<Model>()
        {
            new Model { Id = 3, UploadTime ="20210430T21:30:55+02.00",Status= "ready" },
            new Model { Id = 4, UploadTime ="20210430T22:31:55+02.00",Status= "pending" },
            new Model { Id = 5, UploadTime ="20210430T23:32:55+02.00",Status= "ready" },

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
            prod.UploadTime = p.UploadTime;
            prod.Status = p.Status;
        }
    }
}
