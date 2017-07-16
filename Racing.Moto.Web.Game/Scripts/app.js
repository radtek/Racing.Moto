var $app = {
    MinBalance: {
        Room1: { Min: 0, Name: '初级场' },
        Room2: { Min: 5000, Name: '中级场' },
        Room3: { Min: 10000, Name: '高级场' }
    },
    getValidationMessages: function () {
        var messages = [];
        $('.validation-summary-errors > ul > li').each(function (index, ele) {
            messages.push($(this).text());
        });
        return messages;
    },
    convertToDate: function (dateString) {
        dateString = dateString.replace('T', ' ').replace(/-/g, "/");
        var reggie1 = /(\d{2,4})\/(\d{1,2})\/(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})/;
        var reggie2 = /(\d{2,4})\/(\d{1,2})\/(\d{1,2})/;
        var dateArray = reggie1.exec(dateString) != null ? reggie1.exec(dateString) : reggie2.exec(dateString);
        var year = (+dateArray[1]);
        var month = (+dateArray[2]) - 1;
        var day = (+dateArray[3]);
        var hour = dateArray.length > 4 ? (+dateArray[4]) : 0;
        var minute = dateArray.length > 5 ? (+dateArray[5]) : 0;
        var second = dateArray.length > 6 ? (+dateArray[6]) : 0;
        var dateObject = new Date(year, month, day, hour, minute, second);

        return dateObject;
    },
    formatDate: function (date, fmt) {
        var o = {
            "M+": date.getMonth() + 1, //月份 
            "d+": date.getDate(), //日 
            "H+": date.getHours(), //小时 
            "m+": date.getMinutes(), //分 
            "s+": date.getSeconds(), //秒 
            "q+": Math.floor((date.getMonth() + 3) / 3), //季度 
            "S": date.getMilliseconds() //毫秒 
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    },
    formatDateString: function (dateStr, fmt) {
        if (dateStr == null || dateStr == '') return '';
        var date = $app.convertToDate(dateStr);
        var result = $app.formatDate(date, fmt);

        return result;
    },
    getWeek: function (dateStr) {
        var weeks = new Array("日", "一", "二", "三", "四", "五", "六");
        var weekDay = $app.convertToDate(dateStr).getDay();
        return weeks[weekDay];
    },
    submitOnEnterKey: function (txtId, targetId) {
        $('#' + txtId).keypress(function (event) {
            if (event.keyCode == 13) {
                $("#" + targetId).focus().click();
            }
        });
    },
    isNum: function (txt) {
        if (txt == null || txt == '') {
            return false;
        } else {
            txt = txt.replace(/\s/g, "");
            return !isNaN(txt);
        }
    },
    add: function (arg1, arg2) {
        var r1, r2, m, n;
        try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }
        try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }
        m = Math.pow(10, Math.max(r1, r2));
        n = (r1 >= r2) ? r1 : r2;
        var r = ((arg1 * m + arg2 * m) / m).toFixed(n);
        return Number(r);
    },
    sub: function (arg1, arg2) {
        var r1, r2, m, n;
        try { r1 = arg1.toString().split(".")[1].length } catch (e) { r1 = 0 }
        try { r2 = arg2.toString().split(".")[1].length } catch (e) { r2 = 0 }
        m = Math.pow(10, Math.max(r1, r2));
        n = (r1 >= r2) ? r1 : r2;
        var r = ((arg2 * m - arg1 * m) / m).toFixed(n);
        return Number(r);
    },
    mul: function (arg1, arg2) {
        var m = 0, s1 = arg1.toString(), s2 = arg2.toString();
        try { m += s1.split(".")[1].length } catch (e) { }
        try { m += s2.split(".")[1].length } catch (e) { }
        return Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m);
    },
    div: function (arg1, arg2) {
        var t1 = 0, t2 = 0, r1, r2;
        try { t1 = arg1.toString().split(".")[1].length } catch (e) { }
        try { t2 = arg2.toString().split(".")[1].length } catch (e) { }
        with (Math) {
            r1 = Number(arg1.toString().replace(".", ""));
            r2 = Number(arg2.toString().replace(".", ""));
            console.log(r2 / r1);
            return (r2 / r1) * pow(10, t1 - t2);
        }
    },
    round: function (d, decimals) {
        if (d == null) { return ''; }
        with (Math) {
            return round(d * pow(10, decimals)) / pow(10, decimals);
        }
    },
    padLeft: function (len, char) {
        while (temp.length < len) {
            temp = c + temp;
        }
    },
    getNewlineMsg: function (msgArr) {
        if (!document.all) {
            return msgArr.join('\n');
        } else {//ie
            return msgArr.join('\r\n');
        }
    },
    checkPwd: function (pwd) {
        var res = { "IsValid": true, "Message": "" };
        if (pwd == null || pwd.length == 0) {
            res.IsValid = false;
            res.Message = "请输入密码";
        } else {
            var regex = new RegExp('(?=.*[0-9])(?=.*[a-zA-Z]).{6,20}');
            if (!regex.test(pwd)) {
                res.IsValid = false;
                res.Message = "密码过于简单, 请输入至少6位的数字与字母组合密码";
            }
        }
        return res;
    },
    /** 随机化原数组 **/
    shuffle: function (array) {
        var m = array.length,
            t, i;
        // 如果还剩有元素…
        while (m) {
            // 随机选取一个元素…
            i = Math.floor(Math.random() * m--);
            // 与当前元素进行交换
            t = array[m];
            array[m] = array[i];
            array[i] = t;
        }
        return array;
    },
    exitArena: function getCookie() {
        $.ajax({
            type: 'POST',
            url: '/Moto/Exit',
            data: {},
            success: function (res) {
            }
        });
    }
};
$cookie = {
    add: function (name, value, expiresHours) {
        var cookieString = name + "=" + escape(value);

        if (expiresHours > 0) {    //判断是否设置过期时间 
            var date = new Date();
            date.setTime(date.getTime + expiresHours * 3600 * 1000);
            cookieString = cookieString + "; expires=" + date.toGMTString();
        }

        document.cookie = cookieString;
    },
    get: function (name) {
        var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
        if (arr = document.cookie.match(reg)) {
            return unescape(arr[2]);
        } else {
            return null;
        }
    },
    remove: function (name) {
        var date = new Date();
        date.setTime(date.getTime() - 10000);
        document.cookie = name + "=v; expires=" + date.toGMTString();
    },
};
Number.prototype.add = function (arg) {
    return $app.add(arg, this);
}
Number.prototype.sub = function (arg) {
    return $app.sub(arg, this);
}
Number.prototype.mul = function (arg) {
    return $app.mul(arg, this);
}
Number.prototype.div = function (arg) {
    return $app.div(arg, this);
}
String.prototype.startWith = function (str) {
    var reg = new RegExp("^" + str);
    return reg.test(this);
}
String.prototype.endWith = function (str) {
    var reg = new RegExp(str + "$");
    return reg.test(this);
}
String.prototype.padLeft = function (len, char) {
    var temp = this;
    while (temp.length < len) {
        temp = char + temp;
    }
    return temp;
}
Date.prototype.addDays = Date.prototype.addDays || function (n) {
    this.setDate(this.getDate() + n);
    return this;
}
Date.prototype.addSeconds = Date.prototype.addSeconds || function (n) {
    this.setSeconds(this.getSeconds() + n);
    return this;
}
Array.prototype.insert = function (index, item) {
    this.splice(index, 0, item);
};
Array.prototype.remove = function (index) {
    this.splice(index, 1);
};