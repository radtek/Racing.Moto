app.controller('rateController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {

    $scope.init = function (rateType) {
        $scope.rate.init(rateType);
    };

    $scope.rate = {
        RateType: 0,
        Data: null,
        DataBak: null,
        IsEdit: true,
        ChineseNums: ['一', '二', '三', '四', '五', '六', '七', '八', '九', '十'],
        init: function (rateType) {
            $scope.rate.RateType = rateType;
            $scope.rate.getRates();
        },
        getRates: function () {
            $http.post('/Admin/PK/GetRates', { type: $scope.rate.RateType }).then(function (res) {
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
            var data = {
                type: $scope.rate.RateType,
                rates: $scope.rate.Data
            };
            $http.post('/Admin/PK/SaveRates', data).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.rate.DataBak = angular.copy($scope.rate.Data);
                    $scope.rate.IsEdit = true;

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

            //var data = {
            //    type: $scope.rate.RateType,
            //    batchType: $scope.batch.BatchType,
            //    rate: $scope.batch.BatchRate,
            //};
            //$http.post('/Admin/PK/SaveBatch', data).then(function (res) {
            //    console.log(res);
            //    if (res.data.Success) {
            //        $scope.rate.IsEdit = false;
            //        $scope.rate.getRates();

            //        alert('修改成功!');
            //    } else {
            //        alert(res.data.Message);
            //    }
            //});

            var rate = $scope.batch.BatchRate;
            //1-名次, 2-大小, 3-单双, 4-全部
            angular.forEach($scope.rate.Data, function (item, index, arr) {
                if ($scope.batch.BatchType == '1' || $scope.batch.BatchType == '4') {
                    item.Rate1 = rate;
                    item.Rate2 = rate;
                    item.Rate3 = rate;
                    item.Rate4 = rate;
                    item.Rate5 = rate;
                    item.Rate6 = rate;
                    item.Rate7 = rate;
                    item.Rate8 = rate;
                    item.Rate9 = rate;
                    item.Rate10 = rate;
                } else if ($scope.batch.BatchType == '2' || $scope.batch.BatchType == '4') {
                    item.Big = rate;
                    item.Small = rate;
                } else if ($scope.batch.BatchType == '3' || $scope.batch.BatchType == '4') {
                    item.Odd = rate;
                    item.Even = rate;
                }
            });
        },
    };
}]);