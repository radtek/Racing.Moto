app.controller('userController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.data = {
        UserTypes: { All: 0, Admin: 1, GeneralAgent: 2, Agent: 3, Member: 4, Vistor: 5 },
    };
    $scope.init = function (userType) {
        $scope.user.init(userType);
    };

    $scope.user = {
        UserType: null,
        IsEdit: false,
        CurrentUser: null,
        init: function (userType) {
            $scope.user.UserType = userType;
            $scope.pager.init(userType);
        },
        edit: function () {
            $scope.user.IsEdit = true;
        },
        save: function () {
            var data = {
                type: $scope.user.UserType,
                users: $scope.user.Data
            };
            $http.post('/PK/SaveUsers', data).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.user.DataBak = angular.copy($scope.user.Data);
                    $scope.user.IsEdit = false;

                    alert('修改成功!');
                } else {
                    alert(res.data.Message);
                }
            });
        },
        revert: function () {
            $scope.user.Data = angular.copy($scope.user.DataBak);
        },
    };

    $scope.pager = {
        PageIndex: 1,
        PageSize: 15,
        RowCount: 0,
        Params: {
            UserType: $scope.data.UserTypes.All,
            PageIndex: 1,
            PageSize: 15
        },
        Results: [],
        init: function (userType) {
            $scope.pager.Params.UserType = userType;
            $scope.pager.getResults(1);
        },
        getResults: function (pageIndex) {
            $scope.pager.Params.PageIndex = pageIndex;
            $http.post('/User/GetUsers', $scope.pager.Params).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.pager.Results = res.data.Data.Items;
                    $scope.pager.RowCount = res.data.Data.RowCount;
                } else {
                    alert(res.data.Message)
                }
            });
        },
    };
}]);

//网站参考版
app.controller('userListController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.data = {
        UserTypes: { All: 0, Admin: 1, GeneralAgent: 2, Agent: 3, Member: 4, Vistor: 5 },
    };

    $scope.init = function (userType, fatherUserId, grandFatherUserId) {
        $scope.user.init(userType, fatherUserId, grandFatherUserId);
    };

    $scope.user = {
        UserType: null,
        FatherUserId: null,
        GrandFatherUserId: null,
        init: function (userType, fatherUserId, grandFatherUserId) {
            $scope.user.UserType = parseInt(userType, 10);
            $scope.user.FatherUserId = fatherUserId;
            $scope.user.GrandFatherUserId = grandFatherUserId;
            $scope.pager.init(userType, fatherUserId, grandFatherUserId);
        },
        addUser: function (userType) {
            location.href = $scope.user.getUrl();
        },
        editUser: function (userId) {
            location.href = $scope.user.getUrl() + '/' + userId;
        },
        rebate: function (userId) {
            location.href = '/user/rebate/' + $scope.user.UserType + '/' + userId;
        },
        getUrl: function () {
            var url = '';

            switch ($scope.user.UserType) {
                case $scope.data.UserTypes.GeneralAgent: url = '/user/GeneralAgentManagement'; break;
                case $scope.data.UserTypes.Agent: url = '/user/AgentManagement'; break;
                case $scope.data.UserTypes.Member: url = '/user/MemberManagement'; break;
            }

            return url;
        },
        removeUser: function (userId, enabled) {
            if (confirm('确定删除?')) {
                $http.post('/User/RemoveUser', { id: userId, enabled: enabled }).then(function (res) {
                    console.log(res);
                    if (res.data.Success) {
                        alert('操作成功!');
                        $scope.pager.getResults($scope.pager.PageIndex);
                    } else {
                        alert(res.data.Message)
                    }
                });
            }
        },
        lockUser: function (userId, locked) {
            if (confirm('确定冻结?')) {
                $http.post('/User/LockUser', { id: userId, locked: locked }).then(function (res) {
                    console.log(res);
                    if (res.data.Success) {
                        alert('操作成功!');
                        $scope.pager.getResults($scope.pager.PageIndex);
                    } else {
                        alert(res.data.Message)
                    }
                });
            }
        },
    };

    $scope.pager = {
        PageIndex: 1,
        PageSize: 15,
        RowCount: 0,
        Params: {
            FatherUserId: 0,
            GrandFatherUserId: 0,
            UserType: $scope.data.UserTypes.Agent,
            //IsLocked: 'false',
            PageIndex: 1,
            PageSize: 15
        },
        Results: [],
        init: function (userType, fatherUserId, grandFatherUserId) {
            $scope.pager.Params.UserType = userType;
            $scope.pager.Params.FatherUserId = fatherUserId;
            $scope.pager.Params.GrandFatherUserId = grandFatherUserId;
            $scope.pager.getResults(1);
        },
        getResults: function (pageIndex) {
            $scope.pager.Params.PageIndex = pageIndex;
            $http.post('/User/GetUsers', $scope.pager.Params).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.pager.Results = res.data.Data.Items;
                    $scope.pager.RowCount = res.data.Data.RowCount;
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