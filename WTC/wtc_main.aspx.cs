using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Globalization;
namespace TomTom_Info_Page.WTC
{
    public partial class wtc_main : System.Web.UI.Page
    {
        public static DataTable reported;


        public static int reported_time;
        public static DateTime p_time;
        public static DateTime r_time;
        public static TimeSpan p_span;
        public bool check_login()
        {
            int role_id = 0;
            bool result = false;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
            string query = "select u.user_id,first_name,utc from users u join user_team ut on u.user_id=ut.user_id join teams t on ut.team_id=t.team_id where domain_username='" + User.Identity.Name + "' ";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                //da.FillSchema(ds, SchemaType.Source, "project_types_id");
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string timelimit = "00:00:01";
                    DateTime start = DateTime.Now;
                    string end = DateTime.Now.ToString("dd/MMM/yyyy") + " " + timelimit;
                    DateTime tend = DateTime.ParseExact(end, "dd/MMM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                    TimeSpan span = tend - start;
                    Session["user"] = dt.Rows[0][0].ToString();
                    Session["utc"] = dt.Rows[0][2].ToString();
                    string timelimit2 = "23:59:59";
                    DateTime start2 = DateTime.Now;
                    string end2 = DateTime.Now.ToString("dd/MMM/yyyy") + " " + timelimit2;
                    DateTime tend2 = DateTime.ParseExact(end2, "dd/MMM/yyyy HH:mm:ss", new CultureInfo("en-US"));
                    TimeSpan span2 = tend2 - start2;

                    lbl_name.Text = "Hi " + dt.Rows[0][1].ToString() + "!";
                    string query2 = "select timesheet_id,start_time_server,convert(nvarchar, start_time_local ,120) from timesheet where user_id =" + Session["user"] + " and end_time_local is null";
                    string query3 = "select top (1)role_id from user_role where user_id =" + Session["user"] + " order by 1 desc";
                    SqlCommand cmd3 = new SqlCommand(query3, conn);
                    object test = cmd3.ExecuteScalar();
                    if (test != null)
                        int.TryParse(test.ToString(), out role_id);
                    if (role_id != 0)
                    {
                        menu_std.Visible = false;
                        Menu_mng.Visible = true;
                    }
                    SqlDataAdapter da2 = new SqlDataAdapter(query2, conn);
                    DataTable dt2 = new DataTable();
                    da2.Fill(dt2);
                    if (dt2.Rows.Count > 0)
                    {

                        Session["working"] = 1;
                        Session["timesheet"] = dt2.Rows[0][0].ToString();
                        lbl_date.Text = dt2.Rows[0][1].ToString();
                        //     DateTime login_= DateTime.Parse(dt2.Rows[0][1].ToString());
                        //  TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                        //          DateTime login_tz = TimeZoneInfo.ConvertTimeFromUtc(login_, cstZone);
                        DateTime login_tz = DateTime.Parse(dt2.Rows[0][1].ToString());
                        Session["login_time"] = login_tz;

                        // Timer1.Enabled = true;
                        //DateTime.ParseExact(dt2.Rows[0][1].ToString(),"yyyy-MM-dd HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture);

                        //DateTime login_ = DateTime.Parse(Session["login_time"].ToString());
                        // TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                        // DateTime login_tzz = TimeZoneInfo.ConvertTimeFromUtc(login_, cstZone);



                        lbl_date.Text = "Start time: " + dt2.Rows[0][2].ToString();
                        //.ToString("yyyy-MM-dd HH:mm:ss");
                        result = true;
                    }
                    else

                        Session["working"] = 0;
                }


                else
                {
                    lbl_err.Visible = true;
                    lbl_err.Text = "User: " + User.Identity.Name + " not found in WTC database! Please contact TCO!";
                    btn_start.Visible = false;
                }
            }
            catch (Exception ex)
            {
                lbl_err.Visible = true;
                lbl_err.Text = ex.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            return result;
        }
        private void fill_grid(int state)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
            string query3 = " select task_id,t.project_id, project_name,t.task_type_id,task_type_name,t.sub_task_id,sub_task_name,  cAST(duration AS NVARCHAR) as duration,description from task t join project p on t.project_id = p.project_id join             task_type ta on t.task_type_id = ta.task_type_id join sub_task st on t.sub_task_id = st.sub_task_id join int_project_task_type iptt on t.project_id = iptt.project_id and t.task_type_id = iptt.task_type_id where  timesheet_id =" + Session["timesheet"];
            SqlDataAdapter da3 = new SqlDataAdapter(query3, conn);
            DataTable dt3 = new DataTable();
            try
            {
                int temp = 0;
                conn.Open();
                da3.Fill(dt3);

                if (dt3.Rows.Count > 0)
                {


                    for (int i = 0; i < dt3.Rows.Count; i++)
                    {
                        string time = string.Empty;

                        int total = int.Parse(dt3.Rows[i][7].ToString());
                        temp = temp + total;
                        int h = total / 60;
                        int min = total % 60;
                        if (h < 10)
                            time = "0" + h.ToString();
                        else time = h.ToString();
                        if (min < 10)
                            time = time + ":0" + min.ToString();
                        else
                            time = time + ":" + min.ToString();

                        dt3.Rows[i][7] = time;
                    }

                    string rt = string.Empty;
                    reported_time = temp;
                    Session["reported_time"] = temp;
                    int h2 = temp / 60;
                    int min2 = temp % 60;
                    if (h2 < 10)
                        rt = "0" + h2.ToString();
                    else rt = h2.ToString();
                    if (min2 < 10)
                        rt = rt + ":0" + min2.ToString();
                    else
                        rt = rt + ":" + min2.ToString();

                    lbl_total_reported.Text = rt;

                }
                else
                {

                    lbl_total_reported.Text = "00:00";
                    reported_time = temp;
                    Session["reported_time"] = temp;
                }

                TimeSpan span = DateTime.UtcNow.Subtract(DateTime.Parse(Session["login_time"].ToString()));
                int totalminutes = (int)span.TotalMinutes;
                int hw = totalminutes / 60;
                int minw = totalminutes % 60;
                string tempw = string.Empty;
                if (hw < 10)
                    tempw = "0" + hw;
                else
                    tempw = hw.ToString();
                if (minw < 10)
                    tempw = tempw + ":0" + minw;
                else
                    tempw = tempw + ":" + minw;
                lbl_total_work_time.Text = tempw;
                int totalminutesleft = totalminutes - reported_time;
                if (totalminutesleft <= 0)
                    tb_time_left.Text = "00:00";
                else
                {
                    if (state > 0)
                    {
                        int h2 = totalminutesleft / 60;
                        int min2 = totalminutesleft % 60;

                        string temp2 = string.Empty;
                        if (h2 < 10)
                            temp2 = "0" + h2;
                        else
                            temp2 = h2.ToString();
                        if (min2 < 10)
                            temp2 = temp2 + ":0" + min2;
                        else
                            temp2 = temp2 + ":" + min2;

                        tb_time_left.Text = temp2;
                    }
                }
                reported = dt3;
                Session["reported"] = dt3;
                gv_reported_time.DataSource = reported;
                gv_reported_time.DataBind();

            }
            catch (Exception ex)
            {
                lbl_err.Visible = true;
                lbl_err.Text = ex.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

        }

