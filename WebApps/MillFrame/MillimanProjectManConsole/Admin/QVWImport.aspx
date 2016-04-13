<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QVWImport.aspx.cs" Inherits="MillimanProjectManConsole.Admin.QVWImport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Create Project from QVW Import</title>
    <style type="text/css">
    .CenterMeHorVer{
	    width:600px;
	    height:150px;
	    position:absolute;
	    left:50%;
	    top:50%;
	    margin:-75px 0 0 -300px;
        background-color:white;
        font-size:14px;
        border:1px solid black;
    }

        body {
            background:url("../images/watermark.png");
        }
</style>

</head>
<body>
   <form id="form1" runat="server" style="text-align:center">
        <div class="CenterMeHorVer">  
            <div style="position:absolute;  top: 46px; left: 52px;font-weight:900">Select a signed QVW for import.</div>
            <asp:FileUpload style="position:absolute; top: 70px; left: 52px; width: 513px;" runat="server" ID="ImportQVW" BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px" />
            <asp:Button style="position:absolute; top: 110px; left: 262px; width: 114px;" runat="server" ID="Upload" Text="Import QVW" OnClientClick="return Validate()" OnClick="Upload_Click"  />
        </div>
    </form>

     <script type="text/javascript">
         function Validate() {
            
             var Filename = document.getElementById("ImportQVW");
             if (Filename != null) {
                 if (Filename.value.toLowerCase().indexOf(".qvw") != -1) {
                     return true;
                 }
             }
             alert("File selected for import is not a file with extension '.QVW' - no file imported.");
             return false;
         }
    </script>
</body>
</html>
