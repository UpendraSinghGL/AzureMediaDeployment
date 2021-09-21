namespace Microsoft.Media.DurinMediaLake.Plugin
{
    using Microsoft.Media.DurinMediaLake.Constant;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class CreateVendorAsset : PluginBase
    {
        public override void ExecutePlugin()
        {
            if (this.PluginContext.InputParameters.Contains(PluginConstants.Target) && this.PluginContext.InputParameters[PluginConstants.Target] is Entity)
            {
                var assetfiles = this.PluginContext.InputParameters[PluginConstants.Target] as Entity;
                this.TracingService.Trace("AssetFiles Plugin: Run Started for Id - " + assetfiles.Id);

                string assetfileblobPath = String.Empty;
                string assetblobPath = String.Empty;
                Guid assetid = Guid.Empty;
                Guid vendorID = Guid.Empty;
                Guid vendorshowID = Guid.Empty;
                Guid showID = Guid.Empty;

                if (Convert.ToBoolean(assetfiles["media_isvendorupload"]) == true) //Run if asset is uploaded by vendor
                {
                    assetfileblobPath = assetfiles.GetAttributeValue<string>(MediaAssetConstants.Blobpath);
                    string[] blobPathArr = assetfileblobPath.Split('/');

                    var result = assetfileblobPath.Select((c, i) => new { c, i }).Where(x => x.c == '/').Skip(5 - 1).FirstOrDefault();
                    assetblobPath = assetfileblobPath.Substring(0, result.i + 1);
                    string vendorcontainer = blobPathArr[(int)BlobPathPositions.VendorUpload];

                    var vendorquery = new QueryExpression();
                    vendorquery.EntityName = Vendor.EntityLogicalName;
                    vendorquery.ColumnSet = new ColumnSet(Vendor.VendorId);
                    vendorquery.Criteria.AddCondition(Vendor.UploadedPath, ConditionOperator.Equal, vendorcontainer);
                    var vendor = this.OrganizationService.RetrieveMultiple(vendorquery).Entities;

                    

                    if (vendor.Count > 0)
                    {
                        Entity vendorEntity = vendor.FirstOrDefault();
                        vendorID = (Guid)vendorEntity?.Attributes[Vendor.VendorId];

                        var vendorshowquery = new QueryExpression();
                        vendorshowquery.EntityName = "media_showvendormapping";
                        vendorshowquery.ColumnSet = new ColumnSet(true);
                        vendorshowquery.Criteria.AddCondition("media_vendor", ConditionOperator.Equal, vendorID);
                        var vendorshow = this.OrganizationService.RetrieveMultiple(vendorshowquery).Entities;

                        Entity vendorshowEntity = vendorshow.FirstOrDefault();
                        vendorshowID = ((EntityReference)vendorshowEntity.Attributes["media_show"]).Id;

                        var showquery = new QueryExpression();
                        showquery.EntityName = Show.EntityLogicalName;
                        showquery.ColumnSet = new ColumnSet(Show.IdColumn, Show.EnableTranscoding, Show.EnableTranscription);
                        showquery.Criteria.AddCondition(Show.NameColumn, ConditionOperator.Equal, blobPathArr[(int)BlobPathPositions.Show]);
                        var show = this.OrganizationService.RetrieveMultiple(showquery).Entities;

                        if(show.Count>0)
                        {
                            Entity showEntity = show.FirstOrDefault();
                            showID = (Guid)showEntity?.Attributes["media_assetcontainerid"];
                            assetfiles["media_enablefortranscoding"] = showEntity.Attributes[Show.EnableTranscoding];
                            assetfiles["media_enablefortranscription"] = showEntity.Attributes[Show.EnableTranscription];
                        }

                    }
                    //Check if Show is assigned to Vendor only then create Asset
                    if (vendorshowID == showID)
                    {
                        var query = new QueryExpression();
                        query.EntityName = MediaAssetConstants.EntityLogicalName;
                        query.ColumnSet = new ColumnSet("media_assetid", "media_name", "media_blobpath");
                        query.Criteria.AddCondition(MediaAssetConstants.Blobpath, ConditionOperator.Equal, assetblobPath);

                    var asset = this.OrganizationService.RetrieveMultiple(query).Entities;
                    if (asset.Count == 0)
                    {
                        //Create Asset
                        try
                        {
                            var entity = new Entity(MediaAssetConstants.EntityLogicalName);
                            entity["media_assetstatus"] = new OptionSetValue(UploadStatus.Started);
                            entity["media_folderfilecount"] = 0;
                            entity["media_name"] = blobPathArr[(int)BlobPathPositions.Asset];
                            entity["media_shootday"] = blobPathArr[(int)BlobPathPositions.Asset];
                            entity["media_blobpath"] = assetblobPath;
                            entity["media_isvendorupload"] = true;
                            entity["media_assetstatus"] = new OptionSetValue(UploadStatus.Completed);
                            entity["media_vendor"] = new EntityReference(MediaAssetFileConstants.EntityLogicalName, vendorID); ;

                            assetid = this.OrganizationService.Create(entity);
                        }
                        catch (Exception ex)
                        {
                            this.TracingService.Trace("CreateVendorAsset: Run Failed | " + ex.Message);
                        }
                    }
                    else
                    {
                        Entity retrievedEntity = asset.FirstOrDefault();
                        assetid = (Guid)retrievedEntity?.Attributes["media_assetid"];
                    }
                    if (assetid != Guid.Empty && showID !=Guid.Empty)
                    {
                        assetfiles[MediaAssetConstants.EntityLogicalName] = new EntityReference(MediaAssetFileConstants.EntityLogicalName, assetid);    
                    }
                    this.OrganizationService.Update(assetfiles);
                }
                }
            }
        }
    }
}
