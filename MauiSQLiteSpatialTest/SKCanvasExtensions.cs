using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace MauiSQLiteSpatialTest;

/// <summary>
/// Utility extensions for SKCanvas.
/// </summary>
public static partial class SKCanvasExtensions
{
	/// <summary>
	/// Draws the sample data on the canvas.
	/// </summary>
	/// <param name="canvas">The SKCanvas to draw on.</param>
	/// <param name="sampleData">The sample data to draw.</param>
	/// <param name="w">The width of the canvas.</param>
	/// <param name="h">The height of the canvas.</param>
	public static void DrawSampleData(this SkiaSharp.SKCanvas canvas, IEnumerable<SampleData> sampleData, double w, double h)
	{
		foreach (var item in sampleData)
		{
			DrawSampleData(canvas, item, w, h);
		}
	}

	/// <summary>
	/// Draws the sample data on the canvas.
	/// </summary>
	/// <param name="canvas">The SKCanvas to draw on.</param>
	/// <param name="sampleData">The sample data to draw.</param>
	/// <param name="w">The width of the canvas.</param>
	/// <param name="h">The height of the canvas.</param>
	public static void DrawSampleData(this SkiaSharp.SKCanvas canvas, SampleData sampleData, double w, double h)
	{
		if (ST.GeomFromText(sampleData.Geometry) is NetTopologySuite.Geometries.Point point
			&& Color.TryParse(sampleData.Color, out Color color))
		{
			DrawSampleData(canvas, sampleData.City, color, point.X, point.Y, w, h);
		}
	}

	/// <summary>
	/// Draws the sample data on the canvas.
	/// </summary>
	/// <param name="canvas">The SKCanvas to draw on.</param>
	/// <param name="city">The name of the city.</param>
	/// <param name="color">The color to use for the city.</param>
	/// <param name="x">The x-coordinate of the city.</param>
	/// <param name="y">The y-coordinate of the city.</param>
	/// <param name="w">The width of the canvas.</param>
	/// <param name="h">The height of the canvas.</param>
	public static void DrawSampleData(this SkiaSharp.SKCanvas canvas, string city, Color color, double x, double y, double w, double h)
	{
		PointF viewPoint = new PointF(
			(float)(w * ((180 + x) / 360)),
			(float)(h * ((90 - y) / 180)));
		canvas.DrawCircle(viewPoint.X, viewPoint.Y, 5, new SKPaint { Color = SKColors.Black, IsAntialias = true });
		canvas.DrawText(city, viewPoint.X + 12, viewPoint.Y + 12, new SKFont { Size = 12 }, new SKPaint { Color = color.ToSKColor(), IsAntialias = true });
	}
}
