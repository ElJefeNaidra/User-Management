//Me perkrah datat en-GB
jQuery.validator.methods.date = function (value, element, param)
{
    alert("it has been entered")
    return this.optional(element) || /(^(((0[1-9]|1[0-9]|2[0-8])[/](0[1-9]|1[012]))|((29|30|31)[/](0[13578]|1[02]))|((29|30)[/](0[4,6,9]|11)))[/](19|[2-9][0-9])\d\d$)|(^29[/]02[/](19|[2-9][0-9])(00|04|08|12|16|20|24|28|32|36|40|44|48|52|56|60|64|68|72|76|80|84|88|92|96)$)/.test(value);
}

$('input').on('paste', function () {
    var $el = $(this);
    setTimeout(function () {
        $el.val(function (i, val) {
            return val.replace(/^\s+/g, "")

            var regexp = /[^a-zA-ZçÇëËčČćĆđĐšŠžŽĞğÖöŞşÜüIıİi ]/g;

            if ($($el).val().match(regexp)) {
                $($el).val($($el).val().replace(regexp, ''));
                //   $(this).val($(this).val().trim());
            }
        })
    })
});


function CheckOnlyLetters(element) {
    $('#' + element).on("input", function () {
        // Per Emra e mbiemra ku nuk don me lon hi shprastin as majtas as djathtas
        //$(this).val($(this).val().trim())

        // Per vene ku don me lon shprastin djathtas po jo majtas psh ku ki space
        $(this).val($(this).val().replace(/^\s+/g, ""))

        var regexp = /[^a-zA-ZçÇëËčČćĆđĐšŠžŽĞğÖöŞşÜüIıİi ]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
            //   $(this).val($(this).val().trim());
        }
    }
    );
};

function CheckOnlyLettersAndSpecialChars(element) {
    $('#' + element).on("input", function () {
        // Per Emra e mbiemra ku nuk don me lon hi shprastin as majtas as djathtas
        //$(this).val($(this).val().trim())

        // Per vene ku don me lon shprastin djathtas po jo majtas psh ku ki space
        $(this).val($(this).val().replace(/^\s+/g, ""))

        var regexp = /[^a-zA-ZçÇëËčČćĆđĐšŠžŽĞğÖöŞşÜüIıİi -]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
            //   $(this).val($(this).val().trim());
        }
    }
    );
};

function CheckOnlyAddressChars(element) {
    $('#' + element).on("input", function () {
        // Per Emra e mbiemra ku nuk don me lon hi shprastin as majtas as djathtas
        //$(this).val($(this).val().trim())

        // Per vene ku don me lon shprastin djathtas po jo majtas psh ku ki space
        $(this).val($(this).val().replace(/^\s+/g, ""))

        var regexp = /[^-.\ /0-9a-zA-ZçÇëËčČćĆđĐšŠžŽĞğÖöŞşÜüIıİi]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
            //   $(this).val($(this).val().trim());
        }
    }
    );
};

function CheckOnlyDashesSlashesLettersNumbersDots(element) {
    $('#' + element).on("input", function () {
        // Per Emra e mbiemra ku nuk don me lon hi shprastin as majtas as djathtas
        //$(this).val($(this).val().trim())

        // Per vene ku don me lon shprastin djathtas po jo majtas psh ku ki space
        $(this).val($(this).val().replace(/^\s+/g, ""))

        var regexp = /[^-.\ /0-9a-zA-ZçÇëËčČćĆđĐšŠžŽĞğÖöŞşÜüIıİi]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
            //   $(this).val($(this).val().trim());
        }
    }
    );
};

function CheckOnlyDashesSlashesLettersNumbers(element) {
    $('#' + element).on("input", function () {
        // Per Emra e mbiemra ku nuk don me lon hi shprastin as majtas as djathtas
        //$(this).val($(this).val().trim())

        // Per vene ku don me lon shprastin djathtas po jo majtas psh ku ki space
        $(this).val($(this).val().replace(/^\s+/g, ""))

        var regexp = /[^- /0-9a-zA-Z]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
            //   $(this).val($(this).val().trim());
        }
    }
    );
};

