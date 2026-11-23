namespace Robbiblubber.Spellbook.Controls.Themes;

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Runtime.Versioning;



/// <summary>This class implements the type converter for themes.</summary>
/// <remarks>The designer offers the built-in themes and "(Application)" for the theme of the application. Built-in themes are
///          serialized as references to their static fields; custom themes can only be set at run time.</remarks>
[SupportedOSPlatform("windows")]
internal sealed class _ThemeConverter: TypeConverter
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private constants                                                                                                //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Display text for NULL, the theme of the application.</summary>
	private const string _APPLICATION_TEXT = "(Application)";



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// private static methods                                                                                           //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>Gets the static field of a built-in theme.</summary>
	/// <param name="theme">Theme.</param>
	/// <returns>Returns the field, or NULL if the theme is not a built-in theme.</returns>
	private static FieldInfo? _GetField(Theme theme)
	{
		return typeof(Theme).GetFields(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(m => (m.GetValue(null) == theme));
	}



	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// [override] TypeConverter                                                                                         //
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <inheritdoc/>
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
	{
		return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
	}


	/// <inheritdoc/>
	public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
	{
		return ((destinationType == typeof(InstanceDescriptor)) || (destinationType == typeof(string)) ||
		        base.CanConvertTo(context, destinationType));
	}


	/// <inheritdoc/>
	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
	{
		if(value is string text)
		{
			text = text.Trim();
			if((text.Length == 0) || (text == _APPLICATION_TEXT)) { return null; }

			Theme? rval = (ThemeManager.Themes.FirstOrDefault(m => string.Equals(m.Name, text, StringComparison.OrdinalIgnoreCase)) ??
			               ThemeManager.Find(text));
			if(rval is null) { throw new FormatException($"Unknown theme \"{text}\"."); }

			return rval;
		}

		return base.ConvertFrom(context, culture, value);
	}


	/// <inheritdoc/>
	public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
	{
		if(destinationType == typeof(string)) { return ((value as Theme)?.Name ?? _APPLICATION_TEXT); }
		if((destinationType == typeof(InstanceDescriptor)) && (value is Theme theme))
		{
			FieldInfo? field = _GetField(theme);
			if(field is null) { throw new NotSupportedException("Only built-in themes can be set in the designer."); }

			return new InstanceDescriptor(field, null);
		}

		return base.ConvertTo(context, culture, value, destinationType);
	}


	/// <inheritdoc/>
	public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
	{
		return true;
	}


	/// <inheritdoc/>
	public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context)
	{
		return true;
	}


	/// <inheritdoc/>
	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
	{
		List<Theme?> rval = new() { null };
		rval.AddRange(ThemeManager.BuiltInThemes);

		return new StandardValuesCollection(rval);
	}
}
