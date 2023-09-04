using System.Runtime.InteropServices;

namespace LeapExpression;

static class LeapMotionHandle
{
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
    public static Leap.Vector lastLocation;

    public static int SendMotionCC(float x, float y, float z) {
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
				(int)(8192 + 49152 * (lastLocation.x - x))
			, 14335)
		, 2048);

		Console.Out.WriteLine($"pitch bend value: {pitchBendValue} | lastLocation {lastLocation.x} | x {x} | calc {(int)(8192 + 16383 * (lastLocation.x - x))}");

		// IntPtr outputHandle;
		// int result = midiOutOpen(out outputHandle, MidiInterop.currentOutputDeviceID, IntPtr.Zero, IntPtr.Zero, 0);

		// // Console.Out.WriteLine($"current output device: {currentOutputDeviceID}");

		// if (result == MMSYSERR_NOERROR)
		// {
		// 	// Find Vector offset from lastLocation
		// 	int pitchBendValue = (int)(8192 + 8192 * (lastLocation.x - x));

		// 	// Console.Out.WriteLine($"pitch bend value: {pitchBendValue}");

		// 	// Calculate the LSB and MSB values
		// 	int lsb = pitchBendValue & 0x7F;
		// 	int msb = (pitchBendValue >> 7) & 0x7F;

		// 	// Console.Out.WriteLine($"lsb {lsb} | msb {msb}");

		// 	// Construct the MIDI message
		// 	int message = (MIDI_STATUS_PITCH_BEND << 4) | 1;
		// 	message |= lsb << 8;
		// 	message |= msb << 16;

		// 	// Console.Out.WriteLine($"result: {result} | outputHandle: {outputHandle} | message: {message}");
		// 	result = midiOutShortMsg(outputHandle, message);
		// 	// Console.Out.WriteLine($"new result: {result}");
		// 	if (result != MMSYSERR_NOERROR)
		// 	{
		// 		Console.Out.WriteLine("Failed to send MIDI event to output device. Error code: {0}", result);
		// 	}

		// 	result = midiOutClose(outputHandle);
		// 	if (result != MMSYSERR_NOERROR)
		// 	{
		// 		Console.Out.WriteLine("Failed to close output device. Error code: {0}", result);
		// 	}

		// 	return message;
		// }
		// else
		// {
		// 	Console.Out.WriteLine("Failed to open output device. Error code: {0}", result);
		// }
		return pitchBendValue;
    }

    // static Leap.Vector ResetLocation() {
    //     return new Leap.Vector();
    // }
}
