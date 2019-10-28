$( document ).ready(function() {

    $(document).on("click", ".change-page" , function() {
        var page = "pages/"+$(this).data("page");
        $('#content').load(page);
    });


    $('#content').load("pages/assets.html");
});