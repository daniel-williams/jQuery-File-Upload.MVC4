using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace UploadFiles.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "File Uploads";

            return View();
        }

        

        [HttpGet]
        public ActionResult Uploads()
        {
            var fileData = new List<ViewDataUploadFilesResult>();

            DirectoryInfo dir = new DirectoryInfo(StorageRoot);
            if (dir.Exists)
            {
                string[] extensions = ImageMimeTypes.Keys.ToArray();

                FileInfo[] files = dir.EnumerateFiles()
                         .Where(f => extensions.Contains(f.Extension.ToLower()))
                         .ToArray();

                if(files.Length > 0)
                {
                    foreach (FileInfo file in files)
                    {
                        var fullPath = Path.Combine(StorageRoot, Path.GetFileName(file.Name));
                        var fileNameEncoded = HttpUtility.HtmlEncode(Path.GetFileName(file.Name));

                        fileData.Add(new ViewDataUploadFilesResult()
                        {
                            url = "/Home/Download/?id=" + fileNameEncoded,
                            thumbnail_url = "/Home/Download/?id=" + fileNameEncoded,
                            name = fileNameEncoded,
                            type = ImageMimeTypes[file.Extension],
                            size = Convert.ToInt32(file.Length),
                            delete_url = "/Home/Delete/?id=" + fileNameEncoded,
                            delete_type = "GET"
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

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Uploads(string fileId = "default")
        {
            var fileData = new List<ViewDataUploadFilesResult>();

            foreach (string file in Request.Files)
            {
                
                var headers = Request.Headers;

                if (string.IsNullOrEmpty(headers["X-File-Name"]))
                {
                    UploadWholeFile(Request, fileData);
                }
                else
                {
                    UploadPartialFile(headers["X-File-Name"], Request, fileData);
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

        private void UploadPartialFile(string fileName, HttpRequestBase request, List<ViewDataUploadFilesResult> statuses)
        {
            if (request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];
            var inputStream = file.InputStream;

            var fullName = Path.Combine(StorageRoot, Path.GetFileName(fileName));
            var fileNameEncoded = HttpUtility.HtmlEncode(Path.GetFileName(file.FileName));

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }
            statuses.Add(new ViewDataUploadFilesResult()
            {
                url = "/Home/Download/?id=" + fileNameEncoded,
                thumbnail_url = @"data:image/png;base64," + EncodeFile(fullName),
                name = fileNameEncoded,
                type = file.ContentType,
                size = file.ContentLength,
                delete_url = "/Home/Delete/?id=" + fileNameEncoded,
                delete_type = "GET"
            });
        }

        private void UploadWholeFile(HttpRequestBase request, List<ViewDataUploadFilesResult> statuses)
        {
            for (int i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];

                var fullPath = Path.Combine(StorageRoot, Path.GetFileName(file.FileName));
                var fileNameEncoded = HttpUtility.HtmlEncode(Path.GetFileName(file.FileName));

                file.SaveAs(fullPath);

                statuses.Add(new ViewDataUploadFilesResult()
                {
                    url = "/Home/Download/?id=" + fileNameEncoded,
                    thumbnail_url = @"data:image/png;base64," + EncodeFile(fullPath),
                    name = fileNameEncoded,
                    type = file.ContentType,
                    size = file.ContentLength,
                    delete_url = "/Home/Delete/?id=" + fileNameEncoded,
                    delete_type = "GET"
                });
            }
        }

        private string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }


        [HttpGet]
        public void Delete(string id)
        {
            var filename = id;
            var filePath = Path.Combine(Server.MapPath("~/App_Data/Files"), filename);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }


        [HttpGet]
        public void Download(string id)
        {
            var filename = id;
            var filePath = Path.Combine(Server.MapPath("~/App_Data/Files"), filename);

            var context = HttpContext;

            if (System.IO.File.Exists(filePath))
            {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            }
            else
                context.Response.StatusCode = 404;
        }


        private string StorageRoot
        {
            get { return Path.Combine(Server.MapPath("~/App_Data/Files")); }
        }

        private static Dictionary<string, string> ImageMimeTypes = new Dictionary<string, string>
		{
			{ ".gif", "image/gif" },
			{ ".jpg", "image/jpeg" },
			{ ".png", "image/png" },
		};
    }

    public class ViewDataUploadFilesResult
    {
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
    }
}
