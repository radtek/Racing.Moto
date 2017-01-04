(function ($) {

    var _options = new Array();
    var _queue = new Array();

    jQuery.fn.FloatingBg = function (options) {
        _options[_options.length] = $.extend({}, $.fn.FloatingBg.defaults, options);

        var idx = _options.length - 1;
        var opt = _options[idx];
        this.attr("idx", idx);

        var direction = -1;
        if (opt.direction == -1)
            direction = getDirection();
        else
            direction = opt.direction;

        var dirX = "+";
        var dirY = "+";
        switch (direction) {
            case 0:
                dirX = "+";
                dirY = "-";
                break;
            case 1:
                dirX = "-";
                dirY = "+";
                break;
            case 2:
                dirX = "+";
                dirY = "+";
                break;
            case 3:
                dirX = "-";
                dirY = "-";
                break;
            case 4://left
                dirX = "-";
                dirY = "";
                break;
            case 5://right
                dirX = "+";
                dirY = "";
                break;
            case 6://up
                dirX = "";
                dirY = "+";
                break;
            case 7://right
                dirX = "";
                dirY = "-";
                break;
        }

        this.each(function () {
            var bg = $(this).attr("bg");

            $(this).css("background", "url('" + bg + "')");
            $(this).attr("dirX", dirX);
            $(this).attr("dirY", dirY);

            $(this).attr("cnt", 1);
            doShift($(this));
        });
    }

    function doShift(o) {
        var idx = $(o).attr("idx");
        var opt = _options[idx];

        var tiemId = setTimeout(function () {
            var cnt = $(o).attr("cnt");

            if (cnt > 1000)
                cnt = 0;
            else
                cnt = eval(cnt) + 1;
            $(o).attr("cnt", cnt);

            var dirX = $(o).attr("dirX");
            var dirY = $(o).attr("dirY");

            if (dirX != "" && dirY != "") {
                o.css("backgroundPosition", dirX + cnt + "px" + " " + dirY + cnt + "px");
            }
            else if (dirX == "") {
                o.css("backgroundPosition", "0px" + " " + dirY + cnt + "px");
            }
            else if (dirY == "") {
                o.css("backgroundPosition", dirX + cnt + "px" + " " + "0px");
            }

            doShift(o);
        }, opt.speed);

        _queue.push(tiemId);
    }

    function getDirection() {
        return Math.floor(Math.random() * 4)
    }

    //default values
    jQuery.fn.FloatingBg.defaults = {
        speed: 50,
        direction: -1
    };

    jQuery.fn.FloatingBg.stop = function (o) {
        $(this).each(function () {
            var idx = parseInt($(this).attr("idx"));
            clearTimeout(_options[idx]);
        });
    };

})(jQuery);