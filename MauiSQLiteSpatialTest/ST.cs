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
	/// Creates a new instance of a <see cref="NetTopologySuite.Geometries.Point"/> with the specified coordinates.
	/// </summary>
	/// <param name="x">The X coordinate of the point.</param>
	/// <param name="y">The Y coordinate of the point.</param>
	/// <returns>A <see cref="NetTopologySuite.Geometries.Point"/> representing the specified coordinates.</returns>
	public static NetTopologySuite.Geometries.Point Point(double x, double y) => new NetTopologySuite.Geometries.Point(x, y);

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
	public static double? Area(Geometry? geometry) => geometry?.Area;

	/// <summary>
	/// Returns a geometry that represents all points whose distance from this Geometry is less than or equal to distance.
	/// </summary>
	/// <param name="geometry">A Geometry object or null.</param>
	/// <param name="distance">The buffer distance.</param>
	/// <returns>The buffered geometry or null.</returns>
	public static NetTopologySuite.Geometries.Geometry? Buffer(Geometry? geometry, double distance) => geometry?.Buffer(distance);

	/// <summary>
	/// Returns the centroid of the given geometry.
	/// </summary>
	/// <param name="geometry">A Geometry object or null.</param>
	/// <returns>The centroid of the geometry or null if the geometry is invalid.</returns>
	public static NetTopologySuite.Geometries.Point? Centroid(Geometry? geometry) => geometry?.Centroid;

	/// <summary>
	/// Returns the length of the given geometry.
	/// </summary>
	/// <param name="geometry">A Geometry object or null.</param>
	/// <returns>The length of the geometry or null if the geometry is invalid.</returns>
	public static double? Length(Geometry? geometry) => geometry?.Length;

	/// <summary>
	/// Returns the X coordinate of a Point geometry.
	/// </summary>
	/// <param name="geometry">A Point geometry object or null.</param>
	/// <returns>The X coordinate of a point geometry or null.</returns>
	public static double? X(Geometry? geometry) => (geometry is NetTopologySuite.Geometries.Point pt) ? pt.X : null;

	/// <summary>
	/// Returns the Y coordinate of a Point geometry.
	/// </summary>
	/// <param name="geometry">A Point geometry object or null.</param>
	/// <returns>The Y coordinate of a point geometry or null.</returns>
	public static double? Y(Geometry? geometry) => (geometry is NetTopologySuite.Geometries.Point pt) ? pt.Y : null;
}
