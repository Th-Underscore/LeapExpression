using System.Runtime.InteropServices;

namespace LeapExpression;

public class LeapMotionHandle
{
	public LeapMotionHandle() {

	}

    // MIDI API Constants
	private const int MMSYSERR_NOERROR = 0;
	private const int MIDI_STATUS_PITCH_BEND = 0xE0;

    // MIDI API Functions
    [DllImport("winmm.dll")]
	private static extern int midiOutOpen(out IntPtr handle, int deviceID, IntPtr callback, IntPtr instance, int flags);

	[DllImport("winmm.dll")]
	private static extern int midiOutClose(IntPtr handle);

	[DllImport("winmm.dll")]
	private static extern int midiOutShortMsg(IntPtr handle, int message);

	// MIDI API Function Delegate
	private delegate void MidiOutProc(IntPtr handle, int message, IntPtr instance, IntPtr param1, IntPtr param2);


    /// <summary>
    /// The last location since the last ResetLocation event.
    /// </summary>
    /// <remarks> This is best used for MIDI Note On events to use as a control location for CC values. </remarks>
    public class LastLocation
	{
		public float x { get; set; }
		public float y { get; set; }
		public float z { get; set; }
	};

    public int SendMotionCC(float x, float y, float z)
	{
/*
		Pre-compilation limit calculation:
		amplification: 16384 * 4 - 1 = 65535
		Min = 0 + 2048 = 2048
		Max = 16383 - 2048 = 14335
*/
		int pitchBendValue = Math.Max
		(
			Math.Min
			(
				(int)(8192 + 49152 * (Program.lastLocation.x - x))
			, 14335)
		, 2048);

		// Create the MIDI message bytes
		byte statusByte = 0xE0; // Pitch Bend status byte
		byte lsbByte = (byte)(pitchBendValue & 0x7F); // Least Significant 7 bits
		byte msbByte = (byte)((pitchBendValue >> 7) & 0x7F); // Most Significant 7 bits

		int message = (statusByte | (msbByte << 16) | (lsbByte << 8));

		//Console.Out.WriteLine($"pitch bend value {pitchBendValue}  |  lsb {lsbByte}  |  msb {msbByte}  |  msg {message}");

		// Send the MIDI message
		int result = midiOutShortMsg(MidiInterop.outputHandle, message);

		if (result != MMSYSERR_NOERROR)
		{
			Console.Out.WriteLine($"Failed to send MIDI event to output device. Error code: {result} | msg {message} | handle {MidiInterop.outputHandle}");
		}

		return pitchBendValue;
    }
}
