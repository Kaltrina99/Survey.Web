var mainform = document.getElementById("mainform");
/*    mainform.addEventListener('submit', async function (e) {
        var Errormessage = document.getElementById("Errormessage");
        e.preventDefault;
        var fileExtension = ['xls', 'xlsx', 'xlsm'];
        var button = document.getElementById("btnupload");
        button.classList.add("spinner");
        button.disabled = true;
        button.classList.add("spinner-white");
        button.classList.add("spinner-right");
        button.innerHTML = "Saving...";
        var filename = $('#fileupload').val();
        if (filename.length == 0) {
            alert("Please select a file.");
            button.classList.remove("spinner");
            button.disabled = false;
            button.classList.remove("spinner-white");
            button.classList.remove("spinner-right");
            button.innerHTML = "Save";
            return ;
        }
        else {
            var extension = filename.replace(/^.*\./, '');
            if ($.inArray(extension, fileExtension) == -1) {
                alert("Please select only excel files.");
                button.classList.remove("spinner");
                button.disabled = false;
                button.classList.remove("spinner-white");
                button.classList.remove("spinner-right");
                button.innerHTML = "Save";
                return ;
            }
        }
        var fdata = new FormData(e.target);
       
        var response = await fetch("/Survey/CreateSurveyExcel", { method: 'post', body: fdata });
        var res = await response;
        if (res.ok) {
            window.location.href = "/Survey/ManageForm";
        }
        else {
            Errormessage.innerHTML = res.Errormessage;
        }
        button.classList.remove("spinner");
        button.disabled = false;
        button.classList.remove("spinner-white");
        button.classList.remove("spinner-right");
        button.innerHTML = "Save";
    })*/
mainform.addEventListener("submit", async (e) => {
    e.preventDefault();
    var Errormessage = document.getElementById("Errormessage");
    var button = document.getElementById("btnupload");
    button.disabled = true;
    button.innerHTML = "Saving...";
    button.classList.add("spinner");
    button.classList.add("spinner-white");
    button.classList.add("spinner-right");

    var data = new FormData(mainform);
    var response = await fetch("/Survey/CreateSurveyExcel", { method: 'post', body: data });
    var res = await response;
    if (res.ok)
    {
        window.location.href = "/Survey/ManageForm";
    
    }
    else
    {
        Errormessage.innerHTML = await response.text();
        button.disabled = false;
        button.innerHTML = "Save";
        button.classList.remove("spinner");
        button.classList.remove("spinner-white");
        button.classList.remove("spinner-right");
    }


})


const file = document.querySelector('#fileupload');
file.addEventListener('change', (e) => {
    // Get the selected file
    const [file] = e.target.files;
    // Get the file name and size
    const { name: fileName, size } = file;
    // Convert size in bytes to kilo bytes
    const fileSize = (size / 1000).toFixed(2);
    // Set the text content
    const fileNameAndSize = `${fileName} - ${fileSize}KB`;
    document.querySelector('.file-name').textContent = fileNameAndSize;
});