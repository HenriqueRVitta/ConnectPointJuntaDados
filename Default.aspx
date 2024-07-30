<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="JuntaDados._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>
        <section class="row" aria-labelledby="aspnetTitle">
            <h1 id="aspnetTitle">Junta Dados de Licitações</h1>
            <p class="lead">Atualiza os dados no banco de dados da ConnecPoint conforme informações do do DB_EDITAL tabela tb_edital.</p>
            <p><a href="https://app.connectpoint.com.br/" class="btn btn-primary btn-md" target="_blank">Sobre &raquo;</a></p>
        </section>

        <div class="row">
            <section class="col-md-4" aria-labelledby="gettingStartedTitle">
                <h2 id="gettingStartedTitle">Total de Licitações</h2>
                <p>
                    <asp:Label class="font-monospace" style="font-size: 24px;" ID="TotalLicitacoes" runat="server" Text=""></asp:Label>
                </p>
                <p>
                    <a class="btn btn-default" href="https://app.connectpoint.com.br/" target="_blank">Sobre &raquo;</a>
                </p>
            </section>
            <section class="col-md-4" aria-labelledby="librariesTitle">
                <h2 id="librariesTitle">Total de Registros Processados</h2>
                <p>
                    <asp:Label class="font-monospace" style="font-size: 24px;" ID="TotalRegistrosProcessados" runat="server" Text=""></asp:Label>
                </p>
            </section>
            <section class="col-md-4" aria-labelledby="hostingTitle">
                <h2 id="hostingTitle">Total Registros Atualizados DB_EDITAL</h2>
                <p>
                    <asp:Label class="font-monospace" style="font-size: 24px;" ID="TotalRegistroDbEdital" runat="server" Text=""></asp:Label>
                </p>
            </section>
        </div>
    </main>

</asp:Content>
