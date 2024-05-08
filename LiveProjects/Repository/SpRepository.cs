using LiveProjects.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Drawing.Printing;
using LiveProjects.Models.Pagination;
using Type = LiveProjects.Models.Type;

namespace LiveProjects.Repository
{
    public class SpRepository
    {
        private SqlConnection con;
        private void connection()
        {
            string connection = ConfigurationManager.ConnectionStrings["liveProjectEntities2"].ConnectionString;
            con = new SqlConnection(connection);
        }
        public (List<Resource>, int) GetResource(int PageIndex, int PageSize)
        {
            connection();
            List<Resource> resourceModel = new List<Resource>();
            SqlCommand cmd = new SqlCommand("GetResourcesPageWise", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", PageSize);
            cmd.Parameters.Add("@RecordCount", SqlDbType.VarChar, 30);
            cmd.Parameters["@RecordCount"].Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                resourceModel.Add(new Resource
                {
                    resId = Convert.ToInt32(dr["resId"]),
                    resName = Convert.ToString(dr["resName"]),
                    resDepartment = Convert.ToString(dr["resDepartment"]),
                    isActive = Convert.ToBoolean(dr["isActive"]),
                    technologyGroup1 = new technologyGroup
                    {
                        techGroupName = Convert.ToString(dr["techGroupName"]),
                    },
                    Technology = new Technology
                    {
                        techName = Convert.ToString(dr["primaryTechnology"])
                    },
                    Technology1 = new Technology
                    {
                        techName = Convert.ToString(dr["secondaryTechnology"])
                    },
                    Designation1 = new Designation
                    {
                        desigName = Convert.ToString(dr["desigName"])
                    },
                    Role = new Role
                    {
                        roleName = Convert.ToString(dr["roleName"])
                    }
                });
            }
            return (resourceModel, Convert.ToInt32(cmd.Parameters["@RecordCount"].Value));
        }


        public (List<Allocation>, int) GetAllocation(int PageIndex, int PageSize)
        {
            connection();
            List<Allocation> resourceModel = new List<Allocation>();
            SqlCommand cmd = new SqlCommand("GetAllocation", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", PageSize);
            cmd.Parameters.Add("@RecordCount", SqlDbType.VarChar, 30);
            cmd.Parameters["@RecordCount"].Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                resourceModel.Add(new Allocation
                {
                    allocId = Convert.ToInt32(dr["allocId"]),
                    Project = new Project
                    {
                        projName = Convert.ToString(dr["projName"]),

                    },
                    Resource = new Resource
                    {
                        resName = Convert.ToString(dr["resName"]),

                    },
                    startDate = Convert.ToDateTime(dr["startDate"]),
                    endDate = Convert.ToDateTime(dr["endDate"]),
                    allocation1 = Convert.ToInt32(dr["allocation"]),

                    isBillable = Convert.ToBoolean(dr["isBillable"]),


                    Role1 = new Role
                    {
                        roleName = Convert.ToString(dr["roleName"])
                    },

                    technologyGroup = new technologyGroup
                    {
                        techGroupName = Convert.ToString(dr["techGroupName"])
                    },

                    Technology1 = new Technology
                    {
                        techName = Convert.ToString(dr["techName"])
                    }
                });
            }
            return (resourceModel, Convert.ToInt32(cmd.Parameters["@RecordCount"].Value));
        }

        public (List<Project>, int) GetProject(int PageIndex, int PageSize)
        {
            connection();
            List<Project> resourceModel = new List<Project>();
            SqlCommand cmd = new SqlCommand("GetProjectsPageWise", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", PageSize);
            cmd.Parameters.Add("@RecordCount", SqlDbType.VarChar, 30);
            cmd.Parameters["@RecordCount"].Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                resourceModel.Add(new Project
                {
                    projId = Convert.ToInt32(dr["projId"]),

                    projName = Convert.ToString(dr["projName"]),

                    Resource = new Resource
                    {
                        resName = Convert.ToString(dr["projManager"]),

                    },
                    Technology1 = new Technology
                    {
                        techName = Convert.ToString(dr["technology"]),
                    },

                    startdate = Convert.ToDateTime(dr["startdate"]),
                    enddate = Convert.ToDateTime(dr["enddate"]),
                    status = Convert.ToString(dr["status"])
                });
            }

            return (resourceModel, Convert.ToInt32(cmd.Parameters["@RecordCount"].Value));
        }

