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
        <asp:Label ID="lblMsg" runat="server" CssClass="text-danger" />

        <!-- Tabs -->
        <ul class="nav nav-tabs">
            <li class="nav-item">
                <asp:LinkButton ID="lnkAll" runat="server" CssClass="nav-link" OnClick="lnkAll_Click">All Complaints</asp:LinkButton>
            </li>
            <li class="nav-item">
                <asp:LinkButton ID="lnkOpen" runat="server" CssClass="nav-link" OnClick="lnkOpen_Click">Open Complaints</asp:LinkButton>
            </li>
            <li class="nav-item">
                <asp:LinkButton ID="lnkClosed" runat="server" CssClass="nav-link" OnClick="lnkClosed_Click">Closed Complaints</asp:LinkButton>
            </li>
        </ul>

        <!-- Filter -->
        <div class="row mt-3">
            <div class="col-md-3">
                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged">
                    <asp:ListItem Text="All Categories" Value="All" />
                    <asp:ListItem Text="Missing Miles" Value="Missing Miles" />
                    <asp:ListItem Text="Nominee" Value="Nominee" />
                    <asp:ListItem Text="Redemption" Value="Redemption" />
                    <asp:ListItem Text="Service Center (Emails)" Value="Service Center (Emails)" />
                    <asp:ListItem Text="Other" Value="Other" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlFilter" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Select Filter" Value="" />
                    <asp:ListItem Text="Ticket No" Value="TicketNo" />
                    <asp:ListItem Text="Membership No" Value="MembershipNo" />
                    <asp:ListItem Text="Request Date" Value="RequestDate" />
                    <asp:ListItem Text="Email" Value="Email" />
                    <asp:ListItem Text="Forwarded To" Value="ForwardedTo" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtFilter" runat="server" CssClass="form-control" placeholder="Filter value..." />
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnFilter" runat="server" Text="Go" CssClass="btn btn-success" OnClick="btnFilter_Click" />
            </div>
        </div>

        <!-- Grid -->
        <asp:GridView ID="gvComplaints" runat="server" CssClass="table table-bordered table-striped mt-3" AutoGenerateColumns="False" DataKeyNames="ComplaintID" OnRowCommand="gvComplaints_RowCommand">
            <Columns>
                <asp:BoundField DataField="MembershipNo" HeaderText="Membership ID" />
                <asp:BoundField DataField="RequestDate" HeaderText="Request Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:TemplateField HeaderText="Days Left">
                    <ItemTemplate>
                        <%# 30 - (DateTime.Now - Convert.ToDateTime(Eval("RequestDate"))).Days %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Subject" HeaderText="Subject" />
                <asp:BoundField DataField="ForwardedTo" HeaderText="Forwarded To" />
                <asp:BoundField DataField="Status" HeaderText="Status" />
                <asp:HyperLinkField Text="View Details" DataNavigateUrlFields="ComplaintID" DataNavigateUrlFormatString="ComplaintDetails.aspx?ComplaintID={0}" HeaderText="Action" />
                <asp:ButtonField Text="Reply" CommandName="Reply" HeaderText="Reply" ButtonType="Button" ControlStyle-CssClass="btn btn-primary btn-sm" />
                <asp:ButtonField Text="Forward" CommandName="Forward" HeaderText="Forward" ButtonType="Button" ControlStyle-CssClass="btn btn-warning btn-sm" />
            </Columns>
        </asp:GridView>

        <!-- Reply Modal -->
        <asp:Panel ID="pnlReply" runat="server" CssClass="modal fade" Style="display: none;" ClientIDMode="Static">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-darkgreen">
                        <h5 class="modal-title">Reply to Complaint</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <label for="txtReplyTo">To</label>
                            <asp:TextBox ID="txtReplyTo" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="form-group">
                            <label for="txtReplyFrom">From</label>
                            <asp:TextBox ID="txtReplyFrom" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="form-group">
                            <label for="txtReplySubject">Subject</label>
                            <asp:TextBox ID="txtReplySubject" runat="server" CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <label for="txtReplyBody">Body</label>
                            <asp:TextBox ID="txtReplyBody" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnSendReply" runat="server" Text="Send Reply" CssClass="btn btn-success" OnClick="btnSendReply_Click" />
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Forward Modal -->
        <asp:Panel ID="pnlForward" runat="server" CssClass="modal fade" Style="display: none;" ClientIDMode="Static">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-darkgreen">
                        <h5 class="modal-title">Forward Complaint</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <label for="ddlForwardTo">Forward To</label>
                            <asp:DropDownList ID="ddlForwardTo" runat="server" CssClass="form-control select2" />
                        </div>
                        <div class="form-group">
                            <label for="txtForwardRemarks">Forward Remarks</label>
                            <asp:TextBox ID="txtForwardRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="btnSendForward" runat="server" Text="Forward" CssClass="btn btn-warning" OnClick="btnSendForward_Click" />
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:HiddenField ID="hfComplaintID" runat="server" />

        <script>
            $(document).ready(function () {
                $('.select2').select2();
            });

            function showReplyModal() {
                try {
                    $('#pnlReply').modal('show');
                    console.log('Reply modal opened');
                } catch (e) {
                    console.error('Error opening reply modal: ', e);
                    alert('Error opening reply modal. Check console for details.');
                }
            }

            function showForwardModal() {
                try {
                    $('#pnlForward').modal('show');
                    console.log('Forward modal opened');
                } catch (e) {
                    console.error('Error opening forward modal: ', e);
                    alert('Error opening forward modal. Check console for details.');
                }
            }
        </script>
    </div>
</asp:Content>