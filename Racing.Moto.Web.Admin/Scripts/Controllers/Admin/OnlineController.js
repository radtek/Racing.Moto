app.controller('userTypeOnlineController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.data = {
        UserTypes: { All: 0, Admin: 1, GeneralAgent: 2, Agent: 3, Member: 4, Vistor: 5 },
        getTypeName: function (type) {
            var name = '';
            switch (type) {
                //case $scope.data.UserTypes.All: name = ''; break;
                case $scope.data.UserTypes.Admin: name = '管理员'; break;
                case $scope.data.UserTypes.GeneralAgent: name = '总代理'; break;
                case $scope.data.UserTypes.Agent: name = '代理'; break;
                case $scope.data.UserTypes.Member: name = '会员'; break;
                case $scope.data.UserTypes.Vistor: name = '游客'; break;
            }
            return name;
        },
    };
    $scope.init = function (userType) {
        $scope.user.init(userType);
    };

    $scope.user = {
        UserType: null,
        init: function (userType) {
            $scope.user.UserType = userType;
            $scope.pager.init(userType);
        },
        kickOut: function (userName) {
            if (confirm('确定踢出' + userName + '?')) {
                $http.post('/online/kickout/' + userName).then(function (res) {
                    console.log(res);
                    if (res.data.Success) {
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
            UserType: $scope.data.UserTypes.Agent,
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
            $http.post('/Online/GetOnlineUsers', $scope.pager.Params).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.pager.Results = res.data.Data.Items;
                    $scope.pager.RowCount = res.data.Data.RowCount;

                    angular.forEach($scope.pager.Results, function (item, index, arr) {
                        item.RoleName = $scope.data.getTypeName(item.UserDegree);
                    });
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