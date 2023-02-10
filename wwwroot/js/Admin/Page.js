$(document).ready(function () {
    reindexParameters();
});

function addParameter() {
    var name = $('#NewParameterName').val();
    $('#NewParameterName').val('');

    var newParam = $('<tr class="parameterRow"><td><input type="text" class="parameterName" readonly/></td><td><input type="text" class="parameterValue"/></td><td><a href="#" onclick="deleteParameter(this)"><i class="material-icons md-24" icon-name="delete">delete</i></a></td></tr>')
    newParam.attr('data-name', name);
    newParam.find('.parameterName').val(name);

    $('.parameters').append(newParam);

    var macroitem = $('<span></span>');
    newParam.attr('data-name', name);
    macroitem.html('@{Field:' + name + '}');
    macroitem.attr('data-handler', "Field")
    macroitem.attr('data-name', name)

    $('.MacrosList .value').prepend(macroitem);

    reindexParameters();

    return false;
}

function deleteParameter(e) {
    var row = $(e).closest('tr');
    var name = $(row).attr('data-name');

    $(row).remove();

    $('.MacrosList .value').find('[data-handler="Field"][data-name="' + name + '"]').remove();

    reindexParameters();
    return false;
}

function reindexParameters() {
    $('.parameters .parameterRow').each(function (i, e) {
        var thisRow = $(e);
        thisRow.find('.parameterName').attr('name', 'Page.Parameters[' + i + '].Name');
        thisRow.find('.parameterValue').attr('name', 'Page.Parameters[' + i + '].Value');
    });
}