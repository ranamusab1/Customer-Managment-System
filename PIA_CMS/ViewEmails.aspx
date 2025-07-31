<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewEmails.aspx.cs" Inherits="PIA_CMS.ViewEmails" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .modal-body {
            max-height: 500px;
            overflow-y: auto;
        }
        .bg-darkgreen {
            background-color: darkgreen;
            color: white;
        }
        h2 {
            color: #004d40;
            font-weight: 600;
        }
        .btn {
            transition: background-color 0.3s ease-in-out, color 0.3s ease-in-out, border-color 0.3s ease-in-out;
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
        <h2>View Sent Emails</h2>

        <!-- Filter -->
        <div class="row mb-3">
            <div class="col-md-4">
                <asp:DropDownList ID="ddlFilterBy" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Select Filter" Value="" />
                    <asp:ListItem Text="Date" Value="SentDate" />
                    <asp:ListItem Text="Subject" Value="EmailSubject" />
                    <asp:ListItem Text="User ID" Value="UserID" />
                    <asp:ListItem Text="Ref No" Value="RefNo" />
                    <asp:ListItem Text="Sent To" Value="EmailTo" />
                    <asp:ListItem Text="Membership No" Value="MembershipNo" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <asp:TextBox ID="txtFilter" runat="server" CssClass="form-control" placeholder="Enter filter value..." />
            </div>
            <div class="col-md-4">
                <asp:Button ID="btnSearch" runat="server" Text="Go" CssClass="btn" OnClick="btnSearch_Click" />
            </div>
        </div>

        <!-- Grid -->
        <asp:GridView ID="gvEmails" runat="server" CssClass="table table-striped table-bordered" AutoGenerateColumns="False" OnRowCommand="gvEmails_RowCommand">
            <Columns>
                <asp:BoundField DataField="EmailID" HeaderText="S.No" />
                <asp:BoundField DataField="UserID" HeaderText="User ID" />
                <asp:BoundField DataField="EmailFrom" HeaderText="From" />
                <asp:BoundField DataField="EmailTo" HeaderText="To" />
                <asp:BoundField DataField="EmailSubject" HeaderText="Subject" />
                <asp:BoundField DataField="SentDate" HeaderText="Date Sent" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:Button ID="btnViewDetails" runat="server" Text="View" CssClass="btn btn-sm" CommandName="ViewDetails" CommandArgument='<%# Eval("EmailID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>