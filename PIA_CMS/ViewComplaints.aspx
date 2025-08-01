<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewComplaints.aspx.cs" Inherits="PIA_CMS.ViewComplaints" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .bg-darkgreen {
            background-color: darkgreen;
            color: white;
        }
        .modal-body {
            max-height: 500px;
            overflow-y: auto;
        }
        .table th, .table td {
            vertical-align: middle;
        }
        h2 {
            color: #004d40;
            font-weight: 600;
        }
        .btn-success, .btn-primary, .btn-warning, .btn-info {
            transition: background-color 0.3s ease-in-out, color 0.3s ease-in-out, border-color 0.3s ease-in-out;
        }
        .btn-success {
            background-color: darkgreen !important;
            color: white !important;
            border-color: #004225;
        }
        .btn-success:hover {
            background-color: white !important;
            color: darkgreen !important;
            border-color: darkgreen;
        }
        .btn-primary {
            background-color: darkgreen !important;
            color: white !important;
            border-color: darkgreen;
        }
        .btn-primary:hover {
            background-color: white !important;
            color: darkgreen !important;
            border-color: darkgreen;
        }
        .btn-warning {
            background-color: darkgreen !important;
            color: white !important;
            border-color: darkgreen;
        }
        .btn-warning:hover {
            background-color: white !important;
            color: darkgreen !important;
            border-color: darkgreen;
        }
        .btn-info {
            background-color: darkgreen !important;
            color: white !important;
            border-color: darkgreen;
        }
        .btn-info:hover {
            background-color: white !important;
            color: darkgreen !important;
            border-color: darkgreen;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2>View Complaints</h2>

        <!-- Filter -->
        <div class="row mb-3">
            <div class="col-md-4">
                <asp:DropDownList ID="ddlFilterBy" runat="server" CssClass="form-control select2">
                    <asp:ListItem Text="Select Filter" Value="" />
                    <asp:ListItem Text="Category" Value="Category" />
                    <asp:ListItem Text="Ticket No" Value="TicketNo" />
                    <asp:ListItem Text="Membership No" Value="MembershipNo" />
                    <asp:ListItem Text="Date" Value="RequestDate" />
                    <asp:ListItem Text="Email" Value="Email" />
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
        <asp:GridView ID="gvComplaints" runat="server" CssClass="table table-striped table-bordered" AutoGenerateColumns="False" OnRowCommand="gvComplaints_RowCommand">
            <Columns>
                <asp:BoundField DataField="ComplaintID" HeaderText="S.No" />
                <asp:BoundField DataField="TicketNo" HeaderText="Ticket No" />
                <asp:BoundField DataField="Category" HeaderText="Category" />
                <asp:BoundField DataField="Subject" HeaderText="Subject" />
                <asp:BoundField DataField="RequestDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:Button ID="btnViewDetails" runat="server" Text="View" CssClass="btn btn-primary btn-sm" CommandName="ViewDetails" CommandArgument='<%# Eval("ComplaintID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <!-- Complaint Details Modal -->
        <asp:Panel ID="pnlComplaintDetails" runat="server" CssClass="modal fade" Style="display: none;" ClientIDMode="Static">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-darkgreen">
                        <h5 class="modal-title">Complaint Details</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div id="complaintDetailsContent" runat="server" ClientIDMode="Static"></div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" onclick="redirectToReply();">Reply</button>
                        <button type="button" class="btn btn-info" onclick="showForwardModal();">Forward</button>
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Forward Modal -->
        <asp:Panel ID="pnlForwardComplaint" runat="server" CssClass="modal fade" Style="display: none;" ClientIDMode="Static">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-darkgreen">
                        <h5 class="modal-title">Forward Complaint</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <label>Select Admin:</label>
                            <asp:DropDownList ID="ddlForwardTo" runat="server" CssClass="form-control select2" onchange="updateForwardEmail();">
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>To (Email):</label>
                            <asp:TextBox ID="txtForwardEmail" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="form-group">
                            <label>Subject:</label>
                            <asp:TextBox ID="txtForwardSubject" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="form-group">
                            <label>Forward Remarks:</label>
                            <asp:TextBox ID="txtForwardRemarks" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="3" />
                        </div>
                        <div class="form-group">
                            <label>Complaint Details:</label>
                            <asp:TextBox ID="txtForwardBody" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="5" ReadOnly="true" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnForwardSend" runat="server" Text="Send" CssClass="btn btn-primary" OnClick="btnForwardSend_Click" />
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

            function showComplaintDetailsModal(complaintId) {
                try {
                    $.ajax({
                        url: 'ComplaintDetails.aspx/GetComplaintDetails',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({ complaintId: complaintId }),
                        dataType: 'json',
                        success: function (response) {
                            $('#complaintDetailsContent').html(response.d);
                            $('#pnlComplaintDetails').modal('show').data('complaintId', complaintId);
                            console.log('Complaint details modal opened');
                        },
                        error: function (xhr, status, error) {
                            console.error('Error loading complaint details: ', error);
                            alert('Error loading complaint details. Check console for details.');
                        }
                    });
                } catch (e) {
                    console.error('Error opening complaint details modal: ', e);
                    alert('Error opening complaint details modal. Check console for details.');
                }
            }

            function redirectToReply() {
                var complaintId = $('#pnlComplaintDetails').data('complaintId');
                window.location.href = 'ReplyComplaint.aspx?complaintId=' + complaintId;
                return false;
            }

            function showForwardModal() {
                try {
                    var complaintId = $('#pnlComplaintDetails').data('complaintId');
                    $.ajax({
                        url: 'ViewComplaints.aspx/GetForwardData',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({ complaintId: complaintId }),
                        dataType: 'json',
                        success: function (response) {
                            $('#<%= txtForwardSubject.ClientID %>').val(response.d.subject);
                            $('#<%= txtForwardBody.ClientID %>').val(response.d.body);
                            $('#pnlForwardComplaint').modal('show').data('complaintId', complaintId);
                            console.log('Forward modal opened');
                        },
                        error: function (xhr, status, error) {
                            console.error('Error loading forward data: ', error);
                            alert('Error loading forward data. Check console for details.');
                        }
                    });
                } catch (e) {
                    console.error('Error opening forward modal: ', e);
                    alert('Error opening forward modal. Check console for details.');
                }
            }

            function updateForwardEmail() {
                var adminId = $('#<%= ddlForwardTo.ClientID %>').val();
                if (adminId) {
                    $.ajax({
                        url: 'ViewComplaints.aspx/GetAdminEmail',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({ adminId: adminId }),
                        dataType: 'json',
                        success: function (response) {
                            $('#<%= txtForwardEmail.ClientID %>').val(response.d);
                        },
                        error: function (xhr, status, error) {
                            console.error('Error fetching admin email: ', error);
                            alert('Error fetching admin email. Check console for details.');
                        }
                    });
                } else {
                    $('#<%= txtForwardEmail.ClientID %>').val('');
                }
            }
        </script>
    </asp:Content>