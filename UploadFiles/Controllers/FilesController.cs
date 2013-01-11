using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace UploadFiles.Controllers
{
    public class FilesController : Controller
    {
        private string _StorageRoot;
        private string StorageRoot
        {
            get { return _StorageRoot; }
        }

        public FilesController()
        {
            _StorageRoot = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), ConfigurationManager.AppSettings["DIR_FILE_UPLOADS"]);
        }

        [HttpGet]
        public ActionResult List()
        {
            var fileData = new List<ViewDataUploadFilesResult>();

            DirectoryInfo dir = new DirectoryInfo(StorageRoot);
            if (dir.Exists)
            {
                string[] extensions = MimeTypes.ImageMimeTypes.Keys.ToArray();

                FileInfo[] files = dir.EnumerateFiles()
                         .Where(f => extensions.Contains(f.Extension.ToLower()))
                         .ToArray();

                if (files.Length > 0)
                {
                    foreach (FileInfo file in files)
                    {
                        var fileId = file.Name.Substring(0, 20);
                        var fileNameEncoded = HttpUtility.HtmlEncode(file.Name.Substring(21));
                        var relativePath = "/Files/" + fileId + "/" + fileNameEncoded;

                        fileData.Add(new ViewDataUploadFilesResult()
                        {
                            url = relativePath,
                            thumbnail_url = relativePath, //@"data:image/png;base64," + EncodeFile(fullName),
                            name = fileNameEncoded,
                            type = MimeTypes.ImageMimeTypes[file.Extension],
                            size = Convert.ToInt32(file.Length),
                            delete_url = relativePath,
                            delete_type = "DELETE"
                        });
                    }
                }
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        public ActionResult Find(string id, string filename)
        {
            if (id == null || filename == null)
            {
                return HttpNotFound();
            }

            var filePath = Path.Combine(_StorageRoot, id + "-" + filename);

            FileStreamResult result = new FileStreamResult(new System.IO.FileStream(filePath, System.IO.FileMode.Open), GetMimeType(filePath));
            result.FileDownloadName = filename;

            return result;
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Uploads()
        {
            var fileData = new List<ViewDataUploadFilesResult>();

            foreach (string file in Request.Files)
            {
                UploadWholeFile(Request, fileData);
            }

            var serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            var result = new ContentResult
            {
                Content = "{\"files\":" + serializer.Serialize(fileData) + "}",
            };
            return result;
        }

        private void UploadWholeFile(HttpRequestBase request, List<ViewDataUploadFilesResult> statuses)
        {
            for (int i = 0; i < request.Files.Count; i++)
            {
                HttpPostedFileBase file = request.Files[i];

                var fileId = IDGen.NewID();
                var fileName = Path.GetFileName(file.FileName);
                var fileNameEncoded = HttpUtility.HtmlEncode(fileName);
                var fullPath = Path.Combine(StorageRoot, fileId + "-" + fileName);

                file.SaveAs(fullPath);

                statuses.Add(new ViewDataUploadFilesResult()
                {
                    url = "/Files/" + fileId + "/" + fileNameEncoded,
                    thumbnail_url = "/Files/" + fileId + "/" + fileNameEncoded, //@"data:image/png;base64," + EncodeFile(fullName),
                    name = fileNameEncoded,
                    type = file.ContentType,
                    size = file.ContentLength,
                    delete_url = "/Files/" + fileId + "/" + fileNameEncoded,
                    delete_type = "DELETE"
                });
            }
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        public ActionResult Delete(string id, string filename)
        {
            if (id == null || filename == null)
            {
                return HttpNotFound();
            }

            var filePath = Path.Combine(_StorageRoot, id + "-" + filename);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToAction("Index", "Home");
        }

        private string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }

        private string GetMimeType(string filePath)
        {
            return GetMimeType(new FileInfo(filePath));
        }
        private string GetMimeType(FileInfo file)
        {
            return MimeTypes.ImageMimeTypes[file.Extension];
        }
    }
}
