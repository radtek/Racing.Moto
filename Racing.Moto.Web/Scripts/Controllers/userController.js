app.controller('userController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.init = function (email) {
        $scope.user.init(email);
    };

    $scope.user = {
        Params: { Email: '', OldPassword: '', NewPassword: '', ConfirmPassword: '' },
        init: function (email) {
            $scope.user.Params.Email = email;
        },
        check: function () {
            if ($scope.user.Params.Email == '') {
                return '请输入电子邮箱';
            } else {
                var emailPattern = /^(\w)+(\.\w+)*@(\w)+((\.\w{2,3}){1,3})$/;
                if (!$scope.user.Params.Email.match(emailPattern)) {
                    return '邮箱格式不正确.';
                }
            }
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

            var res = $app.checkPwd(pwd);
            if (!res.IsValid) {
                return res.Message;
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