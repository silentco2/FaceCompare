namespace FaceCompare;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage())
		{
			Title = "FaceCompare",
			Width = 960,
			Height = 720,
			MinimumWidth = 600,
			MinimumHeight = 500
		};
	}
}
