var $pkAdmin = {
    MinRoleId: 99,
    Actions: ['bet', 'user/generalagent|user/agent|user/member', 'log|account/changepassword', 'rate|online/internal', 'report', 'lottery', 'news', 'online', 'account'],
    SubMenus: [
        [{ Name: '前五名', Link: '/Admin/Bet/Add/1', MaxRoleId: 3 }, { Name: '后五名', Link: '/Admin/Bet/Add/2', MaxRoleId: 3 }, { Name: '大小单双', Link: '/Admin/Bet/Add/3', MaxRoleId: 3 }],
        [
            { Name: '总代理', Link: '/Admin/User/GeneralAgent', MaxRoleId: 2 },
            { Name: '代理', Link: '/Admin/User/Agent', MaxRoleId: 3 },
            { Name: '会员', Link: '/Admin/User/Member', MaxRoleId: 4 }
        ],
        [{ Name: '登录日志', Link: '/Admin/Log/LoginRecord', MaxRoleId: 3 }, { Name: '变更密码', Link: '/Admin/Account/ChangePassword', MaxRoleId: 3 }],
        [
            { Name: '赔率设置', Link: '/Admin/Rate/Setting', MaxRoleId: 1 },
            /*{ Name: '盘赔率差设置', Link: '/Admin/Rate/MinusSetting', MaxRoleId: 1 },*/
            { Name: '在线会员', Link: '/Admin/Online/Internal/4', MaxRoleId: 3 },
            { Name: '在线代理', Link: '/Admin/Online/Internal/3', MaxRoleId: 2 },
            { Name: '在线总代理', Link: '/Admin/Online/Internal/2', MaxRoleId: 1 }
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
            case 'report': url = '/admin/report/index'; break;
            case 'lottery': url = '/admin/lottery/history'; break;
            case 'news': url = '/admin/news/index'; break;
            case 'online': url = '/admin/online/management'; break;
            case 'quit': url = '/admin/account/logout'; break;
        }
        //console.log(url);
        if (url != '') {
            location.href = url;
        }
    },
};