using System.ComponentModel.DataAnnotations;

namespace DocumentsManagementSystem.Models
{
    public class CreateViewModel
    {
        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileContent { get; set; }
        [Required]
        public bool FileStatus { get; set; } = false;
    }
}
