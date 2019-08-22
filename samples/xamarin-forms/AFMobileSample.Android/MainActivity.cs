
using Android.App;
using Android.Content.PM;
using Android.OS;
using Java.Lang;
using Android;

namespace AFMobileSample.Droid
{
    [Activity(Label = "AFMobileSample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            JavaSystem.LoadLibrary("gnustl_shared");
            JavaSystem.LoadLibrary("indy");

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                RequestPermissions(new[] { Manifest.Permission.ReadExternalStorage }, 10);
                RequestPermissions(new[] { Manifest.Permission.WriteExternalStorage }, 10);
                RequestPermissions(new[] { Manifest.Permission.Internet }, 10);
            }
        }
    }
}