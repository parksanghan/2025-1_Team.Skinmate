<%@ Page Title="Chat Logs" Language="C#" MasterPageFile="~/Main.master"
    AutoEventWireup="true" CodeBehind="UserLogForm.aspx.cs"
    Inherits="SkinMateAdminWebForm.UserLogForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

  <div style="display:flex; gap:12px; align-items:end; margin:8px 0 16px;">
    <dx:ASPxComboBox ID="cbUser" runat="server" Width="220px"
        DataSourceID="dsUsers"
        TextField="username" ValueField="user_id"
        ValueType="System.Int32"
        NullText="All Users"
        IncrementalFilteringMode="Contains"
        EnableCallbackMode="true" />

    <dx:ASPxComboBox ID="cbLogType" runat="server" Width="180px" NullText="All Types">
      <Items>
        <dx:ListEditItem Text="질의응답" Value="질의응답" />
        <dx:ListEditItem Text="진단분석" Value="진단분석" />
        <dx:ListEditItem Text="사용자설정" Value="사용자설정" />
      </Items>
    </dx:ASPxComboBox>

    <dx:ASPxDateEdit ID="deFrom" runat="server" Width="160px" NullText="From (시작일)" />
    <dx:ASPxDateEdit ID="deTo"   runat="server" Width="160px" NullText="To (종료일)" />
    <dx:ASPxButton ID="btnApply" runat="server" Text="적용" AutoPostBack="true" OnClick="btnApply_Click" />
    <dx:ASPxButton ID="btnReset" runat="server" Text="초기화" AutoPostBack="true" OnClick="btnReset_Click" />
  </div>

  <dx:ASPxGridView ID="gvChatLogs" runat="server"
      DataSourceID="dsChatLogs"
      KeyFieldName="chat_id"
      AutoGenerateColumns="False"
      Width="100%">
    <SettingsPager PageSize="20" />
    <Settings ShowFilterRow="false" VerticalScrollBarMode="Visible" VerticalScrollableHeight="520" />
    <SettingsSearchPanel Visible="true" HighlightResults="true" />
    <Styles><AlternatingRow Enabled="true" /></Styles>
    <Paddings Padding="0px" /><Border BorderWidth="0px" />
    <SettingsDetail ShowDetailRow="true" />

   
    <Templates>
      <DetailRow>
        <div style="padding:12px;">
          <h4 style="margin:4px 0;">Message</h4>
          <dx:ASPxMemo ID="mMessage" runat="server" ReadOnly="true" Width="100%" Rows="4"
                       Text='<%# Eval("message") %>' />
          <h4 style="margin:12px 0 4px;">Response</h4>
          <dx:ASPxMemo ID="mResponse" runat="server" ReadOnly="true" Width="100%" Rows="4"
                       Text='<%# Eval("response") %>' />
          <h4 style="margin:12px 0 4px;">DiagnosisResult (JSON)</h4>
          <dx:ASPxMemo ID="mDiag" runat="server" ReadOnly="true" Width="100%" Rows="6"
                       Text='<%# Eval("diagnosis_result") %>' />
        </div>
      </DetailRow>
    </Templates>

    <Columns>
      <dx:GridViewCommandColumn ShowDeleteButton="true" Width="80" />
      <dx:GridViewDataTextColumn FieldName="chat_id" Caption="ChatId" ReadOnly="true" Width="80" />
      <dx:GridViewDataTextColumn FieldName="user_id" Caption="UserId" Width="80" Visible="false" />
      <dx:GridViewDataTextColumn FieldName="username" Caption="Username" Width="180" />
      <dx:GridViewDataTextColumn FieldName="log_type" Caption="LogType" Width="120" />
      <dx:GridViewDataDateColumn FieldName="timestamp" Caption="Timestamp"
                                 PropertiesDateEdit-DisplayFormatString="yyyy-MM-dd HH:mm:ss" Width="170" />

     
      <dx:GridViewDataTextColumn FieldName="message_preview" Caption="Message (preview)" Width="350">
        <DataItemTemplate>
          <div style="max-width:350px; white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
            <%# Eval("message_preview") %>
          </div>
        </DataItemTemplate>
        <CellStyle Wrap="False" />
      </dx:GridViewDataTextColumn>

 
      <dx:GridViewDataTextColumn FieldName="response_preview" Caption="Response (preview)" Width="350">
        <DataItemTemplate>
          <div style="max-width:350px; white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
            <%# Eval("response_preview") %>
          </div>
        </DataItemTemplate>
        <CellStyle Wrap="False" />
      </dx:GridViewDataTextColumn>
    </Columns>
  </dx:ASPxGridView>

 
  <asp:SqlDataSource ID="dsUsers" runat="server"
      ConnectionString="<%$ ConnectionStrings:PDB %>"
      SelectCommand="SELECT user_id, username FROM dbo.Users ORDER BY username" />

 
<asp:SqlDataSource ID="dsChatLogs" runat="server"
    ConnectionString="<%$ ConnectionStrings:PDB %>"
    CancelSelectOnNullParameter="false"
    SelectCommand="SELECT cl.chat_id, cl.user_id, u.username, cl.log_type, cl.message, cl.response, cl.diagnosis_result, cl.[timestamp],
                          REPLACE(REPLACE(REPLACE(cl.message, CHAR(13)+CHAR(10), ' '), CHAR(13), ' '), CHAR(10), ' ') AS message_preview,
                          REPLACE(REPLACE(REPLACE(cl.response, CHAR(13)+CHAR(10), ' '), CHAR(13), ' '), CHAR(10), ' ') AS response_preview
                   FROM dbo.chat_logs AS cl
                   LEFT JOIN dbo.users AS u ON u.user_id = cl.user_id
                   WHERE (@UserId IS NULL OR cl.user_id = @UserId)
                     AND (@LogType IS NULL OR cl.log_type = @LogType)
                     AND (@From IS NULL OR cl.[timestamp] &gt;= @From)
                     AND (@To IS NULL OR cl.[timestamp] &lt; DATEADD(day, 1, @To))
                   ORDER BY cl.chat_id DESC"
    DeleteCommand="DELETE FROM dbo.chat_logs WHERE chat_id=@chat_id">
    <SelectParameters>
      <asp:ControlParameter Name="UserId"  ControlID="cbUser"   PropertyName="Value" Type="Int32"   ConvertEmptyStringToNull="true" />
      <asp:ControlParameter Name="LogType" ControlID="cbLogType" PropertyName="Value" Type="String"  ConvertEmptyStringToNull="true" />
      <asp:ControlParameter Name="From"    ControlID="deFrom"   PropertyName="Value" Type="DateTime" ConvertEmptyStringToNull="true" />
      <asp:ControlParameter Name="To"      ControlID="deTo"     PropertyName="Value" Type="DateTime" ConvertEmptyStringToNull="true" />
    </SelectParameters>
    <DeleteParameters>
      <asp:Parameter Name="chat_id" Type="Int32" />
    </DeleteParameters>
  </asp:SqlDataSource>

</asp:Content>
