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
            //var now = new Date();
            //var seconds = 10;
            //pkInfo.GameBeginTime = $app.formatDate(now.addSeconds(seconds), 'yyyy-MM-dd HH:mm:ss');
            //pkInfo.GamingSeconds = -seconds;
            //pkInfo.GamePassedSeconds = 0;
            //pkInfo.GameRemainSeconds = 30;
            //pkInfo.PK = { PKId: 1, Ranks: '3,2,5,6,8,7,10,1,9,4', GameSeconds: 30 };


            if (pkInfo == null) {
                return;
            }

            //// audio.wait
            //$('body').oneTime('2s', function () {
            //    //audio.run
            //    motoAudio.wait.play();
            //});

            motoRacing.run(pkInfo);
        });
    }

    // Add a client-side hub method that the server will call
    ticker.client.updatePKInfo = function (pkInfo) {
        // test
        //pkInfo = {};
        //pkInfo.GamingSeconds = 10;
        //pkInfo.GamePassedSeconds = 0;
        //pkInfo.GameRemainSeconds = 20;
        //pkInfo.GameBeginTime = '2017/04/03 18:30:00';
        //pkInfo.PK = { PKId: 1, Ranks: '3,2,5,6,8,7,10,1,9,4', GameSeconds: 20 };

        //console.log(pkInfo);
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
        MaxPKCount: 1,
        ResultShowSeconds: 30,
        BonusShowSeconds: 180,
        Millisec: 2,
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
                //motoRacing.countdownClock(pkInfo);
            }

            if (pkInfo == null || pkInfo.PK.Ranks == null) {
                return;
            }

            // new racing
            if (motoRacing.PKInfo == null || pkInfo.PK.PKId != motoRacing.PKInfo.PK.PKId) {
                motoRacing.PKInfo = pkInfo;
                motoRacing.EndMotos = [];

                //motoRacing.clear();

                // append moto
                motoRacing.append();
                // countdown
                motoRacing.countdown(pkInfo);
                //// move Road
                //motoRacing.moveRoad(pkInfo);
                //// run moto
                ////motoRacing.runMoto(pkInfo);
                //motoRacing.runMoto2(pkInfo);
            }
        },
        getcountdownClock: function () {
            var eleHour = document.getElementById('hour').innerHTML;
            var eleMinute = document.getElementById('minute').innerHTML;
            var eleSecond = document.getElementById('second').innerHTML;

            return eleHour + ':' + eleMinute + ':' + eleSecond;
        },
        countdownClock: function (pkInfo) {
            if (pkInfo.GamingSeconds <= 0) {
                var toGamingSeconds = Math.abs(pkInfo.GamingSeconds);
                // 倒计时结束时间
                var obj = {
                    sec: document.getElementById('second'),
                    mini: document.getElementById('minute'),
                    hour: document.getElementById('hour')
                }
                fnTimeCountDown(toGamingSeconds, obj);
            }
        },
        getUtc: function (time) {
            var year = time.getFullYear();
            var month = time.getMonth();
            var day = time.getDate();
            var hour = time.getHours();
            var minute = time.getMinutes();
            var second = time.getSeconds();

            return Date.UTC(year, month, day, hour, minute, second);
        },
        countdown: function (pkInfo) {
            var countdownSeconds = 5;
            var toGamingSeconds = Math.abs(pkInfo.GamingSeconds);
            //var seconds = countdownSeconds < toGamingSeconds ? toGamingSeconds - countdownSeconds : 0;
            if (pkInfo.GamingSeconds < 0) {
                if (countdownSeconds <= toGamingSeconds) {
                    $('.time-run2').show();
                    $('.time-run').hide();

                    $('body').everyTime('1s', function () {
                        var clock = motoRacing.getcountdownClock();
                        if (clock == '00:00:10') {
                            motoAudio.wait.play();
                        }                        

                        //console.log(clock);
                        if (clock == '00:00:05') {
                            $('.time-run2').hide();
                            $('.time-run').show();
                            var countdownSeconds = 5;
                            motoRacing.countdown2(countdownSeconds, countdownSeconds - 1);

                            motoRacing.startGame();
                        }

                        if (clock == '00:00:04') {
                            motoAudio.countdown.play();
                        }
                    });
                } else {
                    $('.time-run2').hide();
                    $('.time-run').show();

                    motoRacing.startGame();
                }
            } else {
                $('.time-run2').hide();
                $('.time-run').hide();

                motoRacing.startGame();
            }
        },
        startGame: function () {
            var pkInfo = motoRacing.PKInfo;
            pkInfo.GamingSeconds = -5;

            // move Road
            motoRacing.moveRoad(pkInfo);
            // run moto
            motoRacing.runMoto2(pkInfo);
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

                $('body').oneTime('5s', function () {
                    // road moving
                    //$('.start-flag').floating({ direction: 'right', millisec: motoRacing.Millisec });

                    $('.start-flag').floating('destroy');
                    $('.start-flag').hide();
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

                                // when moto at the end, continue to move some instance
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

                                // when moto at the end, continue to move some instance
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

                    //console.log(distances);
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
                                                // when moto at the end, continue to move some instance
                                                motoRacing.moveEndDistance2();

                                                // when the first moto at the end, moveEndFlag
                                                motoRacing.moveEndFlag(motoRacing.PKInfo);
                                            }

                                            motoRacing.EndMotos.push({
                                                Rank: motoRacing.EndMotos.length + 1,
                                                Num: parseInt($(this).attr('alt'))
                                            });

                                            // show Result
                                            if (motoRacing.EndMotos.length == 10) {
                                                //stop showRanks
                                                $('body').stopTime('showRanks');
                                            }
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
            // when moto at the end, continue to move some instance
            var $moto = $('#moto' + num);
            right = parseInt(right, 10) + 1500;
            $moto.animate({ right: right + 'px' }, 2500);
        },
        moveEndDistance2: function () {
            // when moto at the end, continue to move some instance

            //ranks: 3,2,5,6,8,7,10,1,9,4
            var rankArr = motoRacing.PKInfo.PK.Ranks.split(',');

            for (var i = 0; i < rankArr.length; i++) {
                var $moto = $('#moto' + rankArr[i]);
                var positon = 1800 - parseInt($moto.css('right'), 10);

                $moto.floating({ direction: 'left', millisec: 6, position: positon });
            }

            //motoAudio.end
            console.log('motoAudio.end');
            motoAudio.end.play();
        },
        moveEndFlag: function (pkInfo) {
            var $endFlag = $('.end-flag');

            var positon = 220;
            $endFlag.floating({ direction: 'right', millisec: motoRacing.Millisec, position: positon });

            $('body').everyTime('1ds', function () {
                if ($endFlag.css('left') == positon + 'px') {
                    motoRacing.clear();
                    motoRacing.showFinalRanks(motoRacing.PKInfo);

                    $('body').oneTime('3s', function () {
                        motoRacing.showResult();
                    });
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

            //console.log(ranks);

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
            $('.end-flag').floating('destroy');

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
                motoRacing.reload();
            });

            // close result dialog after 5 seconds, then open bonus dialog
            $('body').oneTime(motoRacing.ResultShowSeconds + 's', function () {
                // close
                //$racingResult.dialog("close");
                $racingResult.addClass("hide");
                motoRacing.reload();


                //// open
                //motoRacing.showBonus();
                //// countdown
                //motoRacing.bonusCountdown();
            });
        },
        getResultHtml: function () {
            var ranks = motoRacing.PKInfo.PK.Ranks;

            var title = '<a href="javascript:void(0)" class="close btn-close-result"><img src="/img/del.png"></a>'
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
            // removed
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
                            motoRacing.reload();
                        });
                    }
                }
            });

            // close bonus dialog after 5 seconds
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
            var html = '<a href="javascript:void(0)" class="close close-bonus"><img src="/img/del.png"></a>'
                     + '<a href="javascript:void(0)" class="queding close-bonus"><img src="/img/btn-queding.png"></a>'
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
            var bonusShowSeconds = motoRacing.BonusShowSeconds;

            $('body').everyTime('1s', 'bonusCountdown', function () {
                //var $cuntdown = $('.bonus-countdown');
                //var num = $('.bonus-countdown').text();
                //num = num == '' ? motoRacing.BonusShowSeconds : num - 1;

                var d = fnTimeCountDownResult.getResultStr(bonusShowSeconds);
                $('.bonus-countdown').text(d);

                bonusShowSeconds--;
            }, motoRacing.BonusShowSeconds);

            // reload page
            $('body').oneTime(motoRacing.BonusShowSeconds + 's', function () {
                motoRacing.MaxPKCount--;

                if (motoRacing.MaxPKCount == 0) {
                    //location.href = location.href;
                    motoRacing.reload();
                }
            });
        },
        reload: function () {
            location.href = location.href.replace('#','');
        },
    };


    // motoAudio.init
    motoAudio.init();

});

