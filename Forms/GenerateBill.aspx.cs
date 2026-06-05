using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
public class BillApprovalChecker
{
    private string connectionString = ConfigurationManager.ConnectionStrings["eMB"].ConnectionString;

    public enum BillStatus
    {
        Draft,
        Approved,
        Rejected,
        PendingApproval
    }
    public bool IsBillFullyApproved(int billId)
    {
        string query = "SELECT COUNT(*) FROM [JJM].[dbo].[eMB_Locks] WHERE [BillEE] = @BillId";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BillId", billId);

                try
                {
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error checking bill approval status: " + ex.Message);
                }
            }
        }
    }
    public bool IsBillApprovedBySE(int billId)
    {
        string query = "SELECT COUNT(*) FROM [JJM].[dbo].[eMB_Locks] WHERE [BillSE] = @BillId";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BillId", billId);

                try
                {
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error checking SE approval status: " + ex.Message);
                }
            }
        }
    }
    public bool IsBillApprovedByAE(int billId)
    {
        string query = "SELECT COUNT(*) FROM [JJM].[dbo].[eMB_Locks] WHERE [BillAE] = @BillId";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BillId", billId);

                try
                {
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error checking AE approval status: " + ex.Message);
                }
            }
        }
    }
    public int GetMostRecentBillId()
    {
        string query = "SELECT TOP 1 BillId FROM [JJM].[dbo].[BillMaster] ORDER BY BillDate DESC, BillId DESC";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    return result != null ? (int)result : 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error getting most recent bill ID: " + ex.Message);
                }
            }
        }
    }
    public bool HasNewerProcessedBill(int billId, string userType)
    {
        string query = "";

        switch (userType.ToUpper())
        {
            case "SE":
                query = @"SELECT COUNT(*) FROM [JJM].[dbo].[eMB_Locks] 
                         WHERE [BillSE] > @BillId AND [BillSE] IS NOT NULL";
                break;
            case "AE":
                query = @"SELECT COUNT(*) FROM [JJM].[dbo].[eMB_Locks] 
                         WHERE [BillAE] > @BillId AND [BillAE] IS NOT NULL";
                break;
            case "EE":
                query = @"SELECT COUNT(*) FROM [JJM].[dbo].[eMB_Locks] 
                         WHERE [BillEE] > @BillId AND [BillEE] IS NOT NULL";
                break;
        }

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@BillId", billId);

                try
                {
                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error checking newer processed bills: " + ex.Message);
                }
            }
        }
    }
    public BillStatus GetBillStatus(int billId, string userType)
    {
        bool approvedBySE = IsBillApprovedBySE(billId);
        bool approvedByAE = IsBillApprovedByAE(billId);
        bool approvedByEE = IsBillFullyApproved(billId);
        bool isMostRecent = (billId == GetMostRecentBillId());

        // If fully approved by EE, it's approved
        if (approvedByEE)
        {
            return BillStatus.Approved;
        }

        // If not processed by anyone, it's a draft
        if (!approvedBySE && !approvedByAE && !approvedByEE)
        {
            if (isMostRecent)
            {
                return BillStatus.PendingApproval;
            }
            return BillStatus.Draft;
        }

        // Check rejection scenarios based on user type
        switch (userType.ToUpper())
        {
            case "SE":
                // For SE: if bill was approved by SE but there's a newer bill processed by AE/EE, it's rejected
                if (approvedBySE && !isMostRecent)
                {
                    return BillStatus.Rejected;
                }
                // If it's the most recent and not processed by SE, show lock & forward
                if (isMostRecent && !approvedBySE)
                {
                    return BillStatus.PendingApproval;
                }
                if (!isMostRecent && !approvedBySE)
                {
                    return BillStatus.Draft;
                }
                break;

            case "AE":
                // For AE: if bill was approved by AE but there's a newer bill processed by EE, it's rejected
                if (approvedByAE && !isMostRecent && HasNewerProcessedBill(billId, "EE"))
                {
                    return BillStatus.Rejected;
                }
                // If bill is approved by SE and is most recent and not processed by AE, show lock & forward
                if (approvedBySE && isMostRecent && !approvedByAE)
                {
                    return BillStatus.PendingApproval;
                }
                // If bill is approved by SE but not by AE and there's a newer bill processed by AE, it's rejected
                if (approvedBySE && !approvedByAE && HasNewerProcessedBill(billId, "AE"))
                {
                    return BillStatus.Rejected;
                }
                break;

            case "EE":
                // For EE: if bill is approved by AE and is most recent and not processed by EE, show lock & forward
                if (approvedByAE && isMostRecent && !approvedByEE)
                {
                    return BillStatus.PendingApproval;
                }
                // If bill is approved by AE but not by EE and there's a newer bill processed by EE, it's rejected
                if (approvedByAE && !approvedByEE && HasNewerProcessedBill(billId, "EE"))
                {
                    return BillStatus.Rejected;
                }
                break;
        }

        // Default case: if bill has some approval but doesn't meet pending criteria, it's rejected
        if (approvedBySE || approvedByAE)
        {
            return BillStatus.Rejected;
        }

        return BillStatus.Draft;
    }

    /// <summary>
    /// Determines what to show for a bill in the GridView
    /// </summary>
    /// <param name="billId">The bill ID</param>
    /// <param name="userType">Current user type</param>
    /// <returns>String indicating what to show ("LockForward", "Draft", "Approved", "Rejected")</returns>
    public string GetBillDisplayAction(int billId, string userType)
    {
        BillStatus status = GetBillStatus(billId, userType);

        switch (status)
        {
            case BillStatus.PendingApproval:
                return "LockForward";
            case BillStatus.Draft:
                return "Draft";
            case BillStatus.Approved:
                return "Approved";
            case BillStatus.Rejected:
                return "Rejected";
            default:
                return "Draft";
        }
    }
}
public partial class GenerateBill : Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserId"] == null)
        {
            Response.Redirect("../Login.aspx");
            return;
        }
        string userType = Session["userType"] != null ? Session["userType"].ToString() : null;

        if (userType == null || (userType != "se" && userType != "ae" && userType != "ee"))
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                "Swal.fire('Access Denied','Invalid user type','error').then(function(){ window.history.back(); });",
                true);
            return;
        }

        // Get query parameters
        string embbookid = Request.QueryString["embbookid"];
        string workCode = Request.QueryString["workcode"];

        // Check current book status and determine access
        string currentStatus = GetCurrentBookStatus(embbookid, workCode);

        if (!CanUserAccess(userType, currentStatus))
        {
            string message = GetAccessDeniedMessage(userType, currentStatus);
            string script = "Swal.fire('Access Denied','" + message + "','error').then(function(){ window.history.back(); });";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", script, true);

            return;
        }
        if (!IsPostBack)
        {
            string mode = Request.QueryString["mode"];
            string wc = Request.QueryString["workCode"];
            string workcode = Request.QueryString["workcode"];
            if (mode == "new" && !String.IsNullOrEmpty(wc))
            {
                txtWorkCode.Text = wc;
                pnlSearch.Visible = false;
                pnlHistory.Visible = false;
                mvBill.Visible = true;
                mvBill.ActiveViewIndex = 0;
                SetSubdivisionAndDivision(wc);
                return;
            }
            else if (!string.IsNullOrEmpty(embbookid) && !string.IsNullOrEmpty(workcode))
            {
                txtWorkCode.Text = workcode;
                ShowBillHistoryForWorkCode(workcode);
            }
            else
            {
                // Default view
                pnlSearch.Visible = true;
                pnlHistory.Visible = false;
                mvBill.Visible = false;
            }
            ViewState["ModalOpen"] = false;
        }
        else
        {
            bool modalOpen = ViewState["ModalOpen"] != null && (bool)ViewState["ModalOpen"];
            if (modalOpen)
            {
                mvBill.Visible = true;
                modalBg.Style["display"] = "block";
                pnlBillPopup.Style["display"] = "block";
                CalculateMemo();
            }
        }
    }

    private string GetCurrentBookStatus(string embbookid, string workCode)
    {
        if (string.IsNullOrEmpty(embbookid) || string.IsNullOrEmpty(workCode))
            return "UNLOCKED"; // No active record, SE can lock
        string connectionString = ConfigurationManager.ConnectionStrings["eMB"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            // Get the most recent record (active record with at least one NULL date)
            string query = @"
            SELECT TOP 1 [DateSE], [DateAE], [DateEE]
            FROM [JJM].[dbo].[eMB_Locks] 
            WHERE [embbookid] = @embbookid AND [WorkCode] = @workcode
            AND ([DateSE] IS NULL OR [DateAE] IS NULL OR [DateEE] IS NULL)
            ORDER BY [DateSE] DESC"; // Get most recent active record

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@embbookid", embbookid);
                cmd.Parameters.AddWithValue("@workcode", workCode);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        bool hasDateSE = reader["DateSE"] != DBNull.Value;
                        bool hasDateAE = reader["DateAE"] != DBNull.Value;
                        bool hasDateEE = reader["DateEE"] != DBNull.Value;

                        if (hasDateSE && hasDateAE && hasDateEE)
                        {
                            return "UNLOCKED"; // All dates filled, cycle complete
                        }
                        else if (hasDateSE && hasDateAE && !hasDateEE)
                        {
                            return "LOCKED_BY_AE"; // Waiting for EE approval
                        }
                        else if (hasDateSE && !hasDateAE && !hasDateEE)
                        {
                            return "LOCKED_BY_SE"; // Waiting for AE approval
                        }
                        else
                        {
                            return "UNLOCKED"; // Invalid state, treat as unlocked
                        }
                    }
                    else
                    {
                        return "UNLOCKED"; // No active record found
                    }
                }
            }
        }
    }

    private bool CanUserAccess(string userType, string bookStatus)
    {
        switch (bookStatus)
        {
            case "UNLOCKED":
                return userType == "se"; // Only SE can access unlocked books
            case "LOCKED_BY_SE":
                return userType == "ae"; // Only AE can access books locked by SE
            case "LOCKED_BY_AE":
                return userType == "ee"; // Only EE can access books locked by AE
            default:
                return false;
        }
    }

    private string GetAccessDeniedMessage(string userType, string bookStatus)
    {
        switch (bookStatus)
        {
            case "UNLOCKED":
                if (userType == "ae") return "This book is not locked by SE yet. Only SE can access unlocked books.";
                if (userType == "ee") return "This book is not ready for EE approval. AE must approve first.";
                break;
            case "LOCKED_BY_SE":
                if (userType == "se") return "You have already locked this book. Wait for AE and EE approval before you can access it again.";
                if (userType == "ee") return "This book is waiting for AE approval. EE cannot access until AE approves.";
                break;
            case "LOCKED_BY_AE":
                if (userType == "se") return "This book is locked and waiting for EE approval. You cannot access until the approval cycle completes.";
                if (userType == "ae") return "You have already approved this book. Wait for EE approval before next cycle.";
                break;
        }
        return "You do not have permission to access this book at this time.";
    }
    private void ShowBillHistoryForWorkCode(string workCode)
    {
        SetSubdivisionAndDivision(workCode);
        DataTable history = GetBillHistory(workCode);
        gvHistory.DataSource = history;
        gvHistory.DataBind();

        pnlSearch.Visible = false;
        pnlHistory.Visible = true;
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string workCode = txtWorkCode.Text.Trim();
        string embbookid = getEmbBookId(workCode);
        if (String.IsNullOrEmpty(workCode))
        {
            ShowAlert("Please enter a Work Code.");
            return;
        }
        if(String.IsNullOrEmpty(embbookid))
        {
            ShowAlert("Please enter a valid Work Code. No eMB book found!");
            return;
        }
        Response.Redirect("GenerateBill.aspx?embbookid="+embbookid+"&workcode="+workCode);
        return;
    }
    protected void CalculateMemo()
    {
        decimal sorPct = GetSORPercentage();
        decimal totalUp = GetOverallTotalAmountUpToDate();
        decimal totalPrev = GetOverallTotalAmountSincePrevious();

        // 1. Total Value of work done
        decimal val1 = totalUp * (100 + sorPct) / 100;
        lblMemoTotalValue.Text = val1.ToString("N2");

        // 2a. Previous Running Bill (editable, but set default)
        decimal val2a = (totalUp - totalPrev) * (100 + sorPct) / 100;
        txtPreviousBillAmount.Text = val2a.ToString("N2");

        // 2b. This Bill (editable, default = 0)
        txtThisBillAmount.Text = "0.00";
        decimal val2b = 0;

        // 2. Total
        decimal total2 = val2a + val2b;
        lblTotal2.Text = total2.ToString("N2");

        // 3. Balance
        decimal val3 = totalPrev * (100 + sorPct) / 100;
        lblBalance.Text = val3.ToString("N2");

        // 4. Total work for fixed code
        lblTotalWorkDone.Text = val1.ToString("N2");

        // 5a and 5b default = 0
        txtRecoveryAmount.Text = "0.00";
        txtSecurityDeposit.Text = "0.00";
        txtIncomeTax.Text = "0.00";
        txtGSTAmount.Text = "0.00";
        txtLabourWelfare.Text = "0.00";
        txtTimeExtensionWithheld.Text = "0.00";
        txtRoyalty.Text = "0.00";
        txtMiscellaneousDeposit.Text = "0.00";
        txtPenaltyForTimeExtension.Text = "0.00";
        txtPenaltyForWork.Text = "0.00";
        txtCostOfBillForm.Text = "0.00";

        lblByTransfer.Text = val3.ToString("N2");

        decimal total5 = 0 + val3;
        lblTotal5.Text = total5.ToString("N2");
        txtPayAmount.Text = val3.ToString("N2");
        txtReceivedAmount.Text = total5.ToString("N2");
    }
    protected void gvHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            BillApprovalChecker checker = new BillApprovalChecker();

            // Get bill ID from the row
            int billId = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "BillId"));

            // Get user type from session
            string userType = Session["userType"].ToString();

            // Get the action to display
            string action = checker.GetBillDisplayAction(billId, userType);

            // Find the button in the row
            Button btnLockForward = (Button)e.Row.FindControl("btnLockForward");

            switch (action)
            {
                case "LockForward":
                    btnLockForward.Visible = true;
                    btnLockForward.Text = "Lock & Forward";
                    btnLockForward.CssClass = "btn-lock-forward";
                    btnLockForward.Enabled = true;
                    break;

                case "Draft":
                    btnLockForward.Visible = true;
                    btnLockForward.Text = "Draft";
                    btnLockForward.CssClass = "status-label status-draft";
                    btnLockForward.Enabled = false;
                    break;

                case "Approved":
                    btnLockForward.Visible = true;
                    btnLockForward.Text = "Approved";
                    btnLockForward.CssClass = "status-label status-approved";
                    btnLockForward.Enabled = false;
                    break;

                case "Rejected":
                    btnLockForward.Visible = true;
                    btnLockForward.Text = "Rejected";
                    btnLockForward.CssClass = "status-label status-rejected";
                    btnLockForward.Enabled = false;
                    break;

                default:
                    btnLockForward.Visible = false;
                    break;
            }
        }
    }
    protected void btnLockNForward(string embbookid, string workCode, int billid)
    {
        string userId = Session["UserId"] != null ? Session["UserId"].ToString() : null;
        string userType = Session["userType"] != null ? Session["userType"].ToString() : null;
        string connectionString = ConfigurationManager.ConnectionStrings["eMB"].ConnectionString;
        if (userId == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Session Expired!');", true);
            Response.Redirect("../Login.aspx");
            return;
        }

        if (userType == null || (userType != "se" && userType != "ae" && userType != "ee"))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Invalid user type!');", true);
            return;
        }

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            // Start transaction to ensure both operations succeed or fail together
            using (SqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    // STEP 1: Update eMB_Entry table based on user type
                    string embEntryUpdateQuery = "";

                    if (userType == "se")
                    {
                        embEntryUpdateQuery = @"
                    UPDATE [JJM].[dbo].[eMB_Entry] 
                    SET [SE] = @userId
                    WHERE [embbookid] = @embbookid AND [WorkCode] = @workcode";
                    }
                    else if (userType == "ae")
                    {
                        embEntryUpdateQuery = @"
                    UPDATE [JJM].[dbo].[eMB_Entry] 
                    SET [AE] = @userId
                    WHERE [embbookid] = @embbookid AND [WorkCode] = @workcode";
                    }
                    else if (userType == "ee")
                    {
                        embEntryUpdateQuery = @"
                    UPDATE [JJM].[dbo].[eMB_Entry] 
                    SET [EE] = @userId
                    WHERE [embbookid] = @embbookid AND [WorkCode] = @workcode";
                    }

                    using (SqlCommand embEntryUpdateCmd = new SqlCommand(embEntryUpdateQuery, conn, transaction))
                    {
                        embEntryUpdateCmd.Parameters.AddWithValue("@userId", userId);
                        embEntryUpdateCmd.Parameters.AddWithValue("@embbookid", embbookid);
                        embEntryUpdateCmd.Parameters.AddWithValue("@workcode", workCode);

                        int embEntryRowsAffected = embEntryUpdateCmd.ExecuteNonQuery();

                        // Optional: Check if any records were updated
                        if (embEntryRowsAffected == 0)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No matching records found in eMB_Entry table!');", true);
                            transaction.Rollback();
                            return;
                        }
                    }

                    // STEP 2: Continue with existing lock logic
                    if (userType == "se")
                    {
                        // SE creates a new lock record
                        // First get the data from eMBBookNoIssued table
                        string selectQuery = @"
                    SELECT [eMBBookNo] as embbookid,
                           [WorkCode] as WorkCode,
                           [SubEnginnerId] as se,
                           [SubDivisionId] as ae,
                           [DivisionId] as ee
                    FROM [JJM].[dbo].[eMBBookNoIssued]
                    WHERE [eMBBookNo] = @embbookid AND [WorkCode] = @workcode";

                        string se = null, ae = null, ee = null;

                        using (SqlCommand selectCmd = new SqlCommand(selectQuery, conn, transaction))
                        {
                            selectCmd.Parameters.AddWithValue("@workcode", workCode);
                            selectCmd.Parameters.AddWithValue("@embbookid", embbookid);

                            using (SqlDataReader reader = selectCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    se = reader["se"].ToString();
                                    ae = reader["ae"].ToString();
                                    ee = reader["ee"].ToString();
                                }
                                else
                                {
                                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Record not found in eMBBookNoIssued!');", true);
                                    transaction.Rollback();
                                    return;
                                }
                            }
                        }

                        // Insert new lock record
                        string insertQuery = @"
                    INSERT INTO [JJM].[dbo].[eMB_Locks] 
                    ([embbookid], [WorkCode], [se], [ae], [ee], [DateSE],[BillSE])
                    VALUES (@embbookid, @workcode, @se, @ae, @ee, @currentDate,@billid)";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn, transaction))
                        {
                            insertCmd.Parameters.AddWithValue("@embbookid", embbookid);
                            insertCmd.Parameters.AddWithValue("@workcode", workCode);
                            insertCmd.Parameters.AddWithValue("@se", se);
                            insertCmd.Parameters.AddWithValue("@ae", ae);
                            insertCmd.Parameters.AddWithValue("@ee", ee);
                            insertCmd.Parameters.AddWithValue("@currentDate", DateTime.Now);
                            insertCmd.Parameters.AddWithValue("@billid", billid);

                            int rowsAffected = insertCmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Book locked successfully by SE!'); window.location.href='frmHome.aspx';", true);
                            }
                            else
                            {
                                transaction.Rollback();
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Failed to lock book!');", true);
                            }
                        }
                    }
                    else if (userType == "ae")
                    {
                        // AE updates the current active record (DateAE)
                        string updateQuery = @"
                    UPDATE [JJM].[dbo].[eMB_Locks] 
                    SET [DateAE] = @currentDate, [BillAE]=@billId
                    WHERE [embbookid] = @embbookid AND [WorkCode] = @workcode
                    AND [DateSE] IS NOT NULL AND [DateAE] IS NULL AND [DateEE] IS NULL";

                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@embbookid", embbookid);
                            updateCmd.Parameters.AddWithValue("@workcode", workCode);
                            updateCmd.Parameters.AddWithValue("@currentDate", DateTime.Now);
                            updateCmd.Parameters.AddWithValue("@billId", billid);

                            int rowsAffected = updateCmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Book approved successfully by AE!'); window.location.href='frmHome.aspx';", true);
                            }
                            else
                            {
                                transaction.Rollback();
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No active record found to update!');", true);
                            }
                        }
                    }
                    else if (userType == "ee")
                    {
                        // EE updates the current active record (DateEE) - this completes the cycle
                        string updateQuery = @"
                    UPDATE [JJM].[dbo].[eMB_Locks] 
                    SET [DateEE] = @currentDate,[BillEE]=@billId
                    WHERE [embbookid] = @embbookid AND [WorkCode] = @workcode
                    AND [DateSE] IS NOT NULL AND [DateAE] IS NOT NULL AND [DateEE] IS NULL";

                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn, transaction))
                        {
                            updateCmd.Parameters.AddWithValue("@embbookid", embbookid);
                            updateCmd.Parameters.AddWithValue("@workcode", workCode);
                            updateCmd.Parameters.AddWithValue("@currentDate", DateTime.Now);
                            updateCmd.Parameters.AddWithValue("@billId", billid);

                            int rowsAffected = updateCmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Book approved successfully by EE! Lock cycle completed.'); window.location.href='frmHome.aspx';", true);
                            }
                            else
                            {
                                transaction.Rollback();
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No active record found to update!');", true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Error occurred: "+ex.Message+"');", true);
                }
            }
        }
    }
    protected void gvHistory_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ViewBill")
        {
            int billId = Convert.ToInt32(e.CommandArgument);
            string url = string.Format("ViewBill.aspx?id={0}", billId);
            ScriptManager.RegisterStartupScript(this, GetType(), "OpenWindow",
                "window.open('" + url + "','_blank','width=1200,height=900');", true);
        }
        else if (e.CommandName == "LockForward")
        {
            int billId = Convert.ToInt32(e.CommandArgument);
            string embbookid = Request["embbookid"];
            string workcode = Request["workcode"];
            // LockAndForwardBill(billId);
            btnLockNForward(embbookid, workcode, billId);
            // Refresh history
            string workCode = txtWorkCode.Text.Trim();
            DataTable history = GetBillHistory(workCode);
            gvHistory.DataSource = history;
            gvHistory.DataBind();
        }
    }

    // Show modal popup
    protected void btnNewBill_Click(object sender, EventArgs e)
    {
        string workCode = txtWorkCode.Text.Trim();
        if (String.IsNullOrEmpty(workCode))
        {
            ShowAlert("Please enter a Work Code.");
            return;
        }
        SetSubdivisionAndDivision(workCode);
        ViewState.Remove("BillData");
        rptComponents.DataSource = null;
        rptComponents.DataBind();

        modalBg.Style["display"] = "block";
        pnlBillPopup.Style["display"] = "block";
        mvBill.Visible = true;
        mvBill.ActiveViewIndex = 0;

        ViewState["ModalOpen"] = true;
    }
    
   protected void btnEditeMB(object sender, EventArgs e)
    {
        Response.Redirect("AgreementList.aspx");
        return;
    }

    private void SetSubdivisionAndDivision(string workCode)
    {
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["eMB"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = @"
            SELECT 
                s.subdivision_name AS SubdivisionName,
                d.division_name AS DivisionName,
                w.name_of_work_Eng AS NameOfWorkEng
            FROM eMBBookNoIssued b
            LEFT JOIN Subdivision_master s 
                ON b.SubDivisionId = s.subdivision_code
            LEFT JOIN division_master d 
                ON b.DivisionId = d.division_code
            LEFT JOIN Work_Master w 
                ON b.WorkCode = w.PKWorkCode
            WHERE b.WorkCode = @WorkCode";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@WorkCode", SqlDbType.VarChar).Value = workCode;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtSubDivision.Text = reader["SubdivisionName"] as string ?? string.Empty;
                        txtSubDivision2.Text = reader["SubdivisionName"] as string ?? string.Empty;
                        txtDivision.Text = reader["DivisionName"] as string ?? string.Empty;
                        txtDivision2.Text = reader["DivisionName"] as string ?? string.Empty;
                        txtWorkName.Text = reader["NameOfWorkEng"] as string ?? string.Empty;
                    }
                }
            }
        }
    }

    // Navigate intro -> details
    protected void GoToDetails(object sender, EventArgs e)
    {
        string workCode = txtWorkCode.Text.Trim();
        if (String.IsNullOrEmpty(workCode))
        {
            ShowAlert("Please enter a Work Code.");
            return;
        }

        LoadBillForWorkCode(workCode);
        mvBill.ActiveViewIndex = 1;
        ViewState["ModalOpen"] = true; // Maintain modal state
    }

    // Navigate details -> memo
    protected void GoToMemo(object sender, EventArgs e)
    {
        lblWorkCode.Text = txtWorkCode.Text.Trim();
        mvBill.ActiveViewIndex = 2;
        ViewState["ModalOpen"] = true; // Maintain modal state
    }

    // Navigate memo -> details
    protected void GoToIntro(object sender, EventArgs e)
    {
        mvBill.ActiveViewIndex = 0;
        ViewState["ModalOpen"] = true; // Maintain modal state
    }

    public string GetEMBBookNumber(string workCode)
    {
        string embBookNo = string.Empty;
        string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["eMB"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connStr))
        using (SqlCommand cmd = new SqlCommand("SELECT eMBBookNo FROM eMBBookNoIssued WHERE WorkCode = @WorkCode", conn))
        {
            cmd.Parameters.AddWithValue("@WorkCode", workCode);
            conn.Open();
            var result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                embBookNo = result.ToString();
            }
        }
        return embBookNo;
    }

    public string GetSubEngineerUserName(string workCode)
    {
        string userName = string.Empty;
        string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["eMB"].ConnectionString;

        using (SqlConnection conn = new SqlConnection(connStr))
        using (SqlCommand cmd = new SqlCommand(@"
        SELECT lm.UserName
        FROM eMBBookNoIssued eb
        INNER JOIN LoginMaster lm ON eb.SubEnginnerId = lm.UserId
        WHERE eb.WorkCode = @WorkCode", conn))
        {
            cmd.Parameters.AddWithValue("@WorkCode", workCode);
            conn.Open();

            var result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                userName = result.ToString();
            }
        }
        return userName;
    }

    protected void FinishAndSave(object sender, EventArgs e)
    {
        string workCode = txtWorkCode.Text.Trim();
        string workName = txtWorkName.Text.Trim();

        // Collect all form data
        string voucherNo = txtVoucherNo.Text;
        string voucherDate = txtVoucherDate.Text;
        string SubDivision = txtSubDivision.Text;
        string Division = txtDivision.Text;
        string supplier = txtSupplier.Text;
        string purposeOfSupply = txtPurposeOfSupply.Text;
        string serialNo = txtSerialNo.Text;
        string lastWorkDate = txtLastWorkDate.Text;
        string agreementNo = txtAgreementNo.Text;
        decimal contractAmount = SafeConvert.ToDecimal(txtContractAmount.Text);

        // Table data
        string BillNumber = lblBillNumber.Text;
        string BillDate = lblBillDate.Text;
        string SubDivision2 = txtSubDivision2.Text;
        string Division2 = txtDivision2.Text;

        // Memo fields
        decimal MemoTotalValue = SafeConvert.ToDecimal(lblMemoTotalValue.Text);
        decimal PreviousBillAmount = SafeConvert.ToDecimal(txtPreviousBillAmount.Text);
        decimal ThisBillAmount = SafeConvert.ToDecimal(txtThisBillAmount.Text);
        decimal Total2 = SafeConvert.ToDecimal(lblTotal2.Text);
        decimal Balance3 = SafeConvert.ToDecimal(lblBalance.Text);
        string WorkCode4 = lblWorkCode.Text;
        decimal TotalWorkDone = SafeConvert.ToDecimal(lblTotalWorkDone.Text);
        decimal recoveryAmount = SafeConvert.ToDecimal(txtRecoveryAmount.Text);
        decimal securityDeposit = SafeConvert.ToDecimal(txtSecurityDeposit.Text);
        decimal incomeTax = SafeConvert.ToDecimal(txtIncomeTax.Text);
        decimal gstAmount = SafeConvert.ToDecimal(txtGSTAmount.Text);
        decimal labourWelfare = SafeConvert.ToDecimal(txtLabourWelfare.Text);
        decimal timeExtensionWithheld = SafeConvert.ToDecimal(txtTimeExtensionWithheld.Text);
        decimal royalty = SafeConvert.ToDecimal(txtRoyalty.Text);
        decimal miscellaneousDeposit = SafeConvert.ToDecimal(txtMiscellaneousDeposit.Text);
        decimal penaltyForTimeExtension = SafeConvert.ToDecimal(txtPenaltyForTimeExtension.Text);
        decimal penaltyForWork = SafeConvert.ToDecimal(txtPenaltyForWork.Text);
        decimal costOfBillForm = SafeConvert.ToDecimal(txtCostOfBillForm.Text);
        decimal ByTransfer = SafeConvert.ToDecimal(lblByTransfer.Text);
        decimal Total5 = SafeConvert.ToDecimal(lblTotal5.Text);
        decimal PayAmount = SafeConvert.ToDecimal(txtPayAmount.Text);
        decimal ReceivedAmount = SafeConvert.ToDecimal(txtReceivedAmount.Text);
        string ReceiptDate = txtReceiptDate.Text;
        string paidDate = txtPaidDate.Text;
        string cashier = txtCashier.Text;
        string chequeNo = txtChequeNo.Text;

        // Save to database
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["eMB"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            using (SqlTransaction tran = conn.BeginTransaction())
            {
                try
                {
                    // Insert into BillMaster
                    string masterSql = @"
                INSERT INTO BillMaster (
                    WorkCode, WorkName, BillDate, BillNumber,
                    VoucherNo, VoucherDate, SubDivision, Division, Supplier, PurposeOfSupply,
                    SerialNo, LastWorkDate, AgreementNo, ContractAmount, SubDivision2, Division2,
                    MemoTotalValue, PreviousBillAmount, ThisBillAmount, Total2, Balance3, WorkCode4, TotalWorkDone,
                    RecoveryAmount, SecurityDeposit, IncomeTax, GSTAmount,
                    LabourWelfare, TimeExtensionWithheld, Royalty, MiscellaneousDeposit,
                    PenaltyForTimeExtension, PenaltyForWork, CostOfBillForm,
                    ByTransfer, Total5, PayAmount, ReceivedAmount, ReceiptDate,
                    PaidDate, Cashier, ChequeNo
                ) 
                OUTPUT INSERTED.BillId 
                VALUES (
                    @WorkCode, @WorkName, @BillDate, @BillNumber,
                    @VoucherNo, @VoucherDate, @SubDivision, @Division, @Supplier, @PurposeOfSupply,
                    @SerialNo, @LastWorkDate, @AgreementNo, @ContractAmount, @SubDivision2, @Division2,
                    @MemoTotalValue, @PreviousBillAmount, @ThisBillAmount, @Total2, @Balance3, @WorkCode4, @TotalWorkDone,
                    @RecoveryAmount, @SecurityDeposit, @IncomeTax, @GSTAmount,
                    @LabourWelfare, @TimeExtensionWithheld, @Royalty, @MiscellaneousDeposit,
                    @PenaltyForTimeExtension, @PenaltyForWork, @CostOfBillForm,
                    @ByTransfer, @Total5, @PayAmount, @ReceivedAmount, @ReceiptDate,
                    @PaidDate, @Cashier, @ChequeNo
                )";

                    using (SqlCommand cmd = new SqlCommand(masterSql, conn, tran))
                    {
                        // Add parameters
                        cmd.Parameters.AddWithValue("@WorkCode", workCode);
                        cmd.Parameters.AddWithValue("@WorkName", workName);
                        cmd.Parameters.AddWithValue("@BillDate", BillDate);
                        cmd.Parameters.AddWithValue("@BillNumber", BillNumber);

                        // Intro fields
                        cmd.Parameters.AddWithValue("@VoucherNo", voucherNo);
                        cmd.Parameters.AddWithValue("@VoucherDate", voucherDate);
                        cmd.Parameters.AddWithValue("@SubDivision", SubDivision);
                        cmd.Parameters.AddWithValue("@Division", Division);
                        cmd.Parameters.AddWithValue("@Supplier", supplier);
                        cmd.Parameters.AddWithValue("@PurposeOfSupply", purposeOfSupply);

                        cmd.Parameters.AddWithValue("@SerialNo", serialNo);
                        cmd.Parameters.AddWithValue("@LastWorkDate", lastWorkDate);
                        cmd.Parameters.AddWithValue("@AgreementNo", agreementNo);
                        cmd.Parameters.AddWithValue("@ContractAmount", contractAmount);
                        cmd.Parameters.AddWithValue("@SubDivision2", SubDivision2);
                        cmd.Parameters.AddWithValue("@Division2", Division2);

                        // Memo fields
                        cmd.Parameters.AddWithValue("@MemoTotalValue", MemoTotalValue);
                        cmd.Parameters.AddWithValue("@PreviousBillAmount", PreviousBillAmount);
                        cmd.Parameters.AddWithValue("@ThisBillAmount", ThisBillAmount);
                        cmd.Parameters.AddWithValue("@Total2", Total2);
                        cmd.Parameters.AddWithValue("@Balance3", Balance3);
                        cmd.Parameters.AddWithValue("@WorkCode4", WorkCode4);
                        cmd.Parameters.AddWithValue("@TotalWorkDone", TotalWorkDone);

                        cmd.Parameters.AddWithValue("@RecoveryAmount", recoveryAmount);
                        cmd.Parameters.AddWithValue("@SecurityDeposit", securityDeposit);
                        cmd.Parameters.AddWithValue("@IncomeTax", incomeTax);
                        cmd.Parameters.AddWithValue("@GSTAmount", gstAmount);

                        cmd.Parameters.AddWithValue("@LabourWelfare", labourWelfare);
                        cmd.Parameters.AddWithValue("@TimeExtensionWithheld", timeExtensionWithheld);
                        cmd.Parameters.AddWithValue("@Royalty", royalty);
                        cmd.Parameters.AddWithValue("@MiscellaneousDeposit", miscellaneousDeposit);

                        cmd.Parameters.AddWithValue("@PenaltyForTimeExtension", penaltyForTimeExtension);
                        cmd.Parameters.AddWithValue("@PenaltyForWork", penaltyForWork);
                        cmd.Parameters.AddWithValue("@CostOfBillForm", costOfBillForm);

                        cmd.Parameters.AddWithValue("@ByTransfer", ByTransfer);
                        cmd.Parameters.AddWithValue("@Total5", Total5);
                        cmd.Parameters.AddWithValue("@PayAmount", PayAmount);
                        cmd.Parameters.AddWithValue("@ReceivedAmount", ReceivedAmount);
                        cmd.Parameters.AddWithValue("@ReceiptDate", ReceiptDate);

                        cmd.Parameters.AddWithValue("@PaidDate", paidDate);
                        cmd.Parameters.AddWithValue("@Cashier", cashier);
                        cmd.Parameters.AddWithValue("@ChequeNo", chequeNo);

                        int billId = Convert.ToInt32(cmd.ExecuteScalar());

                        // Save BillDetails
                        DataTable billData = ViewState["BillData"] as DataTable;
                        if (billData != null)
                        {
                            foreach (DataRow row in billData.Rows)
                            {
                                string detailSql = @"
                            INSERT INTO BillDetails (
                                BillId, ComponentID, SORItemNo,
                                ActualUnit, CumulativeQuantity, SORFrom,
                                Description, QuantitySincePrevious, Rate,
                                GST, AmountUpToDate, AmountSincePrevious
                            ) VALUES (
                                @BillId, @ComponentID, @SORItemNo,
                                @ActualUnit, @CumulativeQuantity, @SORFrom,
                                @Description, @QuantitySincePrevious, @Rate,
                                @GST, @AmountUpToDate, @AmountSincePrevious
                            )";

                                using (SqlCommand detCmd = new SqlCommand(detailSql, conn, tran))
                                {
                                    detCmd.Parameters.AddWithValue("@BillId", billId);
                                    detCmd.Parameters.AddWithValue("@ComponentID", row["ComponentID"]);
                                    detCmd.Parameters.AddWithValue("@SORItemNo", row["SORItemNo"]);
                                    detCmd.Parameters.AddWithValue("@ActualUnit", row["ActualUnit"]);
                                    detCmd.Parameters.AddWithValue("@CumulativeQuantity", row["CumulativeQuantity"]);
                                    detCmd.Parameters.AddWithValue("@SORFrom", row["SORFrom"]);
                                    detCmd.Parameters.AddWithValue("@Description", row["Description"]);
                                    detCmd.Parameters.AddWithValue("@QuantitySincePrevious", row["QuantitySincePrevious"]);
                                    detCmd.Parameters.AddWithValue("@Rate", row["Rate"]);
                                    detCmd.Parameters.AddWithValue("@GST", row["GST"]);
                                    detCmd.Parameters.AddWithValue("@AmountUpToDate", row["AmountUpToDate"]);
                                    detCmd.Parameters.AddWithValue("@AmountSincePrevious", row["AmountSincePrevious"]);

                                    detCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    tran.Commit();
                    ShowAlert("Bill saved successfully!");

                    // Close modal and refresh
                    ViewState["ModalOpen"] = false;
                    DataTable history = GetBillHistory(workCode);
                    gvHistory.DataSource = history;
                    gvHistory.DataBind();
                    pnlSearch.Visible = false;
                    pnlHistory.Visible = true;
                    mvBill.Visible = false;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    LogError(ex);
                    ShowAlert("Error saving bill: " + CleanJavaScriptString(ex.Message));
                }
            }
        }
    }

    // Helper: load and bind bill data to repeater and labels
    private void LoadBillForWorkCode(string workCode)
    {
        DataTable billData = GetBillData(workCode);
        if (billData == null || billData.Rows.Count == 0)
        {
            ShowAlert("No bill data found for this Work Code.");
            return;
        }
        ViewState["BillData"] = billData;

        // Bind components
        var comps = new List<object>();
        foreach (DataRow row in billData.Rows)
        {
            int cid = Convert.ToInt32(row["ComponentID"]);
            string cname = row["ComponentName"] != DBNull.Value ? row["ComponentName"].ToString() : String.Empty;
            var anon = new { ComponentID = cid, ComponentName = cname };
            bool exists = false;
            foreach (var c in comps)
            {
                if (c.Equals(anon)) { exists = true; break; }
            }
            if (!exists) comps.Add(anon);
        }
        rptComponents.DataSource = comps;
        rptComponents.DataBind();

        // Bind labels
        decimal total = 0m;
        foreach (DataRow row in billData.Rows)
            total += SafeConvert.ToDecimal(row["AmountSincePrevious"]);
        lblBillNumber.Text = String.Format("BILL-{0}-{1}", txtWorkCode.Text.Trim(), DateTime.Now.ToString("yyyyMMddHHmmss"));
        lblBillDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

        // Load bill history with proper sorting
        LoadBillHistory(workCode);
    }
    private void LoadBillHistory(string workCode)
    {
        DataTable billHistory = GetBillHistory(workCode);
        gvHistory.DataSource = billHistory;
        gvHistory.DataBind();
    }
    private DataTable GetBillData(string workCode)
    {
        DataTable result = new DataTable();
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["eMB"].ConnectionString;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    WITH CurrentQuantities AS (
                        SELECT 
                            e.ComponentID,
                            c.ComponentName,
                            e.SORItemNo,
                            m.SORSubItem AS Description,
                            m.UnitCost as Rate,
                            m.AmountWithGST,
                            m.ActualUnit,
                            m.SORFrom,
                            m.GST,
                            m.Qty,
                            SUM(ISNULL(e.ResultValue, 0)) AS CumulativeQuantity
                        FROM eMB_Entry e
                        INNER JOIN eMB_ComponentMaterialsEntry m 
                            ON e.WorkCode = m.Work_Code 
                            AND e.ComponentID = m.ComponentID 
                            AND e.SORItemNo = m.SORItemNo
                        INNER JOIN eMB_ComponentMaster c ON e.ComponentID = c.ComponentID
                        WHERE e.WorkCode = @WorkCode
                            AND e.Deleted = 0
                            AND e.IsCurrent = 1
                            AND e.IsBacklog = 0
                        GROUP BY e.ComponentID, c.ComponentName, e.SORItemNo, m.SORSubItem, m.UnitCost, m.ActualUnit, m.SORFrom, m.GST, m.AmountWithGST, m.Qty
                    ),
                    PreviousBill AS (
                        SELECT TOP 1 BillId 
                        FROM BillMaster 
                        WHERE WorkCode = @WorkCode 
                        ORDER BY EntryDate DESC
                    ),
                    PreviousQuantities AS (
                        SELECT 
                            d.ComponentID,
                            d.SORItemNo,
                            d.CumulativeQuantity
                        FROM BillDetails d
                        WHERE d.BillId IN (SELECT BillId FROM PreviousBill)
                    )
                SELECT 
                    cq.ComponentID,
                    cq.ComponentName,
                    cq.SORItemNo,
                    cq.Description,
                    cq.ActualUnit,
                    cq.SORFrom,
                    cq.GST,
                    ISNULL(cq.Rate, 0) AS Rate,
                    cq.CumulativeQuantity,
                    ISNULL(pq.CumulativeQuantity, 0) AS PreviousQuantity,
                    cq.CumulativeQuantity - ISNULL(pq.CumulativeQuantity, 0) AS QuantitySincePrevious,
                    cq.AmountWithGST * cq.CumulativeQuantity / cq.Qty AS AmountUpToDate,
                    cq.AmountWithGST * (cq.CumulativeQuantity - ISNULL(pq.CumulativeQuantity, 0)) / cq.Qty AS AmountSincePrevious
                FROM CurrentQuantities cq
                LEFT JOIN PreviousQuantities pq 
                    ON cq.ComponentID = pq.ComponentID 
                    AND cq.SORItemNo = pq.SORItemNo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@WorkCode", SqlDbType.VarChar).Value = workCode;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(result);
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
            ShowAlert("Error retrieving bill data: " + CleanJavaScriptString(ex.Message));
        }

        return result;
    }

    protected decimal SumAmountUpToDate(object componentIdObj)
    {
        int componentId = Convert.ToInt32(componentIdObj);
        var billData = ViewState["BillData"] as DataTable;
        if (billData == null) return 0m;

        var total = billData.Rows.Cast<DataRow>()
                        .Where(r => Convert.ToInt32(r["ComponentID"]) == componentId)
                        .Sum(r => Convert.ToDecimal(r["AmountUpToDate"]));
        return total;
    }

    protected decimal GetOverallTotalAmountUpToDate()
    {
        var billData = ViewState["BillData"] as DataTable;
        if (billData == null) return 0m;
        return billData.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["AmountUpToDate"]));
    }
    protected decimal GetOverallTotalAmountSincePrevious()
    {
        var billData = ViewState["BillData"] as DataTable;
        if (billData == null) return 0m;
        return billData.Rows.Cast<DataRow>().Sum(r => Convert.ToDecimal(r["AmountSincePrevious"]));
    }

    protected decimal SumAmountSincePrevious(object componentIdObj)
    {
        int componentId = Convert.ToInt32(componentIdObj);
        var billData = ViewState["BillData"] as DataTable;
        if (billData == null) return 0m;

        var total = billData.Rows.Cast<DataRow>()
                        .Where(r => Convert.ToInt32(r["ComponentID"]) == componentId)
                        .Sum(r => Convert.ToDecimal(r["AmountSincePrevious"]));
        return total;
    }

    // Get history
    private DataTable GetBillHistory(string workCode)
    {
        DataTable result = new DataTable();
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["eMB"].ConnectionString;

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Use the corrected query with proper sorting
                string query = @"
                SELECT 
                    BillId,
                    BillNumber,
                    EntryDate as BillDate
                FROM BillMaster 
                WHERE WorkCode = @WorkCode
                ORDER BY 
                    CASE 
                        WHEN CHARINDEX('-', REVERSE(BillNumber)) > 0 
                        THEN CAST(RIGHT(BillNumber, CHARINDEX('-', REVERSE(BillNumber)) - 1) AS BIGINT)
                        ELSE 0 
                    END DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@WorkCode", SqlDbType.VarChar).Value = workCode;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(result);
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
            ShowAlert("Error retrieving bill history: " + CleanJavaScriptString(ex.Message));
        }

        return result;
    }
    private string getEmbBookId(string workcode)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["eMB"].ConnectionString;
        string query = @"SELECT [eMBBookNo] FROM [JJM].[dbo].[eMBBookNoIssued] WHERE [WorkCode] = @workcode";

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@workcode", workcode);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                        return result.ToString();
                    else
                        return string.Empty;
                }
            }
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    // Show JS alert
    private void ShowAlert(string message)
    {
        string safe = HttpUtility.JavaScriptStringEncode(message);
        string script = "alert('" + safe + "');";
        ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
    }

    // SafeConvert
    public static class SafeConvert
    {
        public static decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value) return 0m;
            decimal result;
            if (decimal.TryParse(value.ToString(), out result)) return result;
            return 0m;
        }
    }

    private string CleanJavaScriptString(string s)
    {
        return HttpUtility.JavaScriptStringEncode(s);
    }

    private void LogError(Exception ex)
    {
        string stack = ex.StackTrace != null ? ex.StackTrace : "No stack trace";
        System.Diagnostics.Debug.WriteLine("ERROR: " + ex.Message + " " + stack);
    }

    protected void rptComponents_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            dynamic component = e.Item.DataItem;
            int componentId = Convert.ToInt32(component.ComponentID);

            // Find the inner repeater instead of GridView
            Repeater rptSORItems = (Repeater)e.Item.FindControl("rptSORItems");

            DataTable billData = ViewState["BillData"] as DataTable;
            if (billData != null && rptSORItems != null)
            {
                DataView dv = new DataView(billData);
                dv.RowFilter = "ComponentID = " + componentId;
                rptSORItems.DataSource = dv;
                rptSORItems.DataBind();
            }
        }
    }

    protected string NumberToIndianWords(long number)
    {
        if (number == 0)
            return "Zero";
        string[] unitsMap = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
                          "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
                          "Seventeen", "Eighteen", "Nineteen" };
        string[] tensMap = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
        Func<int, string> twoDigit = n =>
        {
            if (n < 20) return unitsMap[n];
            int t = n / 10, u = n % 10;
            return tensMap[t] + (u > 0 ? " " + unitsMap[u] : "");
        };
        var parts = new List<string>();
        long crore = number / 10000000;
        if (crore > 0)
        {
            parts.Add(twoDigit((int)crore) + " Crore");
            number %= 10000000;
        }
        long lakh = number / 100000;
        if (lakh > 0)
        {
            parts.Add(twoDigit((int)lakh) + " Lakh");
            number %= 100000;
        }
        long thousand = number / 1000;
        if (thousand > 0)
        {
            parts.Add(twoDigit((int)thousand) + " Thousand");
            number %= 1000;
        }
        long hundred = number / 100;
        if (hundred > 0)
        {
            parts.Add(unitsMap[hundred] + " Hundred");
            number %= 100;
        }
        if (number > 0)
        {
            if (parts.Count > 0)
                parts.Add("and " + twoDigit((int)number));
            else
                parts.Add(twoDigit((int)number));
        }
        return string.Join(" ", parts);
    }

    protected decimal GetSORPercentage()
    {
        return 10;
    }
    protected decimal GetSayRs()
    {
        return GetOverallTotalAmountSincePrevious() * (100 + GetSORPercentage()) / 100;
    }
    protected string NumberToIndianCurrencyWords(decimal amount)
    {
        amount = amount * (100 + GetSORPercentage()) / 100;
        long rupees = (long)Math.Floor(amount);
        int paise = (int)Math.Round((amount - rupees) * 100);

        var text = new StringBuilder();
        text.Append(NumberToIndianWords(rupees) + " Rupees");
        if (paise > 0)
            text.Append(" and " + NumberToIndianWords(paise) + " Paise");
        return text.ToString();
    }
}
