using SkiaSharp;
using SQLite;

namespace MauiSQLiteSpatialTest;

#pragma warning disable CA1001 // Suppress IDisposable warning for SQLiteConnection

/// <summary>
/// Sample MainPage demonstrating spatial data handling with SQLite and SkiaSharp.
/// </summary>
public partial class MainPage : ContentPage
{
	SQLiteConnection db;

	/// <summary>
	/// Initializes a new instance of the <see cref="MainPage"/> class.
	/// </summary>
	public MainPage()
	{
		// Create an in-memory SQLite database and populate it with sample spatial data
		db = new SQLiteConnection(":memory:");
		db.UseSpatialExtensions();
		var str = db.ExecuteScalar<string>("SELECT ST_GeomFromText('POLYGON((10 10,20 10,20 20,10 10))')");
		var area = db.ExecuteScalar<double>("SELECT ST_Area(ST_GeomFromText('POLYGON((10 10,20 10,20 20,10 10))'))"); // 50
		db.CreateTable<SampleData>();
		db.Insert(new SampleData { City = "Sydney", Geometry = "POINT(151.2093 -33.8688)" });
		db.Insert(new SampleData { City = "New York", Geometry = "POINT(-74.0060 40.7128)" });
		db.Insert(new SampleData { City = "Paris", Geometry = "POINT(2.3522 48.8566)" });
		db.Insert(new SampleData { City = "Tokyo", Geometry = "POINT(139.6917 35.6895)" });
		db.Insert(new SampleData { City = "Cairo", Geometry = "POINT(31.2357 30.0444)" });
		db.Insert(new SampleData { City = "Rio de Janeiro", Geometry = "POINT(-43.1729 -22.9068)" });
		db.Insert(new SampleData { City = "Moscow", Geometry = "POINT(37.6173 55.7558)" });
		db.Insert(new SampleData { City = "London", Geometry = "POINT(-0.1276 51.5074)" });
		db.Insert(new SampleData { City = "Beijing", Geometry = "POINT(116.4074 39.9042)" });
		db.Insert(new SampleData { City = "Delhi", Geometry = "POINT(77.1025 28.7041)" });
		db.Insert(new SampleData { City = "San Francisco", Geometry = "POINT(-122.4194 37.7749)" });
		InitializeComponent();
	}

	/// <summary>
	/// Renders the spatial data onto the SkiaSharp canvas.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	void canvasView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
	{
		SKSurface surface = e.Surface;
		SKCanvas canvas = surface.Canvas;
		canvas.Clear();
		db.Query<SampleData>("SELECT * FROM SampleData")
			.ForEach(item =>
		{
			if (ST.GeomFromText(item.Geometry) is NetTopologySuite.Geometries.Point pt)
			{
				PointF viewPoint = new PointF(
					(float)(canvasView.Width * ((180 + pt.X) / 360)),
					(float)(canvasView.Height * ((90 - pt.Y) / 180)));
				canvas.DrawCircle(viewPoint.X, viewPoint.Y, 5, new SKPaint { Color = SKColors.Black, IsAntialias = true });
				canvas.DrawText(item.City, viewPoint.X + 12, viewPoint.Y + 12, new SKFont { Size = 12 }, new SKPaint { Color = SKColors.Black, IsAntialias = true });
			}
		});
	}
}
