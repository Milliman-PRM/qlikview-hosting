<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeneralIssue.aspx.cs" Inherits="ClientPublisher.HTML.GeneralIssue" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>PRM Analytics</title>
    <link rel="Stylesheet" href="Css/Styles.css" />
    <link href="../Content/Style/bootstrap.css" rel="stylesheet" />
    <link href="../Content/Style/MillframeStyle.css" rel="stylesheet" />
    <script type="text/javascript">
        if (window != window.top) window.top.location.href = window.location.href;
    </script>
</head>
<body style="background-color: white; background-image: url(../Images/watermark.png); background-repeat: repeat">
    <form id="form1" runat="server">
        <div id="divErrorPageContainer" class="center-block">
            <div class="row">
                <div class="roundShadowContainer">
                    <div id="divLogoImage">
                        <img src="../Images/PRMLogo_height80.png" style="width: 200px;" alt="Milliman Logo" />
                    </div>
                    <div class="space"></div>
                    <div class="space"></div>
                    <div class="space"></div>
                    <p class="text-justify">
                        An unspecified error condition has halted processing.  
                        <br />
                        Please contact a system administrator.
                        <br />
                        <asp:Label ID="ErrorMsg" runat="server"></asp:Label>
                    </p>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
