# PIA_CMS - Customer Management System
## Overview
PIA_CMS is a web-based Customer Management System developed for Pakistan International Airlines (PIA) to manage customer complaints and email communications. Built using ASP.NET (C#) with a SQL Server backend, the system allows administrators to view, filter, and manage complaints and emails efficiently. Key features include:
- Viewing and filtering complaints and emails stored in the sendmaillist table.
- Displaying email content from text files (C:\Email\{sno}.txt) in HTML format within a modal.
- Filtering emails by columns like S.No, Sent By, Sent Date, Subject, Sent To, and Membership No.
- Secure admin authentication and session management.
This project is designed for internal use by PIA admins to streamline customer support operations.

## Features

### - Complaint Management:
- View complaints from a database table (complaints) with details like ticket number, user ID, and status.
- Read conversation history from text files (C:\PIA\{tkt_no}.txt) displayed in a modal.
- Filter complaints by various columns for quick searching.

### - Email Management:
- View emails from the sendmaillist table with metadata (sno, userid, senddate, emlsub, sendto, ffnum).
- Display email content from `C:\Email\{sno}.txt in HTML format (supports <h1>, <p>, <strong>, <ul>, etc.).`
- Filter emails by S.No, Sent By, Sent Date, Subject, Sent To, or Membership No.

### - UI/UX:
- Bootstrap-based responsive design with a dark green theme.
- Select2 dropdowns for enhanced filter selection.
- Modal popups for detailed views of complaints and emails.

### - Security:
- Admin authentication via session management (Session["AdminUser"]).
- Parameterized SQL queries to prevent SQL injection.

## Prerequisites

### - Environment:
- Windows Server with IIS for hosting the ASP.NET application.
- Visual Studio (2019 or later) for development and debugging.

### - Dependencies:
- .NET Framework 4.8 or later.
- SQL Server (e.g., SQL Server Express 2019).
- jQuery 3.6.0.
- Bootstrap 5.1.3.
- Select2 4.0.13.

## - File System:
- Folders: C:\PIA and C:\Email (must be writable by the application).

## - Database:
- SQL Server database named PIA_CMS with tables complaints and sendmaillist.

## Setup Instructions
### 1. Clone the Repository
`git clone https://github.com/ranamusab1/Customer-Managment-System`
`cd PIA_CMS`

### 2. Configure Database
- Create a SQL Server database named PIA_CMS.
- Execute the following SQL to create the sendmaillist table:
`CREATE TABLE sendmaillist (
    sno INT PRIMARY KEY,
    userid VARCHAR(50),
    senddate DATETIME,
    emlsub VARCHAR(200),
    sendto VARCHAR(100),
    ffnum VARCHAR(50)
);`
- Insert sample data:
`INSERT INTO sendmaillist (sno, userid, senddate, emlsub, sendto, ffnum)
VALUES
    (1, 'admin1', '2025-08-22 10:30:00', 'Response to Your Complaint', 'test@pia.com', 'MEM001'),
    (3, 'admin1', '2025-08-22 11:45:00', 'Fwd: Urgent Complaint Follow-Up', 'admin2@pia.com', 'MEM003'),
    (4, 'admin1', '2025-08-22 12:15:00', 'Response to Your Service Request', 'customer4@pia.com', 'MEM004');`

### 3. Configure File System
- Create folders `C:\PIA and C:\Email on the server.`
- Ensure the application has read/write permissions for these folders.
- Add sample email files:
```
C:\Email\1.txt:
--- Email by admin1 on 2025-08-22 10:30:00 ---
Subject: Response to Your Complaint
Body: Dear Customer,<br/><br/>Thank you for reaching out to us regarding your concern. We have reviewed your complaint (Ticket No: 1001, Membership No: MEM001) and are taking necessary actions to resolve it promptly.<br/><br/>Please feel free to contact us for further assistance.<br/><br/>Best regards,<br/>Admin One<br/>PIA Customer Support
```

```
C:\Email\3.txt:-
-- Forwarded by admin1 to Admin Two on 2025-08-22 11:45:00 ---
Subject: Fwd: Urgent Complaint Follow-Up
Body: Dear Admin Two,<br/><br/>I am forwarding you the complaint (Ticket No: 1003, Membership No: MEM003) for urgent follow-up. The customer has raised concerns about a delayed response.<br/><br/><b>Details:</b><br/>- <b>Ticket No:</b> 1003<br/>- <b>Issue:</b> Service Delay<br/>- <b>Customer Email:</b> customer3@pia.com<br/><br/>Please review and take appropriate action within 24 hours.<br/><br/>Regards,<br/>Admin One<br/>PIA Customer Support
```

```
C:\Email\4.txt:
--- Email by admin1 on 2025-08-22 12:15:00 ---
Subject: <h1>Response to Your Service Request</h1>
Body: <p>Dear Customer,</p><p>We have received your service request (Ticket No: 1004, Membership No: MEM004). Our team is actively working on it, and we aim to resolve your issue by <strong>August 25, 2025</strong>.</p><p><strong>Request Details:</strong></p><ul><li><strong>Ticket No:</strong> 1004</li><li><strong>Issue:</strong> Booking Error</li><li><strong>Contact:</strong> customer4@pia.com</li></ul><p>For further queries, please reach out to us.</p><p>Best regards,<br/><strong>Admin One</strong><br/>PIA Customer Support</p>
```

### 4. Update `web.config`
Update the connection string in web.config:
`<configuration>
  <connectionStrings>
    <add name="dbConn" connectionString="Data Source=localhost;Initial Catalog=PIA_CMS;Integrated Security=True" providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>`

### 5. Deploy Application
- Open the project in Visual Studio.
- Build and publish to IIS or run locally via Visual Studio.
- Ensure the following dependencies are included in Site.master:
```
<script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
<link href="https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.13/css/select2.min.css" rel="stylesheet" />
<script src="https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.13/js/select2.min.js"></script>
```

### 6. File Permissions
- Ensure IIS user (e.g., IIS_IUSRS) has read/write access to C:\PIA and C:\Email.
- Run in PowerShell to verify:
`dir C:\PIA
dir C:\Email`

## Usage
### Login:
- Navigate to Login.aspx.
- Use credentials (e.g., Username: admin1, Password: admin123).
- Successful login redirects to the dashboard or ViewEmails.aspx.

### View Emails:
- Open ViewEmails.aspx.
- Filter Emails:
- Select a filter type from the dropdown (S.No, Sent By, Sent Date, Subject, Sent To, Membership No).
- Enter a value in the text box (e.g., 4 for S.No, admin1 for Sent By, 2025-08-22 for Sent Date).
- Click Go to filter the grid.

### View Email Details:
- Click the View button for an email (e.g., sno=4).
- Modal displays:
- Metadata: Sent By, Sent Date, Subject, Sent To, Membership No.
- Email Content: Rendered HTML from `C:\Email\4.txt` (e.g., `<h1> for subject, <p> for body`).

### View Complaints:
- Open ViewComplaints.aspx (similar functionality).
- Filter complaints and view conversation history from C:\PIA\{tkt_no}.txt.

### Testing
- Test Environment
- Date: August 22, 2025, 11:11 PM PKT.
- Setup: SQL Server with PIA_CMS database, C:\Email and C:\PIA folders with sample files.

## Test Cases
### - Login:
- Navigate to Login.aspx.
- Enter admin1/admin123.
- Expected: Redirect to dashboard or ViewEmails.aspx.
- Failure: Invalid credentials show error alert.

### - View Emails:
- Open ViewEmails.aspx.
- Grid:
- Verify rows for sno=1,3,4 with correct metadata.

### - Filter:
- Select S.No, enter 4, click Go.
- Expected: Grid shows only sno=4.
- Select Sent Date, enter 2025-08-22, click Go.
- Expected: Grid shows all emails from August 22, 2025.
- Select Subject, enter Service, click Go.
- Expected: Grid shows sno=4 (Subject: Response to Your Service Request).

### - View Details:
- Click View for sno=4.
- Expected Modal:Sent By: admin1
```
Sent Date: 2025-08-22 12:15
Subject: Response to Your Service Request
Sent To: customer4@pia.com
Membership No: MEM004

--- Email by admin1 on 2025-08-22 12:15:00 ---
Subject: Response to Your Service Request (in <h1> font)
Dear Customer,

We have received your service request (Ticket No: 1004, Membership No: MEM004). Our team is actively working on it, and we aim to resolve your issue by August 25, 2025.

Request Details:
- Ticket No: 1004
- Issue: Booking Error
- Contact: customer4@pia.com

For further queries, please reach out to us.

Best regards,
Admin One
PIA Customer Support
```

### - Logs:
- Browser Console:
```
Select2 initialized on page load.
Email details modal opened for sno: 4
```

Visual Studio Output:
```
BindEmails: Query executed with filter=sno, value=4
gvEmails_RowCommand: sno=4
GetEmailDetails: sno=4, userid=admin1, sendto=customer4@pia.com
GetEmailContent: Attempting to read file=C:\Email\4.txt
GetEmailContent: Successfully read content for sno=4
```

## Troubleshoot
### 1. Emails Not Loading:
- Check `web.config` connection string.
- Verify `sendmaillist` data:
`SELECT * FROM sendmaillist;`
- Check Visual Studio Output for errors:
`BindEmails Error: ...`

### 2. Email Content Not Displaying:
- Verify file exists: `C:\Email\4.txt.`
- Check permissions:
`icacls C:\Email`
- Visual Studio Output:
`GetEmailContent: File not found for sno=4`

### 3. Filter Not Working:
- Check `ddlFilterBy` and `txtFilter` inputs.
- Visual Studio Output:
`BindEmails: Query executed with filter=sno, value=4`
- Run SQL manually:
`SELECT * FROM sendmaillist WHERE sno LIKE '%4%';`

### 4. Modal Errors:
- Browser Console:
`Error loading email details: ...`
- Share `xhr.responseText` for debugging.

# Contributing
- Fork the repository.
- Create a branch (`git checkout -b feature/your-feature`).
- Commit changes (`git commit -m "Add your feature"`).
- Push to the branch (`git push origin feature/your-feature`).
- Open a Pull Request.

# License
This project is licensed under the MIT License.
# Contact
For support, contact the PIA IT team or raise an issue on GitHub.

Generated on August 22, 2025
