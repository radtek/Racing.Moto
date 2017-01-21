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
        ChineseNums: ['一', '二', '三', '四', '五', '六', '七', '八', '九', '十'],
        Tabs: [
            { ID: 1, Name: '猜名次(前五名)', ShortName: '【前五名】' },
            { ID: 2, Name: '猜名次(后五名)', ShortName: '【后五名】' },
            { ID: 3, Name: '猜大小单双', ShortName: '【大小单双】' }
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
                    $scope.bet.initBets($scope.bet.Bets);
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
        initBets: function (bets) {
            angular.forEach(bets, function (item, index, arr) {
                item.Tab = $scope.bet.getTabShortName(item.Rank);
                item.ChineseRank = $scope.bet.getChineseRank(item.Rank);
                item.Bonus = $app.round(item.Amount.mul(item.Rate), 2);
            });
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
            return '第' + $scope.bet.ChineseNums[rank - 1] + '名';
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
            angular.forEach($scope.bet.PKRates, function (item, index, arr) {
                $scope.bet.addBet(bets, item.Rank, 1, item.Amount1, item.Number1, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 2, item.Amount2, item.Number2, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 3, item.Amount3, item.Number3, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 4, item.Amount4, item.Number4, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 5, item.Amount5, item.Number5, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 6, item.Amount6, item.Number6, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 7, item.Amount7, item.Number7, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 8, item.Amount8, item.Number8, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 9, item.Amount9, item.Number9, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 10, item.Amount10, item.Number10, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 11, item.BigAmount, item.Big, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 12, item.SmallAmount, item.Small, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 13, item.OddAmount, item.Odd, item.Rate);
                $scope.bet.addBet(bets, item.Rank, 14, item.EvenAmount, item.Even, item.Rate);
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

                $scope.bet.Bets.push.apply($scope.bet.Bets, bets);
                alert('投注成功!');
            });
        },
        addBet: function (bets, rank, num, amount, rate) {
            if ($app.isNum(amount)) {
                var newBet = $scope.bet.newBet(rank, num, parseFloat(amount), rate);
                bets.push(newBet);
            }
        },
        newBet: function (rank, num, amount, rate) {
            return {
                Rank: rank,
                Num: num,
                Amount: amount,

                Tab: $scope.bet.getTabShortName(rank),
                ChineseRank: $scope.bet.getChineseRank(rank),
                Rate: rate,
                Bonus: $app.round(amount.mul(rate), 2)
            };
        },
    };
}]);