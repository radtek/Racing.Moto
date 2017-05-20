app.controller('rebateController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.data = {
        Default: {
            Type1: { A: 0.03, B: 0.04, C: 0.05, MaxPKAmount: 100000, MaxBetAmount: 50000 },
            Type2: { A: 0.03, B: 0.04, C: 0.05, MaxPKAmount: 100000, MaxBetAmount: 50000 },
        },
        RankNames: ['冠軍', '亞軍', '第三名', '第四名', '第五名', '第六名', '第七名', '第八名', '第九名', '第十名', '大', '小', '单', '双'],
    };
    $scope.init = function (userType, userId, parentId) {
        $scope.opt.init(userType, userId, parentId);
    };

    $scope.opt = {
        UserType: 2,
        UserId: 0,
        Rebates: [],
        Rebates1: [],
        Rebates2: [],
        RebatesBak: [],

        ParentUserId: 0,
        ParentRebates: [],

        init: function (userType, userId, parentId) {
            $scope.opt.UserType = userType;
            $scope.opt.UserId = userId;
            $scope.opt.ParentUserId = parentId;

            $scope.opt.getRebates(userId);
            $scope.opt.getParentRebates(parentId);
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
        getParentRebates: function (parentId) {
            $scope.opt.ParentRebates = [];

            $http.post('/User/GetRebates', { id: parentId }).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.opt.ParentRebates = res.data.Data;
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
            var msg = $scope.opt.checkRebate();
            if (msg.length > 0) {
                var msgStr = msg.join('\n');
                alert(msgStr);
                return;
            }

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
        checkRebate: function () {
            //下级退水<自己退水
            var msg = [];
            angular.forEach($scope.opt.Rebates, function (item, index, arr) {
                var rank = $scope.data.RankNames[index];
                var parentRebate = $scope.opt.getParentRebate(item.RebateNo);
                if (parentRebate != null) {
                    if (item.RebateTypeA > parentRebate.RebateTypeA) {
                        msg.push(rank + ' A盘 退水不能大于上级退水: ' + parentRebate.RebateTypeA);
                    }
                    if (item.RebateTypeB > parentRebate.RebateTypeB) {
                        msg.push(rank + ' B盘 退水不能大于上级退水: ' + parentRebate.RebateTypeB);
                    }
                    if (item.RebateTypeC > parentRebate.RebateTypeC) {
                        msg.push(rank + ' C盘 退水不能大于上级退水: ' + parentRebate.RebateTypeC);
                    }
                }
            })
            return msg;
        },
        getParentRebate: function (rebateNo) {
            var rebate = null;
            angular.forEach($scope.opt.ParentRebates, function (item, index, arr) {
                if (item.RebateNo == rebateNo) {
                    rebate = item;
                }
            })
            return rebate;
        },
    };
}]);