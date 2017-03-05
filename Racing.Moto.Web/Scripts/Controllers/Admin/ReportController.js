app.controller('reportSearchController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.data = {
        UserTypes: { All: 0, Admin: 1, GeneralAgent: 2, Agent: 3, Member: 4, Vistor: 5 },
        BetTypes: [
            { ID: 1, Name: '冠軍' }, { ID: 2, Name: '亞軍' }, { ID: 3, Name: '第三名' }, { ID: 4, Name: '第四名' }, { ID: 5, Name: '第五名' },
            { ID: 6, Name: '第六名' }, { ID: 7, Name: '第七名' }, { ID: 8, Name: '第八名' }, { ID: 9, Name: '第九名' }, { ID: 10, Name: '第十名' },
            { ID: 11, Name: '大小' }, { ID: 12, Name: '單雙' },
        ],
        SearchTypes: [{ ID: 1, Name: '期数' }, { ID: 2, Name: '日期' }],
        ReportTypes: [{ ID: 1, Name: '交收報錶' }, { ID: 2, Name: '分類報錶' }],
        SettlementTypes: [{ ID: 1, Name: '已結算' }, { ID: 2, Name: '未結算' }],
    };
    $scope.init = function (userType) {
        $scope.report.init(userType);
    };

    $scope.report = {
        UserType: null,
        HistoryPKs: [],//期数
        Params: { SearchType: '2', ReportType: '1', SettlementType: '1', FromDate: null, ToDate: null, BetType: '' },
        init: function (userType) {
            $scope.report.UserType = parseInt(userType, 10);
            $scope.report.getSearchReportData();
            $scope.report.Params.FromDate = $app.formatDate(new Date(), 'yyyy-MM-dd');
            $scope.report.Params.ToDate = $scope.report.Params.FromDate;
        },
        changeDateType: function (type) {
            var from = '';
            var to = '';

            switch (type) {
                case 1:
                    from = $app.formatDate(new Date(), 'yyyy-MM-dd');
                    to = from;
                    break;
                case 2:
                    from = $app.formatDate((new Date()).addDays(-1), 'yyyy-MM-dd');
                    to = from;
                    break;
                case 3: // 本周
                    var now = new Date(); //当前日期 
                    //var dayOfWeek = now.getDay() == 0 ? 6 : now.getDay() - 1; //今天本周的第几天 
                    //var nowDay = now.getDate(); //当前日 
                    //var weekStartDate = new Date(now.getFullYear(), now.getMonth(), nowDay - dayOfWeek);
                    //var weekEndDate = new Date(now.getFullYear(), now.getMonth(), nowDay + (6 - dayOfWeek));
                    var rangeWeek = $scope.report.getRangeWeek(now);

                    from = $app.formatDate(rangeWeek.From, 'yyyy-MM-dd');
                    to = $app.formatDate(rangeWeek.To, 'yyyy-MM-dd');
                    break;
                case 4: // 上周
                    var prevWeek = new Date(); //当前日期 
                    prevWeek.setDate(prevWeek.getDate() - 7);
                    //var dayOfWeek = now.getDay() == 0 ? 6 : now.getDay() - 1; //今天本周的第几天 
                    //var nowDay = now.getDate(); //当前日 
                    //var weekStartDate = new Date(now.getFullYear(), now.getMonth(), nowDay - dayOfWeek);
                    //var weekEndDate = new Date(now.getFullYear(), now.getMonth(), nowDay + (6 - dayOfWeek));

                    var rangeWeek = $scope.report.getRangeWeek(prevWeek);

                    from = $app.formatDate(rangeWeek.From, 'yyyy-MM-dd');
                    to = $app.formatDate(rangeWeek.To, 'yyyy-MM-dd');
                    break;
                case 5: // 本月
                    var fromDateStr = new Date().getFullYear() + "-" + (new Date().getMonth() + 1) + "-1";//当前月1号
                    var formDate = $app.convertToDate(fromDateStr, 'yyyy-MM-dd');
                    from = $app.formatDate(formDate, 'yyyy-MM-dd');

                    var toDate = formDate;
                    toDate.setMonth(toDate.getMonth() + 1);//下月1号
                    toDate.setDate(toDate.getDate() - 1);   // 当前月最后一天
                    to = $app.formatDate(toDate, 'yyyy-MM-dd');
                    break;
                case 6: // 上月
                    var dateStr = new Date().getFullYear() + "-" + (new Date().getMonth() + 1) + "-1";//当前月1号
                    var date = $app.convertToDate(dateStr, 'yyyy-MM-dd');

                    var formDate = $app.convertToDate(dateStr, 'yyyy-MM-dd');
                    formDate.setMonth(formDate.getMonth() - 1)//上月1号
                    from = $app.formatDate(formDate, 'yyyy-MM-dd');

                    var toDate = date;//当前月1号
                    toDate.setDate(toDate.getDate() - 1);   // 上月最后一天
                    to = $app.formatDate(toDate, 'yyyy-MM-dd');
                    break;
            }

            $scope.report.Params.FromDate = from;
            $scope.report.Params.ToDate = to;
        },
        getRangeWeek: function (date) {
            var dayOfWeek = date.getDay() == 0 ? 6 : date.getDay() - 1; //今天本周的第几天 
            var nowDay = date.getDate(); //当前日 
            var from = new Date(date.getFullYear(), date.getMonth(), nowDay - dayOfWeek);
            var to = new Date(date.getFullYear(), date.getMonth(), nowDay + (6 - dayOfWeek));
            //from = $app.formatDate(weekStartDate, 'yyyy-MM-dd');
            //to = $app.formatDate(weekEndDate, 'yyyy-MM-dd');

            return { From: from, To: to };
        },
        getSearchReportData: function () {
            $http.post('/Admin/Report/getSearchReportData', {}).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.report.HistoryPKs = res.data.Data.HistoryPKs;
                    $scope.report.Params.PKId = res.data.Data.HistoryPKs[0].PKId;
                } else {
                    alert(res.data.Message);
                }
            });
        },
        search: function () {
            //$http.post('/Admin/Report/SearchReport', $scope.report.Params).then(function (res) {
            //    console.log(res);
            //    if (res.data.Success) {
            //        $scope.report.DataBak = angular.copy($scope.report.Data);
            //        $scope.report.IsEdit = false;

            //        alert('修改成功!');
            //    } else {
            //        alert(res.data.Message);
            //    }
            //});
            var url = '/Admin/Report'
            switch ($scope.report.UserType) {
                case $scope.data.UserTypes.Admin: url += '/GeneralAgent/'; break;
                case $scope.data.UserTypes.Agent: url += '/Agent/'; break;
                case $scope.data.UserTypes.Member: url += '/Member/'; break;
            }
            var params = 'PageIndex=1&PageSize=20'
                + '&BetType=' + $scope.report.Params.BetType
                + '&SearchType=' + $scope.report.Params.SearchType
                + '&FromDate=' + $scope.report.Params.FromDate
                + '&ToDate=' + $scope.report.Params.ToDate
                + '&ReportType=' + $scope.report.Params.ReportType
                + '&SettlementType=' + $scope.report.Params.SettlementType;

            location.href = url + $scope.report.UserType + "?" + params;
        },
    };
}]);


