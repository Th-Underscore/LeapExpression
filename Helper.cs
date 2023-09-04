using M;
using System.Linq;

namespace LeapExpression;

class FormHelper
{
	/// <summary>
	/// Convert MidiDevice IList to Dictionary<Name, Device>.
	/// </summary>
	public Dictionary<String, MidiOutputDevice> GetMidiDevices(IList<MidiOutputDevice> devices) {
		var dictionary = devices.ToDictionary(x => x.Name, x => x);
		var key = dictionary.Keys.ElementAt(0);
		var value = dictionary[key];
		Console.Out.WriteLine($"FirstValue (output): {value.Name}");
		return dictionary;
	}
	/// <summary>
	/// Convert MidiDevice IList to Dictionary<Name, Device>.
	/// </summary>
	public Dictionary<String, MidiInputDevice> GetMidiDevices(IList<MidiInputDevice> devices) {
		var dictionary = devices.ToDictionary(x => x.Name, x => x);
		var key = dictionary.Keys.ElementAt(0);
		var value = dictionary[key];
		Console.Out.WriteLine($"FirstValue (input): {value.Name}");
		return dictionary;
	}
}