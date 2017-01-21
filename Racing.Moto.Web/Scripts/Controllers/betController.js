app.controller('betController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', function ($scope, $rootScope, $http, $compile, $timeout, $q) {

    $scope.init = function () {
        $scope.bet.init();
    };

    $scope.bet = {
        PK: null,
        PKRates: [],
        Bets: [],//投注

        TopFive: [],
        LastFive: [],
        BSOE: [],

        NotSavedBetItems: [],
        SavedBetItems: [],
        NotSavedAoumt: 0,
        SavedAoumt: 0,

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
                    $scope.bet.PK = res.data.Data.PK;
                    $scope.bet.PKRates = res.data.Data.PKRates;
                    $scope.bet.Bets = res.data.Data.Bets;

                    $scope.bet.initPKRates($scope.bet.PKRates);
                    $scope.bet.initBetItems($scope.bet.Bets);
                    $scope.bet.setBets($scope.bet.PKRates, $scope.bet.Bets);
                }
            });
        },
        initPKRates: function (pkRates) {
            $scope.bet.TopFive = [];
            $scope.bet.BSOE = [];
            $scope.bet.TopFive = [];

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
            $scope.bet.SavedAoumt = $scope.bet.sumBetAmount($scope.bet.SavedBetItems);
        },
        setBetItem: function (betItem, rate) {
            betItem.TabName = $scope.bet.getTabName(betItem.Num, betItem.Rank);
            betItem.NumName = $scope.bet.getNumName(betItem.Num),
            betItem.ChineseRank = $scope.bet.getChineseRank(betItem.Rank);
            betItem.Bonus = $app.round(betItem.Amount.mul(rate), 2);
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

        /* 未投注 */
        betOnChange: function (pkRateModel, num, amount) {
            var pkRate = $scope.bet.getPKRateFromModel(pkRateModel, num);

            var betItem = $scope.bet.getNotSaveBetItem(pkRate.Rank, num);
            if (betItem == null) {
                $scope.bet.addNotSavedBetItem(pkRate, num, amount);
            } else {
                betItem.Amount = parseFloat(amount);
                $scope.bet.setBetItem(betItem, pkRate.Rate);
            }
            //未投注.投注金额
            $scope.bet.NotSavedAoumt = $scope.bet.sumBetAmount($scope.bet.NotSavedBetItems);
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
                case 11: rate = model.RateBig; break;
                case 12: rate = model.RateSmall; break;
                case 13: rate = model.RateOdd; break;
                case 14: rate = model.RateEven; break;
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
                }
            }
            return item;
        },

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
                item.BigAmount = bet11 != null ? bet11.Amount : null;
                var bet12 = $scope.bet.getBet(bets, item.Rank, 12);
                item.SmallAmount = bet12 != null ? bet12.Amount : null;
                var bet13 = $scope.bet.getBet(bets, item.Rank, 13);
                item.OddAmount = bet13 != null ? bet13.Amount : null;
                var bet14 = $scope.bet.getBet(bets, item.Rank, 14);
                item.EvenAmount = bet14 != null ? bet14.Amount : null;
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
            var bets = [];
            //angular.forEach($scope.bet.PKRates, function (item, index, arr) {
            //    $scope.bet.addBet(bets, item.Rank, 1, item.Amount1, item.Rate1);
            //    $scope.bet.addBet(bets, item.Rank, 2, item.Amount2, item.Rate2);
            //    $scope.bet.addBet(bets, item.Rank, 3, item.Amount3, item.Rate3);
            //    $scope.bet.addBet(bets, item.Rank, 4, item.Amount4, item.Rate4);
            //    $scope.bet.addBet(bets, item.Rank, 5, item.Amount5, item.Rate5);
            //    $scope.bet.addBet(bets, item.Rank, 6, item.Amount6, item.Rate6);
            //    $scope.bet.addBet(bets, item.Rank, 7, item.Amount7, item.Rate7);
            //    $scope.bet.addBet(bets, item.Rank, 8, item.Amount8, item.Rate8);
            //    $scope.bet.addBet(bets, item.Rank, 9, item.Amount9, item.Rate9);
            //    $scope.bet.addBet(bets, item.Rank, 10, item.Amount10, item.Rate10);
            //    $scope.bet.addBet(bets, item.Rank, 11, item.AmountBig, item.RateBig);
            //    $scope.bet.addBet(bets, item.Rank, 12, item.AmountSmall, item.RateSmall);
            //    $scope.bet.addBet(bets, item.Rank, 13, item.AmountOdd, item.RateOdd);
            //    $scope.bet.addBet(bets, item.Rank, 14, item.AmountEven, item.RateEven);
            //});
            angular.forEach($scope.bet.NotSavedBetItems, function (item, index, arr) {
                $scope.bet.addBet(bets, item.Rank, item.Num, item.Amount, item.Rate);
            });
            console.log(bets);

            if (bets.length == 0) {
                alert('请填写下注金额');
                return;
            }
            var data = {
                pkId: $scope.bet.PK.PKId,
                bets: bets
            };
            $http.post('/Moto/SaveBets', data).then(function (res) {
                if (!res.data.Success) {
                    alert(res.data.Message);
                    return;
                }

                //$scope.bet.Bets.push.apply($scope.bet.Bets, bets);
                $scope.bet.init();
                alert('投注成功!');
            });
        },
        addBet: function (bets, rank, num, amount, rate) {
            //if ($app.isNum(amount)) {
                
            //}
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
}]);