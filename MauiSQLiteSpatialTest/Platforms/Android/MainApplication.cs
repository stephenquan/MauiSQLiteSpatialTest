using Android.App;
using Android.Runtime;

namespace MauiSQLiteSpatialTest;

#pragma warning disable CS1591 // Suppress missing XML comment warnings for brevity

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
