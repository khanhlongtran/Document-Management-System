using DocumentsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DocumentsManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private LoginLogoutExampleContext _context;
        public HomeController(ILogger<HomeController> logger, LoginLogoutExampleContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            int userId = int.Parse(HttpContext.Session.GetString("userId"));
            var docs = _context.Documents.Where(d => d.UserId == userId).OrderByDescending(d => d.LastModifier).ToList();
            //ViewBag.Docs = docs;
            return View(docs);
        }
        public IActionResult EditView(int FileId)
        {
            // Nếu người dùng vào một file mà không có quyền => thông báo bạn không có quyền truy cập
            // Tức là userId và document.UserId khác nhau
            // Check xem file này đang ở private hay public
            bool status = _context.Documents.Single(d => d.FileId == FileId).FileStatus;
            // TH1: Nếu public thì ai cũng có thể vào được => chỉ cần File tồn tại

            // TH2: Nếu private thì chỉ có owner có thể vào được => chỉ cần file tồn tại + người khác không thể vào đươc
            // => cần check thêm điều kiện về owner bắt buộc phải là người đó
            // false => private
            if (status == false)
            {
                int userId = int.Parse(HttpContext.Session.GetString("userId")); // User logged
                int ownerId = _context.Documents.Single(d => d.FileId == FileId).UserId; // OwnerId
                if (userId != ownerId)
                {
                    return Content("You don't have permission");
                }
                var document = _context.Documents.SingleOrDefault(d => d.FileId == FileId && d.UserId == userId);
                return View(document);
            }
            else
            {
                var document = _context.Documents.SingleOrDefault(d => d.FileId == FileId);
                return View(document);
            }
        }
        [HttpPost]
        public IActionResult EditView(int FileId, string FileName, string FileContent, bool FileStatus)
        {

            var document = _context.Documents.SingleOrDefault(doc => doc.FileId == FileId);
            if (document != null)
            {
                SaveOldVersion(document);
                document.FileName = FileName;
                document.FileContent = FileContent;
                document.LastModifier = DateTime.Now;
                document.FileStatus = FileStatus;
                _context.SaveChanges();
                UpdateFileContent(document.FileName, document.FileContent);

            }
            return View(document);
        }
        private void UpdateFileContent(string fileName, string newContent)
        {
            // Construct the file path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "upload", fileName);

            // Update the file content by overwriting the entire file
            System.IO.File.WriteAllText(filePath, newContent);
        }
        public void SaveOldVersion(Document d)
        {
            Models.Version v = new Models.Version
            {
                DocId = d.FileId,
                UpdatedContent = d.FileContent,
                UpdatedTime = DateTime.Now,
            };
            _context.Versions.Add(v);
            _context.SaveChanges();
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateViewModel document)
        {
            int user = int.Parse(HttpContext.Session.GetString("userId"));
            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    _logger.LogWarning(error.ErrorMessage);
                }
            }
            if (ModelState.IsValid)
            {
                Document d = new Document
                {
                    FileName = document.FileName,
                    FileContent = document.FileContent,
                    FileStatus = document.FileStatus,
                    UserId = user,
                    LastModifier = DateTime.Now,
                };

                LoginLogoutExampleContext context = new LoginLogoutExampleContext();
                _context.Documents.Add(d);
                _context.SaveChanges();
                UpdateFileContent(d.FileName, d.FileContent);
                return RedirectToAction("index", "home");
            }
            else
            {
                ViewBag.CreateFailed = "File name and file content is not null";
            }
            return View();
        }

        public IActionResult Delete(int FileID)
        {
            int userid = int.Parse((HttpContext.Session.GetString("userId")));
            var doc = _context.Documents.SingleOrDefault(d => d.FileId == FileID && d.UserId == userid);
            if (doc != null)
            {
                var versions = _context.Versions.Where(v => v.DocId == doc.FileId).ToList();
                foreach (var version in versions)
                {
                    _context.Versions.Remove(version);
                }
                _context.Documents.Remove(doc);
                _context.SaveChanges();
                return RedirectToAction("index", "home");
            }
            else
            {
                ViewBag.DeleteFailed = "Delete failed : Docs not exist";
            }
            return View("Index");
        }
        public IActionResult Sort(string sort)
        {
            int userId = int.Parse(HttpContext.Session.GetString("userId"));
            if (sort.Equals("dateDesc"))
            {
                var docs = _context.Documents.Where(d => d.UserId == userId).OrderByDescending(d => d.LastModifier).ToList();
                return View("Index", docs);
            }
            else if (sort.Equals("dateAsc"))
            {
                var docs = _context.Documents.Where(d => d.UserId == userId).OrderBy(d => d.LastModifier).ToList();
                return View("Index", docs);
            }
            else if (sort.Equals("nameAsc"))
            {
                var docs = _context.Documents.Where(d => d.UserId == userId).OrderBy(d => d.FileName).ToList();
                return View("Index", docs);
            }
            else
            {
                return RedirectToAction("index", "home");
            }

        }
        [HttpPost]
        public IActionResult Search(string searchValue)
        {
            var doc = _context.Documents.Where(d => d.FileName.Contains(searchValue));
            ViewBag.Search = "search";
            return View("Index", doc);
        }

        // Upload View (Display list text)
        // => FileDetail, List<FileDetail> Files
        public IActionResult UploadView()
        {
            int userId = int.Parse(HttpContext.Session.GetString("userId"));
            var model = new FilesViewModel();
            // access folder upload and get all files
            foreach (var item in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Upload")))
            {
                // but it must match with in database 
                // so we will get name of this file and compare with name of file in database
                _logger.LogInformation(Path.GetFileName(item));
                _logger.LogInformation(userId.ToString());
                var doc = _context.Documents.SingleOrDefault(d => d.FileName == Path.GetFileName(item) && d.UserId == userId);
                if (doc != null)
                {
                    model.Files.Add(new FileDetail()
                    {
                        Name = System.IO.Path.GetFileName(item),
                        Path = item
                    });
                }
            }
            return View(model);
        }
        // Form upload (IFormFile[] files) using multipart
        [HttpPost]
        public IActionResult UploadView(IFormFile[] files)
        {
            var userId = HttpContext.Session.GetString("userId");
            foreach (var file in files)
            {
                // Get name of file
                var fileName = Path.GetFileName(file.FileName);
                // get path to save
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                // tạo một file và ghi vào file đó (dựa vào đường dẫn (filePath) sẽ ghi được)
                using (var localFile = System.IO.File.OpenWrite(filePath))
                using (var uploadedFile = file.OpenReadStream())
                {
                    uploadedFile.CopyTo(localFile);
                }

                var fileContent = System.IO.File.ReadAllText(filePath);

                var document = new Document()
                {
                    FileName = fileName,
                    FileContent = fileContent,
                    FileStatus = true,
                    UserId = int.Parse(userId)
                };

                _context.Documents.Add(document);
                _context.SaveChanges();
            }

            // Update list to display
            var model = new FilesViewModel();
            // access folder upload and get all files
            foreach (var item in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Upload")))
            {
                model.Files.Add(new FileDetail()
                {
                    Name = System.IO.Path.GetFileName(item),
                    Path = item
                });
            }

            return View(model);
        }

        // Using async/await for multiple download
        // /Download?fileName=?
        public async Task<IActionResult> Download(string fileName)
        {
            if (fileName == null)
            {
                return Content("File name is not available");
            }
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload", fileName);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            };
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        // Get MimeTypes (current only support .txt)
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"}
            };
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
