app.controller('betController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', function ($scope, $rootScope, $http, $compile, $timeout, $q) {

    $scope.init = function () {
        $scope.bet.init();
    };

    $scope.bet = {
        PKRates: [],
        TopFive: [],
        LastFive: [],
        BSOE: [],
        Charaters: ['一', '二', '三', '四', '五', '六', '七', '八', '九', '十'],
        Tabs: [
            { ID: 1, Name: '选号(前五名)' },
            { ID: 2, Name: '选号(后五名)' },
            { ID: 3, Name: '双面' }
        ],
        CurrentTab: 1,
        init: function () {
            $scope.bet.CurrentTab = $scope.bet.Tabs[0];

            $http.post('/api/PKRate/GetCurrentPKRates').success(function (res) {
                console.log(res);
                if (res.Success) {
                    $scope.bet.PKRates = res.Data;
                }
            });
        },
        getTopFive: function (rate) {
            return rate.Rank <= 5;
        },
        getLastFive: function (rate) {
            return rate.Rank > 5 && ran.Rank <= 10;
        },
        getBSOE: function (rate) {
            return rate.Rank > 10;
        }
    };
}]);