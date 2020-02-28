using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Caliburn.Micro;
using Revon.UI.ViewModels;
using Revon.UI.Views;

namespace Revon.UI
{
    public static class FileLocations
    {
        //AddInDirectory is initialized at runtime
        public static String AddInDirectory;
        public static String AssemblyName;
        public static String AssemblyPath;
        public static readonly String imperialTemplateDirectory = @"E:\ProgramData\Autodesk\RVT 2020\Family Templates\English_I\";
        public static readonly String addInResourcesDirectory = @"E:\ProgramData\Autodesk\Revit\Addins\2020\Revon";
        public static readonly String ResourceNameSpace = @"Revon.UI.Resources";
        public static readonly String CommandNameSpace = "Revon.UI.Commands.";
    }

    /// <summary>
    /// Implements the Revit add-in interface IExternalApplication
    /// BootstrapperBase is instantiating Caliburn MVVM
    /// </summary>
    public class App : BootstrapperBase, IExternalApplication
    {

        AppBootstrapper boot;
        ShellViewModel shellViewModel = null;
        ShellView shellForm = null;

        internal static App revonApp = null;
        internal static UIApplication uiApp = null;
        UIControlledApplication uicApp;

        public static App Instance
        {
            get { return revonApp; }
        }

        public static UIApplication UIApp
        {
            get { return uiApp; }
        }

        /// <summary>
        /// Called upon loadin plugin. Global plugin configuration tasks are performed here.
        /// Once configuration is ready, registers the plugin ribbon.
        /// </summary>
        /// <param name="UIApp"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {

            uicApp = application;

            shellForm = null;
            revonApp = this;

            //initialize AssemblyName using reflection
            FileLocations.AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            FileLocations.AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FileLocations.AddInDirectory = application.ControlledApplication.AllUsersAddinsLocation + "\\" + FileLocations.AssemblyName + "\\";
            
            // Create a custom ribbon tab
            String tabName = "BIM System";
            application.CreateRibbonTab(tabName);
            RibbonPanel RevonRibbonPanel = application.CreateRibbonPanel(tabName, "BIM System");

            //load image resources
            BitmapImage largeIcon = GetEmbeddedImageResource("iconLarge.png");
            BitmapImage smallIcon = GetEmbeddedImageResource("iconLarge.png");

            // Create Commands

            // import command
            PushButtonData importButton = new PushButtonData(
                name: "Import",
                text: "BIM System",
                assemblyName: typeof(App).Assembly.Location,
                className: "Revon.UI.Commands.ShellCommand")
            {
                LargeImage = ScaledIcon(largeIcon, 32, 32),
                Image = ScaledIcon(smallIcon, 16, 16),
                ToolTip = "Import Families from BIMSystem cloud database.",
            };

            
            RevonRibbonPanel.AddItem(importButton);
          
            // initialize caliburn
            boot = new AppBootstrapper();

            return Result.Succeeded;
        }

        public void ShowShellForm(UIApplication uiapp)
        {

            if (uiApp == null)
            {
                uiApp = uiapp;
            }

            if (shellForm == null || !shellForm.IsVisible)
            {
                shellViewModel = new ShellViewModel(uiapp);
                new WindowManager().ShowWindow(shellViewModel);

                ChangeMergeDefaultAppConfig();

                // if we have a dialog, we need Idling too
                uiApp.Idling += IdlingHandler;

            }
        }

        #region resources

        /// <summary>
        /// Utility method to retrieve an embedded image resource from the assembly
        /// </summary>
        /// <param name="resourceName">The name of the image, corresponding to the filename of the embedded resouce added to the solution</param>
        /// <returns>The loaded image represented as a BitmapImage</returns>
        BitmapImage GetEmbeddedImageResource(String resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream str = asm.GetManifestResourceStream(FileLocations.ResourceNameSpace + "." + resourceName);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = str;
            bitmapImage.EndInit();

            return bitmapImage;

        }

        /// <summary>
        /// Scale down large icon to desired size for Revit 
        /// ribbon button, e.g., 32 x 32 or 16 x 16
        /// </summary>
        static BitmapSource ScaledIcon(BitmapImage large_icon, int w, int h)
        {
            return BitmapToBitmapSource(ResizeImage(
              BitmapImageToBitmap(large_icon), w, h));
        }

        /// <summary>
        /// Convert a Bitmap to a BitmapSource
        /// </summary>
        static BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();

            BitmapSource retval;

            try
            {
                retval = Imaging.CreateBitmapSourceFromHBitmap(
                  hBitmap, IntPtr.Zero, Int32Rect.Empty,
                  BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
            return retval;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(
              0, 0, width, height);

            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution,
              image.VerticalResolution);

            using (var g = Graphics.FromImage(destImage))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(image, destRect, 0, 0, image.Width,
                      image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        /// <summary>
        /// Convert a BitmapImage to Bitmap
        /// </summary>
        static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        #endregion

        private void ChangeMergeDefaultAppConfig()
        {

            //ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            //fileMap.ExeConfigFilename = configPath;
            //ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            // Merge configuration files
            //ConfigFileManager config = new ConfigFileManager(configPath,
            //    AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, true); //makeMergeFromConfigPathTheSavePath

            string configPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "Revon.UI.dll.config");

            // the default app.config is used.
            // credit to Daniel Hilgarth: http://stackoverflow.com/questions/6150644/change-default-app-config-at-runtime

            AppConfig.Change(configPath);

            // var c = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            // var setting1 = ConfigurationManager.AppSettings["Setting1"];
        }

        #region Idling

        Result IExternalApplication.OnShutdown(UIControlledApplication application)
        {
            if (shellForm != null)
            {
                shellForm.Close();
                shellForm = null;
            }


            return Result.Succeeded;
        }

        /// <summary>
        ///   A handler for the Idling event.
        /// </summary>
        /// <remarks>
        ///   We keep the handler very simple. First we check
        ///   if we still have the dialog. If not, we unsubscribe from Idling,
        ///   for we no longer need it and it makes Revit speedier.
        ///   If we do have the dialog around, we check if it has a request ready
        ///   and process it accordingly.
        /// </remarks>
        /// 
        public void IdlingHandler(object sender, IdlingEventArgs e)
        {
            if (uiApp == null)
            {
                uiApp = sender as UIApplication;
            }
            else
            {
                // fetch the request from the dialog

                RequestId request = shellViewModel.Request.Take();

                if (request != RequestId.None)
                {
                    try
                    {
                        // we take the request, if any was made,
                        // and pass it on to the request executor

                        FamilyRequestHandler.Execute(uiApp, request);
                    }
                    finally
                    {
                        // The dialog may be in its waiting state;
                        // make sure we wake it up even if we get an exception.

                        shellViewModel.WakeUp();
                    }
                }
            }

            e.SetRaiseWithoutDelay();

            return;
        }

        #endregion

    }
}
