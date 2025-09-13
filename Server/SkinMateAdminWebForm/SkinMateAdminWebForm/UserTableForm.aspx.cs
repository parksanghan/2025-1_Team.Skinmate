using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SkinMateAdminWebForm
{
    public partial class UserTableForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
             
        }
        protected void ShowResult(string op, SqlDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                var msg = $"{op} 실패: {e.Exception.Message}";
                e.ExceptionHandled = true; // 예외는 처리하고
                ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(),
                    $"alert('{msg.Replace("'", "\\'")}');", true);
            }
            else
            {
                // 성공 시 그리드 즉시 리바인딩 → 화면에 바로 반영
                gvUsers.DataBind();
            }
        }

 

        protected void dsUsers_Inserted(object sender, SqlDataSourceStatusEventArgs e)=>ShowResult("Insert", e);



        protected void dsUsers_Deleted(object sender, SqlDataSourceStatusEventArgs e)=>ShowResult("Delete", e); 

        protected void dsUsers_Updated(object sender, SqlDataSourceStatusEventArgs e)=>ShowResult("Update", e);
   

        //protected void dsUsers_Inserted(object sender, SqlDataSourceStatusEventArgs e) => ShowResult("Insert", e);
        //protected void dsUsers_Updated(object sender, SqlDataSourceStatusEventArgs e) => ShowResult("Update", e);
        //protected void dsUsers_Deleted(object sender, SqlDataSourceStatusEventArgs e) => ShowResult("Delete", e);

    }
}