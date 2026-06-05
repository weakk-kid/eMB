<%@ Page Title="" Language="C#" MasterPageFile="../MasterPages/MasterPage.master" AutoEventWireup="true" CodeFile="Approval.aspx.cs" Inherits="PHEDChhattisgarh.ApprovalProcess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    
    <!-- Bootstrap CSS -->
    <%--<link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/4.6.2/css/bootstrap.min.css" rel="stylesheet" />--%>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" rel="stylesheet" />
    
    <!-- SweetAlert2 -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/sweetalert2/11.7.12/sweetalert2.min.js"></script>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/sweetalert2/11.7.12/sweetalert2.min.css" rel="stylesheet" />
    
    <style>
        .approval-container {
            padding: 20px;
            background-color: #f8f9fa;
            min-height: 100vh;
        }
        
        .approval-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            border-radius: 10px;
            margin-bottom: 20px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }
        
        .approval-card {
            background: white;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 15px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            border-left: 4px solid #28a745;
            transition: transform 0.2s, box-shadow 0.2s;
        }
        
        .approval-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
        }
        
        .book-info {
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
        }
        
        .book-details {
            flex: 1;
        }
        
        .book-id {
            font-size: 1.2em;
            font-weight: bold;
            color: #2c3e50;
            margin-bottom: 5px;
        }
        
        .work-code {
            color: #6c757d;
            font-size: 0.9em;
            margin-bottom: 10px;
        }
        
        .approval-info {
            display: flex;
            gap: 15px;
            margin-top: 10px;
            flex-wrap: wrap;
        }
        
        .approval-badge {
            padding: 5px 10px;
            border-radius: 20px;
            font-size: 0.8em;
            font-weight: 500;
        }
        
        .approved {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        
        .pending {
            background-color: #fff3cd;
            color: #856404;
            border: 1px solid #ffeaa7;
        }
        
        .action-buttons {
            display: flex;
            gap: 10px;
            margin-top: 15px;
        }
        
        .btn-approve {
            background: linear-gradient(135deg, #28a745, #20c997);
            border: none;
            color: white;
            padding: 8px 20px;
            border-radius: 5px;
            font-weight: 500;
            transition: all 0.2s;
        }
        
        .btn-approve:hover {
            background: linear-gradient(135deg, #218838, #1ea085);
            transform: translateY(-1px);
            color: white;
        }
        
        .btn-view {
            background: linear-gradient(135deg, #007bff, #6f42c1);
            border: none;
            color: white;
            padding: 8px 20px;
            border-radius: 5px;
            font-weight: 500;
            transition: all 0.2s;
        }
        
        .btn-view:hover {
            background: linear-gradient(135deg, #0056b3, #5a2d91);
            transform: translateY(-1px);
            color: white;
        }
        
        .no-records {
            text-align: center;
            padding: 40px;
            color: #6c757d;
        }
        
        .no-records i {
            font-size: 3em;
            margin-bottom: 15px;
            color: #dee2e6;
        }
        
        .stats-container {
            display: flex;
            gap: 15px;
            margin-bottom: 20px;
            flex-wrap: wrap;
        }
        
        .stat-card {
            background: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            flex: 1;
            min-width: 200px;
            text-align: center;
        }
        
        .stat-number {
            font-size: 2em;
            font-weight: bold;
            color: #667eea;
        }
        
        .stat-label {
            color: #6c757d;
            font-size: 0.9em;
            margin-top: 5px;
        }
        
        @media (max-width: 768px) {
            .book-info {
                flex-direction: column;
                align-items: flex-start;
            }
            
            .action-buttons {
                width: 100%;
                justify-content: flex-start;
            }
            
            .stats-container {
                flex-direction: column;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TitleContent" Runat="Server">
eMB Approval Page
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
        <div class="approval-container">
            <!-- Header -->
            <div class="approval-header">
                <div class="d-flex justify-content-between align-items-center">
                    <div>
                        <h2><i class="fas fa-check-circle"></i> <asp:Label ID="lblPageTitle" runat="server" Text="Approval Process"></asp:Label></h2>
                        <p class="mb-0">Review and approve pending eMB Books</p>
                    </div>
                    <div class="text-right">
                        <small>Welcome, <asp:Label ID="lblUserName" runat="server"></asp:Label></small><br />
                        <small><asp:Label ID="lblCurrentDate" runat="server"></asp:Label></small>
                    </div>
                </div>
            </div>

            <!-- Statistics -->
            <div class="stats-container">
                <div class="stat-card">
                    <div class="stat-number">
                        <asp:Label ID="lblTotalPending" runat="server" Text="0"></asp:Label>
                    </div>
                    <div class="stat-label">Books Pending Approval</div>
                </div>
                <div class="stat-card">
                    <div class="stat-number">
                        <asp:Label ID="lblTotalApproved" runat="server" Text="0"></asp:Label>
                    </div>
                    <div class="stat-label">Books Approved Today</div>
                </div>
            </div>

            <!-- Pending Approvals List -->
            <div class="card">
                <div class="card-header">
                    <h5 class="mb-0">
                        <i class="fas fa-list-alt"></i> Pending Approvals
                        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="btn btn-outline-primary btn-sm float-right" OnClick="btnRefresh_Click" />
                    </h5>
                </div>
                <div class="card-body p-0">
                    <asp:Repeater ID="rptPendingApprovals" runat="server" OnItemCommand="rptPendingApprovals_ItemCommand">
                        <ItemTemplate>
                            <div class="approval-card">
                                <div class="book-info">
                                    <div class="book-details">
                                        <div class="book-id">
                                            <i class="fas fa-book"></i> eMB Book ID: <%# Eval("embbookid") %>
                                        </div>
                                        <div class="work-code">
                                            <i class="fas fa-code"></i> Work Code: <%# Eval("WorkCode") %>
                                        </div>
                                        <div class="approval-info">
                                            <span class="approval-badge <%# Eval("DateSE") != DBNull.Value ? "approved" : "pending" %>">
                                                <i class="fas fa-user"></i> SE: <%# Eval("DateSE") != DBNull.Value ? "Approved" : "Pending" %>
                                                <%# Eval("DateSE") != DBNull.Value ? " (" + Convert.ToDateTime(Eval("DateSE")).ToString("dd/MM/yyyy") + ")" : "" %>
                                            </span>
                                            <span class="approval-badge <%# Eval("DateAE") != DBNull.Value ? "approved" : "pending" %>">
                                                <i class="fas fa-user-tie"></i> AE: <%# Eval("DateAE") != DBNull.Value ? "Approved" : "Pending" %>
                                                <%# Eval("DateAE") != DBNull.Value ? " (" + Convert.ToDateTime(Eval("DateAE")).ToString("dd/MM/yyyy") + ")" : "" %>
                                            </span>
                                            <span class="approval-badge <%# Eval("DateEE") != DBNull.Value ? "approved" : "pending" %>">
                                                <i class="fas fa-user-crown"></i> EE: <%# Eval("DateEE") != DBNull.Value ? "Approved" : "Pending" %>
                                                <%# Eval("DateEE") != DBNull.Value ? " (" + Convert.ToDateTime(Eval("DateEE")).ToString("dd/MM/yyyy") + ")" : "" %>
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div class="action-buttons">
                                    <asp:Button ID="btnView" runat="server" Text="View Bills" CssClass="btn btn-view" 
                                        CommandName="ViewBook" 
                                        CommandArgument='<%# Eval("embbookid") + "," + Eval("WorkCode") %>' />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    
                    <!-- No Records Message -->
                    <asp:Panel ID="pnlNoRecords" runat="server" Visible="false" CssClass="no-records">
                        <i class="fas fa-inbox"></i>
                        <h4>No Pending Approvals</h4>
                        <p>There are no books waiting for your approval at the moment.</p>
                    </asp:Panel>
                </div>
            </div>
        </div>

        <!-- Bootstrap JS -->
        <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/4.6.2/js/bootstrap.bundle.min.js"></script>
</asp:Content>