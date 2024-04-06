using liveProjectAllocation.Repository;
using LiveProjects.Models;
using LiveProjects.Models.Pagination;
using LiveProjects.Repository;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;

namespace liveProjectAllocation.Controllers
{
    [Authorize]
    public class ResourceController : Controller
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        public SpRepository repository = new SpRepository();

        public ActionResult GetAllResource(int page = 1)
        {
            int pageSize = 50;
            var (list, total) = repository.GetResource(page, pageSize);
            var pager = new Pager(total, page, pageSize);

            ResourcePagination pageListt = new ResourcePagination();
            pageListt.List = list;
            pageListt.Pager = pager;
            return View(pageListt);
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

        public List<SelectListItem> GetDesignation()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Designation>().GetAllRecords();
            foreach (var item in cat)
            {
                list.Add(new SelectListItem { Value = item.designId.ToString(), Text = item.desigName });
            }
            return list;
        }

        public ActionResult ResourceAdd()
        {
            var techGroup = _unitOfWork.GetRepositoryInstance<technologyGroup>().GetAllRecords().ToList();
            ViewBag.technologyGroup = new SelectList(techGroup, "techGroupId", "techGroupName");

            var primaryTech = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();
            ViewBag.primaryTechnology = new SelectList(primaryTech, "techId", "techName");

            var secondaryTech = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();
            ViewBag.secondaryTechnology = new SelectList(secondaryTech, "techId", "techName");

            var designation = _unitOfWork.GetRepositoryInstance<Designation>().GetAllRecords().ToList();
            ViewBag.designation = new SelectList(designation, "designId", "desigName");

            var role = _unitOfWork.GetRepositoryInstance<Role>().GetAllRecords().ToList();
            ViewBag.resRole = new SelectList(role, "roleId", "roleName");

            return View();
        }

