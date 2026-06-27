using System;
using System.Collections.Generic;

namespace TODOAPI_Auth.DTOs
{
    public class GitHubIssueDto
    {
        public long Id { get; set; }
        public int Number { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Url { get; set; } = string.Empty;
        public List<string> Labels { get; set; } = new();
    }
}
