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
		var centroid = db.ExecuteScalar<string>("SELECT ST_AsText(ST_Centroid(ST_GeomFromText('POLYGON((10 10,20 10,20 20,10 10))')))"); // POINT(15 15)
		db.CreateTable<SampleData>();
		db.Insert(new SampleData { City = "Sydney", Color = "Red", Geometry = "POINT(151.2093 -33.8688)" });
		db.Insert(new SampleData { City = "New York", Color = "Green", Geometry = "POINT(-74.0060 40.7128)" });
		db.Insert(new SampleData { City = "Paris", Color = "Orange", Geometry = "POINT(2.3522 48.8566)" });
		db.Insert(new SampleData { City = "Tokyo", Color = "Red", Geometry = "POINT(139.6917 35.6895)" });
		db.Insert(new SampleData { City = "Cairo", Color = "Orange", Geometry = "POINT(31.2357 30.0444)" });
		db.Insert(new SampleData { City = "Rio de Janeiro", Color = "Blue", Geometry = "POINT(-43.1729 -22.9068)" });
		db.Insert(new SampleData { City = "Moscow", Color = "Green", Geometry = "POINT(37.6173 55.7558)" });
		db.Insert(new SampleData { City = "London", Color = "Blue", Geometry = "POINT(-0.1276 51.5074)" });
		db.Insert(new SampleData { City = "Beijing", Color = "Red", Geometry = "POINT(116.4074 39.9042)" });
		db.Insert(new SampleData { City = "Delhi", Color = "Purple", Geometry = "POINT(77.1025 28.7041)" });
		db.Insert(new SampleData { City = "San Francisco", Color = "Green", Geometry = "POINT(-122.4194 37.7749)" });
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
		canvas.DrawSampleData(db.Query<SampleData>("SELECT * FROM SampleData"), canvasView.Width, canvasView.Height);
	}
}
