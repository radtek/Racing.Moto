(function ($) {
    var floatingBg = {
        init: function (options) {

            return this.each(function () {
                var $this = $(this);

                var settings = $this.data('options');

                if (typeof (settings) == 'undefined') {

                    var defaults = {
                        backgroud: '/images/road.png',
                        direction: 0,
                        millisec: 50
                    }

                    settings = $.extend({}, defaults, options);

                    $this.data('options', settings);
                } else {
                    settings = $.extend({}, settings, options);
                }

                $this.css("background", "url('" + settings.backgroud + "')");
                var timeId = floatingBg.move($this);
                $this.data('timeId', timeId);
            });
        },
        destroy: function (options) {
            return $(this).each(function () {
                var $this = $(this);

                var timeId = $this.data('timeId');
                clearTimeout(timeId);

                // 删除元素对应的数据
                $this.removeData('options');
                $this.removeData('timeId');
            });
        },
        move: function ($this) {
            var opt = $this.data('options');
            if (opt == null) {
                return;
            }

            var dir = floatingBg.getDirection(opt.direction);

            var timeId = setTimeout(function () {
                var cnt = $this.attr("cnt") != null ? $this.attr("cnt") : 0;

                //if (cnt > 1000) {
                //    cnt = 0;
                //} else {
                //    cnt = eval(cnt) + 1;
                //}
                cnt = eval(cnt) + 20;

                $this.attr("cnt", cnt);

                if (dir.X != "" && dir.Y != "") {
                    $this.css("backgroundPosition", dir.X + cnt + "px" + " " + dir.Y + cnt + "px");
                }
                else if (dir.X == "") {
                    $this.css("backgroundPosition", "0px" + " " + dir.Y + cnt + "px");
                }
                else if (dir.Y == "") {
                    $this.css("backgroundPosition", dir.X + cnt + "px" + " " + "0px");
                }

                floatingBg.move($this);
            }, opt.millisec);

            return timeId;
        },
        getDirection: function (direction) {
            if (direction == -1) {
                direction = getDirection();
            }

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
            };

            return { X: dirX, Y: dirY };
        },
    };

    $.fn.floatingBg = function () {
        var method = arguments[0];

        if (floatingBg[method]) {
            method = floatingBg[method];
            arguments = Array.prototype.slice.call(arguments, 1);
        } else if (typeof (method) == 'object' || !method) {
            method = floatingBg.init;
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.pluginName');
            return this;
        }

        return method.apply(this, arguments);
    }

})(jQuery);