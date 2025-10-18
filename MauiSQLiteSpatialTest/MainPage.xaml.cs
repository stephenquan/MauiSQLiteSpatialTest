using CsvHelper;
using SkiaSharp;
using SQLite;
using SQuan.Helpers.SQLiteSpatial;

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
	SQLiteSpatialConnection db = new(":memory:");
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
		// Load sample spatial data from CSV files into the in-memory databases
		await LoadFromCsv<UsaStates>(db, "usa_states.csv"); // all US states
		await LoadFromCsv<UsaCities>(db, "usa_cities.csv"); // some US cities

		// Do some test spatial queries
		double? area_50_units = db.ExecuteScalar<double?>("SELECT ST_Area('POLYGON((10 10,20 10,20 20,10 10))')");
		string? centroid_at_16_13 = db.ExecuteScalar<string?>("SELECT ST_Centroid('POLYGON((10 10,20 10,20 20,10 10))')");
		string? circle_buffer = db.ExecuteScalar<string?>("SELECT ST_Buffer('POINT(10 10)', 5)");
		double? distance_5_units = db.ExecuteScalar<double?>("SELECT ST_Distance('POINT(0 0)', 'POINT(3 4)')");

		// Order the dataset with distances from Los Angeles.
		var results = db.Query<SampleData>("SELECT * FROM UsaCities ORDER BY ST_Distance(Geometry, 'POINT(-118.243683 34.052235)')");
		foreach (var result in results)
		{
			System.Diagnostics.Trace.WriteLine("City: " + result.Name);
		}

		// Create a selection 1.0 degree (111 km) buffer around Los Angeles, CA.
		selection = new NetTopologySuite.Geometries.Point(-118.2437, 34.0522).Buffer(1.0);

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
