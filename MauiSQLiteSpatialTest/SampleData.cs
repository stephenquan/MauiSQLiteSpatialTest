namespace MauiSQLiteSpatialTest;

/// <summary>
/// Sample spatial data to be stored in the SQLite database
/// </summary>
public partial class SampleData
{
	/// <summary>
	/// Gets or sets the city name.
	/// </summary>
	public string City { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the color for visualization.
	/// </summary>
	public string Color { get; set; } = "Black";

	/// <summary>
	/// Gets or sets the geometry in WKT format.
	/// </summary>
	public string Geometry { get; set; } = string.Empty;
}
