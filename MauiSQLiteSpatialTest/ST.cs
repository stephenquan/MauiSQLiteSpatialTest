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
	/// Returns the WKT representation of the given geometry.
	/// </summary>
	/// <param name="geometry"></param>
	/// <returns></returns>
	public static string? AsText(Geometry? geometry) => geometry?.AsText();

	/// <summary>
	/// Returns the area of the given geometry.
	/// </summary>
	/// <param name="geometry">A Geometry object or null.</param>
	/// <returns>The area of the geometry or null if the geometry is invalid.</returns>
	public static object? Area(Geometry? geometry) => geometry?.Area;

	/// <summary>
	/// Returns the centroid of the given geometry.
	/// </summary>
	/// <param name="geometry">A Geometry object or null.</param>
	/// <returns>The centroid of the geometry or null if the geometry is invalid.</returns>
	public static object? Centroid(Geometry? geometry) => geometry?.Centroid;

	/// <summary>
	/// Returns the length of the given geometry.
	/// </summary>
	/// <param name="geometry">A Geometry object or null.</param>
	/// <returns>The length of the geometry or null if the geometry is invalid.</returns>
	public static object? Length(Geometry? geometry) => geometry?.Length;

	/// <summary>
	/// Returns the X coordinate of a Point geometry.
	/// </summary>
	/// <param name="geometry">A Point geometry object or null.</param>
	/// <returns>The X coordinate of a point geometry or null.</returns>
	public static object? X(Geometry? geometry) => (geometry is NetTopologySuite.Geometries.Point pt) ? pt.X : null;

	/// <summary>
	/// Returns the Y coordinate of a Point geometry.
	/// </summary>
	/// <param name="geometry">A Point geometry object or null.</param>
	/// <returns>The Y coordinate of a point geometry or null.</returns>
	public static object? Y(Geometry? geometry) => (geometry is NetTopologySuite.Geometries.Point pt) ? pt.Y : null;
}