        [HttpPost]
        public ActionResult ResourceAdd(Resource rsc)
        {
            if (ModelState.IsValid)
            {
                if(rsc.primaryTechnology == rsc.secondaryTechnology)
                {
                    ModelState.AddModelError("secondaryTechnology", "already select this technology");
                    var techGroup1 = _unitOfWork.GetRepositoryInstance<technologyGroup>().GetAllRecords().ToList();
                    int? ids = rsc.technologyGroup;
                    ViewBag.technologyGroup = new SelectList(techGroup1, "techGroupId", "techGroupName", ids);

                    var primaryTech1 = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();
                    int? techids = rsc.primaryTechnology;
                    ViewBag.primaryTechnology = new SelectList(primaryTech1, "techId", "techName", techids);

                    var secondaryTech1 = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();
                    ViewBag.secondaryTechnology = new SelectList(secondaryTech1, "techId", "techName");

                    var designation1 = _unitOfWork.GetRepositoryInstance<Designation>().GetAllRecords().ToList();
                    ViewBag.designation = new SelectList(designation1, "designId", "desigName");

                    var role1 = _unitOfWork.GetRepositoryInstance<Role>().GetAllRecords().ToList();
                    ViewBag.resRole = new SelectList(role1, "roleId", "roleName");
                    return View(rsc);
                }
                
                _unitOfWork.GetRepositoryInstance<Resource>().Add(rsc);
                return RedirectToAction("GetAllResource");
            }

            var techGroup = _unitOfWork.GetRepositoryInstance<technologyGroup>().GetAllRecords().ToList();
           
            ViewBag.technologyGroup = new SelectList(techGroup, "techGroupId", "techGroupName");

            var primaryTech = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();
            ViewBag.primaryTechnology = new SelectList(primaryTech, "techId", "techName");

            var secondaryTech = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();
            ViewBag.secondaryTechnology = new SelectList(secondaryTech, "techId", "techName");

            var designation = _unitOfWork.GetRepositoryInstance<Designation>().GetAllRecords().ToList();
            ViewBag.designation = new SelectList(designation, "designId", "desigName");

            var role = _unitOfWork.GetRepositoryInstance<Role>().GetAllRecords().ToList();
            ViewBag.resRole = new SelectList(role, "roleId", "roleName");
            return View(rsc);
        }
        public ActionResult ResourceEdit(int resId)
        {
            Resource resource = _unitOfWork.GetRepositoryInstance<Resource>().GetFirstOrDefault(resId);
            int? selectedTechGroup = resource.technologyGroup;
            int? selectedtech = resource.primaryTechnology;
            var techGroupList = _unitOfWork.GetRepositoryInstance<technologyGroup>().GetAllRecords().ToList();
            var techList = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();

            ViewBag.technologyGroup = new SelectList(techGroupList, "techGroupId", "techGroupName", selectedTechGroup);
            ViewBag.primaryTechnology = new SelectList(techList, "techId", "techName", selectedtech);
            ViewBag.roleList = GetRole();
            ViewBag.designationList = GetDesignation();
            var secondaryTech = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();
            ViewBag.secondaryTechnology = new SelectList(secondaryTech, "techId", "techName");
            return View(_unitOfWork.GetRepositoryInstance<Resource>().GetFirstOrDefault(resId));
        }
        [HttpPost]
        public ActionResult ResourceEdit(Resource resource)
        {
            if (ModelState.IsValid)
            {
                if (resource.primaryTechnology == resource.secondaryTechnology)
                {
                    ModelState.AddModelError("secondaryTechnology", "already select this technology");
                    var techGroup1 = _unitOfWork.GetRepositoryInstance<technologyGroup>().GetAllRecords().ToList();
                    int? ids = resource.technologyGroup;
                    ViewBag.technologyGroup = new SelectList(techGroup1, "techGroupId", "techGroupName", ids);

                    var primaryTech1 = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();
                    int? techids = resource.primaryTechnology;
                    ViewBag.primaryTechnology = new SelectList(primaryTech1, "techId", "techName", techids);

                    var secondaryTech1 = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();
                    ViewBag.secondaryTechnology = new SelectList(secondaryTech1, "techId", "techName");

                    var designation1 = _unitOfWork.GetRepositoryInstance<Designation>().GetAllRecords().ToList();
                    ViewBag.designation = new SelectList(designation1, "designId", "desigName");

                    var role1 = _unitOfWork.GetRepositoryInstance<Role>().GetAllRecords().ToList();
                    ViewBag.resRole = new SelectList(role1, "roleId", "roleName");
                    return View(resource);
                }
                _unitOfWork.GetRepositoryInstance<Resource>().Update(resource);
                return RedirectToAction("GetAllResource");
            }

            var techGroup = _unitOfWork.GetRepositoryInstance<technologyGroup>().GetAllRecords().ToList();

            ViewBag.technologyGroup = new SelectList(techGroup, "techGroupId", "techGroupName");

            var primaryTech = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();
            ViewBag.primaryTechnology = new SelectList(primaryTech, "techId", "techName");

            var secondaryTech = _unitOfWork.GetRepositoryInstance<Technology>().GetAllRecords().ToList();
            ViewBag.secondaryTechnology = new SelectList(secondaryTech, "techId", "techName");

            var designation = _unitOfWork.GetRepositoryInstance<Designation>().GetAllRecords().ToList();
            ViewBag.designation = new SelectList(designation, "designId", "desigName");

            var role = _unitOfWork.GetRepositoryInstance<Role>().GetAllRecords().ToList();
            ViewBag.resRole = new SelectList(role, "roleId", "roleName");
            return View(resource);

        }

        public ActionResult Delete(int id)
        {
            var msg = "";
            var CattoDelete = _unitOfWork.GetRepositoryInstance<Resource>().GetFirstOrDefault(id);
            var Project = _unitOfWork.GetRepositoryInstance<Project>().GetAllRecords();
            var allocation = _unitOfWork.GetRepositoryInstance<Allocation>().GetAllRecords();

            var usedInProject = Project.Any(x => x.projId == CattoDelete.resId);
            var usedInAllocation = allocation.Any(x => x.allocId == CattoDelete.resId);

            if (usedInProject || usedInAllocation)
            {
                msg = "Resource is already used in another table and cannot be deleted.";
            }
            else
            {
                try
                {
                    
                    _unitOfWork.GetRepositoryInstance<Resource>().Remove(CattoDelete);
                    _unitOfWork.SaveChanges();
                    msg = "Resource is deleted successfully.";
                }
                catch (DbEntityValidationException ex)
                {
                   
                    msg = "Resource Already Used in other table.";
                   
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}
