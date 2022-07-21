var form = document.querySelector("#addquestionform");
var backbutton = document.getElementById("backtolistbutton");
var selectquestiontype = document.querySelectorAll(".questiontype input[type=radio]");
var optioninsertdiv = document.querySelector(".questionoptions");
var questiontextwarning = document.getElementById("questiontextwarning");
var generalerror = document.getElementById("generalerror");

if (selectquestiontype != null) {
    selectquestiontype.forEach(element => {
        element.addEventListener("change", () => {
            if (element.classList.contains("selectquestionradio")) {

                optioninsertdiv.style = "display:flex";
            }
            else {
                optioninsertdiv.style = "display:none";
            }
        })
    })
}


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

    $(wrapper).on("click", ".remove_field", function (e) {

        e.preventDefault();
        $(this).parent('div').remove();
        x--;
        resetOptionIndexes();
    })
});

function resetOptionIndexes() {
    var options = document.querySelectorAll(".otheroption");
    for (var i = 0; i < options.length; i++) {
        var opinput = options[i].querySelector("input[type=text]");
        opinput.setAttribute("name", `otherOptions[${i}]`);
    }
}


form.addEventListener("submit", e => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const formProps = Object.fromEntries(formData);
    var questionlabel = document.getElementsByName("Question.QuestionText")[0];
    var radiobuttoncheck = document.querySelector('input[name="FieldType"]:checked');
    if (questionlabel.value == null || questionlabel.value.length == 0) {
        questiontextwarning.innerHTML = "You need to add a question label";
    }
    else if (radiobuttoncheck == null) {
        document.getElementById("questionfieldtypewarning").innerHTML = "You you need to select a field type";
    }
    else {
        console.log(formProps);
        $.ajax({
            url: "/Survey/Designer",
            method: "POST",
            data: formProps,
            success: function () {
                window.location.reload();

            },
            error: function (err) {

                document.getElementById("generalerror").innerHTML = err.responseText;
            }
        });
    }

});

function deleteQuestion(question, id) {


    if (confirm("Are you sure you wanna delete this question?")) {

        $.ajax({
            url: `/Survey/DeleteQuestion/${id}`,
            method: "Get",
            success: function () {
                question.parentNode.parentNode.parentNode.remove();
                resetOrderInput();
            },
            error: function (err) {
                generalerror.innerHTML = e.messsage;
            }
        });
    }


}
function resetOrderInput() {
    var questionorders = document.getElementsByClassName("questionorder");
    for (var indesh = 0; questionorders.length > indesh; indesh++) {
        questionorders[indesh].querySelector(".questionid").setAttribute("name", `OriginalQuestion[${indesh}].Id`);
        questionorders[indesh].querySelector(".questionorderid").setAttribute("name", `OriginalQuestion[${indesh}].QuestionOrder`);
    }
}
function resetQuestionOrder() {
    var questionorders = document.getElementsByClassName("questionorder");

    document.getElementById("saveorderbutton").style = "display:block";
    for (var indesh = 0; questionorders.length > indesh; indesh++) {
        questionorders[indesh].querySelector(".questionorderid").value = indesh;
    }
}

document.getElementById("orderform").addEventListener("submit", x => {
    x.preventDefault();
    var form = x.target;
    const formData = new FormData(form);
    const formProps = Object.fromEntries(formData);
    $.ajax({
        url: "/Survey/OrderQuestions",
        method: "POST",
        data: formProps,
        success: function () {
            document.getElementById("saveorderbutton").style = "display:none";


        },
        error: function (err) {


        }
    });
})