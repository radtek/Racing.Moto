$(function () {
    var $elememts = $('.game-wrap').html();
    var $racingResult = $("#racingResult");
    var $bonusResult = $("#bonusResult");

    var ticker = $.connection.pKTickerHub;

    function init() {
        ticker.server.getPKInfo().done(function (pkInfo) {
            console.log(pkInfo);

            // test
            //pkInfo = {};
            //pkInfo.GamingSeconds = -1;
            //pkInfo.GamePassedSeconds = 0;
            //pkInfo.GameRemainSeconds = 50;
            //pkInfo.GameBeginTime = '2017/04/03 18:20:00';
            //pkInfo.PK = { PKId: 1, Ranks: '3,2,5,6,8,7,10,1,9,4', GameSeconds: 20 };


            if (pkInfo == null) {
                return;
            }
            motoRacing.run(pkInfo);
        });
    }

    // Add a client-side hub method that the server will call
    ticker.client.updatePKInfo = function (pkInfo) {
        // test
        //pkInfo.GamingSeconds = -5;
        //pkInfo.GamePassedSeconds = 0;
        //pkInfo.GameRemainSeconds = 20;
        //pkInfo.PK = { PKId: 1, Ranks: '3,2,5,6,8,7,10,1,9,4', GameSeconds: 20 };

        console.log(pkInfo);
        if (pkInfo == null) {
            return;
        }

        motoRacing.run(pkInfo);
    }

    // Start the connection
    $.connection.hub.start().done(init);

    // moto
    var motoRacing = {
        PKInfo: null,
        ResultShowSeconds: 5,
        BonusShowSeconds: 30,
        Millisec: 3,
        RoadLength: 610,//910
        Colors: ['red', 'blue', 'yellow', 'green', 'gray', 'aqua', 'blueviolet', 'brown', 'Highlight', 'aquamarine', 'teal'],
        //Easings: ['easeInQuad', 'easeInQuart', 'easeInOutSine', 'easeOutSine', 'easeOutQuad', 'linear', 'easeInCirc', 'easeInOutQuad', 'easeOutCubic', 'easeInOutCubic'],
        //Easings: ['easeInQuart', 'easeInOutExpo', 'easeOutCirc', 'swing', 'easeOutExpo', 'easeOutBack', 'swing', 'easeInOutSine', 'easeOutSine', 'easeInOutQuad'],
        Easings: ['easeInBounce', 'easeOutBounce', 'easeInOutBounce', 'easeInElastic', 'easeInBounce', 'easeOutBounce', 'easeInOutBounce', 'easeInElastic', 'easeInBack', 'easeInOutBounce'],
        Durations: [],
        StepDurations: { Step1: [], Step2: [], Step3: [] },
        MotoInitPosition: [],
        EndMotos: [],// motos that at the end 
        resetElements: function () {
            $('.game-wrap').html($elememts);
        },
        resetMoto: function (num) {
            var $moto = $('#moto' + num);
            $moto[0].src = '/img/moto-' + num + ".png";
            $moto.addClass('car-' + num).removeClass('car-run-' + num);
        },
        run: function (pkInfo) {
            if (pkInfo != null) {
                // countdown
                var clock = motoRacing.getcountdownClock();
                if (clock == '::' || clock == '00:00:00') {
                    motoRacing.countdownClock(pkInfo);
                }
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
                //motoRacing.runMoto(pkInfo);
                motoRacing.runMoto2(pkInfo);
            }
        },
        getcountdownClock: function () {
            var eleHour = document.getElementById('hour').innerHTML;
            var eleMinute = document.getElementById('minute').innerHTML;
            var eleSecond = document.getElementById('second').innerHTML;

            return eleHour + ':' + eleMinute + ':' + eleSecond;
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
            var countdownSeconds = 5;
            var toGamingSeconds = Math.abs(pkInfo.GamingSeconds);
            //var seconds = countdownSeconds < toGamingSeconds ? toGamingSeconds - countdownSeconds : 0;
            if (countdownSeconds <= toGamingSeconds && pkInfo.GamingSeconds < 0) {
                var seconds = Math.abs(pkInfo.GamingSeconds + countdownSeconds) + 's';
                $('body').oneTime(seconds, function () {
                    $('.time-run2').hide();
                    $('.time-run').show();

                    motoRacing.countdown2(countdownSeconds, 4);
                });
            } else if (countdownSeconds > toGamingSeconds && pkInfo.GamingSeconds < 0) {
                $('.time-run2').hide();
                $('.time-run').show();

                motoRacing.countdown2(toGamingSeconds, toGamingSeconds - 1);
            } else {
                $('.time-run2').show();
                $('.time-run').hide();
            }
        },
        countdown2: function (countdownSeconds, index) {
            //var index = 4;
            $('body').everyTime('1s', 'countdown', function () {
                if (index <= 0) {
                    //$('.time-run2').show();
                    $('.time-run').hide();
                } else {
                    var number = index;
                    if (number <= 1) {
                        $('.time-run').addClass('time-run-go');
                    } else {
                        $('.time-run').removeClass('time-run-go');
                    }
                    var name = 'countdown-' + (number >= 1 ? number : 1);
                    document.getElementById("countdown").src = '/img/' + name + ".png";
                }
                //console.log(index);

                index = index - 1;
            }, countdownSeconds);
        },
        append: function () {
            var html = '';
            for (var i = 10; i > 0; i--) {
                html += '<img id="moto' + i + '" src="/img/moto-' + i + '.png" class="car-' + i + '" alt="' + i + '" />';
            }
            $('.car-list').html(html);
        },
        appendRunMotos: function () {
            var html = '';
            for (var i = 10; i > 0; i--) {
                html += '<img id="moto' + i + '" src="/img/moto-' + i + '.run.gif" class="car-run-' + i + '" alt="' + i + '" />';
            }
            $('.car-list').html(html);
        },
        moveRoad: function (pkInfo) {
            var seconds = (pkInfo.GamingSeconds < 0 ? Math.abs(pkInfo.GamingSeconds) : 0) + 's';
            //console.log('GamingSeconds:' + seconds);
            $('body').oneTime(seconds, function () {
                // road moving
                $('.saidao').floatingBg({ direction: 5, millisec: motoRacing.Millisec, backgroud: '/img/bg_saidao.jpg', });
                $('.bg-top').floatingBg({ direction: 5, millisec: motoRacing.Millisec, backgroud: '/img/bg_top.jpg', });
                $('.start-flag').floating({ direction: 'right', millisec: motoRacing.Millisec });

                $('body').oneTime(5000, function () {
                    // road moving
                    //$('.start-flag').floating({ direction: 'right', millisec: motoRacing.Millisec });

                    $('.start-flag').floating('destroy');
                });
            });
        },
        runMoto: function (pkInfo) {
            var seconds = (pkInfo.GamingSeconds < 0 ? Math.abs(pkInfo.GamingSeconds) : 0) + 's';

            $('body').oneTime(seconds, function () {
                var speeds = motoRacing.calculateSpeeds();
                // moto init position
                motoRacing.MotoInitPosition = motoRacing.getMotoInitPositions();
                // moto running 
                motoRacing.appendRunMotos();

                // run
                for (var i = 0; i < speeds.length; i++) {
                    var param = speeds[i].Rank != 10
                        ? {
                            duration: speeds[i].Duration, easing: speeds[i].Easing, complete: function () {
                                // when the first moto at the end, moveEndFlag
                                if (motoRacing.EndMotos.length == 0) {
                                    motoRacing.moveEndFlag(pkInfo);
                                }

                                var num = $(this).attr('alt');
                                motoRacing.EndMotos.push({
                                    Rank: motoRacing.EndMotos.length + 1,
                                    Num: parseInt(num)
                                });
                                //console.log(motoRacing.EndMotos);

                                // when moto and the end, continue to move some instance
                                motoRacing.moveEndDistance($(this).attr('alt'), $(this).css('right'));

                                //resetMoto
                                motoRacing.resetMoto(num);
                            }
                        }
                        : {
                            duration: speeds[i].Duration, easing: speeds[i].Easing, complete: function () {
                                motoRacing.showFinalRanks(pkInfo);
                                motoRacing.clear();

                                motoRacing.showResult();

                                // when moto and the end, continue to move some instance
                                motoRacing.moveEndDistance($(this).attr('alt'), $(this).css('right'));

                                //resetMoto
                                var num = $(this).attr('alt');
                                motoRacing.resetMoto(num);

                                //clear
                                //motoRacing.clear();
                            }
                        };

                    var $moto = $('#moto' + speeds[i].Num);

                    $moto.animate({ right: speeds[i].MoveLength }, param);    //speeds[i].MoveLength + pos 
                }

                // show ranks
                $('body').everyTime('1s', 'showRanks', function () {
                    motoRacing.showRanks();
                });
            });
        },
        getMotoInitPositions: function () {
            var positons = [];
            for (var num = 1; num <= 10; num++) {
                var $moto = $('#moto' + num);
                var pos = parseInt($moto.css('right'), 10);
                positons.push(pos);
            }
            return positons;
        },
        runMoto2: function (pkInfo) {
            var secondsToRun = (pkInfo.GamingSeconds < 0 ? Math.abs(pkInfo.GamingSeconds) : 0) + 's';

            $('body').oneTime(secondsToRun, function () {
                // moto init durations
                motoRacing.Durations = motoRacing.getDurations();
                // moto init position
                motoRacing.MotoInitPosition = motoRacing.getMotoInitPositions();
                // moto running 
                motoRacing.appendRunMotos();

                //ranks: 3,2,5,6,8,7,10,1,9,4
                var rankArr = pkInfo.PK.Ranks.split(',');
                for (var i = 0; i < rankArr.length; i++) {
                    var motoNum = rankArr[i];
                    var distances = motoRacing.getDistances2(i + 1, motoNum);
                    var durations = motoRacing.getDurations2(pkInfo.GameRemainSeconds, i + 1);

                    console.log(distances);
                    //console.log(durations);
                    motoRacing.runMotoSingle(motoNum, distances, durations)
                }

                // show ranks
                $('body').everyTime('1s', 'showRanks', function () {
                    motoRacing.showRanks();
                });
            });
        },
        getDistances2: function (rank, motoNum) {
            var unit = 10;
            var random1 = motoRacing.getRandomNum(0, 15);
            var step1 = unit + random1 * 12;
            var random2 = motoRacing.getRandomNum(1, 20);
            var step2 = random2 * 15;
            var random3 = motoRacing.getRandomNum(1, 30);
            var step3 = random3 * 13;
            var random4 = motoRacing.getRandomNum(0, 55);
            var step4 = rank <= 3 ? 620 - 120 * rank : 3 * random4;
            var distances = [step1, step2, step3, step4];

            return distances;
        },
        getDurations2: function (remainSeconds, rank) {
            if (remainSeconds < 10) { remainSeconds = 10; }
            var step4Seconds = 3;
            var unit = remainSeconds <= 10 ? (remainSeconds - step4Seconds) / 3 : step4Seconds;
            var step4 = remainSeconds <= 10 ? unit : step4Seconds;

            var step1 = (unit + 0.1 * (10 - rank));
            var step2 = (unit + 0.2 * rank);
            var step3 = remainSeconds - step1 - step2 - step4; //(unit - 0.2 * rank) * 1000;
            //var durations = [step1, step2, step3, step4];
            var durations = [step1.toFixed(3) * 1000, step2.toFixed(3) * 1000, step3.toFixed(3) * 1000, step4.toFixed(3) * 1000];
            return durations;
        },
        runMotoSingle: function (motoNum, distances, durations) {
            //step1
            $('#moto' + motoNum).animate({ right: distances[0] }, {
                easing: 'easeOutBack2',
                duration: durations[0],
                complete: function () {
                    //step2
                    var motoNum = $(this).attr('alt');
                    $('#moto' + motoNum).animate({ right: distances[1] }, {
                        easing: 'easeInBack2',
                        duration: durations[1],
                        complete: function () {
                            //step3
                            var motoNum = $(this).attr('alt');
                            $('#moto' + motoNum).animate({ right: distances[2] }, {
                                easing: 'easeInOutBack2',
                                duration: durations[2],
                                complete: function () {
                                    //step4
                                    var motoNum = $(this).attr('alt');
                                    $('#moto' + motoNum).animate({ right: distances[3] }, {
                                        easing: 'easeOutQuint',
                                        duration: durations[3],
                                        complete: function () {
                                            //var motoNum = $(this).attr('alt');
                                            //console.log("motoNum: " + motoNum);

                                            if (motoRacing.EndMotos.length == 0) {
                                                // when moto and the end, continue to move some instance
                                                motoRacing.moveEndDistance2();

                                                // when the first moto at the end, moveEndFlag
                                                motoRacing.moveEndFlag(motoRacing.PKInfo);
                                            }

                                            motoRacing.EndMotos.push({
                                                Rank: motoRacing.EndMotos.length + 1,
                                                Num: parseInt($(this).attr('alt'))
                                            });
                                        }
                                    });
                                }
                            });
                        }
                    });
                }
            });
        },
        moveEndDistance: function (num, right) {
            // when moto and the end, continue to move some instance
            var $moto = $('#moto' + num);
            right = parseInt(right, 10) + 1500;
            $moto.animate({ right: right + 'px' }, 2500);
        },
        moveEndDistance2: function () {
            // when moto and the end, continue to move some instance

            //ranks: 3,2,5,6,8,7,10,1,9,4
            var rankArr = motoRacing.PKInfo.PK.Ranks.split(',');

            for (var i = 0; i < rankArr.length; i++) {
                var $moto = $('#moto' + rankArr[i]);
                var positon = 1800 - parseInt($moto.css('right'), 10);

                $moto.floating({ direction: 'left', millisec: 6, position: positon });
            }
        },
        moveEndFlag: function (pkInfo) {
            var $endFlag = $('.end-flag');

            var positon = 220;
            $endFlag.floating({ direction: 'right', millisec: motoRacing.Millisec, position: positon });

            $('body').everyTime('1ds', function () {
                if ($endFlag.css('left') == positon + 'px') {
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

                var duration = (pkInfo.GameRemainSeconds > 2)
                    ? (pkInfo.GameRemainSeconds - 2 + rank * 0.2) * 1000
                    : (pkInfo.GameRemainSeconds / 10) * rank * 1000;

                //var length = 1200 - 50 * rank;
                var length = motoRacing.RoadLength;
                speeds.push({
                    'Num': motoNum,
                    'Rank': rank,
                    'Duration': duration,
                    'Easing': motoRacing.Easings[random],
                    'MoveLength': length
                });
            }
            console.log(speeds);
            return speeds;
        },
        getDurations: function () {
            var durations = [];

            var pkInfo = motoRacing.PKInfo;
            var seconds = 3;
            var timeStep = seconds / 10;

            //ranks: 3,2,5,6,8,7,10,1,9,4
            var rankArr = pkInfo.PK.Ranks.split(',');
            for (var i = 0; i < rankArr.length; i++) {
                var rank = i + 1;
                var duration = (pkInfo.GameRemainSeconds > seconds)
                    ? ((pkInfo.PK.GameSeconds - seconds) + (rank - 1) * timeStep)
                    : timeStep * rank;

                durations.push(duration);
            }
            return durations;
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

            console.log(ranks);

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
            $('.start-flag').floating('destroy');

            $('body').stopTime();
        },
        showResult: function () {
            var ranks = motoRacing.PKInfo.PK.Ranks;

            var html = motoRacing.getResultHtml();

            //$(".ui-dialog-titlebar").addClass('hide');
            $racingResult.html(html);
            $racingResult.removeClass("hide");
            $('.btn-close-result').click(function () {
                $racingResult.addClass("hide");
            });

            // close result dialog after 5 seconds, then open bonus dialog
            $('body').oneTime(motoRacing.ResultShowSeconds + 's', function () {
                // close
                //$racingResult.dialog("close");
                $racingResult.addClass("hide");
                // open
                motoRacing.showBonus();
                // countdown
                motoRacing.bonusCountdown();
            });
        },
        getResultHtml: function () {
            var ranks = motoRacing.PKInfo.PK.Ranks;

            var title = '<a href="#" class="close btn-close-result"><img src="/img/del.png"></a>'
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
                        var html = motoRacing.getBonusHtml(res.Data);
                        //console.log(html);
                        $bonusResult.html(html);
                        $bonusResult.removeClass("hide");

                        $('.close-bonus').click(function () {
                            $bonusResult.addClass("hide");
                        });
                    }
                }
            });

            // close bonus dialog after 30 seconds
            $('body').oneTime(motoRacing.BonusShowSeconds + 's', function () {
                // close
                $bonusResult.addClass("hide");
                // resetElements
                motoRacing.resetElements();
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
            var html = '<a href="#" class="close close-bonus"><img src="/img/del.png"></a>'
                     + '<a href="#" class="queding close-bonus"><img src="/img/btn-queding.png"></a>'
                     + '<div class="bonus-countdown">' + motoRacing.BonusShowSeconds + '</div>'
                     + '<div class="huodejiangjin">获得奖金：' + totalBonus + '</div>'
                     + '<div class="jieguo">'
                     + '    <div class="text">结果</div>'
                     + '    <div class="shishi z-shishi">' + ranksHtml + '</div>'
                     + '</div>'
                     + '<div class="jieguo-2">'
                     + '    <ul class="bonus">' + bonusResultHtml + '</ul>'
                     + '</div>';

            return html;
        },
        bonusCountdown: function () {
            $('body').everyTime('1s', 'bonusCountdown', function () {
                var $cuntdown = $('.bonus-countdown');
                var num = $('.bonus-countdown').text();
                num = num == '' ? motoRacing.BonusShowSeconds : num - 1;
                $cuntdown.text(num);
            }, motoRacing.BonusShowSeconds);
        },
    };
});