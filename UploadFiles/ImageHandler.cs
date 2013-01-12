using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Routing;

namespace UploadFiles
{
    public class ImageHandler : IHttpHandler
    {
        private string _StorageRoot;
        private string StorageRoot
        {
            get { return _StorageRoot; }
        }

        public ImageHandler()
        {
            _StorageRoot = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), ConfigurationManager.AppSettings["DIR_FILE_UPLOADS"]);
        }

        public void ProcessRequest(HttpContext context)
        {
            RouteValueDictionary routeValues = context.Request.RequestContext.RouteData.Values;

            string id = (string)routeValues["id"];
            string filename = (string)routeValues["filename"];

            try
            {
                if (id == null || filename == null)
                {
                    throw new Exception();
                }

                //var filePath = Path.Combine(_StorageRoot, id + "-" + filename);
                FileInfo file = new FileInfo(Path.Combine(StorageRoot, id + "-" + filename));

                if (!file.Exists)
                {
                    throw new Exception();
                }

                context.Response.Buffer = true;
                context.Response.Clear();
                context.Response.ClearContent();
                context.Response.ClearHeaders();
                context.Response.AddHeader("Content-Length", file.Length.ToString());
                context.Response.ContentType = MimeTypes.ImageMimeTypes[file.Extension];
                context.Response.TransmitFile(file.FullName);
                context.Response.Flush();
                context.Response.Close();
                context.Response.End();
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 404;
                context.Response.End();
                return;
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }

    public class DobImageRouteHandler : IRouteHandler
    {
        public DobImageRouteHandler()
        {
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new ImageHandler();
        }
    }
}