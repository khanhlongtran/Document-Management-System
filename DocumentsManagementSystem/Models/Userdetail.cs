using System;
using System.Collections.Generic;

namespace DocumentsManagementSystem.Models
{
    public partial class Userdetail
    {
        public Userdetail()
        {
            Documents = new HashSet<Document>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Mobile { get; set; } = null!;

        public virtual ICollection<Document> Documents { get; set; }
    }
}
