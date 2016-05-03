<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Events.aspx.cs" Inherits="MillimanSupport.Events" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <style type="text/css">
        div.RadScheduler_[Default] .rsMonthView .rsTodayCell
        {
        background-color: #CCFF00;
        color: #000;
        border: 1px solid #000;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
    <telerik:RadSkinManager ID="QsfSkinManager" runat="server" ShowChooser="false" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadScheduler1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadScheduler1" LoadingPanelID="RadAjaxLoadingPanel1">
                    </telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
    </telerik:RadAjaxLoadingPanel>

    <div style="width:100%;height:100%">
        <telerik:RadScheduler runat="server" ID="Schedule" Width="100%" DayStartTime="08:00:00"
            DayEndTime="17:00:00" TimeZoneOffset="03:00:00"
            DataKeyField="ID" DataSubjectField="Subject" DataStartField="Start" DataEndField="End"
            DataRecurrenceField="RecurrenceRule" DataRecurrenceParentKeyField="RecurrenceParentId"
            DataReminderField="Reminder" Height="100%" ReadOnly="True" BorderStyle="Solid" BorderColor="#CCCCCC" SelectedView="MonthView" AppointmentStyleMode="Default">
            <AdvancedForm Modal="true" EnableTimeZonesEditing="false"></AdvancedForm>
            <TimelineView UserSelectable="false"></TimelineView>
            <TimeSlotContextMenuSettings EnableDefault="true"></TimeSlotContextMenuSettings>
            <AppointmentContextMenuSettings EnableDefault="true"></AppointmentContextMenuSettings>
            <Reminders Enabled="true"></Reminders>
            
            <MonthView AdaptiveRowHeight="false" />
            
        </telerik:RadScheduler>
    </div>

    </form>
</body>
</html>
