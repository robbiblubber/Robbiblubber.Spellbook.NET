namespace Robbiblubber.Spellbook.Controls.Samples.EditCommands;

using System.Runtime.Versioning;



/// <summary>This class implements a text pad that provides its edit commands through <see cref="IEditableBearing"/>.</summary>
/// <remarks><para>The pad does not implement <see cref="IEditable"/> itself; it returns an object that does. That object pastes the
///          clipboard text in upper case and passes all other commands to the registered adapter of the inner text box.</para>
///          <para>If <see cref="UpperCasePaste"/> is <c>false</c>, <see cref="Editable"/> returns NULL and the
///          <see cref="EditCommandProvider"/> uses the adapter of the inner text box directly.</para></remarks>
[SupportedOSPlatform("windows")]
public partial class UpperCasePadControl: UserControl, IEditableBearing
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private members                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Editable implementation.</summary>
	private readonly _UpperCaseEditable _Editable;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// constructors                                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Creates a new instance of this class.</summary>
	public UpperCasePadControl()
	{
		InitializeComponent();
		_Editable = new _UpperCaseEditable(_TextPad);
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public properties                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets or sets if text is pasted in upper case.</summary>
	[System.ComponentModel.DefaultValue(true)]
	public bool UpperCasePaste
	{
		get; set;
	} = true;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Handles the TextChanged event of the text box.</summary>
	/// <param name="sender">Sending object.</param>
	/// <param name="e">Event arguments.</param>
	private void _TextPad_TextChanged(object? sender, EventArgs e)
	{
		_Editable?.RaiseEditStateChanged();
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [interface] IEditableBearing                                                                                     //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	[System.ComponentModel.Browsable(false)]
	public IEditable? Editable
	{
		get { return (UpperCasePaste ? _Editable : null); }
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private classes                                                                                                  //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>This class implements the edit commands of the pad.</summary>
	private sealed class _UpperCaseEditable: IEditable
	{
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private members                                                                                              //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Text box.</summary>
		private readonly TextBox _Text;



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// constructors                                                                                                 //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Creates a new instance of this class.</summary>
		/// <param name="text">Text box.</param>
		public _UpperCaseEditable(TextBox text)
		{
			_Text = text;
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// private properties                                                                                           //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Gets the adapter registered for the text box.</summary>
		private IEditAdapter? _Adapter
		{
			get { return EditAdapters.Find(_Text); }
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// public methods                                                                                               //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>Raises the <see cref="EditStateChanged"/> event.</summary>
		public void RaiseEditStateChanged()
		{
			EditStateChanged?.Invoke(this, EventArgs.Empty);
		}



		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// [interface] IEditable                                                                                        //
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <inheritdoc/>
		public event EventHandler? EditStateChanged;


		/// <inheritdoc/>
		public bool CanExecute(EditCommand command)
		{
			return (_Adapter?.CanExecute(_Text, command) ?? false);
		}


		/// <inheritdoc/>
		public void Execute(EditCommand command)
		{
			if(!CanExecute(command)) { return; }

			if(command == EditCommand.PASTE)
			{
				_Text.Paste(Clipboard.GetText().ToUpperInvariant());
			}
			else { _Adapter?.Execute(_Text, command); }
		}
	}
}
