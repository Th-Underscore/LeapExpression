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
	/// NotePressed state for Leap Motion tracking
	/// </summary>
	public static bool NotePressed;

	// MIDI handle variables
	static MidiPassThrough midiPassThrough;

	// MIDI API Callback Delegate
	private delegate void MidiInProc(IntPtr handle, int message, IntPtr instance, IntPtr param1, IntPtr param2);

	// MIDI Input Callback Function
	private static void MidiInCallback(IntPtr handle, int message, IntPtr instance, IntPtr param1, IntPtr param2)
	{
		// Handle MIDI input events here
		Console.Out.WriteLine($"Received MIDI event: 0x{{0:X8}} | handle {handle} | message {message} | instance {instance} | param1 {param1} & {param1 & 0xF0} == 0x90 - {(param1 & 0xF0) == 0x90} | param2 {param2}", message);

		// Set Note Pressed state to true if event is Note On
		NotePressed = ((param1 & 0xF0) == 0x90);
		Console.Out.WriteLine($"is Note On: {NotePressed}");

		// Reflect the MIDI event to the output device
		Console.Out.WriteLine($"current output device: {currentOutputDeviceID}");

		Console.Out.WriteLine($"outputHandle: {outputHandle} | message: {message}");
		
		int result = midiOutShortMsg(outputHandle, (int)param1);
		Console.Out.WriteLine($"new result: {result}");
		if (result != MMSYSERR_NOERROR)
		{
			Console.Out.WriteLine("Failed to send MIDI event to output device. Error code: {0}", result);
		}
	}

    /// <summary>
    /// Open a MIDI device input and reflect all data to output.
    /// </summary>
	public static void ReflectMidiInput()
	{
		midiPassThrough = new MidiPassThrough();        
		
		Console.Out.WriteLine($"ReflectMidiInput() current reflected input device: {currentInputDeviceID}");

		Console.Out.WriteLine($"is MMSYSERR_NOERROR (0) | deviceID {(int)inputHandle}");

		midiPassThrough.StartMidiInput();

		Console.Out.WriteLine("Receiving MIDI events. Change input/output to stop...");

		// Task.Run(() => 
		// {
		pauseReflectingEvent = new AutoResetEvent(false);
		isReflecting = true;
		pauseReflectingEvent.WaitOne();

		midiPassThrough.CloseMidiOutput();
		
		midiPassThrough.StopMidiInput();
		midiPassThrough.CloseMidiInput();
		// });
	}

	public static IntPtr inputHandle;
	public static IntPtr outputHandle;

	public class MidiPassThrough {
		public MidiPassThrough()
		{
			OpenMidiInput();
			OpenMidiOutput();
		}

		/// <summary>
		/// Open MIDI input to the current selected input device.
		/// </summary>
		public void OpenMidiInput()
		{
			int result = midiInOpen(out inputHandle, currentInputDeviceID, Marshal.GetFunctionPointerForDelegate<MidiInProc>(MidiInCallback), IntPtr.Zero, CALLBACK_FUNCTION);
			if (result != MMSYSERR_NOERROR)
			{
				Console.Error.WriteLine("Failed to open MIDI input. Error code: {0}");
			}
		}

		/// <summary>
		/// Open MIDI output to the current selected output device.
		/// </summary>
		public void OpenMidiOutput()
		{
			int result = midiOutOpen(out outputHandle, currentOutputDeviceID, IntPtr.Zero, IntPtr.Zero, 0);
			if (result != MMSYSERR_NOERROR)
			{
				Console.Error.WriteLine("Failed to open MIDI output. Error code: {0}");
			}
		}

		/// <summary>
		/// Start handling MIDI input of the current selected input device.
		/// </summary>
		public void StartMidiInput()
		{
			int result = midiInStart(inputHandle);
			if (result != MMSYSERR_NOERROR)
			{
				Console.Error.WriteLine("Failed to start MIDI input. Error code: {0}");
			}
		}

		/// <summary>
		/// Close MIDI input of the current selected input device.
		/// </summary>
		public void CloseMidiInput()
		{
			int result = midiInClose(inputHandle);
			if (result != MMSYSERR_NOERROR)
			{
				Console.Error.WriteLine("Failed to close MIDI input. Error code: {0}");
			}
		}

		/// <summary>
		/// Close MIDI output of the current selected output device.
		/// </summary>
		public void CloseMidiOutput()
		{
			int result = midiOutClose(outputHandle);
			if (result != MMSYSERR_NOERROR)
			{
				Console.Error.WriteLine("Failed to close MIDI output. Error code: {0}");
			}
		}

		public void StopMidiInput()
		{
			int result = midiInStop(inputHandle);
			if (result != MMSYSERR_NOERROR)
			{
				Console.Error.WriteLine("Failed to stop MIDI input. Error code: {0}");
			}
		}
	}

	/// <summary>
    /// Wait event for reflecting MIDI input to output
    /// </summary>
    public static AutoResetEvent pauseReflectingEvent;
	public static bool isReflecting;

	/// <summary>
    /// ID/Index of current MIDI input device
    /// </summary>
	public static int currentInputDeviceID;

	/// <summary>
    /// ID/Index of current MIDI output device
    /// </summary>
	public static int currentOutputDeviceID;
}