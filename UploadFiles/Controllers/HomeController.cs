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

        private string StorageRoot
        {
            get { return Path.Combine(Server.MapPath("~/App_Data/Files")); }
        }

        [HttpPost]
        public ActionResult Uploads()
        {
            var r = new List<ViewDataUploadFilesResult>();

            foreach (string file in Request.Files)
            {
                var statuses = new List<ViewDataUploadFilesResult>();
                var headers = Request.Headers;

                if (string.IsNullOrEmpty(headers["X-File-Name"]))
                {
                    UploadWholeFile(Request, statuses);
                }
                else
                {
                    UploadPartialFile(headers["X-File-Name"], Request, statuses);
                }

                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var result = new ContentResult
                {
                    Content = serializer.Serialize(statuses),
                    ContentType = "application/json"
                };
                return result;
            }

            return Json(r);
        }

        private void UploadPartialFile(string fileName, HttpRequestBase request, List<ViewDataUploadFilesResult> statuses)
        {
            if (request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var file = request.Files[0];
            var inputStream = file.InputStream;

            var fullName = Path.Combine(StorageRoot, Path.GetFileName(fileName));

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
                name = fileName,
                size = file.ContentLength,
                type = file.ContentType,
                url = "/Home/Download/?id=" + fileName,
                delete_url = "/Home/Delete/?id=" + fileName,
                thumbnail_url = @"data:image/png;base64," + EncodeFile(fullName),
                delete_type = "GET",
            });
        }

        private void UploadWholeFile(HttpRequestBase request, List<ViewDataUploadFilesResult> statuses)
        {
            for (int i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];

                var fullPath = Path.Combine(StorageRoot, Path.GetFileName(file.FileName));

                file.SaveAs(fullPath);

                statuses.Add(new ViewDataUploadFilesResult()
                {
                    name = file.FileName,
                    size = file.ContentLength,
                    type = file.ContentType,
                    url = "/Home/Download/?id=" + file.FileName,
                    delete_url = "/Home/Delete/?id=" + file.FileName,
                    thumbnail_url = @"data:image/png;base64," + EncodeFile(fullPath),
                    delete_type = "GET",
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
    }

    public class ViewDataUploadFilesResult
    {
        public string name { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string delete_url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_type { get; set; }
    }
}
