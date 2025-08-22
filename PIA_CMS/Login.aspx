<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PIA_CMS.Login" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8" />
    <title>PIA Award+Plus Admin Login</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            background-color: darkgreen;
            font-family: 'Segoe UI', sans-serif;
            height: 100vh;
            margin: 0;
            display: flex;
            justify-content: center;
            align-items: center;
        }
        .login-card {
            background-color: #fff;
            padding: 30px 35px;
            border-radius: 16px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.25);
            width: 100%;
            max-width: 420px;
            text-align: center;
        }
        .login-card img {
            width: 200px;
            margin-bottom: 15px;
        }
        .login-card h2 {
            font-size: 24px;
            font-weight: 600;
            color: #004d40;
            margin-bottom: 10px;
        }
        .login-card p {
            font-size: 14px;
            color: #666;
            margin-bottom: 20px;
        }
        .form-control {
            border-radius: 6px;
            font-size: 14px;
        }
        .btn-login {
            background-color: darkgreen;
            color: white;
            font-weight: 500;
            border-radius: 6px;
            padding: 10px 16px;
            width: 100%;
            transition: background-color 0.3s ease-in-out, color 0.3s ease-in-out, border-color 0.3s ease-in-out;
        }
        .btn-login:hover {
            background-color: white;
            color: darkgreen;
            border-color: darkgreen;
        }
        .text-danger {
            font-size: 13px;
            margin-bottom: 10px;
            display: block;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-card">
            <img src="pialogo2.png" alt="PIA Logo" />
            <h2>PIA Award+Plus</h2>
            <p>Admin Login</p>
            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" />
            <div class="mb-3 text-start">
                <asp:TextBox ID="txtUsername" runat="server" placeholder="Username" CssClass="form-control" />
            </div>
            <div class="mb-3 text-start">
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Password" CssClass="form-control" />
            </div>
            <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" CssClass="btn btn-login" />
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>