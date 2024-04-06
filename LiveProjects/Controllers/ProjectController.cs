
using liveProjectAllocation.Repository;
using LiveProjects.Models;
using LiveProjects.Models.Pagination;
using LiveProjects.Repository;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace liveProjectAllocation.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {

        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        public SpRepository repository = new SpRepository();

        public List<SelectListItem> GetTechnology()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords();
            foreach (var item in cat)
            {
                list.Add(new SelectListItem { Value = item.techId.ToString(), Text = item.techName });

            }
            return list;
        }
        public List<SelectListItem> GetResource()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Resource>().GetAllRecords().Where(t => t.isActive);  

            foreach (var item in cat)
            {
                list.Add(new SelectListItem { Value = item.resId.ToString(), Text = item.resName });
            }
            return list;
        }
        
        public ActionResult GetAllProject(int page = 1)
        {
            int pageSize = 50;
            var (list, total) = repository.GetProject(page, pageSize);
            var pager = new Pager(total, page, pageSize);

            ProjectPagination pageListt = new ProjectPagination();
            pageListt.List = list;
            pageListt.Pager = pager;
            return View(pageListt);
        }  
        public ActionResult ProjectAdd()
        {
            ViewBag.Technologylist = GetTechnology();
            ViewBag.Resourcelist = GetResource();
            return View();
        }
        [HttpPost]
        public ActionResult ProjectAdd(Project tbl)
        {
            if (tbl == null)
            {
                return HttpNotFound();
            }

            
            if (tbl.enddate < tbl.startdate)
            {
                ModelState.AddModelError("enddate", "End date must be after the start date.");
            }

            
            bool isProjectExistsWithSameNameAndManager = _unitOfWork.GetRepositoryInstance<Project>()
                .GetAllRecordsIQueryable()
                .Any(c => c.projName == tbl.projName);


            if (!isProjectExistsWithSameNameAndManager && ModelState.IsValid)
            {
                _unitOfWork.GetRepositoryInstance<Project>().Add(tbl);
                _unitOfWork.SaveChanges();
                return RedirectToAction("GetAllProject");
            }
            else
            {
                ModelState.AddModelError("projName", "This project already Exists");
            }

            ViewBag.Technologylist = GetTechnology();
            ViewBag.Resourcelist = GetResource();
            return View();
        }


        public ActionResult ProjectEdit(int projectId)
        {
            ViewBag.Technologylist = GetTechnology();
            ViewBag.Resourcelist = GetResource();
            return View(_unitOfWork.GetRepositoryInstance<Project>().GetFirstOrDefault(projectId));
        }

        [HttpPost]
        public ActionResult ProjectEdit(Project tbl)
        {
            if (tbl.enddate < tbl.startdate)
            {
                ModelState.AddModelError("enddate", "End date must be after the start date.");
            }

            bool isProjectExistsWithSameNameAndManager = _unitOfWork.GetRepositoryInstance<Project>()
                .GetAllRecordsIQueryable()
                .Any(c => c.projName == tbl.projName && c.projId != tbl.projId);

            if (!isProjectExistsWithSameNameAndManager && ModelState.IsValid)
            {
                _unitOfWork.GetRepositoryInstance<Project>().Update(tbl);
                _unitOfWork.SaveChanges();
                return RedirectToAction("GetAllProject");
            }
            else
            {
                ModelState.AddModelError("projName", "This project already Exists");
            }

            ViewBag.Technologylist = GetTechnology();
            ViewBag.Resourcelist = GetResource();
            return View(tbl);
        }
        

        public ActionResult Delete(int id)
        {
            var msg = "";
            var CattoDelete = _unitOfWork.GetRepositoryInstance<Project>().GetFirstOrDefault(id);
            var allocation = _unitOfWork.GetRepositoryInstance<Allocation>().GetAllRecords();

            var allocationn = allocation.Any(x => x.allocId == CattoDelete.projId);
            if (allocationn)
            {
                msg = "Resource Already Used in other table.";
            }
            else
            {
                _unitOfWork.GetRepositoryInstance<Project>().Remove(CattoDelete);
                _unitOfWork.SaveChanges();
                msg = "Resource is Deleted Successfully.";
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}