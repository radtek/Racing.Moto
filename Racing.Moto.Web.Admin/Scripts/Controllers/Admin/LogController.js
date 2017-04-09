app.controller('loginRecordController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.init = function () {
        $scope.pager.getResults(1);
    };

    $scope.pager = {
        PageIndex: 1,
        PageSize: 15,
        RowCount: 0,
        Params: {
            PageIndex: 1,
            PageSize: 15
        },
        Results: [],
        getResults: function (pageIndex) {
            $scope.pager.Params.PageIndex = pageIndex;
            $http.post('/Log/GetLoginLogRecords', $scope.pager.Params).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.pager.Results = res.data.Data.Items;
                    $scope.pager.RowCount = res.data.Data.RowCount;

                    angular.forEach($scope.pager.Results, function (item, index, arr) {
                        item.RoleName = $scope.data.getTypeName(item.UserDegree);
                    });
                } else {
                    alert(res.data.Message)
                }
            });
        },
        pageChanged: function () {
            $scope.pager.getResults($scope.pager.PageIndex);
        },
    };
}]);