$(function () {

    var $racingResult = $("#racingResult");
    var $bonusResult = $("#bonusResult");

    var ticker = $.connection.pKTickerHub;

    function init() {
        ticker.server.getPKInfo().done(function (pkInfo) {
            console.log(pkInfo);
            //$stockTable.append("aaa");
            // test
            //pkInfo.GamingSeconds = -5;
            //pkInfo.GamePassedSeconds = 0;
            //pkInfo.GameRemainSeconds = 20;
            //pkInfo.PK = { PKId: 1, Ranks: '3,2,5,6,8,7,10,1,9,4', GameSeconds: 20 };

            motoRacing.run(pkInfo);
        });

        $racingResult.dialog({ autoOpen: false, modal: true, position: "center", width: 600, minHeight: 300, resizable: false, });
        $bonusResult.dialog({
            autoOpen: false, modal: true, position: "center", width: 600, minHeight: 300, resizable: false,
            buttons: {
                "确定": function () { $(this).dialog("close"); }
            }
        });
    }

    // Add a client-side hub method that the server will call
    ticker.client.updatePKInfo = function (pkInfo) {
        console.log(pkInfo);

        // test
        //pkInfo.GamingSeconds = -5;
        //pkInfo.GamePassedSeconds = 0;
        //pkInfo.GameRemainSeconds = 20;
        //pkInfo.PK = { PKId: 1, Ranks: '3,2,5,6,8,7,10,1,9,4', GameSeconds: 20 };

        motoRacing.run(pkInfo);
    }

    // Start the connection
    $.connection.hub.start().done(init);

    // moto
    var motoRacing = {
        PKInfo: null,
        Millisec: 5,
        RoadLength: 910,
        Colors: ['red', 'blue', 'yellow', 'green', 'gray', 'aqua', 'blueviolet', 'brown', 'Highlight', 'aquamarine', 'teal'],
        //Easings: ['easeInQuad', 'easeInQuart', 'easeInOutSine', 'easeOutSine', 'easeOutQuad', 'linear', 'easeInCirc', 'easeInOutQuad', 'easeOutCubic', 'easeInOutCubic'],
        Easings: ['easeInOutSine', 'easeOutSine', 'linear', 'swing', 'easeInOutSine', 'easeInOutQuad', 'swing', 'easeInOutSine', 'easeOutSine', 'easeInOutQuad'],
        MotoInitPosition: [],
        EndMotos: [],// motos that at the end 
        run: function (pkInfo) {
            if (pkInfo == null || pkInfo.PK.Ranks == null) {
                return;
            }
            // new racing
            if (motoRacing.PKInfo == null || pkInfo.PK.PKId != motoRacing.PKInfo.PK.PKId) {
                motoRacing.PKInfo = pkInfo;
                motoRacing.EndMotos = [];

                // result dialog
                $racingResult.dialog("close");

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
                html += '<img id="moto' + i + '" src="/img/moto-' + i + '.png" class="car-' + i + '" alt="' + i + '" />';
            }
            $('.car-list').html(html);
        },
        moveRoad: function (pkInfo) {
            var seconds = (pkInfo.GamingSeconds < 0 ? Math.abs(pkInfo.GamingSeconds) : 0) + 's';
            $('body').oneTime(seconds, function () {
                // road moving
                $('.saidao').floatingBg({ direction: 5, millisec: motoRacing.Millisec, backgroud: '/img/bg_saidao.jpg', });
                $('.bg-top').floatingBg({ direction: 5, millisec: motoRacing.Millisec, backgroud: '/img/bg_top.jpg', });
                $('.start-flag').floating({ direction: 'right', millisec: motoRacing.Millisec });
            });
        },
        runMoto: function (pkInfo) {
            var seconds = (pkInfo.GamingSeconds < 0 ? Math.abs(pkInfo.GamingSeconds) : 0) + 's';

            $('body').oneTime(seconds, function () {
                var speeds = motoRacing.calculateSpeeds();
                // moto init position
                motoRacing.MotoInitPosition = [];
                // run
                for (var i = 0; i < speeds.length; i++) {
                    var param = speeds[i].Rank != 10
                        ? {
                            duration: speeds[i].Duration, easing: speeds[i].Easing, complete: function () {
                                // when the first moto at the end, moveEndFlag
                                if (motoRacing.EndMotos.length == 0) {
                                    motoRacing.moveEndFlag(pkInfo);
                                }

                                motoRacing.EndMotos.push({
                                    Rank: motoRacing.EndMotos.length + 1,
                                    Num: parseInt($(this).attr('alt'))
                                });
                                console.log(motoRacing.EndMotos);
                            }
                        }
                        : {
                            duration: speeds[i].Duration, easing: speeds[i].Easing, complete: function () {
                                motoRacing.showFinalRanks(pkInfo);
                                motoRacing.clear();

                                motoRacing.showResult();
                            }
                        };

                    var $moto = $('#moto' + speeds[i].Num);
                    var pos = parseInt($moto.css('right'), 10);
                    $moto.animate({ right: motoRacing.RoadLength + pos }, param);

                    // moto init position
                    motoRacing.MotoInitPosition.push(pos);
                }

                // show ranks
                $('body').everyTime('1s', 'showRanks', function () {
                    motoRacing.showRanks();
                });
            });
        },
        moveEndFlag: function (pkInfo) {
            var $endFlag = $('.end-flag');

            $endFlag.floating({ direction: 'right', millisec: motoRacing.Millisec, position: 0 });

            $('body').everyTime('1ds', function () {
                if ($endFlag.css('left') == '0px') {
                    motoRacing.clear();
                }
            });
        },
        calculateSpeeds: function () {
            var speeds = [];
            var pkInfo = motoRacing.PKInfo;

            //ranks: 3,2,5,6,8,7,10,1,9,4
            var rankArr = pkInfo.PK.Ranks.split(',');

            // speeds
            for (var i = 0; i < rankArr.length; i++) {
                var motoNum = rankArr[i];
                var rank = i + 1;
                var random = motoRacing.getRandomNum(0, 9);

                var duration = (pkInfo.GameRemainSeconds > 10)
                    ? (pkInfo.PK.GameSeconds - 10 + rank) * 1000
                    : (pkInfo.GameRemainSeconds / 10) * rank * 1000;

                speeds.push({
                    'Num': motoNum,
                    'Rank': rank,
                    'Duration': duration,
                    'Easing': motoRacing.Easings[random]
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
                var motoNum = i + 1;
                var $moto = $('#moto' + motoNum);
                var pos = motoRacing.MotoInitPosition[i];
                var right = parseInt($moto.css('right'), 10);
                var currentDistance = right - pos;

                //if moto at the end, the rank of the moto will not be changed
                var endMoto = motoRacing.getEndMoto(motoNum);
                var distance = endMoto != null
                    ? currentDistance + (10 - endMoto.Rank) * 100
                    : currentDistance;

                ranks.push({
                    Num: i + 1,
                    Distance: distance
                });
            }

            //desc order
            ranks.sort(function (a, b) {
                return b.Distance - a.Distance
            });

            return ranks;
        },
        getEndMoto: function (motoNum) {
            var moto = null;
            for (var i = 0; i < motoRacing.EndMotos.length; i++) {
                if (motoRacing.EndMotos[i].Num == motoNum) {
                    moto = motoRacing.EndMotos[i];
                    break;
                }
            }
            return moto;
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
                    Distance: 0
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
        showResult: function () {
            var ranks = motoRacing.PKInfo.PK.Ranks;

            var html = '';
            var ranksArr = ranks.split(',');
            for (var i = 0; i < ranksArr.length; i++) {
                html += '<li>'
                      + '   <span>第' + (i + 1) + '名</span>'
                      + '   <span>' + ranksArr[i] + '号车</span>'
                      + '</li>';
            }
            html = '<ul>' + html + '</ul>';

            $(".ui-dialog-titlebar").addClass('hide');
            $racingResult.html(html);
            $racingResult.dialog("open");
        },
        showBonus: function () {
            var html = '奖金 10000';
            $dialog.html(html);
            $dialog.dialog("open");
        },
    };
});

var racingOpt = {
    showResult: function (ranks) {
        ranks = '3,2,5,6,8,7,10,1,9,4';

        var $dialog = $("#racingResult");

        var html = '';
        var ranksArr = ranks.split(',');
        for (var i = 0; i < ranksArr.length; i++) {
            html += '<li>'
                  + '   <span>第' + (i + 1) + '名</span>'
                  + '   <span>' + ranksArr[i] + '号车</span>'
                  + '</li>';
        }
        html = '<ul>' + html + '</ul>';

        //$(".ui-dialog-titlebar").addClass('hide');
        $dialog.html(html);
        $dialog.dialog("open");
    },
    showBonus: function () {
        var html = '奖金 10000';
        var $dialog = $("#bonusResult");

        //var ranksArr = ranks.split(',');
        //for (var i = 0; i < ranksArr.length; i++) {
        //    html += '<li>'
        //          + '   <span>第' + (i + 1) + '名</span>'
        //          + '   <span>' + ranksArr[i] + '号车</span>'
        //          + '</li>';
        //}
        //html = '<ul>' + html + '</ul>';

        //$(".ui-dialog-titlebar").addClass('hide');
        $dialog.html(html);
        $dialog.dialog("open");
    },
};