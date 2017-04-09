app.controller('annoucementController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {

    $scope.init = function () {
        $scope.news.init();
    };

    $scope.news = {
        Data: null,
        init: function () {
            $http.post('/PK/GetNewsAnnouncement').then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.news.Data = res.data.Data;                    

                    // editor init
                    $scope.ue.init();
                }
            });
        },
        save: function () {
            $scope.news.Data.PostContent = $scope.ue.getContent();
            $http.post('/PK/SaveNews', $scope.news.Data).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    alert('修改成功!');
                } else {
                    alert(res.data.Message);
                }
            });
        },
        revert: function () {
            //$scope.news.Data.PostContent = $scope.news.Data.PostContentBak;
            $scope.ue.UE.setContent($scope.news.Data.PostContent);
        },
    };

    $scope.ue = {
        UE: null,
        init: function () {
            $scope.ue.UE = UE.getEditor('PostContent');
        },
        getContent: function () {
            return $scope.ue.UE.getContent();
        },
        setContent: function (content) {
            $scope.ue.UE.setContent(content);
        },
        //reset: function () {
        //    $scope.ue.UE.reset();
        //},
    };
}]);

app.controller('marqueeController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {

    $scope.init = function () {
        $scope.news.init();
    };

    $scope.news = {
        Data: null,
        init: function () {
            $http.post('/PK/GetNewsMarquee').then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.news.Data = res.data.Data;
                    $scope.news.Data.PostContentBak = $scope.news.Data.PostContent;
                }
            });
        },
        save: function () {
            $http.post('/PK/SaveNews', $scope.news.Data).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    alert('修改成功!');
                } else {
                    alert(res.data.Message);
                }
            });
        },
        revert: function () {
            $scope.news.Data.PostContent = $scope.news.Data.PostContentBak;
        },
    };
}]);