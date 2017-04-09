app.controller('lotteryController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.data = {
    };
    $scope.init = function () {
        $scope.pager.getResults(1);
    };

    $scope.pager = {
        PageIndex: 1,
        PageSize: 15,
        RowCount: 0,
        Params: {
            Key: '',
            PageIndex: 1,
            PageSize: 15
        },
        Results: [],
        init: function () {
            $scope.pager.getResults(1);
        },
        getResults: function (pageIndex) {
            $scope.pager.Params.PageIndex = pageIndex;
            $http.post('/Admin/Lottery/GetHistory', $scope.pager.Params).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.pager.Results = res.data.Data.Items;
                    $scope.pager.RowCount = res.data.Data.RowCount;

                    angular.forEach($scope.pager.Results, function (item, index, arr) {
                        item.PKNo = item.PKId.toString().padLeft(8, '0');
                        item.RankArr = item.Ranks != null ? item.Ranks.split(',') : [];
                        item.Week = $app.getWeek(item.EndTime);
                    });
                    console.log($scope.pager.Results);
                } else {
                    alert(res.data.Message)
                }
            });
        },
        pageChanged: function () {
            $scope.pager.getResults($scope.pager.PageIndex);
        },
        search: function () {
            if ($scope.pager.Params.Key == '') {
                alert('请输入期数');
                return;
            }
            var pkNoPattern = /[0-9]+/;
            if (!$scope.pager.Params.Key.match(pkNoPattern)) {
                alert('请输入有效的期数');
                return;
            }
            $scope.pager.getResults(1);
        },
        reset: function () {
            $scope.pager.Params.Key = '';
            $scope.pager.getResults(1);
        },
    };
}]);