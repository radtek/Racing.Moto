app.controller('betInfoController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {

    $scope.init = function (type) {
        $scope.bet.init(type);
    };

    $scope.bet = {
        Type: 1,
        Statistic: null,
        RankAmounts: null,

        init: function (type) {
            $scope.bet.Type = parseInt(type, 10);
            $scope.bet.getBets();
        },
        getBets: function () {
            $http.post('/Bet/GetBetInfo', {}).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.bet.Statistic = res.data.Data;
                    $scope.bet.PK = res.data.Data.PKModel.PK;
                    //$scope.bet.RankAmounts = res.data.Data.BetAmountRankModels;
                    $scope.countdown.init(res.data.Data.PKModel);
                }
            });
        },
        getNumName: function (num) {
            var name = num;
            switch (num) {
                case 11: name = '大'; break;
                case 12: name = '小'; break;
                case 13: name = '单'; break;
                case 14: name = '双'; break;
            }
            return name;
        },
        getRankName: function (rank) {
            var name = '';
            switch (rank) {
                case 1: name = '冠軍'; break;
                case 2: name = '亞軍'; break;
                case 3: name = '第三名'; break;
                case 4: name = '第四名'; break;
                case 5: name = '第五名'; break;
                case 6: name = '第六名'; break;
                case 7: name = '第七名'; break;
                case 8: name = '第八名'; break;
                case 9: name = '第九名'; break;
                case 10: name = '第十名'; break;
            }
            return name;
        }
    };


    $scope.countdown = {
        init: function (pkModel) {
            var eleHour = document.getElementById('hour');
            var eleMinute = document.getElementById('minute');
            var eleSecond = document.getElementById('second');

            if (pkModel.OpeningRemainSeconds <= 0) {
                eleHour.innerHTML = '00';
                eleMinute.innerHTML = '00';
                eleSecond.innerHTML = '00';
            } else {
                var beginTime = $app.convertToDate(pkModel.CloseBeginTime);
                //var beginTime = $app.convertToDate(pkModel.GameBeginTime);  // 比赛开始倒计时
                var year = beginTime.getFullYear();
                var month = beginTime.getMonth();
                var day = beginTime.getDate();
                var hour = beginTime.getHours();
                var minute = beginTime.getMinutes();
                var second = beginTime.getSeconds();


                var d = Date.UTC(year, month, day, hour, minute, second);
                var obj = {
                    sec: eleSecond,
                    mini: eleMinute,
                    hour: eleHour
                }
                fnTimeCountDown(d, obj);
            }
        },
    };
}]);

$(function () {
    var __refreshSeconds = parseInt($('#EstateTime').val(), 10);
    var __count = 0;

    $('body').everyTime('1s', function () {
        __count++;
        if (__count >= __refreshSeconds) {
            __count = 0;

            var appElement = document.querySelector('[ng-controller=betInfoController]');
            var scope = angular.element(appElement).scope();
            scope.bet.getBets();
            scope.$apply();
        }
    });

    $('#EstateTime').change(function () {
        __refreshSeconds = $('#EstateTime').val();
    })
})