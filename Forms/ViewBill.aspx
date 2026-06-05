<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewBill.aspx.cs" Inherits="ViewBill" %>
<!DOCTYPE html>
<html>
<head>
    <title>Bill View</title>
    <style>
        /* Include all styles from GenerateBill.aspx */
        .print-container { 
            width: 210mm; 
            margin: 0 auto; 
            padding: 10mm; 
            box-sizing: border-box;
            font-family: Arial, sans-serif;
            font-size: 12px;
        }
        @media print {
            body { margin: 0; }
            .no-print { display: none; }
            .page-break { 
                page-break-after: always;
                margin-top: 10mm;
            }
            .form-container { 
                border: 2px solid #000; 
                margin-bottom: 10mm;
            }
        }
        table { width: 100%; border-collapse: collapse; }
        td, th { padding: 8px; border: 1px solid #ccc; }
        
        /* Bill table styles */
        .bill-table { 
            width: 100%; 
            border-collapse: collapse; 
            margin-bottom: 10px;
        }
        .bill-table th, .bill-table td { 
            border: 1px solid #ddd; 
            padding: 8px; 
            text-align: left;
        }
        .component-header { 
            background-color: #f2f2f2; 
            font-weight: bold; 
        }
        .total { 
            font-weight: bold; 
            background-color: #d4d4d4; 
            font-size: 1.1em; 
            padding: 10px;
            margin: 10px 0;
        }
        
        /* FIXED Modal styles */
        .modalBackground {
            position: fixed; 
            top: 0; 
            left: 0; 
            width: 100%; 
            height: 100%;
            background-color: rgba(0, 0, 0, 0.7);
            display: none;
            z-index: 1000;
        }
        
        .modalPopup {
            position: fixed; 
            top: 5%; 
            left: 50%; 
            transform: translateX(-50%);
            background-color: #fff; 
            border: 1px solid #888; 
            padding: 20px;
            width: 90%; 
            max-width: 1200px;
            height: 85%; 
            max-height: 85vh;
            overflow-y: auto;
            overflow-x: hidden;
            display: none;
            z-index: 1001;
            box-shadow: 0 4px 8px rgba(0,0,0,0.3);
            border-radius: 5px;
        }
        
        .closeModal { 
            position: absolute; 
            top: 10px; 
            right: 15px; 
            font-size: 20px; 
            cursor: pointer; 
            color: #666;
            font-weight: bold;
        }
        
        .closeModal:hover {
            color: #000;
        }
        
        /* Button styles within modal */
        .modal-buttons {
            margin-top: 20px;
            padding-top: 15px;
            border-top: 1px solid #eee;
            text-align: center;
        }
        
        .modal-buttons input[type="submit"] {
            margin: 0 10px;
            padding: 8px 20px;
        }
        
        /* Table container for better scrolling */
        .table-container {
            border: 1px solid #ddd;
            margin: 10px 0;
        }
        
        /* Ensure content stays within modal */
        .modalPopup * {
            box-sizing: border-box;
        }
        
        /* Form elements within modal */
        .modalPopup input[type="text"], 
        .modalPopup textarea {
            max-width: 100%;
        }
        .form-container {
            max-width: 800px;
            margin: 20px auto;
            border: 2px solid #000;
            padding: 0;
            font-family: Arial, sans-serif;
            font-size: 12px;
            background: white;
        }

        .form-header {
            display: flex;
            justify-content: space-between;
            padding: 10px 15px;
            border-bottom: 1px solid #000;
            background: #f8f8f8;
        }

        .form-header-left {
            font-weight: bold;
        }

        .form-header-right {
            text-align: right;
            font-weight: bold;
        }

        .form-body {
            display: flex;
            min-height: 600px;
        }

        .left-column {
            width: 45%;
            padding: 15px;
            border-right: 1px solid #000;
            background: #f9f9f9;
        }

        .right-column {
            width: 55%;
            padding: 15px;
        }

        .notes-title {
            font-weight: bold;
            font-size: 14px;
            margin-bottom: 10px;
            text-decoration: underline;
        }

        .notes-subtitle {
            font-size: 11px;
            margin-bottom: 15px;
        }

        .note-item {
            margin-bottom: 15px;
            line-height: 1.4;
        }

        .note-number {
            font-weight: bold;
            margin-right: 5px;
        }

        .highlight-blue {
            color: #0066cc;
            font-weight: bold;
        }

        .right-header {
            text-align: right;
            margin-bottom: 20px;
        }

        .division-fields .form-field {
            margin-bottom: 5px;
            display: flex;
            align-items: center;
            border-bottom: 1px solid #000;
            padding-bottom: 2px;
        }

        .division-fields .form-field label {
            min-width: auto;
            margin-right: 5px;
            font-weight: bold;
        }

        .division-line {
            border-bottom: 1px solid #000;
            margin-bottom: 5px;
            padding-bottom: 2px;
            min-height: 20px;
        }

        .form-field {
            margin-bottom: 15px;
            display: flex;
            align-items: center;
        }

        .form-field label {
            margin-right: 10px;
            min-width: 120px;
            font-weight: bold;
        }

        .form-field input, .form-field textarea {
            flex: 1;
            padding: 5px;
            border: 1px solid #666;
            font-size: 12px;
        }

        .cash-voucher {
            display: flex;
            justify-content: space-between;
            margin-bottom: 15px;
        }

        .workname-box {
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

        .cash-voucher input {
            width: 60px;
            text-align: center;
            border: none;
            border-bottom: 1px solid #000;
            background: transparent;
        }

        .running-account {
            text-align: center;
            font-weight: bold;
            margin-bottom: 20px;
            font-size: 13px;
        }

        .contractor-field {
            margin-bottom: 30px;
        }

        .underline-input {
            border: none !important;
            border-bottom: 1px solid #000 !important;
            background: transparent !important;
            padding: 5px 0 !important;
            width: 100% !important;
            resize: none;
            white-space: pre-wrap;
            overflow-wrap: break-word;
        }

        .reference-table {
            margin-bottom: 20px;
        }

        .reference-row {
            display: flex;
            margin-bottom: 10px;
            align-items: center;
        }

        .reference-row label {
            min-width: 100px;
            font-weight: bold;
        }

        .completion-section {
            margin-bottom: 20px;
        }

        .completion-item {
            margin-bottom: 8px;
            padding-left: 15px;
            position: relative;
        }

        .completion-item:before {
            content: "• ";
            position: absolute;
            left: 0;
            font-weight: bold;
        }
        
        /* Responsive adjustments */
        @media (max-width: 768px) {
            .modalPopup {
                width: 95%;
                height: 90%;
                top: 5%;
                padding: 15px;
            }
        }
        .bill-table {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 10px;
        }
        .bill-table th, .bill-table td {
            border: 1px solid #000;
            padding: 5px;
            text-align: left;
        }
        .total-row {
            font-weight: bold;
            background-color: #eee;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="print-container">
            <div class="no-print" style="text-align:center; margin:20px;">
                <button onclick="window.print()">Print Bill</button>
                <button onclick="window.close()">Close</button>
            </div>
            <!-- Intro Section -->
            <div id="divIntro" runat="server" class="form-container page-break"></div>
            
            <!-- Details Section -->
            <div id="divDetails" runat="server" class="page-break"></div>
            
            <!-- Memo Section -->
            <div id="divMemo" runat="server" class="page-break"></div>
            
            <div class="no-print" style="text-align:center; margin:20px;">
                <button onclick="window.print()">Print Bill</button>
                <button onclick="window.close()">Close</button>
            </div>
        </div>
    </form>
</body>
</html>