using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PHEDChhattisgarh
{
    public partial class ApprovalProcess : System.Web.UI.Page
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["eMB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("../Login.aspx");
                return;
            }

            string userType = Session["userType"] != null ? Session["userType"].ToString() : null;

            if (userType == null || (userType != "ae" && userType != "ee"))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "Swal.fire('Access Denied','This page is only accessible to AE and EE users','error').then(function(){ window.history.back(); });",
                    true);
                return;
            }

            if (!IsPostBack)
            {
                InitializePage();
                LoadPendingApprovals();
            }
        }

        private void InitializePage()
        {
            string userType = Session["userType"].ToString();
            string userId = Session["UserId"].ToString();

            // Set page title based on user type
            if (userType == "ae")
            {
                lblPageTitle.Text = "AE Approval Process";
            }
            else if (userType == "ee")
            {
                lblPageTitle.Text = "EE Approval Process";
            }

            // Set user info
            lblUserName.Text = userId;
            lblCurrentDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        private void LoadPendingApprovals()
        {
            string userType = Session["userType"].ToString();
            string userId = Session["UserId"].ToString();

            DataTable dt = GetPendingApprovals(userType, userId);

            if (dt.Rows.Count > 0)
            {
                rptPendingApprovals.DataSource = dt;
                rptPendingApprovals.DataBind();
                pnlNoRecords.Visible = false;

                // Update statistics
                lblTotalPending.Text = dt.Rows.Count.ToString();
            }
            else
            {
                rptPendingApprovals.DataSource = null;
                rptPendingApprovals.DataBind();
                pnlNoRecords.Visible = true;
                lblTotalPending.Text = "0";
            }

            // Load today's approved count
            LoadTodayApprovedCount(userType, userId);
        }

        private DataTable GetPendingApprovals(string userType, string userId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = string.Empty;

                if (userType == "ae")
                {
                    // AE can see books where DateSE is not null, DateAE is null, and they are the assigned AE
                    query = @"
                        SELECT [embbookid], [WorkCode], [se], [ae], [ee], 
                               [DateSE], [DateAE], [DateEE]
                        FROM [JJM].[dbo].[eMB_Locks] 
                        WHERE [ae] = @userId 
                        AND [DateSE] IS NOT NULL 
                        AND [DateAE] IS NULL 
                        AND [DateEE] IS NULL
                        ORDER BY [DateSE] DESC";
                }
                else if (userType == "ee")
                {
                    // EE can see books where DateSE and DateAE are not null, DateEE is null, and they are the assigned EE
                    query = @"
                        SELECT [embbookid], [WorkCode], [se], [ae], [ee], 
                               [DateSE], [DateAE], [DateEE]
                        FROM [JJM].[dbo].[eMB_Locks] 
                        WHERE [ee] = @userId 
                        AND [DateSE] IS NOT NULL 
                        AND [DateAE] IS NOT NULL 
                        AND [DateEE] IS NULL
                        ORDER BY [DateAE] DESC";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return dt;
        }

        private void LoadTodayApprovedCount(string userType, string userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = string.Empty;

                if (userType == "ae")
                {
                    query = @"
                        SELECT COUNT(*) 
                        FROM [JJM].[dbo].[eMB_Locks] 
                        WHERE [ae] = @userId 
                        AND [DateAE] IS NOT NULL 
                        AND CAST([DateAE] AS DATE) = CAST(GETDATE() AS DATE)";
                }
                else if (userType == "ee")
                {
                    query = @"
                        SELECT COUNT(*) 
                        FROM [JJM].[dbo].[eMB_Locks] 
                        WHERE [ee] = @userId 
                        AND [DateEE] IS NOT NULL 
                        AND CAST([DateEE] AS DATE) = CAST(GETDATE() AS DATE)";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    int count = (int)cmd.ExecuteScalar();
                    lblTotalApproved.Text = count.ToString();
                }
            }
        }

        protected void rptPendingApprovals_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string[] args = e.CommandArgument.ToString().Split(',');
            string embbookid = args[0];
            string workCode = args[1];

            if (e.CommandName == "ViewBook")
            {
                // Redirect to the Components page for viewing
                Response.Redirect("GenerateBill.aspx?embbookid=" + embbookid + "&workcode=" + workCode);
            }
        }

        private void ApproveBook(string embbookid, string workCode)
        {
            string userType = Session["userType"].ToString();
            string userId = Session["UserId"].ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string updateQuery = string.Empty;
                string successMessage = string.Empty;

                if (userType == "ae")
                {
                    updateQuery = @"
                UPDATE [JJM].[dbo].[eMB_Locks] 
                SET [DateAE] = @currentDate
                WHERE [embbookid] = @embbookid 
                AND [WorkCode] = @workcode
                AND [ae] = @userId
                AND [DateSE] IS NOT NULL 
                AND [DateAE] IS NULL 
                AND [DateEE] IS NULL";
                    successMessage = "Book approved successfully by AE!";
                }
                else if (userType == "ee")
                {
                    updateQuery = @"
                UPDATE [JJM].[dbo].[eMB_Locks] 
                SET [DateEE] = @currentDate
                WHERE [embbookid] = @embbookid 
                AND [WorkCode] = @workcode
                AND [ee] = @userId
                AND [DateSE] IS NOT NULL 
                AND [DateAE] IS NOT NULL 
                AND [DateEE] IS NULL";
                    successMessage = "Book approved successfully by EE! Lock cycle completed.";
                }

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@embbookid", embbookid);
                    cmd.Parameters.AddWithValue("@workcode", workCode);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@currentDate", DateTime.Now);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        string script = "Swal.fire('Success', '" + successMessage + "', 'success');";
                        ScriptManager.RegisterStartupScript(this, GetType(), "success", script, true);

                        // Refresh data
                        LoadPendingApprovals();
                    }
                    else
                    {
                        string errorScript = "Swal.fire('Error', 'Unable to approve the book. Please try again.', 'error');";
                        ScriptManager.RegisterStartupScript(this, GetType(), "error", errorScript, true);
                    }
                }
            }
        }


        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPendingApprovals();
            ScriptManager.RegisterStartupScript(this, GetType(), "refresh",
                "Swal.fire('Refreshed', 'Data has been refreshed successfully!', 'info');", true);
        }

        // Optional: Add method to check if user can view specific book
        private bool CanUserViewBook(string embbookid, string workCode, string userType, string userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = string.Empty;

                if (userType == "ae")
                {
                    query = @"
                        SELECT COUNT(*) 
                        FROM [JJM].[dbo].[eMB_Locks] 
                        WHERE [embbookid] = @embbookid 
                        AND [WorkCode] = @workcode
                        AND [ae] = @userId
                        AND [DateSE] IS NOT NULL 
                        AND [DateAE] IS NULL";
                }
                else if (userType == "ee")
                {
                    query = @"
                        SELECT COUNT(*) 
                        FROM [JJM].[dbo].[eMB_Locks] 
                        WHERE [embbookid] = @embbookid 
                        AND [WorkCode] = @workcode
                        AND [ee] = @userId
                        AND [DateAE] IS NOT NULL 
                        AND [DateEE] IS NULL";
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@embbookid", embbookid);
                    cmd.Parameters.AddWithValue("@workcode", workCode);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}