        public List<Allocation> GetResourceFilter(string[] search)
        {
            connection();
            List<Allocation> allocModel = new List<Allocation>();
            SqlCommand cmd = new SqlCommand("SpGetFilteredResources", con);
            cmd.CommandType = CommandType.StoredProcedure;
            string srch = string.Join(",", search);
            cmd.Parameters.AddWithValue("@SearchValues", srch);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                allocModel.Add(new Allocation
                {
                    allocId = Convert.ToInt32(dr["allocId"]),
                    Project = new Project
                    {
                        projName = Convert.ToString(dr["ProjectName"]),

                    },
                    Resource = new Resource
                    {
                        resName = Convert.ToString(dr["ResourceName"]),

                    },
                    startDate = Convert.ToDateTime(dr["startDate"]),
                    endDate = Convert.ToDateTime(dr["endDate"]),
                    allocation1 = Convert.ToInt32(dr["allocation"]),

                    isBillable = Convert.ToBoolean(dr["isBillable"]),


                    Role1 = new Role
                    {
                        roleName = Convert.ToString(dr["RoleName"])
                    },

                    Technology1 = new Technology
                    {
                        techName = Convert.ToString(dr["TechnologyName"])
                    },
                    technologyGroup = new technologyGroup
                    {
                        techGroupName = Convert.ToString(dr["techGroup"])
                    }
                });
            }
            return allocModel;
        }


        public (List<Allocation>, int) GetAllreports(SearchModel _search, int PageIndex, int PageSize)
        {
            if(_search.resourceId == -1)
            {
                _search.resourceId = null;
            }
            if (_search.ProjectId == -1)
            {
                _search.ProjectId = null;
            }
            if (_search.techGroupId == -1)
            {
                _search.techGroupId = null;
            }
            if (_search.techid == -1)
            {
                _search.techid = null;
            }
            connection();
            List<Allocation> allocModel = new List<Allocation>();
            SqlCommand cmd = new SqlCommand("GetAllReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", PageSize);
            cmd.Parameters.AddWithValue("@StartDate", _search.Startdate);
            cmd.Parameters.AddWithValue("@EndDate", _search.Enddate);
            cmd.Parameters.AddWithValue("@resource", _search.resourceId);
            cmd.Parameters.AddWithValue("@Project", _search.ProjectId);
            cmd.Parameters.AddWithValue("@techGroup", _search.techGroupId);
            cmd.Parameters.AddWithValue("@tech", _search.techid);
            cmd.Parameters.Add("@RecordCount", SqlDbType.VarChar, 30);
            cmd.Parameters["@RecordCount"].Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                allocModel.Add(new Allocation
                {
                    allocId = Convert.ToInt32(dr["allocId"]),
                    Project = new Project
                    {
                        projName = Convert.ToString(dr["projName"]),

                    },
                    Resource = new Resource
                    {
                        resName = Convert.ToString(dr["resName"]),

                    },
                    startDate = Convert.ToDateTime(dr["startDate"]),
                    endDate = Convert.ToDateTime(dr["endDate"]),
                    allocation1 = Convert.ToInt32(dr["allocation"]),

                    isBillable = Convert.ToBoolean(dr["isBillable"]),


                    Role1 = new Role
                    {
                        roleName = Convert.ToString(dr["roleName"])
                    },

                    technologyGroup = new technologyGroup
                    {
                        techGroupName = Convert.ToString(dr["techGroupName"])
                    },

                    Technology1 = new Technology
                    {
                        techName = Convert.ToString(dr["techName"])
                    }
                });
            }

