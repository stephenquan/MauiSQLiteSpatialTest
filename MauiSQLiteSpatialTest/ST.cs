using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace MauiSQLiteSpatialTest;

/// <summary>
/// Implement simple OGC functions with thin wrappers around NetTopologySuite.
/// </summary>
public static partial class ST
{
	/// <summary>
	/// Returns a Geometry object from its WKT representation.
	/// </summary>
	/// <param name="wkt">The WKT representation of the geometry.</param>
	/// <returns>A Geometry object or null if the WKT is invalid.</returns>
	public static Geometry? GeomFromText(string wkt) => (new WKTReader()).Read(wkt);

	/// <summary>
	/// Returns the area of the given geometry.
	/// </summary>
	/// <param name="geometry">A Geometry object or null.</param>
	/// <returns>The area of the geometry or null if the geometry is invalid.</returns>
	public static object? Area(Geometry? geometry) => (geometry is not null) ? geometry.Area : null;
}
