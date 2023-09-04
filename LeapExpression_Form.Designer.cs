using M;

namespace LeapExpression;

partial class MainWindow
{
	/// <summary>
	///  Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	private System.Windows.Forms.TextBox tbValue;
	
	private System.Windows.Forms.TextBox velX;
	private System.Windows.Forms.TextBox velY;
	private System.Windows.Forms.TextBox velZ;

	private System.Windows.Forms.ComboBox MidiInputComboBox;
	private System.Windows.Forms.ComboBox MidiOutputComboBox;


	/// <summary>
	///  Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	///  Required method for Designer support - do not modify
	///  the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		components = new System.ComponentModel.Container();
		AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		ClientSize = new System.Drawing.Size(800, 450);
		Text = "Mark's LE";

		tbValue = new System.Windows.Forms.TextBox();
		tbValue.Location = new System.Drawing.Point(50, 50);
		tbValue.Size = new System.Drawing.Size(100, 20);
		Controls.Add(tbValue);

		velX = new System.Windows.Forms.TextBox();
		velY = new System.Windows.Forms.TextBox();
		velZ = new System.Windows.Forms.TextBox();

		MidiInputComboBox = new System.Windows.Forms.ComboBox();
		MidiInputComboBox.Location = new System.Drawing.Point(200, 50);
		MidiInputComboBox.Name = "Midi Inputs";
		MidiInputComboBox.Size = new System.Drawing.Size(150, 21);
		MidiInputComboBox.TabIndex = 0;
		MidiInputComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

		MidiOutputComboBox = new System.Windows.Forms.ComboBox();
		MidiOutputComboBox.Location = new System.Drawing.Point(350, 50);
		MidiOutputComboBox.Name = "Midi Outputs";
		MidiOutputComboBox.Size = new System.Drawing.Size(150, 21);
		MidiOutputComboBox.TabIndex = 0;
		MidiOutputComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

		this.Controls.Add(MidiInputComboBox);
		this.Controls.Add(MidiOutputComboBox);
		}

	#endregion
}
