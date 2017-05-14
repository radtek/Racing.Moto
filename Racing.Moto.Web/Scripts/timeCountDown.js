/* by zhangxinxu 2010-07-27 
* http://www.zhangxinxu.com/wordpress/?p=987
* 倒计时的实现
*/
var fnTimeCountDown = function (remainSeconds, o, callback) {
    var f = {
        callback: null,
        zero: function (n) {
            var n = parseInt(n, 10);
            if (n > 0) {
                if (n <= 9) {
                    n = "0" + n;
                }
                return String(n);
            } else {
                return "00";
            }
        },
        dv: function () {
            var dur = remainSeconds;
            var pms = {
                sec: "00",
                mini: "00",
                hour: "00",
                day: "00",
                month: "00",
                year: "0",
                isZero: false
            };
            if (dur >= 0) {
                pms.sec = f.zero(dur % 60);
                pms.mini = Math.floor((dur / 60)) > 0 ? f.zero(Math.floor((dur / 60)) % 60) : "00";

            } else {
                pms.isZero = true;
            }
            return pms;
        },
        ui: function () {
            if (o.sec && o.mini) {
                var dv = f.dv();

                o.sec.innerHTML = dv.sec;
                o.mini.innerHTML = dv.mini;
                o.hour.innerHTML = "00";

                remainSeconds--;
            }

            if (!f.dv().isZero) {
                setTimeout(f.ui, 1000);
            } else {
                //callback
                if (typeof f.callback === "function") {
                    f.callback();
                }
            }
        }
    };
    f.callback = callback;
    f.ui();
};
var fnTimeCountDown1 = function (remainSeconds1, o1, callback1) {
    var f1 = {
        callback: null,
        zero: function (n) {
            var n = parseInt(n, 10);
            if (n > 0) {
                if (n <= 9) {
                    n = "0" + n;
                }
                return String(n);
            } else {
                return "00";
            }
        },
        dv: function () {
            var dur = remainSeconds1;
            var pms = {
                sec: "00",
                mini: "00",
                hour: "00",
                day: "00",
                month: "00",
                year: "0",
                isZero: false
            };
            if (dur >= 0) {
                pms.sec = f1.zero(dur % 60);
                pms.mini = Math.floor((dur / 60)) > 0 ? f1.zero(Math.floor((dur / 60)) % 60) : "00";

            } else {
                pms.isZero = true;
            }
            return pms;
        },
        ui: function () {
            if (o1.sec && o1.mini) {
                var dv = f1.dv();

                o1.sec.innerHTML = dv.sec;
                o1.mini.innerHTML = dv.mini;
                o1.hour.innerHTML = "00";

                remainSeconds1--;
            }

            if (!f1.dv().isZero) {
                setTimeout(f1.ui, 1000);
            } else {
                //callback
                if (typeof f1.callback === "function") {
                    f1.callback();
                }
            }
        }
    };
    f1.callback = callback1;
    f1.ui();
};
var fnTimeCountDown2 = function (remainSeconds2, o2, callback2) {
    var f2 = {
        callback: null,
        zero: function (n) {
            var n = parseInt(n, 10);
            if (n > 0) {
                if (n <= 9) {
                    n = "0" + n;
                }
                return String(n);
            } else {
                return "00";
            }
        },
        dv: function () {
            var dur = remainSeconds2;
            var pms = {
                sec: "00",
                mini: "00",
                hour: "00",
                day: "00",
                month: "00",
                year: "0",
                isZero: false
            };
            if (dur >= 0) {
                pms.sec = f2.zero(dur % 60);
                pms.mini = Math.floor((dur / 60)) > 0 ? f2.zero(Math.floor((dur / 60)) % 60) : "00";

            } else {
                pms.isZero = true;
            }
            return pms;
        },
        ui: function () {
            if (o2.sec && o2.mini) {
                var dv = f2.dv();

                o2.sec.innerHTML = dv.sec;
                o2.mini.innerHTML = dv.mini;
                o2.hour.innerHTML = "00";

                remainSeconds2--;
            }

            if (!f2.dv().isZero) {
                setTimeout(f2.ui, 1000);
            } else {
                //callback
                if (typeof f2.callback === "function") {
                    f2.callback();
                }
            }
        }
    };
    f2.callback = callback2;
    f2.ui();
};

var fnTimeCountDownResult = {
    pms: {
        sec: "00",
        mini: "00",
        hour: "00"
    },
    zero: function (n) {
        var n = parseInt(n, 10);
        if (n > 0) {
            if (n <= 9) {
                n = "0" + n;
            }
            return String(n);
        } else {
            return "00";
        }
    },
    getResult: function (seconds) {
        var pms = {
            sec: "00",
            mini: "00",
            hour: "00"
        };
        if (seconds >= 0) {
            pms.sec = fnTimeCountDownResult.zero(seconds % 60);
            pms.mini = Math.floor((seconds / 60)) > 0 ? fnTimeCountDownResult.zero(Math.floor((seconds / 60)) % 60) : "00";
        }
        return pms;
    },
    getResultStr: function (seconds) {
        var pms = fnTimeCountDownResult.getResult(seconds);
        return pms.hour + ":" + pms.mini + ":" + pms.sec;
    },
};