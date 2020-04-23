/*/// <reference path="~/Scripts/jquery-3.3.1.min.js"/>*/


function SaveChanges(patientId) {

    var list = [32];
    for (i = 0; i < 32; i++) {
        var id = "State_" + i;
        var element = document.getElementById(id);
        if (element.innerText == "") { alert("Choose the description for tooth condition!"); return; }
        list[i] = element.innerText;
    }

    $.ajax('/Home/ShowTeeth', {
        method: 'post',
        contentType: 'application/json',
        data: JSON.stringify(list),
        traditional: true,
        success: function (d) {
            alert("All good!");
        }
    });
    window.location.href = "/Home/ShowTeeth/?id=" + patientId;
}

function StateChange(selValue, i) {

    if (selValue == "empty") { alert("Please choose the right value for tooth state!"); }
    else {
        document.getElementById("State_" + i).innerHTML = selValue;
    }
}


function SubmitForm(formname) {
    //alert("SubmitForm here!");
    var fields = $(".ss-item-required")
        .find("select, textarea, input").serializeArray();
    var fire = true;

    $.each(fields, function (i, field) {
        if (!field.value) {
            alert(field.name + ' is required');
            fire = false;
        }
    });
    console.log(fields);
    if (fire) {
        var frm = document.getElementsByName(formname)[0];
        frm.submit(); // Submit the form
        frm.reset();  // Reset all form data
        return false; // Prevent page refresh
    }
}

function Filevalidation(name) {
   
    var fi = name; //document.getElementById("files");
    // Check if any file is selected.
    var fsize = 0;
    if (fi.files.length > 0) {
        for (var i = 0; i <= fi.files.length - 1; i++) {
            fsize += fi.files.item(i).size;
            var file = Math.round((fsize / 1024));
            // The size of the file.
            if (file >= 4096) {
                document.getElementById("btnCrt").style.display = "none";
                alert("The total size is too big, please select les files");
            }
            else {
                document.getElementById("btnCrt").style.display = "block";
                document.getElementById('size').innerHTML = '<b>' + file + '</b> KB';
            }
        }
    }
    
}

function DeleteConfirm(x, kind) {
    switch (kind) {
        case "m":
            if (confirm("You are about to delete a manipulation!")) {
                window.location.href = "/Home/DeleteManipulation/?id=" + x;
            }
            break;
        case "i":
            if (confirm("You are about to delete an image!")) {
                window.location.href = "/Home/DeleteVisitPicture/?id=" + x;
            }
            break;
        case "v":
            if (confirm("You are about to delete a visit!")) {
                window.location.href = "/Home/DeleteVisit/?id=" + x;
            }
            break;
        case "p":
            if (confirm("You are about to delete a patient!")) {
                window.location.href = "/Home/DeletePatient/?id=" + x;
            }
            break;
        case "im":
            if (confirm("You are about to delete an image!")) {
                window.location.href = "/Home/DeleteManipulationPicture/?id=" + x;
            }
            break;
        default:
            alert("Unknown event!");
    }
}


function ShowPassword(passfield, repassfield) {
    var passwordShow = document.getElementById("passwordChbx");
    var passWordField = document.getElementById(passfield);
    if (passwordShow.checked == true) {
        passWordField.type = "text";
        if (repassfield != '') {
            document.getElementById(repassfield).type = "text";
        }
    } else {
        passWordField.type = "password";
        if (repassfield != '') {
            document.getElementById(repassfield).type = "password";
        }
    }
}