app.controller('userManagementController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.data = {
        UserTypes: { Admin: 1, GeneralAgent: 2, Agent: 3, Member: 4 },
        LoginUserId: null,
        ParentUsers: []
    };

    $scope.init = function (userType, userId, loginUserId) {
        $scope.data.LoginUserId = parseInt(loginUserId, 10);
        $scope.user.init(userType, userId);
        $scope.webApi.getParentUsers(userType, loginUserId);
    };

    $scope.user = {
        UserType: null,
        UserId: null,
        IsEdit: false,
        CurrentUser: {},
        init: function (userType, userId) {
            $scope.user.UserType = parseInt(userType, 10);
            $scope.user.UserId = userId != null ? parseInt(userId, 10) : 0;
            if ($scope.user.UserId > 0) {
                $scope.user.getCurrentUser($scope.user.UserId);
            }
        },
        getCurrentUser: function (userId) {
            $http.post('/Admin/User/GetUser/' + userId, {}).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.user.CurrentUser = angular.merge(res.data.Data, $scope.user.CurrentUser);
                    $scope.user.CurrentUser.IsLocked = $scope.user.CurrentUser.IsLocked ? 'true' : 'false';
                    $scope.user.CurrentUser.Password = '';
                } else {
                    alert(res.data.Message)
                }
            });
        },
        save: function () {
            var data = {
                type: $scope.user.UserType,
                user: $scope.user.CurrentUser
            };
            $http.post('/Admin/User/SaveUser', data).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.user.DataBak = angular.copy($scope.user.Data);

                    alert('操作成功!');

                    $scope.user.gotoList();
                } else {
                    alert(res.data.Message);
                }
            });
        },
        gotoList: function () {
            var url = '';

            switch ($scope.user.UserType) {
                case $scope.data.UserTypes.GeneralAgent: url = '/admin/user/GeneralAgent'; break;
                case $scope.data.UserTypes.Agent: url = '/admin/user/Agent'; break;
                case $scope.data.UserTypes.Member: url = '/admin/user/Member'; break;
            }

            location.href = url;
        },
        revert: function () {
            $scope.user.Data = angular.copy($scope.user.DataBak);
        },
    };

    $scope.webApi = {
        getParentUsers: function (userType, loginUserId) {
            // 添加代理/会员时取父亲节点
            if (userType == $scope.data.UserTypes.Agent || userType == $scope.data.UserTypes.Member) {
                $http.post('/api/User/GetParentUsers/' + userType, {}).then(function (res) {
                    console.log(res);
                    if (res.data.Success) {
                        $scope.data.ParentUsers = res.data.Data;
                        if ($scope.data.ParentUsers.length > 0) {
                            $scope.user.CurrentUser.ParentUserId = $scope.data.ParentUsers[0].UserId;
                        }
                    }
                });
            } else {
                if ($scope.user.UserType == $scope.data.UserTypes.GeneralAgent) {
                    // 添加总代理, 登录人是admin
                    $scope.user.CurrentUser.ParentUserId = loginUserId;
                    $scope.data.ParentUsers = [{ UserId: loginUserId, UserName: 'Admin' }];
                }
            }
        },
    };
}]);