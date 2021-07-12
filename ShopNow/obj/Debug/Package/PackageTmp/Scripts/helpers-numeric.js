function convertToDecimal(number) {
    while (number.indexOf(',') > -1 || number.indexOf('.') > -1) {
        number = number.replace(".", "");
        number = number.replace(",", "");
    }
    var result = parseFloat(number) / 100;
    return result;
}

function formatToDecimal(number) {
    return number.toFixed(2);
}

function formatToCurrency(number) {
    return parseFloat(number).toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
}

function formatDateOnly(datetime) {
    return datetime.moment().format("Do MMM YYYY");
}
