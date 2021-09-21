namespace Microsoft.Media.DurinMediaLake.Plugin
{
    using Microsoft.Media.DurinMediaLake.Constant;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class CompareTemplate : PluginBase
    {
        public override void ExecutePlugin()
        {
            if (this.PluginContext.InputParameters.Contains(PluginConstants.Target) && this.PluginContext.InputParameters[PluginConstants.Target] is Entity)
            {
                var assetfiles = this.PluginContext.InputParameters[PluginConstants.Target] as Entity;
                this.TracingService.Trace("CompareTemplate Plugin: Run Started for Id - " + assetfiles.Id);

                Entity mediaassetfile = this.OrganizationService.Retrieve(MediaAssetFileConstants.EntityLogicalName, assetfiles.Id, new ColumnSet(MediaAssetConstants.isVendorUploaded, "media_asset"));

                string validtemplateid = String.Empty;
                string trackid = String.Empty;
                Guid vendorID = Guid.Empty;
                Guid showID = Guid.Empty;
                int isValid = 0;
                int isValidrule = 1;
                List <ValidationRules> metadataList = new List<ValidationRules>();
                List <ValidationRules> templateList = new List<ValidationRules>();

                try
                {
                    if (Convert.ToBoolean(mediaassetfile.Attributes[MediaAssetConstants.isVendorUploaded]) == true && ((OptionSetValue)assetfiles.Attributes[MediaAssetFileConstants.MetadataStatus]).Value == MetadataExtractStatus.Extracted) //Run if asset is uploaded by vendor
                    {
                        EntityReference assetRef = mediaassetfile.GetAttributeValue<EntityReference>(MediaAssetConstants.EntityLogicalName);
                        Entity mediasset = this.OrganizationService.Retrieve(MediaAssetConstants.EntityLogicalName, assetRef.Id, new ColumnSet("media_assetid", Vendor.EntityLogicalName,MediaAssetConstants.RefShow));

                        //Get Tracks from Asset files
                        ColumnSet columnSet = new ColumnSet("media_trackid");
                        DataCollection <Entity> retrievedtracks = GetRecordFilteredByNameAndLookup("media_track", mediaassetfile.Id, columnSet, "media_assetfile");

                        //Get Metadata from Tracks
                        foreach (var track in retrievedtracks)
                        {
                            trackid = Convert.ToString(track.Attributes["media_trackid"]);
                            QueryExpression querymetadata = new QueryExpression("media_metadata");
                            querymetadata.ColumnSet = new ColumnSet(true);
                            querymetadata.Criteria = new FilterExpression();
                            querymetadata.Criteria.AddCondition("media_track", ConditionOperator.Equal, trackid);

                            var metadatas = this.OrganizationService.RetrieveMultiple(querymetadata).Entities;

                            foreach (var metadata in metadatas)
                            {
                                metadataList.Add(new ValidationRules { FieldName = Convert.ToString(metadata.Attributes["media_name"]), Tracks = ((EntityReference)metadata.Attributes["media_track"]).Name, Value = Convert.ToString(metadata.Attributes["media_keyvalue"]) });
                            }
                        }

                        vendorID = ((EntityReference)mediasset.Attributes["media_vendor"]).Id;
                        showID = ((EntityReference)mediasset.Attributes[MediaAssetConstants.RefShow]).Id;

                        //Get Show 
                        columnSet = new ColumnSet(Show.EnableTranscoding,Show.EnableTranscription);
                        DataCollection<Entity> retrievedshowEntity = GetRecordFilteredByNameAndLookup(Show.EntityLogicalName, showID, columnSet, Show.IdColumn);
                        var retrievedshow = retrievedshowEntity.FirstOrDefault();

                        //Get Templates
                        columnSet = new ColumnSet("media_vendortemplateid");
                        DataCollection<Entity> retrievedtemplates = GetRecordFilteredByNameAndLookup("media_vendortemplate_media_vendor", vendorID, columnSet, Vendor.VendorId);

                        foreach (var entity in retrievedtemplates)
                        {
                            validtemplateid = Convert.ToString(entity.Attributes["media_vendortemplateid"]);
                            var queryrule = new QueryExpression();
                            queryrule.EntityName = VendorRules.EntityLogicalName;
                            queryrule.ColumnSet = new ColumnSet(VendorRules.FieldName, VendorRules.Tracks, VendorRules.Operator, VendorRules.Value);
                            queryrule.Criteria.AddCondition("media_validationtemplate", ConditionOperator.Equal, validtemplateid);

                            var rules = this.OrganizationService.RetrieveMultiple(queryrule).Entities;

                            foreach (var rule in rules)
                            {
                                templateList.Add(new ValidationRules { FieldName = ((EntityReference)rule.Attributes["media_fieldname"]).Name, Tracks = ((EntityReference)rule.Attributes["media_tracks"]).Name, Operator = ((OptionSetValue)rule.Attributes["media_operator"]).Value, Value = Convert.ToString(rule.Attributes["media_value"]) });
                            }
                            foreach (var template in templateList)
                            {
                                isValidrule = CompareAttributeTemplate(template, metadataList);
                                if (isValidrule == 0)
                                {
                                    templateList.Clear();
                                    break;
                                }
                            }
                            if (isValidrule == 1)
                            {
                                isValid = 1;
                                break;
                            }
                        }

                        if (isValid == 1)
                        {
                            //Update Template Matched
                            mediaassetfile.Attributes[MediaAssetFileConstants.VendorMetadataStatus] = new OptionSetValue(VendorMetadataStatus.Matched);
                            this.OrganizationService.Update(mediaassetfile);
                        }
                        else
                        {
                            //Update Template Not Matched
                            if(Convert.ToBoolean(retrievedshow.Attributes[Show.EnableTranscoding])==true)
                                mediaassetfile.Attributes[MediaAssetFileConstants.VendorMetadataStatus] = new OptionSetValue(VendorMetadataStatus.TranscodingRequired);
                            else
                                mediaassetfile.Attributes[MediaAssetFileConstants.VendorMetadataStatus] = new OptionSetValue(VendorMetadataStatus.NotMatched);
                            this.OrganizationService.Update(mediaassetfile);
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    this.TracingService.Trace("CompareTempalte Failed" + ex.Message);
                }
            }
        }
        public DataCollection<Entity> GetRecordFilteredByNameAndLookup(string entityLogicalName, Guid recordName, ColumnSet columnSet, string attributename)
		        {

			        var fetchQuery = new QueryExpression
			        {
				        EntityName = entityLogicalName,
				        ColumnSet = columnSet,
				        Criteria = new FilterExpression
				        {
					        Conditions =
								        {
									        new ConditionExpression
									        {
										        AttributeName = attributename,
										        Operator = ConditionOperator.Equal,
										        Values = { recordName }
									        }
								        }
				        }
			        };

                DataCollection<Entity> fetchedEntityList = this.OrganizationService.RetrieveMultiple(fetchQuery).Entities;

                return fetchedEntityList;

            
		}
        public int CompareAttributeTemplate(ValidationRules template, List<ValidationRules> metadatalist)
        {
            int istempValid = 0;
            List<ValidationRules> matchedrule = metadatalist.Where(fieldName => fieldName.FieldName.ToLower() == template.FieldName.ToLower() && fieldName.Tracks.ToLower() == template.Tracks.ToLower()).Select(m => new ValidationRules
            {
                FieldName = m.FieldName,
                Tracks = m.Tracks,
                Value = m.Value

            }).ToList();
            var match = matchedrule.FirstOrDefault();
            if (matchedrule.Count > 0)
            {
                    Enum Op = (Operator)template.Operator;
                    switch (Op)
                    {
                        case Operator.Equals:
                            if (template.Value == match.Value)
                            {
                                istempValid = 1;
                            }
                            break;
                        case Operator.Max:
                            if (Convert.ToInt32(match.Value) <= Convert.ToInt32(template.Value))
                                istempValid = 1;
                            break;
                        case Operator.Min:
                            if (Convert.ToInt32(match.Value) >= Convert.ToInt32(template.Value))
                                istempValid = 1;
                            break;
                        case Operator.In:
                            string[] values = template.Value.Split(',');
                            foreach (var value in values)
                            {
                                if (value == match.Value)
                                    istempValid = 1;
                            }
                            break;
                    }
            }
            else
                istempValid = 0;

            return istempValid;
        }
    }
}
