<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Santander.aspx.cs" Inherits="Bancos_Santander" Title="Untitled Page" %>

<%@ Register Assembly="Boleto.Net" Namespace="BoletoNet" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<cc1:BoletoBancario id="boletoBancario" runat="server" CodigoBanco="33"></cc1:BoletoBancario>
</asp:Content>

