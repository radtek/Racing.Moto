app.controller('userManagementController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.data = {
        UserTypes: { All: 0, Admin: 1, GeneralAgent: 2, Agent: 3, Member: 4, Vistor: 5 },
    };

    $scope.init = function (userType, userId) {
        $scope.user.init(userType, userId);
    };

    $scope.user = {
        UserType: null,
        UserId: null,
        IsEdit: false,
        CurrentUser: { IsLocked: 'false', UserExtension: {} },
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
                    $scope.user.CurrentUser = res.data.Data;
                    $scope.user.CurrentUser.IsLocked = $scope.user.CurrentUser.IsLocked ? 'true' : 'false';
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
                case $scope.data.UserTypes.General: url = '/admin/user/Agent'; break;
                case $scope.data.UserTypes.Member: url = '/admin/user/Member'; break;
            }

            location.href = url;
        },
        revert: function () {
            $scope.user.Data = angular.copy($scope.user.DataBak);
        },
    };
}]);