using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.Configuration;
using System.Text;
using System.Collections.Generic;

public partial class ViewBill : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack && Request.QueryString["id"] != null)
        {
            int billId = Convert.ToInt32(Request.QueryString["id"]);
            LoadBillData(billId);
        }
    }

    private void LoadBillData(int billId)
    {
        string connStr = WebConfigurationManager.ConnectionStrings["eMB"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            // Get master data
            string masterSql = @"SELECT * FROM BillMaster WHERE BillId = @BillId";
            SqlCommand cmd = new SqlCommand(masterSql, conn);
            cmd.Parameters.AddWithValue("@BillId", billId);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                // Render Intro Section
                divIntro.InnerHtml = RenderIntroSection(reader);

                // Get details
                DataTable details = GetBillDetails(billId);
                divDetails.InnerHtml = RenderDetailsSection(details, billId, reader);

                // Render Memo Section
                divMemo.InnerHtml = RenderMemoSection(reader);
                reader.Close();
            }
        }
    }
    private string SafeFormatDate(object dateValue)
    {
        DateTime date;
        if (dateValue != DBNull.Value && dateValue != null && DateTime.TryParse(dateValue.ToString(), out date))
        {
            return date.ToString("dd/MM/yyyy");
        }
        return "N/A";
    }
    private string RenderIntroSection(SqlDataReader reader)
    {
        return @"
    <div class='form-container'>
                        <!-- Form Header -->
                        <div class='form-header'>
                            <div class='form-header-left'>
                                XVII-E-47<br/>
                                P.W.D.
                            </div>
                            <div class='form-header-right'>
                                P.W. Account Form-E<br/>
                                (Full Sheet)
                            </div>
                        </div>

                        <!-- Form Body -->
                        <div class='form-body'>
                            <!-- Left Column - Notes (Read-only) -->
                            <div class='left-column'>
                                <div class='notes-title'>NOTES</div>
                                <div class='notes-subtitle'>Chapter K, paragraphs 284, 288 & 289 of the<br/>P.W.D Account Code</div>

                                <div class='note-item'>
                                    <span class='note-number'>1)</span>
                                    This form provides only for payment for work or supplier actually measured.
                                </div>

                                <div class='note-item'>
                                    <span class='note-number'>2)</span>
                                    The <span class='highlight-blue'>full name</span> of the work as given in the estimate should be entered on the bill except in the case of <span class='highlight-blue'>bills for stock materials</span>. That purpose of supply applicable to the should be filled in and the rest scored out.
                                </div>

                                <div class='note-item'>
                                    <span class='note-number'>3)</span>
                                    If the outlay on the work is recorded by sub heads the total for each sub head should be shown in column 5 of account 1 and against the total for each sub head should be shown in column 5 of account 1 and against the total there should be as entry in column 6.
                                </div>

                                <div class='note-item'>
                                    <span class='note-number'>4)</span>
                                    In part II the second signature is necessary only when the officer who prepares the bill is not the officer who authorises the payment in such a case two signatures are essential.
                                </div>

                                <div class='note-item'>
                                    <span class='note-number'>5)</span>
                                    The figures against item 8 in part III memorandum of payments should agree with the total of figures against lines 4 & 5 if the net amount to be paid is less than Rs 10 and it can not be included in a cheque the payment should be made in cash the pay order being altered suitably and the alteration attested by date initials. The figures in the pay order will be the net amount in line 5(C) and the payee's acknowledgement should be for the gross amount 5(a+b+c) the payment should be attested by some known person when the payee's acknowledgement is given by a seal mark of thumb impression.
                                </div>

                                <div class='note-item'>
                                    <span class='note-number'>6)</span>
                                    Prt. IV is reserved for any remarks which the disbursing officer of the Divisional Officer may wish to record in respect of the execution of the work check of measurements of the state of contractor's accounts.
                                </div>
                            </div>

                            <!-- Right Column - Form Fields -->
                            <div class='right-column'>
                                <div class='right-header'>
                                    <div class='division-fields'>
                                        <div class='form-field' style='margin-bottom:5px;'>
                                            <label style='width:auto; padding-right:5px;'>Sub-Division:</label>
                                            <span style='flex:1;'>" + reader["SubDivision"].ToString() + @"</span>
                                        </div>
                                        <div class='form-field' style='margin-bottom:5px;'>
                                            <label style='width:auto; padding-right:5px;'>Division:</label>
                                            <span style='flex:1;'>" + reader["Division"].ToString() + @"</span>
                                        </div>
                                    </div>

                                    <div class='running-account'>
                                        RUNNING ACCOUNT BILL
                                    </div>
                                    <div class='form-section'>
                                        <div class='form-field'>
                                            <label>Cash Book Voucher No.:</label>
                                            <span>" + reader["VoucherNo"].ToString() + @"</span>
                                        </div>
                                        <div class='form-field'>
                                            <label>Date:</label>
                                            <span>" + SafeFormatDate(reader["VoucherDate"]) + @"</span>
                                        </div>
                                    </div>
                                </div>

                                <div class='contractor-field'>
                                    <div class='form-field'>
                                        <label>Name of Contractors<br/>Or Supplier</label>
                                        <div class='workname-box'>" + reader["Supplier"].ToString() + @"</div>
                                    </div>
                                </div>

                                <div class='form-section'>
                                    <div class='form-field'>
                                        <label>Name of work:</label>
                                        <div class='workname-box'>" + reader["WorkName"].ToString() + @"</div>
                                    </div>

                                    <div class='form-field'>
                                        <label>Purpose of Supply:</label>
                                        <div class='workname-box'>" + reader["PurposeOfSupply"].ToString() + @"</div>
                                    </div>

                                    <div class='form-field'>
                                        <label>Serial No. of The Bill:</label>
                                        <div class='workname-box'>" + reader["SerialNo"].ToString() + @"</div>
                                    </div>

                                    <div class='form-field'>
                                        <label>No. and date his last for the Work:</label>
                                        <div class='workname-box'>" + reader["LastWorkDate"].ToString() + @"</div>
                                    </div>
                                </div>

                                <div class='reference-table'>
                                    <div class='reference-row'>
                                        <label>Reference to Agreement No. from Date:</label>
                                    </div>
                                    <div class='workname-box'>" + reader["AgreementNo"].ToString() + @"</div>
                                </div>

                                <div class='contract-amount'>
                                    <div class='form-field'>
                                        <label>Probable amount of contract (in Rs.)</label>
                                        <div class='workname-box'>" + reader["ContractAmount"].ToString() + @"</div>
                                    </div>
                                </div>

                                <div class='completion-section'>
                                    <div class='completion-item'>Due date of completion as per agreement</div>
                                    <div class='completion-item'>Actual date of completion of work</div>
                                    <div class='completion-item'>To be filled in form other agreement on</div>
                                    <div class='completion-item'>'E' Forms</div>
                                </div>
                            </div>
                        </div>
                    </div>";
    }

    private string RenderDetailsSection(DataTable details, int billId, SqlDataReader reader)
    {
        // Calculate overall totals
        decimal overallAmountUpToDate = 0;
        decimal overallAmountSincePrevious = 0;
        foreach (DataRow row in details.Rows)
        {
            overallAmountUpToDate += SafeConvert.ToDecimal(row["AmountUpToDate"]);
            overallAmountSincePrevious += SafeConvert.ToDecimal(row["AmountSincePrevious"]);
        }

        decimal sorPercentage = GetSORPercentage();
        decimal sorAmountUpToDate = overallAmountUpToDate * sorPercentage / 100;
        decimal sorAmountSincePrevious = overallAmountSincePrevious * sorPercentage / 100;
        decimal totalValue = overallAmountUpToDate * (100 + sorPercentage) / 100;
        decimal previousBillValue = (overallAmountUpToDate - overallAmountSincePrevious) * (100 + sorPercentage) / 100;
        decimal netValue = overallAmountSincePrevious * (100 + sorPercentage) / 100;

        StringBuilder sb = new StringBuilder();

        // Start of details section
        sb.Append("<div style='max-width: 1000px; margin: 0 auto; border: 2px solid #000; " +
                  "font-family: Arial, sans-serif; font-size: 12px; background: white;'>");

        // Header Section
        sb.Append("<div style='display: flex; justify-content: space-between; padding: 10px 15px; " +
                  "border-bottom: 2px solid #000; background: #f8f8f8;'>" +
                  "<div style='font-weight: bold;'>" +
                  "XVII-E-47<br/>P.W.D." +
                  "</div>" +
                  "<div style='text-align: right; font-weight: bold;'>" +
                  "RUNNING ACCOUNT BILL<br/>Account of Work Done or Supplies made" +
                  "</div>" +
                  "</div>");

        // Bill Info
        sb.Append("<div style='padding: 10px 15px; border-bottom: 1px solid #000; background: #f9f9f9;'>" +
                  "<div style='font-weight: bold;'>Bill Number: " + reader["BillNumber"] + "</div>" +
                  "<div style='font-weight: bold;'>Date: " + Convert.ToDateTime(reader["BillDate"]).ToString("dd/MM/yyyy") + "</div>" +
                  "</div>");

        // Main Table
        sb.Append("<div class='table-container'>" +
                  "<table style='width: 100%; border-collapse: collapse; font-size: 11px;'>" +
                  "<thead>" +
                  "<tr style='background: #e8e8e8; font-weight: bold;'>" +
                  "<th style='border: 1px solid #000; padding: 8px; text-align: center; width: 8%;'>Unit</th>" +
                  "<th style='border: 1px solid #000; padding: 8px; text-align: center; width: 12%;'>Quantity executed or supplied up to date as per eMB</th>" +
                  "<th style='border: 1px solid #000; padding: 8px; text-align: center; width: 8%;'>Category of Item (SOR/NON-SOR)</th>" +
                  "<th style='border: 1px solid #000; padding: 8px; text-align: center; width: 25%;'>Item of work or supplies (grouped under sub-heads and sub-works of estimate)</th>" +
                  "<th style='border: 1px solid #000; padding: 8px; text-align: center; width: 10%;'>Rate (without GST)</th>" +
                  "<th style='border: 1px solid #000; padding: 8px; text-align: center; width: 10%;'>GST Rate (%)</th>" +
                  "<th style='border: 1px solid #000; padding: 8px; text-align: center; width: 12%;'>Amount Up To Date <br/> (with GST)</th>" +
                  "<th style='border: 1px solid #000; padding: 8px; text-align: center; width: 12%;'>Amount Since previous Bill<br/>(For each sub-head)<br/>(with GST)</th>" +
                  "</tr>" +
                  "<tr style='background: #f0f0f0; font-weight: bold; font-size: 10px;'>" +
                  "<td style='border: 1px solid #000; padding: 4px; text-align: center;'>1</td>" +
                  "<td style='border: 1px solid #000; padding: 4px; text-align: center;'>2</td>" +
                  "<td style='border: 1px solid #000; padding: 4px; text-align: center;'>3</td>" +
                  "<td style='border: 1px solid #000; padding: 4px; text-align: center;'>4</td>" +
                  "<td style='border: 1px solid #000; padding: 4px; text-align: center;'>5</td>" +
                  "<td style='border: 1px solid #000; padding: 4px; text-align: center;'>6</td>" +
                  "<td style='border: 1px solid #000; padding: 4px; text-align: center;'>7</td>" +
                  "<td style='border: 1px solid #000; padding: 4px; text-align: center;'>8</td>" +
                  "</tr>" +
                  "</thead>" +
                  "<tbody>");

        // Track processed components
        Dictionary<int, bool> processedComponents = new Dictionary<int, bool>();

        // First pass: Process components
        foreach (DataRow row in details.Rows)
        {
            int componentId = Convert.ToInt32(row["ComponentID"]);

            // Skip if we've already processed this component
            if (processedComponents.ContainsKey(componentId))
                continue;

            processedComponents[componentId] = true;

            string componentName = row["ComponentName"].ToString();
            decimal componentAmountUpToDate = 0;
            decimal componentAmountSincePrevious = 0;

            // Component header
            sb.Append("<tr style='background: #f5f5f5;'>" +
                      "<td colspan='8' style='border: 1px solid #000; padding: 8px; font-weight: bold; background: #e0e0e0;'>" +
                      "Component: " + componentName +
                      "</td></tr>");

            // Second pass: Process items for this component
            foreach (DataRow itemRow in details.Rows)
            {
                if (Convert.ToInt32(itemRow["ComponentID"]) != componentId)
                    continue;

                componentAmountUpToDate += SafeConvert.ToDecimal(itemRow["AmountUpToDate"]);
                componentAmountSincePrevious += SafeConvert.ToDecimal(itemRow["AmountSincePrevious"]);

                sb.Append("<tr>" +
                          "<td style='border: 1px solid #000; padding: 6px; text-align: center;'>" + itemRow["ActualUnit"] + "</td>" +
                          "<td style='border: 1px solid #000; padding: 6px; text-align: center;'>" + SafeConvert.ToDecimal(itemRow["CumulativeQuantity"]).ToString("N2") + "</td>" +
                          "<td style='border: 1px solid #000; padding: 6px; text-align: center;'>" + itemRow["SORFrom"] + "</td>" +
                          "<td style='border: 1px solid #000; padding: 6px; text-align: center;'>" + itemRow["Description"] + "</td>" +
                          "<td style='border: 1px solid #000; padding: 6px; text-align: center;'>Rs. " + SafeConvert.ToDecimal(itemRow["Rate"]).ToString("N2") + "</td>" +
                          "<td style='border: 1px solid #000; padding: 6px; text-align: center;'>" + itemRow["GST"] + "</td>" +
                          "<td style='border: 1px solid #000; padding: 6px; text-align: center;'>Rs. " + SafeConvert.ToDecimal(itemRow["AmountUpToDate"]).ToString("N2") + "</td>" +
                          "<td style='border: 1px solid #000; padding: 6px; text-align: center;'>Rs. " + SafeConvert.ToDecimal(itemRow["AmountSincePrevious"]).ToString("N2") + "</td>" +
                          "</tr>");
            }

            // Component total
            sb.Append("<tr style='background:#fff; font-weight:bold;'>" +
                      "<td colspan='6' style='border:1px solid #000; padding:6px; text-align:right;'>" +
                      "Component Total:" +
                      "</td>" +
                      "<td style='border:1px solid #000; padding:6px; text-align:center;'>" +
                      "Rs. " + componentAmountUpToDate.ToString("N2") +
                      "</td>" +
                      "<td style='border:1px solid #000; padding:6px; text-align:center;'>" +
                      "Rs. " + componentAmountSincePrevious.ToString("N2") +
                      "</td></tr>");
        }

        // Overall totals
        sb.Append("<tr style='background:#d0d0d0; font-weight:bold;'>" +
                  "<td colspan='6' style='border:1px solid #000; padding:6px; text-align:right;'>" +
                  "Overall Total:" +
                  "</td>" +
                  "<td style='border:1px solid #000; padding:6px; text-align:center;'>" +
                  "Rs. " + overallAmountUpToDate.ToString("N2") +
                  "</td>" +
                  "<td style='border:1px solid #000; padding:6px; text-align:center;'>" +
                  "Rs. " + overallAmountSincePrevious.ToString("N2") +
                  "</td></tr>");

        sb.Append("<tr style='background:#d0d0d0; font-weight:bold;'>" +
                  "<td colspan='6' style='border:1px solid #000; padding:6px; text-align:right;'>" +
                  "Add <strong>" + sorPercentage.ToString() + "%</strong> Above/Below SOR Rs.:" +
                  "</td>" +
                  "<td style='border:1px solid #000; padding:6px; text-align:center;'>" +
                  "Rs. " + sorAmountUpToDate.ToString("N2") +
                  "</td>" +
                  "<td style='border:1px solid #000; padding:6px; text-align:center;'>" +
                  "Rs. " + sorAmountSincePrevious.ToString("N2") +
                  "</td></tr>");

        sb.Append("<tr style='background:#d0d0d0; font-weight:bold;'>" +
                  "<td colspan='6' style='border:1px solid #000; padding:6px; text-align:right;'>" +
                  "Total Value of work done or Supplies or made to date [A]:" +
                  "</td>" +
                  "<td colspan='2' style='border:1px solid #000; padding:6px; text-align:center;'>" +
                  "Rs. " + totalValue.ToString("N2") +
                  "</td></tr>");

        sb.Append("<tr style='background:#d0d0d0; font-weight:bold;'>" +
                  "<td colspan='6' style='border:1px solid #000; padding:6px; text-align:right;'>" +
                  "Deduct value of work or supplies shown on previous bill" +
                  "</td>" +
                  "<td colspan='2' style='border:1px solid #000; padding:6px; text-align:center;'>" +
                  "Rs. " + previousBillValue.ToString("N2") +
                  "</td></tr>");

        sb.Append("<tr style='background:#c0c0c0; font-weight:bold;'>" +
                  "<td colspan='6' style='border:1px solid #000; padding:6px; text-align:right;'>" +
                  "Net Value of works supplies since previous bill [F]" +
                  "</td>" +
                  "<td colspan='2' style='border:1px solid #000; padding:6px; text-align:center;'>" +
                  "Rs. " + netValue.ToString("N2") +
                  "</td></tr>");

        sb.Append("<tr style='background:#fff; font-weight:bold;'>" +
                  "<td colspan='4' style='border:1px solid #000; padding:6px; text-align:left;'>" +
                  "Figure [F] in Words: " + NumberToIndianCurrencyWords(overallAmountSincePrevious) +
                  "</td>" +
                  "<td colspan='4' style='border:1px solid #000; padding:6px; text-align:center;'>" +
                  "SAY RS.: " + netValue.ToString("N2") +
                  "</td></tr>");

        sb.Append("</tbody></table></div>");

        // Signature section
        sb.Append("<div CssClass='page-break' style='border-top: 2px solid #000; padding: 15px; background: #f0f0f0; text-align: center;'>" +
                  "<div style='font-weight: bold; font-size: 16px;'>" +
                  "II - CERTIFICATE AND SIGNATURE" +
                  "</div></div>");

        sb.Append("<div style='padding: 15px 10px; font-size: 12px; font-family: Arial, sans-serif; border: 1px solid #000; margin-top: 20px;'>" +
                  "The measurement were made by " +
                  "<strong>" + GetSubEngineerUserNameForBill(reader["WorkCode"].ToString()) + "</strong> S/E " +
                  "and recorded on Date " +
                  "<span style='display:inline-block; border-bottom:1px solid #000; min-width:100px;'>&nbsp;</span> " +
                  "to " +
                  "<span style='display:inline-block; border-bottom:1px solid #000; min-width:100px;'>&nbsp;</span> " +
                  "of eMB Book No. " +
                  "<strong>" + GetEMBBookNumberForBill(reader["WorkCode"].ToString()) + "</strong> " +
                  "and advance payment has been previously made without detailed measurements." +
                  "</div>");

        sb.Append("<div style='display:flex; justify-content:space-between; margin-top:30px; font-family:Arial, sans-serif; font-size:12px;'>" +
                  "<div style='width:45%;'>" +
                  "<div style='border:1px solid #000; margin-left:10px; height:80px; width:100%;'></div>" +
                  "<div style='margin-top:4px; margin-bottom:10px; text-align:center;'>" +
                  "Signature of officer preparing the bill" +
                  "</div>" +
                  "<div style='margin-bottom:20px; margin-left:10px;'>" +
                  "Date: <span style='border-bottom:1px solid #000; display:inline-block; width:120px;'>&nbsp;</span>" +
                  "</div>" +
                  "<div style='border:1px solid #000; margin-left:10px; height:80px; width:100%;'></div>" +
                  "<div style='margin-top:4px; margin-bottom:10px; text-align:center;'>" +
                  "Signature of officer authorising the bill" +
                  "</div>" +
                  "<div style='margin-left:10px; margin-bottom:10px;'>" +
                  "Date: <span style='border-bottom:1px solid #000; display:inline-block; width:120px;'>&nbsp;</span>" +
                  "</div></div>" +
                  "<div style='width:45%; text-align:right;'>" +
                  "<div style='margin-bottom:8px; margin-right:10px'>" +
                  "<strong>Rank - Sub Divisional Officer</strong>" +
                  "</div>" +
                  "<div style='margin-bottom:6px;'>" +
                  "<div class='form-field' style='margin-bottom:5px;'>" +
                  "<label style='width:auto; '>Sub-Division:</label>" +
                  "<span>" + reader["SubDivision2"].ToString() + "</span>" +
                  "</div></div>" +
                  "<div style='margin-bottom:20px;'>" +
                  "<div class='form-field' style='margin-bottom:5px;'>" +
                  "<label style='width:auto; '>Division:</label>" +
                  "<span>" + reader["Division2"].ToString() + "</span>" +
                  "</div></div>" +
                  "<div>" +
                  "Payment Date: <span style='border-bottom:1px solid #000; margin-right:10px; display:inline-block; width:120px;'>&nbsp;</span>" +
                  "</div></div></div>");

        sb.Append("</div>"); // End of main container
        return sb.ToString();
    }

    // Stub implementations for helper methods
    private decimal GetSORPercentage()
    {
        return 10; // Default value
    }
    private string GetSubEngineerUserNameForBill(string workCode)
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

    private string GetEMBBookNumberForBill(string workCode)
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

    // Reuse this from GenerateBill.aspx.cs
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

    private string RenderMemoSection(SqlDataReader reader)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(@"
    <div style='max-width: 800px; margin: 0 auto; border: 2px solid #000; font-family: Arial, sans-serif; font-size: 11px; background: white;'>
        <!-- Header Section -->
        <div style='text-align: center; padding: 8px; border-bottom: 2px solid #000; background: #f8f8f8; font-weight: bold; font-size: 13px; text-transform: uppercase;'>
            III - MEMORANDUM OF PAYMENT
        </div>

        <!-- Main Memorandum Table -->
        <table style='width: 100%; border-collapse: collapse; font-size: 10px;'>
            <thead>
                <tr style='background: #e8e8e8; font-weight: bold;'>
                    <th style='border: 1px solid #000; padding: 6px; text-align: center; width: 70%; text-transform: uppercase; font-weight: bold;'>
                        FIGURES OF MEMORANDUM
                    </th>
                    <th style='border: 1px solid #000; padding: 6px; text-align: center; width: 30%; text-transform: uppercase; font-weight: bold;'>
                        RS.
                    </th>
                </tr>
            </thead>
            <tbody>
                <!-- Row 1: Total Value -->
                <tr>
                    <td style='border: 1px solid #000; padding: 6px; font-weight: bold;'>
                        1. Total Value of work done as per account I, Column 5 entry [A]
                    </td>
                    <td style='border: 1px solid #000; padding: 6px; text-align: center;'>
                        " + Convert.ToDecimal(reader["MemoTotalValue"]).ToString("N2") + @"
                    </td>
                </tr>
    
                <!-- Row 2: Deduct amount without -->
                <tr>
                    <td style='border: 1px solid #000; padding: 6px; font-weight: bold;'>
                        2. Deduct amount with -
                    </td>
                    <td style='border: 1px solid #000; padding: 6px; text-align: right;'>
                    </td>
                </tr>
    
                <!-- Sub-row 2a: From previous bill -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 25px;'>
                        a) From previous Running Bill
                    </td>
                    <td style='border: 1px solid #000; padding: 6px; text-align: center;'>
                        " + Convert.ToDecimal(reader["PreviousBillAmount"]).ToString("N2") + @"
                    </td>
                </tr>
    
                <!-- Sub-row 2b: From this Bill -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 25px;'>
                        b) From this Bill
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["ThisBillAmount"]).ToString("N2") + @"
                    </td>
                </tr>

                <!-- Total 2 [3] 4, 5 (c) G -->
                <tr>
                    <td style='border: 1px solid #000; padding: 6px; text-align: right; font-weight: bold;'>
                        Total 2(a) + 2(b)
                    </td>
                    <td style='border: 1px solid #000; padding: 6px; text-align: center; font-weight: bold;'>
                        " + Convert.ToDecimal(reader["Total2"]).ToString("N2") + @"
                    </td>
                </tr>
    
                <!-- Row 3: Balance -->
                <tr>
                    <td style='border: 1px solid #000; padding: 6px; font-weight: bold;'>
                        3. Balance i.e. up to date payment (Item 1-2)
                    </td>
                    <td style='border: 1px solid #000; padding: 6px; text-align: center; font-weight: bold;'>
                        " + Convert.ToDecimal(reader["Balance3"]).ToString("N2") + @"
                    </td>
                </tr>
    
                <!-- Row 4: Total account of payment -->
                <tr>
                    <td style='border: 1px solid #000; padding: 6px; font-weight: bold;'>
                        4. Total amount of work done for Work Code 
                        " + reader["WorkCode4"].ToString() + @"
                    </td>
                    <td style='border: 1px solid #000; padding: 6px; text-align: center;'>
                        " + Convert.ToDecimal(reader["TotalWorkDone"]).ToString("N2") + @"
                    </td>
                </tr>
    
                <!-- Row 5: Payment not to be made -->
                <tr>
                    <td style='border: 1px solid #000; padding: 6px; font-weight: bold;'>
                        5. Payment now to be made as detailed below:
                    </td>
                    <td style='border: 1px solid #000; padding: 6px; text-align: right;'>
                    </td>
                </tr>
    
                <!-- Sub-row 5a: By recovery -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 25px;'>
                        a) By recovery of the amount creditable to this work
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["RecoveryAmount"]).ToString("N2") + @"
                    </td>
                </tr>
    
                <!-- Sub-row 5b: by recovery of amounts -->
                <tr>
                    <td style='border: 1px solid #000; padding: 4px; padding-left: 25px;'>
                        b) By other deductions
                    </td>
                </tr>

                <!-- Sub-row 5b1: Security Deposit -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 50px;'>
                        1) Security Deposit
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["SecurityDeposit"]).ToString("N2") + @"
                    </td>
                </tr>

                <!-- Sub-row 5b2: Income Tax -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 50px;'>
                        2) Income Tax
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["IncomeTax"]).ToString("N2") + @"
                    </td>
                </tr>

                <!-- Sub-row 5b3: GST Amount -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 50px;'>
                        3) GST Amount
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["GSTAmount"]).ToString("N2") + @"
                    </td>
                </tr>

                <!-- Sub-row 5b4: Labour Welfare -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 50px;'>
                        4) Labour Welfare
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["LabourWelfare"]).ToString("N2") + @"
                    </td>
                </tr>

                <!-- Sub-row 5b5: Time Extension (Withheld) -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 50px;'>
                        5) Time Extension (Withheld)
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["TimeExtensionWithheld"]).ToString("N2") + @"
                    </td>
                </tr>

                <!-- Sub-row 5b6: Royalty -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 50px;'>
                        6) Royalty
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["Royalty"]).ToString("N2") + @"
                    </td>
                </tr>

                <!-- Sub-row 5b7: Miscellaneous Deposit -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 50px;'>
                        7) Miscellaneous Deposit
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["MiscellaneousDeposit"]).ToString("N2") + @"
                    </td>
                </tr>

                <!-- Sub-row 5b8: Penalty for Time Extension -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 50px;'>
                        8) Penalty for Time Extension
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["PenaltyForTimeExtension"]).ToString("N2") + @"
                    </td>
                </tr>

                <!-- Sub-row 5b9: Penalty for Work -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 50px;'>
                        9) Penalty for Work
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["PenaltyForWork"]).ToString("N2") + @"
                    </td>
                </tr>

                <!-- Sub-row 5b10: Cost of Bill Form -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 50px;'>
                        10) Cost of Bill Form
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["CostOfBillForm"]).ToString("N2") + @"
                    </td>
                </tr>

                <!-- Sub-row 5c: By transfer -->
                <tr>
                    <td style='border: 1px solid #000; padding: 2px; padding-left: 25px;'>
                        c) By Transfer
                    </td>
                    <td style='border: 1px solid #000; padding: 2px; text-align: center;'>
                        " + Convert.ToDecimal(reader["ByTransfer"]).ToString("N2") + @"
                    </td>
                </tr>
    
                <!-- Total 5 [3] 4 (c) H -->
                <tr>
                    <td style='border: 1px solid #000; padding: 6px; text-align: right; font-weight: bold;'>
                        Total 5(a) + 5(b) + 5(c)
                    </td>
                    <td style='border: 1px solid #000; padding: 6px; text-align: center; font-weight: bold;'>
                        " + Convert.ToDecimal(reader["Total5"]).ToString("N2") + @"
                    </td>
                </tr>

            </tbody>
        </table>

        <!-- Payment Section -->
        <div style='border-top: 2px solid #000; padding: 12px; background: #f9f9f9;'>
            <div style='display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 15px;'>
                <div style='font-weight: bold; flex: 1;'>
                    Pay Total By Transfer Rs. 
                    <span style='font-weight: bold; color: #333; background-color: #f0f0f0; padding: 4px 8px; border-radius: 4px;'>"
                        + Convert.ToDecimal(reader["PayAmount"]).ToString("N2") + @"</span>
                </div>
                <div style='text-align: right;'>
                    <div style='border: 1px solid #000; padding: 8px; width: 120px; height: 40px; display: flex; align-items: center; justify-content: center; margin-bottom: 8px; background: white;'>
                        <!-- Officer Box -->
                    </div>
                    <div style='font-size: 9px; text-align: center;'>
                        Dated Initials of Disbursing Officer
                    </div>
                </div>
            </div>
        </div>

        <!-- Bottom Section -->
        <div style='border-top: 1px solid #000; padding: 12px; background: white;'>
            <div style='margin-bottom: 12px; font-size: 14px;'>
                Received Rs <span style='font-weight: bold; color: #333; background-color: #f0f0f0; padding: 4px 8px; border-radius: 4px;'>"
                    + Convert.ToDecimal(reader["ReceivedAmount"]).ToString("N2") + @"</span> 
                as per above memorandum on account of this work in full settlement of all demands.
            </div>

            <div style='display: flex; justify-content: space-between; align-items: flex-start; margin-top: 20px;'>
                <div style='flex: 1;'>
                    <div style='margin-bottom: 15px;'>
                        Date: <span style='border-bottom: 1px solid #000;'>" + reader["ReceiptDate"].ToString() + @"</span>
                    </div>
    
                    <!-- Thumb Impression Box -->
                    <div style='margin-bottom: 8px;'>
                        <div style='border: 1px solid #000; width: 80px; height: 60px; display: inline-block; margin-bottom: 5px; background: white;'></div>
                    </div>
                    <div style='font-size: 9px; margin-bottom: 20px;'>
                        Left Hand Thumb Impression
                    </div>
    
                    <!-- Contractor Signature Box -->
                    <div style='margin-bottom: 8px;'>
                        <div style='border: 1px solid #000; width: 150px; height: 40px; display: inline-block; margin-bottom: 5px; background: white;'></div>
                    </div>
                    <div style='font-size: 9px; text-align: center; width: 150px;'>
                        Full Signature of Contractor
                    </div>
                </div>

                <div style='text-align: right;'>
                    <div style='border: 1px solid #000; padding: 15px; width: 100px; height: 60px; display: flex; align-items: center; justify-content: center; font-weight: bold; background: white;'>
                        STAMP
                    </div>
                    <div style='display: flex; justify-content: space-between; margin-top: 10px;'>
                        <div style='flex: 1;'>
                        </div>
                        <div style='text-align: right;'>
                            <div style='margin-bottom: 8px; margin-right:20px'>
                                Dated: <span style='border-bottom: 1px solid #000;'>" + reader["PaidDate"].ToString() + @"</span>
                            </div>
                            <div style='margin-right:20px'>
                                Cashier: <span style='border-bottom: 1px solid #000;'>" + reader["Cashier"].ToString() + @"</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Moved up the Dated and Cashier fields -->

            <div style='margin-top: 20px; border-top: 1px solid #000; padding-top: 8px;'>
                <div style='display: flex; justify-content: space-between; align-items: flex-start;'>
                    <div>
                        Vide Cheque No.: <span style='border-bottom: 1px solid #000;'>" + reader["ChequeNo"].ToString() + @"</span>
                    </div>
                    <div style='text-align: right;'>
                        <div style='border: 1px solid #000; width: 180px; height: 30px; display: inline-block; margin-bottom: 5px; background: white;'></div>
                        <div style='font-size: 9px; text-align: center; width: 180px;'>
                            Dated initials of person actually making the payment
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>");

        return sb.ToString();
    }

    private DataTable GetBillDetails(int billId)
    {
        DataTable dt = new DataTable();
        string connStr = WebConfigurationManager.ConnectionStrings["eMB"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            string sql = @"SELECT 
                    c.ComponentName, d.Description, d.ActualUnit, d.QuantitySincePrevious,
                    d.Rate, d.SORFrom, d.GST, d.AmountUpToDate, d.ComponentID,
                    d.CumulativeQuantity, d.AmountSincePrevious
                FROM BillDetails d
                JOIN eMB_ComponentMaster c ON d.ComponentID = c.ComponentID
                WHERE BillId = @BillId";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@BillId", billId);
            conn.Open();
            dt.Load(cmd.ExecuteReader());
        }
        return dt;
    }
}