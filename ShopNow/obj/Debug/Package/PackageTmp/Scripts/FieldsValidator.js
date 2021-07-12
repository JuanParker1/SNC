function ValidateEmptyRequiredFiels(data) {
    var res = true;
    var cnt = 0;
    $.each(data, function (k, v) {

        var n = v.search('-');
        console.log('n: ' + n + ' \n v: ' + v);
        if (n > -1) {
            var splitted = v.split('-')[1];
            var root = v.split('-')[0];
            //console.log('splitted: ' + splitted);
            if (splitted == 'gender') {
                //console.log("input[name='" + v + "']:checked");
                var genderVal = $("input[name='" + v + "']:checked").val();
                if (typeof (genderVal) == 'undefined') {
                    //console.log('error dapat dito');
                    var errmsg = '<br/><span style="color:#ce2121" class="errmsg">this field is required</span>';
                    $('#' + v).closest('div').find('.errmsg').remove();
                    $('#' + v).closest('div').append(errmsg);

                    cnt += 1;
                }
                //console.log('genderVal: ' + genderVal);
            }
        }
        var value = $('#' + v).val();

        if (value == "" || value == null) {
            var errmsg = '<span style="color:#ce2121" class="errmsg">this field is required</span>';
            $('#' + v).closest('div').find('.errmsg').remove();
            $('#' + v).closest('div').append(errmsg);

            $('#' + v).focus();
            cnt += 1;
            //alert('fill in all the required fields');

            //res = false;
            //return false;
        }
        // alert('cnt: '+ cnt + 'v: ' + v);

    });
    //console.log('cnt: ' + cnt);
    if (cnt > 0)
        return false
    else
        return res;

}