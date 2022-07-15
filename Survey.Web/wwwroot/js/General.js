var arinputs = document.getElementsByClassName("checkarabic");
for (let i = 0; i < arinputs.length; i++) {
    arinputs[i].addEventListener('keydown', (e) => {
        var val = e.target.value;
        var arabic = /[\u0600-\u06FF]/;
        if (arabic.test(val)) {
            e.target.classList.add("rtl")
        }
        else {
            e.target.classList.remove("rtl");
        }
    });
}

function GoBack() {
    window.history.back();
}
