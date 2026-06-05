<%@ Page Title="" Language="C#" MasterPageFile="../MasterPages/MasterPage.master" AutoEventWireup="true" CodeFile="GenerateBill.aspx.cs" Inherits="GenerateBill" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

    <style>
        .phed-cg-progress-container {
            padding: 10px 0px !important;
            max-width: 100% !important;
            margin: 0 auto !important;
        }
        .phed-cg-stepper-wrapper {
            display: flex !important;
            justify-content: space-between !important;
            position: relative !important;
        }
        .phed-cg-stepper-item {
            position: relative !important;
            z-index: 1 !important;
            text-align: center !important;
            flex: 1 !important;
        }
        .phed-cg-step-indicator {
            width: 30px !important;
            height: 30px !important;
            border: 3px solid #dee2e6 !important;
            background: white !important;
            border-radius: 25% !important;
            display: flex !important;
            align-items: center !important;
            justify-content: center !important;
            margin: 0 auto 5px !important;
            font-weight: bold !important;
            transition: all 0.3s ease !important;
        }
        .phed-cg-stepper-item.phed-cg-completed .phed-cg-step-indicator {
            background: #0066cc !important;
            border-color: #0066cc !important;
            color: white !important;
        }
        .phed-cg-stepper-item.phed-cg-completed .phed-cg-step-indicator::after {
            content: '' !important;
            margin-left: 2px !important;
        }
        .phed-cg-stepper-wrapper::before {
            content: '' !important;
            position: absolute !important;
            top: 15px !important;
            left: 10% !important;
            right: 10% !important;
            height: 3px !important;
            background: #dee2e6 !important;
            z-index: 0 !important;
        }
        .phed-cg-step-text {
            font-size: 14px !important;
            color: #6c757d !important;
            margin-top: 0px !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="TitleContent" Runat="Server">
eMB Abstract Bill Generation
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="BreadcrumbContent" Runat="Server">
<ul class="breadcrumb-title">
    <li class="breadcrumb-item">
        <a href="frmHome.aspx"> <i class="fa fa-home"></i> </a>
    </li>
    <li class="breadcrumb-item">
        <a href="frmHome.aspx">Dashboard</a>
    </li> 
</ul>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" Runat="Server">
    <style>
        .bill-history-container {
        margin: 20px 0;
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        border-radius: 8px;
        overflow: hidden;
        background: white;
    }

    /* Header with Icon */
    .bill-history-header {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        padding: 20px;
        margin: 0;
        font-size: 18px;
        font-weight: 600;
        display: flex;
        align-items: center;
        gap: 10px;
    }

    .bill-history-header::before {
        content: "📋";
        font-size: 20px;
    }

    /* GridView Base Styling */
    .styled-gridview {
        width: 100%;
        border-collapse: collapse;
        margin: 0;
        background: white;
    }

    /* Header Styling */
    .styled-gridview th {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        font-weight: 600;
        padding: 15px 12px;
        text-align: left;
        font-size: 14px;
        letter-spacing: 0.5px;
        border: none;
    }

    /* Row Styling */
    .styled-gridview td {
        padding: 12px;
        border-bottom: 1px solid #e8e8e8;
        vertical-align: middle;
        font-size: 13px;
        color: #333;
    }

    /* Alternating Row Colors */
    .styled-gridview tr:nth-child(even) {
        background-color: #f8f9fa;
    }

    .styled-gridview tr:nth-child(odd) {
        background-color: white;
    }

    /* Row Hover Effect */
    .styled-gridview tr:hover {
        background-color: #e3f2fd;
        transition: background-color 0.2s ease;
    }

    /* Bill Number Styling */
    .bill-number {
        font-family: 'Courier New', monospace;
        font-weight: 600;
        color: #2c3e50;
        font-size: 12px;
    }

    /* Date Styling */
    .bill-date {
        color: #5a5a5a;
        font-weight: 500;
    }

    /* View Button Styling */
    .btn-view {
        background: linear-gradient(135deg, #36d1dc 0%, #5b86e5 100%);
        color: white;
        border: none;
        padding: 8px 16px;
        border-radius: 20px;
        font-size: 12px;
        font-weight: 500;
        cursor: pointer;
        transition: all 0.3s ease;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .btn-view:hover {
        transform: translateY(-2px);
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
    }

    /* Lock & Forward Button */
    .btn-lock-forward {
        background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%);
        color: white;
        border: none;
        padding: 8px 16px;
        border-radius: 20px;
        font-size: 12px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.3s ease;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .btn-lock-forward:hover {
        transform: translateY(-2px);
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
    }

    /* Status Labels */
    .status-label {
        padding: 6px 12px;
        border-radius: 15px;
        font-size: 11px;
        font-weight: 600;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        display: inline-block;
        min-width: 80px;
        text-align: center;
    }

    .status-approved {
        background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
        color: white;
    }

    .status-rejected {
        background: linear-gradient(135deg, #ff6b6b 0%, #ee5a24 100%);
        color: white;
    }

    .status-draft {
        background: linear-gradient(135deg, #feca57 0%, #ff9ff3 100%);
        color: white;
    }

    /* Action Column Styling */
    .action-column {
        text-align: center;
        width: 120px;
    }

    .view-column {
        text-align: center;
        width: 80px;
    }
        .data-table { 
    width: 100%; 
    border-collapse: collapse; 
}

.data-table td, 
.data-table th { 
    padding: 8px; 
    border: 1px solid #ccc; 
}

/* Alternative naming options: */

.custom-grid { 
    width: 100%; 
    border-collapse: collapse; 
}

.custom-grid td, 
.custom-grid th { 
    padding: 8px; 
    border: 1px solid #ccc; 
}

.report-table { 
    width: 100%; 
    border-collapse: collapse; 
}

.report-table td, 
.report-table th { 
    padding: 8px; 
    border: 1px solid #ccc; 
}
        
        /* Bill table styles */
.gb-bill-table { 
            width: 100%; 
            border-collapse: collapse; 
            margin-bottom: 10px;
        }
.gb-bill-table th, .bill-table td { 
            border: 1px solid #ddd; 
            padding: 8px; 
            text-align: left;
        }
.gb-component-header { 
            background-color: #f2f2f2; 
            font-weight: bold; 
        }
.gb-total { 
            font-weight: bold; 
            background-color: #d4d4d4; 
            font-size: 1.1em; 
            padding: 10px;
            margin: 10px 0;
        }
        
        /* FIXED Modal styles */
.gb-modalBackground {
            position: fixed; 
            top: 0; 
            left: 0; 
            width: 100%; 
            height: 100%;
            background-color: rgba(0, 0, 0, 0.7);
            display: none;
            z-index: 1000;
        }
        
.gb-modalPopup {
            position: fixed; 
            top: 5%; 
            left: 50%; 
            transform: translateX(-50%);
            background-color: #fff; 
            border: 1px solid #888; 
            padding: 20px;
            width: 100%; 
            max-width: 1200px;
            height: 85%; 
            max-height: 85vh;
            overflow-y: auto;
            overflow-x: hidden;
            display: none;
            z-index: 10000;
            box-shadow: 0 4px 8px rgba(0,0,0,0.3);
            border-radius: 5px;
        }
        
.gb-closeModal { 
            position: absolute; 
            top: 10px; 
            right: 15px; 
            font-size: 20px; 
            cursor: pointer; 
            color: #666;
            font-weight: bold;
        }
        
.gb-closeModal:hover {
            color: #000;
        }
        
        /* Button styles within modal */
.gb-modal-buttons {
            margin-top: 20px;
            padding-top: 15px;
            border-top: 1px solid #eee;
            text-align: center;
        }
        
.gb-modal-buttons input[type="submit"] {
            margin: 0 10px;
            padding: 8px 20px;
        }
        
        /* Table container for better scrolling */
.gb-table-container {
            border: 1px solid #ddd;
            margin: 10px 0;
            width: 100px
        }
        
        /* Ensure content stays within modal */
.gb-modalPopup * {
            box-sizing: border-box;
        }
        
        /* Form elements within modal */
.gb-modalPopup input[type="text"], 
.gb-modalPopup textarea {
            max-width: 100%;
        }
.gb-form-container {
            max-width: 800px;
            margin: 20px auto;
            border: 2px solid #000;
            padding: 0;
            font-family: Arial, sans-serif;
            font-size: 12px;
            background: white;
        }

.gb-form-header {
            display: flex;
            justify-content: space-between;
            padding: 10px 15px;
            border-bottom: 1px solid #000;
            background: #f8f8f8;
        }

.gb-form-header-left {
            font-weight: bold;
        }

.gb-form-header-right {
            text-align: right;
            font-weight: bold;
        }

.gb-form-body {
            display: flex;
            min-height: 600px;
        }

.gb-left-column {
            width: 45%;
            padding: 15px;
            border-right: 1px solid #000;
            background: #f9f9f9;
        }

.gb-right-column {
            width: 55%;
            padding: 15px;
        }

.gb-notes-title {
            font-weight: bold;
            font-size: 14px;
            margin-bottom: 10px;
            text-decoration: underline;
        }

.gb-notes-subtitle {
            font-size: 11px;
            margin-bottom: 15px;
        }

.gb-note-item {
            margin-bottom: 15px;
            line-height: 1.4;
        }

.gb-note-number {
            font-weight: bold;
            margin-right: 5px;
        }

.gb-highlight-blue {
            color: #0066cc;
            font-weight: bold;
        }

.gb-right-header {
            text-align: right;
            margin-bottom: 20px;
        }

.gb-division-fields .form-field {
            margin-bottom: 5px;
            display: flex;
            align-items: center;
            border-bottom: 1px solid #000;
            padding-bottom: 2px;
        }

.gb-division-fields .form-field label {
            min-width: auto;
            margin-right: 5px;
            font-weight: bold;
        }

.gb-division-line {
            border-bottom: 1px solid #000;
            margin-bottom: 5px;
            padding-bottom: 2px;
            min-height: 20px;
        }

.gb-form-field {
            margin-bottom: 15px;
            display: flex;
            align-items: center;
        }

.gb-form-field label {
            margin-right: 10px;
            min-width: 120px;
            font-weight: bold;
        }

.gb-form-field input, .form-field textarea {
            flex: 1;
            padding: 5px;
            border: 1px solid #666;
            font-size: 12px;
        }

.gb-cash-voucher {
            display: flex;
            justify-content: space-between;
            margin-bottom: 15px;
        }

.gb-workname-box {
          width: 100%;
          box-sizing: border-box;
          border: none;
          border-bottom: 1px solid #000;
          background: transparent;
          overflow: hidden;
          resize: none;
          padding: 4px 0;
          font: inherit;
        }

.gb-cash-voucher input {
            width: 60px;
            text-align: center;
            border: none;
            border-bottom: 1px solid #000;
            background: transparent;
        }

.gb-running-account {
            text-align: center;
            font-weight: bold;
            margin-bottom: 20px;
            font-size: 13px;
        }

.gb-contractor-field {
            margin-bottom: 30px;
        }

.gb-underline-input {
            border: none !important;
            border-bottom: 1px solid #000 !important;
            background: transparent !important;
            padding: 5px 0 !important;
            width: 100% !important;
            resize: none;
            white-space: pre-wrap;
            overflow-wrap: break-word;
        }

.gb-reference-table {
            margin-bottom: 20px;
        }

.gb-reference-row {
            display: flex;
            margin-bottom: 10px;
            align-items: center;
        }

.gb-reference-row label {
            min-width: 100px;
            font-weight: bold;
        }

.gb-completion-section {
            margin-bottom: 20px;
        }

.gb-completion-item {
            margin-bottom: 8px;
            padding-left: 15px;
            position: relative;
        }

.gb-completion-item:before {
            content: "• ";
            position: absolute;
            left: 0;
            font-weight: bold;
        }
.btn-new-bill {
    background: linear-gradient(135deg, #4f46e5 0%, #3b82f6 100%);
    border: none;
    color: white;
    padding: 12px 24px;
    font-size: 16px;
    font-weight: 600;
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.3s ease;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
    margin-bottom: 20px;
    display: inline-flex;
    align-items: center;
    gap: 8px;
}

.btn-new-bill:hover {
    background: linear-gradient(135deg, #4338ca 0%, #2563eb 100%);
    transform: translateY(-2px);
    box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
}

.btn-new-bill:active {
    transform: translateY(0);
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
}

.btn-new-bill:focus {
    outline: none;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06), 0 0 0 3px rgba(79, 70, 229, 0.3);
}

/* Add an icon before the text (optional) */
.btn-new-bill::before {
    content: "+";
    font-size: 18px;
    font-weight: bold;
}

/* Responsive design */
@media (max-width: 768px) {
    .btn-new-bill {
        width: 100%;
        justify-content: center;
        padding: 14px 20px;
    }
}
        
        /* Responsive adjustments */
        @media (max-width: 768px) {
.gb-modalPopup {
                width: 95%;
                height: 90%;
                top: 5%;
                padding: 15px;
            }
        }
    </style>
    <script type="text/javascript">
        function showBillPopup() {
            document.getElementById('<%= modalBg.ClientID %>').style.display = 'block';
            document.getElementById('<%= pnlBillPopup.ClientID %>').style.display = 'block';
        }
        function hideBillPopup() {
            document.getElementById('<%= modalBg.ClientID %>').style.display = 'none';
            document.getElementById('<%= pnlBillPopup.ClientID %>').style.display = 'none';
        }
        function autoResizeTextarea(el) {
            el.style.height = '20px';                   // reset any previous explicit height
            el.style.height = el.scrollHeight + 'px';   // set to the full content height
        }

        document.addEventListener('DOMContentLoaded', function () {
            var boxes = document.querySelectorAll('.workname-box');
            boxes.forEach(function (box) {
                autoResizeTextarea(box);

                box.addEventListener('input', function () {
                    autoResizeTextarea(box);
                });
            });
        });
    </script>
    <asp:Panel
          ID="modalBg"
          runat="server"
          Cssclass="gb-modalBackground"
          Style="display:none" />

        <!-- Panel 1: Search for Work Code -->
        <asp:Panel ID="pnlSearch" runat="server" Visible="true">
            <asp:Label ID="lblEnterCode" runat="server" Text="Enter Work Code:" AssociatedControlID="txtWorkCode" />
            <asp:TextBox ID="txtWorkCode" runat="server" Width="200px" />
            &nbsp;
            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
        </asp:Panel>

        <!-- Panel 2: History of existing bills -->
        <asp:Panel ID="pnlHistory" runat="server" Visible="false">
           <asp:Button 
    ID="btnNewBill" 
    runat="server" 
    Text="Generate New Bill" 
    OnClick="btnNewBill_Click" 
    CssClass="btn-new-bill" />
            <asp:Button 
    ID="Button1" 
    runat="server" 
    Text="Edit eMB" 
    OnClick="btnEditeMB" 
    CssClass="btn-new-bill" />
    <h3 class="bill-history-header">Bill History</h3>
    
    <asp:GridView ID="gvHistory" runat="server" 
        AutoGenerateColumns="false" 
        DataKeyNames="BillId" 
        OnRowCommand="gvHistory_RowCommand" 
        OnRowDataBound="gvHistory_RowDataBound"
        CssClass="styled-gridview"
        GridLines="None"
        UseAccessibleHeader="true"> 
        
        <HeaderStyle CssClass="gridview-header" />
        <RowStyle CssClass="gridview-row" />
        <AlternatingRowStyle CssClass="gridview-alt-row" />
        
        <Columns>
            <asp:TemplateField HeaderText="Bill Number">
                <ItemTemplate>
                    <span class="bill-number"><%# Eval("BillNumber") %></span>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Date">
                <ItemTemplate>
                    <span class="bill-date"><%# Eval("BillDate", "{0:dd/MM/yyyy}") %></span>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Action">
                <ItemTemplate>
                    <asp:Button ID="btnView" runat="server" 
                        Text="View" 
                        CssClass="btn-view"
                        CommandName="ViewBill" 
                        CommandArgument='<%# Eval("BillId") %>' />
                </ItemTemplate>
                <ItemStyle CssClass="view-column" />
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <asp:Button ID="btnLockForward" runat="server" 
                        Text="Lock and Forward" 
                        CssClass="btn-lock-forward"
                        CommandName="LockForward" 
                        CommandArgument='<%# Eval("BillId") %>' />
                </ItemTemplate>
                <ItemStyle CssClass="action-column" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
        </asp:Panel>

        <!-- Modal Panel for Bill Creation -->
        <asp:Panel ID="pnlBillPopup" runat="server" Cssclass="gb-modalPopup" Style="display:none;">
            <span class="gb-closeModal" onclick="hideBillPopup()">×</span>
            <asp:MultiView ID="mvBill" runat="server" ActiveViewIndex="0">
                <!-- View 0: Introduction -->
                <asp:View ID="viewIntro" runat="server">
                    <div class="gb-form-container">
                        <!-- Form Header -->
                        <div class="gb-form-header">
                            <div class="gb-form-header-left">
                                XVII-E-47<br/>
                                P.W.D.
                            </div>
                            <div class="gb-form-header-right">
                                P.W. Account Form-E<br/>
                                (Full Sheet)
                            </div>
                        </div>
        
                        <!-- Form Body -->
                        <div class="gb-form-body">
                            <!-- Left Column - Notes (Read-only) -->
                            <div class="gb-left-column">
                                <div class="gb-notes-title">NOTES</div>
                                <div class="gb-notes-subtitle">Chapter K, paragraphs 284, 288 & 289 of the<br/>P.W.D Account Code</div>
                
                                <div class="gb-note-item">
                                    <span class="gb-note-number">1)</span>
                                    This form provides only for payment for work or supplier actually measured.
                                </div>
                
                                <div class="gb-note-item">
                                    <span class="gb-note-number">2)</span>
                                    The <span class="gb-highlight-blue">full name</span> of the work as given in the estimate should be entered on the bill except in the case of <span class="gb-highlight-blue">bills for stock materials</span>. That purpose of supply applicable to the should be filled in and the rest scored out.
                                </div>
                
                                <div class="gb-note-item">
                                    <span class="gb-note-number">3)</span>
                                    If the outlay on the work is recorded by sub heads the total for each sub head should be shown in column 5 of account 1 and against the total for each sub head should be shown in column 5 of account 1 and against the total there should be as entry in column 6.
                                </div>
                
                                <div class="gb-note-item">
                                    <span class="gb-note-number">4)</span>
                                    In part II the second signature is necessary only when the officer who prepares the bill is not the officer who authorises the payment in such a case two signatures are essential.
                                </div>
                
                                <div class="gb-note-item">
                                    <span class="gb-note-number">5)</span>
                                    The figures against item 8 in part III memorandum of payments should agree with the total of figures against lines 4 & 5 if the net amount to be paid is less than Rs 10 and it can not be included in a cheque the payment should be made in cash the pay order being altered suitably and the alteration attested by date initials. The figures in the pay order will be the net amount in line 5(C) and the payee's acknowledgement should be for the gross amount 5(a+b+c) the payment should be attested by some known person when the payee's acknowledgement is given by a seal mark of thumb impression.
                                </div>
                
                                <div class="gb-note-item">
                                    <span class="gb-note-number">6)</span>
                                    Prt. IV is reserved for any remarks which the disbursing officer of the Divisional Officer may wish to record in respect of the execution of the work check of measurements of the state of contractor's accounts.
                                </div>
                            </div>
            
                            <!-- Right Column - Form Fields -->
                            <div class="gb-right-column">
                                <div class="gb-right-header">
                                    <div class="gb-division-fields">
                                        <div class="gb-form-field" style="margin-bottom:5px;">
                                            <label style="width:auto; padding-right:5px;">Sub-Division:</label>
                                            <asp:TextBox ID="txtSubDivision" runat="server" ReadOnly="true" style="flex:1;" />
                                        </div>
                                        <div class="gb-form-field" style="margin-bottom:5px;">
                                            <label style="width:auto; padding-right:5px;">Division:</label>
                                            <asp:TextBox ID="txtDivision" runat="server" ReadOnly="true" style="flex:1;" />
                                        </div>
                                    </div>
                    
                                    <div class="gb-running-account">
                                        RUNNING ACCOUNT BILL
                                    </div>
                                    <div class="gb-form-section">
                                        <div class="gb-form-field">
                                            <label>Cash Book Voucher No.:</label>
                                            <asp:TextBox ID="txtVoucherNo" runat="server" />
                                        </div>
                                        <div class="gb-form-field">
                                            <label>Date:</label>
                                            <asp:TextBox ID="txtVoucherDate" runat="server" placeholder="DD/MM/YYYY" />
                                        </div>
                                    </div>
                                </div>
                
                                <div class="gb-contractor-field">
                                    <div class ="form-field">
                                        <label>Name of Contractors<br/>Or Supplier</label>
                                        <asp:TextBox
                                            ID="txtSupplier"
                                            runat="server"
                                            TextMode="MultiLine"
                                            Cssclass="gb-workname-box"
                                        />
                                    </div>
                                </div>
                
                                <div class="gb-form-section">
                                    <div class="gb-form-field">
                                        <label>Name of work:</label>
                                        <asp:TextBox ID="txtWorkName" runat="server" TextMode="MultiLine" ReadOnly="true" Cssclass="gb-workname-box"/>
                                    </div>
                    
                                    <div class="gb-form-field">
                                        <label>Purpose of Supply:</label>
                                        <asp:TextBox ID="txtPurposeOfSupply" runat="server" TextMode="MultiLine" Cssclass="gb-workname-box" />
                                    </div>
                    
                                    <div class="gb-form-field">
                                        <label>Serial No. of The Bill:</label>
                                        <asp:TextBox ID="txtSerialNo" runat="server" TextMode="MultiLine" Cssclass="gb-workname-box" />
                                    </div>
                    
                                    <div class="gb-form-field">
                                        <label>No. and date his last for the Work:</label>
                                        <asp:TextBox ID="txtLastWorkDate" runat="server" />
                                    </div>
                                </div>
                
                                <div class="gb-reference-table">
                                    <div class="gb-reference-row">
                                        <label>Reference to Agreement No. from Date:</label>
                                    </div>
                                    <asp:TextBox ID="txtAgreementNo" runat="server" />
                                </div>
                
                                <div class="gb-contract-amount">
                                    <div class="gb-form-field">
                                        <label>Probable amount of contract (in Rs.)</label>
                                        <asp:TextBox ID="txtContractAmount" runat="server" />
                                    </div>
                                </div>
                
                                <div class="gb-completion-section">
                                    <div class="gb-completion-item">Due date of completion as per agreement</div>
                                    <div class="gb-completion-item">Actual date of completion of work</div>
                                    <div class="gb-completion-item">To be filled in form other agreement on</div>
                                    <div class="gb-completion-item">"E" Forms</div>
                                </div>

                                <div class="gb-modal-buttons">
                                    <asp:Button ID="btnIntroNext" runat="server" Text="Next → Details" OnClick="GoToDetails" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:View>

                <!-- View 1: Details Table -->
                <asp:View ID="viewTable" runat="server">
                    <div style="max-width: 1000px; margin: 0 auto; border: 2px solid #000; font-family: Arial, sans-serif; font-size: 12px; background: white;">
        
                        <!-- Header Section -->
                        <div style="display: flex; justify-content: space-between; padding: 10px 15px; border-bottom: 2px solid #000; background: #f8f8f8;">
                            <div style="font-weight: bold;">
                                XVII-E-47<br/>
                                P.W.D.
                            </div>
                            <div style="text-align: right; font-weight: bold;">
                                RUNNING ACCOUNT BILL<br/>
                                Account of Work Done or Supplies made
                            </div>
                        </div>
        
                        <!-- Bill Info -->
                        <div style="padding: 10px 15px; border-bottom: 1px solid #000; background: #f9f9f9;">
                            <asp:Label ID="lblBillNumber" runat="server" style="font-weight: bold;" /><br />
                            <asp:Label ID="lblBillDate" runat="server" style="font-weight: bold;" />
                        </div>
        
                        <!-- Main Table -->
                        <asp:Repeater ID="rptComponents" runat="server" OnItemDataBound="rptComponents_ItemDataBound">
                            <HeaderTemplate>
                                <div style="width: 100%; overflow-x: auto; border: 1px solid #ddd; margin: 10px 0;">
                                    <table style="width: auto; min-width: 100%; border-collapse: collapse; font-size: 11px;">
                                        <thead>
                                            <tr style="background: #e8e8e8; font-weight: bold;">
                                                <th style="border: 1px solid #000; padding: 8px; text-align: center; width: 8%;">Unit</th>
                                                <th style="border: 1px solid #000; padding: 8px; text-align: center; width: 12%;">
                                                    Quantity executed or supplied up to date as per eMB
                                                </th>
                                                <th style="border: 1px solid #000; padding: 8px; text-align: center; width: 8%;">Category of Item (SOR/NON-SOR)</th>
                                                <th style="border: 1px solid #000; padding: 8px; text-align: center; width: 25%;">
                                                    Item of work or supplies (grouped under sub-heads and sub-works of estimate)
                                                </th>
                                                <th style="border: 1px solid #000; padding: 8px; text-align: center; width: 10%;">Rate (without GST)</th>
                                                <th style="border: 1px solid #000; padding: 8px; text-align: center; width: 10%;">GST Rate (%)</th>
                                                <th style="border: 1px solid #000; padding: 8px; text-align: center; width: 12%;">Amount Up To Date <br/> (with GST)</th>
                                                <th style="border: 1px solid #000; padding: 8px; text-align: center; width: 12%;">
                                                    Amount Since previous Bill<br/>
                                                    (For each sub-head)<br/>
                                                    (with GST)
                                                </th>
                                            </tr>
                                            <tr style="background: #f0f0f0; font-weight: bold; font-size: 10px;">
                                                <td style="border: 1px solid #000; padding: 4px; text-align: center;">1</td>
                                                <td style="border: 1px solid #000; padding: 4px; text-align: center;">2</td>
                                                <td style="border: 1px solid #000; padding: 4px; text-align: center;">3</td>
                                                <td style="border: 1px solid #000; padding: 4px; text-align: center;">4</td>
                                                <td style="border: 1px solid #000; padding: 4px; text-align: center;">5</td>
                                                <td style="border: 1px solid #000; padding: 4px; text-align: center;">6</td>
                                                <td style="border: 1px solid #000; padding: 4px; text-align: center;">7</td>
                                                <td style="border: 1px solid #000; padding: 4px; text-align: center;">8</td>
                                            </tr>
                                        </thead>
                                        <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr style="background: #f5f5f5;">
                                    <td colspan="8" style="border: 1px solid #000; padding: 8px; font-weight: bold; background: #e0e0e0;">
                                        Component: <%# Eval("ComponentName") %>
                                    </td>
                                </tr>
                                <asp:Repeater ID="rptSORItems" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td style="border: 1px solid #000; padding: 6px; text-align: center;"><%# Eval("ActualUnit") %></td>
                                            <td style="border: 1px solid #000; padding: 6px; text-align: center;"><%# Eval("CumulativeQuantity", "{0:N2}") %></td>
                                            <td style="border: 1px solid #000; padding: 6px; text-align: center;"><%# Eval("SORFrom") %></td>
                                            <td style="border: 1px solid #000; padding: 6px; text-align: center;"><%# Eval("Description") %></td>
                                            <td style="border: 1px solid #000; padding: 6px; text-align: center;"><%# "Rs. " + Eval("Rate", "{0:N2}") %></td>
                                            <td style="border: 1px solid #000; padding: 6px; text-align: center;"><%# Eval("GST") %></td>
                                            <td style="border: 1px solid #000; padding: 6px; text-align: center;"><%# "Rs. " + Eval("AmountUpToDate", "{0:N2}") %></td>
                                            <td style="border: 1px solid #000; padding: 6px; text-align: center;"><%# "Rs. " + Eval("AmountSincePrevious", "{0:N2}") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <tr style="background:#fff; font-weight:bold;">
                                    <td colspan="6" style="border:1px solid #000; padding:6px; text-align:right;">
                                        Component Total:
                                    </td>
                                    <td style="border:1px solid #000; padding:6px; text-align:center;">
                                        <%# "Rs. " + SumAmountUpToDate(Eval("ComponentID")).ToString("N2") %>
                                    </td>
                                    <td style="border:1px solid #000; padding:6px; text-align:center;">
                                        <%# "Rs. " + SumAmountSincePrevious(Eval("ComponentID")).ToString("N2") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                        </tbody>
                                        <!-- Overall Total -->
                                        <tr style="background:#d0d0d0; font-weight:bold;">
                                            <td colspan="6" style="border:1px solid #000; padding:6px; text-align:right;">
                                                Overall Total:
                                            </td>
                                            <td style="border:1px solid #000; padding:6px; text-align:center;">
                                                <%# "Rs. " + GetOverallTotalAmountUpToDate().ToString("N2") %>
                                            </td>
                                            <td style="border:1px solid #000; padding:6px; text-align:center;">
                                                <%# "Rs. " + GetOverallTotalAmountSincePrevious().ToString("N2") %>
                                            </td>
                                        </tr>
                                        <tr style="background:#d0d0d0; font-weight:bold;">
                                          <td colspan="6" style="border:1px solid #000; padding:6px; text-align:right;">
                                            Add <strong><%= GetSORPercentage() %>%</strong> Above/Below SOR Rs.:
                                          </td>
                                          <td style="border:1px solid #000; padding:6px; text-align:center;">
                                                <%# "Rs. " + (GetOverallTotalAmountUpToDate()*(GetSORPercentage())/100).ToString("N2") %>
                                          </td><td style="border:1px solid #000; padding:6px; text-align:center;">
                                                <%# "Rs. " + (GetOverallTotalAmountSincePrevious()*(GetSORPercentage())/100).ToString("N2") %>
                                          </td>
                                        </tr>

                                        <!-- Total Value of work done or Supplies or made to date [A] -->
                                        <tr style="background:#d0d0d0; font-weight:bold;">
                                            <td colspan="6" style="border:1px solid #000; padding:6px; text-align:right;">
                                                Total Value of work done or Supplies or made to date [A]:
                                            </td>
                                            <td colspan="2" style="border:1px solid #000; padding:6px; text-align:center;">
                                                <%# "Rs. " + (GetOverallTotalAmountUpToDate()*(100+GetSORPercentage())/100).ToString("N2") %>
                                            </td>
                                        </tr>

                                        <!-- Deduct value of work or supplies shown on previous bill   -->
                                        <tr style="background:#d0d0d0; font-weight:bold;">
                                            <td colspan="6" style="border:1px solid #000; padding:6px; text-align:right;">
                                                Deduct value of work or supplies shown on previous bill
                                            </td>
                                            <td colspan="2" style="border:1px solid #000; padding:6px; text-align:center;">
                                                <%# "Rs. " + ((GetOverallTotalAmountUpToDate()-GetOverallTotalAmountSincePrevious())*(100+GetSORPercentage())/100).ToString("F2", System.Globalization.CultureInfo.InvariantCulture)%>
                                            </td>
                                        </tr>
                                
                                        <!-- Net Value of works supplies since previous bill [F]   -->
                                        <tr style="background:#c0c0c0; font-weight:bold;">
                                            <td colspan="6" style="border:1px solid #000; padding:6px; text-align:right;">
                                                Net Value of works supplies since previous bill [F]
                                            </td>
                                            <td colspan="2" style="border:1px solid #000; padding:6px; text-align:center;">
                                                <%# "Rs. " + (GetOverallTotalAmountSincePrevious()*(100+GetSORPercentage())/100).ToString("N2") %>
                                            </td>
                                        </tr>
                                        
                                        <tr style="background:#fff; font-weight:bold;">
                                            <td colspan="4" style="border:1px solid #000; padding:6px; text-align:left;">
                                                Figure [F] in Words:
                                                <%= NumberToIndianCurrencyWords(GetOverallTotalAmountSincePrevious()) %>
                                            </td>
                                            <td colspan="4" style="border:1px solid #000; padding:6px; text-align:center;">
                                                SAY Amount (Rs.):
                                                <%= GetSayRs() %>
                                            </td>
                                        </tr>

                                    </table>
                                </div>
                            </FooterTemplate>
                        </asp:Repeater>
        
                        <!-- Total Section -->
                        <div style="border-top: 2px solid #000; padding: 15px; background: #f0f0f0; text-align: center;">
                            <div style="font-weight: bold; font-size: 16px;">
                                II - CERTIFICATE AND SIGNATURE
                            </div>
                        </div>
                        <!-- Measurement Statement Line -->
                        <div style="padding: 15px 10px; font-size: 12px; font-family: Arial, sans-serif; border: 1px solid #000; margin-top: 20px;">
                          The measurement were made by 
                          <strong><%= GetSubEngineerUserName(txtWorkCode.Text.Trim()) %></strong> S/E  
                          and recorded on Date 
                          <span style="display:inline-block; border-bottom:1px solid #000; min-width:100px;">&nbsp;</span> 
                          to 
                          <span style="display:inline-block; border-bottom:1px solid #000; min-width:100px;">&nbsp;</span> 
                          of eMB Book No. 
                          <strong><%= GetEMBBookNumber(txtWorkCode.Text.Trim()) %></strong>  
                          and advance payment has been previously made without detailed measurements.
                        </div>

                        <!-- Signature & Authorization Section -->
                        <div style="display:flex; justify-content:space-between; margin-top:30px; font-family:Arial, sans-serif; font-size:12px;">

                          <!-- LEFT SIDE -->
                          <div style="width:45%;">
                            <!-- Officer preparing the bill -->
                            <div style="border:1px solid #000; margin-left:10px; height:80px; width:100%;"></div>
                            <div style="margin-top:4px; margin-bottom:10px; text-align:center;">
                              Signature of officer preparing the bill
                            </div>
                            <div style="margin-bottom:20px; margin-left:10px;">
                              Date: <span style="border-bottom:1px solid #000; display:inline-block; width:120px;">&nbsp;</span>
                            </div>

                            <!-- Officer authorising the bill -->
                            <div style="border:1px solid #000; margin-left:10px; height:80px; width:100%;"></div>
                            <div style="margin-top:4px; margin-bottom:10px; text-align:center;">
                              Signature of officer authorising the bill
                            </div>
                            <div style="margin-left:10px; margin-bottom:10px;">
                              Date: <span style="border-bottom:1px solid #000; display:inline-block; width:120px;">&nbsp;</span>
                            </div>
                          </div>

                          <!-- RIGHT SIDE -->
                          <div style="width:45%; text-align:right;">
                            <div style="margin-bottom:8px; margin-right:10px">
                              <strong>Rank - Sub Divisional Officer</strong>
                            </div>
                            <div style="margin-bottom:6px;">
                              <div class="gb-form-field" style="margin-bottom:5px;">
                                <label style="width:auto; ">Sub-Division:</label>
                                <asp:TextBox ID="txtSubDivision2" runat="server" ReadOnly="true" style="margin-right:10px" />
                              </div>
                            </div>
                            <div style="margin-bottom:20px;">
                              <div class="gb-form-field" style="margin-bottom:5px;">
                                <label style="width:auto; ">Division:</label>
                                <asp:TextBox ID="txtDivision2" runat="server" ReadOnly="true" style="margin-right:10px" />
                              </div>
                            </div>
                            <div>
                              Payment Date: <span style="border-bottom:1px solid #000; margin-right:10px; display:inline-block; width:120px;">&nbsp;</span>
                            </div>
                          </div>

                        </div>

                    </div>
    
                    <div class="gb-modal-buttons">
                        <asp:Button ID="btnTableBack" runat="server" Text="← Back" OnClick="GoToIntro" />
                        <asp:Button ID="btnTableNext" runat="server" Text="Next → Memo" OnClick="GoToMemo" />
                    </div>
                </asp:View>

                <!-- View 2: Memorandum of Payment -->
                <asp:View ID="viewMemo" runat="server">
                    <style>
