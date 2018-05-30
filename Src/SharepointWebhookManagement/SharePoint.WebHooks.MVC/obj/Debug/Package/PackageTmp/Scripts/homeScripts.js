window.WebhookWeb = window.WebhookWeb || {};
window.WebhookWeb.HomeScripts = window.WebhookWeb.HomeScripts || {};


WebhookWeb.HomeScripts = new (function () {

    this.ShowLoading = function () {
        var container = document.getElementById("myModal");
        container.style.display = "block"
    }

    this.HideLoading = function () {
        var container = document.getElementById("myModal");
        container.style.display = "none";
        var container = document.getElementById("result");
        container.style.display = "none";
    }

    this.ReoladPage = function () {
        window.location.href = "/Home/Index";
    }
});

$(document).ready(function () {

    WebhookWeb.HomeScripts.HideLoading();

    $("#btnCreate").click(function (e) {

        e.preventDefault();
        WebhookWeb.HomeScripts.ShowLoading();
        $.post("/Home/Create",
                {
                    selectedSharePointList: $('#ListDropDown option:selected').val(),
                    notificationUrl: $("#notificationUrlVal").val()
                })
                .done(function () {
                    var loadingContainer = document.getElementById("loading");
                    loadingContainer.style.display = "none";
                    var container = document.getElementById("result");
                    container.style.display = "block";
                    document.getElementById("msgOperation").innerHTML = "Se ha creado la suscripción de forma correcta";
                })
                .fail(function (error) {
                    var loadingContainer = document.getElementById("loading");
                    loadingContainer.style.display = "none";
                    var container = document.getElementById("result");
                    container.style.display = "block";
                    document.getElementById("msgOperation").innerHTML = "Error al crear la suscripción" + error;
                });
    });
    $("#btnDelete").click(function (e) {

        e.preventDefault();
        WebhookWeb.HomeScripts.ShowLoading();
        $.post("/Home/Delete",
                {
                    id: $("#hiddenId").val(),
                    listId: $("#hiddenResource").val()
                })
                .done(function () {
                    var loadingContainer = document.getElementById("loading");
                    loadingContainer.style.display = "none";
                    var container = document.getElementById("result");
                    container.style.display = "block";
                    document.getElementById("msgOperation").innerHTML = "Se ha borrado la suscripción de forma correcta";
                })
                .fail(function (error) {
                    var loadingContainer = document.getElementById("loading");
                    loadingContainer.style.display = "none";
                    var container = document.getElementById("result");
                    container.style.display = "block";
                    document.getElementById("msgOperation").innerHTML = "Error al eliminar la suscripción" + error;
                });
    });
});