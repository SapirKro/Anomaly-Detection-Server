using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication13.Models
{
    interface IModelsManager
    {

        IEnumerable<Model> GetAllModels();
        Model GetModelById(int id);
        void AddModel(Model p);
        void UpdateModel(Model p);
        void DeleteModel(int id);
    }
}