        private void fill_planingid()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
            string query = string.Empty;
            if (tb_mask.Text.Trim().Length < 1)
                query = "select project_id,project_name planning_id from project where is_active=1 order by project_name";
             else
                query = "select distinct project_id,project_name planning_id from project where is_active=1 and LOWER( task_type_name) like '%" + tb_mask.Text.Trim().ToLower() + "%')order by project_name";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                //da.FillSchema(ds, SchemaType.Source, "project_types_id");
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                lbl_err.Visible = true;
                lbl_err.Text = ex.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
            if (dt.Rows.Count > 0)
            {
                ddl_planning_id.DataSource = dt;
                ddl_planning_id.DataValueField = "project_id";
                ddl_planning_id.DataTextField = "planning_id";
                ddl_planning_id.DataBind();
                ddl_planning_id.Items.Insert(0, "--Select--");
                if (ddl_planning_id.SelectedIndex == 0)
                {
                    ddl_activity.Enabled = false;
                }
            }
            else
            {
                lbl_err.Visible = true;
                lbl_err.Text = "No projects registered!";
            }
        }

        private void fill_activity()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
            string query = string.Empty;
            query = "select task_type_id,task_type_name Activity from task_type order by task_type_name";

            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                //da.FillSchema(ds, SchemaType.Source, "project_types_id");
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                lbl_err.Visible = true;
                lbl_err.Text = ex.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            ddl_activity.DataSource = dt;
            ddl_activity.DataValueField = "task_type_id";
            ddl_activity.DataTextField = "task_type_name";
            ddl_activity.DataBind();
                ListItem l = new ListItem("--Select--", "0");
            ddl_activity.Items.Insert(0, l);
            
