(function (DurinMedia) {
    var CopyVendorUtility = {};

    CopyVendorUtility.ShowVendorDialog = function (selectedRows, selectedcontrol) {
        CopyVendorUtility.openCommonDialog(selectedRows, selectedcontrol, 'vendor');
    }

    CopyVendorUtility.openCommonDialog = function (selectedRows, selectedcontrol, copyType) {
        var selectedIds = selectedRows.map(x => x.Id);
        CopyVendorUtility.CheckIfMultipleShowSelected(selectedcontrol.getEntityName(), selectedcontrol.getFetchXml(), selectedIds)
            .then((res) => {
                if (!res.isMultipleShowSelected) {
                    Xrm.Navigation.openDialog("CopyToVendorDialog", { height: 300, width: 400 },
                        {
                            media_entityName: selectedcontrol.getEntityName(),
                            media_entityIds: selectedIds.join(','),
                            media_showId: res.showId,
                            media_type: copyType
                        });
                }
                else {
                    CopyVendorUtility.showErrorMessage("Please select record for the same show");
                }
            })
    }

    CopyVendorUtility.showErrorMessage = function (msg) {
        Xrm.Navigation.openErrorDialog({ message: msg });
    }

    CopyVendorUtility.CheckIfMultipleShowSelected = function (entity, fetchXml, selectedIds) {
        return new Promise((res, rej) => {
            var apiUrl = '';
            var showColumnName = '';
            switch (entity) {
                case "media_asset":
                    {
                        apiUrl = `/api/data/v9.1/media_assets?fetchXml=${fetchXml}`;
                        showColumnName = '_media_assetcontainer_value';

                    } break;
                case "media_season": {
                    apiUrl = `/api/data/v9.1/media_seasons?fetchXml=${fetchXml}`;
                    showColumnName = "_media_show_value";
                } break;
                case "media_assetfiles": {
                    apiUrl = `/api/data/v9.1/media_assetfileses?fetchXml=${fetchXml}`;
                    showColumnName = "media_asset.media_assetcontainer";
                } break;
            }
            fetch(apiUrl).then((response) => response.json()).then(data => {
                var showid = '';
                for (index = 0; index < data.value.length; index++) {
                    var etn = data.value[index];
                    var etnid = etn[`${entity}id`];
                    var assetContainerId = etn[showColumnName];
                    if (selectedIds.indexOf(etnid) >= 0) {
                        if (showid != '' && showid != assetContainerId) {
                            res({ showId: showid, isMultipleShowSelected: true });
                            break;
                        } else {
                            showid = assetContainerId;
                        }
                    }
                }
                res({ showId: showid, isMultipleShowSelected: false });
            });
        });
    }

    CopyVendorUtility.GetVendorList = function () {
        var showId = parent.Xrm.Page.getAttribute('media_showId').getValue();
        return fetch(`/api/data/v9.1/media_showvendormappings?$select=media_name&$expand=media_Vendor($select=media_name,media_folderpath)&$filter=_media_show_value eq ${showId}`).then(res => res.json())
    }

    CopyVendorUtility.GetNexisList = function () {
        var showId = parent.Xrm.Page.getAttribute('media_showId').getValue();
        return fetch(`/api/data/v9.1/media_shownexismappings?$select=media_name&$expand=media_Nexis($select=media_name,media_linkedservicename)&$filter=_media_show_value eq ${showId}`).then(res => res.json())
    }

    CopyVendorUtility.OnDialogOk = function (formContext) {
        var type = formContext.getAttribute('media_type').getValue();
        var showId = formContext.getFormContext().getAttribute('media_showId').getValue();
        var entityName = formContext.getFormContext().getAttribute('media_entityName').getValue();
        var entityIds = formContext.getFormContext().getAttribute('media_entityIds').getValue();
        var selectedLocation = formContext.getFormContext().getAttribute('media_selectedLocation').getValue();
        // var vendorids
        // container name
        // file path
        // season , containername/name 
        if (selectedLocation) {
            Xrm.Page.ui.close();
        } else {
            CopyVendorUtility.showErrorMessage(`Please select a location to continue.`);
        }
        debugger;
    }
    CopyVendorUtility.OnDialogCancel = function (formContext) {
        Xrm.Page.ui.close();
    }

    CopyVendorUtility.CopyToNexis = function (selectedRows, selectedcontrol) {
        CopyVendorUtility.openCommonDialog(selectedRows, selectedcontrol, 'nexis');
    }

    DurinMedia.CopyVendorUtility = CopyVendorUtility;
})(window.DurinMedia = window.DurinMedia || {})