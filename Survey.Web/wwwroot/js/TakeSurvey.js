var selectLanguage = document.getElementById("selectLanguageId");
var jsonelement = document.getElementById("data-json-skip");
var rawjson = jsonelement.getAttribute("name")
var currentSurveyData = document.getElementById("currentsurveyversion");
var form_id = document.getElementById("form_id").value;

//Initial Functions
{
    //check if the connection is valid
    function checkConnection() {
        return fetch("/Survey/EndPointCheck/", {
            method: "post"
        }).then(x => {
            var ur = new URL(x.url);
            if (x.status != 200 || ur.pathname == "/offline.html") {
                return false;
            } else {
                return true
            }
        })
            .catch(z => {
                return false;
            });

    }
    async function loadFilesForDraft(data) {
        const formData = new FormData();
        for (var pair of data.entries()) {
            var filein = document.getElementsByName(`${pair[0]}`)[0];
            if (filein != null && filein.type == "file") {
                var key = filein.getAttribute("data-val-savedfiles");
                pair[1] = key;

            }
            formData.append(pair[0], pair[1]);
        }

        return formData;
    }

    async function autoSaveForm() {
        var form = document.getElementById("SurveyForm");
        var db = new Localbase("db");
        const formData = new FormData(form);
        const realdata = await loadFilesForDraft(formData)
        const formProps = Object.fromEntries(realdata);
        const data = await db.collection("Surveys").doc(form_id).get();
        if (data == null) {
            await db.collection("Surveys").doc(form_id).set(formProps);
        }
        else {
            await db.collection("Surveys").add(formProps, form_id);
        }

    }
    async function deleteAutoSavedSurvey() {
        let db = new Localbase('db');
        var data = await db.collection("Surveys").doc(form_id).get();
        for (var key in data) {
            if (key.includes("].File") && data[key].trim() != "") {

                await db.collection("Files").doc(data[key]).delete();

            }

            //formData.append(key, data[key]);
        }
        await db.collection("Surveys").doc(form_id).delete()
    }
    async function restoreAutoSavedSurvey() {
        let db = new Localbase('db');
        var data = await db.collection("Surveys").doc(form_id).get();
        for (var key in data) {
            var question = document.getElementsByName(key);
            for (var i = 0; i < question.length; i++) {
                if (question[i] != null) {
                    if (question[i].type == "radio" && question[i].value == data[key]) {
                        question[i].checked = true;
                    }
                    else if (question[i].type == "checkbox" && data[key] == "true") {
                        question[i].checked = true;
                    }
                    else if (question[i].type == "file") {
                        var currentinput = question[i];
                        var container = currentinput.parentNode;
                        currentinput.setAttribute("data-val-savedfiles", data[key]);
                        if (data[key].trim() == "") {
                            continue;
                        }
                        var savedfile = await db.collection("Files").doc(data[key]).get();
                        var { name: fileName, size } = savedfile;
                        // Convert size in bytes to kilo bytes
                        var fileSize = (size / 1000).toFixed(2);
                        var fileNameAndSize = `  <div class="file-item file_${data[key]}"><span>${fileName}</span> <span>${fileSize} KB</span><span class="remove-file-input" name="${data[key]}">x</span></div>`;
                        container.querySelector('.file-list').innerHTML += fileNameAndSize;
                        var removefilebutton = container.querySelectorAll(".remove-file-input");
                        //remove Download Event Listener
                        {
                            removefilebutton.forEach(button => {
                                button.addEventListener("click", z => {
                                    var button = z.target;
                                    var n = button.getAttribute("name");
                                    let db = new Localbase('db');
                                    db.collection("Files").doc(n).delete().then(x => {
                                        button.parentNode.remove();
                                        currentinput.value = "";
                                        currentinput.setAttribute("data-val-savedfiles", "");
                                    });

                                })
                            })
                        }

                    }
                    else {
                        question[i].value = data[key];
                    }
                    var form = question[i].closest("fieldset");
                    if (form != null) {
                        form.style = "display:block";
                        form.classList.add("activeform");
                        form.disabled = false;
                    }
                }
            }
        }
    }
    async function setSruvey() {
        let db = new Localbase('db');
        var all = await db.collection("Surveys").doc(form_id).get();
        if (all != null && all.length != 0) {
            Swal.fire({
                title: "Unsaved Record?", text: "There is one unsaved record, would you like to restore it?", icon: "warning",
                showCancelButton: true,
                allowOutsideClick: false,
                confirmButtonText: "Yes, Load Record"
            }).then(async function (result) {
                if (!result.isDismissed) {
                    await restoreAutoSavedSurvey();
                }
                else {

                    await deleteAutoSavedSurvey();

                }
            });
        }

    }
    var autosaveinterval;
    autosaveinterval = setInterval(autoSaveForm, 10000);
    setSruvey();
    if (rawjson != "") {
        var jsonSkip = JSON.parse(rawjson);
        jsonelement.remove();
    }
    //reset indexes for question inputs
    function resetIndexesForQuestions() {
        var regex = /(?<=Questions\[)(\d*)/mg;
        var fields = document.querySelectorAll(".activeform");
        for (var i = 0; i < fields.length; i++) {
            var inputlist = fields[i].querySelectorAll(".questionInput");
            for (var z = 0; inputlist.length > z; z++) {
                var oldname = inputlist[z].getAttribute("name");
                nameAttr = oldname.replace(regex, `${i}`);
                inputlist[z].setAttribute("name", nameAttr);
            }

        }
    }
}

//Translate Question
{
    selectLanguage.addEventListener("change", (e) => {
        var selectedlang = e.target.value;
        translateQuestions(selectedlang);

    })

    function translateQuestions(lang) {
        var arabic = /[\u0600-\u06FF]/;
        var questions = document.querySelectorAll(".isquestion");
        questions.forEach(x => {
            var translation = x.querySelectorAll(".translation");
            translation.forEach(z => {

                if (z.classList.contains(`${lang}-translation`)) {

                    z.classList.remove("disabled");
                    z.classList.add("activet");
                } else {
                    z.classList.remove("activet");
                    z.classList.add("disabled");
                }
            })
            var questiontext = x.querySelector(".questiontext span.activet");
            if (questiontext != null) {
                if (arabic.test(questiontext.innerHTML)) {
                    x.classList.add("rtl")
                } else {
                    x.classList.remove("rtl")

                }
            }
        });
    }
}


//SkipLogic
{
    function showSkip(field, childid) {
        var conditionType = field.getAttribute("data-val-ctype");
        var isFilled = false
        if (conditionType == 0) {
            var skiploglist = jsonSkip.filter(ob => { return ob.childquestion === childid });
            for (var i = 0; i < skiploglist.length; i++) {
                if (skiploglist[i].isChecked) {
                    isFilled = true;
                    break;
                }
            }
        }
        else {
            var skiploglist = jsonSkip.filter(ob => { return ob.childquestion === childid });
            for (var i = 0; i < skiploglist.length; i++) {
                if (!skiploglist[i].isChecked) {
                    isFilled = false;
                    break;
                }
                isFilled = true;

            }

        }
        if (isFilled) {
            field.style = "display:block";
            field.classList.add("activeform");
            field.disabled = false;
        }
        else {
            field.style = "display:none";
            field.classList.remove("activeform");
            field.disabled = true;
        }
    }
    //One Select
    {
        var selectinputs = document.querySelectorAll(".skp");
        if (selectinputs != null) {
            for (var i = 0; i < selectinputs.length; i++) {
                selectinputs[i].addEventListener("change", (x) => {
                    var pid = x.target.getAttribute("data-val-q");
                    for (var j = 0; j < jsonSkip.length; j++) {
                        if (jsonSkip[j].parentquestion == pid) {
                            var con = jsonSkip[j];
                            var fieldset = document.querySelector(`[data-val-question='question_${con.childquestion}']`);

                            if (eval(`${x.target.value} ${con.Operator} ${con.Optionid}`)) {
                                jsonSkip[j].isChecked = true;
                            } else {
                                jsonSkip[j].isChecked = false;
                            }
                            showSkip(fieldset, con.childquestion)

                        }
                    }

                })
            }
        }
    }
    //Number Question 
    {
        var numberinput = document.querySelectorAll(".skpn");
        if (numberinput != null) {
            for (var i = 0; numberinput.length > i; i++) {
                numberinput[i].addEventListener("input", (x) => {
                    var pid = x.target.getAttribute("data-val-q");
                    for (var j = 0; j < jsonSkip.length; j++) {
                        if (jsonSkip[j].parentquestion == pid) {
                            var con = jsonSkip[j];
                            var fieldset = document.querySelector(`[data-val-question='question_${con.childquestion}']`);
                            if (eval(`${x.target.value} ${con.Operator} ${con.value}`)) {
                                jsonSkip[j].isChecked = true;

                            } else {
                                jsonSkip[j].isChecked = false;

                            }
                            showSkip(fieldset, con.childquestion)
                        }
                    }
                })
            }
        }
    }
    //Multi Select
    {
        var multiinputs = document.querySelectorAll(".skpm");
        if (multiinputs != null) {
            for (var i = 0; i < multiinputs.length; i++) {
                multiinputs[i].addEventListener("change", (x) => {
                    var pid = x.target.getAttribute("data-val-q");
                    if (x.target.checked) {
                        for (var j = 0; j < jsonSkip.length; j++) {
                            if (jsonSkip[j].parentquestion == pid) {
                                var con = jsonSkip[j];
                                var opid = x.target.getAttribute("data-val-option");
                                if (eval(`${opid} ${con.Operator} ${con.Optionid}`)) {
                                    var fieldset = document.querySelector(`[data-val-question='question_${con.childquestion}']`);
                                    jsonSkip[j].isChecked = true;
                                    showSkip(fieldset, con.childquestion)


                                }
                            }
                        }
                    } else {
                        for (var j = 0; j < jsonSkip.length; j++) {
                            if (jsonSkip[j].parentquestion == pid) {
                                var con = jsonSkip[j];
                                var opid = x.target.getAttribute("data-val-option");
                                if (eval(`${opid} ${con.Operator} ${con.Optionid}`)) {
                                    var fieldset = document.querySelector(`[data-val-question='question_${con.childquestion}']`);
                                    jsonSkip[j].isChecked = false;
                                    showSkip(fieldset, con.childquestion)

                                }
                            }
                        }
                    }

                })
            }
        }
    }


}


//File-Input 
{
    async function fileinput() {
        var filelist = document.querySelectorAll('.question-input-file');
        filelist.forEach(file => {
            file.addEventListener('change', async (e) => {
                // Get the selected file
                var input = e.target;
                var container = e.target.parentNode;
                container.querySelector('.file-list').innerHTML = "";
                var [file] = e.target.files;

                // Get the file name and size
                // var link =URL.createObjectURL(file);
                if (file != null) {
                    let db = new Localbase('db');
                    var name = input.getAttribute("data-val-savedfiles");
                    if (name != "") {
                        var data = await db.collection("Files").doc(name).get();
                        if (data != null) {
                            await db.collection("Files").doc(name).delete();
                        }
                    }

                    var response = await db.collection("Files").add(file);
                    var key = response.match(/(?<=\"key\"\:\")(.*)(?=\"\,\"data)/mg);

                    var savedfiles = input.getAttribute("data-val-savedfiles");
                    savedfiles = key;

                    input.setAttribute("data-val-savedfiles", savedfiles);

                    var { name: fileName, size } = file;
                    // Convert size in bytes to kilo bytes
                    var fileSize = (size / 1000).toFixed(2);

                    var fileNameAndSize = `  <div class="file-item file_${key}"><span>${fileName}</span> <span>${fileSize} KB</span><span class="remove-file-input" name="${key}">x</span></div>`;
                    container.querySelector('.file-list').innerHTML += fileNameAndSize;
                    var removefilebutton = container.querySelectorAll(".remove-file-input");
                    //remove Download Event Listener
                    {
                        removefilebutton.forEach(button => {
                            button.addEventListener("click", z => {
                                var button = z.target;
                                var n = button.getAttribute("name");
                                let db = new Localbase('db');
                                db.collection("Files").doc(n).delete().then(x => {
                                    button.parentNode.remove();
                                    input.value = "";
                                    input.setAttribute("data-val-savedfiles", "");
                                });

                            })
                        })
                    }
                }

            });
        })
    }
    fileinput();

}

var SurveyForm = document.getElementById("SurveyForm");
//Submit
{

    //load files from localstorage
    async function loadFilesForSubmition(data) {
        const fromdata = new FormData();
        let db = new Localbase('db');
        for (var pair of data.entries()) {
            console.log(pair[0] + ', ' + pair[1]);
            var filein = document.getElementsByName(`${pair[0]}`)[0];
            if (filein != null && filein.type == "file") {
                var key = filein.getAttribute("data-val-savedfiles");
                if (key.trim() != "") {
                    var file = await db.collection("Files").doc(key).get();
                    pair[1] = file;
                    await db.collection("Files").doc(key).delete();
                }
            }
            fromdata.append(pair[0], pair[1]);
        }
        return fromdata;
    }

    SurveyForm.addEventListener("submit", async e => {
        e.preventDefault();
        if (!checkrequired()) {
            Swal.fire("Required Questions", "Please Fill Out All The Required Questions ", "error");
            return;
        }
        resetIndexesForQuestions();
        const formData = new FormData(e.target);
        const data = await loadFilesForSubmition(formData);
        const formProps = Object.fromEntries(data);

        checkConnection().then(x => {


            var agentid = document.getElementById("agentid").value;
            var formid = document.getElementById("formidd").value;
            if (x) {
                $.ajax({
                    url: `/Survey/Survey/${formid}?SAgTRid=${agentid}`,
                    method: "POST",
                    data: data,
                    contentType: false,
                    processData: false,
                    success: function () {
                        Swal.fire("Success", "You'r Form Was Submited", "success").then(async function (result) {
                            let db = new Localbase('db');
                            var data = await db.collection("Surveys").doc(form_id).get();
                            if (data != null) { await db.collection("Surveys").doc(form_id).delete(); }
                            window.location.href = "/survey/SuccessfullySubmitted";
                        })
                    },
                    error: function (err) {
                        Swal.fire("Something Went Wrong!", "", "error");
                    }
                })
            } else {
                SaveDataLocal(formProps);
                Swal.fire("Offline Mode", "It Looks Like You'r Offline. We Will Submit The Form When You Come Back Online", "warning").then(function (result) {
                    if (result.value) {
                        window.location.reload();
                    }
                })
            }
        });

    })

    function SaveDataLocal(dataform) {
        console.log(dataform);
        let db = new Localbase('db');
        db.collection("surveys").add(dataform);
    }

    var checkdbinterval;
    checkdbinterval = setInterval(checkDb, 20000);

    function checkDb() {
        checkConnection().then(x => {
            if (!x) {
                return;
            }
            let db = new Localbase('db');
            db.collection("surveys").get().then(z => {
                if (z.length > 0) {
                    z.forEach(da => {
                        SaveDataFromStorage(da).then(v => {
                            if (v) {
                                Swal.fire("Success", "You'r Saved Data From Offline Mode Has Been Saved", "success");
                                db.collection('surveys').doc(da).delete()
                            } else {
                                Swal.fire("Something Went Wrong!", "You'r Saved Data From Offline Mode Was Not Saved ", "warning");
                            }
                        })
                    });

                }
            })
        })

    }


    function SaveDataFromStorage(data) {
        const formData = new FormData();
        for (var key in data) {
            formData.append(key, data[key]);
        }
        var agentid = document.getElementById("agentid").value;
        var formid = document.getElementById("formidd").value;
        return fetch(`/Survey/Survey/${formid}?SAgTRid=${agentid}`, {
            method: "post",
            body: formData
        })
            .then(x => {
                if (x.status == 200) {
                    return true
                };
                return false
            })
            .catch(x => {
                return false
            });
    }

    //Validate Form
    {
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, "");
        }

        function checkrequired() {
            var isvalid = true;
            var inputs = document.querySelectorAll("[data-val-req='true']");
            for (var i = 0; i < inputs.length; i++) {
                var field = inputs[i].closest("fieldset");
                if (field.disabled == false) {
                    if (inputs[i].type == "radio") {
                        var checkname = inputs[i].getAttribute("name");
                        var checked = field.querySelector(`input[name="${checkname}"]:checked`)
                        if (checked == null) {
                            var div = field.querySelector(".isquestion");
                            div.classList.add("invalid-required")
                            div.querySelector(".questionreqmessage").style = "display:block;"
                            isvalid = false;

                        } else {
                            var div = field.querySelector(".isquestion");
                            div.classList.remove("invalid-required")
                            div.querySelector(".questionreqmessage").style = "display:none;"

                        }
                        continue;
                    }
                    else if (inputs[i].type == "checkbox") {
                        var checked = $(".questionInput:checkbox").filter(":checked")
                        // var checked = field.querySelector(`.questionInput:checkbox:checked`)
                        if (checked.length <= 0) {
                            var div = field.querySelector(".isquestion");
                            div.classList.add("invalid-required")
                            div.querySelector(".questionreqmessage").style = "display:block;"
                            isvalid = false;

                        } else {
                            var div = field.querySelector(".isquestion");
                            div.classList.remove("invalid-required")
                            div.querySelector(".questionreqmessage").style = "display:none;"

                        }
                        continue;


                    }
                    else if (inputs[i].type == "file") {
                        var file = inputs[i].getAttribute("data-val-savedfiles");
                        if (file.trim() === "") {
                            var div = field.querySelector(".isquestion");
                            div.classList.add("invalid-required")
                            div.querySelector(".questionreqmessage").style = "display:block;"
                            isvalid = false;

                        }
                        else {
                            var div = field.querySelector(".isquestion");
                            div.classList.remove("invalid-required")
                            div.querySelector(".questionreqmessage").style = "display:none;"
                        }
                    }
                    else if (inputs[i].value.trim() === "" || inputs[i].value == null) {
                        var div = field.querySelector(".isquestion");
                        div.classList.add("invalid-required")
                        div.querySelector(".questionreqmessage").style = "display:block;"
                        isvalid = false;
                    } else {
                        var div = field.querySelector(".isquestion");
                        div.classList.remove("invalid-required")
                        div.querySelector(".questionreqmessage").style = "display:none;"

                    }
                }
            }
            return isvalid;
        }
    }
    //Drafts
    {
        async function loadFilesForDraft(data) {
            const formData = new FormData();
            for (var pair of data.entries()) {
                var filein = document.getElementsByName(`${pair[0]}`)[0];
                if (filein != null && filein.type == "file") {
                    var key = filein.getAttribute("data-val-savedfiles");
                    pair[1] = key;

                }
                formData.append(pair[0], pair[1]);
            }

            return formData;
        }
        async function LoadSavedDraftList() {
            let db = new Localbase('db');
            var saveddraftdata = await db.collection("Drafts").get({
                keys: true
            });
            var bodymodal = document.getElementById("draftlist");
            var thisformid = document.getElementById("formidd").value;
            if (saveddraftdata != null) {
                var alllist = "";

                saveddraftdata.forEach(x => {
                    if (x.data.formid == thisformid) {
                        var st = `  <li><span>${x.key}</span>
                    <span><button data-val-record="${x.key}" data-val-recordoffline="true" class="btn btn-primary btn-sm loaddraftsbutton">Load</button></span></li>`;
                        alllist = alllist + st;
                    }

                })

                bodymodal.innerHTML = alllist;

            }
            var loadbuttonlist = document.querySelectorAll(".loaddraftsbutton");
            loadbuttonlist.forEach(x => {
                x.addEventListener("click", e => {
                    loaddraftdata(e.target)
                })
            })
        }
        LoadSavedDraftList();


        var savedraft = document.querySelector("#saveasdraft");
        savedraft.addEventListener("click", () => savedraftfunction())
        async function savedraftfunction() {
            var errormessage = document.getElementById("savedrafterror");

            var draftname = document.getElementById("draftnameinput").value;
            if (draftname.trim() == "") {
                errormessage.innerHTML = "You need to put in a uniqe name";
                errormessage.style = "display:block";
                return;
            }

            var form = document.getElementById("SurveyForm");

            const formData = new FormData(form);
            const realdata = await loadFilesForDraft(formData)
            const formProps = Object.fromEntries(realdata);

            document.getElementById("savedrafterror").style = "display:none";
            var draft = {};
            let db = new Localbase('db');
            var data = await db.collection("Drafts").doc(draftname).get();
            if (data != null) {
                errormessage.style = "display:block";
                return;
            }
            await db.collection("Drafts").add(formProps, draftname);

            Swal.fire("Draft Saved", "Your survey draft is saved. Please keep in mind that clearing browser caches will also remove these drafts", "warning").then(async function (result) {
                if (result.value) {
                    let db = new Localbase('db');
                    var data = await db.collection("Surveys").doc(form_id).get();
                    await db.collection("Surveys").doc(form_id).delete()
                    window.location.reload();
                };;
            })

        }

        async function loaddraftdata(button) {

            var isoffline = button.getAttribute("data-val-recordoffline");
            var draftkey = button.getAttribute("data-val-record");
            if (isoffline) {
                let db = new Localbase('db');
                var data = await db.collection("Drafts").doc(draftkey).get();
                for (var key in data) {
                    var question = document.getElementsByName(key);
                    for (var i = 0; i < question.length; i++) {
                        if (question[i] != null) {
                            if (question[i].type == "radio" && question[i].value == data[key]) {
                                question[i].checked = true;
                            }
                            else if (question[i].type == "checkbox" && data[key] == "true") {
                                question[i].checked = true;
                            }
                            else if (question[i].type == "file") {
                                var currentinput = question[i];
                                var container = currentinput.parentNode;
                                currentinput.setAttribute("data-val-savedfiles", data[key]);
                                if (data[key].trim() == "") {
                                    continue;
                                }
                                var savedfile = await db.collection("Files").doc(data[key]).get();
                                var { name: fileName, size } = savedfile;
                                // Convert size in bytes to kilo bytes
                                var fileSize = (size / 1000).toFixed(2);
                                var fileNameAndSize = `  <div class="file-item file_${data[key]}"><span>${fileName}</span> <span>${fileSize} KB</span><span class="remove-file-input" name="${data[key]}">x</span></div>`;
                                container.querySelector('.file-list').innerHTML += fileNameAndSize;
                                var removefilebutton = container.querySelectorAll(".remove-file-input");
                                //remove Download Event Listener
                                {
                                    removefilebutton.forEach(button => {
                                        button.addEventListener("click", z => {
                                            var button = z.target;
                                            var n = button.getAttribute("name");
                                            let db = new Localbase('db');
                                            db.collection("Files").doc(n).delete().then(x => {
                                                button.parentNode.remove();
                                                currentinput.value = "";
                                                currentinput.setAttribute("data-val-savedfiles", "");
                                            });

                                        })
                                    })
                                }

                            }
                            else {
                                question[i].value = data[key];
                            }
                            var form = question[i].closest("fieldset");
                            if (form != null) {
                                form.style = "display:block";
                                form.classList.add("activeform");
                                form.disabled = false;
                            }
                        }
                    }
                }
                await db.collection("Drafts").doc(draftkey).delete();
                button.parentNode.parentNode.remove();
            }
        }
    }
} var selectLanguage = document.getElementById("selectLanguageId");
var jsonelement = document.getElementById("data-json-skip");
var rawjson = jsonelement.getAttribute("name")
var currentSurveyData = document.getElementById("currentsurveyversion");
var form_id = document.getElementById("form_id").value;

//Initial Functions
{
    //check if the connection is valid
    function checkConnection() {
        return fetch("/Survey/EndPointCheck/", {
            method: "post"
        }).then(x => {
            var ur = new URL(x.url);
            if (x.status != 200 || ur.pathname == "/offline.html") {
                return false;
            } else {
                return true
            }
        })
            .catch(z => {
                return false;
            });

    }
    async function loadFilesForDraft(data) {
        const formData = new FormData();
        for (var pair of data.entries()) {
            var filein = document.getElementsByName(`${pair[0]}`)[0];
            if (filein != null && filein.type == "file") {
                var key = filein.getAttribute("data-val-savedfiles");
                pair[1] = key;

            }
            formData.append(pair[0], pair[1]);
        }

        return formData;
    }

    async function autoSaveForm() {
        var form = document.getElementById("SurveyForm");
        var db = new Localbase("db");
        const formData = new FormData(form);
        const realdata = await loadFilesForDraft(formData)
        const formProps = Object.fromEntries(realdata);
        const data = await db.collection("Surveys").doc(form_id).get();
        if (data == null) {
            await db.collection("Surveys").doc(form_id).set(formProps);
        }
        else {
            await db.collection("Surveys").add(formProps, form_id);
        }

    }
    async function deleteAutoSavedSurvey() {
        let db = new Localbase('db');
        var data = await db.collection("Surveys").doc(form_id).get();
        for (var key in data) {
            if (key.includes("].File") && data[key].trim() != "") {

                await db.collection("Files").doc(data[key]).delete();

            }

            //formData.append(key, data[key]);
        }
        await db.collection("Surveys").doc(form_id).delete()
    }
    async function restoreAutoSavedSurvey() {
        let db = new Localbase('db');
        var data = await db.collection("Surveys").doc(form_id).get();
        for (var key in data) {
            var question = document.getElementsByName(key);
            for (var i = 0; i < question.length; i++) {
                if (question[i] != null) {
                    if (question[i].type == "radio" && question[i].value == data[key]) {
                        question[i].checked = true;
                    }
                    else if (question[i].type == "checkbox" && data[key] == "true") {
                        question[i].checked = true;
                    }
                    else if (question[i].type == "file") {
                        var currentinput = question[i];
                        var container = currentinput.parentNode;
                        currentinput.setAttribute("data-val-savedfiles", data[key]);
                        if (data[key].trim() == "") {
                            continue;
                        }
                        var savedfile = await db.collection("Files").doc(data[key]).get();
                        var { name: fileName, size } = savedfile;
                        // Convert size in bytes to kilo bytes
                        var fileSize = (size / 1000).toFixed(2);
                        var fileNameAndSize = `  <div class="file-item file_${data[key]}"><span>${fileName}</span> <span>${fileSize} KB</span><span class="remove-file-input" name="${data[key]}">x</span></div>`;
                        container.querySelector('.file-list').innerHTML += fileNameAndSize;
                        var removefilebutton = container.querySelectorAll(".remove-file-input");
                        //remove Download Event Listener
                        {
                            removefilebutton.forEach(button => {
                                button.addEventListener("click", z => {
                                    var button = z.target;
                                    var n = button.getAttribute("name");
                                    let db = new Localbase('db');
                                    db.collection("Files").doc(n).delete().then(x => {
                                        button.parentNode.remove();
                                        currentinput.value = "";
                                        currentinput.setAttribute("data-val-savedfiles", "");
                                    });

                                })
                            })
                        }

                    }
                    else {
                        question[i].value = data[key];
                    }
                    var form = question[i].closest("fieldset");
                    if (form != null) {
                        form.style = "display:block";
                        form.classList.add("activeform");
                        form.disabled = false;
                    }
                }
            }
        }
    }
    async function setSruvey() {
        let db = new Localbase('db');
        var all = await db.collection("Surveys").doc(form_id).get();
        if (all != null && all.length != 0) {
            Swal.fire({
                title: "Unsaved Record?", text: "There is one unsaved record, would you like to restore it?", icon: "warning",
                showCancelButton: true,
                allowOutsideClick: false,
                confirmButtonText: "Yes, Load Record"
            }).then(async function (result) {
                if (!result.isDismissed) {
                    await restoreAutoSavedSurvey();
                }
                else {

                    await deleteAutoSavedSurvey();

                }
            });
        }

    }
    var autosaveinterval;
    autosaveinterval = setInterval(autoSaveForm, 10000);
    setSruvey();
    if (rawjson != "") {
        var jsonSkip = JSON.parse(rawjson);
        jsonelement.remove();
    }
    //reset indexes for question inputs
    function resetIndexesForQuestions() {
        var regex = /(?<=Questions\[)(\d*)/mg;
        var fields = document.querySelectorAll(".activeform");
        for (var i = 0; i < fields.length; i++) {
            var inputlist = fields[i].querySelectorAll(".questionInput");
            for (var z = 0; inputlist.length > z; z++) {
                var oldname = inputlist[z].getAttribute("name");
                nameAttr = oldname.replace(regex, `${i}`);
                inputlist[z].setAttribute("name", nameAttr);
            }

        }
    }
}

//Translate Question
{
    selectLanguage.addEventListener("change", (e) => {
        var selectedlang = e.target.value;
        translateQuestions(selectedlang);

    })

    function translateQuestions(lang) {
        var arabic = /[\u0600-\u06FF]/;
        var questions = document.querySelectorAll(".isquestion");
        questions.forEach(x => {
            var translation = x.querySelectorAll(".translation");
            translation.forEach(z => {

                if (z.classList.contains(`${lang}-translation`)) {

                    z.classList.remove("disabled");
                    z.classList.add("activet");
                } else {
                    z.classList.remove("activet");
                    z.classList.add("disabled");
                }
            })
            var questiontext = x.querySelector(".questiontext span.activet");
            if (questiontext != null) {
                if (arabic.test(questiontext.innerHTML)) {
                    x.classList.add("rtl")
                } else {
                    x.classList.remove("rtl")

                }
            }
        });
    }
}


//SkipLogic
{
    function showSkip(field, childid) {
        var conditionType = field.getAttribute("data-val-ctype");
        var isFilled = false
        if (conditionType == 0) {
            var skiploglist = jsonSkip.filter(ob => { return ob.childquestion === childid });
            for (var i = 0; i < skiploglist.length; i++) {
                if (skiploglist[i].isChecked) {
                    isFilled = true;
                    break;
                }
            }
        }
        else {
            var skiploglist = jsonSkip.filter(ob => { return ob.childquestion === childid });
            for (var i = 0; i < skiploglist.length; i++) {
                if (!skiploglist[i].isChecked) {
                    isFilled = false;
                    break;
                }
                isFilled = true;

            }

        }
        if (isFilled) {
            field.style = "display:block";
            field.classList.add("activeform");
            field.disabled = false;
        }
        else {
            field.style = "display:none";
            field.classList.remove("activeform");
            field.disabled = true;
        }
    }
    //One Select
    {
        var selectinputs = document.querySelectorAll(".skp");
        if (selectinputs != null) {
            for (var i = 0; i < selectinputs.length; i++) {
                selectinputs[i].addEventListener("change", (x) => {
                    var pid = x.target.getAttribute("data-val-q");
                    for (var j = 0; j < jsonSkip.length; j++) {
                        if (jsonSkip[j].parentquestion == pid) {
                            var con = jsonSkip[j];
                            var fieldset = document.querySelector(`[data-val-question='question_${con.childquestion}']`);

                            if (eval(`${x.target.value} ${con.Operator} ${con.Optionid}`)) {
                                jsonSkip[j].isChecked = true;
                            } else {
                                jsonSkip[j].isChecked = false;
                            }
                            showSkip(fieldset, con.childquestion)

                        }
                    }

                })
            }
        }
    }
    //Number Question 
    {
        var numberinput = document.querySelectorAll(".skpn");
        if (numberinput != null) {
            for (var i = 0; numberinput.length > i; i++) {
                numberinput[i].addEventListener("input", (x) => {
                    var pid = x.target.getAttribute("data-val-q");
                    for (var j = 0; j < jsonSkip.length; j++) {
                        if (jsonSkip[j].parentquestion == pid) {
                            var con = jsonSkip[j];
                            var fieldset = document.querySelector(`[data-val-question='question_${con.childquestion}']`);
                            if (eval(`${x.target.value} ${con.Operator} ${con.value}`)) {
                                jsonSkip[j].isChecked = true;

                            } else {
                                jsonSkip[j].isChecked = false;

                            }
                            showSkip(fieldset, con.childquestion)
                        }
                    }
                })
            }
        }
    }
    //Multi Select
    {
        var multiinputs = document.querySelectorAll(".skpm");
        if (multiinputs != null) {
            for (var i = 0; i < multiinputs.length; i++) {
                multiinputs[i].addEventListener("change", (x) => {
                    var pid = x.target.getAttribute("data-val-q");
                    if (x.target.checked) {
                        for (var j = 0; j < jsonSkip.length; j++) {
                            if (jsonSkip[j].parentquestion == pid) {
                                var con = jsonSkip[j];
                                var opid = x.target.getAttribute("data-val-option");
                                if (eval(`${opid} ${con.Operator} ${con.Optionid}`)) {
                                    var fieldset = document.querySelector(`[data-val-question='question_${con.childquestion}']`);
                                    jsonSkip[j].isChecked = true;
                                    showSkip(fieldset, con.childquestion)


                                }
                            }
                        }
                    } else {
                        for (var j = 0; j < jsonSkip.length; j++) {
                            if (jsonSkip[j].parentquestion == pid) {
                                var con = jsonSkip[j];
                                var opid = x.target.getAttribute("data-val-option");
                                if (eval(`${opid} ${con.Operator} ${con.Optionid}`)) {
                                    var fieldset = document.querySelector(`[data-val-question='question_${con.childquestion}']`);
                                    jsonSkip[j].isChecked = false;
                                    showSkip(fieldset, con.childquestion)

                                }
                            }
                        }
                    }

                })
            }
        }
    }


}


//File-Input 
{
    async function fileinput() {
        var filelist = document.querySelectorAll('.question-input-file');
        filelist.forEach(file => {
            file.addEventListener('change', async (e) => {
                // Get the selected file
                var input = e.target;
                var container = e.target.parentNode;
                container.querySelector('.file-list').innerHTML = "";
                var [file] = e.target.files;

                // Get the file name and size
                // var link =URL.createObjectURL(file);
                if (file != null) {
                    let db = new Localbase('db');
                    var name = input.getAttribute("data-val-savedfiles");
                    if (name != "") {
                        var data = await db.collection("Files").doc(name).get();
                        if (data != null) {
                            await db.collection("Files").doc(name).delete();
                        }
                    }

                    var response = await db.collection("Files").add(file);
                    var key = response.match(/(?<=\"key\"\:\")(.*)(?=\"\,\"data)/mg);

                    var savedfiles = input.getAttribute("data-val-savedfiles");
                    savedfiles = key;

                    input.setAttribute("data-val-savedfiles", savedfiles);

                    var { name: fileName, size } = file;
                    // Convert size in bytes to kilo bytes
                    var fileSize = (size / 1000).toFixed(2);

                    var fileNameAndSize = `  <div class="file-item file_${key}"><span>${fileName}</span> <span>${fileSize} KB</span><span class="remove-file-input" name="${key}">x</span></div>`;
                    container.querySelector('.file-list').innerHTML += fileNameAndSize;
                    var removefilebutton = container.querySelectorAll(".remove-file-input");
                    //remove Download Event Listener
                    {
                        removefilebutton.forEach(button => {
                            button.addEventListener("click", z => {
                                var button = z.target;
                                var n = button.getAttribute("name");
                                let db = new Localbase('db');
                                db.collection("Files").doc(n).delete().then(x => {
                                    button.parentNode.remove();
                                    input.value = "";
                                    input.setAttribute("data-val-savedfiles", "");
                                });

                            })
                        })
                    }
                }

            });
        })
    }
    fileinput();

}

var SurveyForm = document.getElementById("SurveyForm");
//Submit
{

    //load files from localstorage
    async function loadFilesForSubmition(data) {
        const fromdata = new FormData();
        let db = new Localbase('db');
        for (var pair of data.entries()) {
            console.log(pair[0] + ', ' + pair[1]);
            var filein = document.getElementsByName(`${pair[0]}`)[0];
            if (filein != null && filein.type == "file") {
                var key = filein.getAttribute("data-val-savedfiles");
                if (key.trim() != "") {
                    var file = await db.collection("Files").doc(key).get();
                    pair[1] = file;
                    await db.collection("Files").doc(key).delete();
                }
            }
            fromdata.append(pair[0], pair[1]);
        }
        return fromdata;
    }

    SurveyForm.addEventListener("submit", async e => {
        e.preventDefault();
        if (!checkrequired()) {
            Swal.fire("Required Questions", "Please Fill Out All The Required Questions ", "error");
            return;
        }
        resetIndexesForQuestions();
        const formData = new FormData(e.target);
        const data = await loadFilesForSubmition(formData);
        const formProps = Object.fromEntries(data);

        checkConnection().then(x => {


            var agentid = document.getElementById("agentid").value;
            var formid = document.getElementById("formidd").value;
            if (x) {
                $.ajax({
                    url: `/Survey/Survey/${formid}?SAgTRid=${agentid}`,
                    method: "POST",
                    data: data,
                    contentType: false,
                    processData: false,
                    success: function () {
                        Swal.fire("Success", "You'r Form Was Submited", "success").then(async function (result) {
                            let db = new Localbase('db');
                            var data = await db.collection("Surveys").doc(form_id).get();
                            if (data != null) { await db.collection("Surveys").doc(form_id).delete(); }
                            window.location.href = "/survey/SuccessfullySubmitted";
                        })
                    },
                    error: function (err) {
                        Swal.fire("Something Went Wrong!", "", "error");
                    }
                })
            } else {
                SaveDataLocal(formProps);
                Swal.fire("Offline Mode", "It Looks Like You'r Offline. We Will Submit The Form When You Come Back Online", "warning").then(function (result) {
                    if (result.value) {
                        window.location.reload();
                    }
                })
            }
        });

    })

    function SaveDataLocal(dataform) {
        console.log(dataform);
        let db = new Localbase('db');
        db.collection("surveys").add(dataform);
    }

    var checkdbinterval;
    checkdbinterval = setInterval(checkDb, 20000);

    function checkDb() {
        checkConnection().then(x => {
            if (!x) {
                return;
            }
            let db = new Localbase('db');
            db.collection("surveys").get().then(z => {
                if (z.length > 0) {
                    z.forEach(da => {
                        SaveDataFromStorage(da).then(v => {
                            if (v) {
                                Swal.fire("Success", "You'r Saved Data From Offline Mode Has Been Saved", "success");
                                db.collection('surveys').doc(da).delete()
                            } else {
                                Swal.fire("Something Went Wrong!", "You'r Saved Data From Offline Mode Was Not Saved ", "warning");
                            }
                        })
                    });

                }
            })
        })

    }


    function SaveDataFromStorage(data) {
        const formData = new FormData();
        for (var key in data) {
            formData.append(key, data[key]);
        }
        var agentid = document.getElementById("agentid").value;
        var formid = document.getElementById("formidd").value;
        return fetch(`/Survey/Survey/${formid}?SAgTRid=${agentid}`, {
            method: "post",
            body: formData
        })
            .then(x => {
                if (x.status == 200) {
                    return true
                };
                return false
            })
            .catch(x => {
                return false
            });
    }

    //Validate Form
    {
        String.prototype.trim = function () {
            return this.replace(/^\s+|\s+$/g, "");
        }

        function checkrequired() {
            var isvalid = true;
            var inputs = document.querySelectorAll("[data-val-req='true']");
            for (var i = 0; i < inputs.length; i++) {
                var field = inputs[i].closest("fieldset");
                if (field.disabled == false) {
                    if (inputs[i].type == "radio") {
                        var checkname = inputs[i].getAttribute("name");
                        var checked = field.querySelector(`input[name="${checkname}"]:checked`)
                        if (checked == null) {
                            var div = field.querySelector(".isquestion");
                            div.classList.add("invalid-required")
                            div.querySelector(".questionreqmessage").style = "display:block;"
                            isvalid = false;

                        } else {
                            var div = field.querySelector(".isquestion");
                            div.classList.remove("invalid-required")
                            div.querySelector(".questionreqmessage").style = "display:none;"

                        }
                        continue;
                    }
                    else if (inputs[i].type == "checkbox") {
                        var checked = $(".questionInput:checkbox").filter(":checked")
                        // var checked = field.querySelector(`.questionInput:checkbox:checked`)
                        if (checked.length <= 0) {
                            var div = field.querySelector(".isquestion");
                            div.classList.add("invalid-required")
                            div.querySelector(".questionreqmessage").style = "display:block;"
                            isvalid = false;

                        } else {
                            var div = field.querySelector(".isquestion");
                            div.classList.remove("invalid-required")
                            div.querySelector(".questionreqmessage").style = "display:none;"

                        }
                        continue;


                    }
                    else if (inputs[i].type == "file") {
                        var file = inputs[i].getAttribute("data-val-savedfiles");
                        if (file.trim() === "") {
                            var div = field.querySelector(".isquestion");
                            div.classList.add("invalid-required")
                            div.querySelector(".questionreqmessage").style = "display:block;"
                            isvalid = false;

                        }
                        else {
                            var div = field.querySelector(".isquestion");
                            div.classList.remove("invalid-required")
                            div.querySelector(".questionreqmessage").style = "display:none;"
                        }
                    }
                    else if (inputs[i].value.trim() === "" || inputs[i].value == null) {
                        var div = field.querySelector(".isquestion");
                        div.classList.add("invalid-required")
                        div.querySelector(".questionreqmessage").style = "display:block;"
                        isvalid = false;
                    } else {
                        var div = field.querySelector(".isquestion");
                        div.classList.remove("invalid-required")
                        div.querySelector(".questionreqmessage").style = "display:none;"

                    }
                }
            }
            return isvalid;
        }
    }
    //Drafts
    {
        async function loadFilesForDraft(data) {
            const formData = new FormData();
            for (var pair of data.entries()) {
                var filein = document.getElementsByName(`${pair[0]}`)[0];
                if (filein != null && filein.type == "file") {
                    var key = filein.getAttribute("data-val-savedfiles");
                    pair[1] = key;

                }
                formData.append(pair[0], pair[1]);
            }

            return formData;
        }
        async function LoadSavedDraftList() {
            let db = new Localbase('db');
            var saveddraftdata = await db.collection("Drafts").get({
                keys: true
            });
            var bodymodal = document.getElementById("draftlist");
            var thisformid = document.getElementById("formidd").value;
            if (saveddraftdata != null) {
                var alllist = "";

                saveddraftdata.forEach(x => {
                    if (x.data.formid == thisformid) {
                        var st = `  <li><span>${x.key}</span>
                    <span><button data-val-record="${x.key}" data-val-recordoffline="true" class="btn btn-primary btn-sm loaddraftsbutton">Load</button></span></li>`;
                        alllist = alllist + st;
                    }

                })

                bodymodal.innerHTML = alllist;

            }
            var loadbuttonlist = document.querySelectorAll(".loaddraftsbutton");
            loadbuttonlist.forEach(x => {
                x.addEventListener("click", e => {
                    loaddraftdata(e.target)
                })
            })
        }
        LoadSavedDraftList();


        var savedraft = document.querySelector("#saveasdraft");
        savedraft.addEventListener("click", () => savedraftfunction())
        async function savedraftfunction() {
            var errormessage = document.getElementById("savedrafterror");

            var draftname = document.getElementById("draftnameinput").value;
            if (draftname.trim() == "") {
                errormessage.innerHTML = "You need to put in a uniqe name";
                errormessage.style = "display:block";
                return;
            }

            var form = document.getElementById("SurveyForm");

            const formData = new FormData(form);
            const realdata = await loadFilesForDraft(formData)
            const formProps = Object.fromEntries(realdata);

            document.getElementById("savedrafterror").style = "display:none";
            var draft = {};
            let db = new Localbase('db');
            var data = await db.collection("Drafts").doc(draftname).get();
            if (data != null) {
                errormessage.style = "display:block";
                return;
            }
            await db.collection("Drafts").add(formProps, draftname);

            Swal.fire("Draft Saved", "Your survey draft is saved. Please keep in mind that clearing browser caches will also remove these drafts", "warning").then(async function (result) {
                if (result.value) {
                    let db = new Localbase('db');
                    var data = await db.collection("Surveys").doc(form_id).get();
                    await db.collection("Surveys").doc(form_id).delete()
                    window.location.reload();
                };;
            })

        }

        async function loaddraftdata(button) {

            var isoffline = button.getAttribute("data-val-recordoffline");
            var draftkey = button.getAttribute("data-val-record");
            if (isoffline) {
                let db = new Localbase('db');
                var data = await db.collection("Drafts").doc(draftkey).get();
                for (var key in data) {
                    var question = document.getElementsByName(key);
                    for (var i = 0; i < question.length; i++) {
                        if (question[i] != null) {
                            if (question[i].type == "radio" && question[i].value == data[key]) {
                                question[i].checked = true;
                            }
                            else if (question[i].type == "checkbox" && data[key] == "true") {
                                question[i].checked = true;
                            }
                            else if (question[i].type == "file") {
                                var currentinput = question[i];
                                var container = currentinput.parentNode;
                                currentinput.setAttribute("data-val-savedfiles", data[key]);
                                if (data[key].trim() == "") {
                                    continue;
                                }
                                var savedfile = await db.collection("Files").doc(data[key]).get();
                                var { name: fileName, size } = savedfile;
                                // Convert size in bytes to kilo bytes
                                var fileSize = (size / 1000).toFixed(2);
                                var fileNameAndSize = `  <div class="file-item file_${data[key]}"><span>${fileName}</span> <span>${fileSize} KB</span><span class="remove-file-input" name="${data[key]}">x</span></div>`;
                                container.querySelector('.file-list').innerHTML += fileNameAndSize;
                                var removefilebutton = container.querySelectorAll(".remove-file-input");
                                //remove Download Event Listener
                                {
                                    removefilebutton.forEach(button => {
                                        button.addEventListener("click", z => {
                                            var button = z.target;
                                            var n = button.getAttribute("name");
                                            let db = new Localbase('db');
                                            db.collection("Files").doc(n).delete().then(x => {
                                                button.parentNode.remove();
                                                currentinput.value = "";
                                                currentinput.setAttribute("data-val-savedfiles", "");
                                            });

                                        })
                                    })
                                }

                            }
                            else {
                                question[i].value = data[key];
                            }
                            var form = question[i].closest("fieldset");
                            if (form != null) {
                                form.style = "display:block";
                                form.classList.add("activeform");
                                form.disabled = false;
                            }
                        }
                    }
                }
                await db.collection("Drafts").doc(draftkey).delete();
                button.parentNode.parentNode.remove();
            }
        }
    }
}