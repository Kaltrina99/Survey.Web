
$(document).ready(function () {
    var wrapper = $(".input_fields_wrap"); //Fields wrapper
    var add_button = $(".add_field_button"); //Add button ID

    var x = 1; //initlal text box count


    $(add_button).click(function (e) { //on add input button click
        e.preventDefault();

        //text box increment
        $(wrapper).append(`<div class="row form-group otheroption"><div class="col-6"><textarea name="otherOptions[${x - 1}]" class="form-control" placeholder="Option"> </textarea></div><button class="remove_field btn btn-sm font-weight-bolder btn-light-danger" style="margin-left:40px;"><i class="la la-remove"></i>Remove</button></div>`); //add input box
        x++;

    });

    $(wrapper).on("click", ".remove_field", function (e) { //user click on remove text

        e.preventDefault();
        $(this).parent('div').remove();
        x--;
        ResetIndexesForExistingOptions();
        ResetIndexesForNewOptions();
    })
});
function ResetIndexesForExistingOptions()
{
    var regex = /(?<=\[)(.+)(?=\])/gm;
    var divs = document.getElementsByClassName("existingopt");
    for (var i = 0; i < divs.length; i++) {
        var area = divs[i].querySelector("textarea");
        var text = divs[i].querySelector("input[type=hidden]");

        var atname = area.getAttribute("name");
        atname = atname.replace(regex, `${i}`);
        area.setAttribute("name", atname);

        var textname = text.getAttribute("name");
        textname = textname.replace(regex, `${i}`);
        text.setAttribute("name", textname);
    }
}
function ResetIndexesForNewOptions()
{
    var regex = /(?<=\[)(.+)(?=\])/gm;
    var divs =document.getElementsByClassName("otheroption");
    for (var i = 0; i < divs.length; i++)
    {
        var area = divs[i].querySelector("textarea");
        var atname = area.getAttribute("name");
        atname = atname.replace(regex,`${i}`);
        area.setAttribute("name", atname);
    }
}
var backbutton = document.getElementById("backtolistbutton");
backbutton.addEventListener("click", () => {
    window.history.back();
})