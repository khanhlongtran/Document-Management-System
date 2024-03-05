using DocumentsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentsManagementSystem.Controllers
{
    public class VersionController : Controller
    {
        LoginLogoutExampleContext context = new LoginLogoutExampleContext();
        public IActionResult Index(int FileId)
        {
            var ver = context.Versions.Where(v => v.DocId == FileId).ToList();

            return View(ver);
        }
        public IActionResult Restore(int versionid, int docid)
        {
            var ver = context.Versions.SingleOrDefault(v => v.VersionId == versionid && v.DocId == docid);
            if (ver != null)
            {
                var doc = context.Documents.SingleOrDefault(d => d.FileId == docid);

                doc.FileContent = ver.UpdatedContent;
                doc.LastModifier = DateTime.Now;
                context.SaveChanges();
                return RedirectToAction("index", "home");
            }
            return RedirectToAction("index", "home");
        }
    }
}