.gb-memo-input {
                            background-color: #f5f5f5 !important;
                            border: none !important;
                            border-bottom: 1px solid #000 !important;
                            outline: none;
                        }
.gb-memo-input:focus {
                            border-bottom: 2px solid #0066cc !important;
                        }
                    </style>
    
                    <div style="max-width: 800px; margin: 0 auto; border: 2px solid #000; font-family: Arial, sans-serif; font-size: 11px; background: white; page-break-inside: avoid;">
                        <!-- Header Section -->
                        <div style="text-align: center; padding: 8px; border-bottom: 2px solid #000; background: #f8f8f8; font-weight: bold; font-size: 13px; text-transform: uppercase;">
                            III - MEMORANDUM OF PAYMENT
                        </div>

                        <!-- Main Memorandum Table -->
                        <table style="width: 100%; border-collapse: collapse; font-size: 10px;">
                            <thead>
                                <tr style="background: #e8e8e8; font-weight: bold;">
                                    <th style="border: 1px solid #000; padding: 6px; text-align: center; width: 70%; text-transform: uppercase; font-weight: bold;">
                                        FIGURES OF MEMORANDUM
                                    </th>
                                    <th style="border: 1px solid #000; padding: 6px; text-align: center; width: 30%; text-transform: uppercase; font-weight: bold;">
                                        RS.
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <!-- Row 1: Total Value -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 6px; font-weight: bold;">
                                        1. Total Value of work done as per account I, Column 5 entry [A]
                                    </td>
                                    <td style="border: 1px solid #000; padding: 6px; text-align: center;">
                                        <asp:Label ID="lblMemoTotalValue" runat="server" Font-Bold="true" />
                                    </td>
                                </tr>
    
                                <!-- Row 2: Deduct amount without -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 6px; font-weight: bold;">
                                        2. Deduct amount with -
                                    </td>
                                    <td style="border: 1px solid #000; padding: 6px; text-align: right;">
                                    </td>
                                </tr>
    
                                <!-- Sub-row 2a: From previous bill -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 25px;">
                                        a) From previous Running Bill
                                    </td>
                                    <td style="border: 1px solid #000; padding: 6px; text-align: center;">
                                        <asp:Label ID="txtPreviousBillAmount" runat="server" Font-Bold="true" />
                                    </td>
                                </tr>
    
                                <!-- Sub-row 2b: From this Bill -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 25px;">
                                        b) From this Bill
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="txtThisBillAmount" runat="server" Font-Bold="true" />
                                    </td>
                                </tr>

                                <!-- Total 2 [3] 4, 5 (c) G -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 6px; text-align: right; font-weight: bold;">
                                        Total 2(a) + 2(b)
                                    </td>
                                    <td style="border: 1px solid #000; padding: 6px; text-align: center; font-weight: bold;">
                                        <asp:Label ID="lblTotal2" runat="server" Font-Bold="true" />
                                    </td>
                                </tr>
    
                                <!-- Row 3: Balance -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 6px; font-weight: bold;">
                                        3. Balance i.e. up to date payment (Item 1-2)
                                    </td>
                                    <td style="border: 1px solid #000; padding: 6px; text-align: center; font-weight: bold;">
                                        <asp:Label ID="lblBalance" runat="server" Font-Bold="true" />
                                    </td>
                                </tr>
    
                                <!-- Row 4: Total account of payment -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 6px; font-weight: bold;">
                                        4. Total amount of work done for Work Code 
                                        <asp:Label ID="lblWorkCode" Cssclass="gb-memo-input" runat="server" Font-Bold="true" />
                                    </td>
                                    <td style="border: 1px solid #000; padding: 6px; text-align: center;">
                                        <asp:Label ID="lblTotalWorkDone" runat="server" Font-Bold="true"
                                          Text='<%# ((GetOverallTotalAmountUpToDate() * (100 + GetSORPercentage()) / 100).ToString("N2")) %>' />
                                    </td>
                                </tr>
    
                                <!-- Row 5: Payment not to be made -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 6px; font-weight: bold;">
                                        5. Payment now to be made as detailed below:
                                    </td>
                                    <td style="border: 1px solid #000; padding: 6px; text-align: right;">
                                    </td>
                                </tr>
    
                                <!-- Sub-row 5a: By recovery -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 25px;">
                                        a) By recovery of the amount creditable to this work
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="txtRecoveryAmount" runat="server"/>
                                    </td>
                                </tr>
    
                                <!-- Sub-row 5b: by recovery of amounts -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 4px; padding-left: 25px;">
                                        b) By other deductions
                                    </td>
                                </tr>

                                <!-- Sub-row 5b1: Security Deposit -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 50px;">
                                        1) Security Deposit
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="txtSecurityDeposit" runat="server"/>
                                    </td>
                                </tr>

                                <!-- Sub-row 5b2: Income Tax -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 50px;">
                                        2) Income Tax
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="txtIncomeTax" runat="server"/>
                                    </td>
                                </tr>

                                <!-- Sub-row 5b3: GST Amount -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 50px;">
                                        3) GST Amount
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="txtGSTAmount" runat="server"/>
                                    </td>
                                </tr>

                                <!-- Sub-row 5b4: Labour Welfare -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 50px;">
                                        4) Labour Welfare
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="txtLabourWelfare" runat="server"/>
                                    </td>
                                </tr>

                                <!-- Sub-row 5b5: Time Extension (Withheld) -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 50px;">
                                        5) Time Extension (Withheld)
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="txtTimeExtensionWithheld" runat="server"/>
                                    </td>
                                </tr>

                                <!-- Sub-row 5b6: Royalty -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 50px;">
                                        6) Royalty
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="txtRoyalty" runat="server"/>
                                    </td>
                                </tr>

                                <!-- Sub-row 5b7: Miscellaneous Deposit -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 50px;">
                                        7) Miscellaneous Deposit
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="txtMiscellaneousDeposit" runat="server"/>
                                    </td>
                                </tr>

                                <!-- Sub-row 5b8: Penalty for Time Extension -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 50px;">
                                        8) Penalty for Time Extension
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="txtPenaltyForTimeExtension" runat="server"/>
                                    </td>
                                </tr>

                                <!-- Sub-row 5b9: Penalty for Work -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 50px;">
                                        9) Penalty for Work
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="txtPenaltyForWork" runat="server"/>
                                    </td>
                                </tr>

                                <!-- Sub-row 5b10: Cost of Bill Form -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 50px;">
                                        10) Cost of Bill Form
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="txtCostOfBillForm" runat="server"/>
                                    </td>
                                </tr>

                                <!-- Sub-row 5c: By transfer -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 2px; padding-left: 25px;">
                                        c) By Transfer
                                    </td>
                                    <td style="border: 1px solid #000; padding: 2px; text-align: center;">
                                        <asp:Label ID="lblByTransfer" runat="server" Font-Bold="true" />
                                    </td>
                                </tr>
    
                                <!-- Total 5 [3] 4 (c) H -->
                                <tr>
                                    <td style="border: 1px solid #000; padding: 6px; text-align: right; font-weight: bold;">
                                        Total 5(a) + 5(b) + 5(c)
                                    </td>
                                    <td style="border: 1px solid #000; padding: 6px; text-align: center; font-weight: bold;">
                                        <asp:Label ID="lblTotal5" runat="server" Font-Bold="true" />
                                    </td>
                                </tr>

                            </tbody>
                        </table>

                        <!-- Payment Section -->
                        <div style="border-top: 2px solid #000; padding: 12px; background: #f9f9f9;">
                            <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 15px;">
                                <div style="font-weight: bold; flex: 1;">
                                    Pay Total By Transfer Rs. 
                                    <asp:Label ID="txtPayAmount" runat="server" 
                                        style="font-weight: bold; color: #333; background-color: #f0f0f0; padding: 4px 8px; border-radius: 4px;" />
                                </div>
                                <div style="text-align: right;">
                                    <div style="border: 1px solid #000; padding: 8px; width: 120px; height: 40px; display: flex; align-items: center; justify-content: center; margin-bottom: 8px; background: white;">
                                        <!-- Officer Box -->
                                    </div>
                                    <div style="font-size: 9px; text-align: center;">
                                        Dated Initials of Disbursing Officer
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Bottom Section -->
                        <div style="border-top: 1px solid #000; padding: 12px; background: white;">
                            <div style="margin-bottom: 12px; font-size: 14px;">
                                Received Rs <asp:Label ID="txtReceivedAmount" runat="server" style="font-weight: bold; color: #333; background-color: #f0f0f0; padding: 4px 8px; border-radius: 4px;" /> 
                                as per above memorandum on account of this work in full settlement of all demands.
                            </div>

                            <div style="display: flex; justify-content: space-between; align-items: flex-start; margin-top: 20px;">
                                <div style="flex: 1;">
                                    <div style="margin-bottom: 15px;">
                                        Date: <asp:TextBox ID="txtReceiptDate" runat="server" placeholder="DD/MM/YYYY" Cssclass="gb-memo-input" style="width: 100px;" />
                                    </div>
    
                                    <!-- Thumb Impression Box -->
                                    <div style="margin-bottom: 8px;">
                                        <div style="border: 1px solid #000; width: 80px; height: 60px; display: inline-block; margin-bottom: 5px; background: white;"></div>
                                    </div>
                                    <div style="font-size: 9px; margin-bottom: 20px;">
                                        Left Hand Thumb Impression
                                    </div>
    
                                    <!-- Contractor Signature Box -->
                                    <div style="margin-bottom: 8px;">
                                        <div style="border: 1px solid #000; width: 150px; height: 40px; display: inline-block; margin-bottom: 5px; background: white;"></div>
                                    </div>
                                    <div style="font-size: 9px; text-align: center; width: 150px;">
                                        Full Signature of Contractor
                                    </div>
                                </div>

                                <div style="text-align: right;">
                                    <div style="border: 1px solid #000; padding: 15px; width: 100px; height: 60px; display: flex; align-items: center; justify-content: center; font-weight: bold; background: white;">
                                        STAMP
                                    </div>
                                    <div style="display: flex; justify-content: space-between; margin-top: 10px;">
                                        <div style="flex: 1;">
                                        </div>
                                        <div style="text-align: right;">
                                            <div style="margin-bottom: 8px; margin-right:20px">
                                                Dated: <asp:TextBox ID="txtPaidDate" runat="server" placeholder="DD/MM/YYYY" Cssclass="gb-memo-input" style="width: 100px;" />
                                            </div>
                                            <div style="margin-right:20px">
                                                Cashier: <asp:TextBox ID="txtCashier" runat="server" Cssclass="gb-memo-input" style="width: 200px;" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Moved up the Dated and Cashier fields -->

                            <div style="margin-top: 20px; border-top: 1px solid #000; padding-top: 8px;">
                                <div style="display: flex; justify-content: space-between; align-items: flex-start;">
                                    <div>
                                        Vide Cheque No.: <asp:TextBox ID="txtChequeNo" runat="server" Cssclass="gb-memo-input" style="width: 250px;" />
                                    </div>
                                    <div style="text-align: right;">
                                        <div style="border: 1px solid #000; width: 180px; height: 30px; display: inline-block; margin-bottom: 5px; background: white;"></div>
                                        <div style="font-size: 9px; text-align: center; width: 180px;">
                                            Dated initials of person actually making the payment
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Validation Error Display -->
                    <div id="validationErrors" style="color: red; font-weight: bold; margin: 10px 0; display: none;"></div>

                    <div class="gb-modal-buttons">
                        <asp:Button ID="btnMemoBack" runat="server" Text="← Back" OnClick="GoToDetails" />
                        <asp:Button ID="btnMemoSave" runat="server" Text="Save Bill" OnClick="FinishAndSave" OnClientClick="return validateNumericFields();" />
                    </div>

                    <script type="text/javascript">
                        function validateNumericFields() {
                            var isValid = true;
                            var errorMessages = [];

                            // Get all numeric fields
                            var numericFields = document.querySelectorAll('.numeric-field');

                            // Regular expression to match valid numbers (including decimals)
                            var numberRegex = /^\d*\.?\d*$/;

                            numericFields.forEach(function (field) {
                                var value = field.value.trim();

                                // Skip validation if field is empty (assuming optional fields)
                                if (value === '') return;

                                // Check if the value contains only numbers and decimal points
                                if (!numberRegex.test(value)) {
                                    isValid = false;
                                    field.style.backgroundColor = '#ffebee';
                                    field.style.border = '1px solid #f44336';

                                    // Get field label for error message
                                    var fieldName = field.id.replace('txt', '').replace(/([A-Z])/g, ' $1').trim();
                                    errorMessages.push(fieldName + ' must contain only numbers');
                                } else {
                                    // Reset field styling if valid
                                    field.style.backgroundColor = 'transparent';
                                    field.style.border = 'none';
                                    field.style.borderBottom = '1px solid #000';
                                }
                            });

                            // Display validation errors
                            var errorDiv = document.getElementById('validationErrors');
                            if (!isValid) {
                                errorDiv.innerHTML = errorMessages.join('<br/>');
                                errorDiv.style.display = 'block';

                                // Scroll to first invalid field
                                var firstInvalidField = document.querySelector('.numeric-field[style*="background-color: rgb(255, 235, 238)"]');
                                if (firstInvalidField) {
                                    firstInvalidField.focus();
                                }
                            } else {
                                errorDiv.style.display = 'none';
                            }

                            return isValid;
                        }

                        // Add real-time validation on blur
                        document.addEventListener('DOMContentLoaded', function () {
                            var numericFields = document.querySelectorAll('.numeric-field');
                            var numberRegex = /^\d*\.?\d*$/;

                            numericFields.forEach(function (field) {
                                field.addEventListener('blur', function () {
                                    var value = this.value.trim();

                                    if (value !== '' && !numberRegex.test(value)) {
                                        this.style.backgroundColor = '#ffebee';
                                        this.style.border = '1px solid #f44336';
                                    } else {
                                        this.style.backgroundColor = 'transparent';
                                        this.style.border = 'none';
                                        this.style.borderBottom = '1px solid #000';
                                    }
                                });
                            });
                        });
                    </script>
                </asp:View>
            </asp:MultiView>
        </asp:Panel>
</asp:Content>