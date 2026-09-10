using System;
using System.Drawing;
using System.Windows.Forms;

namespace Caddx_PCTool;

public static class RichTextBoxExtension
{
	public static void AppendTextColorful(this RichTextBox rtbox, string text, Color color, bool addNewline = true)
	{
		if (addNewline)
		{
			text += Environment.NewLine;
		}
		((TextBoxBase)rtbox).SelectionStart = ((TextBoxBase)rtbox).TextLength;
		((TextBoxBase)rtbox).SelectionLength = 0;
		rtbox.SelectionColor = color;
		((TextBoxBase)rtbox).AppendText(text);
		rtbox.SelectionColor = ((Control)rtbox).ForeColor;
	}

	public static void AppendTextColorful(this RichTextBox rtbox, Color color)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		Font selectionFont = new Font("Verdana", 13f, (FontStyle)2, (GraphicsUnit)3);
		((Control)rtbox).Focus();
		int selectionStart = ((TextBoxBase)rtbox).SelectionStart;
		int length = ((TextBoxBase)rtbox).SelectedText.Length;
		((TextBoxBase)rtbox).Select(selectionStart, length);
		rtbox.SelectionFont = selectionFont;
		rtbox.SelectionColor = color;
		rtbox.SelectionBackColor = Color.Yellow;
		((TextBoxBase)rtbox).Select(0, 0);
	}

	public static void AutoSignColorAppendText(this RichTextBox rtbox, Color color, string info)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		Font selectionFont = new Font("Verdana", 13f, (FontStyle)1, (GraphicsUnit)3);
		((Control)rtbox).Focus();
		rtbox.SelectionColor = color;
		rtbox.SelectionFont = selectionFont;
		rtbox.SelectionColor = color;
		rtbox.SelectionBackColor = Color.Yellow;
		((TextBoxBase)rtbox).AppendText(info + "\n");
		((TextBoxBase)rtbox).Select(0, 0);
	}
}
