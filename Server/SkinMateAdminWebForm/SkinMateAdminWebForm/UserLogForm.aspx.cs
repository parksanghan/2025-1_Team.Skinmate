using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SkinMateAdminWebForm
{
    public partial class UserLogForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            cbUser.Value = null;     // All Users
            cbLogType.Value = null;  // All Types
            deFrom.Value = null;     // from clear
            deTo.Value = null;     // to clear

            gvChatLogs.DataBind();   // 그리드 다시 바인딩
        }

        // 선택적으로 [적용] 버튼에 핸들러를 쓰고 싶다면(필수 아님)
        protected void btnApply_Click(object sender, EventArgs e)
        {
            gvChatLogs.DataBind();
        }
    }
}