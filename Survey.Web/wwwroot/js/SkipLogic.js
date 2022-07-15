
var formid = document.getElementById("formid").value;



function addcondition(e) {
    var cond = e.getAttribute("name");
    var z = JSON.parse(cond)
    for (var i = 0; i < z.length; i++) {
        conditions.push(z[i]);
    }
    e.parentNode.removeChild(e);
}

function checkNumber(id, e) {
    var container = e.parentNode.parentNode;
    var evalue = e.value;
    if (id !== null) {
        conditions.forEach(con => {

            if (con.questionId == id) {
                if (eval(`${evalue} ${con.Operator} ${con.value}`) && con.isChecked == false) {
                    con.isChecked = true;
                    var emptycon = document.createElement("div");
                    emptycon.setAttribute("id", `number[${con.questionId}][${con.id}].skip`);
                    var index = document.getElementsByClassName("questiontext").length;

                    container.appendChild(emptycon);
                    $.ajax({
                        url: `/Survey/DisplaySkippedQuestions/${con.id}?index=${index}&number=true`,
                        method: "Get",
                        success: function (data) {
                            emptycon.innerHTML = data;
                            resetQuestionIndexes();
                        },
                        error: function (err) {
                        }
                    })
                }
                if (con.isChecked == true && !(eval(`${evalue} ${con.Operator} ${con.value}`))) {
                    con.isChecked = false;
                    var recon = document.getElementById(`number[${con.questionId}][${con.id}].skip`);
                    recon.remove();
                }
            }
        });
    }
}

function checkCondition(id, e) {
    var container = e.parentNode.parentNode.parentNode.parentNode.parentNode;
    var index = document.getElementsByClassName("isquestion").length;
    var insert = container.getElementsByClassName("childQuestion")[0];
  
    else {
        $.ajax({
            url: `/Survey/DisplaySkippedQuestions/${id}?index=${index}`,
            method: "Get",
            success: function (data) {
                insert.innerHTML = data;
                resetQuestionIndexes();
            },
            error: function (err) {

            }
        })
    }
}

function checkconditionmulti(id, e) {
    var name = e.getAttribute("name");
    var insert = document.getElementById(name);
    var index = document.getElementsByClassName("questiontext").length;
   
   

    if (e.checked && id != null) {
        $.ajax({
            url: `/Survey/DisplaySkippedQuestions/${id}?index=${index}`,
            method: "Get",
            success: function (data) {
                insert.innerHTML = data;
                resetQuestionIndexes();
            },
            error: function (err) {

            }
        })
    }

    else {
        insert.innerHTML = "";
        resetQuestionIndexes();
    }

}

function resetQuestionIndexes() {
    var regex = /(?<=Model\[)(.)(?=].)/g;
    var inputcontainers = document.getElementsByClassName("isquestion");
    for (var i = 0; i < inputcontainers.length; i++) {
        var inputlist = inputcontainers[i].querySelectorAll("input[type = text], input[type = hidden], input[type = checkbox], input[type = number],input[type = radio],textarea");
        for (var j = 0; j < inputlist.length; j++) {

            var oldname = inputlist[j].getAttribute("name");
            nameAttr = oldname.replace(regex, `${i}`);
            inputlist[j].setAttribute("name", nameAttr);

            if (inputlist[j].type == "checkbox") {
                var insert = document.getElementById(oldname);
                if ( insert !== null ) {
                    insert.setAttribute("id", nameAttr);

                }
            }

        }

    }
}

function translateQuestions(questions) {
    var arabic = /[\u0600-\u06FF]/;
   
    for (var i = 0; i < questions.questionTranslations.length; i++) {
        //perkthe pytjen
        var question = document.querySelectorAll(`[data-val-question='${questions.questionTranslations[i].id}']`)[0];
        if (typeof question !== "undefined" && question !== null) {
            question.innerText = questions.questionTranslations[i].translation;
            var con = question.parentNode;
            con.querySelector(".questiondescription").innerHTML = questions.questionTranslations[i].description;

            if(arabic.test(questions.questionTranslations[i].translation))
            {
                

                question.parentNode.classList.add("rtl");
            }
            else {
                question.parentNode.classList.remove("rtl");
            }
            if (questions.questionTranslations[i].optionTranslations.length > 0) {
                var container = question.parentNode.parentNode;
                console.log(container);
                var options = container.querySelectorAll(".option-text");
                for (var j = 0; j < questions.questionTranslations[i].optionTranslations.length; j++)
                    options[j].firstChild.nodeValue = questions.questionTranslations[i].optionTranslations[j];

            }
        }
    }

};
