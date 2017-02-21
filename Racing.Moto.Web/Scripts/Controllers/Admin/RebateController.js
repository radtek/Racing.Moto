app.controller('rebateController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.data = {
        Default: {
            Type1: { A: 0.04, B: 0.03, C: 0.02 },
            Type2: { A: 0.04, B: 0.03, C: 0.02 },
        },
        RankNames: ['冠軍', '亞軍', '第三名', '第四名', '第五名', '第六名', '第七名', '第八名', '第九名', '第十名'],
    };
    $scope.init = function (userType, userId) {
        $scope.opt.init();
    };

    $scope.opt = {
        UserType: 2,
        UserId: 0,
        Rebates: [],
        Rebates1: [],
        Rebates2: [],
        RebatesBak: [],
        init: function (userType, userId) {
            $scope.opt.UserType = userType;
            $scope.opt.UserId = userId;

            $scope.opt.getRebates(userId);
        },
        getRebates: function (userId) {
            $scope.opt.Rebates = [];
            $scope.opt.Rebates1 = [];
            $scope.opt.Rebates2 = [];
            $scope.opt.RebatesBak = [];

            //$scope.opt.Default = angular.copy($scope.data.Default);
            $http.post('/Admin/User/GetRebates', { userId: userId }).then(function (res) {
                if (res.data.Success) {
                    $scope.opt.Rebates = res.data.Data;
                    $scope.opt.RebatesBak = angular.copy(res.data.Data);

                    angular.forEach($scope.opt.Rebates, function (item, index, arr) {
                        if (index < 7) {
                            $scope.opt.Rebates1.push(item);
                        } else {
                            $scope.opt.Rebates2.push(item);
                        }
                    })
                }
            });
        },
        saveBatch: function (type) {
            // type: 1-名次, 2-大小单双
            angular.forEach($scope.opt.Rebates, function (item, index, arr) {
                if (type == 1) {
                    if (index <= 10) {
                        item.RebateTypeA = $scope.data.Type1.A;
                        item.RebateTypeB = $scope.data.Type1.B;
                        item.RebateTypeC = $scope.data.Type1.C;
                        item.Changed = true;
                    }
                } else {
                    if (index > 10) {
                        item.RebateTypeA = $scope.data.Type2.A;
                        item.RebateTypeB = $scope.data.Type2.B;
                        item.RebateTypeC = $scope.data.Type2.C;
                        item.Changed = true;
                    }
                }
            })
        },
        saveRebates: function () {
            $http.post('/Admin/User/SaveRebates', $scope.opt.Rebates).then(function (res) {
                if (res.data.Success) {
                    $scope.opt.reset();
                    alert('操作成功!');
                } else {
                    alert(res.data.Message);
                }
            });
        },
        reset: function () {
            angular.forEach($scope.opt.Rebates, function (item, index, arr) {
                item.Changed = false;
            })
        },
    };
}]);