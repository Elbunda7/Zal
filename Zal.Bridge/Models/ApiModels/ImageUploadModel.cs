﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Bridge.Models.ApiModels
{
    public class ImageUploadModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool SetAsMain { get; set; }
    }
}
