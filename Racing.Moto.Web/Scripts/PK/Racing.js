$(function () {

    var ticker = $.connection.pKTickerHub;

    function init() {
        ticker.server.getPKInfo().done(function (pkInfo) {
            //console.log(pkInfo);
            $stockTable.append("aaa");
        });
    }

    // Add a client-side hub method that the server will call
    ticker.client.updatePKInfo = function (pkInfo) {
        // test
        pkInfo.RemainSeconds = 25;
        pkInfo.PK = { PKId: 1, Ranks: '3,2,5,6,8,7,10,1,9,4', GameSeconds: 30 };

        motoRacing.run(pkInfo);
    }

    // Start the connection
    $.connection.hub.start().done(init);

    // moto
    var motoRacing = {
        PKInfo: null,
        RoadLength: 1000,
        Colors: ['red', 'blue', 'yellow', 'green', 'gray', 'aqua', 'blueviolet', 'brown', 'Highlight', 'aquamarine', 'teal'],
        //Easings: ['easeInQuad', 'easeInQuart', 'easeInOutSine', 'easeOutSine', 'easeOutQuad', 'linear', 'easeInCirc', 'easeInOutQuad', 'easeOutCubic', 'easeInOutCubic'],
        Easings: ['easeInOutSine', 'easeOutSine', 'linear', 'swing', 'easeInOutSine', 'easeInOutQuad', 'swing', 'easeInOutSine', 'easeOutSine', 'easeInOutQuad'],
        run: function (pkInfo) {
            // new racing
            if (motoRacing.PKInfo == null || pkInfo.PK.PKId != motoRacing.PKInfo.PK.PKId) {
                motoRacing.PKInfo = pkInfo;

                // road moving
                $('#roadBg').floatingBg({ direction: 5, speed: 5, backgroud: '/images/road.png', });

                // moto append
                motoRacing.append();
                // moto moving
                motoRacing.motoRun();
            }
        },
        append: function () {
            var html = '';
            for (var i = 0; i < 10; i++) {
                var top = (100 + 20 * i) + 'px;';
                var bg = motoRacing.Colors[i];
                html += '<div id="moto' + (i + 1) + '" class="moto" style="top:' + top + ' background-color:' + bg + '"></div>';
            }
            $('#motos').html(html);
        },
        motoRun: function () {
            var speeds = motoRacing.calculateSpeeds();
            // run
            for (var i = 0; i < speeds.length; i++) {
                var param = speeds[i].Rank != 10
                    ? { duration: speeds[i].Duration, easing: speeds[i].Easing }
                    : {
                        duration: speeds[i].Duration, easing: speeds[i].Easing, complete: function () {
                            $('#roadBg').floatingBg('destroy');
                        }
                    };

                $('#moto' + (i + 1)).animate({ right: motoRacing.RoadLength }, param);
            }
        },
        calculateSpeeds: function () {
            var speeds = [];
            var pkInfo = motoRacing.PKInfo;

            //ranks: 3,2,5,6,8,7,10,1,9,4
            var rankArr = pkInfo.PK.Ranks.split(',');

            // speeds
            for (var i = 0; i < rankArr.length; i++) {
                var num = motoRacing.getRandomNum(0, 9);

                var duration = (pkInfo.RemainSeconds > 10)
                    ? (pkInfo.PK.GameSeconds - 10 + parseInt(rankArr[i])) * 1000
                    : (pkInfo.RemainSeconds / 10) * parseInt(rankArr[i]) * 1000;

                speeds.push({
                    'Rank': rankArr[i],
                    'Duration': duration,
                    'Easing': motoRacing.Easings[num]
                });
            }
            console.log(speeds);
            return speeds;
        },
        getRandomNum: function (min, max) {
            var range = max - min;
            var rand = Math.random();
            return (min + Math.round(rand * range));
        }
    };

});