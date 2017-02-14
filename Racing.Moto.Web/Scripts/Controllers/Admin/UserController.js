app.controller('userController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {

    $scope.init = function (userType) {
        $scope.user.init(userType);
    };

    $scope.user = {
        UserTypes: { All: 0, Admin: 1, GeneralAgent: 2, Agent: 3, User: 4, Vistor: 5 },
        IsEdit: false,
        CurrentUser: null,
        init: function (userType) {
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
            $http.post('/Admin/PK/SaveUsers', data).then(function (res) {
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
            UserType: $scope.user.UserTypes.All,
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
            $http.post('/Admin/User/GetUsers', $scope.pager.Params).then(function (res) {
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

app.controller('userOnlineController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {

    $scope.init = function (userType) {
        $scope.user.init(userType);
    };

    $scope.user = {
        UserTypes: { All: 0, Admin: 1, GeneralAgent: 2, Agent: 3, User: 4, Vistor: 5 },
        init: function (userType) {
            $scope.pager.init(userType);
        },
    };

    $scope.pager = {
        PageIndex: 1,
        PageSize: 15,
        RowCount: 0,
        Params: {
            UserType: $scope.user.UserTypes.Agent,
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
            $http.post('/Admin/User/GetOnlineUsers', $scope.pager.Params).then(function (res) {
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