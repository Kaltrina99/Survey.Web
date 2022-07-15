var form = document.getElementById("formSkip");
var errorMessage = document.getElementById("errorMessage");

form.addEventListener("submit", (e) => {
    e.preventDefault();
    var formdata = new FormData(form);
    fetch("/survey/SkipLogic", { method: "post", body: formdata })
        .then(x => {
            if (x.ok)
            {
                window.location.reload();
            }
        })
        .catch(x => {
            errorMessage.innerHTML = "";
            errorMessage.innerHTML = x.message;
        })


})