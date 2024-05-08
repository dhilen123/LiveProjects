using liveProjectAllocation.Repository;
using LiveProjects.Models;
using LiveProjects.Repository;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace LiveProjects.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        SpRepository repo = new SpRepository();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
                return View(); 
        }

        [HttpPost]
        public ActionResult Create(TimeEntry tmentry)
        {
            return View();
        }

        public JsonResult UserData(string StartDate, string EndDate)
        {
            string userId = User.Identity.GetUserId();

            if (userId != null)
            {
                var list = repo.getData(userId, StartDate, EndDate);
                return Json(list,JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = "User ID not found in session" }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public JsonResult PunchIn()
        {
            string userId = User.Identity.GetUserId();

            TimeEntry existingTimeEntry = _unitOfWork.GetRepositoryInstance<TimeEntry>().GetAllRecords()
                .FirstOrDefault(te => te.userId == userId &&
                                      te.PunchIn.Date == DateTime.Today &&
                                      te.PunchOut == null);

            if (existingTimeEntry != null)
            {
                existingTimeEntry.BreakStartTime = null;
                existingTimeEntry.BreakEndTime = null;
                existingTimeEntry.PunchIn = DateTime.Now;
                _unitOfWork.SaveChanges();
                return Json(new { success = true, id = existingTimeEntry.Id });
            }
            else
            {
                TimeEntry newTimeEntry = new TimeEntry
                {
                    userId = userId,
                    PunchIn = DateTime.Now
                };

                _unitOfWork.GetRepositoryInstance<TimeEntry>().Add(newTimeEntry);
                _unitOfWork.SaveChanges();

                return Json(new { success = true, id = newTimeEntry.Id });
            }
        }


        [HttpPost]
        public async Task<JsonResult> StartEndBreak(bool startBreak)
        {
            string userId = User.Identity.GetUserId(); 

            TimeEntry currentTimeEntry = GetCurrentTimeEntry(userId);

            if (currentTimeEntry != null)
            {
                if (startBreak)
                {
                    currentTimeEntry.BreakStartTime = DateTime.Now;
                }
                else
                {
                    currentTimeEntry.BreakEndTime = DateTime.Now;
                    var getstartendbreak = _unitOfWork.GetRepositoryInstance<TimeEntry>().GetAllRecords()
                    .FirstOrDefault(te => te.userId == userId && te.PunchIn.Date == DateTime.Today);


                    TimeSpan breaktime = DateTime.Now - (DateTime)getstartendbreak.BreakStartTime;
                    if(getstartendbreak.breakDifff.HasValue)
                    {
                        getstartendbreak.breakDifff = getstartendbreak.breakDifff.Value.Add(breaktime);
                    }
                    else
                    {
                        getstartendbreak.breakDifff = Convert.ToDateTime(breaktime.ToString());
                    }

                    getstartendbreak.BreakStartTime = null;
                }
                _unitOfWork.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }


        [HttpPost]
        public async Task<JsonResult> PunchOut()
        {
            string userId = User.Identity.GetUserId();

            TimeEntry currentTimeEntry = GetCurrentTimeEntry(userId);

            if (currentTimeEntry != null)
            {
                currentTimeEntry.PunchOut = DateTime.Now;
                _unitOfWork.SaveChanges();
               
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        private TimeEntry GetCurrentTimeEntry(string userId)
        {
            return _unitOfWork.GetRepositoryInstance<TimeEntry>().GetAllRecords()
                .FirstOrDefault(te => te.userId == userId &&
                                      te.PunchIn.Date == DateTime.Today);
        }

        public JsonResult checkpunchIn()
        {
            string userId = User.Identity.GetUserId();

             var checkk = _unitOfWork.GetRepositoryInstance<TimeEntry>().GetAllRecords()
            .Where(x => x.PunchIn.Date == DateTime.Today
                    && x.userId == userId
                    && x.PunchOut == null)
            .Select(x => new { x.Id, x.PunchIn, x.PunchOut, x.BreakStartTime, x.BreakEndTime, x.userId,x.Diffrnt,x.breakDifff })
            .ToList();

            var checkcontinue = _unitOfWork.GetRepositoryInstance<TimeEntry>().GetAllRecords()
                .Where(x => x.PunchIn.Date == DateTime.Today
                        && x.userId == userId
                        && x.PunchOut != null)
                .Select(x => new { x.Id, x.PunchIn, x.PunchOut, x.BreakStartTime, x.BreakEndTime, x.userId,x.Diffrnt,x.breakDifff })
                .ToList();
 
            if(checkk.Count == 0 && checkcontinue.Count == 0)
            {
                return Json(new { data = "datanull" }, JsonRequestBehavior.AllowGet);
            }
           else if(checkk.Count == 0)
            {
                return Json(new { data = checkcontinue }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { data = checkk }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult continueWork()
        {
            string userId = User.Identity.GetUserId();

            var DifferenceV = _unitOfWork.GetRepositoryInstance<TimeEntry>().GetAllRecords()
                .Where(x => x.PunchIn.Date == DateTime.Today
                        && x.userId == userId
                        && x.PunchOut != null)
                .ToList();
            var entryToUpdate = DifferenceV.FirstOrDefault();
            var TimeDiff = DateTime.Now - entryToUpdate.PunchOut.Value;

            if (entryToUpdate.Diffrnt.HasValue)
            {
                entryToUpdate.Diffrnt = entryToUpdate.Diffrnt.Value.Add(TimeDiff);
            }
            else
            {
                entryToUpdate.Diffrnt = Convert.ToDateTime(TimeDiff.ToString());
            }
            _unitOfWork.SaveChanges();

            return Json(new {data = true },JsonRequestBehavior.AllowGet);
        }

        public ActionResult Attendance()
        {
            return View();
        }

        public ActionResult Calendar()
        {
            return View(DateTime.Today);
        }

        [HttpPost]
        public JsonResult FetchData(string date)
        {
            string userId = User.Identity.GetUserId();
            var list = repo.getDatedata(userId,date);

            var dummyData = new
            {
                Date = date,
                lists = list
            };
            return Json(dummyData);
        }

        [HttpPost]
        public ActionResult FetchDataForMonth(int year, int month)
        {
            string userId = User.Identity.GetUserId();

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            List<DateTime> allDatesOfMonth = Enumerable.Range(0, (lastDayOfMonth - firstDayOfMonth).Days + 1)
                                                        .Select(offset => firstDayOfMonth.AddDays(offset))
                                                        .ToList();

            var userData = repo.getUserData(userId);

            List<object> responseData = new List<object>();

            foreach (DateTime date in allDatesOfMonth)
            {
                var matchingRecord = userData.FirstOrDefault(record => record.PunchIn.Date == date.Date);

                if (matchingRecord != null)
                {
                    responseData.Add(new
                    {
                        Date = date,
                        PunchIn = matchingRecord.PunchIn,
                        PunchOut = matchingRecord.PunchOut,
                        TotalTime = matchingRecord.TotalTime
                    });
                }
                else
                {
                    responseData.Add(new
                    {
                        Date = date,
                        PunchIn = (DateTime?)null,
                        PunchOut = (DateTime?)null, 
                        TotalTime = (TimeSpan?)null 
                    });
                }
            }

            return Json(new { data = responseData });
        }


    }
}