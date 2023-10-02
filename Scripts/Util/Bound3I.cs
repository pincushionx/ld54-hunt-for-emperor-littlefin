using Godot;
using Pincushion.Util.Extensions;
using System;

namespace Pincushion.Util {
	public struct Bound3I
	{
		public Vector3I Position;
		public Vector3I Size;

		public Vector3I Min {
			get
			{
				return Position;
			}
		}
		public Vector3I Max {
			get
			{
				return Position + Size - Vector3I.Zero;
			}
		}

		public Bound3I GetRotatedValue(Vector3I rotation)
		{
			Vector3I pos = Position.GetRotatedValue(rotation);
			Vector3I size = Size.GetRotatedValue(rotation);

			Bound3I subject = new Bound3I() {
				Position = pos,
				Size = size
			};

			if (subject.Size.X < 0)
            {
                subject.Position.X += subject.Size.X;
                subject.Size.X *= -1;
            }
            if (subject.Size.Y < 0)
            {
                subject.Position.Y += subject.Size.Y;
                subject.Size.Y *= -1;
            }
            if (subject.Size.Z < 0)
            {
                subject.Position.Z += subject.Size.Z;
                subject.Size.Z *= -1;
            }
            return subject;
		}
	}
}