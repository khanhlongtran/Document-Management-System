namespace DocumentsManagementSystem.Models
{
    // Create path to download file
    public class FileDetail
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    // Access this folder, after that, list all file in this folder and display
    public class FilesViewModel
    {
        public List<FileDetail> Files { get; set; }
        = new List<FileDetail>();
    }
}