app.controller('reportListController', ['$scope', '$rootScope', '$http', '$compile', '$timeout', '$q', '$sce', function ($scope, $rootScope, $http, $compile, $timeout, $q, $sce) {
    $scope.data = {
        UserTypes: { All: 0, Admin: 1, GeneralAgent: 2, Agent: 3, Member: 4, Vistor: 5 },
    };

    $scope.init = function (searchParams) {
        $scope.pager.init(searchParams);
    };

    $scope.pager = {
        PageIndex: 1,
        PageSize: 20,
        RowCount: 0,
        UserType: 4,
        Params: { UserType: 4, SearchType: '2', ReportType: '1', SettlementType: '1', FromDate: null, ToDate: null, BetType: '' },
        Results: [],
        Statistics: {},
        init: function (searchParams) {
            $scope.pager.Params = JSON.parse(searchParams)
            $scope.pager.getResults(1);
        },
        getResults: function (pageIndex) {
            $scope.pager.Params.PageIndex = pageIndex;
            $http.post('/Admin/Report/SearchReport', $scope.pager.Params).then(function (res) {
                console.log(res);
                if (res.data.Success) {
                    $scope.pager.Results = res.data.Data.Items;
                    $scope.pager.RowCount = res.data.Data.RowCount;

                    $scope.pager.setStatistics($scope.pager.Results);
                } else {
                    alert(res.data.Message)
                }
            });
        },
        setStatistics: function () {
            var betCount = 0,
                betAmount = 0,
                memberWinOrLoseAmount = 0,
                receiveAmount = 0,
                rebateAmount = 0,
                contributeHigherLevelAmount = 0,
                payHigherLevelAmount = 0;
            angular.forEach($scope.pager.Results, function (item, index, arr) {
                betCount = betCount.add(item.BetCount);
                betAmount = betAmount.add(item.BetAmount);
                memberWinOrLoseAmount = memberWinOrLoseAmount.add(item.MemberWinOrLoseAmount);
                receiveAmount = receiveAmount.add(item.ReceiveAmount);
                rebateAmount = rebateAmount.add(item.RebateAmount);
                contributeHigherLevelAmount = contributeHigherLevelAmount.add(item.ContributeHigherLevelAmount);
                payHigherLevelAmount = payHigherLevelAmount.add(item.PayHigherLevelAmount);
            })
            $scope.pager.Statistics = {
                BetCount: betCount,
                BetAmount: betAmount,
                MemberWinOrLoseAmount: memberWinOrLoseAmount,
                ReceiveAmount: receiveAmount,
                RebateAmount: rebateAmount,
                ContributeHigherLevelAmount: contributeHigherLevelAmount,
                PayHigherLevelAmount: payHigherLevelAmount,
            };
            console.log($scope.pager.Statistics);
        },
        pageChanged: function () {
            $scope.pager.getResults($scope.pager.PageIndex);
        },
    };
}]);