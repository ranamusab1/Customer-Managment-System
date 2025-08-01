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
                <asp:DropDownList ID="ddlFilterBy" runat="server" CssClass="form-control select2">
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
                <asp:Button ID="btnSearch" runat="server" Text="Go" CssClass="btn btn-success" OnClick="btnSearch_Click" />
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
                        <asp:Button ID="btnViewDetails" runat="server" Text="View" CssClass="btn btn-primary btn-sm" CommandName="ViewDetails" CommandArgument='<%# Eval("EmailID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <!-- Modal -->
        <asp:Panel ID="pnlEmailDetails" runat="server" CssClass="modal fade" Style="display: none;" ClientIDMode="Static">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-darkgreen">
                        <h5 class="modal-title">Email Details</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div id="emailDetailsContent" runat="server" ClientIDMode="Static"></div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <script type="text/javascript">
            $(document).ready(function () {
                try {
                    $('.select2').select2();
                } catch (e) {
                    console.error('Error initializing Select2: ', e);
                }
            });

            function showEmailDetailsModal(emailId) {
                try {
                    $.ajax({
                        url: 'EmailDetails.aspx/GetEmailDetails',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({ emailId: emailId }),
                        dataType: 'json',
                        success: function (response) {
                            $('#emailDetailsContent').html(response.d);
                            $('#pnlEmailDetails').modal('show');
                            console.log('Email details modal opened');
                        },
                        error: function (xhr, status, error) {
                            console.error('Error loading email details: ', error);
                            alert('Error loading email details. Check console for details.');
                        }
                    });
                } catch (e) {
                    console.error('Error opening email details modal: ', e);
                    alert('Error opening email details modal. Check console for details.');
                }
            }
        </script>
    </asp:Content>