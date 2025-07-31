<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendEmail.aspx.cs" Inherits="PIA_CMS.SendEmail" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        h2 {
            color: #004d40;
            font-weight: 600;
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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2>Send Email</h2>
        <asp:Label ID="lblMsg" runat="server" ForeColor="Green" />
        <div class="form-group">
            <label>Membership ID</label>
            <asp:TextBox ID="txtMembershipId" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group">
            <label>To (Email Address)</label>
            <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" TextMode="Email" />
        </div>
        <div class="form-group">
            <label>BCC (Optional)</label>
            <asp:TextBox ID="txtBCC" runat="server" CssClass="form-control" TextMode="Email" />
        </div>
        <div class="form-group">
            <label>Subject</label>
            <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group">
            <label>Body</label>
            <asp:TextBox ID="txtBody" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6" />
        </div>
        <asp:Button ID="btnSend" runat="server" Text="Send Email" CssClass="btn btn-success" OnClick="btnSend_Click" />
    </div>
</asp:Content>