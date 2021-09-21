namespace Microsoft.Media.DurinMediaLake.Constant
{
    public class PluginConstants
    {
        public const string Target = "Target";
    }

    public class MediaAssetFileConstants
    {
        public const string EntityLogicalName = "media_assetfiles";
        public const string MediaInfoMetadata = "media_mediainfometadata";
        public const string UploadStatus = "media_uploadstatus";
        public const string MetadataStatus = "media_metadatastatus";
        public const string VendorMetadataStatus = "media_vendormetadatastatus";
        public const string AlefileContent = "media_alefilecontent";
        public const string BlobPath = "media_blobpath";
        public const string miscInfo = "media_miscinfo";
        public const string Status = "statecode";
    }

    public class UploadStatus
    {
        public const int Completed = 207940001;
        public const int Started = 207940000;
        public const int PartiallyUpload = 207940003;
    }

    public class MediaAssetConstants
    {
        public const string EntityLogicalName = "media_asset";
        public const string UploadedFileCount = "media_uploadedfilecount";
        public const string AssetStatus = "media_assetstatus";
        public const string UploadedFile = "media_uploadedfile";
        public const string FolderFileCount = "media_folderfilecount";
        public const string Blobpath = "media_blobpath";
        public const string RefShow = "media_assetcontainer";
        public const string isVendorUploaded = "media_isvendorupload";
        public const string RefSeason = "media_season";
        public const string RefEpisode = "media_episode";
        public const string RefBlock = "media_block";
    }

    public class MediaTrackConstants
    {
        public const string EntityLogicalName = "media_track";
        public const string Type = "media_type";
        public const string Format = "media_format";
        public const string RefAssetFile = "media_assetfile";
    }

    public class MetadataConstants
    {
        public const string EntityLogicalName = "media_metadata";
        public const string KeyName = "media_name";
        public const string KeyValue = "media_keyvalue";
        public const string RefTrack = "media_track";
        public const string CameraMetadataEntityLogicalName = "media_camerarawfilemetadata";
    }

    public class MiscInfo
    {
        public string AleFileNameField { get; set; }
        public string MatchType { get; set; }
        public int TruncateCharFromStart { get; set; }
        public int TruncateCharFromEnd { get; set; }
    }

    public enum FileType
    {
        Audio = 207940000,
        Video = 207940001,
        Image = 207940002,
        Other = 207940003,
        Script = 207940004,
    };

    public enum BlobPathPositions
    {
        VendorUpload = 0,
        Show = 1,
        Season = 2,
        EpisodeBlock = 3,
        Asset = 4,
    };

    public enum Operator
    {
        Max = 207940000,
        Min = 207940001,
        Equals = 207940002,
        In = 207940003,
    };

    public class Show
    {
        public const string EntityLogicalName = "media_assetcontainer";
        public const string NameColumn = "media_name";
        public const string IdColumn = "media_assetcontainerid";
        public const string LookupColumn = "_media_show_value";
        public const string EnableTranscoding = "media_enabletranscoding";
        public const string EnableTranscription = "media_enabletranscription";
    }
    public class Vendor
    {
        public const string EntityLogicalName = "media_vendor";
        public const string NameColumn = "media_name";
        public const string UploadedPath = "media_uploadpath";
        public const string VendorId = "media_vendorid";
    }

    public class ValidationRules
    {
        public string FieldName { get; set; }
        public string Tracks { get; set; }
        public int Operator { get;set; }
        public string Value { get; set; }
    }
    public class VendorRules
    {
        public const string EntityLogicalName = "media_validationrule";
        public const string FieldName = "media_fieldname";
        public const string Tracks = "media_tracks";
        public const string Operator = "media_operator";
        public const string Value = "media_value";
    }
    public class VendorMetadataStatus
    {
        public const int Matched = 207940000;
        public const int NotMatched = 207940001;
        public const int TranscodingRequired = 207940003;
        public const int MatchFailed = 207940002;
    }
    public class MetadataExtractStatus
    {
        public const int Extracted = 207940000;
        public const int NotExtracted = 207940001;
        public const int ExtractFailed = 207940002;
    }
}
