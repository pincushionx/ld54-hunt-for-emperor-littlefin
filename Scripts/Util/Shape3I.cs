using Godot;
using System;

namespace Pincushion.Util {
	public class Shape3I
	{
		public Bound3I[] Parts;

		public void Move(Vector3I move)
		{
			for(var i = 0; i < Parts.Length; i++)
			{
				Parts[i].Position += move;
			}
		}
		public void Rotate(Vector3I rotation)
		{
			for(var i = 0; i < Parts.Length; i++)
			{
				Parts[i] = Parts[i].GetRotatedValue(rotation);
			}
		}

		public Shape3I Clone()
		{
			Bound3I[] parts = new Bound3I[Parts.Length];
			for(var i = 0; i < Parts.Length; i++)
			{
				parts[i] = Parts[i];
			}
			
			return new Shape3I() {
				Parts = parts
			};
		}
	}
}