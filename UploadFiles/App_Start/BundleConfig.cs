using System.Web;
using System.Web.Optimization;

namespace UploadFiles
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/styles/Bootstrap")
                .Include("~/styles/Bootstrap/bootstrap.css")
                .Include("~/styles/Bootstrap/bootstrap-responsive.css")
                .Include("~/styles/Bootstrap/bootstrap-image-gallery.css")
                );

            bundles.Add(new StyleBundle("~/styles/Site")
                .Include("~/styles/site.css")
                );

            bundles.Add(new StyleBundle("~/styles/FileUpload")
                .Include("~/styles/FileUpload/jquery.fileupload-ui.css")
                );





            bundles.Add(new ScriptBundle("~/scripts/Modernizr")
                .Include("~/scripts/modernizr-*")
                );

            bundles.Add(new ScriptBundle("~/scripts/jQuery")
                .Include("~/scripts/jquery-{version}.js")
                .Include("~/scripts/jquery-ui-{version}.js")
                .Include("~/scripts/jquery.validate.js")
                );

            bundles.Add(new ScriptBundle("~/scripts/Bootstrap")
                .Include("~/scripts/Bootstrap/bootstrap.js")
                .Include("~/scripts/Bootstrap/bootstrap-image-gallery.js")
                );
                        

            bundles.Add(new ScriptBundle("~/scripts/FileUpload")
                .Include("~/scripts/FileUpload/vendor/jquery.ui.widget.js")
                .Include("~/scripts/FileUpload/tmpl.js")
                .Include("~/scripts/FileUpload/load-image.js")
                .Include("~/scripts/FileUpload/canvas-to-blob.js")

                .Include("~/scripts/FileUpload/jquery.iframe-transport.js")
                .Include("~/scripts/FileUpload/jquery.fileupload.js")
                .Include("~/scripts/FileUpload/jquery.fileupload-fp.js")
                .Include("~/scripts/FileUpload/jquery.fileupload-ui.js")
                //.Include("~/scripts/FileUpload/locale.js")
                .Include("~/scripts/FileUpload/main.js")
                );

            bundles.Add(new ScriptBundle("~/scripts/Knockout")
                .Include("~/scripts/knockout-{version}.js")
                .Include("~/scripts/knockout.mapping-{version}.js")
                .Include("~/scripts/koExternalTemplateEngine_all.js")
                );
        }
    }
}