namespace Robbiblubber.Spellbook.Controls.Samples.MultiForm;

using System.Runtime.Versioning;



/// <summary>This class provides the application entry point.</summary>
[SupportedOSPlatform("windows")]
public static class Program
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static properties                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Gets the application context.</summary>
	public static MultiFormApplicationContext Context
	{
		get; private set;
	} = null!;



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// public static methods                                                                                            //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Runs the application.</summary>
	/// <param name="args">Command line arguments: files to open.</param>
	[STAThread]
	public static void Main(string[] args)
	{
		using SingleInstance instance = new("robbiblubber.org/Spellbook.Samples.MultiForm");
		if(!instance.IsFirstInstance)
		{
			if(MultiFormApplicationContext.Forward(instance, args)) { return; }
		}

		ApplicationConfiguration.Initialize();

		Context = new MultiFormApplicationContext(instance);
		Context.ArgumentsReceived += _Context_ArgumentsReceived;

		string info = instance.IsFirstInstance ? "First instance" : "The first instance did not answer, running on its own";
		Open(args, Environment.CurrentDirectory, info);

		Application.Run(Context);
		Context.Dispose();
	}


	/// <summary>Opens a window for each file, or an empty window if there are no files.</summary>
	/// <param name="files">Files.</param>
	/// <param name="workingDirectory">Directory relative paths refer to.</param>
	/// <param name="info">Text shown in the status bar of the new windows.</param>
	public static void Open(string[] files, string workingDirectory, string info)
	{
		if(files.Length == 0)
		{
			Context.ShowForm(new NoteWindow(null, info));
			return;
		}

		SingleInstanceEventArgs e = new(files, workingDirectory);
		foreach(string i in files)
		{
			string path = e.GetFullPath(i);
			NoteWindow? open = Context.Forms.OfType<NoteWindow>().FirstOrDefault(m => string.Equals(m.FileName, path, StringComparison.OrdinalIgnoreCase));

			if(open != null)
			{
				open.ShowInfo(info);
				MultiFormApplicationContext.ActivateForm(open);
			}
			else { Context.ShowForm(new NoteWindow(path, info)); }
		}
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// event handlers                                                                                                   //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>Arguments of a further start received.</summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">Event arguments.</param>
	private static void _Context_ArgumentsReceived(object? sender, SingleInstanceEventArgs e)
	{
		Open(e.Arguments, e.WorkingDirectory, "Received from a further start in " + e.WorkingDirectory);
	}
}
