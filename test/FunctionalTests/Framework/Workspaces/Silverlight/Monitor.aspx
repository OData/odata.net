<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Monitor.aspx.cs" Inherits="Monitor"
    EnableViewState="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Astoria Silverlight Testing Monitor</title>
    <link href="css/SoberTable.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="position: absolute; top: 5%; left: 5%; width: 90%;">
        <h3>
            Astoria Silverlight Test Monitor
        </h3>
        <asp:Button runat="server" ID="btnClearMessages" Text="Clear Messages" 
            onclick="btnClearMessages_Click" />
        <asp:Button runat="server" ID="btnClearMessageHistory" 
            Text="Clear Message History" onclick="btnClearMessageHistory_Click" />
        <asp:GridView runat="server" ID="gvMonitor" CssClass="generic_data_table" AutoGenerateColumns="False">
            <Columns>
                <asp:TemplateField HeaderText="LTM">
                    <ItemTemplate>
                        MessageID :
                        <%#DataBinder.Eval(Container.DataItem,"Incoming.MessageID")     %><br />
                        InstanceID :
                        <%#DataBinder.Eval(Container.DataItem, "Incoming.InstanceID")%><br />
                        Method:
                        <%#DataBinder.Eval(Container.DataItem, "Incoming.Method")%><br />
                        Parameter1:
                        <%#DataBinder.Eval(Container.DataItem, "Incoming.Parameter1")%><br />
                        Parameter2:
                        <%#DataBinder.Eval(Container.DataItem, "Incoming.Parameter2")%><br />
                        Parameter3:
                        <%#DataBinder.Eval(Container.DataItem, "Incoming.Parameter3")%><br />
                        Parameter4:
                        <%#DataBinder.Eval(Container.DataItem, "Incoming.Parameter4")%><br />
                        ReturnValue:
                        <%#DataBinder.Eval(Container.DataItem, "Incoming.ReturnValue")%><br />
                        Exception
                        <%#DataBinder.Eval(Container.DataItem, "Incoming.Exception")%><br />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Silverlight">
                    <ItemTemplate>
                        MessageID :
                        <%#DataBinder.Eval(Container.DataItem, "OutGoing.MessageID")%><br />
                        InstanceID :
                        <%#DataBinder.Eval(Container.DataItem, "OutGoing.InstanceID")%><br />
                        Method:
                        <%#DataBinder.Eval(Container.DataItem, "OutGoing.Method")%><br />
                        Parameter1:
                        <%#DataBinder.Eval(Container.DataItem, "OutGoing.Parameter1")%><br />
                        Parameter2:
                        <%#DataBinder.Eval(Container.DataItem, "OutGoing.Parameter2")%><br />
                        Parameter3:
                        <%#DataBinder.Eval(Container.DataItem, "OutGoing.Parameter3")%><br />
                        Parameter4:
                        <%#DataBinder.Eval(Container.DataItem, "OutGoing.Parameter4")%><br />
                        ReturnValue:
                        <%#DataBinder.Eval(Container.DataItem, "OutGoing.ReturnValue")%><br />
                        Exception
                        <%#DataBinder.Eval(Container.DataItem, "OutGoing.Exception")%><br />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    </form>
</body>
</html>
