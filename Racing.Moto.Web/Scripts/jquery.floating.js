(function ($) {
    var floating = {
        init: function (options) {

            return this.each(function () {
                var $this = $(this);

                var settings = $this.data('options');

                if (typeof (settings) == 'undefined') {

                    var defaults = {
                        direction: 'right',
                        millisec: 50,
                        position: null
                    }

                    settings = $.extend({}, defaults, options);

                    $this.data('options', settings);
                    $this.data('pos', floating.getInitPos(settings, $this));
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

                //var timeId = $this.data('timeId');
                //clearTimeout(timeId);

                //// 删除元素对应的数据
                //$this.removeData('options');
                //$this.removeData('timeId');
                //$this.removeData('pos');

                floating.clear($this);
            });
        },
        clear: function ($this) {
            var timeId = $this.data('timeId');
            clearTimeout(timeId);

            // 删除元素对应的数据
            $this.removeData('options');
            $this.removeData('timeId');
            $this.removeData('pos');
        },
        move: function ($this) {
            var opt = $this.data('options');
            if (opt == null) {
                return;
            }

            var position = opt.position;//位置
            var pos = parseInt($this.data('pos'), 10);

            if (position != null && position == pos) {
                floating.clear($this);
                return;
            }

            var timeId = setTimeout(function () {
                pos = pos + 20;
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
        getInitPos: function (opt, $this) {
            var pos = 0;
            switch (opt.direction) {
                case 'left': pos = $this.css("right"); break;
                case 'right': pos = $this.css("left"); break;
                case 'top': pos = $this.css("bottom"); break;
                case 'bottom': pos = $this.css("top"); break;
            }
            return pos == 'auto' ? '0px' : pos;
        }
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