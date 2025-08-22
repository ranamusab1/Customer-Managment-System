<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewEmails.aspx.cs" Inherits="PIA_CMS.ViewEmails" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .bg-darkgreen {
            background-color: darkgreen;
            color: white;
        }
        .modal-body {
            max-height: 600px;
            overflow-y: auto;
        }
        .table th, .table td {
            vertical-align: middle;
        }
        h2 {
            color: #004d40;
            font-weight: 600;
        }
        .btn-success, .btn-primary {
            transition: background-color 0.3s ease-in-out, color 0.3s ease-in-out, border-color 0.3s ease-in-out;
            background-color: darkgreen !important;
            color: white !important;
            border-color: darkgreen;
        }
        .btn-success:hover, .btn-primary:hover {
            background-color: white !important;
            color: darkgreen !important;
            border-color: darkgreen;
        }
        .conversation-entry {
            border-bottom: 1px solid #ddd;
            padding: 10px 0;
            margin-bottom: 10px;
        }
        .conversation-header {
            font-weight: bold;
            color: #004d40;
            margin-bottom: 5px;
        }
        .conversation-body {
            margin: 0;
            padding-left: 10px;
            color: #333;
        }
        .conversation-subject {
            font-style: italic;
            color: #555;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2>View Emails</h2>
        <!-- Filter -->
        <div class="row mb-3">
            <div class="col-md-4">
                <asp:DropDownList ID="ddlFilterBy" runat="server" CssClass="form-control select2">
                    <asp:ListItem Text="Select Filter" Value="" />
                    <asp:ListItem Text="S.No" Value="sno" />
                    <asp:ListItem Text="Sent By" Value="userid" />
                    <asp:ListItem Text="Sent Date" Value="senddate" />
                    <asp:ListItem Text="Subject" Value="emlsub" />
                    <asp:ListItem Text="Sent To" Value="sendto" />
                    <asp:ListItem Text="Membership No" Value="ffnum" />
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
        <asp:GridView ID="gvEmails" runat="server" CssClass="table table-striped table-bordered" AutoGenerateColumns="False" DataKeyNames="sno" OnRowCommand="gvEmails_RowCommand">
            <Columns>
                <asp:BoundField DataField="sno" HeaderText="S.No" />
                <asp:BoundField DataField="userid" HeaderText="Sent By" />
                <asp:BoundField DataField="senddate" HeaderText="Sent Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:BoundField DataField="emlsub" HeaderText="Subject" />
                <asp:BoundField DataField="sendto" HeaderText="Sent To" />
                <asp:BoundField DataField="ffnum" HeaderText="Membership No" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:Button ID="btnViewDetails" runat="server" Text="View" CssClass="btn btn-primary btn-sm" CommandName="ViewDetails" CommandArgument='<%# Container.DataItemIndex %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <!-- Email Details Modal -->
        <asp:Panel ID="pnlEmailDetails" runat="server" CssClass="modal fade" Style="display: none;" ClientIDMode="Static">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-darkgreen">
                        <h5 class="modal-title">Email Details</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div id="emailDetailsContent" runat="server" ClientIDMode="Static"></div>
                        <hr />
                        <h6>Email Content</h6>
                        <div id="emailContent" class="border p-3 mb-3"></div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            try {
                $('.select2').select2({
                    placeholder: "Select an option",
                    allowClear: true
                });
                console.log('Select2 initialized on page load.');
            } catch (e) {
                console.error('Error initializing Select2: ', e);
                alert('Select2 initialization failed.');
            }
        });

        function showEmailDetailsModal(emailId) {
            try {
                $.ajax({
                    url: 'ViewEmails.aspx/GetEmailDetails',
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify({ sno: emailId }),
                    dataType: 'json',
                    success: function (response) {
                        $('#emailDetailsContent').html(response.d.details);
                        $('#emailContent').html(response.d.content);
                        $('#pnlEmailDetails').modal('show');
                        console.log('Email details modal opened for sno: ' + emailId);
                    },
                    error: function (xhr, status, error) {
                        console.error('Error loading email details: ', error, xhr.responseText);
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