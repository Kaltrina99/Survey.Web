//Global
var surveyFrom = document.getElementById("SurveyForm");
var jsonElement = document.getElementById("data-json-skip");
var rawjson = jsonElement.getAttribute("name")
var form_Id = document.getElementById("form_id").value;
var saveDraftButton = document.querySelector("#saveasdraft");
var autoSaveSeconds = 10000;
var CheckDbSeconds = 40000;

if (rawjson != "") {
    var jsonSkip = JSON.parse(rawjson);
    jsonElement.remove();
}

//Local Database 
var db = new Localbase("db");
//db.config.debug = false;
var DraftDb = "Drafts";
var FilesDb = "Files";
var AutoSaveDb = "AutoSaveDb";
var OfflineSurveyDb = "OfflineSurveyDb";

String.prototype.trim = function () {
    return this.replace(/^\s+|\s+$/g, "");
}

function ResetIndexesForQuestions() {
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


async function CheckConnection()
{
    var response = await fetch("/Survey/EndpointCheck/", { method: "post" });
    var url = new URL(response.url);
    if (!response.ok || url.pathname == "/offline.html")
    {
        return false;
    }
    return true;
}

function LoadFilesFromDraft(data) {
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

async function AutoSaveForm()
{
    const formData = new FormData(SurveyForm);
    const loadedData = LoadFilesFromDraft(formData);
    const formProps = Object.fromEntries(loadedData);
    const data = await db.collection(AutoSaveDb).doc(form_Id).get();
    if (data == null)
    {
        await db.collection(AutoSaveDb).doc(form_Id).set(formProps);
        return;
    }
    await db.collection(AutoSaveDb).add(formProps, form_Id);
}

async function DeleteAutoSavedSurvey()
{
    var data = await db.collection(AutoSaveDb).doc(form_Id).get();
    for (var key in data) {
        if (key.includes("].File") && data[key].trim() != "") {

            await db.collection(FilesDb).doc(data[key]).delete();
        }
    }
    await db.collection(AutoSaveDb).doc(form_Id).delete()
}

async function RestoreAutoSavedSurvey()
{
    var data = await db.collection(AutoSaveDb).doc(form_Id).get();
    for (var key in data)
    {
        var question = document.getElementsByName(key);
        if (question != null)
        {
            for (var i = 0; i < question.length; i++)
            {
                await RestoreQuestionAutoSaved(question[i], data[key]);
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

async function RestoreQuestionAutoSaved(question,data)
{
    if (question.type == "radio" && question.value == data)
    {
        question.checked = true;
    }
    else if (question.type == "checkbox" && data == "true")
    {
        question.checked = true;
    }
    else if (question.type == "file")
    {
        var savedfile = await db.collection(FilesDb).doc(data).get();
        if (data.trim() == "" || savedfile == null) {
            return;
        }
      
        var { name: fileName, size } = savedfile;
        // Convert size in bytes to kilo bytes
        var fileSize = (size / 1000).toFixed(2);
        var fileNameAndSize = `  <div class="file-item file_${data}"><span>${fileName}</span> <span>${fileSize} KB</span><span class="remove-file-input" name="${data}">x</span></div>`;
        var container = question.parentNode;
        container.querySelector('.file-list').innerHTML += fileNameAndSize;

        AddEventRemoveFileButton(container, question);

        question.setAttribute("data-val-savedfiles", data);

    }
    else {
        question.value = data;
    }
}

function AddEventRemoveFileButton(container,question)
{
    var removefilebutton = container.querySelectorAll(".remove-file-input");
    removefilebutton.forEach(button => {
        button.addEventListener("click", z => {
            var button = z.target;
            var n = button.getAttribute("name");

            db.collection(FilesDb).doc(n).delete().then(x => {
                button.parentNode.remove();
                question.value = "";
                question.setAttribute("data-val-savedfiles", "");
            });

        })
    })
}

async function NotifyUserForAutoSave() {
    var all = await db.collection(AutoSaveDb).doc(form_Id).get();
    if (all != null && all.length != 0) {
        Swal.fire({
            title: "Unsaved Record?", text: "There is one unsaved record, would you like to restore it?", icon: "warning",
            showCancelButton: true,
            allowOutsideClick: false,
            confirmButtonText: "Yes, Load Record"
        })
            .then(async function (result) {
                if (!result.isDismissed) {
                    await RestoreAutoSavedSurvey();
                }
                else {
                    await DeleteAutoSavedSurvey();
                }
            });
    }

}

async function AddEventToFileInput()
{
    var filelist = document.querySelectorAll('.question-input-file');
    filelist.forEach(file => {
        file.addEventListener('change', async (e) => {
            // Get the selected file
            var input = e.target;
            var container = e.target.parentNode;
            container.querySelector('.file-list').innerHTML = "";
            var [file] = e.target.files;
            var { name: fileName, size } = file;
            // Convert size in bytes to kilo bytes
            var fileSize = (size / 1000).toFixed(2);
            if (fileSize > 10000)
            {
                input.value = "";
                Swal.fire("File To Large", "Your file is larger than 10MB", "error");
                return;
            }
           

            if (file != null) {
                var name = input.getAttribute("data-val-savedfiles");
                if (name != "") {
                    var data = await db.collection(FilesDb).doc(name).get();
                    if (data != null) {
                        await db.collection(FilesDb).doc(name).delete();
                    }
                }

                var response = await db.collection(FilesDb).add(file);
                var key = response.match(/(?<=\"key\"\:\")(.*)(?=\"\,\"data)/mg);

                input.setAttribute("data-val-savedfiles", key);

                
               

                var fileNameAndSize = `  <div class="file-item file_${key}"><span>${fileName}</span> <span>${fileSize} KB</span><span class="remove-file-input" name="${key}">x</span></div>`;
                container.querySelector('.file-list').innerHTML += fileNameAndSize;
                AddEventRemoveFileButton(container, input)
            }

        });
    })
}
async function RemoveFileFromData(data)
{
    for (var pair of data.entries()) {
        var filein = document.getElementsByName(`${pair[0]}`)[0];
        if (filein != null && filein.type == "file") {
            var key = filein.getAttribute("data-val-savedfiles");
            if (key.trim() != "") {
                await db.collection(FilesDb).doc(key).delete();
            }
        }
    }
}
async function LoadFilesForSubmittion(data)
{
    const fromdata = new FormData();
    for (var pair of data.entries()) {
        var filein = document.getElementsByName(`${pair[0]}`)[0];
        if (filein != null && filein.type == "file") {
            var key = filein.getAttribute("data-val-savedfiles");
            if (key.trim() != "") {
                var file = await db.collection(FilesDb).doc(key).get();
                pair[1] = file;
            }
        }
        fromdata.append(pair[0], pair[1]);
    }
    const date = new Date();
    fromdata.append("EndTime", date.toLocaleString("en-US"))
    return fromdata;
}

function CheckRequiredQuestions() {
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

async function SendDataApi(formData)
{
    const returnresponse = { Message: "", Success: false}
    var agentid = document.getElementById("agentid").value;
    var formid = document.getElementById("formidd").value;
    var Success = false;
    var response = await fetch(`/Survey/Survey/${formid}?SAgTRid=${agentid}`, { method: "post", body: formData });
  
    if (response.ok)
    {
        Success= true;
    }
    returnresponse.Success = Success;
    returnresponse.Message = await response.text();
    return returnresponse;
}

function AddEvenToForm() {
    SurveyForm.addEventListener("submit", async e => {
        e.preventDefault();
        var subbutton = document.getElementById('submitbutton');
        subbutton.innerHTML = 'Submitting...';
        subbutton.disabled = true;
        subbutton.classList.add("spinner");
        subbutton.classList.add("spinner-white");
        subbutton.classList.add("spinner-right");
        if (!CheckRequiredQuestions()) {
            Swal.fire("Required Questions", "Please Fill Out All The Required Questions ", "error");
            subbutton.innerHTML = 'Submit';
            subbutton.disabled = false;
            subbutton.classList.remove("spinner");
            subbutton.classList.remove("spinner-right");
            subbutton.classList.remove("spinner-white");

            return;
        }
        ResetIndexesForQuestions();
        const formData = new FormData(e.target);
        const data = await LoadFilesForSubmittion(formData);
        const s = Object.fromEntries(data);

        var isConnected = await CheckConnection();
        if (isConnected) {
            

            var response = await SendDataApi(data);
        
            if (response.Success) {
              
                Swal.fire({ title: "Success", text: "Your Form Was Submited", icon: "success", allowOutsideClick: false }).then(async function (result) {
                    var data = await db.collection(AutoSaveDb).doc(form_id).get();
                    await RemoveFileFromData(formData);
                    if (data != null) {
                        await db.collection(AutoSaveDb).doc(form_id).delete();
                    }
                    window.location.href = "/survey/SuccessfullySubmitted";
                })
            }
            else
            {
                Swal.fire("Something Went Wrong!", `${response.Message}`, "error");

            }
        }
        else {
            const formProps = Object.fromEntries(data);
            await db.collection(OfflineSurveyDb).add(formProps);
            Swal.fire({ title: "Offline Mode", text: "It Looks Like You Are Offline. We Will Submit The Form When You Come Back Online", icon: "warning", allowOutsideClick: false }).then(async function (result) {
                var data = await db.collection(AutoSaveDb).doc(form_id).get();
                if (data != null) {
                    await db.collection(AutoSaveDb).doc(form_id).delete();
                }
                if (result.value) {

                    window.location.reload();
                }
            })
        }
        subbutton.innerHTML = 'Submit';
        subbutton.disabled = false;
        subbutton.classList.remove("spinner");
        subbutton.classList.remove("spinner-right");
        subbutton.classList.remove("spinner-white");


    });
    
}

async function CheckDb()
{
    var connection = await CheckConnection();
    if (connection)
    {
        var data = await db.collection(OfflineSurveyDb).get();
        if (data != null && data.length > 0) {
            data.forEach(async da => {
                var response =await SaveDataFromStorage(da);
                if (response) {
                    await db.collection(OfflineSurveyDb).doc(da).delete()
                    Swal.fire("Success", "Your Saved Data From Offline Mode Has Been Saved", "success");

                   
                }
                else {
                        //sead
                        Swal.fire("Something Went Wrong!", "Your Saved Data From Offline Mode Was Not Saved ", "warning");
                    }
                })
        }
           
    }
}


async function SaveDataFromStorage(data) {
    const formData = new FormData();
    for (var key in data) {
        formData.append(key, data[key]);
    }
    var response = await SendDataApi(formData)
    if (response.Success)
    {
        for (var key in data) {
            if (key.includes("].File") && data[key] != ""&& data[key] != null) {
                var das = await db.collection(FilesDb).doc(data[key]).get({ keys: true });
                console.log(das);
                if (das!=null)
                {
                    await db.collection(FilesDb).doc(das).delete();
                }
            }
        }
    }
    return response.Success;
}

async function LoadFilesForDraft(data) {
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

async function RenderSavedDraftList()
{

    var saveddraftdata = await db.collection(DraftDb).get({ keys: true });
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
            LoadDataFromDraft(e.target)
        })
    })
}

async function LoadDataFromDraft(button)
{
    var draftkey = button.getAttribute("data-val-record");
    var data = await db.collection(DraftDb).doc(draftkey).get();
        for (var key in data) {
            var question = document.getElementsByName(key);
            for (var i = 0; i < question.length; i++) {
                if (question[i] != null) {
                    await RestoreQuestionAutoSaved(question[i], data[key]);
                    var form = question[i].closest("fieldset");
                    if (form != null) {
                        form.style = "display:block";
                        form.classList.add("activeform");
                        form.disabled = false;
                    }
                }
            }
    }
    await db.collection(DraftDb).doc(draftkey).delete();
        button.parentNode.parentNode.remove();
}

async function SaveDraft()
{
    var errormessage = document.getElementById("savedrafterror");
    var draftname = document.getElementById("draftnameinput").value;
    if (draftname.trim() == "") {
        errormessage.innerHTML = "You need to put in a uniqe name";
        errormessage.style = "display:block";
        return;
    }

    var form = document.getElementById("SurveyForm");

    const formData = new FormData(form);
    const realdata = await LoadFilesForDraft(formData)
    const formProps = Object.fromEntries(realdata);
    document.getElementById("savedrafterror").style = "display:none";

    var data = await db.collection(DraftDb).doc(draftname).get();
    if (data != null) {
        errormessage.style = "display:block";
        return;
    }
    await db.collection(DraftDb).add(formProps, draftname);

    Swal.fire("Draft Saved", "Your survey draft is saved. Please keep in mind that clearing browser caches will also remove these drafts", "warning").then(async function (result) {
        clearInterval(AutoSaveInterval);
        if (result.value) {
            var data = await db.collection(AutoSaveDb).doc(form_id).get();
            await db.collection(AutoSaveDb).doc(form_id).delete()
            window.location.reload();
        };;
    })
}

saveDraftButton.addEventListener("click", () => { SaveDraft()})
RenderSavedDraftList();
AddEvenToForm();
AddEventToFileInput();
NotifyUserForAutoSave();
var AutoSaveInterval;
AutoSaveInterval = setInterval(AutoSaveForm, autoSaveSeconds);
var CheckDbInterval;
CheckDbInterval = setInterval(CheckDb, CheckDbSeconds);

