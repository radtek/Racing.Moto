app.controller('annoucementController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {

    $scope.init = function () {
        $scope.news.init();
    };

    $scope.news = {
        Data: null,
        init: function () {
            $http.post('/AdminNews/GetAnnouncement').then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.news.Data = res.data.Data;
                }
            });
        },
        save: function () {
            $http.post('/AdminNews/SaveNews').then(function (res) {
                console.log(res);
                if (!res.data.Success) {
                    alert(res.data.Message);
                }
            });
        },
    };
}]);

app.controller('marqueeController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {

    $scope.init = function () {
        $scope.news.init();
    };

    $scope.news = {
        Data: null,
        init: function () {
            $http.post('/AdminNews/GetMarquee').then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.news.Data = res.data.Data;
                }
            });
        },
        save: function () {
            $http.post('/AdminNews/SaveNews').then(function (res) {
                console.log(res);
                if (!res.data.Success) {
                    alert(res.data.Message);
                }
            });
        },
    };
}]);