using liveProjectAllocation.Repository;
using LiveProjects.Models;
using LiveProjects.Models.Pagination;
using LiveProjects.Repository;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveProjects.Controllers
{
    [Authorize]
    public class AllocationController : Controller
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

        public List<SelectListItem> GetRole()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Role>().GetAllRecords();
            foreach (var item in cat)
            {
                list.Add(new SelectListItem { Value = item.roleId.ToString(), Text = item.roleName });
            }
            return list;
        }

        public ActionResult getGroupTechnology()
        {
            return Json(_unitOfWork.GetRepositoryInstance<technologyGroup>().GetAllRecords().Select(x => new
            {
                techGroupId = x.techGroupId,
                techGroupName = x.techGroupName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Technologies(int TechID)
        {
            var technologies = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords()
               .Where(t => t.techGroupId == TechID).ToList();
            return Json(technologies.Select(x => new Technology
            {
                techId = x.techId,
                techName = x.techName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Allocation(int page = 1)
        {
            int pageSize = 50;
            var (list, total) = repository.GetAllocation(page, pageSize);
            var pager = new Pager(total, page, pageSize);

            AllocationPagination pageListt = new AllocationPagination();
            pageListt.List = list;
            pageListt.Pager = pager;
            ViewBag.resId = new SelectList(_unitOfWork.GetRepositoryInstance<Resource>().GetAllRecords(), "resId", "resName");
            return View(pageListt);
        }


        [HttpGet]
        public JsonResult Search(string selectedValues)
        {
            string[] searchValues = selectedValues.Split(',');
            var Filterr = repository.GetResourceFilter(searchValues);
           
            ViewBag.resId = new SelectList(_unitOfWork.GetRepositoryInstance<Allocation>().GetAllRecords(), "resId", "Resource.resName");
            return Json(new { data = Filterr }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AllocationAdd()
        {
            ViewBag.projectList = GetProject();
            ViewBag.ResourceList = GetResource();
            ViewBag.TechnologyList = GetTechnology();
            ViewBag.RoleList = GetRole();
            ViewBag.TechnologiGroupList = getGroupTechnology();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AllocationAdd(Allocation allocation)
        {
            if (allocation == null)
            {
                return View("Error");
            }
            else
            {
                if (allocation.allocation1 == 0)
                {
                    ModelState.AddModelError("allocation1", "Allocation value must be greater than 0.");
                }

                if (allocation.startDate < DateTime.Today)
                {
                    ModelState.AddModelError("StartDate", "Please enter the current date or a future date for Start Date.");
                }

                if (allocation.endDate < DateTime.Today)
                {
                    ModelState.AddModelError("EndDate", "Please enter the current date or a future date for End Date.");
                }

                if (allocation.endDate < allocation.startDate)
                {
                    ModelState.AddModelError("enddate", "End date must be after the start date.");
                    ViewBag.projectList = GetProject();
                    ViewBag.ResourceList = GetResource();
                    ViewBag.TechnologyList = GetTechnology();
                    ViewBag.RoleList = GetRole();
                    ViewBag.TechnologiGroupList = getGroupTechnology();
                    return View();
                }

                var existingAllocation = _unitOfWork.GetRepositoryInstance<Allocation>().GetAllRecords()
                    .Any(a => a.resId == allocation.resId && 
                              a.projId == allocation.projId &&
                              ((a.startDate <= allocation.startDate && a.endDate >= allocation.startDate) ||
                               (a.startDate <= allocation.endDate && a.endDate >= allocation.endDate) ||
                               (a.startDate >= allocation.startDate && a.endDate <= allocation.endDate)));

                if (existingAllocation)
                {
                    ModelState.AddModelError("startDate", "Resource is already allocated for the specified project within the selected date range. Please choose a different date range.");
                }
                
                var overlappingAllocations = _unitOfWork.GetRepositoryInstance<Allocation>().GetAllRecords()
                    .Where(a => a.resId == allocation.resId &&
                                ((a.startDate <= allocation.startDate && a.endDate >= allocation.startDate) ||
                                 (a.startDate <= allocation.endDate && a.endDate >= allocation.endDate) ||
                                 (a.startDate >= allocation.startDate && a.endDate <= allocation.endDate)))
                    .ToList();

                var totalAllocation = overlappingAllocations.Sum(a => a.allocation1) + allocation.allocation1;

                if (totalAllocation > 100)
                {
                    ModelState.AddModelError("allocation1", "Total allocation cannot exceed 100% for the selected resource within the specified date range.");
                }

                if (ModelState.IsValid)
                {
                    _unitOfWork.GetRepositoryInstance<Allocation>().Add(allocation);
                    return RedirectToAction("Allocation");
                }

                ViewBag.ProjectList = GetProject();
                ViewBag.ResourceList = GetResource();
                ViewBag.TechnologyList = GetTechnology();
                ViewBag.RoleList = GetRole();
                ViewBag.TechnologiGroupList = getGroupTechnology();

                return View(allocation);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AllocationReset()
        {
            ModelState.Clear();
            return RedirectToAction("AllocationAdd");
        }

       
        public ActionResult AllocationEdit(int id)
        {
            Allocation allocation = _unitOfWork.GetRepositoryInstance<Allocation>().GetFirstOrDefault(id);
            int? selectedTechGroup = allocation.techGroup;
            int? selectedtech = allocation.technology;
            var techGroupList = _unitOfWork.GetRepositoryInstance<technologyGroup>().GetAllRecords().ToList();
            var techList = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();

            ViewBag.techGroup = new SelectList(techGroupList, "techGroupId", "techGroupName", selectedTechGroup);
            ViewBag.technology = new SelectList(techList, "techId", "techName", selectedtech);

            
            ViewBag.techId = allocation.technology;

            ViewBag.projectList = GetProject();
            ViewBag.ResourceList = GetResource();
            ViewBag.TechnologyList = GetTechnology();
            ViewBag.RoleList = GetRole();
           
            return View(_unitOfWork.GetRepositoryInstance<Allocation>().GetFirstOrDefault(id));
        }

        [HttpPost]
        public ActionResult AllocationEdit(Allocation allocation)
        {
            int? selectedTechGroup = allocation.techGroup;
            int? selectedtech = allocation.technology;
            var techGroupList = _unitOfWork.GetRepositoryInstance<technologyGroup>().GetAllRecords().ToList();
            var techList = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();
            ViewBag.techGroup = new SelectList(techGroupList, "techGroupId", "techGroupName", selectedTechGroup);
            ViewBag.technology = new SelectList(techList, "techId", "techName", selectedtech);

            if (allocation == null)
            {
                throw new ArgumentNullException("Some Method received a null argument!");
            }
            else
            {
                if (allocation.allocation1 == 0)
                {
                    ModelState.AddModelError("allocation1", "Allocation value must be greater than 0.");
                }

                if (allocation.endDate < allocation.startDate)
                {
                    ModelState.AddModelError("EndDate", "Please enter a future date for End Date.");
                    ViewBag.techGroup = new SelectList(techGroupList, "techGroupId", "techGroupName", selectedTechGroup);
                    ViewBag.technology = new SelectList(techList, "techId", "techName", selectedtech);
                }

                var existAllocation = _unitOfWork.GetRepositoryInstance<Allocation>().GetAllRecords()
                   .Any(a => a.resId == allocation.resId && a.allocId != allocation.allocId &&
                             a.projId == allocation.projId &&
                             ((a.startDate <= allocation.startDate && a.endDate >= allocation.startDate) ||
                              (a.startDate <= allocation.endDate && a.endDate >= allocation.endDate) ||
                              (a.startDate >= allocation.startDate && a.endDate <= allocation.endDate)));

                if (existAllocation)
                {
                    ModelState.AddModelError("startDate", "Resource is already allocated for the specified project within the selected date range. Please choose a different date range.");
                }

                var overlappingAllocations = _unitOfWork.GetRepositoryInstance<Allocation>().GetAllRecords()
                    .Where(a => a.resId == allocation.resId &&
                                ((a.startDate <= allocation.startDate && a.endDate >= allocation.startDate) ||
                                 (a.startDate <= allocation.endDate && a.endDate >= allocation.endDate) ||
                                 (a.startDate >= allocation.startDate && a.endDate <= allocation.endDate)))
                    .ToList();

                var totalAllocation = overlappingAllocations.Sum(a => a.allocation1) + allocation.allocation1;

                if (totalAllocation > 100)
                {
                    ModelState.AddModelError("allocation1", "Total allocation cannot exceed 100% for the selected resource within the specified date range.");
                }

                if (ModelState.IsValid)
                {
                    var existingAllocation = _unitOfWork.GetRepositoryInstance<Allocation>().GetFirstOrDefault(allocation.allocId);

                    if (existingAllocation != null)
                    {
                        existingAllocation.projId = allocation.projId;
                        existingAllocation.resId = allocation.resId;
                        existingAllocation.allocation1 = allocation.allocation1;
                        existingAllocation.startDate = allocation.startDate;
                        existingAllocation.endDate = allocation.endDate;
                        existingAllocation.isBillable = allocation.isBillable;
                        existingAllocation.Role1 = allocation.Role1;
                        existingAllocation.techGroup = allocation.techGroup;
                        existingAllocation.technology = allocation.technology;
                        
                        _unitOfWork.GetRepositoryInstance<Allocation>().Update(existingAllocation);
                        _unitOfWork.SaveChanges(); 

                        return RedirectToAction("Allocation");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Existing allocation not found.");
                    }
                }

               
                ViewBag.ProjectList = GetProject();
                ViewBag.ResourceList = GetResource();
                ViewBag.TechnologyList = GetTechnology();
                ViewBag.RoleList = GetRole();

                return View(allocation);
            }
        }


        public ActionResult Delete(int id)
        {
            var msg = "";
            var CattoDelete = _unitOfWork.GetRepositoryInstance<Allocation>().GetFirstOrDefault(id);
            _unitOfWork.GetRepositoryInstance<Allocation>().Remove(CattoDelete);
            _unitOfWork.SaveChanges();
            msg = "Resource is Deleted Successfully.";

            return Json(msg, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Search(string projId, string projectName)
        {
            var query = _unitOfWork.GetRepositoryInstance<Project>().GetAllRecordsIQueryable();

            if (!string.IsNullOrEmpty(projectName))
            {
                query = query.Where(p => p.projName.Contains(projectName));
            }

            List<Project> projects = query.ToList();

            return View(projects);
        }

    }

}
