namespace LeapExpression;

static class Program
{
	private static FormHelper helper = new FormHelper();

	/// <summary>
	/// The leap motion handle for the running program.
	/// </summary>
	public static LeapMotionHandle leapMotionHandle;

	/// <summary>
	/// The tracking location at the last specified MIDI event.
	/// <summary>
	public static LeapMotionHandle.LastLocation lastLocation;

	/// <summary>
	///  The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main()
	{
		// To customize application configuration such as set high DPI settings or default font,
		// see https://aka.ms/applicationconfiguration.
		ApplicationConfiguration.Initialize();

		Task.Run(() =>
		{
			while (true) {
				Console.Out.WriteLine("try ReflectMidiInput()");
				MidiInterop.ReflectMidiInput();
				Console.Out.WriteLine("tried ReflectMidiInput()");
			}
		});

		// Task.Run(() =>
		// {
		var mainWindow = new MainWindow();
		Application.Run(mainWindow);

		// });
		
		// var midiInput = mainWindow.GetMidiInputDevice();
		// var inputDevices = helper.GetMidiDevices(MidiDevice.Inputs);
	}    
}