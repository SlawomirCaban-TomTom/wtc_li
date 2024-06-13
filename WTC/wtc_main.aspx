<%@ Page Title="WTC 5.0" Language="C#" MasterPageFile="~/Master_Page.Master" AutoEventWireup="true"
    CodeBehind="wtc_main.aspx.cs" Inherits="TomTom_Info_Page.WTC.wtc_main" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style type="text/css">
        .style1 {
            width: 329px;
        }

        .auto-style2 {
            width: 151px;
        }

        .auto-style3 {
            width: 321px;
        }

        .bgimg {
            background-image: url('../img/wtc.jpg');
            background-size: cover;
        }

        .auto-style4 {
            width: 373px;
        }

        .auto-style5 {
            width: 644px;
        }

        .auto-style6 {
            width: 332px;
        }
    </style>
    <div class="bgimg">
        <asp:Label runat="server" Text="Work Time Controller - WTC 5.0" Font-Bold="True" ForeColor="#996633"></asp:Label><br />
        <asp:Menu Visible="false" ID="Menu_mng" runat="server" BackColor="#F7F6F3" DynamicHorizontalOffset="2" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#7C6F57" Orientation="Horizontal" StaticSubMenuIndent="10px">
            <DynamicHoverStyle BackColor="#7C6F57" ForeColor="White" />
            <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            <DynamicMenuStyle BackColor="#F7F6F3" />
            <DynamicSelectedStyle BackColor="#5D7B9D" />
            <Items>
                <asp:MenuItem NavigateUrl="~/WTC/wtc_main.aspx" Text="WTC Main Page" Value="WTC Main Page"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~/WTC/history.aspx" Text="History" Value="1"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~/WTC/edit_project.aspx" Text="Manage Existing PlanningId" Value="Manage Existing PlanningIds"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~/WTC/new_project.aspx" Text="Add New PlanningId" Value="Add New PlanningId"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~/WTC/back_dated_entry.aspx" Text="Back dated entry" Value="Back dated entry"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~/WTC/leaves_history.aspx" Text="Leaves History" Value="Leaves History"></asp:MenuItem>
               
            </Items>
            <StaticHoverStyle BackColor="#7C6F57" ForeColor="White" />
            <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            <StaticSelectedStyle BackColor="#5D7B9D" />
        </asp:Menu>
        <asp:Menu Visible="true" ID="menu_std" runat="server" BackColor="#F7F6F3" DynamicHorizontalOffset="2" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#7C6F57" Orientation="Horizontal" StaticSubMenuIndent="10px">
            <DynamicHoverStyle BackColor="#7C6F57" ForeColor="White" />
            <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            <DynamicMenuStyle BackColor="#F7F6F3" />
            <DynamicSelectedStyle BackColor="#5D7B9D" />
            <Items>
                <asp:MenuItem NavigateUrl="~/WTC/wtc_main.aspx" Text="WTC Main Page" Value="WTC Main Page"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~/WTC/history.aspx" Text="History" Value="1"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~/WTC/back_dated_entry.aspx" Text="Back dated entry" Value="Back dated entry"></asp:MenuItem>
                <asp:MenuItem NavigateUrl="~/WTC/leaves_history.aspx" Text="Leaves History" Value="Leaves History"></asp:MenuItem>
           </Items>
            <StaticHoverStyle BackColor="#7C6F57" ForeColor="White" />
            <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            <StaticSelectedStyle BackColor="#5D7B9D" />
        </asp:Menu>
        <br />
         <script language="javascript" type="text/javascript">
             function openCalendar1() {

                 window.open('calendar.aspx?Ctlid=<%=tb_start_date.ClientID %>', 'Calendar', 'scrollbars=no,resizable=no,width=450,height=250');
          return false;
      }
      function openCalendar2() {

          window.open('calendar.aspx?Ctlid=<%=tb_end_date.ClientID %>', 'Calendar', 'scrollbars=no,resizable=no,width=450,height=250');
                 return false;
             }

         </script>
       <table>
            <tr>
                <td>

                    <strong>

                        <asp:Label runat="server" ID="lbl_name" Text=""></asp:Label>
                    </strong>
                </td>

                <td class="style1"></td>


                <td><strong>
                    <asp:Label runat="server" ID="lbl_date" Text=""></asp:Label>
                </strong>
                </td>

            </tr>
            <tr>

                <td>
                    <asp:Button runat="server" Text="Start work" Height="70px" Width="131px"
                        ID="btn_start" OnClick="btn_start_Click"></asp:Button></td>

                <td class="style1"></td>

            </tr>
      

        </table>
        <br />
      
        <asp:Panel runat="server" ID="report_time" Visible="False">
            <br />
            <hr />
            <br />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">

                <ContentTemplate>

                   <table>
                        <tr>
                            <td class="auto-style3"></td>

                            <td class="auto-style2">
                                <asp:Label runat="server" Text="Total work time: "></asp:Label></td>
                            <td>
                                <asp:Label runat="server" ID="lbl_total_work_time" Text=""></asp:Label>


                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            <table>
                <tr>
                    <td class="auto-style3"></td>

                    <td class="auto-style2">
                        <asp:Label runat="server" Text="Total reported time: "></asp:Label></td>
                    <td>
                        <asp:Label ID="lbl_total_reported" runat="server"></asp:Label>

                    </td>
                </tr>
                <tr>
                    <td class="auto-style3"></td>

                    <td class="auto-style2">
                        <asp:Label runat="server" Text="Start Date "></asp:Label></td>
                    <td>
                        <asp:TextBox ID="tb_start_date" runat="server" MaxLength="7" Width="49px" Height="21px"></asp:TextBox>

                    </td>
                </tr>
                 <tr>
     <td class="auto-style3"></td>

     <td class="auto-style2">
         <asp:Label runat="server" Text="End Date "></asp:Label></td>
     <td>
         <asp:TextBox ID="tb_end_date" runat="server" MaxLength="7" Width="49px" Height="21px"></asp:TextBox>

     </td>
 </tr>
            </table>
            <table>
