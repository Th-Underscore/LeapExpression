using M;
using System.Linq;

namespace LeapExpression;

class FormHelper
{
	/// <summary>
	/// Convert MidiDevice IList to Dictionary<Name, Device>.
	/// </summary>
	public Dictionary<String, MidiOutputDevice> GetMidiDevices(IList<MidiOutputDevice> devices) => devices.ToDictionary(x => x.Name, x => x);
	/// <summary>
	/// Convert MidiDevice IList to Dictionary<Name, Device>.
	/// </summary>
	public Dictionary<String, MidiInputDevice> GetMidiDevices(IList<MidiInputDevice> devices) => devices.ToDictionary(x => x.Name, x => x);
}