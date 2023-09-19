using System;
using System.Runtime.InteropServices;

namespace LeapExpression;

public static class MidiInterop
{
	// MIDI API Constants
	private const int CALLBACK_FUNCTION = 0x00030000;
	private const int MMSYSERR_NOERROR = 0;

	// MIDI API Functions
	[DllImport("winmm.dll")]
	private static extern int midiInOpen(out IntPtr handle, int deviceID, IntPtr callback, IntPtr instance, int flags);

	[DllImport("winmm.dll")]
	private static extern int midiInClose(IntPtr handle);

	[DllImport("winmm.dll")]
	private static extern int midiInStart(IntPtr handle);

	[DllImport("winmm.dll")]
	private static extern int midiInStop(IntPtr handle);

	[DllImport("winmm.dll")]
	private static extern int midiOutOpen(out IntPtr handle, int deviceID, IntPtr callback, IntPtr instance, int flags);

	[DllImport("winmm.dll")]
	private static extern int midiOutClose(IntPtr handle);

	[DllImport("winmm.dll")]
	private static extern int midiOutShortMsg(IntPtr handle, int message);

    // [DllImport("winmm.dll")]
    // private static extern int midiInGetDevCaps(int deviceID, out MIDIINCAPS caps, int sizeOfMidiInCaps);

    // [DllImport("winmm.dll")]
    // private static extern int midiOutGetDevCaps(int deviceID, out MIDIOUTCAPS caps, int sizeOfMidiOutCaps);

    // [StructLayout(LayoutKind.Sequential)]
    // private struct MIDIINCAPS
    // {
    //     public short wMid;
    //     public short wPid;
    //     public int vDriverVersion;	 // MMVERSION
    //     [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    //     public string szPname;
    //     public uint dwSupport;
    // }

    // [StructLayout(LayoutKind.Sequential)]
    // private struct MIDIOUTCAPS
    // {
    //     public short wMid;
    //     public short wPid;
    //     public int vDriverVersion;	 //MMVERSION
    //     [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    //     public string szPname;
    //     public ushort wTechnology;
    //     public short wVoices;
    //     public short wNotes;
    //     public ushort wChannelMask;
    //     public uint dwSupport;
    // }

	// Leap Motion handle variables

	/// <summary>
	/// The last MIDI note pressed
	/// </summary>
	private static nint notePressed { get; set; } = 0;

	/// <summary>
	/// Number of frames since last Note On event (default 6 = passed)
	/// </summary>
	public static byte framesSinceNotePressed { get; set; } = 4;

	/// <summary>
	/// Number of frames to wait since last Note On event
	/// </summary>
	public static byte waitFrames { get; set; } = 0;

	/// <summary> MIDI API Callback Delegate </summary>
	private delegate void MidiInProc(IntPtr handle, int message, IntPtr instance, IntPtr param1, IntPtr param2);

	/// <summary> MIDI Input Callback Function </summary>
	private static void MidiInCallback(IntPtr handle, int message, IntPtr instance, IntPtr param1, IntPtr param2)
	{
		// Stop if current instance has stopped outputting in other thread
		if (!isReflecting) { return; }

		// Set Note Pressed state to true if event is Note On
		if ((param1 & 0xF0) == 0x90)
		{
			nint currentNote = param1 & 0x0FF;
			Console.Out.WriteLine($"notePressed {notePressed} | currentNote {currentNote} | difference {notePressed - currentNote}");

			waitFrames = 5; // Plan to make this depend on distance between two notes

			framesSinceNotePressed = 0;

			notePressed = currentNote;
			Console.Out.WriteLine($"waitFrames {waitFrames} | framesSinceNotePressed {framesSinceNotePressed} | notePressed {notePressed}");
		}

		// Reflect the MIDI event to the output device
		int result = midiOutShortMsg(outputHandle, (int)param1);
		Console.Out.WriteLine($"midiOut {inputHandle}---[{(int)param1}] -> {outputHandle}");
		if (result != MMSYSERR_NOERROR)
		{
			Console.Out.WriteLine("Failed to send MIDI event to output device. Error code: {0} | msg {1}", result, param1);
		}
	}

