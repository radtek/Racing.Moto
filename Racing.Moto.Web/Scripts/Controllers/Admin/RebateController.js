app.controller('rebateController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.data = {
        Default: {
            Type1: { A: 0.04, B: 0.03, C: 0.02 },
            Type2: { A: 0.04, B: 0.03, C: 0.02 },
        },
        UserType: 2,
    };
    $scope.init = function (userType) {
        $scope.data.UserType = userType;
        $scope.opt.init();
    };

    $scope.opt = {
        Default: {},
        init: function () {
            $scope.opt.Default = angular.copy($scope.data.Default);
        },
    };
}]);