using System;

namespace raptor;

[Serializable]
public class Clipboard_Data
{
	public enum kinds
	{
		symbols,
		comment
	}

	public kinds kind;

	public Component symbols;

	public CommentBox cb;

	public Guid guid;

	public logging_info log;

	public Clipboard_Data(Component c, Guid g, logging_info l)
	{
		kind = kinds.symbols;
		symbols = c;
		cb = null;
		guid = g;
		log = l;
	}

	public Clipboard_Data(CommentBox b, Guid g)
	{
		kind = kinds.comment;
		symbols = null;
		cb = b;
		guid = g;
		log = null;
	}
}
