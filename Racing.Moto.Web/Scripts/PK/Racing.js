$(function () {

    var ticker = $.connection.pKTickerHub;

    function init() {
        ticker.server.getPKInfo().done(function (pkInfo) {
            //console.log(pkInfo);
            //$stockTable.append("aaa");
        });
    }

    // Add a client-side hub method that the server will call
    ticker.client.updatePKInfo = function (pkInfo) {
        // test
        pkInfo.GamingSeconds = -5;
        pkInfo.GamePassedSeconds = 0;
        pkInfo.GameRemainSeconds = 20;
        pkInfo.PK = { PKId: 1, Ranks: '3,2,5,6,8,7,10,1,9,4', GameSeconds: 20 };

        motoRacing.run(pkInfo);
    }

    // Start the connection
    $.connection.hub.start().done(init);

    // moto
    var motoRacing = {
        PKInfo: null,
        RoadLength: 910,
        Colors: ['red', 'blue', 'yellow', 'green', 'gray', 'aqua', 'blueviolet', 'brown', 'Highlight', 'aquamarine', 'teal'],
        //Easings: ['easeInQuad', 'easeInQuart', 'easeInOutSine', 'easeOutSine', 'easeOutQuad', 'linear', 'easeInCirc', 'easeInOutQuad', 'easeOutCubic', 'easeInOutCubic'],
        Easings: ['easeInOutSine', 'easeOutSine', 'linear', 'swing', 'easeInOutSine', 'easeInOutQuad', 'swing', 'easeInOutSine', 'easeOutSine', 'easeInOutQuad'],
        MotoInitPosition: [],
        run: function (pkInfo) {
            // new racing
            if (motoRacing.PKInfo == null || pkInfo.PK.PKId != motoRacing.PKInfo.PK.PKId) {
                motoRacing.PKInfo = pkInfo;

                // append moto
                motoRacing.append();
                // countdown
                motoRacing.countdown(pkInfo);
                // move Road
                motoRacing.moveRoad(pkInfo);
                // run moto
                motoRacing.runMoto(pkInfo);
            }
        },
        countdown: function (pkInfo) {
            var seconds = pkInfo.GamingSeconds < -4 ? Math.abs(pkInfo.GamingSeconds + 4) : 0;
            if (pkInfo.GamingSeconds < -4) {
                var seconds = Math.abs(pkInfo.GamingSeconds + 4) + 's';
                $('body').oneTime(seconds, function () {
                    $('.time-run').show();
                    var index = 3;
                    $('body').everyTime('1s', 'countdown', function () {
                        //var name = 'time-' + (index >= 0 ? index : 0);
                        var name = 'time-' + (index >= 1 ? index : 1);
                        document.getElementById("countdown").src = '/img/' + name + ".png";
                        index--;
                        if (index < 0) {
                            $('.time-run').hide();
                        }
                    }, 5);
                });
            } else if (-4 < pkInfo.GamingSeconds && pkInfo.GamingSeconds < 0) {
                $('.time-run').show();
                var seconds = Math.abs(pkInfo.GamingSeconds);
                var index = seconds;
                $('body').everyTime('1s', 'countdown', function () {
                    var name = 'time-' + (index >= 1 ? index : 1);
                    document.getElementById("countdown").src = '/img/' + name + ".png";
                    index--;
                    if (index < 0) {
                        $('.time-run').hide();
                    }
                }, seconds + 1);
            }
        },
        append: function () {
            var html = '';
            for (var i = 10; i > 0; i--) {
                html += '<img id="moto' + i + '" src="/img/moto-' + i + '.png" class="car-' + i + '" alt="moto' + i + '" />';
            }
            $('.car-list').html(html);
        },
        moveRoad: function (pkInfo) {
            var seconds = (pkInfo.GamingSeconds < 0 ? Math.abs(pkInfo.GamingSeconds) : 0) + 's';
            $('body').oneTime(seconds, function () {
                // road moving
                $('.saidao').floatingBg({ direction: 5, millisec: 5, backgroud: '/img/bg_saidao.jpg', });
                $('.bg-top').floatingBg({ direction: 5, millisec: 5, backgroud: '/img/bg_top.jpg', });
                $('.start-flag').floating({ direction: 'right', millisec: 5 });
            });
        },
        runMoto: function (pkInfo) {
            //$('.start-flag').addClass('hide');
            //$('.end-flag').removeClass('hide');

            var seconds = (pkInfo.GamingSeconds < 0 ? Math.abs(pkInfo.GamingSeconds) : 0) + 's';

            $('body').oneTime(seconds, function () {
                var speeds = motoRacing.calculateSpeeds();
                // moto init position
                motoRacing.MotoInitPosition = [];
                // run
                for (var i = 0; i < speeds.length; i++) {
                    var param = speeds[i].Rank != 10
                        ? { duration: speeds[i].Duration, easing: speeds[i].Easing }
                        : {
                            duration: speeds[i].Duration, easing: speeds[i].Easing, complete: function () {
                                motoRacing.moveEndFlag(pkInfo);
                                motoRacing.showFinalRanks(pkInfo);
                                motoRacing.clear();
                            }
                        };

                    var $moto = $('#moto' + (i + 1));
                    var right = parseInt($moto.css('right'), 10);
                    $moto.animate({ right: motoRacing.RoadLength }, param);

                    // moto init position
                    motoRacing.MotoInitPosition.push(right - speeds[i].Rank);
                }

                // show ranks
                $('body').everyTime('1s', 'showRanks', function () {
                    motoRacing.showRanks();
                });
            });
        },
        moveEndFlag: function (pkInfo) {
            // run end-flag floating when 1 secondes left            
            //var seconds = (pkInfo.PK.GameSeconds - 1 > 0) ? (pkInfo.PK.GameSeconds - 1) * 1000 + 500 + 'ms' : '500ms';
            //var duration = (pkInfo.PK.GameSeconds - 1 > 0) ? 1000 : 500;
            //var seconds = '1s';

            //$('body').oneTime(seconds, function () {
            //});
            // move 400px
            $('.end-flag').animate({ left: 0 }, { duration: 1000, easing: 'linear' });
        },
        calculateSpeeds: function () {
            var speeds = [];
            var pkInfo = motoRacing.PKInfo;

            //ranks: 3,2,5,6,8,7,10,1,9,4
            var rankArr = pkInfo.PK.Ranks.split(',');

            // speeds
            for (var i = 0; i < rankArr.length; i++) {
                var num = motoRacing.getRandomNum(0, 9);

                var duration = (pkInfo.GameRemainSeconds > 10)
                    ? (pkInfo.PK.GameSeconds - 10 + parseInt(rankArr[i])) * 1000
                    : (pkInfo.GameRemainSeconds / 10) * parseInt(rankArr[i]) * 1000;

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
        },
        calculateRanks: function () {
            var ranks = [];
            for (var i = 0; i < 10; i++) {
                var $moto = $('#moto' + (i + 1));
                var pos = motoRacing.MotoInitPosition[i];
                var right = parseInt($moto.css('right'), 10);

                ranks.push({
                    Num: i + 1,
                    Right: right - pos
                });
            }
            console.log(ranks);

            //desc order
            ranks.sort(function (a, b) {
                return b.Right - a.Right
            });

            console.log(ranks);

            return ranks;
        },
        showRanks: function () {
            var ranks = motoRacing.calculateRanks();

            motoRacing.appendRanks(ranks);
        },
        appendRanks: function (ranks) {
            var html = '<div style="margin-left:184px; ">&nbsp;</div>';
            for (var i = 0; i < ranks.length; i++) {
                html += '<div class="shishi-' + ranks[i].Num + ' color-' + ranks[i].Num + '">' + ranks[i].Num + '</div>';
            }
            $('#ranks').html(html);
        },
        showFinalRanks: function (pkInfo) {
            //ranks: 3,2,5,6,8,7,10,1,9,4
            var rankArr = pkInfo.PK.Ranks.split(',');

            var ranks = [];
            for (var i = 0; i < rankArr.length; i++) {
                ranks.push({
                    Num: rankArr[i],
                    Right: 0
                });
            }

            motoRacing.appendRanks(ranks);
        },
        clear: function () {
            $('.saidao').floatingBg('destroy');
            $('.bg-top').floatingBg('destroy');
            $('.start-flag').floatingBg('destroy');

            $('body').stopTime();
        },
    };

});