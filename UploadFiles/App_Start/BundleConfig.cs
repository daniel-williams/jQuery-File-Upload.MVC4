using System.Web;
using System.Web.Optimization;

namespace UploadFiles
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/styles/Bootstrap/css")
                .Include("~/styles/Bootstrap/bootstrap.css")
                .Include("~/styles/Bootstrap/bootstrap-responsive.css")
                );

            bundles.Add(new StyleBundle("~/styles/css")
                .Include("~/styles/site.css")
                );

            bundles.Add(new StyleBundle("~/styles/FileUpload/css")
                .Include("~/styles/FileUpload/jquery.fileupload-ui.css")
                );


            bundles.Add(new ScriptBundle("~/scripts/js")
                .Include("~/scripts/jquery-{version}.js")
                .Include("~/scripts/jquery-ui-{version}.js")
                .Include("~/scripts/jquery.validate.js")
                .Include("~/scripts/bootstrap.js")
                );

            bundles.Add(new ScriptBundle("~/scripts/modernizr")
                .Include("~/scripts/modernizr-*")
                );

            bundles.Add(new ScriptBundle("~/scripts/FileUpload/js")
                .Include("~/scripts/FileUpload/tmpl.js")
                .Include("~/scripts/FileUpload/load-image.js")
                .Include("~/scripts/FileUpload/canvas-to-blob.js")
                .Include("~/scripts/FileUpload/jquery.iframe-transport.js")
                .Include("~/scripts/FileUpload/jquery.fileupload.js")
                .Include("~/scripts/FileUpload/jquery.fileupload-ip.js")
                .Include("~/scripts/FileUpload/jquery.fileupload-ui.js")
                .Include("~/scripts/FileUpload/locale.js")
                .Include("~/scripts/FileUpload/main.js")
                );
        }
    }
}