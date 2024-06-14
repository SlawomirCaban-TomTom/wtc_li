using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
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
            tb_start_date.Text = DateTime.Now.ToString("dd MMM yyyy");
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
                    Session["user"] = dt.Rows[0][0].ToString();
                    Session["utc"] = dt.Rows[0][2].ToString();
                    lbl_name.Text = "Hi " + dt.Rows[0][1].ToString() + "!";
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
                    result = true;  
                }
                else
                {
                    lbl_err.Visible = true;
                    lbl_err.Text = "User: " + User.Identity.Name + " not found in WTC database! Please contact TCO!";                    
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
        private void fill_grid()
        {
            DataTable dt3 = new DataTable();
             SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
              string query3 = " select * from v_temp_data where  user_id =" + Session["user"] +" and working_date=cast('"+tb_start_date.Text+"' as date)";
              SqlDataAdapter da3 = new SqlDataAdapter(query3, conn);

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
                          int total = int.Parse(dt3.Rows[i][13].ToString());
                          temp += total;
                          int h = total / 60;
                          int min = total % 60;
                          if (h < 10)
                              time = "0" + h.ToString();
                          else time = h.ToString();
                          if (min < 10)
                              time = time + ":0" + min.ToString();
                          else
                              time = time + ":" + min.ToString();
                          dt3.Rows[i][13] = time;
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
            //lbl_total_work_time.Text = DateTime.Parse(tb_start_date.Text).ToString();

            lbl_total_work_time.Text = query3 ;
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
            ddl_activity.DataTextField = "Activity";
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
            ddl_region.Items.Insert(0, "--Select--");


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
            ddl_sub_region.Items.Insert(0, "--Select--");


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
            ddl_country.DataBind();
            ddl_country.Items.Insert(0, "--Select--");


        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.EnableViewState = true;
            Page.MaintainScrollPositionOnPostBack = true;
            if (!Page.IsPostBack)
            {
                if(check_login())                
                {
                    
                    report_time.Visible = true;
                    fill_planingid();
                    fill_activity();
                    fill_region();
                    fill_sub_region();
                    fill_country();

                    fill_grid();
                  
                }
             
            }

        }
        protected void btn_start_Click(object sender, EventArgs e)
        {
            btn_start.Enabled = false;

            if (Session["working"].ToString() == "1")
            {
                fill_grid();
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
            
            Session["planning_id"] = ddl_planning_id.SelectedValue;

         
        }

        protected void bt_insert_temp_report(object sender, EventArgs e)
        {
            bt_report.Enabled = false;
            string temp = tb_worked_time.Text;
            if (ddl_planning_id.SelectedIndex == 0)
            {
                lbl_err.Visible = true;
                lbl_err.Text = "Please select Planning ID!";

                bt_report.Enabled = true;

            }
            else
            {
                if (ddl_activity.SelectedItem.Value == "0")
                {
                    lbl_err.Visible = true;
                    lbl_err.Text = "Please select Activity!";
                    bt_report.Enabled = true;
                }
                else
                {
                    if (ddl_region.SelectedItem.Value == "0")
                    {
                        lbl_err.Visible = true;
                        lbl_err.Text = "Please select region!";
                        bt_report.Enabled = true;
                    }
                    else
                    {
                        if (ddl_sub_region.SelectedItem.Value == "0")
                        {
                            lbl_err.Visible = true;
                            lbl_err.Text = "Please select sub region!";
                            bt_report.Enabled = true;
                        }
                        else
                        {
                            if (ddl_country.SelectedItem.Value == "0")
                            {
                                lbl_err.Visible = true;
                                lbl_err.Text = "Please select country!";
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
                                        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WTCConnStr"].ConnectionString);
                                        string query = "sp_add_temp_report";
                                        SqlCommand cmd = new SqlCommand(query, conn);
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.Add("user_id", SqlDbType.Int).Value = Session["user"].ToString();
                                        cmd.Parameters.Add("working_date", SqlDbType.Date).Value = DateTime.Parse(tb_start_date.Text);
                                        cmd.Parameters.Add("project_id", SqlDbType.Int).Value = ddl_planning_id.SelectedItem.Value;
                                        cmd.Parameters.Add("activity_id", SqlDbType.Int).Value = ddl_activity.SelectedItem.Value;
                                        cmd.Parameters.Add("region_id", SqlDbType.Int).Value = ddl_region.SelectedItem.Value;
                                        cmd.Parameters.Add("sub_region_id", SqlDbType.Int).Value = ddl_sub_region.SelectedItem.Value;
                                        cmd.Parameters.Add("country_id", SqlDbType.Int).Value = ddl_country.SelectedItem.Value;
                                        cmd.Parameters.Add("duration", SqlDbType.Int).Value = duration;
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
                }
            }
        }
        protected void on_tb_date_update(object sender, EventArgs e)
        {
            if ((tb_start_date.Text.Length > 7) && (tb_end_date.Text.Length > 7))
                {
                if (check_diff(DateTime.Parse(tb_start_date.Text), DateTime.Parse(tb_end_date.Text)) > 14)
                {
                    tb_end_date.Text = string.Empty;
                    lbl_err.Text = "Please chose shorter period!";
                    lbl_err.Visible = true;
                }
                else
                { 
                    lbl_err.Visible = false;
                }
                
            }
        }
        protected double check_diff(DateTime start_date, DateTime end_date)
        {
            return (end_date - start_date).TotalDays;
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
            string temp = tb_start_date.Text;
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