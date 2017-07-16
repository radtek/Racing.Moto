var $deskes = $("#divDesks");
var _roomLevel = $("#hidRoomLevel").val();

$(function () {
    //var $elememts = $('.game-wrap').html();
    $deskes = $("#divDesks");
    _roomLevel = $("#hidRoomLevel").val();

    var ticker = $.connection.pKRoomTickerHub;

    function init() {
        ticker.server.getPKRoomInfo().done(function (pkRooms) {
            console.log(pkRooms);

            // test
            //pkRooms[0].RoomDesks[0] = { RoomLevel: 1, RoomDeskId: 1 };
            //pkRooms[0].RoomDesks[0].Users = [
            //    { UserId: 1, UserName: 'user01', Avatar: '/img/avatars/user1.jpg', Num: 1 },
            //    { UserId: 2, UserName: 'user02', Avatar: '/img/avatars/user11.jpg', Num: 8 }
            //];
            //pkRooms[0].RoomDesks[3] = { RoomLevel: 1, RoomDeskId: 4 };
            //pkRooms[0].RoomDesks[3].Users = [
            //    { UserId: 3, UserName: 'user03', Avatar: '/img/avatars/user5.jpg', Num: 2 },
            //    { UserId: 4, UserName: 'user04', Avatar: '/img/avatars/user13.jpg', Num: 5 }
            //];


            if (pkRooms == null) {
                return;
            }

            motoRoom.refresh(pkRooms);
        });
    }

    // Add a client-side hub method that the server will call
    ticker.client.updatePkRoomInfo = function (pkRooms) {
        console.log(pkRooms);

        //return;

        //// test
        //pkRooms[0].RoomDesks[0].Users = [
        //    { UserId: 1, UserName: 'user01', Avatar: '/img/avatars/user1.jpg', Num: 1 },
        //    { UserId: 2, UserName: 'user02', Avatar: '/img/avatars/user11.jpg', Num: 8 }
        //];
        //pkRooms[0].RoomDesks[3].Users = [
        //    { UserId: 3, UserName: 'user03', Avatar: '/img/avatars/user5.jpg', Num: 2 },
        //    { UserId: 4, UserName: 'user04', Avatar: '/img/avatars/user13.jpg', Num: 5 }
        //];

        //console.log(pkRooms);
        if (pkRooms == null) {
            return;
        }

        motoRoom.refresh(pkRooms);
    }

    // Start the connection
    $.connection.hub.start().done(init);

});

// room
var motoRoom = {
    CurrentRoom: null,
    refresh: function (pkRooms) {
        motoRoom.CurrentRoom = motoRoom.getCurrentRoom(pkRooms);

        var roomHtml = '';
        for (var i = 0; i < motoRoom.CurrentRoom.RoomDesks.length; i++) {
            roomHtml += motoRoom.getDeskHtml(motoRoom.CurrentRoom.RoomDesks[i]);
        }

        $deskes.html(roomHtml);
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
    getDeskHtml: function (desk) {
        var roomLevel = desk.RoomLevel;//roomLevel: 房间号
        var deskNum = desk.RoomDeskId;//deskNum: 桌号
        var avatarHtml = motoRoom.getAvatarHtml(desk.Users);
        var deskImgHtml = desk.Users.length == 10 ? '<img src="/img/room/desk_red.png" />' : '<img src="/img/room/desk_blue.png" />';

        var deskHtml =
        '<div class="col-md-3">' +
        '    <a href="javascript:;" onclick="motoRoom.join(' + roomLevel + ',' + deskNum + ')">' + deskImgHtml + '</a>' +
        '    <div class="desk-no">NO.' + deskNum + '</div>' +
             avatarHtml +
        '</div>';

        return deskHtml;
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
    join: function (roomLevel, deskNo) {
        console.log(roomLevel);
        console.log(deskNo);
        $.ajax({
            type: 'POST',
            url: '/Moto/Join',
            data: { RoomLevel: roomLevel, DeskNo: deskNo },
            success: function (res) {
                if (res.Success) {
                    location.href = "/Moto/Arena/" + roomLevel + "/" + deskNo;
                } else {
                    alert(res.Message);
                }
            }
        });
    },
};