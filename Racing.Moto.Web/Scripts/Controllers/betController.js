app.controller('betController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {

    $scope.init = function () {
        $scope.bet.init();
    };

    $scope.bet = {
        Disabled: false,
        PKModel: null,
        PKRates: [],
        Bets: [],//投注

        TopFive: [],
        LastFive: [],
        BSOE: [],

        NotSavedBetItems: [],
        SavedBetItems: [],
        NotSavedAmount: 0,
        SavedAmount: 0,

        ChineseNums: ['一', '二', '三', '四', '五', '六', '七', '八', '九', '十'],
        Tabs: [
            { ID: 1, Name: '猜名次(前五名)', ShortName: '前五名' },
            { ID: 2, Name: '猜名次(后五名)', ShortName: '后五名' },
            { ID: 3, Name: '猜大小单双', ShortName: '大小单双' }
        ],
        CurrentTab: 1,
        init: function () {
            $scope.bet.CurrentTab = $scope.bet.Tabs[0].ID;

            $http.post('/Moto/GetCurrentPKInfo').then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.bet.PKModel = res.data.Data.PKModel;
                    $scope.bet.PKRates = res.data.Data.PKRates;
                    $scope.bet.Bets = res.data.Data.Bets;

                    // 设置下注信息
                    $scope.bet.initPKRates($scope.bet.PKRates);
                    $scope.bet.initBetItems($scope.bet.Bets);
                    $scope.bet.setBets($scope.bet.PKRates, $scope.bet.Bets);
                    // 重置输入框背景色
                    $scope.bet.resetBgColor();
                    // 倒计时
                    $scope.countdown.init($scope.bet.PKModel);
                }
            });
        },
        initPKRates: function (pkRates) {
            $scope.bet.TopFive = [];
            $scope.bet.BSOE = [];
            $scope.bet.LastFive = [];

            angular.forEach(pkRates, function (item, index, arr) {
                if (item.Rank <= 5) {
                    $scope.bet.TopFive.push(item);
                } else if (item.Rank > 5 && item.Rank <= 10) {
                    $scope.bet.LastFive.push(item);
                }
                $scope.bet.BSOE.push(item);
            });
        },
        initBetItems: function (bets) {
            $scope.bet.NotSavedBetItems = [];//未投注
            $scope.bet.SavedBetItems = [];//已投注

            angular.forEach(bets, function (bet, index, arr) {
                angular.forEach(bet.BetItems, function (betItem, index, arr) {
                    $scope.bet.setBetItem(betItem, bet.Rate);
                    //已投注
                    $scope.bet.SavedBetItems.push(betItem);
                });
            });
            //已投注.投注金额
            $scope.bet.SavedAmount = $scope.bet.sumBetAmount($scope.bet.SavedBetItems);
        },
        setBetItem: function (betItem, rate) {
            betItem.TabName = $scope.bet.getTabName(betItem.Num, betItem.Rank);
            betItem.NumName = $scope.bet.getNumName(betItem.Num),
            betItem.ChineseRank = $scope.bet.getChineseRank(betItem.Rank);
            betItem.Bonus = betItem.Amount != null ? $app.round(betItem.Amount.mul(rate), 2) : 0;
            betItem.Rate = rate;
        },
        getTab: function (id) {
            var tab = null;
            for (var i = 0; i < $scope.bet.Tabs.length; i++) {
                if ($scope.bet.Tabs[i].ID == id) {
                    tab = $scope.bet.Tabs[i];
                    break;
                }
            }
            return tab;
        },
        getTabShortName: function (rank) {
            var id = 1;
            if (rank <= 5) { id = 1; }
            else if (rank > 10) { id = 3; }
            else { id = 2; }

            var tab = $scope.bet.getTab(id);

            return tab.ShortName;
        },
        getChineseRank: function (rank) {
            return $scope.bet.ChineseNums[rank - 1];
        },

        //refresh
        refresh: function (pkModel) {
            // 重新初始化
            $scope.bet.init();

            // 设置可以下注
            $scope.bet.setDisabled(false);
        },
        setDisabled: function (disabled) {
            $scope.bet.Disabled = disabled;
        },

        /* Popover for quick bet */
        CurrentPKRate: null,
        CurrentPKRateNum: null,
        showPopover: function (pkRate, num) {
            $scope.bet.resetPopoverIsOpen(pkRate, num, true);
            $scope.bet.CurrentPKRate = pkRate;
            $scope.bet.CurrentPKRateNum = num;
        },
        hideAllPopover: function () {
            for (var i = 0; i < $scope.bet.PKRates.length; i++) {
                for (var j = 1; j <= 14; j++) {
                    $scope.bet.setPopoverIsOpen($scope.bet.PKRates[i], j, false);
                }
            }
        },
        quickBet: function (amount) {
            //console.log(amount);
            $scope.bet.setPKRateAmount($scope.bet.CurrentPKRate, $scope.bet.CurrentPKRateNum, amount);
            $scope.bet.resetPopoverIsOpen($scope.bet.CurrentPKRate, $scope.bet.CurrentPKRateNum, false);
            $scope.bet.betOnChange($scope.bet.CurrentPKRate, $scope.bet.CurrentPKRateNum, amount);
            // 重置输入框背景色
            if (amount == null || amount == '') {
                $scope.bet.setBgColor($scope.bet.CurrentPKRate, $scope.bet.CurrentPKRateNum, 'bg-color-white');
            } else {
                $scope.bet.resetBgColor();
            }

            //$scope.bet.CurrentPKRateNum = 0;
            //console.log($scope.bet.CurrentPKRate);
        },
        setPKRateAmount: function (pkRate, num, amount) {
            switch (num) {
                case 1: pkRate.Amount1 = amount; break;
                case 2: pkRate.Amount2 = amount; break;
                case 3: pkRate.Amount3 = amount; break;
                case 4: pkRate.Amount4 = amount; break;
                case 5: pkRate.Amount5 = amount; break;
                case 6: pkRate.Amount6 = amount; break;
                case 7: pkRate.Amount7 = amount; break;
                case 8: pkRate.Amount8 = amount; break;
                case 9: pkRate.Amount9 = amount; break;
                case 10: pkRate.Amount10 = amount; break;
                case 11: pkRate.Amount11 = amount; break;
                case 12: pkRate.Amount12 = amount; break;
                case 13: pkRate.Amount13 = amount; break;
                case 14: pkRate.Amount14 = amount; break;
            }
        },
        resetPopoverIsOpen: function (pkRate, num, isOpen) {
            // hide all first
            $scope.bet.hideAllPopover();
            // show
            $scope.bet.setPopoverIsOpen(pkRate, num, isOpen);
        },
        setPopoverIsOpen: function (pkRate, num, isOpen) {
            switch (num) {
                case 1: pkRate.PopoverIsOpen1 = isOpen; break;
                case 2: pkRate.PopoverIsOpen2 = isOpen; break;
                case 3: pkRate.PopoverIsOpen3 = isOpen; break;
                case 4: pkRate.PopoverIsOpen4 = isOpen; break;
                case 5: pkRate.PopoverIsOpen5 = isOpen; break;
                case 6: pkRate.PopoverIsOpen6 = isOpen; break;
                case 7: pkRate.PopoverIsOpen7 = isOpen; break;
                case 8: pkRate.PopoverIsOpen8 = isOpen; break;
                case 9: pkRate.PopoverIsOpen9 = isOpen; break;
                case 10: pkRate.PopoverIsOpen10 = isOpen; break;
                case 11: pkRate.PopoverIsOpen11 = isOpen; break;
                case 12: pkRate.PopoverIsOpen12 = isOpen; break;
                case 13: pkRate.PopoverIsOpen13 = isOpen; break;
                case 14: pkRate.PopoverIsOpen14 = isOpen; break;
            }
        },

        /* 未投注 */
        betOnChange: function (pkRateModel, num, amount) {
            var pkRate = $scope.bet.getPKRateFromModel(pkRateModel, num);

            var betItem = $scope.bet.getNotSaveBetItem(pkRate.Rank, num);
            if (betItem == null && amount != null && amount > 0) {
                // add
                $scope.bet.addNotSavedBetItem(pkRate, num, amount);
                // 重置输入框背景色
                $scope.bet.resetBgColor(pkRate, num, 'bg-color-pink');
            } else {
                if (amount == null || amount == 0) {
                    // remove
                    $scope.bet.removeNotSaveBetItem(pkRate.Rank, num);
                    // 重置输入框背景色
                    $scope.bet.resetBgColor(pkRate, num, 'bg-color-white');
                } else {
                    // edit
                    betItem.Amount = amount != null ? parseFloat(amount) : null;
                    $scope.bet.setBetItem(betItem, pkRate.Rate);
                    // 重置输入框背景色
                    $scope.bet.resetBgColor(pkRate, num, 'bg-color-red');
                }
            }
            //未投注.投注金额
            $scope.bet.NotSavedAmount = $scope.bet.sumBetAmount($scope.bet.NotSavedBetItems);

            // 重置输入框背景色
            $scope.bet.resetBgColor();
        },
        getPKRateFromModel: function (model, num) {
            var rate;

            switch (num) {
                case 1: rate = model.Rate1; break;
                case 2: rate = model.Rate2; break;
                case 3: rate = model.Rate3; break;
                case 4: rate = model.Rate4; break;
                case 5: rate = model.Rate5; break;
                case 6: rate = model.Rate6; break;
                case 7: rate = model.Rate7; break;
                case 8: rate = model.Rate8; break;
                case 9: rate = model.Rate9; break;
                case 10: rate = model.Rate10; break;
                case 11: rate = model.Rate11; break;
                case 12: rate = model.Rate12; break;
                case 13: rate = model.Rate13; break;
                case 14: rate = model.Rate14; break;
            }

            return {
                Rank: model.Rank,
                Num: num,
                Rate: rate
            };
        },
        addNotSavedBetItem: function (pkRate, num, amount) {
            var betItem = {
                Rank: pkRate.Rank,
                Num: num,
                Amount: parseFloat(amount)
            };
            $scope.bet.setBetItem(betItem, pkRate.Rate);
            $scope.bet.NotSavedBetItems.insert(0, betItem);
        },
        getTabName: function (num, rank) {
            if (num <= 10) {
                return rank <= 5 ? '前五名' : '后五名';
            } else {
                return '大小单双';
            }
        },
        getNumName: function (num) {
            if (num <= 10) {
                return num + '号';
            }

            var name = '';
            switch (num) {
                case 11: name = '大'; break;
                case 12: name = '小'; break;
                case 13: name = '单'; break;
                case 14: name = '双'; break;
            }
            return name;
        },
        getNotSaveBetItem: function (rank, num) {
            var item = null;
            for (var i = 0; i < $scope.bet.NotSavedBetItems.length; i++) {
                if ($scope.bet.NotSavedBetItems[i].Rank == rank && $scope.bet.NotSavedBetItems[i].Num == num) {
                    item = $scope.bet.NotSavedBetItems[i];
                    break;
                }
            }
            return item;
        },
        removeNotSaveBetItem: function (rank, num) {
            var index = null;
            for (var i = 0; i < $scope.bet.NotSavedBetItems.length; i++) {
                if ($scope.bet.NotSavedBetItems[i].Rank == rank && $scope.bet.NotSavedBetItems[i].Num == num) {
                    index = i;
                    break;
                }
            }
            if (index != null) {
                $scope.bet.NotSavedBetItems.remove(index);
            }
            // 重置输入框背景色
            $scope.bet.resetBgColor();
        },
        removeNotSaveBetItem2: function (index) {
            //删除输入/选择的金额
            var betItem = $scope.bet.NotSavedBetItems[index];
            $scope.bet.removeEnteredAmount(betItem.Rank, betItem.Num);
            //删除未下注
            $scope.bet.NotSavedBetItems.remove(index);
            // 重置输入框背景色
            $scope.bet.resetBgColor();
        },
        removeEnteredAmount: function (rank, num) {
            //删除输入/选择的金额
            var pkRate = null;
            for (var i = 0; i < $scope.bet.PKRates.length; i++) {
                if ($scope.bet.PKRates[i].Rank == rank) {
                    pkRate = $scope.bet.PKRates[i];
                    break;
                }
            }
            if (pkRate != null) {
                $scope.bet.setPKRateAmount(pkRate, num, null);

                // 重置输入框背景色
                $scope.bet.resetBgColor(pkRate, num, 'bg-color-white');
            }
        },
        //重置输入框背景色
        resetBgColor: function () {
            for (var i = 0; i < $scope.bet.PKRates.length; i++) {
                var rank = i + 1;
                for (var j = 1; j <= 14; j++) {
                    if ($scope.bet.existInNotSavedBetItems(rank, j)) {
                        $scope.bet.setBgColor($scope.bet.PKRates[i], j, 'bg-color-pink');
                    } else if ($scope.bet.existInSavedBetItems(rank, j)) {
                        $scope.bet.setBgColor($scope.bet.PKRates[i], j, 'bg-color-red');
                    } else {
                        $scope.bet.setBgColor($scope.bet.PKRates[i], j, 'bg-color-white');
                    }
                }
            }
        },
        setBgColor: function (pkRate, num, color) {
            switch (num) {
                case 1: pkRate.BgColor1 = color; break;
                case 2: pkRate.BgColor2 = color; break;
                case 3: pkRate.BgColor3 = color; break;
                case 4: pkRate.BgColor4 = color; break;
                case 5: pkRate.BgColor5 = color; break;
                case 6: pkRate.BgColor6 = color; break;
                case 7: pkRate.BgColor7 = color; break;
                case 8: pkRate.BgColor8 = color; break;
                case 9: pkRate.BgColor9 = color; break;
                case 10: pkRate.BgColor10 = color; break;
                case 11: pkRate.BgColor11 = color; break;
                case 12: pkRate.BgColor12 = color; break;
                case 13: pkRate.BgColor13 = color; break;
                case 14: pkRate.BgColor14 = color; break;
            }
        },
        existInSavedBetItems: function (rank, num) {
            var exist = false;
            for (var i = 0; i < $scope.bet.SavedBetItems.length; i++) {
                if ($scope.bet.SavedBetItems[i].Rank == rank && $scope.bet.SavedBetItems[i].Num == num) {
                    exist = true;
                    break;
                }
            }
            return exist;
        },
        existInNotSavedBetItems: function (rank, num) {
            var exist = false;
            for (var i = 0; i < $scope.bet.NotSavedBetItems.length; i++) {
                if ($scope.bet.NotSavedBetItems[i].Rank == rank && $scope.bet.NotSavedBetItems[i].Num == num) {
                    exist = true;
                    break;
                }
            }
            return exist;
        },

        //计算投注金额
        sumBetAmount: function (betItems) {
            var amount = 0;
            angular.forEach(betItems, function (item, index, arr) {
                amount = amount.add(item.Amount);
            });
            return amount;
        },

        setBets: function (pkRates, bets) {
            angular.forEach(pkRates, function (item, index, arr) {
                var bet1 = $scope.bet.getBet(bets, item.Rank, 1);
                item.Amount1 = bet1 != null ? bet1.Amount : null;
                var bet2 = $scope.bet.getBet(bets, item.Rank, 2);
                item.Amount2 = bet2 != null ? bet2.Amount : null;
                var bet3 = $scope.bet.getBet(bets, item.Rank, 3);
                item.Amount3 = bet3 != null ? bet3.Amount : null;
                var bet4 = $scope.bet.getBet(bets, item.Rank, 4);
                item.Amount4 = bet4 != null ? bet4.Amount : null;
                var bet5 = $scope.bet.getBet(bets, item.Rank, 5);
                item.Amount5 = bet5 != null ? bet5.Amount : null;
                var bet6 = $scope.bet.getBet(bets, item.Rank, 6);
                item.Amount6 = bet6 != null ? bet6.Amount : null;
                var bet7 = $scope.bet.getBet(bets, item.Rank, 7);
                item.Amount7 = bet7 != null ? bet7.Amount : null;
                var bet8 = $scope.bet.getBet(bets, item.Rank, 8);
                item.Amount8 = bet8 != null ? bet8.Amount : null;
                var bet9 = $scope.bet.getBet(bets, item.Rank, 9);
                item.Amount9 = bet9 != null ? bet9.Amount : null;
                var bet10 = $scope.bet.getBet(bets, item.Rank, 10);
                item.Amount10 = bet10 != null ? bet10.Amount : null;

                var bet11 = $scope.bet.getBet(bets, item.Rank, 11);
                item.Amount11 = bet11 != null ? bet11.Amount : null;
                var bet12 = $scope.bet.getBet(bets, item.Rank, 12);
                item.Amount12 = bet12 != null ? bet12.Amount : null;
                var bet13 = $scope.bet.getBet(bets, item.Rank, 13);
                item.Amount13 = bet13 != null ? bet13.Amount : null;
                var bet14 = $scope.bet.getBet(bets, item.Rank, 14);
                item.Amount14 = bet14 != null ? bet14.Amount : null;
            });
        },
        getBet: function (bets, rank, num) {
            var bet = null;
            for (var i = 0; i < bets.length; i++) {
                if (bets[i].Rank == rank && bets[i].Num == num) {
                    bet = bets[i];
                }
            }
            return bet;
        },
        save: function () {
            //console.log($scope.bet.PKRates);
            if ($scope.bet.Disabled) {
                return;
            }
            var bets = [];
            angular.forEach($scope.bet.NotSavedBetItems, function (item, index, arr) {
                $scope.bet.addBet(bets, item.Rank, item.Num, item.Amount, item.Rate);
            });
            console.log(bets);

            if (bets.length == 0) {
                alert('请填写下注金额');
                return;
            }
            var data = {
                pkId: $scope.bet.PKModel.PK.PKId,
                bets: bets
            };
            $http.post('/Moto/SaveBets', data).then(function (res) {
                if (!res.data.Success) {
                    alert(res.data.Message);
                    return;
                }

                
                $scope.bet.NotSavedAmount = 0;  //未投注方案.投注金额
                $scope.bet.updateBalance(res.data.Data);// 更新余额
                $scope.bet.init();
                alert('投注成功!');
            });
        },
        updateBalance: function (balance) {
            $('#balance').text(balance);
        },
        addBet: function (bets, rank, num, amount, rate) {
            var newBet = $scope.bet.newBet(rank, num, parseFloat(amount), rate);
            bets.push(newBet);
        },
        newBet: function (rank, num, amount, rate) {
            return {
                Rank: rank,
                Num: num,
                Amount: amount,
                Rate: rate
            };
        },
    };

    $scope.countdown = {
        init: function (pkModel) {
            var eleHour = document.getElementById('hour');
            var eleMinute = document.getElementById('minute');
            var eleSecond = document.getElementById('second');

            if (pkModel.OpeningRemainSeconds <= 0) {
                eleHour.innerHTML = '00';
                eleMinute.innerHTML = '00';
                eleSecond.innerHTML = '00';
            } else {
                var closeBeginTime = $app.convertToDate(pkModel.CloseBeginTime);
                var year = closeBeginTime.getFullYear();
                var month = closeBeginTime.getMonth();
                var day = closeBeginTime.getDate();
                var hour = closeBeginTime.getHours();
                var minute = closeBeginTime.getMinutes();
                var second = closeBeginTime.getSeconds();


                var d = Date.UTC(year, month, day, hour, minute, second);
                var obj = {
                    sec: eleSecond,
                    mini: eleMinute,
                    hour: eleHour
                }
                fnTimeCountDown(d, obj);
            }
        },
    };
}]);

$(function () {
    var ticker = $.connection.pKTickerHub;

    function init() {
        ticker.server.getPKInfo().done(function (pkInfo) {
            console.log(pkInfo);
            motoRacing.refresh(pkInfo);
        });
    }

    // Add a client-side hub method that the server will call
    ticker.client.updatePKInfo = function (pkInfo) {
        console.log(pkInfo);
        motoRacing.refresh(pkInfo);
    }

    // Start the connection
    $.connection.hub.start().done(init);

    var motoRacing = {
        PKInfo: null,
        Scope: null,
        refresh: function (pkInfo) {
            if (pkInfo != null && (motoRacing.PKInfo == null || pkInfo.PK.PKId != motoRacing.PKInfo.PK.PKId)) {
                motoRacing.PKInfo = pkInfo;

                if (motoRacing.Scope == null) {

                    var appElement = document.querySelector('[ng-controller=betController]');
                    motoRacing.Scope = angular.element(appElement).scope();
                }

                motoRacing.Scope.bet.refresh(pkInfo)
            }
        },
    };
})

// timeCountDownCallback
var __timeCountDownCallback = function () {
    var appElement = document.querySelector('[ng-controller=betController]');
    var scope = angular.element(appElement).scope();
    scope.bet.setDisabled(true);
};