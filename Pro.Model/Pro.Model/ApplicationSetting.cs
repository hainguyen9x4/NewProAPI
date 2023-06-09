﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pro.Model
{
    [Table("AppSetting")]
    public class ApplicationSetting
    {
        [BsonId]
        public int AppSettingId { get; set; }

        [BsonElement("AppSettingName")]
        public string AppSettingName { get; set; }

        [BsonElement("AppSettingValue")]
        public string AppSettingValue { get; set; }

        [BsonElement("AppSettingIsActive")]
        public bool AppSettingIsActive { get; set; }

        [BsonElement("Descriptions")]
        public string Descriptions { get; set; }

        [BsonElement("NumberImage")]
        public int NumberImage { get; set; }
    }
}
