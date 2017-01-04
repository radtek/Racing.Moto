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
        $stockTable.append("aaa");
    }

    // Start the connection
    $.connection.hub.start().done(init);

    $("#roadBg").MyFloatingBg({ direction: 5, speed: 5 });


    // moto
    var motoRacing = {
        PKInfo: null,
        run: function (pkInfo) {
            // new racing
            if (pkInfo.PK.PKId != motoRacing.PKInfo.PK.PKId) {
                motoRacing.PKInfo = pkInfo;
            }
        },
        calculatSpeed: function () {
        },
    };

});