<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReceiveEmail.aspx.cs" Inherits="PIA_CMS.ReceiveEmail" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .alert {
            margin-top: 20px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <asp:Label ID="lblMsg" runat="server" CssClass="alert alert-danger" Visible="False" />
    </div>
</asp:Content>