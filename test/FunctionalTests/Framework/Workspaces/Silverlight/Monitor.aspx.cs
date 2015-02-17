//---------------------------------------------------------------------
// <copyright file="Monitor.aspx.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Monitor : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        gvMonitor.DataSource = AstoriaTestSilverlightContext.messageHistory;//.OrderByDescending(t => t.OutGoing.MessageID);   
        gvMonitor.DataBind();
    }
    protected void btnClearMessages_Click(object sender, EventArgs e)
    {
        AstoriaTestSilverlightContext.messages.Clear();
    }
    protected void btnClearMessageHistory_Click(object sender, EventArgs e)
    {
        AstoriaTestSilverlightContext.messageHistory.Clear(); 
        gvMonitor.DataSource = AstoriaTestSilverlightContext.messageHistory;//.OrderByDescending(t => t.OutGoing.MessageID);   
        gvMonitor.DataBind();
    }
}
