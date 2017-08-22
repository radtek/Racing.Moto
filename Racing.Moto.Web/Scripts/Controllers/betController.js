app.controller('betController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.UserId = null;

    $scope.init = function (userId) {
        $scope.UserId = userId
        $scope.bet.init();
        $scope.prevPK.getPrevPK();
    };

    $scope.bet = {
        Disabled: false,
        PKModel: null,
        PKRates: [],
        Rebates: [],//退水
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
            console.log($app.formatDate(new Date(), 'yyyy/MM/dd HH:mm:ss'));

            $http.post('/Moto/GetCurrentPKInfo').then(function (res) {

                console.log($app.formatDate(new Date(), 'yyyy/MM/dd HH:mm:ss'));
                //console.log(res);
                if (res.data.Success) {
                    $scope.bet.PKModel = res.data.Data.PKModel;
                    $scope.bet.PKRates = res.data.Data.PKRates;
                    $scope.bet.Rebates = res.data.Data.Rebates;
                    $scope.bet.Bets = res.data.Data.Bets;

                    // 设置下注信息
                    $scope.bet.initPKRates($scope.bet.PKRates);

                    $scope.bet.initBetItems($scope.bet.Bets);
                    //$scope.bet.setBets($scope.bet.PKRates, $scope.bet.Bets);
                    // 重置输入框背景色
                    //$scope.bet.resetBgColor();

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
            $scope.bet.NotSavedAmount = 0;//未投注
            $scope.bet.SavedBetItems = [];//已投注
            $scope.bet.SavedAmount = 0;//已投注

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
            if ($scope.bet.PKModel == null || pkModel == null || pkModel.PK.PKId == $scope.bet.PKModel.PK.PKId) {
                return;
            }
            // 重新初始化
            $scope.bet.init();

            // 设置可以下注
            $scope.bet.setDisabled(false);
        },
        setDisabled: function (disabled) {
            $scope.bet.Disabled = disabled;
            console.log($scope.bet.Disabled);
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
                    $scope.bet.setBgColor(pkRate, num, 'bg-color-white');
                } else {
                    // edit
                    betItem.Amount = amount != null ? parseFloat(amount) : null;
                    $scope.bet.setBetItem(betItem, pkRate.Rate);
                    // 重置输入框背景色
                    $scope.bet.setBgColor(pkRate, num, 'bg-color-red');
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
                $scope.bet.setBgColor(pkRate, num, 'bg-color-white');
            }
        },
        //重置输入框背景色
        resetBgColor: function () {
            for (var i = 0; i < $scope.bet.PKRates.length; i++) {
                var rank = i + 1;
                for (var j = 1; j <= 14; j++) {
                    var pkRate = $scope.bet.PKRates[i];
                    var amount = $scope.bet.getPKRateAmount(pkRate, j);
                    if (amount != null && amount > 0) {
                        if ($scope.bet.existInNotSavedBetItems(rank, j)) {
                            $scope.bet.setBgColor(pkRate, j, 'bg-color-pink');
                        } else if ($scope.bet.existInSavedBetItems(rank, j)) {
                            $scope.bet.setBgColor(pkRate, j, 'bg-color-red');
                        } else {
                            $scope.bet.setBgColor(pkRate, j, 'bg-color-white');
                        }
                    }
                }
            }
        },
        //已下注重置, 3秒后 清空背景色, 数值
        retsetSavedBetItems: function () {
            $timeout(function () {
                for (var i = 0; i < $scope.bet.PKRates.length; i++) {
                    var rank = i + 1;
                    for (var j = 1; j <= 14; j++) {
                        var pkRate = $scope.bet.PKRates[i];
                        if ($scope.bet.getBgColor(pkRate, j) == 'bg-color-red') {
                            $scope.bet.setPKRateAmount(pkRate, j, null);
                            // 重置输入框背景色
                            $scope.bet.setBgColor(pkRate, j, 'bg-color-white');
                        }
                    }
                }
            }, 1000);
        },
        getBgColor: function (pkRate, num) {
            var color = null;
            switch (num) {
                case 1: color = pkRate.BgColor1; break;
                case 2: color = pkRate.BgColor2; break;
                case 3: color = pkRate.BgColor3; break;
                case 4: color = pkRate.BgColor4; break;
                case 5: color = pkRate.BgColor5; break;
                case 6: color = pkRate.BgColor6; break;
                case 7: color = pkRate.BgColor7; break;
                case 8: color = pkRate.BgColor8; break;
                case 9: color = pkRate.BgColor9; break;
                case 10: color = pkRate.BgColor10; break;
                case 11: color = pkRate.BgColor11; break;
                case 12: color = pkRate.BgColor12; break;
                case 13: color = pkRate.BgColor13; break;
                case 14: color = pkRate.BgColor14; break;
            }
            return color;
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
        getPKRateAmount: function (pkRate, num) {
            var amount = null;
            switch (num) {
                case 1: amount = pkRate.Amount1; break;
                case 2: amount = pkRate.Amount2; break;
                case 3: amount = pkRate.Amount3; break;
                case 4: amount = pkRate.Amount4; break;
                case 5: amount = pkRate.Amount5; break;
                case 6: amount = pkRate.Amount6; break;
                case 7: amount = pkRate.Amount7; break;
                case 8: amount = pkRate.Amount8; break;
                case 9: amount = pkRate.Amount9; break;
                case 10: amount = pkRate.Amount10; break;
                case 11: amount = pkRate.Amount11; break;
                case 12: amount = pkRate.Amount12; break;
                case 13: amount = pkRate.Amount13; break;
                case 14: amount = pkRate.Amount14; break;
            }
            return amount;
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

            // 单注限额
            var maxBetAmountMsg = $scope.rebate.checkMaxBetAmount(bets);
            if (maxBetAmountMsg.length > 0) {
                alert(maxBetAmountMsg.join('\n'));
                return;
            }

            // 单期限额
            $scope.bet.getSumAmountsByPkId().then(function (res) {
                if (!res.data.Success) {
                    alert(res.data.Message);
                    return;
                }

                var sumAmounts = res.data.Data;
                var maxPKAmountMsg = $scope.rebate.checkMaxPKAmount(bets, sumAmounts);
                if (maxPKAmountMsg.length > 0) {

                    alert(maxPKAmountMsg.join('\n'));
                    return;
                }

                // save
                var data = {
                    pkId: $scope.bet.PKModel.PK.PKId,
                    bets: bets
                };
                $http.post('/Moto/SaveBets', data).then(function (res) {
                    if (!res.data.Success) {
                        if (res.data.Code != '' && res.data.Data != null) {
                            $('#balance').text(res.data.Data);
                        }
                        alert(res.data.Message);
                        return;
                    } else {
                        $scope.bet.updateBalance(res.data.Data);// 更新余额
                        $scope.bet.setSavedBets();// 设置保存投注

                        alert('投注成功!');
                    }
                });
            });
        },
        // 设置保存投注
        setSavedBets: function () {

            // 已投注
            var amount = 0;
            angular.forEach($scope.bet.NotSavedBetItems, function (item, index, arr) {
                $scope.bet.addSavedItem($scope.bet.SavedBetItems, item.Rank, item.Num, item.Amount, item.Rate);
                amount = amount.add(item.Amount);
            });
            $scope.bet.SavedAmount += amount; //已投注.投注金额

            // 未投注
            $scope.bet.NotSavedBetItems = [];
            $scope.bet.NotSavedAmount = 0;  //未投注.投注金额

            // 重置输入框背景色
            $scope.bet.resetBgColor();
            //已下注重置, 1秒后 清空背景色, 数值
            $scope.bet.retsetSavedBetItems();
            //$scope.bet.init();
        },
        getSumAmountsByPkId: function () {
            var deferred = $q.defer();

            var data = {
                userId: $scope.UserId,
                pkId: $scope.bet.PKModel.PK.PKId
            };

            $http.post('/Moto/GetSumAmountsByPkId', data).then(function (res) {
                deferred.resolve(res);
            });

            return deferred.promise;
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

        addSavedItem: function (items, rank, num, amount, rate) {
            var newItem = $scope.bet.newSavedItem(rank, num, parseFloat(amount), rate);
            items.push(newItem);
        },
        newSavedItem: function (rank, num, amount, rate) {
            return {
                Rank: rank,
                Num: num,
                TabName: $scope.bet.getTabName(num, rank),
                ChineseRank: $scope.bet.getChineseRank(rank),
                NumName: $scope.bet.getNumName(num),
                Rate: rate,
                Amount: amount,
                Bonus: amount != null ? $app.round(amount.mul(rate), 2) : 0
            };
        },
    };

    $scope.quickBet = {
        CurrentPKRate: null,
        CurrentPKRateNum: null,
        showPopover: function (pkRate, num) {
            $scope.quickBet.resetPopoverIsOpen(pkRate, num, true);
            $scope.quickBet.CurrentPKRate = pkRate;
            $scope.quickBet.CurrentPKRateNum = num;
        },
        hideAllPopover: function () {
            for (var i = 0; i < $scope.bet.PKRates.length; i++) {
                for (var j = 1; j <= 14; j++) {
                    $scope.quickBet.setPopoverIsOpen($scope.bet.PKRates[i], j, false);
                }
            }
        },
        save: function (amount) {
            //console.log(amount);
            $scope.bet.setPKRateAmount($scope.quickBet.CurrentPKRate, $scope.quickBet.CurrentPKRateNum, amount);
            $scope.bet.betOnChange($scope.quickBet.CurrentPKRate, $scope.quickBet.CurrentPKRateNum, amount);
            $scope.quickBet.resetPopoverIsOpen($scope.quickBet.CurrentPKRate, $scope.quickBet.CurrentPKRateNum, false);
            // 重置输入框背景色
            if (amount == null || amount == '') {
                $scope.bet.setBgColor($scope.quickBet.CurrentPKRate, $scope.quickBet.CurrentPKRateNum, 'bg-color-white');
            } else {
                $scope.bet.resetBgColor();
            }

            //$scope.quickBet.CurrentPKRateNum = 0;
            //console.log($scope.quickBet.CurrentPKRate);
        },
        resetPopoverIsOpen: function (pkRate, num, isOpen) {
            // hide all first
            $scope.quickBet.hideAllPopover();
            // show
            $scope.quickBet.setPopoverIsOpen(pkRate, num, isOpen);
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
    };

    // 退水
    $scope.rebate = {
        checkMaxBetAmount: function (bets) {
            var msg = [];
            angular.forEach(bets, function (bet, index, arr) {
                var rebate = $scope.rebate.getRebate(bet.Num);
                if (bet.Amount > rebate.MaxBetAmount) {
                    msg.push('第' + bet.Rank + '名, 第' + bet.Num + '号, 不能大于单注限额: ' + rebate.MaxBetAmount + ' .');
                }
            });
            return msg;
        },
        checkMaxPKAmount: function (bets, sumAmounts) {
            var msg = [];
            angular.forEach(bets, function (bet, index, arr) {
                var rebate = $scope.rebate.getRebate(bet.Num);
                var sumAmount = $scope.rebate.getSumAmount(sumAmounts, bet.Rank, bet.Num);
                if (sumAmount != null) {
                    if (bet.Amount + sumAmount.Amount > rebate.MaxPKAmount) {
                        msg.push('第' + bet.Rank + '名, 第' + bet.Num + '号, 不能大于单期限额: ' + rebate.MaxPKAmount + ' .');
                    }
                }
            });
            return msg;
        },
        getRebate: function (num) {
            var rebate = null;

            angular.forEach($scope.bet.Rebates, function (item, index, arr) {
                if (item.RebateNo == num) {
                    rebate = item;
                }
            });

            return rebate;
        },
        getSumAmount: function (sumAmounts, rank, num) {
            var sumAmount = null;

            angular.forEach(sumAmounts, function (item, index, arr) {
                if (item.Rank == rank && item.Num == num) {
                    sumAmount = item;
                }
            });

            return sumAmount;
        },
    }

    $scope.countdown = {
        init: function (pkModel) {
            var eleHour = document.getElementById('hour');
            var eleMinute = document.getElementById('minute');
            var eleSecond = document.getElementById('second');

            //if (pkModel.GamingSeconds < 0) {
            //    var toGamingSeconds = Math.abs(pkModel.GamingSeconds);
            //    var obj = {
            //        hour: eleHour,
            //        mini: eleMinute,
            //        sec: eleSecond
            //    }
            //    fnTimeCountDown(toGamingSeconds, obj, __timeCountDownCallback);
            //}

            if (pkModel.OpeningRemainSeconds > 0) {
                //距离封盘
                var openingRemainSeconds = Math.abs(pkModel.OpeningRemainSeconds);
                var obj1 = {
                    hour: document.getElementById('hour1'),
                    mini: document.getElementById('minute1'),
                    sec: document.getElementById('second1')
                }
                fnTimeCountDown1(openingRemainSeconds, obj1, __timeCountDownCallback);
            }
            if (pkModel.ToLotterySeconds > 0) {
                //距离开奖
                var toLotterySeconds = Math.abs(pkModel.ToLotterySeconds);
                var obj2 = {
                    hour: document.getElementById('hour2'),
                    mini: document.getElementById('minute2'),
                    sec: document.getElementById('second2')
                }
                fnTimeCountDown2(toLotterySeconds, obj2);
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
    };

    $scope.prevPK = {
        getPrevPK: function () {
            $rootScope.Loading = false;

            $http.post('/api/pk/GetPrevPK', {}).then(function (res) {
                if (res.data.Success) {
                    prevMotoRacing.refresh(res.data.Data);
                }

            });
        },
    };
}]);

$(function () {
    var ticker = $.connection.pKTickerHub;

    function init() {
        ticker.server.getPKInfo().done(function (pkInfo) {
            console.log(pkInfo);
            motoRacing.init(pkInfo);
        });
    }

    // Add a client-side hub method that the server will call
    ticker.client.updatePKInfo = function (pkInfo) {
        //console.log(pkInfo);
        if (motoRacing.PKInfo != null) {
            motoRacing.refresh(pkInfo);
        } else {
            motoRacing.PKInfo = pkInfo;
        }
    }

    // Start the connection
    $.connection.hub.start().done(init);

    var motoRacing = {
        PKInfo: null,
        Scope: null,
        refresh: function (pkInfo) {
            if (pkInfo != null && (motoRacing.PKInfo == null || pkInfo.PK.PKId != motoRacing.PKInfo.PK.PKId)) {
                console.log('refresh');
                //motoRacing.init(pkInfo);

                location.href = location.href.replace('#', '');
            }
        },
        init: function (pkInfo) {
            if (pkInfo != null) {
                motoRacing.PKInfo = pkInfo;

                //if (motoRacing.Scope == null) {

                //    var appElement = document.querySelector('[ng-controller=betController]');
                //    motoRacing.Scope = angular.element(appElement).scope();
                //}

                //try {

                //    motoRacing.Scope.bet.refresh(pkInfo)
                //    motoRacing.Scope.$apply();
                //} catch (e) {
                //    console.log(e);
                //}
            }
        }
    };
})

// timeCountDownCallback
var __timeCountDownCallback = function () {
    try {
        var appElement = document.querySelector('[ng-controller=betController]');
        var scope = angular.element(appElement).scope();
        scope.bet.setDisabled(true);
        scope.$apply();
    } catch (e) {
        console.log(e);
    }
};