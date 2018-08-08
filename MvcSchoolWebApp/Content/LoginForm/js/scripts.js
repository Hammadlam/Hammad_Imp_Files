jQuery(document).ready(function () {

    /*
        Fullscreen background
    */
    $.backstretch([
                    "Content/LoginForm/img/backgrounds/Business Image 1a.jpg?n=1",
                    "Content/LoginForm/img/backgrounds/Business Image 1b.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1c.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1d.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1e.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1f.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1g.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1h.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1i.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1j.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1k.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1l.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1m.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1o.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 1p.png?n=1"
    ], { duration: 3000, fade: 750 });

    /*
        Form validation
    */
    $('.login-form input[type="text"], .login-form input[type="password"], .login-form textarea').on('focus', function () {
        $(this).removeClass('input-error');
    });

    $('.login-form').on('submit', function (e) {

        $(this).find('input[type="text"], input[type="password"], textarea').each(function () {
            if ($(this).val() == "") {
                e.preventDefault();
                $(this).addClass('input-error');
            }
            else {
                $(this).removeClass('input-error');
            }
        });

    });


});



