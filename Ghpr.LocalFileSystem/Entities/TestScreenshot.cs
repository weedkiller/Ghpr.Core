﻿using System;
using Newtonsoft.Json;

namespace Ghpr.LocalFileSystem.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TestScreenshot
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        public byte[] Data { get; set; }
        public Guid TestGuid { get; set; }

    }
}