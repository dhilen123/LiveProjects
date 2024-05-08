using liveProjectAllocation.Repository;
using LiveProjects.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LiveProjects.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        public ActionResult Index()
        {
            List<Project> AllProject = _unitOfWork.GetRepositoryInstance<Project>().GetAllRecordsIQueryable().ToList();
            List<Resource> AllResource = _unitOfWork.GetRepositoryInstance<Resource>().GetAllRecordsIQueryable().ToList();
            List<Allocation> AllAllocation = _unitOfWork.GetRepositoryInstance<Allocation>().GetAllRecordsIQueryable().ToList();

            ViewBag.TotalProjects = AllProject.Count;
            ViewBag.TotalResources = AllResource.Count;
            ViewBag.TotalAllocations = AllAllocation.Count;
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login","Account");
        }
       
    }
}