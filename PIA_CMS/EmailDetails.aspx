<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailDetails.aspx.cs" Inherits="PIA_CMS.EmailDetails" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .bg-darkgreen {
            background-color: #004225;
            color: white;
        }
        .btn {
            transition: background-color 0.3s ease-in-out, color 0.3s ease-in-out, border-color 0.3s ease-in-out;
            margin-right: 10px;
        }
        .btn {
            background-color: darkgreen !important;
            color: white !important;
            border-color: darkgreen !important;
        }
        .btn:hover {
            background-color: white !important;
            color: darkgreen !important;
            border-color: darkgreen !important;
        }
        h2 {
            color: #004d40;
            font-weight: 600;
        }
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
        .card-footer {
            background-color: #f8f9fa;
            border-top: 1px solid #ddd;
        }
        .alert {
            margin-top: 20px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2>Email Details</h2>
        <asp:Label ID="lblMsg" runat="server" CssClass="alert alert-danger" Visible="False" />
        <asp:HiddenField ID="hfEmailID" runat="server" />
        <div class="card">
            <div class="card-header bg-darkgreen">
                Email Information
            </div>
            <div class="card-body">
                <asp:Label ID="lblEmailDetails" runat="server" />
            </div>
            <div class="card-footer">
                <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn" OnClick="btnBack_Click" />
            </div>
        </div>
    </div>
</asp:Content>