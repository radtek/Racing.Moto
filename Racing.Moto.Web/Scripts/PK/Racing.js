$(function () {
    var $elememts = $('.game-wrap').html();
    var $racingResult = $("#racingResult");
    var $bonusResult = $("#bonusResult");

    var ticker = $.connection.pKTickerHub;

    function init() {
        ticker.server.getPKInfo().done(function (pkInfo) {
            console.log(pkInfo);

            if (pkInfo == null) {
                return;
            }

            // test
            //pkInfo.GamingSeconds = -5;
            //pkInfo.GamePassedSeconds = 0;
            //pkInfo.GameRemainSeconds = 20;
            //pkInfo.PK = { PKId: 1, Ranks: '3,2,5,6,8,7,10,1,9,4', GameSeconds: 20 };

            motoRacing.run(pkInfo);
        });
    }

    // Add a client-side hub method that the server will call
    ticker.client.updatePKInfo = function (pkInfo) {
        console.log(pkInfo);
        if (pkInfo == null) {
            return;
        }

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
        RoadLength: 910,//910
        Colors: ['red', 'blue', 'yellow', 'green', 'gray', 'aqua', 'blueviolet', 'brown', 'Highlight', 'aquamarine', 'teal'],
        //Easings: ['easeInQuad', 'easeInQuart', 'easeInOutSine', 'easeOutSine', 'easeOutQuad', 'linear', 'easeInCirc', 'easeInOutQuad', 'easeOutCubic', 'easeInOutCubic'],
        Easings: ['easeInQuart', 'easeInOutExpo', 'easeOutCirc', 'swing', 'easeOutExpo', 'easeOutBack', 'swing', 'easeInOutSine', 'easeOutSine', 'easeInOutQuad'],
        MotoInitPosition: [],
        EndMotos: [],// motos that at the end 
        resetElements: function () {
            $('.game-wrap').html($elememts);
        },
        run: function (pkInfo) {
            if (pkInfo != null) {
                // countdown
                motoRacing.countdownClock(pkInfo);
            }

            if (pkInfo == null || pkInfo.PK.Ranks == null) {
                return;
            }

            // new racing
            if (motoRacing.PKInfo == null || pkInfo.PK.PKId != motoRacing.PKInfo.PK.PKId) {
                motoRacing.PKInfo = pkInfo;
                motoRacing.EndMotos = [];

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
        countdownClock: function (pkInfo) {
            var eleHour = document.getElementById('hour');
            var eleMinute = document.getElementById('minute');
            var eleSecond = document.getElementById('second');

            if (pkInfo.OpeningRemainSeconds <= 0) {
                eleHour.innerHTML = '00';
                eleMinute.innerHTML = '00';
                eleSecond.innerHTML = '00';
            } else {
                var closeBeginTime = $app.convertToDate(pkInfo.GameBeginTime);
                var year = closeBeginTime.getFullYear();
                var month = closeBeginTime.getMonth();
                var day = closeBeginTime.getDate();
                var hour = closeBeginTime.getHours();
                var minute = closeBeginTime.getMinutes();
                var second = closeBeginTime.getSeconds();

                // 倒计时结束时间
                var d = Date.UTC(year, month, day, hour, minute, second);
                var obj = {
                    sec: eleSecond,
                    mini: eleMinute,
                    hour: eleHour
                }
                fnTimeCountDown(d, obj);
            }
        },
        countdown: function (pkInfo) {
            var countdownSeconds = 4;
            var seconds = pkInfo.GamingSeconds < -countdownSeconds ? Math.abs(pkInfo.GamingSeconds + countdownSeconds) : 0;
            if (pkInfo.GamingSeconds < -countdownSeconds) {
                var seconds = Math.abs(pkInfo.GamingSeconds + countdownSeconds) + 's';
                $('body').oneTime(seconds, function () {
                    $('.time-run2').hide();
                    $('.time-run').show();
                    var index = 3;
                    $('body').everyTime('1s', 'countdown', function () {
                        var name = 'time-' + (index >= 0 ? index : 0);
                        document.getElementById("countdown").src = '/img/' + name + ".png";
                        index--;
                        if (index < -1) {
                            //$('.time-run2').show();
                            $('.time-run').hide();
                        }
                    }, 5);
                });
            } else if (-countdownSeconds < pkInfo.GamingSeconds && pkInfo.GamingSeconds < 0) {
                $('.time-run2').hide();
                $('.time-run').show();
                var seconds = Math.abs(pkInfo.GamingSeconds);
                var index = seconds;
                $('body').everyTime('1s', 'countdown', function () {
                    var name = 'time-' + (index >= 1 ? index : 1);
                    document.getElementById("countdown").src = '/img/' + name + ".png";
                    index--;
                    if (index < 0) {
                        //$('.time-run2').show();
                        $('.time-run').hide();
                    }
                }, seconds + 1);
            } else {
                $('.time-run2').show();
                $('.time-run').hide();
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
            console.log('GamingSeconds:' + seconds);
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

                                // when moto and the end, continue to move some instance
                                //motoRacing.moveEndDistance($(this).attr('alt'), $(this).css('right'));
                            }
                        }
                        : {
                            duration: speeds[i].Duration, easing: speeds[i].Easing, complete: function () {
                                motoRacing.showFinalRanks(pkInfo);
                                motoRacing.clear();

                                motoRacing.showResult();

                                // when moto and the end, continue to move some instance
                                //motoRacing.moveEndDistance($(this).attr('alt'), $(this).css('right'));
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
        moveEndDistance: function (num, right) {
            // when moto and the end, continue to move some instance
            var $moto = $('#moto' + num);
            right = parseInt(right, 10) + 150;
            $moto.animate({ right: right + 'px' }, 1000);
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
                    ? (pkInfo.PK.GameSeconds - 10 + rank) * 500
                    : (pkInfo.GameRemainSeconds / 10) * rank * 500;

                speeds.push({
                    'Num': motoNum,
                    'Rank': rank,
                    'Duration': duration,
                    'Easing': motoRacing.Easings[random]
                });
            }
            //console.log(speeds);
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
            var ranks = motoRacing.getFinalRanks(pkInfo);
            motoRacing.appendRanks(ranks);
        },
        getFinalRanks: function (pkInfo) {
            //ranks: 3,2,5,6,8,7,10,1,9,4
            var rankArr = pkInfo.PK.Ranks.split(',');

            var ranks = [];
            for (var i = 0; i < rankArr.length; i++) {
                ranks.push({
                    Num: rankArr[i],
                    Distance: 0
                });
            }
            return ranks;
        },
        clear: function () {
            $('.saidao').floatingBg('destroy');
            $('.bg-top').floatingBg('destroy');
            $('.start-flag').floatingBg('destroy');

            $('body').stopTime();
        },
        showResult: function () {
            var ranks = motoRacing.PKInfo.PK.Ranks;

            var html = motoRacing.getResultHtml();

            //$(".ui-dialog-titlebar").addClass('hide');
            $racingResult.html(html);
            $racingResult.removeClass("hide");

            // close result dialog after 5 seconds, then open bonus dialog
            $('body').oneTime('5s', function () {
                // close
                //$racingResult.dialog("close");
                $racingResult.addClass("hide");
                // open
                motoRacing.showBonus();
            });

            // close bonus dialog after 60 seconds
            $('body').oneTime('60s', function () {
                // close
                $bonusResult.addClass("hide");
                // resetElements
                motoRacing.resetElements();
            });
        },
        getResultHtml: function () {
            var ranks = motoRacing.PKInfo.PK.Ranks;

            var title = '<a href="javacript:;" class="close"><img src="/img/del.png"></a>'
                     + '<div class="title">第' + motoRacing.PKInfo.PK.PKId + '期比赛结果</div>';       
    
            var li = '';
            var ranksArr = ranks.split(',');
            for (var i = 0; i < ranksArr.length; i++) {
                //<li class="car_1"><span class="mingci" style="color: #fe0000">第1名</span><span class="time">48'53</span><img src="img/1.png" class="carZ"></li>
                var style = (i < 3) ? 'style="color: #fe0000"' : '';
                li += '<li class="car_' + (i + 1) + '">'
                      + '   <span class="mingci" ' + style + '>第' + (i + 1) + '名</span>'
                      + '   <span class="time">' + ranksArr[i] + '</span>'
                      + '   <img src="/img/' + ranksArr[i] + '.png" class="carZ">'
                      + '</li>';
            }

            var ul = '<ul class="list">'
                    + '  <div class="jiangpai"><img src="/img/jiangpai.png"></div>'
                    + li
                    + '</ul>';

            return title + ul;
        },
        showBonus: function () {
            $.ajax({
                type: 'POST',
                url: '/api/bonus/getBonus',
                data: { PKId: motoRacing.PKInfo.PK.PKId, UserId: $('#hidUserId').val() },
                success: function (res) {
                    if (res.Success) {
                        // test
                        //res.Data = [
                        //    { Rank: 1, Num: 2, Amount: 100 },
                        //    { Rank: 2, Num: 3, Amount: 200 },
                        //    { Rank: 5, Num: 6, Amount: 500 }
                        //];
                        var html = motoRacing.getBonusHtml(res.Data);
                        console.log(html);
                        $bonusResult.html(html);
                        $bonusResult.removeClass("hide");

                        $('.close-bonus').click(function () {
                            $bonusResult.addClass("hide");
                        });
                    }
                },
                //dataType: dataType
            });
        },
        getBonusRanksHtml: function () {
            var html = '';
            var ranks = motoRacing.PKInfo.PK.Ranks.split(',');
            for (var i = 0; i < ranks.length; i++) {
                html += '<div class="shishi-' + ranks[i] + ' color-' + ranks[i] + '">' + ranks[i] + '</div>';
            }
            return html;
        },
        getBonusResultHtml: function (bonusArr) {
            var html = '';
            for (var i = 0; i < bonusArr.length; i++) {
                html += '<li>'
                      + '   <span>【第' + bonusArr[i].Rank + '名】</span>'
                      + '   <span style="margin-left: 20px;">(' + bonusArr[i].Num + '号)</span>'
                      + '   <span style="margin-left: 20px;">获胜奖金: ' + bonusArr[i].Amount + '</span>'
                      + '</li>';
            }
            return html;
        },
        getBonusHtml: function (bonusArr) {
            var totalBonus = 0;
            for (var i = 0; i < bonusArr.length; i++) {
                totalBonus += bonusArr[i].Amount;
            }
            var ranksHtml = motoRacing.getBonusRanksHtml();
            var bonusResultHtml = motoRacing.getBonusResultHtml(bonusArr);
            var html = '<a href="javacript:;" class="close close-bonus"><img src="/img/del.png"></a>'
                     + '<a href="javacript:;" class="queding close-bonus"><img src="/img/btn-queding.png"></a>'
                     + '<div class="huodejiangjin">获得奖金：' + totalBonus + '</div>'
                     + '<div class="jieguo">'
                     + '    <div class="text">结果</div>'
                     + '    <div class="shishi z-shishi">' + ranksHtml + '</div>'
                     + '</div>'
                     + '<div class="jieguo-2">'
                     + '    <ul>' + bonusResultHtml + '</ul>'
                     + '</div>';

            return html;
        },
    };
});