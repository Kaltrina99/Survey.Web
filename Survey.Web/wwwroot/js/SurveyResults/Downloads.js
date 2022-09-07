var form = document.getElementById("downloadForm");
var formid = document.getElementById("formid").value;
var tablebody = document.getElementById("tablebody");

form.addEventListener("submit", e => {
    e.preventDefault();
    getDownload();
});

async function getDownload() {
    var option = document.getElementById("selectdownloadoption").value;
    var response = await fetch(`/SurveyResults/NewDownload/${formid}?option=${option}`);
    var data = await response.text();
    tablebody.innerHTML = "";
    window.location.reload();
    resetorder();
}
resetorder();
function resetorder() {
    var list = document.getElementsByClassName("ordernumber");
    for (i = 0; i < list.length; i++)
    {
        var index = i + 1;
        list[i].innerHTML = index;
    }
}