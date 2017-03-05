app.controller('userReportController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.Data = {
        UserId: null, IsSettlementDone: null,
        Ranks: ['冠军', '亚军', '第三名', '第四名', '第五名', '第六名', '第七名', '第八名', '第九名', '第十名']
    };

    $scope.init = function (userId, isSettlementDone) {
        //$scope.Data.UserId = userId;
        //$scope.Data.IsSettlementDone = isSettlementDone;

        $scope.pager.init(userId, isSettlementDone);
    };

    $scope.opt = {
        getRankName: function () { }
    };

    $scope.pager = {
        PageIndex: 1,
        PageSize: 10,
        RowCount: 0,
        Params: {
            UserId: 0,
            IsSettlementDone: true,
            PageIndex: 1,
            PageSize: 10
        },
        Results: [],
        Statistics: [],
        init: function (userId, isSettlementDone) {
            $scope.pager.Params.UserId = userId;
            $scope.pager.Params.IsSettlementDone = isSettlementDone;

            $scope.pager.getResults(1);
        },
        getResults: function (pageIndex) {
            $scope.pager.Params.PageIndex = pageIndex;
            $http.post('/Manage/GetUserBonusReport', $scope.pager.Params).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.pager.Results = res.data.Data.Items;
                    $scope.pager.RowCount = res.data.Data.RowCount;

                    $scope.pager.setResults();
                } else {
                    alert(res.data.Message)
                }
            });

            $http.post('/Manage/GetUserBonusReportStatistics', $scope.pager.Params).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.pager.Statistics = res.data.Data;
                } else {
                    alert(res.data.Message)
                }
            });
        },
        setResults: function () {
            angular.forEach($scope.pager.Results, function (item, index, arr) {
                item.Week = $app.getWeek(item.CreateTime);
                item.RankName = $scope.Data.Ranks[item.Rank - 1];
                item.NumName = item.Num <= 10 ? item.Num : $scope.pager.getBSOEName(item.Num);
            });
        },
        getBSOEName: function (num) {
            var name = '';
            switch (num) {
                case 11: name = '大'; break;
                case 12: name = '小'; break;
                case 13: name = '单'; break;
                case 14: name = '双'; break;
            }
            return name
        },
        pageChanged: function () {
            $scope.pager.getResults($scope.pager.PageIndex);
        },
    };
}]);