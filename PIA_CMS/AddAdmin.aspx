<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddAdmin.aspx.cs" Inherits="PIA_CMS.AddAdmin" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
        .btn-primary {
            background-color: #004225 !important;
            border-color: #004225 !important;
            color: white !important;
        }
        .btn-primary:hover, .btn-warning:hover {
            background-color: white !important;
            color: #004225 !important;
            border-color: #004225 !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h4>Add New Admin</h4>
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger" Visible="false"></asp:Label>
        <div class="form-group">
            <label for="txtUsername">Username</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Enter username"></asp:TextBox>
        </div>
        <div class="form-group">
            <label for="txtName">Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter full name"></asp:TextBox>
        </div>
        <div class="form-group">
            <label for="txtEmail">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter email" TextMode="Email"></asp:TextBox>
        </div>
        <div class="form-group">
            <label for="txtPassword">Password</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Enter password" TextMode="Password"></asp:TextBox>
        </div>
        <div class="form-group">
            <label for="ddlRole">Role</label>
            <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control select2">
                <asp:ListItem Text="Select Role" Value="" />
                <asp:ListItem Text="SuperAdmin" Value="SuperAdmin" />
                <asp:ListItem Text="Admin" Value="Admin" />
                <asp:ListItem Text="User" Value="User" />
            </asp:DropDownList>
        </div>
        <asp:Button ID="btnAddAdmin" runat="server" Text="Add Admin" CssClass="btn btn-primary" OnClick="btnAddAdmin_Click" />
    </div>
</asp:Content>