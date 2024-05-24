using System.Drawing;
using System.Drawing.Drawing2D;

namespace raptor;

public class Balloon
{
	public enum Corner
	{
		Upper_Left,
		Lower_Left,
		Upper_Right,
		Lower_Right
	}

	public static int radius = 10;

	public static GraphicsPath Make_Path(System.Drawing.Rectangle rec, Corner corner)
	{
		GraphicsPath graphicsPath = new GraphicsPath();
		if (rec.Width < 2 * radius)
		{
			rec.Width = 3 * radius;
		}
		if (rec.Height < 2 * radius)
		{
			rec.Height = 2 * radius;
		}
		graphicsPath.StartFigure();
		graphicsPath.AddLine(-rec.Width / 2, rec.Height / 2 - radius, -rec.Width / 2, -rec.Height / 2 + radius);
		graphicsPath.AddArc(-rec.Width / 2, -rec.Height / 2, 2 * radius, 2 * radius, 180f, 90f);
		switch (corner)
		{
		case Corner.Upper_Left:
			graphicsPath.AddLine(-rec.Width / 2 + radius, -rec.Height / 2, -rec.Width / 2 + radius, -rec.Height / 2 - radius);
			graphicsPath.AddLine(-rec.Width / 2 + radius, -rec.Height / 2 - radius, -rec.Width / 2 + 2 * radius, -rec.Height / 2);
			graphicsPath.AddLine(-rec.Width / 2 + 2 * radius, -rec.Height / 2, rec.Width / 2 - radius, -rec.Height / 2);
			break;
		case Corner.Upper_Right:
			graphicsPath.AddLine(-rec.Width / 2 + radius, -rec.Height / 2, rec.Width / 2 - 2 * radius, -rec.Height / 2);
			graphicsPath.AddLine(rec.Width / 2 - 2 * radius, -rec.Height / 2, rec.Width / 2 - radius, -rec.Height / 2 - radius);
			graphicsPath.AddLine(rec.Width / 2 - radius, -rec.Height / 2 - radius, rec.Width / 2 - radius, -rec.Height / 2);
			break;
		case Corner.Lower_Right:
			graphicsPath.AddLine(-rec.Width / 2 + radius, -rec.Height / 2, rec.Width / 2 - radius, -rec.Height / 2);
			break;
		}
		graphicsPath.AddArc(rec.Width / 2 - 2 * radius, -rec.Height / 2, 2 * radius, 2 * radius, 270f, 90f);
		graphicsPath.AddLine(rec.Width / 2, -rec.Height / 2 + radius, rec.Width / 2, rec.Height / 2 - radius);
		graphicsPath.AddArc(rec.Width / 2 - 2 * radius, rec.Height / 2 - 2 * radius, 2 * radius, 2 * radius, 0f, 90f);
		switch (corner)
		{
		case Corner.Upper_Right:
			graphicsPath.AddLine(-rec.Width / 2 + radius, rec.Height / 2, rec.Width / 2 - radius, rec.Height / 2);
			break;
		case Corner.Lower_Left:
			graphicsPath.AddLine(rec.Width / 2 - radius, rec.Height / 2, -rec.Width / 2 + 2 * radius, rec.Height / 2);
			graphicsPath.AddLine(-rec.Width / 2 + 2 * radius, rec.Height / 2, -rec.Width / 2 + radius, rec.Height / 2 + radius);
			graphicsPath.AddLine(-rec.Width / 2 + radius, rec.Height / 2 + radius, -rec.Width / 2 + radius, rec.Height / 2);
			break;
		case Corner.Lower_Right:
			graphicsPath.AddLine(rec.Width / 2 - radius, rec.Height / 2, rec.Width / 2 - radius, rec.Height / 2 + radius);
			graphicsPath.AddLine(rec.Width / 2 - radius, rec.Height / 2 + radius, rec.Width / 2 - 2 * radius, rec.Height / 2);
			graphicsPath.AddLine(rec.Width / 2 - 2 * radius, rec.Height / 2, -rec.Width / 2 + radius, rec.Height / 2);
			break;
		}
		graphicsPath.AddArc(-rec.Width / 2, rec.Height / 2 - 2 * radius, 2 * radius, 2 * radius, 90f, 90f);
		graphicsPath.CloseFigure();
		Matrix matrix = new Matrix();
		matrix.Translate(rec.Left + rec.Width / 2, rec.Top + rec.Height / 2);
		graphicsPath.Transform(matrix);
		return graphicsPath;
	}
}
