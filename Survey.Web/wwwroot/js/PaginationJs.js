$(function () {
    $("#pageSize").change(function () {
        $("#form1").submit();
    });
    var pagbuttons = document.getElementsByClassName("paginationbutton");
    
    for (var i = 0; i < pagbuttons.length; i++)
    {
        pagbuttons[i].addEventListener("click", (e) => {
            e.preventDefault;
            var target = e.target;
            var li = target.getAttribute("data-val-link");
            $("#form1").attr('action', li).submit();
        })
    }
});