<tr>
<td>
 <asp:Button ID="Button_ref" runat="server" OnClick="Buttonref_Click" Text="Reset Filter!" />
</td>
</tr>
                <tr>
                    <td class="style2">
                        <asp:Label runat="server" Text="Search PlanningId:"></asp:Label></td>
                    <td class="auto-style4">
                        <asp:TextBox ID="tb_mask" runat="server" Width="162px" BackColor="#FFFF99" BorderColor="Yellow"></asp:TextBox>
                        <asp:Button runat="server" Text="GO!" OnClick="Unnamed6_Click" ID="btn_go"></asp:Button></td>
                </tr>
              
                <tr>
                    <td class="style2">
                        <asp:Label runat="server" Text="Planning Id:"></asp:Label>
                    </td>
                    <td class="auto-style4">
                        <asp:DropDownList ID="ddl_planning_id" runat="server" AutoPostBack="True" Height="28px" OnSelectedIndexChanged="ddl_planning_SelectedIndexChanged" Width="370px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Activity:"></asp:Label>
                    </td>
                    <td class="auto-style4">
                        <asp:DropDownList ID="ddl_activity" runat="server" AutoPostBack="True" Height="28px" OnSelectedIndexChanged="ddl_activity_SelectedIndexChanged" Width="370px">
                        </asp:DropDownList>
                    </td>
                </tr>

                <tr>
                    <td></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Region:" ID="lbl_region" Visible="true"></asp:Label>
                    </td>
                    <td class="auto-style4">
                        <asp:DropDownList ID="ddl_region" runat="server" AutoPostBack="True" Visible="true" Height="28px"  Width="370px">
                        </asp:DropDownList>
                    </td>
                </tr>
                 <tr>
                    <td>
                        <asp:Label runat="server" Text="Sub Region:" ID="lbl_sub_region" Visible="true"></asp:Label>
                    </td>
                    <td class="auto-style4">
                        <asp:DropDownList ID="ddl_sub_region" runat="server" AutoPostBack="True" Visible="true" Height="28px"  Width="370px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Country:" ID="lbl_country" Visible="true"></asp:Label>
                    </td>
                    <td class="auto-style4">
                        <asp:DropDownList ID="ddl_country" runat="server" AutoPostBack="True" Visible="true" Height="28px" Width="370px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="Comment:"></asp:Label>
                    </td>
                    <td class="auto-style4">
                        <asp:TextBox ID="tb_desc" runat="server" Height="23px" MaxLength="50" Width="365px"></asp:TextBox>
                    </td>
                </tr>
            </table>

            <br />


            <br />
            <asp:Button runat="server" Text="Report Time" Height="68px" Width="110px"
                OnClick="Unnamed7_Click" ID="bt_report"></asp:Button><br />
            <hr />

            <asp:Button runat="server" Text="Report 35 min break" Height="30px" Width="111px" Font-Bold="True" Font-Size="X-Small" OnClick="Unnamed10_Click" ID="bt_break"></asp:Button><br />

        </asp:Panel>
      
    </div>

    <br />
    <hr />
    <asp:GridView runat="server" ID="gv_reported_time" AutoGenerateColumns="False"
        BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
        CellPadding="4" EnableModelValidation="True" ForeColor="Black"
        Width="831px"
        OnRowDeleting="gvVochByDate_RowCommand">
        <Columns>
           

        </Columns>
        <FooterStyle BackColor="#CCCC99" ForeColor="Black" />
        <HeaderStyle BackColor="#333333" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
        <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" Wrap="False" />
        <SelectedRowStyle BackColor="#CC3333" Font-Bold="True" ForeColor="White" />
    </asp:GridView>


    <br />
    <br />
    <asp:Label ID="lbl_err" runat="server" Font-Bold="True" ForeColor="#FF3300"
        Visible="False"></asp:Label>



</asp:Content>