             ddl_activity.Enabled = true;
        }
        private void fill_region()
        {
SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
            string query = string.Empty;
            query = "select region_id,region_name from region";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                //da.FillSchema(ds, SchemaType.Source, "project_types_id");
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                lbl_err.Visible = true;
                lbl_err.Text = ex.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            ddl_region.DataSource = dt;
            ddl_region.DataValueField = "region_id";
            ddl_region.DataTextField = "region_name";
            ddl_region.DataBind();


        }
        private void fill_sub_region()
        {

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
            string query = string.Empty;
            query = "select sub_region_id,sub_region_name from sub_region";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                //da.FillSchema(ds, SchemaType.Source, "project_types_id");
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                lbl_err.Visible = true;
                lbl_err.Text = ex.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            ddl_sub_region.DataSource = dt;
            ddl_sub_region.DataValueField = "sub_region_id";
            ddl_sub_region.DataTextField = "sub_region_name";
            ddl_sub_region.DataBind();


        }
        private void fill_country()
        {

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
            string query = string.Empty;
            query = "select country_id,country_name from country";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            try
            {
                conn.Open();
                //da.FillSchema(ds, SchemaType.Source, "project_types_id");
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                lbl_err.Visible = true;
                lbl_err.Text = ex.ToString();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            ddl_country.DataSource = dt;
            ddl_country.DataValueField = "country_id";
            ddl_country.DataTextField = "country_name";
            ddl_country.DataBind();


        }

        protected void Page_Load(object sender, EventArgs e)
        {

            btn_start.Enabled = true;
            //Timer1.Enabled = false;
            Session.Timeout = 180;
            Page.EnableViewState = true;
            Page.MaintainScrollPositionOnPostBack = true;
            int state = 0;
            if (!Page.IsPostBack)
            {
                //ddl_sub_task.Visible = true;
                if (check_login())
                {
                    if (Session["utc"].ToString() == "270")
                        bt_break.Text = "Report 60 min break";
                    else
                        bt_break.Text = "Report 35 min break";
                    // Timer1_Tick();
                    lbl_date.Visible = true;
                    btn_start.Text = "Stop Work";
                    report_time.Visible = true;
                    fill_planingid();
                    fill_region();
                    fill_sub_region();
                    fill_country();

                    state++;

                    fill_grid(state);
                    /*if ((!string.IsNullOrEmpty((string)Page.Session["task_id"])) && (!string.IsNullOrEmpty((string)Page.Session["project_id"])) && (!string.IsNullOrEmpty((string)Page.Session["sub_task_id"])))
                    {
                        ddl_project.SelectedValue = Session["project_id"].ToString();
                        fill_task();
                        ddl_task.SelectedValue = Session["task_id"].ToString();
                        ddl_sub_task.SelectedValue = Session["sub_task_id"].ToString();
                        if (ddl_project.SelectedValue == "1")
                        {
                            ddl_sub_task.Enabled = false;

                            ddl_sub_task.Visible = false;
                        }
                        else
                        {
                            ddl_sub_task.Visible = true;
                            ddl_sub_task.Enabled = true;
                        }
                    }*/
                }
                else
                {

                    lbl_date.Visible = false;
                    Session["working"] = 0;
                    report_time.Visible = false;
                    btn_start.Text = "Start Work";

                }
            }
            else
            {
                if (Session["working"].ToString() == "1")
                {
                    // Timer1_Tick();

                    fill_grid(state);
                }
            }

        }
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                // System.Threading.Thread.Sleep(100);
                TimeSpan span = DateTime.UtcNow.Subtract(DateTime.Parse(Session["login_time"].ToString()));
                int totalsec = (int)span.TotalSeconds;
                int h = totalsec / 3600;
                int min = (totalsec - (3600 * h)) / 60;
                int sec = (totalsec - (3600 * h) - 60 * min);
                string temp = string.Empty;
                if (h < 10)
                    temp = "0" + h;
                else
                    temp = h.ToString();
                if (min < 10)
                    temp = temp + ":0" + min;
                else
                    temp = temp + ":" + min;
                if (sec < 10)
                    temp = temp + ":0" + sec;
                else
                    temp = temp + ":" + sec;
                lbl_total_work_time.Text = temp;
            }
            catch (Exception ex)
            {
                lbl_err.Visible = true;
                lbl_err.Text = ex.ToString();

            }
        }

        protected void btn_start_Click(object sender, EventArgs e)
        {
            btn_start.Enabled = false;

            if (Session["working"].ToString() == "1")
            {
                fill_grid(0);
                TimeSpan span = DateTime.Now.Subtract(DateTime.Parse(Session["login_time"].ToString()));
                int totalminutes = (int)span.TotalMinutes;
                int totalminutesleft = totalminutes - reported_time;
                if (Math.Abs(totalminutesleft) <= 5)
                {
                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
                    string query = "sp_add_WTC_User_Logout_Info";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("user_id", SqlDbType.Int).Value = Session["user"];
                    cmd.Parameters.Add("recordID", SqlDbType.Int).Value = Session["timesheet"];
                    cmd.Parameters.Add("logout_workstation", SqlDbType.VarChar, 25).Value = Request.UserHostName;
                    cmd.Parameters.Add("logout_by", SqlDbType.VarChar, 20).Value = User.Identity.Name;
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        Session["working"] = 0;
                        conn.Close();
                        conn.Dispose();
                        Server.Transfer("~/wtc/enjoy.aspx");
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        conn.Dispose();
                        lbl_err.Visible = true;
                        lbl_err.Text = ex.ToString() + query;
                    }
                }
                else
                {
                    lbl_err.Visible = true;
                    lbl_err.Text = "Please report out all worked time and then click \"Stop work\"!";
                }
            }
            else
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
                string query = "sp_add_WTC_User_Login_Info";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("user_id", SqlDbType.Int).Value = Session["user"];
                cmd.Parameters.Add("login_workstation", SqlDbType.VarChar, 25).Value = Request.UserHostName;
                cmd.Parameters.Add("login_by", SqlDbType.VarChar, 20).Value = User.Identity.Name;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Session["login_time"] = DateTime.Now.ToString();
                    Server.Transfer("~/wtc/time.aspx");
                }
                catch (Exception ex)
                {
                    lbl_err.Visible = true;
                    lbl_err.Text = ex.ToString() + query;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }

            }

        }

        protected void ddl_planning_SelectedIndexChanged(object sender, EventArgs e)
        {
            fill_activity();
            Session["planning_id"] = ddl_planning_id.SelectedValue;

         
        }

        protected void Unnamed7_Click(object sender, EventArgs e)
        {
            bt_report.Enabled = false;
            string temp = tb_time_left.Text;
            if (ddl_planning_id.SelectedIndex == 0)
            {
                lbl_err.Visible = true;
                lbl_err.Text = "Please select project!";

                bt_report.Enabled = true;

            }
            else
            {
                if (ddl_activity.SelectedItem.Value == "0")
                {
                    lbl_err.Visible = true;
                    lbl_err.Text = "Please select task!";
                    bt_report.Enabled = true;
                }
                else
                {
                    if (Regex.IsMatch(temp, "^[0-9]+:[0-9]{2}$"))
                    {
                        string[] time = temp.Split(':');
                        int duration = 60 * int.Parse(time[0]) + int.Parse(time[1]);
                        if (duration == 0)
                        {
                            lbl_err.Visible = true;
                            lbl_err.Text = "Please report more time!";
                            bt_report.Enabled = true;
                        }
                        else
                        {
                            TimeSpan span = DateTime.Now.Subtract(DateTime.Parse(Session["login_time"].ToString()));

                            if (span.TotalMinutes - reported_time - duration < -6)
                            {
                                lbl_err.Visible = true;
                                lbl_err.Text = "You are trying too report more than you have been working today! total:" + span.TotalMinutes + " reported:" + reported_time + " duration:" + duration;
                                bt_report.Enabled = true;
                            }
                            else
                            {

                                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
                                string query = "sp_add_TASK";
                                SqlCommand cmd = new SqlCommand(query, conn);
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("project", SqlDbType.Int).Value = ddl_planning_id.SelectedValue;
                                cmd.Parameters.Add("task_type", SqlDbType.Int).Value = ddl_activity.SelectedValue;
                                cmd.Parameters.Add("region", SqlDbType.Int).Value = ddl_region.SelectedValue;
                                cmd.Parameters.Add("duration", SqlDbType.Int).Value = duration;
                                cmd.Parameters.Add("timesheet", SqlDbType.Int).Value = Session["timesheet"];
                                cmd.Parameters.Add("description", SqlDbType.NVarChar, 400).Value = tb_desc.Text;
                                try
                                {
                                    conn.Open();
                                    cmd.ExecuteNonQuery();
                                    conn.Close();
                                    conn.Dispose();
                                    Response.Redirect(Request.RawUrl);
                                }
                                catch (Exception ex)
                                {
                                    conn.Close();
                                    conn.Dispose();
                                    lbl_err.Visible = true;
                                    lbl_err.Text = ex.ToString() + query;
                                    bt_report.Enabled = true;
                                }

                            }
                        }

                    }
                    else
                    {
                        lbl_err.Visible = true;
                        lbl_err.Text = "Reported time contains syntax error!";
                    }
                }
            }
        }

        protected void gvVochByDate_RowCommand(object sender, GridViewDeleteEventArgs e)
        {
            DataTable temp = (DataTable)Session["reported"];
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
            string query = "sp_del_reported_task";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("task_id", SqlDbType.Int).Value = temp.Rows[e.RowIndex][0].ToString();
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                //fill_grid(1);
                Response.Redirect(Request.RawUrl);
            }
            catch (Exception ex)
            {
                conn.Close();
                conn.Dispose();
                lbl_err.Text = ex.ToString() + query;
                lbl_err.Visible = true;
            }
        }

        protected void ddl_activity_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["activity_id"] = ddl_activity.SelectedValue;
            Session["region"] = null;
            Session["sub_region"] = null;
            Session["country"] = null;
        }
        
        protected void tb_mask_TextChanged(object sender, System.EventArgs e)
        {
            fill_planingid();
        }
        protected void Unnamed10_Click(object sender, EventArgs e)
        {

            bt_break.Enabled = false;
            string temp = tb_time_left.Text;
            int duration = 35;
            if (Session["utc"].ToString() == "270")
                duration = 60;
            TimeSpan span = DateTime.Now.Subtract(DateTime.Parse(Session["login_time"].ToString()));
            if (span.TotalMinutes - reported_time - duration < -6)
            {
                lbl_err.Visible = true;
                lbl_err.Text = "You are trying too report more than you have been working today! total:" + span.TotalMinutes + " reported:" + reported_time + " duration:" + duration;
                bt_break.Enabled = true;
            }
            else
            {



                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
                string query = "sp_add_TASK";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("project", SqlDbType.Int).Value = 2;
                cmd.Parameters.Add("task_type", SqlDbType.Int).Value = 2;
                cmd.Parameters.Add("sub_task", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("duration", SqlDbType.Int).Value = duration;
                cmd.Parameters.Add("timesheet", SqlDbType.Int).Value = Session["timesheet"];
                cmd.Parameters.Add("description", SqlDbType.NVarChar, 400).Value = string.Empty;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                    Response.Redirect(Request.RawUrl);
                }
                catch (Exception ex)
                {
                    conn.Close();
                    conn.Dispose();
                    lbl_err.Visible = true;
                    lbl_err.Text = ex.ToString() + query;
                    bt_break.Enabled = true;
                }
            }

        }

        protected void Unnamed6_Click(object sender, EventArgs e)
        {
            fill_planingid();

        }
        protected void Buttonref_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);

        }
        

        protected void Button2_Click(object sender, EventArgs e)
        {
            fill_planingid();
        }

       
    }
}