    /// <summary>
    /// Open a MIDI device input and reflect all data to output.
    /// </summary>
	public static void ReflectMidiInput()
	{
		OpenMidiInput();
		OpenMidiOutput();
		
		StartMidiInput();

		Console.Out.WriteLine("Receiving MIDI events. Change input/output to stop...");

		// Task.Run(() => 
		// {
		pauseReflectingEvent = new AutoResetEvent(false);
		isReflecting = true;

		Console.Out.WriteLine($"pauseReflectingEvent ${pauseReflectingEvent} | isReflecting {isReflecting}");

		pauseReflectingEvent.WaitOne();

		CloseMidiOutput();
		
		StopMidiInput();
		CloseMidiInput();
		// });
	}

	// I/O MIDI device handles
	public static IntPtr inputHandle;
	public static IntPtr outputHandle;

	/// <summary>
	/// Open MIDI input to the current selected input device.
	/// </summary>
	private static void OpenMidiInput()
	{
		int result = midiInOpen(out inputHandle, inputDeviceID, Marshal.GetFunctionPointerForDelegate<MidiInProc>(MidiInCallback), IntPtr.Zero, CALLBACK_FUNCTION);
		if (result != MMSYSERR_NOERROR)
		{
			Console.Error.WriteLine("Failed to open MIDI input. Error code: {0}", result);
		}
		else
		{
			Console.Out.WriteLine("Opened MIDI input.");
		}
	}

	/// <summary>
	/// Open MIDI output to the current selected output device.
	/// </summary>
	private static void OpenMidiOutput()
	{
		int result = midiOutOpen(out outputHandle, outputDeviceID, IntPtr.Zero, IntPtr.Zero, 0);
		if (result != MMSYSERR_NOERROR)
		{
			Console.Error.WriteLine("Failed to open MIDI output. Error code: {0}", result);
		}
		else
		{
			Console.Out.WriteLine("Opened MIDI output.");
		}
	}

	/// <summary>
	/// Start handling MIDI input of the current selected input device.
	/// </summary>
	private static void StartMidiInput()
	{
		int result = midiInStart(inputHandle);
		if (result != MMSYSERR_NOERROR)
		{
			Console.Error.WriteLine("Failed to start MIDI input. Error code: {0}", result);
		}
		else
		{
			Console.Out.WriteLine("Started MIDI input.");
		}
	}

	/// <summary>
	/// Close MIDI input of the current selected input device.
	/// </summary>
	private static void CloseMidiInput()
	{
		int result = midiInClose(inputHandle);
		if (result != MMSYSERR_NOERROR)
		{
			Console.Error.WriteLine("Failed to close MIDI input. Error code: {0}", result);
		}
		else
		{
			Console.Out.WriteLine("Closed MIDI input.");
		}
	}

	/// <summary>
	/// Close MIDI output of the current selected output device.
	/// </summary>
	private static void CloseMidiOutput()
	{
		int result = midiOutClose(outputHandle);
		if (result != MMSYSERR_NOERROR)
		{
			Console.Error.WriteLine("Failed to close MIDI output. Error code: {0}", result);
		}
		else
		{
			Console.Out.WriteLine("Closed MIDI output.");
		}
	}

	private static void StopMidiInput()
	{
		int result = midiInStop(inputHandle);
		if (result != MMSYSERR_NOERROR)
		{
			Console.Error.WriteLine("Failed to stop MIDI input. Error code: {0}", result);
		}
		else
		{
			Console.Out.WriteLine("Stopped MIDI input.");
		}
	}

	/// <summary>
    /// Wait event for reflecting MIDI input to output
    /// </summary>
    public static AutoResetEvent pauseReflectingEvent;
	/// <summary>
	/// If reflecting input to output in current instance of MidiInterop.
	/// </summary>
	/// <remarks> This is just a global state for now; the class is static. </remarks>
	public static bool isReflecting;

	/// <summary>
    /// ID/Index of current MIDI input device
    /// </summary>
	public static int inputDeviceID;

	/// <summary>
    /// ID/Index of current MIDI output device
    /// </summary>
	public static int outputDeviceID;
}