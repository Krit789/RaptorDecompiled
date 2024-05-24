using System.Windows.Forms;

namespace raptor;

public class Buffered : Panel
{
	public Buffered()
	{
		SetStyle(ControlStyles.DoubleBuffer, value: true);
		SetStyle(ControlStyles.AllPaintingInWmPaint, value: true);
		SetStyle(ControlStyles.UserPaint, value: true);
	}
}