function CheckOnlyEmailAddressChars(element) {
    $('#' + element).on("input", function () {
        // Per Emra e mbiemra ku nuk don me lon hi shprastin as majtas as djathtas
        //$(this).val($(this).val().trim())

        // Per vene ku don me lon shprastin djathtas po jo majtas psh ku ki space
        $(this).val($(this).val().replace(/^\s+/g, ""))

        var regexp = /^[_a-z0-9-]+(\\.[_a-z0-9-]+)*@[a-z0-9]+(\\.[a-z0-9]+)*(\\.[a-z]{2,})$/;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
            //   $(this).val($(this).val().trim());
        }
    }
    );
};



function CheckOnlyNumbers(element) {
    $('#' + element).on("input", function () {
        // Numra jane nuk duhet te kene shprastina as majtas as djathtas
        $(this).val($(this).val().trim())

        var regexp = /[^0-9]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
        }
    }
    );
};

function CheckOnlyNumbersAndBackslash(element) {
    $('#' + element).on("input", function () {
        // Numra jane nuk duhet te kene shprastina as majtas as djathtas
        $(this).val($(this).val().trim())

        var regexp = /[^0-9/]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
        }
    }
    );
};

function DisableEnableControlBasedOnCheckBox(element, controll) {
    element.checked ? document.getElementById(controll).disabled = false : document.getElementById(controll).disabled = true;
};


function CheckOnlyLoginNameChars(element) {
    $('#' + element).on("input", function () {
        // Per Emra e mbiemra ku nuk don me lon hi shprastin as majtas as djathtas
        $(this).val($(this).val().trim())

        // Per vene ku don me lon shprastin djathtas po jo majtas psh ku ki space
        //$(this).val($(this).val().replace(/^\s+/g, ""))

        var regexp = /[^./0-9a-zA-Z]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
            //   $(this).val($(this).val().trim());
        }
    }
    );
};

function CheckOnlyLettersAndNumbers(element)
{
    $('#' + element).on("input", function () {
        // Numra jane nuk duhet te kene shprastina as majtas as djathtas
        $(this).val($(this).val().trim())
        var regexp = /[^0-9a-zA-Z]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
        }
    }
    );
};

function CheckOnlyLettersAndNumbersAndDashes(element) {
    $('#' + element).on("input", function () {
        // Numra jane nuk duhet te kene shprastina as majtas as djathtas
        $(this).val($(this).val().trim())
        var regexp = /[^0-9a-zA-Z-]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
        }
    }
    );
};

function CheckOnlyDecimal(element) {
    $('#' + element).on("input", function () {
        // Numra jane nuk duhet te kene shprastina as majtas as djathtas
        $(this).val($(this).val().trim())
        var regexp = /[^0-9.]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
        }
    }
    );
};

function ValidationRegexNumbersAndUpperCaseLetters(element) {
    $('#' + element).on("input", function () {
        // Numra jane nuk duhet te kene shprastina as majtas as djathtas
        $(this).val($(this).val().trim())
        var regexp = /[^0-9A-Z]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
        }
    }
    );
};


function CheckOnlyDashesSlashesLettersNumbersDotsSpecial(element) {
    $('#' + element).on("input", function () {
        // Per Emra e mbiemra ku nuk don me lon hi shprastin as majtas as djathtas
        //$(this).val($(this).val().trim())

        // Per vene ku don me lon shprastin djathtas po jo majtas psh ku ki space
        $(this).val($(this).val().replace(/^\s+/g, ""))

        var regexp = /[^-.\ /0-9a-zA-Z@çÇëËčČćĆđĐšŠžŽĞğÖöŞşÜüIıİi]/g;

        if ($(this).val().match(regexp)) {
            $(this).val($(this).val().replace(regexp, ''));
            //   $(this).val($(this).val().trim());
        }
    }
    );
};