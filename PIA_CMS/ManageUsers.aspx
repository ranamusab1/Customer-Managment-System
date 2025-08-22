<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="PIA_CMS.ManageUsers" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .card {
            border: 1px solid #ddd;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        .card-header {
            background-color: #004225;
            color: white;
            font-weight: 600;
        }
        .card-body {
            padding: 20px;
        }
        .btn-primary, .btn-warning {
            background-color: #004225 !important;
            border-color: #004225 !important;
            color: white !important;
        }
        .btn-primary:hover, .btn-warning:hover {
            background-color: white !important;
            color: #004225 !important;
            border-color: #004225 !important;
        }
        .alert {
            margin-bottom: 20px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="card">
            <div class="card-header">
                <h4>Manage Users</h4>
            </div>
            <div class="card-body">
                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-info w-100" Visible="false"></asp:Label>
                <asp:GridView ID="gvUsers" runat="server" CssClass="table table-striped table-bordered" AutoGenerateColumns="False" OnRowCommand="gvUsers_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="sno" HeaderText="ID" />
                        <asp:BoundField DataField="user_name" HeaderText="Username" />
                        <asp:BoundField DataField="NameOfEmployee" HeaderText="Name" />
                        <asp:BoundField DataField="EMAIL" HeaderText="Email" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <%# Eval("enable").ToString() == "1" ? "Active" : "Inactive" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:Button ID="btnToggleStatus" runat="server" Text='<%# Eval("enable").ToString() == "1" ? "Deactivate" : "Activate" %>'
                                    CssClass='<%# Eval("enable").ToString() == "1" ? "btn btn-warning btn-sm" : "btn btn-primary btn-sm" %>'
                                    CommandName="ToggleStatus" CommandArgument='<%# Eval("sno") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>