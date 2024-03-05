using System;
using System.Collections.Generic;

namespace DocumentsManagementSystem.Models
{
    public partial class Version
    {
        public int VersionId { get; set; }
        public int DocId { get; set; }
        public string? UpdatedContent { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public virtual Document Doc { get; set; } = null!;
    }
}
