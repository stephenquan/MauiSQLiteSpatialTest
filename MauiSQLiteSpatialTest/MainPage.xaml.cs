using CsvHelper;
using SkiaSharp;
using SQLite;

namespace MauiSQLiteSpatialTest;

#pragma warning disable CA1001 // Suppress IDisposable warning for SQLiteConnection

/// <summary>
/// Provides a collection of sample data representing U.S. states.
/// </summary>
public class UsaStates : SampleData;

/// <summary>
/// Provides a collection of sample data representing U.S. cities in California.
/// </summary>
public class UsaCities : SampleData;

/// <summary>
/// Sample MainPage demonstrating spatial data handling with SQLite and SkiaSharp.
/// </summary>
public partial class MainPage : ContentPage
{
	SQLiteConnection db = new(":memory:");
	MapToViewTransform transform = new();
	NetTopologySuite.Geometries.Geometry? selection;
	bool loaded = false;

	/// <summary>
	/// Initializes a new instance of the <see cref="MainPage"/> class.
	/// </summary>
	public MainPage()
	{
		InitializeComponent();
		Dispatcher.Dispatch(async () => await PostInitialize());
	}

	async Task PostInitialize()
	{
		// Spatially enable the SQLite database.
		db.UseSpatialExtensions();

		// Load sample spatial data from CSV files into the in-memory databases
		await LoadFromCsv<UsaStates>(db, "usa_states.csv"); // all US states
		await LoadFromCsv<UsaCities>(db, "usa_cities.csv"); // some US cities

		// Do some test spatial queries
		double area_50_units = db.ExecuteScalar<double>("SELECT ST_Area(ST_GeomFromText('POLYGON((10 10,20 10,20 20,10 10))'))");
		var centroid_at_50_50 = db.ExecuteScalar<string>("SELECT ST_AsText(ST_Centroid(ST_GeomFromText('POLYGON((10 10,20 10,20 20,10 10))')))");
		var circle_buffer = db.ExecuteScalar<string>("SELECT ST_AsText(ST_Buffer(ST_GeomFromText('POINT(10 10)'), 5))");

		// Create a selection 1.0 degree (111 km) buffer around Los Angeles, CA.
		selection = ST.Buffer(ST.Point(-118.2437, 34.0522), 1.0);

		// Finalize loading.
		loaded = true;
		canvasView.InvalidateSurface();
	}

	async Task LoadFromCsv<T>(SQLiteConnection db, string fileName)
	{
		using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
		using var reader = new StreamReader(stream);
		using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
		db.RunInTransaction(() =>
		{
			db.CreateTable<T>();
			foreach (var record in csv.GetRecords<T>())
			{
				db.Insert(record);
			}
		});
	}

	/// <summary>
	/// Renders the spatial data onto the SkiaSharp canvas.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	void canvasView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
	{
		if (!loaded)
		{
			return;
		}

		if (transform.FollowMapExtent)
		{
			transform.UpdateScale(canvasView.Width, canvasView.Height);
		}
		transform.UpdateCenter(canvasView.Width, canvasView.Height);

		SKSurface surface = e.Surface;
		SKCanvas canvas = surface.Canvas;
		canvas.Clear();
		canvas.DrawSampleData(db.Query<SampleData>("SELECT * FROM UsaStates"), transform);
		//canvas.DrawSampleData(db.Query<SampleData>("SELECT '' as Name, 'Orange' as Color, ST_Buffer(Geometry, 1) as Geometry FROM UsaCities WHERE Name = 'Los Angeles,CA'"), transform);
		if (selection is not null)
		{
			canvas.DrawGeometry("", Colors.Orange, selection, transform);
		}
		canvas.DrawSampleData(db.Query<SampleData>("SELECT * FROM UsaCities"), transform);
	}

	void OnReset(object sender, EventArgs e)
	{
		transform.MapExtent = Rect.FromLTRB(-125, 24.3, -66.9, 49.4); // USA extent
		transform.FollowMapExtent = true;
		canvasView.InvalidateSurface();
	}

	void OnZoomIn(object sender, EventArgs e)
	{
		transform.ScaleBy(2);
		canvasView.InvalidateSurface();
	}

	void OnZoomOut(object sender, EventArgs e)
	{
		transform.ScaleBy(0.5);
		canvasView.InvalidateSurface();
	}

	void OnMapPressed(object sender, PointerEventArgs e)
	{
		if (e.GetPosition(canvasView) is Point pt)
		{
			PointF mapPoint = transform.UnProject(pt.X, pt.Y);
			transform.PanTo(mapPoint);
			transform.ScaleBy(2.0);
			//selection = ST.Buffer(ST.Point(mapPoint.X, mapPoint.Y), 1.0);
			canvasView.InvalidateSurface();
		}
	}
}
