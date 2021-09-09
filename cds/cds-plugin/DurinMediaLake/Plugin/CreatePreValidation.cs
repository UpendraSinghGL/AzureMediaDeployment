namespace Microsoft.Media.DurinMediaLake.Plugin
{
    using Microsoft.Media.DurinMediaLake.Constant;
    using Microsoft.Media.DurinMediaLake.Models;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
	using Microsoft.Xrm.Sdk.Query;
	using Newtonsoft.Json;
    using System;
    using System.Linq;

    public class CreatePreValidation : PluginBase
    {
        public override void ExecutePlugin()
        {
            try
            {
                // The InputParameters collection contains all the data passed in the message request.
                if (this.PluginContext.InputParameters.Contains(PluginConstants.Target) &&
                    PluginContext.InputParameters[PluginConstants.Target] is Entity)
                {
                    // Obtain the target entity from the input parameters.
                    Entity assetEntity = (Entity)this.PluginContext.InputParameters[PluginConstants.Target];

                    // Verify that the target entity represents the media_assetfiles.
                    // If not, this plug-in was not registered correctly.
                    if (assetEntity.LogicalName != MediaAssetConstants.EntityLogicalName)
                        return;

                    string blobPath = String.Empty;
                    string showId = String.Empty;
                    string seasonId = String.Empty;
                    string episodeblockId = String.Empty;

                    this.TracingService.Trace("CreatePreValidation Plugin: Run Started for Id - " + assetEntity.Id);

                    if (PluginContext.MessageName.ToLower() != "create" && PluginContext.Stage != 10)
                    {
                        return;
                    }
                    else
                    {
                        blobPath = assetEntity.GetAttributeValue<string>(MediaAssetConstants.Blobpath);
                        string name = assetEntity.GetAttributeValue<string>("media_name");
                        var query = new QueryExpression();
                        query.EntityName = MediaAssetConstants.EntityLogicalName;
                        query.ColumnSet = new ColumnSet("media_name", "media_blobpath");
                        query.Criteria.AddCondition("media_name", ConditionOperator.Equal, name);
                        query.Criteria.AddCondition(MediaAssetConstants.Blobpath, ConditionOperator.Equal, blobPath);

                        var assetrecord= this.OrganizationService.RetrieveMultiple(query).Entities;

                        if (assetrecord.Count > 0)
                        {
                            this.TracingService.Trace("Duplicate asset found");
                            throw new Exception("Duplicate asset found");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
                //this.TracingService.Trace("AddAssetRelatedRecords: Run Failed | " + ex.Message);
            }
        }


	}
}
