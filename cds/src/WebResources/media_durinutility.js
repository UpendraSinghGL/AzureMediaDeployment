(function (DurinMedia) {
    var Utility = {};
    DurinMedia.Utility = Utility;
    Utility.displayIconInView = function (a, b, c) {
        return [JSON.parse(a).media_logo_url_Value];
    }

    Utility.NavigateTo = function (selectedid, entity, attribute) {
        fetch(Xrm.Utility.getGlobalContext().getClientUrl() + `/api/data/v9.1/${entity}(${selectedid.replace(/[{}]/g, '')})/${attribute}`)
            .then(res => res.json())
            .then(data => {
                location.href = data.value;
            })
    }

    Utility.OnMacUserSave = function (context) {
        if (context && context.getFormContext()) {
            var formContext = context.getFormContext();
            var selectedUser = formContext.getAttribute('media_user').getValue();
            if (selectedUser && selectedUser.length > 0) {
                formContext.getAttribute('media_name').setValue(selectedUser[0].name);
            }
        }

    }
}(window.DurinMedia = window.DurinMedia || {}))