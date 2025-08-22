<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReplyComplaint.aspx.cs" Inherits="PIA_CMS.ReplyComplaint" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .bg-darkgreen {
            background-color: #004225;
            color: white;
        }
        h2 {
            color: #004d40;
            font-weight: 600;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2>Reply to Complaint</h2>
        <asp:Label ID="lblMsg" runat="server" CssClass="alert alert-danger" Visible="False" />
        <div class="form-group">
            <label>To:</label>
            <asp:TextBox ID="txtReplyEmail" runat="server" CssClass="form-control" ReadOnly="true" />
        </div>
        <div class="form-group">
            <label>Subject:</label>
            <asp:TextBox ID="txtReplySubject" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group">
            <label>Body:</label>
            <asp:TextBox ID="txtReplyBody" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="6" />
        </div>
        <asp:Button ID="btnReplySend" runat="server" Text="Send Reply" CssClass="btn btn-primary" OnClick="btnReplySend_Click" />
    </div>
</asp:Content>