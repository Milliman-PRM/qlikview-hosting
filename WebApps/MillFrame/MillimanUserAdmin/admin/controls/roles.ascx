<%@ Control Language="C#" AutoEventWireup="true" CodeFile="roles.ascx.cs" Inherits="admin_controls_roles" %>
<%@ Register Src="js-include1.ascx" TagName="js" TagPrefix="uc1" %>
<%@ Register Src="js-include3.ascx" TagName="js" TagPrefix="uc3" %>
<%@ Register Src="~/js/js/jquery.ascx" TagName="jquery" TagPrefix="uc4" %>
<%@ Register Src="search-box.ascx" TagName="search" TagPrefix="uc2" %>
<%@ Register Src="a-z-menu.ascx" TagName="a" TagPrefix="uc5" %>

<%-- gridview banner --%>
<div class="gvBanner">
    <span class="gvBannerUsers">
        <asp:Image ID="Image1" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/decoy-icon-36px.png" />
    </span>Groups:
    <%-- create new Role form elements --%>
    <asp:TextBox runat="server" ID="NewRole" MaxLength="50" Width="135px" ToolTip="Type the name of a new group you want to create."></asp:TextBox>
    <asp:Button ID="Button2" runat="server" OnClick="AddRole" Text="Add Group" ToolTip="Click to create new group." />
    <%-- search box --%>
    <uc2:search ID="search1" runat="server" />
    <%-- message label --%>
    <div class="messageWrap">
        <asp:HyperLink ID="Msg" runat="server" Style="color: red" Visible="False">[Msg]</asp:HyperLink>
    </div>
</div>
<%-- a-z navigation --%>
<uc5:a ID="a1" runat="server" />
<asp:GridView ID="UserRoles" runat="server" AutoGenerateColumns="False"
    OnRowDataBound="UserRoles_RowDataBound"
    CssClass="gv">
    <Columns>
        <asp:TemplateField>
            <HeaderStyle CssClass="gvHeader" Width="1px" />
            <ItemStyle CssClass="gvHeader" Width="1px" />
        </asp:TemplateField>
        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20px" HeaderText="#">
            <ItemTemplate>
                <%# Convert.ToInt32(DataBinder.Eval(Container, "DataItemIndex")) + 1 %>.
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" Width="20px"></ItemStyle>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Del">
            <HeaderTemplate>
                <input id="chkAll" onclick="SelectAllCheckboxes('chkRows', this.checked);" runat="server" type="checkbox" title="Check all checkboxes" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="chkRows" runat="server" ToolTip="Select for deletion" />
            </ItemTemplate>
            <ItemStyle Width="25px" HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                GROUP NAME
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="RoleName" runat="server" Text='<%# Eval("Role Name") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Left" Width="300px" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                FRIENDLY NAME
            </HeaderTemplate>
            <ItemTemplate>
                <asp:TextBox ID="FriendlyName" runat="server" Text='<%# Eval("Friendly Name") %>' Width="95%"
                    ValidationGroup="check" CssClass="friendlyName"></asp:TextBox>
            </ItemTemplate>
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Left" Width="300px" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                EXTERNAL NAME
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="ExternalName" runat="server" Text='<%# Eval("External Name") %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Left" Width="200px" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                GROUP CATEGORY
            </HeaderTemplate>
            <ItemTemplate>
                <center> <asp:TextBox ID="txtGroupCategory" runat="server" Text='<%# Eval("Group Category") %>' Width="95%"></asp:TextBox></center>
            </ItemTemplate>
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Left" Width="300px" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                MAX USER LIMIT
            </HeaderTemplate>
            <ItemTemplate>
                <center> <asp:TextBox ID="UserLimit" runat="server" Text='<%# Eval("Maximum Number Users") %>' Width="50px"></asp:TextBox></center>
            </ItemTemplate>
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Center" Width="50px" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                USER COUNT
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Label ID="UserCount" runat="server" Text='<%# Eval("User Count") %>' Width="25%"></asp:Label>
            </ItemTemplate>
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Center" Width="25px" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                DELETE GROUP
            </HeaderTemplate>
            <ItemTemplate>
                <asp:LinkButton ID="Button1" runat="server" CommandArgument='<%# Eval("Role Name") %>' CommandName="DeleteRole" OnClientClick="return confirm('Are you sure?')" OnCommand="DeleteRole" Text="Delete" ToolTip="Click to delete this role." />
            </ItemTemplate>
            <HeaderStyle Width="100px" />
            <ItemStyle HorizontalAlign="Center" Width="25px" />
        </asp:TemplateField>
    </Columns>
    <RowStyle CssClass="gvRowStyle" />
    <AlternatingRowStyle CssClass="gvAlternateRowStyle" />
    <SelectedRowStyle CssClass="gvSelected" />
    <HeaderStyle CssClass="gvHeader" />
    <EditRowStyle CssClass="gvEdit" />
