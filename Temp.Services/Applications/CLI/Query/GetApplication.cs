﻿namespace Temp.Services.Applications.CLI.Query;

public class GetApplication
{


    public class ApplicationViewModel
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

