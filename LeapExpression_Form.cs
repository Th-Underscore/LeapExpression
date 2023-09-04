using M;
using System.Linq;

namespace LeapExpression;

public partial class MainWindow : Form
{
	ControllerLeap clMyLeap;
	private FormHelper helper = new FormHelper();

	public MainWindow()
	{
		InitializeComponent();

		/// Leap Motion Controller Handle
		clMyLeap = new ControllerLeap();
		clMyLeap.MotionEvent += HandleMotionEvent;

		/// MIDI Devices Dropdown

		// Inputs
		String[] midiInputDevices = MidiDevice.Inputs.Where(x => x != null)
													 .Select(x => x.Name)
													 .ToArray();
		MidiInputComboBox.Items.Clear();
		MidiInputComboBox.Items.AddRange(midiInputDevices);
		MidiInputComboBox.SelectedIndex = 0;

		MidiInputComboBox.DropDown += new System.EventHandler(MidiInputsComboBox_DropDownOpen);
		MidiInputComboBox.DropDownClosed += new System.EventHandler(MidiInputsComboBox_DropDownUpdate);

		// Outputs
		String[] midiOutputDevices = MidiDevice.Outputs.Where(x => x != null)
													 .Select(x => x.Name)
													 .ToArray();
		MidiOutputComboBox.Items.Clear();
		MidiOutputComboBox.Items.AddRange(midiOutputDevices);
		MidiOutputComboBox.SelectedIndex = 0;

		MidiOutputComboBox.DropDown += new System.EventHandler(MidiOutputsComboBox_DropDownOpen);
		MidiOutputComboBox.DropDownClosed += new System.EventHandler(MidiOutputsComboBox_DropDownUpdate);
	}

	~MainWindow()
	{
		if (clMyLeap != null)
			clMyLeap.MotionEvent -= HandleMotionEvent;
	}

	/// <summary>
	/// Motion event handle for Leap Controller.
	/// </summary>
	private void HandleMotionEvent(object sender, Controller.MotionEventArgs e)
	{
		var mLeftRight = e.msState.fValue[(int)Controller.Motions.mLeftRight];

		// tbValue.Text = mLeftRight.ToString();

		if (MidiInterop.isReflecting)
		{
			tbValue.Text = LeapMotionHandle.SendMotionCC(mLeftRight, e.msState.fValue[(int)Controller.Motions.mForwardBack], e.msState.fValue[(int)Controller.Motions.mDistance]).ToString();
		}

		if (MidiInterop.NotePressed)
		{
			Console.Out.WriteLine($"Note Pressed At: ${mLeftRight}");
			LeapMotionHandle.lastLocation.x = mLeftRight;
			MidiInterop.NotePressed = false;
			Console.Out.WriteLine($"Last Location Set");
		}
	}

	/// <summary>
	/// Update MIDI input dropdown menu on open.
	/// </summary>
	private void MidiInputsComboBox_DropDownOpen(object sender, System.EventArgs e)
	{
		String[] midiInputDevices = MidiDevice.Inputs.Where(x => x != null)
													 .Select(x => x.Name)
													 .ToArray();
		MidiInputComboBox.Items.Clear();
		MidiInputComboBox.Items.AddRange(midiInputDevices);

		Console.Out.WriteLine("opened input dropdown: " + MidiInputComboBox.Text);
	}

	/// <summary>
	/// Get MIDI input dropdown result.
	/// </summary>
	private void MidiInputsComboBox_DropDownUpdate(object sender, System.EventArgs e)
	{
		MidiInterop.isReflecting = false;
		MidiInterop.pauseReflectingEvent.Set();

		Dictionary<String, MidiInputDevice> inputDevices = helper.GetMidiDevices(MidiDevice.Inputs);
		var midiInput = MidiInputComboBox.SelectedItem.ToString();
        var midiDevice = inputDevices[midiInput];

		Console.Out.WriteLine($"selected input device: {midiInput} = {midiDevice.Name} | Index: {midiDevice.Index}");

		MidiInterop.currentInputDeviceID = midiDevice.Index;
	}

	/// <summary>
	/// Update MIDI input dropdown menu on open.
	/// </summary>
	private void MidiOutputsComboBox_DropDownOpen(object sender, System.EventArgs e)
	{
		String[] midiOutputDevices = MidiDevice.Outputs.Where(x => x != null)
													 .Select(x => x.Name)
													 .ToArray();
		MidiOutputComboBox.Items.Clear();
		MidiOutputComboBox.Items.AddRange(midiOutputDevices);

		Console.Out.WriteLine("opened output dropdown: " + MidiOutputComboBox.Text);
	}

	/// <summary>
	/// Get MIDI output dropdown result.
	/// </summary>
	private void MidiOutputsComboBox_DropDownUpdate(object sender, System.EventArgs e)
	{
		MidiInterop.isReflecting = false;
		MidiInterop.pauseReflectingEvent.Set();

		var midiOutput = MidiOutputComboBox.SelectedItem.ToString();
		Dictionary<String, MidiOutputDevice> outputDevices = helper.GetMidiDevices(MidiDevice.Outputs);
		var midiDevice = outputDevices[midiOutput];

		Console.Out.WriteLine($"selected output device: {midiOutput} = {midiDevice.Name} | Index: {midiDevice.Index}");

		MidiInterop.currentOutputDeviceID = midiDevice.Index;
	}

	/// <summary>
	/// Retrieve MIDI input device.
	/// </summary>
	public String GetMidiInputDevice() => MidiInputComboBox.Text;
	
	/// <summary>
	/// Retrieve MIDI output device.
	/// </summary>
	public String GetMidiOutputDevice() => MidiOutputComboBox.Text;
}