app.controller('rebateController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.data = {
        Default: {
            Type1: { A: 0.04, B: 0.03, C: 0.02, MaxPKAmount: 100000, MaxBetAmount: 50000 },
            Type2: { A: 0.04, B: 0.03, C: 0.02, MaxPKAmount: 100000, MaxBetAmount: 50000 },
        },
        RankNames: ['冠軍', '亞軍', '第三名', '第四名', '第五名', '第六名', '第七名', '第八名', '第九名', '第十名', '大', '小', '单', '双'],
    };
    $scope.init = function (userType, userId) {
        $scope.opt.init(userType, userId);
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
            $http.post('/User/GetRebates', { id: userId }).then(function (res) {
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
                    if (index < 10) {
                        item.TypeAChanged = $scope.data.Default.Type1.A != item.RebateTypeA;
                        item.TypeBChanged = $scope.data.Default.Type1.B != item.RebateTypeB;
                        item.TypeCChanged = $scope.data.Default.Type1.C != item.RebateTypeC;
                        item.MaxPKAmountChanged = $scope.data.Default.Type1.MaxPKAmount != item.MaxPKAmount;
                        item.MaxBetAmountChanged = $scope.data.Default.Type1.MaxBetAmount != item.MaxBetAmount;

                        item.RebateTypeA = $scope.data.Default.Type1.A;
                        item.RebateTypeB = $scope.data.Default.Type1.B;
                        item.RebateTypeC = $scope.data.Default.Type1.C;
                        item.MaxPKAmount = $scope.data.Default.Type1.MaxPKAmount;
                        item.MaxBetAmount = $scope.data.Default.Type1.MaxBetAmount;
                    }
                } else {
                    if (index >= 10) {
                        item.TypeAChanged = $scope.data.Default.Type2.A != item.RebateTypeA;
                        item.TypeBChanged = $scope.data.Default.Type2.B != item.RebateTypeB;
                        item.TypeCChanged = $scope.data.Default.Type2.C != item.RebateTypeC;
                        item.MaxPKAmountChanged = $scope.data.Default.Type2.MaxPKAmount != item.MaxPKAmount;
                        item.MaxBetAmountChanged = $scope.data.Default.Type2.MaxBetAmount != item.MaxBetAmount;


                        item.RebateTypeA = $scope.data.Default.Type2.A;
                        item.RebateTypeB = $scope.data.Default.Type2.B;
                        item.RebateTypeC = $scope.data.Default.Type2.C;
                        item.MaxPKAmount = $scope.data.Default.Type2.MaxPKAmount;
                        item.MaxBetAmount = $scope.data.Default.Type2.MaxBetAmount;
                    }
                }
            })
        },
        saveRebates: function () {
            var data = {
                userId: $scope.opt.UserId,
                userRebates: $scope.opt.Rebates
            };
            $http.post('/User/SaveRebates', data).then(function (res) {
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
                item.TypeAChanged = false;
                item.TypeBChanged = false;
                item.TypeCChanged = false;
            })
        },
        cancel: function () {
            var action = $scope.opt.UserType == 2 ? 'GeneralAgent' : 'Agent';
            location.href = '/user/' + action;
        },
    };
}]);