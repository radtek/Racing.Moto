var $app = {
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
    submitOnEnterKey: function (txtId, targetId) {
        $('#' + txtId).keypress(function (event) {
            if (event.keyCode == 13) {
                $("#" + targetId).focus().click();
            }
        });
    },
};