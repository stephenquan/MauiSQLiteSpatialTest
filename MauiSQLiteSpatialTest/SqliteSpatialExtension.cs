using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using SQLite;
using SQLitePCL;

namespace MauiSQLiteSpatialTest;

/// <summary>
/// Provides extension methods for enabling spatial functions in SQLite connections.
/// </summary>
public static class SqliteSpatialExtensions
{
	/// <summary>
	/// Applies spatial extensions to the given SQLite connection.
	/// </summary>
	/// <param name="db"></param>
	public static void UseSpatialExtensions(this SQLiteConnection db)
	{
		var dbHandle = db.Handle;
		SQLitePCL.raw.sqlite3_create_function(db.Handle, "ST_GeomFromText", 1, SQLitePCL.raw.SQLITE_UTF8 | SQLitePCL.raw.SQLITE_DETERMINISTIC, null, ST_GeomFromText);
		SQLitePCL.raw.sqlite3_create_function(db.Handle, "ST_Area", 1, SQLitePCL.raw.SQLITE_UTF8 | SQLitePCL.raw.SQLITE_DETERMINISTIC, null, ST_Area);
	}

	/// <summary>
	/// Implements the ST_GeomFromText function for SQLite.
	/// </summary>
	/// <param name="ctx"></param>
	/// <param name="user_data"></param>
	/// <param name="args"></param>
	static void ST_GeomFromText(sqlite3_context ctx, object user_data, sqlite3_value[] args)
		=> ST_StringFunction(ctx, user_data, args, (s) => ST.GeomFromText(s) is not null ? s : null);

	/// <summary>
	/// Implements the ST_Area function for SQLite.
	/// </summary>
	/// <param name="ctx"></param>
	/// <param name="user_data"></param>
	/// <param name="args"></param>
	static void ST_Area(sqlite3_context ctx, object user_data, sqlite3_value[] args)
		=> ST_GeometryFunction(ctx, user_data, args, (g) => ST.Area(g));

	/// <summary>
	/// Internal helper to handle string-based SQLite functions.
	/// </summary>
	/// <param name="ctx"></param>
	/// <param name="user_data"></param>
	/// <param name="args"></param>
	/// <param name="func"></param>
	static void ST_StringFunction(sqlite3_context ctx, object user_data, sqlite3_value[] args, Func<string, object?> func)
	{
		try
		{
			SQLitePCL.utf8z utf8z = raw.sqlite3_value_text(args[0]);
			string str0 = utf8z.utf8_to_string();
			SetResult(ctx, func(str0));
		}
		catch (Exception ex)
		{
			SetResultError(ctx, ex);
		}
	}

	/// <summary>
	/// Internal helper to handle geometry-based SQLite functions.
	/// </summary>
	/// <param name="ctx"></param>
	/// <param name="user_data"></param>
	/// <param name="args"></param>
	/// <param name="func"></param>
	static void ST_GeometryFunction(sqlite3_context ctx, object user_data, sqlite3_value[] args, Func<Geometry?, object?> func)
	{
		try
		{
			SQLitePCL.utf8z utf8z = raw.sqlite3_value_text(args[0]);
			string wkt = utf8z.utf8_to_string();
			WKTReader reader = new();
			var geometry = reader.Read(wkt);
			SetResult(ctx, func(geometry));
		}
		catch (Exception ex)
		{
			SetResultError(ctx, ex);
		}
	}

	/// <summary>
	/// Internal helper to set the result of a SQLite function call.
	/// </summary>
	/// <param name="ctx"></param>
	/// <param name="result"></param>
	static void SetResult(sqlite3_context ctx, object? result)
	{
		switch (result)
		{
			case null:
				raw.sqlite3_result_null(ctx);
				break;
			case string s:
				raw.sqlite3_result_text(ctx, utf8z.FromString(s));
				break;
			case double d:
				raw.sqlite3_result_double(ctx, d);
				break;
			case int i:
				raw.sqlite3_result_int(ctx, i);
				break;
			default:
				raw.sqlite3_result_null(ctx);
				break;
		}
	}

	/// <summary>
	/// Internal helper to set an error result for a SQLite function call.
	/// </summary>
	/// <param name="ctx"></param>
	/// <param name="ex"></param>
	static void SetResultError(sqlite3_context ctx, Exception ex)
	{
		raw.sqlite3_result_error(ctx, utf8z.FromString(ex.Message));
		raw.sqlite3_result_error_code(ctx, raw.SQLITE_ERROR);
	}
}
