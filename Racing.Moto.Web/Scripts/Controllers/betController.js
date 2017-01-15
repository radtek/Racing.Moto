app.controller('betController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', function ($scope, $rootScope, $http, $compile, $timeout, $q) {

    $scope.init = function () {
        $scope.bet.init();
    };

    $scope.bet = {
        TopFive: [],
        LastFive: [],
        BSOE: [],
        Tabs: [
            { ID: 1, Name: '选号(前五名)' },
            { ID: 2, Name: '选号(后五名)' },
            { ID: 3, Name: '双面' }
        ],
        CurrentTab: null,
        init: function () {
            $scope.bet.CurrentTab = $scope.bet.Tabs[0];

            for (var i = 1; i <= 10; i++) {

            }
        },
    };
}]);