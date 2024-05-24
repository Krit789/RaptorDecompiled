using System.Drawing;
using System.Drawing.Drawing2D;

namespace raptor;

public class StopSign
{
	public static GraphicsPath Make_Path(int x, int y, int size)
	{
		GraphicsPath graphicsPath = new GraphicsPath();
		graphicsPath.StartFigure();
		graphicsPath.AddLine(x, y + size / 3, x + size / 3, y);
		graphicsPath.AddLine(x + size / 3, y, x + 2 * size / 3, y);
		graphicsPath.AddLine(x + 2 * size / 3, y, x + size, y + size / 3);
		graphicsPath.AddLine(x + size, y + size / 3, x + size, y + 2 * size / 3);
		graphicsPath.AddLine(x + size, y + 2 * size / 3, x + 2 * size / 3, y + size);
		graphicsPath.AddLine(x + 2 * size / 3, y + size, x + size / 3, y + size);
		graphicsPath.AddLine(x + size / 3, y + size, x, y + 2 * size / 3);
		graphicsPath.AddLine(x, y + 2 * size / 3, x, y + size / 3);
		return graphicsPath;
	}

	public static void Draw(Graphics gr, int x, int y, int size)
	{
		GraphicsPath path = Make_Path(x, y, size);
		gr.FillPath(PensBrushes.redbrush, path);
		gr.DrawPath(PensBrushes.black_pen, path);
	}
}
