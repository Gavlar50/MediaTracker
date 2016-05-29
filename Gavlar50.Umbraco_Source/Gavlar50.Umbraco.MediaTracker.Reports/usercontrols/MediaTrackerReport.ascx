<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MediaTrackerReport.ascx.cs" Inherits="Gavlar50.Umbraco.MediaTracker.Reports.usercontrols.MediaTrackerReport" %>

<div class="form-horizontal">
   <div class="control-group umb-control-group">
      <div class="umb-el-wrap">
         <label class="control-label">Media Id</label>
         <div class="controls controls-row">
            <asp:TextBox runat="server" ID="txtMediaId"></asp:TextBox>
            <asp:Label runat="server" ID="lblMediaError" Visible="False">Invalid media id - please enter a valid number</asp:Label>
            <asp:Button runat="server" ID="btnShowUsages" Text="Tracked For Id" ToolTip="Show usages for selected media" OnClick="btnShowUsages_Click" CssClass="btn btn-primary"/>
            <span>&nbsp;|&nbsp;</span>
            <asp:Button runat="server" ID="btnShowAll" Text="All Tracked" ToolTip="Show usages for all tracked media" OnClick="btnShowAll_Click" CssClass="btn btn-primary"/>
            <span>&nbsp;|&nbsp;</span>
            <asp:Button runat="server" ID="btnShowUnused" Text="Unused" ToolTip="Show unused media" OnClick="btnShowUnused_Click" CssClass="btn btn-danger"/>
            <span>&nbsp;|&nbsp;</span>
            <asp:Button runat="server" ID="btnSizes" Text="By Size" ToolTip="Show media by file size" OnClick="btnShowSizes_Click" CssClass="btn btn-warning"/>
         </div>
      </div>
   </div>
</div>

<asp:Literal runat="server" ID="lblReportHeading"></asp:Literal>
<asp:GridView runat="server" ID="grdMedia" GridLines="None" AutoGenerateColumns="False" EmptyDataText="">
   <Columns>
      <asp:BoundField HeaderText="Page" DataField="ContentName">
         <ItemStyle Width="200px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField HeaderText="Property" DataField="Property">
         <ItemStyle Width="200px"></ItemStyle>
      </asp:BoundField>
      <asp:TemplateField HeaderText="Url">
         <ItemTemplate>
            <asp:HyperLink runat="server" NavigateUrl='<%#Eval("Url") %>' Target="Blank" CssClass="control-label"><%#Eval("Url") %></asp:HyperLink>
         </ItemTemplate>
      </asp:TemplateField>
   </Columns>
</asp:GridView>

<asp:GridView runat="server" ID="grdAllMedia" GridLines="None" AutoGenerateColumns="False" EmptyDataText="">
   <Columns>
      <asp:TemplateField HeaderText="Media">
         <ItemTemplate>
            <asp:HyperLink runat="server" NavigateUrl='<%#Eval("Media") %>' Target="Blank" CssClass="control-label"><%#Eval("Media") %></asp:HyperLink>
         </ItemTemplate>
      </asp:TemplateField>
      <asp:BoundField HeaderText="Page" DataField="ContentName">
         <ItemStyle Width="200px"></ItemStyle>
      </asp:BoundField>
      <asp:BoundField HeaderText="Property" DataField="Property">
         <ItemStyle Width="200px"></ItemStyle>
      </asp:BoundField>
      <asp:TemplateField HeaderText="Url">
         <ItemTemplate>
            <asp:HyperLink runat="server" NavigateUrl='<%#Eval("Url") %>' Target="Blank" CssClass="control-label"><%#Eval("Url") %></asp:HyperLink>
         </ItemTemplate>
      </asp:TemplateField>
   </Columns>
</asp:GridView>

<asp:GridView runat="server" ID="grdUnused" GridLines="None" AutoGenerateColumns="False" EmptyDataText="">
   <Columns>
      <asp:BoundField HeaderText="Name" DataField="Name">
         <ItemStyle Width="200px"></ItemStyle>
      </asp:BoundField>      
      <asp:BoundField HeaderText="Size (kb)" DataField="Size">
         <ItemStyle Width="200px"></ItemStyle>
      </asp:BoundField>      
      <asp:TemplateField HeaderText="Media">
         <ItemTemplate>
            <asp:HyperLink runat="server" NavigateUrl='<%#Eval("Media") %>' Target="Blank"><%#Eval("Media") %></asp:HyperLink>
         </ItemTemplate>
      </asp:TemplateField>   
   </Columns>
</asp:GridView>

<script type="text/javascript">
   $(function() {
      $("#form1 table th").css("text-align", "left");
   });
</script>