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

    public class AddAssetRelatedRecords : PluginBase
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

                    this.TracingService.Trace("AddAssetRelatedRecords Plugin: Run Started for Id - " + assetEntity.Id);

                    if (Convert.ToBoolean(assetEntity["media_isvendorupload"]) == true) //Run if asset is uploaded by vendor
                    {
                        blobPath = assetEntity.GetAttributeValue<string>(MediaAssetConstants.Blobpath);
                        string[] blobPathArr = blobPath.Split('/');

                        if (blobPathArr.Length > 0)
                        {
                            string showRecordName = blobPathArr[(int)BlobPathPositions.Show];
                            string seasonRecordName = blobPathArr[(int)BlobPathPositions.Season];
                            string episodeblockRecordName = blobPathArr[(int)BlobPathPositions.EpisodeBlock];

                            // Retrieve the related show by show name in blob path
                            ColumnSet columnSet = new ColumnSet("media_name", "media_assetcontainerid");
                            Entity retrievedShow = GetRecordFilteredByNameAndLookup(Show.EntityLogicalName, showRecordName, columnSet);

                            if (retrievedShow != null)
                            {
                                showId = Convert.ToString(retrievedShow.Id);
                                assetEntity[MediaAssetConstants.RefShow] = new EntityReference(Show.EntityLogicalName, Guid.Parse(showId));  
                                this.TracingService.Trace(string.Format("AddAssetRelatedRecords: Successfully fetched ref show '{0}' - {1}", retrievedShow[Show.NameColumn], showId));
                            }

							// Retrieve the related season by season name in blob path and show Id 
                            columnSet = new ColumnSet("media_name", "media_seasonid");
                            Entity retrievedSeason = GetRecordFilteredByNameAndLookup(MediaAssetConstants.RefSeason, seasonRecordName, columnSet, showId, "media_show");

							if (retrievedSeason != null)
							{
                                seasonId = Convert.ToString(retrievedSeason.Id);
								assetEntity[MediaAssetConstants.RefSeason] = new EntityReference(Show.EntityLogicalName, Guid.Parse(seasonId));
                                this.TracingService.Trace(string.Format("AddAssetRelatedRecords: Successfully fetched ref season '{0}' - {1}", retrievedSeason[Show.NameColumn], seasonId));
							}

							// Retrieve the related episode/block by episode/block name in blob path and season id 
                            columnSet = new ColumnSet("media_name", "media_episodeid");
							Entity retrievedEpisode = GetRecordFilteredByNameAndLookup(MediaAssetConstants.RefEpisode, episodeblockRecordName, columnSet, seasonId, MediaAssetConstants.RefSeason);

							if (retrievedEpisode != null)
							{
                                episodeblockId = Convert.ToString(retrievedEpisode.Id);
								assetEntity[MediaAssetConstants.RefEpisode] = new EntityReference(Show.EntityLogicalName, Guid.Parse(episodeblockId));
                                this.TracingService.Trace(string.Format("AddAssetRelatedRecords: Successfully fetched ref episode '{0}' - {1}", retrievedEpisode[Show.NameColumn], episodeblockId));
							}
							else
							{
                                columnSet = new ColumnSet("media_name", "media_blockid");
                                Entity retrievedBlock = GetRecordFilteredByNameAndLookup(MediaAssetConstants.RefBlock, episodeblockRecordName, columnSet, seasonId, MediaAssetConstants.RefSeason);
                                if (retrievedBlock != null)
                                {
                                    episodeblockId = Convert.ToString(retrievedBlock.Id);
                                    assetEntity[MediaAssetConstants.RefBlock] = new EntityReference(Show.EntityLogicalName, Guid.Parse(episodeblockId));
                                    this.TracingService.Trace(string.Format("AddAssetRelatedRecords: Successfully fetched ref block '{0}' - {1}", retrievedBlock[Show.NameColumn], episodeblockId));
                                }
                            }

                            assetEntity.Attributes[MediaAssetConstants.AssetStatus] = new OptionSetValue(UploadStatus.Completed);
                            this.OrganizationService.Update(assetEntity);

                            this.TracingService.Trace("AddAssetRelatedRecords: Run Completed");
                        }
                        else
                        {
                            this.TracingService.Trace("AddAssetRelatedRecords: blob path is blank!");
                        }

                    }
					else
					{
                        this.TracingService.Trace("AddAssetRelatedRecords Plugin: Operations skipped | Asset is not uploaded by vendor. Asset Id - " + assetEntity.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                this.TracingService.Trace("AddAssetRelatedRecords: Run Failed | " + ex.Message);
            }
        }

		public Entity GetRecordFilteredByNameAndLookup(string entityLogicalName, string recordName, ColumnSet columnSet, string lookupfieldId = null, string lookupfieldName = null)
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
										AttributeName = "media_name",
										Operator = ConditionOperator.Equal,
										Values = { recordName }
									}
								}
				}
			};
            //fetchQuery.AddLink("media_season", "media_assetcontainerid", "media_Show");
            if (lookupfieldId != null)
            {
                fetchQuery.Criteria.AddCondition(lookupfieldName, ConditionOperator.Equal, lookupfieldId);
            }

            DataCollection<Entity> fetchedEntityList = this.OrganizationService.RetrieveMultiple(fetchQuery).Entities;

			Entity retrievedEntity = fetchedEntityList.FirstOrDefault();

            return retrievedEntity;

            
		}
	}
}
