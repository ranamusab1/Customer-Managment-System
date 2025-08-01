<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailDetails.aspx.cs" Inherits="PIA_CMS.EmailDetails" MasterPageFile="~/Site.master" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Email Details</title>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <style>
        .card {
            border: 1px solid #ddd;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .card-header {
            background-color: #f8f9fa;
            border-bottom: 1px solid #ddd;
            font-weight: 600;
        }
        .card-body {
            padding: 20px;
        }
        .alert {
            margin-top: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <asp:Label ID="lblMsg" runat="server" CssClass="alert alert-danger" Visible="False" />
            <asp:HiddenField ID="hfEmailID" runat="server" />
            <div class="card">
                <div class="card-header">
                    Email Information
                </div>
                <div class="card-body">
                    <asp:Label ID="lblEmailDetails" runat="server" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>