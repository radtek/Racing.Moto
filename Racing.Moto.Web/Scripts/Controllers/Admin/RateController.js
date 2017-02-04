app.controller('rateController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {

    $scope.init = function () {
        $scope.rate.init();
    };

    $scope.rate = {
        Data: null,
        DataBak: null,
        IsEdit: false,
        ChineseNums: ['一', '二', '三', '四', '五', '六', '七', '八', '九', '十'],
        init: function () {
            $http.post('/Admin/PK/GetRates').then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.rate.Data = res.data.Data;
                    $scope.rate.DataBak = angular.copy(res.data.Data);
                }
            });
        },
        edit: function () {
            $scope.rate.IsEdit = true;
        },
        save: function () {
            $http.post('/Admin/PK/SaveRates', $scope.rate.Data).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.rate.DataBak = angular.copy($scope.rate.Data);
                    $scope.rate.IsEdit = false;

                    alert('修改成功!');
                } else {
                    alert(res.data.Message);
                }
            });
        },
        revert: function () {
            $scope.rate.Data = angular.copy($scope.rate.DataBak);
        },
    };

    $scope.batch = {
        BatchType: '',
        BatchRate: '',
        saveBatchRate: function () {
            if ($scope.batch.BatchType == '') {
                alert('请选择批量修改类型');
                return;
            }

            var data = {
                type: $scope.batch.BatchType,
                rate: $scope.batch.BatchRate,
            };
            $http.post('/Admin/PK/SaveBatch', data).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.rate.IsEdit = false;
                    $scope.rate.init();

                    alert('修改成功!');
                } else {
                    alert(res.data.Message);
                }
            });
        },
    };
}]);