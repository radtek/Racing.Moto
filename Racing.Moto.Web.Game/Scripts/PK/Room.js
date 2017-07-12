$(function () {
    //var $elememts = $('.game-wrap').html();
    var $deskes = $("#divDesks");
    var _roomLevel = $("#hidRoomLevel").val();

    var ticker = $.connection.pKRoomTickerHub;

    function init() {
        ticker.server.getPKRoomInfo().done(function (pkRooms) {
            console.log(pkRooms);

            // test
            //pkRooms = {};
            //var now = new Date();
            //var seconds = 10;
            //pkRooms.GameBeginTime = $app.formatDate(now.addSeconds(seconds), 'yyyy-MM-dd HH:mm:ss');
            //pkRooms.GamingSeconds = -seconds;
            //pkRooms.GamePassedSeconds = 0;
            //pkRooms.GameRemainSeconds = 30;
            //pkRooms.PK = { PKId: 1, Ranks: '3,2,5,6,8,7,10,1,9,4', GameSeconds: 30 };


            if (pkRooms == null) {
                return;
            }

            motoRoom.refresh(pkRooms);
        });
    }

    // Add a client-side hub method that the server will call
    ticker.client.updatePKRoomInfo = function (pkRooms) {
        console.log(pkRooms);
        // test
        //pkRooms = {};
        //pkRooms.GamingSeconds = 10;
        //pkRooms.GamePassedSeconds = 0;
        //pkRooms.GameRemainSeconds = 20;
        //pkRooms.GameBeginTime = '2017/04/03 18:30:00';
        //pkRooms.PK = { PKId: 1, Ranks: '3,2,5,6,8,7,10,1,9,4', GameSeconds: 20 };

        //console.log(pkRooms);
        if (pkRooms == null) {
            return;
        }

        motoRoom.refresh(pkRooms);
    }

    // Start the connection
    $.connection.hub.start().done(init);

    // room
    var motoRoom = {
        Room: null,
        refresh: function (pkRooms) {
            motoRoom.Room = motoRoom.getCurrentRoom(pkRooms);

        },
        getCurrentRoom: function (pkRooms) {
            var room = null;
            for (var i = 0; i < pkRooms.length; i++) {
                if (pkRooms[i].RoomLevel == _roomLevel) {
                    room = pkRooms[i];
                    break;
                }
            }
            return room;
        },
        getDeskHtml: function (deskNum) {
            //deskNum: 桌号
            var html =
            '<div class="col-md-3">' +
            '    <a href="/Moto/Room/1/' + deskNum + '"><img src="~/img/room/desk_blue.png"></a>' +
            '    <div class="desk-no desk-no-' + deskNum + '">NO.' + deskNum + '</div>' +
            '    <img src="~/img/avatars/user1.jpg" class="desk-user desk-user-1" alt="avatar">' +
            '    <img src="~/img/avatars/user2.jpg" class="desk-user desk-user-2" alt="avatar">' +
            '    <img src="~/img/avatars/user3.jpg" class="desk-user desk-user-3" alt="avatar">' +
            '    <img src="~/img/avatars/user4.jpg" class="desk-user desk-user-4" alt="avatar">' +
            '    <img src="~/img/avatars/user5.jpg" class="desk-user desk-user-5" alt="avatar">' +
            '    <img src="~/img/avatars/user6.jpg" class="desk-user desk-user-6" alt="avatar">' +
            '    <img src="~/img/avatars/user7.jpg" class="desk-user desk-user-7" alt="avatar">' +
            '    <img src="~/img/avatars/user8.jpg" class="desk-user desk-user-8" alt="avatar">' +
            '    <img src="~/img/avatars/user9.jpg" class="desk-user desk-user-9" alt="avatar">' +
            '    <img src="~/img/avatars/user10.jpg" class="desk-user desk-user-10" alt="avatar">' +
            '</div>';
        },
        getAvatarHtml: function (users) {
            var html = '';

            for (var i = 0; i < 10; i++) {
                var num = i + 1;
                var user = motoRoom.getUserByNum(users, num);
                if (user != null) {
                    html += '<img src="' + user.Avatar + '" class="desk-user desk-user-' + num + '" alt="avatar">'
                }
            }

            return html;
        },
        getUserByNum: function (users, userNum) {
            //userNum: 桌子上第n位
            var user = null;
            for (var i = 0; i < users.length; i++) {
                if (users[i].Num == userNum) {
                    user = users[i];
                    break;
                }
            }
            return user;
        },
        //getRadomAvatarArrs: function () {
        //    var avatarArrs = motoRoom.getAvatarArrs();
        //    var randomAvatarArrs = $app.shuffle(avatarArrs);// 随机化原数组
        //    return randomAvatarArrs.slice(0, 10);   //返回前十个
        //},
        //getAvatarArrs: function () {
        //    var maxLen = 17;//目前有17个头像

        //    var avatarArrs = [];
        //    for (var i = 1; i <= maxLen; i++) {
        //        avatarArrs.push(i);
        //    }
        //    return avatarArrs;
        //},
    };

});