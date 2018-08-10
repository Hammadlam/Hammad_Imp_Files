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
"Content/LoginForm/img/backgrounds/Business Image 1p.png?n=1",

"Content/LoginForm/img/backgrounds/Business Image 2.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 3.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 4.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 5.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 6.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 7.png?n=1",
"Content/LoginForm/img/backgrounds/Business Image 8.jpeg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 9.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 10.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 11.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 12.jpeg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 13.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 14.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 15.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 16.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 17.png?n=1",
"Content/LoginForm/img/backgrounds/Business Image 18.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 19.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 20.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 21.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 22.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 23.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 24.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 25.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 26.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 27.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 28.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 29.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 30.png?n=1",
"Content/LoginForm/img/backgrounds/Business Image 31.png?n=1",
"Content/LoginForm/img/backgrounds/Business Image 32.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 33.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 34.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 35.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 36.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 37.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 38.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 39.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 40.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 41.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 42.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 43.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 44.png?n=1",
"Content/LoginForm/img/backgrounds/Business Image 45.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 46.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 47.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 48.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 49.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 50.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 51.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 52.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 53.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 54.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 55.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 56.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 57.png?n=1",
"Content/LoginForm/img/backgrounds/Business Image 58.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 59.jpg?n=1",
"Content/LoginForm/img/backgrounds/Business Image 60.jpg?n=1"
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



