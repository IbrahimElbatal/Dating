﻿using System;

namespace DatingApp.API.Dtos
{
    public class PhotoForDetailsDto
    {
        public int Id { get; set; }
        public bool IsMain { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
