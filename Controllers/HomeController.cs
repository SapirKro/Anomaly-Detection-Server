using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Text;
using System.Threading.Tasks;

using CLI;
namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "The app made by:Sapir Kroitoro ,Omer Mazal,Amichai Vollin,Ben Rotlevi.";

            return View();
        }
		public ActionResult About1()
		{
			ViewBag.HTMLCheck = true;
			ViewBag.Message = "The app made by:Sapir Kroitoro ,Omer Mazal,Amichai Vollin,Ben Rotlevi.";

			return View();
		}
		/* public ActionResult Contact()
		 {
			 ViewBag.Message1 = "Your contact page.1";

			 ex1main e1 = new ex1main(35);
			 int num = e1.mymain();
			 ViewBag.Message2 = num;
			 return View();
		 }*/
	}
}