var motoAudio = {
    init: function () {
        var mediaNames = ["wait", "countdown", "start", "run", "end", "applause"];
        for (var i = 0; i < mediaNames.length; i++) {
            motoAudio.initPlayer(mediaNames[i]);
        }
    },
    initPlayer: function (mediaName) {
        $("#jplayer_" + mediaName).jPlayer("destroy");
        $("#jplayer_" + mediaName).jPlayer({
            ready: function () {
                $(this).jPlayer("setMedia", {
                    title: mediaName,
                    mp3: "../../Content/Audio/" + mediaName + ".mp3"
                });
            },
            play: function () {
                //var name = $(this).attr('id').split('_')[1];
                //if (name != 'countdown') {
                //    $(this).jPlayer("pauseOthers");// To avoid multiple jPlayers playing together.
                //}
            },
            ended: function () { // The $.jPlayer.event.ended event
                var name = $(this).attr('id').split('_')[1];
                if (name == 'wait' || name == 'run') {
                    $(this).jPlayer("play"); // Repeat the media
                } else if (name == 'countdown') {
                    motoAudio.start.play();
                } else if (name == 'start') {
                    motoAudio.run.play();
                } else if (name == 'end') {
                    motoAudio.applause.play();
                } else if (name == 'applause') {
                    //motoAudio.wait.play();
                }
            },
            swfPath: "../jPlayer",
            supplied: "mp3,wav",
            wmode: "window",
            globalVolume: true,
            useStateClassSkin: true,
            autoBlur: false,
            smoothPlayBar: true,
            keyEnabled: true
        });
    },
    wait: {
        play: function () {
            motoAudio.countdown.pause();
            motoAudio.start.pause();
            motoAudio.run.pause();
            motoAudio.end.pause();
            motoAudio.applause.pause();
            $("#jplayer_wait").jPlayer("play");
        },
        pause: function () {
            $("#jplayer_wait").jPlayer("pause");
        },
    },
    countdown: {
        play: function () {
            motoAudio.start.pause();
            motoAudio.run.pause();
            motoAudio.end.pause();
            motoAudio.applause.pause();
            $("#jplayer_countdown").jPlayer("play");
        },
        pause: function () {
            $("#jplayer_countdown").jPlayer("pause");
        },
    },
    start: {
        play: function () {
            motoAudio.wait.pause();
            motoAudio.countdown.pause();
            motoAudio.run.pause();
            motoAudio.end.pause();
            motoAudio.applause.pause();
            $("#jplayer_start").jPlayer("play");
        },
        pause: function () {
            $("#jplayer_start").jPlayer("pause");
        },
    },
    run: {
        play: function () {
            $("#jplayer_run").jPlayer("play");
            motoAudio.wait.pause();
            motoAudio.countdown.pause();
            motoAudio.start.pause();
            motoAudio.end.pause();
            motoAudio.applause.pause();
        },
        pause: function () {
            $("#jplayer_run").jPlayer("pause");
        },
    },
    end: {
        play: function () {
            motoAudio.wait.pause();
            motoAudio.countdown.pause();
            motoAudio.start.pause();
            motoAudio.run.pause();
            motoAudio.applause.pause();
            $("#jplayer_end").jPlayer("play");
        },
        pause: function () {
            $("#jplayer_end").jPlayer("pause");
        },
    },
    applause: {
        play: function () {
            motoAudio.wait.pause();
            motoAudio.countdown.pause();
            motoAudio.start.pause();
            motoAudio.run.pause();
            motoAudio.end.pause();
            $("#jplayer_applause").jPlayer("play");
        },
        pause: function () {
            $("#jplayer_applause").jPlayer("pause");
        },
    },
};