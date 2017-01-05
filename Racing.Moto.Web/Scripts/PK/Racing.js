$(function () {

    var ticker = $.connection.pKTickerHub, // the generated client-side hub proxy
        $stockTable = $('#stockTable'),
        $stockTableBody = $stockTable.find('tbody'),
        rowTemplate = '<tr data-symbol="{Symbol}"><td>{Symbol}</td><td>{Price}</td><td>{DayOpen}</td><td>{Direction} {Change}</td><td>{PercentChange}</td></tr>';

    function init() {
        ticker.server.getPKInfo().done(function (pkInfo) {
            //$stockTableBody.empty();
            //$.each(stocks, function () {
            //    var stock = formatStock(this);
            //    $stockTableBody.append(rowTemplate.supplant(stock));
            //});
            //console.log(pkInfo);
            $stockTable.append("aaa");
        });
    }

    // Add a client-side hub method that the server will call
    ticker.client.updatePKInfo = function (pkInfo) {
        //var displayStock = formatStock(stock),
        //    $row = $(rowTemplate.supplant(displayStock));

        //$stockTableBody.find('tr[data-symbol=' + stock.Symbol + ']')
        //    .replaceWith($row);
        //console.log(pkInfo);
        //$stockTable.html(pkInfo);

        //$stockTable.append("aaa");

        pkInfo.PK = { PKId: 1 };

        motoRacing.run(pkInfo);
    }

    // Start the connection
    $.connection.hub.start().done(init);


    // moto
    var motoRacing = {
        PKInfo: null,
        run: function (pkInfo) {
            // new racing
            if (motoRacing.PKInfo == null || pkInfo.PK.PKId != motoRacing.PKInfo.PK.PKId) {
                motoRacing.PKInfo = pkInfo;

                // road moving
                $("#roadBg").floatingBg({ direction: 5, speed: 5 });

                // moto moving

            }

            //test
            //$("#roadBg").floatingBg("destroy");
            //$("#roadBg").floatingBg({ direction: 5, speed: 5 });
        },
        calculateSpeed: function (ranks) {
            var pk = motoRacing.PKInfo.PK;

            //ranks: 3,2,5,6,8,7,10,1,9,4
            var rankArr = pk.Ranks.split(',');
            //var 

        },
    };

});