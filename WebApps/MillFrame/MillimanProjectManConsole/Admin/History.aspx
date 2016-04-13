<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="MillimanProjectManConsole.Admin.History" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
        <telerik:RadGrid ID="RadGrid1" runat="server" OnItemDataBound="RadGrid1_ItemDataBound">
      <%--  <MasterTableView>
         <Columns>
            <telerik:GridBoundColumn UniqueName="ProjectName" DataField="Project_Name" HeaderText="Project">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn UniqueName="PublishedOn" DataField="Published_On" HeaderText="Pushed To Server On">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn UniqueName="PublishedBy" DataField="Published_By" HeaderText="Pushed to Server By">
            </telerik:GridBoundColumn>
         </Columns>
       </MasterTableView>--%>
       </telerik:RadGrid>
    
    </div>
    </form>
</body>
</html>
