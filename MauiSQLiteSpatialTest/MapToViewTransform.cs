namespace MauiSQLiteSpatialTest;

/// <summary>
/// Helper class to transform geographic coordinates to view coordinates.
/// </summary>
public class MapToViewTransform
{
	/// <summary>
	/// Gets or sets the geographic extent (in degrees) of the map.
	/// Default is the extent of the USA.
	/// </summary>
	public Rect MapExtent { get; set; } = Rect.FromLTRB(-125, 24.3, -66.9, 49.4); // USA extent

	/// <summary>
	/// Gets or sets whether the view should automatically follow the map extent.
	/// </summary>
	public bool FollowMapExtent { get; set; } = true;

	/// <summary>
	/// Gets or sets the center point of the view in view coordinates.
	/// Default is (0,0).
	/// </summary>
	public PointF ViewCenter { get; set; } = new PointF(0, 0);

	/// <summary>
	/// Gets or sets the scale factor for projecting geographic coordinates to view coordinates.
	/// Default is 1.0 (no scaling).
	/// </summary>
	public double ViewScale { get; set; } = 1.0;

	/// <summary>
	/// Resets the scale to fit the extent within the given view dimensions.
	/// </summary>
	/// <param name="viewWidth"></param>
	/// <param name="viewHeight"></param>
	public void UpdateScale(double viewWidth, double viewHeight)
	{
		if (MapExtent.Width > 0 && MapExtent.Height > 0)
		{
			double scaleX = viewWidth / MapExtent.Width;
			double scaleY = viewHeight / MapExtent.Height;
			ViewScale = Math.Min(scaleX, scaleY) * 0.9;
		}
	}

	/// <summary>
	/// Updates the center point of the view based on the specified width and height.
	/// </summary>
	/// <param name="viewWidth">The width of the view. Must be a positive value.</param>
	/// <param name="viewHeight">The height of the view. Must be a positive value.</param>
	public void UpdateCenter(double viewWidth, double viewHeight)
	{
		ViewCenter = new PointF((float)(viewWidth / 2), (float)(viewHeight / 2));
	}

	/// <summary>
	/// Changes the map extent to center on the specified map coordinates with the given width and height.
	/// </summary>
	/// <param name="mapX"></param>
	/// <param name="mapY"></param>
	/// <param name="mapWidth"></param>
	/// <param name="mapHeight"></param>
	public void SetMapExtent(double mapX, double mapY, double mapWidth, double mapHeight)
	{
		MapExtent = new Rect(mapX - mapWidth / 2, mapY - mapHeight / 2, mapWidth, mapHeight);
	}

	/// <summary>
	/// Pans the map view to center on the specified map coordinates.
	/// </summary>
	/// <remarks>This method adjusts the map's current extent so that the specified coordinates are at the center of
	/// the view. Ensure that the map coordinates are within the valid range of the map's coordinate system.</remarks>
	/// <param name="mapX">The X-coordinate of the map location to center on.</param>
	/// <param name="mapY">The Y-coordinate of the map location to center on.</param>
	public void PanTo(double mapX, double mapY)
	{
		SetMapExtent(mapX, mapY, MapExtent.Width, MapExtent.Height);
	}

	/// <summary>
	/// Pans the view to the specified map point.
	/// </summary>
	/// <param name="mapPoint">The <see cref="Point"/> representing the target location to pan to.</param>
	public void PanTo(Point mapPoint)
		=> PanTo(mapPoint.X, mapPoint.Y);

	/// <summary>
	/// Adjust the zoom level by the specified zoom factor.
	/// </summary>
	/// <param name="zoomFactor"></param>
	public void ScaleBy(double zoomFactor)
	{
		SetMapExtent(
			MapExtent.Center.X,
			MapExtent.Center.Y,
			MapExtent.Width / zoomFactor,
			MapExtent.Height / zoomFactor);
	}

	/// <summary>
	/// Projects geographic coordinates (x, y) to view coordinates (Point).
	/// </summary>
	/// <param name="mapX"></param>
	/// <param name="mapY"></param>
	/// <returns></returns>
	public PointF Project(double mapX, double mapY)
		=> new PointF(
			(float)(ViewCenter.X + (mapX - MapExtent.Center.X) * ViewScale),
			(float)(ViewCenter.Y - (mapY - MapExtent.Center.Y) * ViewScale));

	/// <summary>
	/// Unprojects view coordinates (viewX, viewY) to geographic coordinates (Point).
	/// </summary>
	/// <param name="viewX"></param>
	/// <param name="viewY"></param>
	/// <returns></returns>
	public Point UnProject(double viewX, double viewY)
		=> new Point(
			(viewX - ViewCenter.X) / ViewScale + MapExtent.Center.X,
			-(viewY - ViewCenter.Y) / ViewScale + MapExtent.Center.Y);
}
