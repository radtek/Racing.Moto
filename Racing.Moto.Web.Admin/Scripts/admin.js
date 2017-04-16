var $pkAdmin = {
    MinRoleId: 99,
    Actions: ['bet', 'user/generalagent|user/agent|user/member|user/rebate', 'log|account/changepassword', 'rate|online/internal', 'report', 'lottery', 'news', 'online', 'account'],
    SubMenus: [
        [{ Name: '冠亞軍', Link: '/Bet/Info/1', MaxRoleId: 3 }, { Name: '三四五六名', Link: '/Bet/Info/2', MaxRoleId: 3 }, { Name: '七八九十名', Link: '/Bet/Info/3', MaxRoleId: 3 }],
        [
            { Name: '总代理', Link: '/User/GeneralAgent', MaxRoleId: 2 },
            { Name: '代理', Link: '/User/Agent', MaxRoleId: 3 },
            { Name: '会员', Link: '/User/Member', MaxRoleId: 4 }
        ],
        [{ Name: '登录日志', Link: '/Log/LoginRecord', MaxRoleId: 3 }, { Name: '变更密码', Link: '/Account/ChangePassword', MaxRoleId: 3 }],
        [
            { Name: '赔率设置', Link: '/Rate/Setting', MaxRoleId: 3 },
            /*{ Name: '盘赔率差设置', Link: '/Rate/MinusSetting', MaxRoleId: 1 },*/
            { Name: '在线会员', Link: '/Online/Internal/4', MaxRoleId: 3 },
            { Name: '在线代理', Link: '/Online/Internal/3', MaxRoleId: 2 },
            { Name: '在线总代理', Link: '/Online/Internal/2', MaxRoleId: 1 }
        ],
    ],
    init: function (minRoleId) {
        //console.log(minRoleId);
        $pkAdmin.MinRoleId = minRoleId;
        // content
        var contentHeidht = $(window).height() - $('.top').height();
        $('.content-wrapper').css('height', contentHeidht);
        // menu
        var subMenuIndex = $pkAdmin.getSubMenuIndex();
        $pkAdmin.showSubMenu(subMenuIndex);
    },
    getSubMenuIndex: function () {
        var index = 99;
        var url = location.href.toLowerCase().split('?')[0];
        for (var i = 0; i < $pkAdmin.Actions.length; i++) {

            var actionArr = $pkAdmin.Actions[i].split('|');
            //console.log(actionArr);
            for (var j = 0; j < actionArr.length; j++) {
                if (url.indexOf(actionArr[j]) > -1) {
                    index = i;
                    break;
                }
            }
            if (index != 99) {
                break;
            }
        }
        //console.log(index);
        return index;
    },
    setMenu: function () {
        // menu
        $('.but_1').hover(function () {
            $(this).addClass('but_1_m').removeClass('but_1');
        }, function () {
            $(this).addClass('but_1').removeClass('but_1_m');
        });
        $pkAdmin.setSubMenu();
    },
    showSubMenu: function (menuIndex) {
        $('#a_span').html('');
        if (menuIndex < $pkAdmin.SubMenus.length) {
            for (var i = 0; i < $pkAdmin.SubMenus[menuIndex].length; i++) {
                if ($pkAdmin.MinRoleId <= $pkAdmin.SubMenus[menuIndex][i].MaxRoleId) {
                    var width = $pkAdmin.SubMenus[menuIndex][i].Name.length * 15;
                    var $ele = $('<a href="' + $pkAdmin.SubMenus[menuIndex][i].Link + '">' + $pkAdmin.SubMenus[menuIndex][i].Name + '</a>').css('width', width + 'px');
                    $('#a_span').append($ele);

                    if (i < $pkAdmin.SubMenus[menuIndex].length - 1) {
                        $('#a_span').append('<span style="float:left;"><img src="/images/admin/main_34.gif" width="1" height="23"></span>');
                    }
                }
            }
        }
        $pkAdmin.setSubMenu();
    },
    setSubMenu: function () {
        $('#a_span > a')
            .hover(function () { window.status = 'javascript:void(0)'; return true; }, function () { })
            .focus(function () { window.status = 'javascript:void(0)'; return true; });
    },
    goto: function (page) {
        $('#a_span').html('');

        var url = '';
        switch (page) {
            case 'report': url = '/report/index'; break;
            case 'lottery': url = '/lottery/history'; break;
            case 'news': url = '/news/index'; break;
            case 'online': url = '/online/management'; break;
            case 'quit': url = '/account/logout'; break;
        }
        //console.log(url);
        if (url != '') {
            location.href = url;
        }
    },
};