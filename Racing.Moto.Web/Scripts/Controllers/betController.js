app.controller('betController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', function ($scope, $rootScope, $http, $compile, $timeout, $q) {

    $scope.init = function () {
        $scope.bet.init();
    };

    $scope.bet = {
        PKRates: [],
        TopFive: [],
        LastFive: [],
        BSOE: [],
        ChineseNums: ['一', '二', '三', '四', '五', '六', '七', '八', '九', '十'],
        Tabs: [
            { ID: 1, Name: '猜名次(前五名)' },
            { ID: 2, Name: '猜名次(后五名)' },
            { ID: 3, Name: '猜大小单双' }
        ],
        CurrentTab: 1,
        init: function () {
            $scope.bet.CurrentTab = $scope.bet.Tabs[0].ID;

            $http.get('/api/PKRate/GetCurrentPKRates').then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.bet.PKRates = res.data.Data;

                    $scope.bet.initPKRates($scope.bet.PKRates);
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
        save: function () {
            console.log($scope.bet.PKRates);
            var bets = [];
            angular.forEach($scope.bet.PKRates, function (item, index, arr) {
                //if(item.Amount1 != null && )
            });
        },
    };
}]);