</asp:GridView>
<div class="space"></div>
<div class="row">
    <div class="col-md-12" style="float: none; width: 415px;">
        <div class="col-md-9">
            <asp:Button ID="ApplyChanges" runat="server" ToolTip="Click to apply changes to roles" Text="Save"
                OnClick="ApplyChanges_Click" CssClass="btn btn-primary" OnClientClick="if (!Validate()) {return false;}"></asp:Button>
            <asp:Button ID="btnDeleteSelected" runat="server" OnClick="btnDeleteSelected_Click"
                OnClientClick="return confirm('DELETE selected ROLE(S)?');" Text="Delete"
                ToolTip="DELETE selected ROLES." CssClass="btn btn-primary"></asp:Button>
            <asp:Label ID="lblBadChars" runat="server"></asp:Label>
        </div>
    </div>
</div>
<%-- jquery js --%>
<uc4:jquery ID="jquery1" runat="server" />
<%-- check all checkboxes javascript --%>
<uc1:js ID="js1" runat="server" />

<script src="../../Content/Script/jquery.v1.9.1.js"></script>
<script src="../../Content/Script/bootstrap.js"></script>
<script src="../../Content/Script/bootstrap-dialog.min.js"></script>
<link href="../../Content/Style/bootstrap-dialog.min.css" rel="stylesheet" />
<script type="text/javascript">

    var lblBadChars = "<%= ConfigurationManager.AppSettings["BadCharactersInFriendlyName"].ToString() %>";
    var badChars = lblBadChars.replace(/,/g, "");

    //this function executes when edit individual cell
    $(".friendlyName").blur(function () {
        var rowNum = $(this).closest('tr').find("td:eq(1)").text();
        var $controlValue = $(this).val();
        CheckValidData($controlValue);
    });

    //$('input.friendlyName').keyup(function () {
    //    console.log("1");
    //    var $controlValue = $(this).val();
    //    CheckValidData($controlValue);
    //});

    //this is fired when click save
    function Validate() {
        try {
            var friendlyNameArray = [];
            //get all the friendly names from all text boxes and add to array
            $('.friendlyName').each(function (i, obj) {
                friendlyNameArray.push(obj.value);
            });

            //creaete one big word by removing all commas
            var oneWordArrayString = friendlyNameArray.join().replace(/,/g, "");
            CheckValidData(oneWordArrayString);
        }
        catch (err) {
            return false;
            var txt = 'Error=>' + err.description;
            showDangerAlert(txt);
        }
    }

    function CheckValidData(oneWordArrayString) {
        //seperate the bad chars into array for speed
        var badCharArray = [];
        for (i = 0; i < oneWordArrayString.length; i++) {
            if (badChars.indexOf(oneWordArrayString[i]) > -1) {
                badCharArray.push(oneWordArrayString[i]);
            }
        }

        //check the 
        if (badCharArray.length > 0) {
            disableLinkButton();
            var msg = 'Friendly name can not contain invalid character(s) like <b>' + badCharArray.join(',') + '</b>';
            showErrorAlert(msg);
            return false;
        }
        else {
            enableLinkButton();
        }

        return true;
    }

    //***************** Alert Messages ******************************// 
    function showErrorAlert(alertMessage) {
        BootstrapDialog.show({
            title: 'Data Entry Issue',
            message: alertMessage,
            type: BootstrapDialog.TYPE_WARNING, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
            closable: true, // <-- Default value is false
            draggable: true, // <-- Default value is false
            buttons: [{
                label: 'OK',
                hotkey: 13, // Keycode of keyup event of key 'A' is 65.
                cssClass: 'btn-warning',
                action: function (dialog) {
                    dialog.close();
                }
            }],
        });
    }

    function enableLinkButton() {
        document.getElementById('<%= ApplyChanges.ClientID %>').disabled = false;
    }

    function disableLinkButton() {
        document.getElementById('<%= ApplyChanges.ClientID %>').disabled = true;
    }

</script>
