(function ($) {
    var floating = {
        init: function (options) {

            return this.each(function () {
                var $this = $(this);

                var settings = $this.data('options');

                if (typeof (settings) == 'undefined') {

                    var defaults = {
                        direction: 'right',
                        millisec: 50
                    }

                    settings = $.extend({}, defaults, options);

                    $this.data('options', settings);
                    $this.data('pos', 0);
                } else {
                    settings = $.extend({}, settings, options);
                }

                var timeId = floating.move($this);
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
                $this.removeData('pos');
            });
        },
        move: function ($this) {
            var opt = $this.data('options');
            if (opt == null) {
                return;
            }

            var timeId = setTimeout(function () {
                var pos = parseInt($this.data('pos'), 10);
                pos = pos + 1;
                $this.data('pos', pos);

                switch (opt.direction) {
                    case 'left': $this.css("right", pos + "px"); break;
                    case 'right': $this.css("left", pos + "px"); break;
                    case 'top': $this.css("bottom", pos + "px"); break;
                    case 'bottom': $this.css("top", pos + "px"); break;
                }

                floating.move($this);
            }, opt.millisec);

            return timeId;
        },
    };

    $.fn.floating = function () {
        var method = arguments[0];

        if (floating[method]) {
            method = floating[method];
            arguments = Array.prototype.slice.call(arguments, 1);
        } else if (typeof (method) == 'object' || !method) {
            method = floating.init;
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.pluginName');
            return this;
        }

        return method.apply(this, arguments);
    }

})(jQuery);