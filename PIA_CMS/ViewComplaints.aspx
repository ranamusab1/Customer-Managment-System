<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewComplaints.aspx.cs" Inherits="PIA_CMS.ViewComplaints" MasterPageFile="~/Site.master" %>

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
        .btn-success, .btn-primary, .btn-warning, .btn-info {
            transition: background-color 0.3s ease-in-out, color 0.3s ease-in-out, border-color 0.3s ease-in-out;
            background-color: darkgreen !important;
            color: white !important;
            border-color: darkgreen;
        }
        .btn-success:hover, .btn-primary:hover, .btn-warning:hover, .btn-info:hover {
            background-color: white !important;
            color: darkgreen !important;
            border-color: darkgreen;
        }
        .attachment-img {
            width: 100px;
            height: 100px;
            object-fit: cover;
            margin: 5px;
            cursor: pointer;
        }
        .attachment-pdf {
            width: 100px;
            height: 100px;
            margin: 5px;
            cursor: pointer;
            background: url('https://via.placeholder.com/100?text=PDF') no-repeat center;
            background-size: cover;
        }
        .full-size-img {
            max-width: 100%;
            max-height: 80vh;
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
        .forward-history-table {
            width: 100%;
            margin-top: 10px;
        }
        .forward-history-table th {
            background-color: #f8f9fa;
            font-weight: 600;
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
                    <asp:ListItem Text="Ticket No" Value="tkt_no" />
                    <asp:ListItem Text="Membership No" Value="ffnum" />
                    <asp:ListItem Text="Date" Value="Req_Date" />
                    <asp:ListItem Text="Email" Value="email" />
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
        <asp:GridView ID="gvComplaints" runat="server" CssClass="table table-striped table-bordered" AutoGenerateColumns="False" OnRowCommand="gvComplaints_RowCommand" DataKeyNames="tkt_no">
            <Columns>
                <asp:BoundField DataField="tkt_no" HeaderText="Ticket No" />
                <asp:BoundField DataField="Category" HeaderText="Category" />
                <asp:BoundField DataField="Subject" HeaderText="Subject" />
                <asp:BoundField DataField="Req_Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="fwd_to" HeaderText="Forwarded To" />
                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <asp:Button ID="btnViewDetails" runat="server" Text="View" CssClass="btn btn-primary btn-sm" CommandName="ViewDetails" CommandArgument='<%# Container.DataItemIndex %>' />
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
                        <!-- Hidden field to store ComplaintID -->
                        <asp:HiddenField ID="hfComplaintId" runat="server" ClientIDMode="Static" />
                        <!-- Complaint Details -->
                        <div id="complaintDetailsContent" runat="server" ClientIDMode="Static"></div>
                        <hr />
                        <!-- Conversation -->
                        <h6>Conversation</h6>
                        <div id="conversationContent" class="border p-3 mb-3"></div>
                        <!-- Attachments -->
                        <h6>Attachments</h6>
                        <div id="attachmentsContent" class="d-flex flex-wrap mb-3"></div>
                        <!-- Reply Form -->
                        <h6>Reply</h6>
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
                        <asp:Button ID="btnReplySend" runat="server" Text="Send Reply" CssClass="btn btn-primary mt-2" OnClick="btnReplySend_Click" />
                        <hr />
                        <!-- Forward Form -->
                        <h6>Forward</h6>
                        <div class="form-group">
                            <label>Select Admin:</label>
                            <asp:DropDownList ID="ddlForwardTo" runat="server" CssClass="form-control select2" onchange="console.log('ddlForwardTo changed: ' + this.value);">
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>Forward Remarks:</label>
                            <asp:TextBox ID="txtForwardRemarks" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Enter forward remarks..." />
                        </div>
                        <asp:Button ID="btnForwardSend" runat="server" Text="Forward" CssClass="btn btn-info mt-2" OnClick="btnForwardSend_Click" />
                        <hr />
                        <!-- Forward History -->
                        <h6>Forward History</h6>
                        <div id="forwardHistoryContent" class="border p-3 mb-3"></div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Full-Size Attachment Modal -->
        <div class="modal fade" id="fullSizeAttachmentModal" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-darkgreen">
                        <h5 class="modal-title">Attachment</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body text-center">
                        <div id="fullSizeAttachmentContent"></div>
                    </div>
                </div>
            </div>
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
                    alert('Select2 initialization failed. Dropdown will work as standard select.');
                }
            });

            function showComplaintDetailsModal(tkt_no) {
                try {
                    $.ajax({
                        url: 'ViewComplaints.aspx/GetComplaintDetails',
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({ tkt_no: tkt_no }),
                        dataType: 'json',
                        success: function (response) {
                            $('#complaintDetailsContent').html(response.d.details);
                            $('#conversationContent').html(response.d.conversation);
                            $('#attachmentsContent').html(response.d.attachments);
                            $('#forwardHistoryContent').html(response.d.forwardHistory);
                            $('#<%= txtReplyEmail.ClientID %>').val(response.d.email);
                            $('#<%= txtReplySubject.ClientID %>').val(response.d.replySubject);
                            $('#<%= txtReplyBody.ClientID %>').val(response.d.replyBody);
                            $('#hfComplaintId').val(tkt_no);

                            // Log email for debugging
                            console.log('txtReplyEmail set to: ' + response.d.email);

                            // Re-bind admins when modal opens
                            $.ajax({
                                url: 'ViewComplaints.aspx/BindForwardAdminsAjax',
                                type: 'POST',
                                contentType: 'application/json; charset=utf-8',
                                data: '{}',
                                dataType: 'json',
                                success: function (response) {
                                    console.log('BindForwardAdminsAjax response: ', response.d);
                                    $('#<%= ddlForwardTo.ClientID %>').html(response.d);
                                    $('#<%= ddlForwardTo.ClientID %>').select2({
                                        placeholder: "Select Admin",
                                        allowClear: true
                                    });
                                    console.log('ddlForwardTo re-bound with admins.');
                                    if (response.d.includes('Error') || response.d === "<option value=''>Select Admin</option>") {
                                        alert('No admins loaded in dropdown. Please check database for active admins (enable=1).');
                                    }
                                },
                                error: function (xhr, status, error) {
                                    console.error('Error re-binding admins: ', error, xhr.responseText);
                                    alert('Error loading admins for forward dropdown. Check console for details.');
                                    $('#<%= ddlForwardTo.ClientID %>').html('<option value="">No Admins Available</option>');
                                }
                            });

                            $('#pnlComplaintDetails').modal('show').data('tkt_no', tkt_no);
                            console.log('Complaint details modal opened for tkt_no: ' + tkt_no);
                        },
                        error: function (xhr, status, error) {
                            console.error('Error loading complaint details: ', error, xhr.responseText);
                            alert('Error loading complaint details. Check console for details.');
                        }
                    });
                } catch (e) {
                    console.error('Error opening complaint details modal: ', e);
                    alert('Error opening complaint details modal. Check console for details.');
                }
            }

            function showFullSizeAttachment(filePath) {
                var ext = filePath.split('.').pop().toLowerCase();
                var content = '';
                if (['jpg', 'jpeg', 'png', 'gif'].includes(ext)) {
                    content = `<img src="${filePath}" class="full-size-img" alt="Attachment" />`;
                } else if (ext === 'pdf') {
                    content = `<iframe src="${filePath}" style="width:100%;height:80vh;"></iframe>`;
                } else {
                    content = `<a href="${filePath}" target="_blank">Download ${filePath.split('/').pop()}</a>`;
                }
                $('#fullSizeAttachmentContent').html(content);
                $('#fullSizeAttachmentModal').modal('show');
            }
        </script>
    </asp:Content><%--  --%>