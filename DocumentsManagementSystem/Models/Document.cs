using System;
using System.Collections.Generic;

namespace DocumentsManagementSystem.Models
{
    public partial class Document
    {
        public Document()
        {
            Versions = new HashSet<Version>();
        }

        public int FileId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileContent { get; set; } = null!;
        public bool FileStatus { get; set; }
        public int UserId { get; set; }
        public DateTime? LastModifier { get; set; }

        public virtual Userdetail User { get; set; } = null!;
        public virtual ICollection<Version> Versions { get; set; }
    }
}
