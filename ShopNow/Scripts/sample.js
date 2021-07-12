function ValidateEmptyRequiredFiels(data) {
    var cnt = 0;
    $.each(data, function (k, v) {
        var value = $('#' + v).val();
        var reg = /^([a - zA - Z0 - 9_\-\.] +)@([a - zA - Z0 - 9_\-\.] +)\.([a - zA - Z]{2, 5 })$/;

        if (value == "" || value == null || value == 0) {
            //$('#' + v).popover('destroy');
            $('#' + v).attr('data-content', '' + v + ' is required');
            $('#' + v).popover({ placement: 'right', viewport: '.popoverArea' });
            $('#' + v).popover('show');
            cnt += 1;
        }
        else if (v == "Email" || v == "ContactPersonEmail") {
            $('#' + v).attr('data-content', 'Enter the valid Email address');
        }
        else
            $('#' + v).popover('dispose');
    });
    if (cnt > 0)
        return false
    else
        return true;
}
        //var data = $('#form-task').serializeArray().reduce(function(obj, item) {
        //    obj[item.name] = item.value;
        //    return obj;
        //}, {});