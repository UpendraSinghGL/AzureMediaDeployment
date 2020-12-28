namespace Microsoft.Media.DurinMediaLake.Plugin
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class CameraMetadataExtraction : PluginBase
    {
        public override void ExecutePlugin()
        {
            if (this.PluginContext.InputParameters.Contains("Target") && this.PluginContext.InputParameters["Target"] is Entity)
            {
                var asset = this.PluginContext.InputParameters["Target"] as Entity;
                if (asset != null)
                {
                    var cameraFileMetatdata = Convert.ToString(asset.Attributes["media_camerarawfilemetadata"]);
                    var assetid = Convert.ToString(asset.Attributes["media_assetid"]);
                    if (!string.IsNullOrEmpty(cameraFileMetatdata))
                    {

                        var query = new QueryExpression();
                        query.EntityName = "media_assetfiles";
                        query.ColumnSet = new ColumnSet("media_name");
                        query.Criteria.AddCondition("media_asset", ConditionOperator.Equal, assetid);

                        var assetFiles = this.OrganizationService.RetrieveMultiple(query).Entities;
                        if (assetFiles.Count > 0)
                        {
                            var lines = cameraFileMetatdata.Split('\n');

                            int columnLineNo = -1;
                            int dataStartFromLineNo = -1;
                            var columns = new List<string>();
                            for (int lineno = 0; lineno < lines.Length; lineno++)
                            {
                                if (string.IsNullOrWhiteSpace(lines[lineno]))
                                    continue;
                                var line = lines[lineno];
                                if (Convert.ToString(line).Trim('\t') == "Column")
                                {
                                    columnLineNo = lineno + 1;
                                }
                                if (Convert.ToString(line).Trim('\t') == "Data")
                                {

                                    dataStartFromLineNo = lineno + 1;
                                    continue;
                                }

                                if (lineno == columnLineNo)
                                {
                                    columns.AddRange(line.Split('\t'));
                                }
                                else if (dataStartFromLineNo > -1 && dataStartFromLineNo <= lineno)
                                {
                                    var assetfileid = string.Empty;

                                    var data = line.Split('\t');

                                    Dictionary<string, string> attrdict = new Dictionary<string, string>();
                                    for (int columnindex = 0; columnindex < columns.Count; columnindex++)
                                    {
                                        attrdict.Add(columns[columnindex], data[columnindex]);
                                        if (columns[columnindex] == "Source File")
                                        {
                                            var assetfile = assetFiles.Where(x => Convert.ToString(x.Attributes["media_name"]) == data[columnindex]).FirstOrDefault();
                                            if (assetfile != null)
                                                assetfileid = Convert.ToString(assetfile.Attributes["media_assetfilesid"]);
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(assetfileid))
                                    {

                                        foreach (string key in attrdict.Keys)
                                        {
                                            try
                                            {
                                                var entity = new Entity("media_camerarawfilemetadata");
                                                entity.Attributes.Add("media_keyname", key);
                                                entity.Attributes.Add("media_keyvalue", attrdict[key]);
                                                entity.Attributes.Add("media_assetfiles", new EntityReference("media_assetfiles", Guid.Parse(assetfileid)));
                                                this.OrganizationService.Create(entity);
                                            }
                                            catch (Exception e)
                                            {
                                            }
                                        }
                                    }
                                }

                            }

                        }
                    }
                }
            }
        }
    }
}
