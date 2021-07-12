function crud(That, url, params, fields) {
    var state = $(That).attr('id').split('-')[1];
    $(That).attr('disabled', 'disabled');
    switch (state) {
        case 'create':
            url = url + 'Create';
            $.getJSON(url, params, function (data) {
                $(That).html("<span class='fa fa-pencil-square-o'></span>&nbsp; Edit").attr('id', 'luckyBtn-edit-' + data.Id);
                console.log(data);
                $.each(fields, function (k, v) {
                    $.each(data, function (key, val) {
                        if (v.toLowerCase() == key.toLowerCase()) {
                            $('#' + v + '-preview').removeClass('hidden').html(val);
                            $('#' + v + '-input').addClass('hidden');
                            $('#' + v + '-input').next('span').addClass('hidden');//this line is for hiding select2
                            return false;
                        }
                    });
                });
                $(That).removeAttr('disabled');
            });
            break;
        case 'edit':
            var id = $(That).attr('id').split('-')[2];
            url = url + 'Edit';
            $.getJSON(url, { id: id }, function (data) {
                console.log(data);
                $.each(fields, function (k, v) {
                    $.each(data, function (key, val) {
                        if (key.toLowerCase() == v.toLowerCase()) {                           
                            $('#' + v + '-input').removeClass('hidden').val(val);
                            $('#' + v + '-preview').html('').addClass('hidden');
                            //for shift In
                            if (v == "in") {
                                var In_hr = data.In.Hours;
                                var In_min = data.In.Minutes;
                                var timeIn = formatTime(In_hr, In_min);
                                console.log(timeIn);
                                $('#' + v + '-input').removeClass('hidden').val(timeIn);
                            }
                            //for shift Out
                            if (v == "out") {
                                 var out_hr = data.Out.Hours;
                                 var out_min = data.Out.Minutes;
                                var timeOut = formatTime(out_hr, out_min);
                                console.log(timeOut);
                                $('#' + v + '-input').removeClass('hidden').val(timeOut);
                            }
                            console.log(v);
                            console.log('val: ' + val);
                            //for shedule select2
                            $('#' + v + '-input').next('span').removeClass('hidden').val(val).trigger('change');
                            return false;
                        }
                    });
                })
                $(That).removeAttr('disabled');
                $(That).html("<span class='fa fa-pencil-square-o'></span>&nbsp; Save Changes").attr('id', 'luckyBtn-update-' + data.Id);
            });
        case 'update':
            var id = $(That).attr('id').split('-')[2];
            url = url + 'Update/?id=' + id;
            $.getJSON(url, params, function (data) {
                $(That).html("<span class='fa fa-pencil-square-o'></span>&nbsp; Edit").attr('id', 'luckyBtn-edit-' + data.Id);
                $.each(fields, function (k, v) {
                    $.each(data, function (key, val) {
                        if (v.toLowerCase() == key.toLowerCase()) {
                            $('#' + v + '-preview').removeClass('hidden').html(val);
                            $('#' + v + '-input').addClass('hidden');
                            $('#' + v + '-input').next('span').addClass('hidden');//this line is for hiding select2
                            return false;
                        }
                    });
                });
                $(That).html("<span class='fa fa-pencil-square-o'></span>&nbsp; Edit").attr('id', 'luckyBtn-edit-' + data.Id).removeAttr('disabled');
            });


    }
}

function formatTime(hr, min) {
    
    if(hr < 10)
        hr = '0' + hr;
    
    if (min < 10)
        min = '0' + min;

    return hr + ':' + min;
}