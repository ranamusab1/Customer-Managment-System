<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="PIA_CMS.Home" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .container-flex {
            background: linear-gradient(to right, rgb(128 128 128 / 0.71), rgb(128 128 128 / 0.71)),
                        url('piabanner.jpg') no-repeat center center;
            background-size: cover;
            min-height: calc(100vh - 100px);
            display: flex;
            justify-content: center;
            align-items: center;
            color: white;
            text-align: center;
        }
        h2 {
            color: #004d40;
            font-weight: 600;
        }
        p {
            color: #666;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-flex">
        <h2>Welcome to the Admin Panel</h2>
    </div>
</asp:Content>