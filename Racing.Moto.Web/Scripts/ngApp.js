var app = angular.module('RacingMotoNgApp', [
    'angular.filter',
    'ui.bootstrap'
]).run(function ($rootScope) {
    $rootScope.RequestCount = 0;
    $rootScope.HideCount = 0;
    $rootScope.Loading = true;
});

app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common['Accept'] = '*/*';

    $httpProvider.interceptors.push(['$q', '$rootScope', '$timeout', function ($q, $rootScope, $timeout) {
        return {
            'request': function (config) {
                if (config.url.toLowerCase().indexOf('template/typeahead') > -1) {
                    $rootScope.Loading = false;
                } else {
                    $rootScope.RequestCount++;
                }
                return config;
            },
            'requestError': function (rejection) {
                if ($rootScope.RequestCount > 0) {
                    $rootScope.RequestCount--;
                }
                $timeout(function () {
                    if ($rootScope.RequestCount == 0 && $rootScope.Loading == false) {
                        $rootScope.Loading = true;
                    }
                }, 500);

                return $q.reject(rejection);
            },
            'response': function (response) {
                if ($rootScope.RequestCount > 0) {
                    $rootScope.RequestCount--;
                }
                $timeout(function () {
                    if ($rootScope.RequestCount == 0 && $rootScope.Loading == false) {
                        $rootScope.Loading = true;
                    }
                }, 500);
                return response;
            },
            'responseError': function (rejection) {
                if ($rootScope.RequestCount > 0) {
                    $rootScope.RequestCount--;
                }
                $timeout(function () {
                    if ($rootScope.RequestCount == 0 && $rootScope.Loading == false) {
                        $rootScope.Loading = true;
                    }
                }, 500);
                return $q.reject(rejection);
            }
        };
    }]);

    if (!$httpProvider.defaults.headers.get) {
        $httpProvider.defaults.headers.common = {};
    }
    $httpProvider.defaults.headers.common["Cache-Control"] = "no-cache";
    $httpProvider.defaults.headers.common.Pragma = "no-cache";
    $httpProvider.defaults.headers.common["If-Modified-Since"] = "0";

}]);

app.directive('inputMask', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.inputmask();
        }
    };
});
app.directive('disableElements', function () {
    return {
        //scope: {
        //    disableElements: '='
        //},
        link: function ($scope, $element, $attrs) {
            $scope.$watch($attrs.disableElements, function (isDisabled) {
                if (isDisabled) {
                    angular.element('input', $element).attr('disabled', 'disabled').addClass('ng-disabled');
                } else {
                    angular.element('input', $element).removeAttr('disabled').removeClass('ng-disabled');
                }
            });
            
        }
    }
});