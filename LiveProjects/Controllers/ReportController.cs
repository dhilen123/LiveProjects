using liveProjectAllocation.Repository;
using LiveProjects.Models;
using LiveProjects.Models.Pagination;
using LiveProjects.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LiveProjects.Controllers
{
     [Authorize]
    public class ReportController : Controller
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        public SpRepository repository = new SpRepository();
        public List<SelectListItem> GetProject()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Project>().GetAllRecords();
            foreach (var item in cat)
            {
                list.Add(new SelectListItem { Value = item.projId.ToString(), Text = item.projName });
            }
            return list;
        }
        public List<SelectListItem> GetResource()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Resource>().GetAllRecords().Where(t => t.isActive);  // Filter by isActive property

            foreach (var item in cat)
            {
                list.Add(new SelectListItem { Value = item.resId.ToString(), Text = item.resName });
            }
            return list;
        }
        public List<SelectListItem> Gettechnology()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<technologyGroup>().GetAllRecords();
            foreach (var item in cat)
            {
                list.Add(new SelectListItem { Value = item.techGroupId.ToString(), Text = item.techGroupName });
            }
            return list;
        }

        public List<SelectListItem> Gettechnologi()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords();
            foreach (var item in cat)
            {
                list.Add(new SelectListItem { Value = item.techId.ToString(), Text = item.techName });
            }
            return list;
        }

     

        public ActionResult GetAllReport()
        {
            AllocationPagination pagedlist = new AllocationPagination();
            ViewBag.technologyGroupList = Gettechnology();
            ViewBag.technologiesList = Gettechnologi();
            ViewBag.projectList = GetProject();
            ViewBag.ResourceList = GetResource();
            return View(pagedlist);
        }

        public JsonResult SearchReport(SearchModel Search, int page = 1,int pageSize = 0)
        {
                var (list, total) = repository.GetAllreports(Search, page, pageSize);
                var pager = new Pager(total, page, pageSize);

                AllocationPagination pagelist = new AllocationPagination();
                pagelist.List = list;
                pagelist.Pager = pager;
                var pageList1 = pagelist;
                return Json(pageList1, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GethourlyReport()
        {
            AllocationPagination pagedlist = new AllocationPagination();
            ViewBag.projectList = GetProject();
            ViewBag.ResourceList = GetResource();
            ViewBag.technologyGroupList = Gettechnology();
            var role = _unitOfWork.GetRepositoryInstance<Role>().GetAllRecords().ToList();
            ViewBag.resRole = new SelectList(role, "roleId", "roleName");
            return View(pagedlist);
        }


        public JsonResult SearchHourlyReport(SearchHourlyModel Search, int page = 1, int pageSize = 0)
        {
            var (list, total) = repository.GetHourlyreports(Search,page,pageSize);
            var pager = new Pager(total, page, pageSize);
            AllocationPagination pagelist = new AllocationPagination();
            pagelist.List = list;
            pagelist.Pager = pager;
            var pageList1 = pagelist;
            return Json(pageList1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getGroupTechnology()
        {
            return Json(_unitOfWork.GetRepositoryInstance<technologyGroup>().GetAllRecords().Select(x => new
            {
                techGroupId = x.techGroupId,
                techGroupName = x.techGroupName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult getTechnologies()
        {
            return Json(_unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().Select(x => new
            {
                techId = x.techId,
                techName = x.techName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult getProjectes()
        {
            return Json(_unitOfWork.GetRepositoryInstance<Project>().GetAllRecords().Select(x => new
            {
                projId = x.projId,
                projName = x.projName

            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult getResource1()
        {
            return Json(_unitOfWork.GetRepositoryInstance<Resource>().GetAllRecords().Select(x => new
            {
                resId = x.resId,
                resName = x.resName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRole()
        {
            return Json(_unitOfWork.GetRepositoryInstance<Role>().GetAllRecords().Select(x => new
            {
                roleId = x.roleId,
                roleName = x.roleName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}