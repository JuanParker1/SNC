function ValidateEmptyRequiredFiels(data) {
    var cnt = 0;
    $.each(data, function (k, v) {

        //var n = v.search('-');
        //console.log('n: ' + n + ' \n v: ' + v);
        //if (n > -1) {
        //    var splitted = v.split('-')[1];
        //    var root = v.split('-')[0];
        //    if (splitted == 'gender') {
        //        var genderVal = $("input[name='" + v + "']:checked").val();
        //        if (typeof (genderVal) == 'undefined') {
        //            var errmsg = '<br/><span style="color:#ce2121" class="errmsg">This fields is required</span>';
        //            $('#' + v).closest('div').find('.errmsg').remove();
        //            $('#' + v).closest('div').append(errmsg);

        //            cnt += 1;
        //        }
        //    }
        //}
 
        var value = $('#' + v).val();
        var errmsg = '<text style="color:#ce2121" class="errmsg">This fields is required</text>';

        if (value == "" || value == null) {
            $('#' + v).closest('div').find('.errmsg').remove();
            $('#' + v).closest('div').append(errmsg);
            $('#' + v).focus();
            cnt += 1;
        }
        else {
            $('#' + v).closest('div').find('.errmsg').remove();
        }
    });

    if (cnt > 0)
        return false
    else
        return true;
}