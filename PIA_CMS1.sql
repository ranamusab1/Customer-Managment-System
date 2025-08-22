


CREATE TABLE [dbo].[admin_login](
    [sno] [int] IDENTITY(1,1) NOT NULL,
    [user_name] [varchar](30) NULL,
    [password] [varchar](30) NULL,
    [login_id] [varchar](50) NULL,
    [enable] [int] NULL,
    [station_id] [varchar](50) NULL,
    [ftl] [int] NULL,
    [search] [int] NULL,
    [ip] [varchar](50) NULL,
    [OlReq] [decimal](1, 0) NULL,
    [LastLogin] [datetime] NULL,
    [logincount] [int] NULL,
    [creation_date] [datetime] NULL,
    [EMAIL] [varchar](50) NULL,
    [phone_no] [varchar](20) NULL,
    [mobile] [varchar](20) NULL,
    [Authorized_Station_ID] [varchar](20) NULL,
    [OnlineRedemption_Allow] [varchar](1) NULL,
    [NameOfEmployee] [varchar](30) NULL,
    [User_Authorized_To_Create_Subuser] [varchar](1) NULL,
    [Station_Code] [varchar](15) NULL,
    [pwd_change_dt] [datetime] NULL,
    [PIN_View_Allowed] [int] NULL,
    [close_dt] [datetime] NULL,
    [API_token] [varchar](50) NULL,
 CONSTRAINT [PK_admin_login] PRIMARY KEY CLUSTERED 
(
    [sno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


INSERT INTO admin_login (user_name, password, login_id, enable, EMAIL, NameOfEmployee, creation_date)
VALUES 
    ('admin1', 'admin123', 'admin1@pia.com', 1, 'admin1@pia.com', 'Admin One', GETDATE()),
    ('admin2', 'admin123', 'admin2@pia.com', 1, 'admin2@pia.com', 'Admin Two', GETDATE());


	select * from [admin_login];


CREATE TABLE [dbo].[cms](
    [tkt_no] [numeric](10, 0) NULL,
    [ffnum] [varchar](250) NULL,
    [Req_Date] [smalldatetime] NULL,
    [Category] [varchar](50) NULL,
    [Subject] [varchar](1600) NULL,
    [cstatus] [varchar](1) NULL,
    [UpdateDate] [smalldatetime] NULL,
    [UpdateBy] [varchar](50) NULL,
    [fwd_to] [varchar](50) NULL,
    [fwd_date] [smalldatetime] NULL,
    [fwd_remarks] [varchar](250) NULL,
    [fwd_by] [varchar](50) NULL,
    [attachments] [varchar](100) NULL,
    [email] [varchar](3000) NULL,
    [TopCategory] [varchar](30) NULL,
    [CorporateDetails] [varchar](12) NULL,
    [urgent] [varchar](5) NULL,
    [Req_By] [varchar](25) NULL,
    [PointsExp] [varchar](1) NULL,
    [tier] [varchar](1) NULL,
    [download_datetime] [datetime] NULL,
    [hitit_ref_no] [varchar](50) NULL
) ON [PRIMARY]


INSERT INTO cms (tkt_no, ffnum, Req_Date, Category, Subject, cstatus, email, Req_By)
VALUES 
    (1001, 'MEM001', GETDATE(), 'Missing Miles', 'Test Complaint', 'O', 'test@pia.com', 'Test User'),
    (1002, 'MEM002', GETDATE(), 'Service Issue', 'Test Complaint 2', 'O', 'test2@pia.com', 'Test User 2');


select * from cms;


CREATE TABLE [dbo].[sendmaillist](
    [sno] [int] NULL,
    [userid] [varchar](50) NULL,
    [senddate] [smalldatetime] NULL,
    [emlsub] [varchar](100) NULL,
    [sendto] [varchar](50) NULL,
    [cc] [varchar](250) NULL,
    [ffnum] [varchar](9) NULL
) ON [PRIMARY]


INSERT INTO sendmaillist (sno, userid, senddate, emlsub, sendto, ffnum)
VALUES 
    (1, 'admin1', GETDATE(), 'Test Reply', 'test@pia.com', 'MEM001'),
    (2, 'admin2', GETDATE(), 'Fwd: Test Complaint', 'admin1@pia.com', 'MEM001');


select * from sendmaillist;

CREATE TABLE [dbo].[cms_fwd_remarks](
    [sno] [int] IDENTITY(1,1) NOT NULL,
    [fwd_to] [varchar](50) NULL,
    [fwd_by] [varchar](50) NULL,
    [fwd_date] [smalldatetime] NULL,
    [remarks] [varchar](255) NULL,
    [tkt_no] [int] NULL
) ON [PRIMARY]

ALTER TABLE [dbo].[cms_fwd_remarks] ADD CONSTRAINT PK_cms_fwd_remarks PRIMARY KEY (sno);


INSERT INTO cms_fwd_remarks (fwd_to, fwd_by, fwd_date, remarks, tkt_no)
VALUES 
    ('Admin Two', 'admin1', GETDATE(), 'Urgent: Please review', 1001),
    ('Admin One', 'admin2', GETDATE(), 'Follow-up required', 1001);

	select * from cms_fwd_remarks;
	SELECT sno, NameOfEmployee FROM admin_login WHERE enable = 1 AND NameOfEmployee IS NOT NULL AND NameOfEmployee != '' ORDER BY NameOfEmployee;