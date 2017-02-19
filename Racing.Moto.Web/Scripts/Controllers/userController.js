app.controller('userController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {

    //$scope.init = function () {
    //};

    $scope.user = {
        Params: { OldPassword: '', NewPassword: '', ConfirmPassword: '' },
        check: function () {
            if ($scope.user.Params.OldPassword == '') {
                return '请输入原始密码';
            }
            if ($scope.user.Params.NewPassword == '') {
                return '请输入新设密码';
            }
            if ($scope.user.Params.ConfirmPassword == '') {
                return '请输入确认密码';
            }
            if ($scope.user.Params.NewPassword != $scope.user.Params.ConfirmPassword) {
                return '两次输入的密码不一致';
            }
            return '';
        },
        save: function () {
            var msg = $scope.user.check();
            if (msg != '') {
                alert(msg);
                return;
            }
            $http.post('/Manage/SaveChangePassword', $scope.user.Params).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    alert('操作成功!');
                } else {
                    alert(res.data.Message);
                }
            });
        },
    };
}]);