namespace MauiSQLiteSpatialTest;

#pragma warning disable CS1591 // Suppress missing XML comment warnings

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}