<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ComplaintDetails.aspx.cs" Inherits="PIA_CMS.ComplaintDetails" MasterPageFile="~/Site.master" %>

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
        .card-body {
            max-height: 500px;
            overflow-y: auto;
        }
        .form-group {
            margin-bottom: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2>Complaint Details</h2>
        <asp:Label ID="lblMsg" runat="server" CssClass="text-danger" />
        <asp:HiddenField ID="hfComplaintID" runat="server" />

        <div class="card mt-3">
            <div class="card-header bg-darkgreen">
                <h5 class="card-title mb-0">Complaint Information</h5>
            </div>
            <div class="card-body">
                <asp:Label ID="lblComplaintDetails" runat="server" CssClass="d-block mb-3" />
                <hr />
                <h6>Email Details</h6>
                <asp:Label ID="lblEmailDetails" runat="server" CssClass="d-block mb-3" />
                <hr />
                <div class="form-group">
                    <label for="ddlMoveCategory">Move to Category</label>
                    <asp:DropDownList ID="ddlMoveCategory" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Select Category" Value="" />
                        <asp:ListItem Text="Missing Miles" Value="Missing Miles" />
                        <asp:ListItem Text="Nominee" Value="Nominee" />
                        <asp:ListItem Text="Redemption" Value="Redemption" />
                        <asp:ListItem Text="Service Center (Emails)" Value="Service Center (Emails)" />
                        <asp:ListItem Text="Other" Value="Other" />
                    </asp:DropDownList>
                </div>
            </div>
            <div class="card-footer">
                <asp:Button ID="btnMoveCategory" runat="server" Text="Move Category" CssClass="btn btn-success" OnClick="btnMoveCategory_Click" />
                <asp:Button ID="btnReply" runat="server" Text="Reply" CssClass="btn btn-primary" OnClick="btnReply_Click" />
                <asp:Button ID="btnForward" runat="server" Text="Forward" CssClass="btn btn-warning" OnClick="btnForward_Click" />
                <asp:Button ID="btnMarkSolved" runat="server" Text="Mark as Solved" CssClass="btn btn-info" OnClick="btnMarkSolved_Click" />
                <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-secondary" PostBackUrl="~/ViewComplaints.aspx" />
            </div>
        </div>

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