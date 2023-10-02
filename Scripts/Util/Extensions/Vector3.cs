using Godot;

namespace Pincushion.Util.Extensions
{
	public static class Vector3Extensions
	{
		public static Vector2 ToVector2XZ(this Godot.Vector3 vector3)
		{
			return new Vector2(vector3.X, vector3.Z);
		}

		public static Vector3I ToVector3I(this Godot.Vector3 vector3, float cellSize)
		{
			return new Vector3I(
				Mathf.FloorToInt(vector3.X / cellSize),
				Mathf.FloorToInt(vector3.Y / cellSize), 
				Mathf.FloorToInt(vector3.Z / cellSize));
		}
	}
}