(function (DurinMedia) {
    var Utility = {};
    DurinMedia.Utility = Utility;
    Utility.displayIconInView = function (a, b, c) {
        return [JSON.parse(a).media_logo_url_Value];
    }

    Utility.NavigateTo = function (selectedid, entity, attribute) {
        fetch(`/api/data/v9.1/${entity}(${selectedid.replace(/[{}]/g, '')})/${attribute}`)
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
            if (formContext.ui.getFormType() == 1) {
                formContext.getAttribute('media_token').setValue(Math.random().toString(36).substr(2));
            }
        }

    }

    Utility.RenewToken = function (context) {
        context.getAttribute('media_token').setValue(Math.random().toString(36).substr(2));
        context.data.refresh(true).then(()=>{
            Xrm.App.addGlobalNotification({
                type: 1,
                level: 1,
                message: "A new token renewed. "
            }).then(
                function success(result) {

                },
                function (error) {
                    console.log(error.message);
                    // handle error conditions
                }
            );
        });
    }
}(window.DurinMedia = window.DurinMedia || {}))