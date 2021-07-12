function readURL(input) {

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            console.log(e);

            $('#' + input.alt)
                .attr('src', e.target.result)
        };

        reader.readAsDataURL(input.files[0]);
    }
    else {
        var img = input.value;
        $('#' + input.alt).attr('src', img);
    }
    $("#x").show().css("margin-right", "10px");
}