using Godot;

namespace Pincushion.Util.Extensions
{
	public static class Vector2Extensions
	{
		public static Vector3 ToVector3(this Godot.Vector2 vector2, int y)
		{
			return new Vector3(vector2.X, y, vector2.Y);
		}
	}
}