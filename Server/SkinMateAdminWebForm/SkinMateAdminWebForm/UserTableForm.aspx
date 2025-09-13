    <%@ Page Title="Users" Language="C#" MasterPageFile="~/Main.master"
        AutoEventWireup="true" CodeBehind="UserTableForm.aspx.cs"
        Inherits="SkinMateAdminWebForm.UserTableForm" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

 
      <dx:ASPxGridView ID="gvUsers" runat="server"
          DataSourceID="dsUsers"
          KeyFieldName="user_id"
          AutoGenerateColumns="False"
          Width="100%">

  
        <SettingsEditing Mode="PopupEditForm"/>
        <SettingsPopup>
          <EditForm Modal="true" Width="600px"
                    HorizontalAlign="WindowCenter"
                    VerticalAlign="WindowCenter" />
        </SettingsPopup>
        <SettingsPager PageSize="20" />
        <Settings ShowFilterRow="true"
                  VerticalScrollBarMode="Visible"
                  VerticalScrollableHeight="560" />
        <SettingsSearchPanel Visible="true" HighlightResults="true" />
        <Styles>
          <AlternatingRow Enabled="true" />
        </Styles>
        <Paddings Padding="0px" />
        <Border BorderWidth="0px" />
       
        <Columns>
          <dx:GridViewCommandColumn
              ShowNewButtonInHeader="true"
              ShowEditButton="true"
              ShowDeleteButton="true"
              Width="120" />

          <dx:GridViewDataTextColumn FieldName="user_id"
                                     Caption="ID"
                                     ReadOnly="true"
                                     Width="70" />

          <dx:GridViewDataTextColumn FieldName="username"
                                     Caption="Username"
                                     Width="240">
            <PropertiesTextEdit MaxLength="63" NullText="username..." />
          </dx:GridViewDataTextColumn>

 
          <dx:GridViewDataTextColumn FieldName="password"
                                     Caption="Password"
                                     Width="240">
            <PropertiesTextEdit Password="true"
                                MaxLength="63"
                                NullText="password..." />
          </dx:GridViewDataTextColumn>
        </Columns>

      </dx:ASPxGridView>
 
      <asp:SqlDataSource ID="dsUsers" runat="server"
          ConnectionString="<%$ ConnectionStrings:PDB %>"
          SelectCommand="SELECT user_id, username, password FROM dbo.Users ORDER BY user_id DESC"
          InsertCommand="INSERT INTO dbo.Users(username, password) VALUES(@username, @password)"
          UpdateCommand="UPDATE dbo.Users SET username=@username, password=@password WHERE user_id=@user_id"
          DeleteCommand="DELETE FROM dbo.Users WHERE user_id=@user_id"
          OnUpdated="dsUsers_Updated"
          OnDeleted="dsUsers_Deleted"
          OnInserted="dsUsers_Inserted">
        <InsertParameters>
          <asp:Parameter Name="username" Type="String" />
          <asp:Parameter Name="password" Type="String" />
        </InsertParameters>
        <UpdateParameters>
          <asp:Parameter Name="username" Type="String" />
          <asp:Parameter Name="password" Type="String" />
          <asp:Parameter Name="user_id" Type="Int32" />
        </UpdateParameters>
        <DeleteParameters>
          <asp:Parameter Name="user_id" Type="Int32" />
        </DeleteParameters>
      </asp:SqlDataSource>

    </asp:Content>