            return (allocModel, Convert.ToInt32(cmd.Parameters["@RecordCount"].Value));
        }

        public (List<Allocation>, int) GetHourlyreports(SearchHourlyModel _search, int PageIndex, int PageSize)
        {
            if (_search.resourceId == -1)
            {
                _search.resourceId = null;
            }
            if (_search.ProjectId == -1)
            {
                _search.ProjectId = null;
            }
            if (_search.techGroupId == -1)
            {
                _search.techGroupId = null;
            }
            if (_search.roleId == -1)
            {
                _search.roleId = null;
            }
            connection();
            List<Allocation> allocModel = new List<Allocation>();
            SqlCommand cmd = new SqlCommand("GetHourlyReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@startDate", _search.Startdate);
            cmd.Parameters.AddWithValue("@endDate", _search.Enddate);
            cmd.Parameters.AddWithValue("@searchResId", _search.resourceId);
            cmd.Parameters.AddWithValue("@searchProjId", _search.ProjectId);
            cmd.Parameters.AddWithValue("@searchTechGroupId", _search.techGroupId);
            cmd.Parameters.AddWithValue("@searchRoleId", _search.roleId);
            cmd.Parameters.AddWithValue("@underUtilizer", _search.underUtilize);
            cmd.Parameters.AddWithValue("@overUtilizer", _search.overUtilize);
            cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
            cmd.Parameters.AddWithValue("@PageSize", PageSize);
            cmd.Parameters.Add("@TotalRecord", SqlDbType.VarChar, 30);
            cmd.Parameters["@TotalRecord"].Direction = ParameterDirection.Output;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                allocModel.Add(new Allocation
                {
                    Project = new Project
                    {
                        projName = Convert.ToString(dr["projName"])
                    },
                    Resource = new Resource
                    {
                        resName = Convert.ToString(dr["resName"]),
                        Type = new Type
                        {
                            TypeName = Convert.ToString(dr["TypeName"]),
                            AvailableHours = Convert.ToDecimal(dr["AvailableHours"])
                        }
                    },
                    startDate = Convert.ToDateTime(dr["StartDate"]),
                    endDate = Convert.ToDateTime(dr["EndDate"]),
                    allocation1 = Convert.ToInt32(dr["Allocation"]),

                    Role1 = new Role
                    {
                        roleName = Convert.ToString(dr["roleName"])
                    },

                    technologyGroup = new technologyGroup
                    {
                        techGroupName = Convert.ToString(dr["techGroupName"])
                    },
                    PlannedHours = Convert.ToDecimal(dr["PlannedHours"]),
                    WorkingHours = Convert.ToDecimal(dr["WorkingHours"]),
                    TotalDays = Convert.ToInt32(dr["totaldays"])
                });
            }

            object totalRecordValue = cmd.Parameters["@TotalRecord"].Value;
            int totalRecord;

            if (totalRecordValue != DBNull.Value)
            {
                totalRecord = Convert.ToInt32(totalRecordValue);
            }
            else
            {
                totalRecord = 0;
            }

            return (allocModel, totalRecord);
        }

        public List<TimeEntry> getData(string userid, string StartDate, string EndDate)
        {
            connection();
            List<TimeEntry> timeE = new List<TimeEntry>();
            SqlCommand cmd = new SqlCommand("GetTimeEntriesWithUserName", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StartDate", StartDate);
            cmd.Parameters.AddWithValue("@EndDate", EndDate);
            cmd.Parameters.AddWithValue("@UserId", userid);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                DateTime? punchIn = null;
                DateTime? punchOut = null;
                DateTime? breakDifff = null;
                TimeSpan? totalWorkingHours = null;

                if (dr["PunchIn"] != DBNull.Value && DateTime.TryParse(dr["PunchIn"].ToString(), out DateTime parsedPunchIn))
                {
                    punchIn = parsedPunchIn;
                }

                if (dr["PunchOut"] != DBNull.Value && DateTime.TryParse(dr["PunchOut"].ToString(), out DateTime parsedPunchOut))
                {
                    punchOut = parsedPunchOut;
                }

                if (dr["breakDifff"] != DBNull.Value && DateTime.TryParse(dr["breakDifff"].ToString(), out DateTime parsedBreakDifff))
                {
                    breakDifff = parsedBreakDifff;
                }

                if (dr["TotalWorkingHours"] != DBNull.Value && TimeSpan.TryParse(dr["TotalWorkingHours"].ToString(), out TimeSpan parsedTotalHours))
                {
                    totalWorkingHours = parsedTotalHours;
                }

                timeE.Add(new TimeEntry
                {
                    PunchIn = punchIn ?? DateTime.MinValue,
                    PunchOut = punchOut ?? DateTime.MinValue,
                    breakDifff = breakDifff,
                    TotalTime = totalWorkingHours,
                    AspNetUser = new AspNetUser
                    {
                        Email = Convert.ToString(dr["Email"])
                    }
                });
            }
            return timeE;
        }

        public List<TimeEntry> getDatedata(string userId, string daate)
        {
            connection();
            List<TimeEntry> timeE = new List<TimeEntry>();
            SqlCommand cmd = new SqlCommand("spgetDatedata", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@SelectedDate", daate);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                DateTime? punchIn = null;
                DateTime? punchOut = null;
                DateTime? breakDifff = null;
                TimeSpan? totalWorkingHours = null;

                if (dr["PunchIn"] != DBNull.Value && DateTime.TryParse(dr["PunchIn"].ToString(), out DateTime parsedPunchIn))
                {
                    punchIn = parsedPunchIn;
                }

                if (dr["PunchOut"] != DBNull.Value && DateTime.TryParse(dr["PunchOut"].ToString(), out DateTime parsedPunchOut))
                {
                    punchOut = parsedPunchOut;
                }

                if (dr["breakDifff"] != DBNull.Value && DateTime.TryParse(dr["breakDifff"].ToString(), out DateTime parsedBreakDifff))
                {
                    breakDifff = parsedBreakDifff;
                }

                if (dr["TotalWorkingHours"] != DBNull.Value && TimeSpan.TryParse(dr["TotalWorkingHours"].ToString(), out TimeSpan parsedTotalHours))
                {
                    totalWorkingHours = parsedTotalHours;
                }

                timeE.Add(new TimeEntry
                {
                    PunchIn = punchIn ?? DateTime.MinValue, 
                    PunchOut = punchOut ?? DateTime.MinValue,
                    breakDifff = breakDifff, 
                    TotalTime = totalWorkingHours 
                });
            }

            return timeE;
        }



        public List<TimeEntry> getUserData(string userId)
        {
            connection();
            List<TimeEntry> timeE = new List<TimeEntry>();
            SqlCommand cmd = new SqlCommand("spgetUserData", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            con.Open();
            da.Fill(dt);
            con.Close();

            foreach (DataRow dr in dt.Rows)
            {
                TimeSpan totalWorkingHours;
                if (dr["TotalWorkingHours"] != DBNull.Value && TimeSpan.TryParse(dr["TotalWorkingHours"].ToString(), out totalWorkingHours))
                {
                    TimeSpan breakTime;
                    if (dr["BreakTime"] != DBNull.Value && TimeSpan.TryParse(dr["BreakTime"].ToString(), out breakTime))
                    {
                        timeE.Add(new TimeEntry
                        {
                            PunchIn = Convert.ToDateTime(dr["PunchIn"]),
                            PunchOut = Convert.ToDateTime(dr["PunchOut"]),
                            BreakTTime = breakTime,
                            TotalTime = totalWorkingHours
                        });
                    }
                    else
                    {
                        // Handle the case where BreakTime cannot be parsed
                    }
                }
                else
                {
                    // Handle the case where TotalWorkingHours cannot be parsed
                }
            }
            return timeE;
        }

    }
}