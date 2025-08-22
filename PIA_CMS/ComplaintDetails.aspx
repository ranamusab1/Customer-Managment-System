<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ComplaintDetails.aspx.cs" Inherits="PIA_CMS.ComplaintDetails" MasterPageFile="~/Site.master" %>

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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <asp:Label ID="lblMsg" runat="server" CssClass="alert alert-danger" Visible="False" />
        <asp:HiddenField ID="hfComplaintID" runat="server" />
        <div class="card">
            <div class="card-header">
                Complaint Information
            </div>
            <div class="card-body">
                <asp:Label ID="lblComplaintDetails" runat="server" />
            </div>
        </div>
        <asp:Button ID="btnReply" runat="server" Text="Reply" CssClass="btn btn-primary mt-3" OnClick="btnReply_Click" />
        <asp:Button ID="btnForward" runat="server" Text="Forward" CssClass="btn btn-info mt-3" OnClick="btnForward_Click" />
    </div>
</asp:Content>