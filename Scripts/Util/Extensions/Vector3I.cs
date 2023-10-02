using System.Net;
using Godot;

namespace Pincushion.Util.Extensions
{
	public static class Vector3IExtensions
	{
        public static Vector3I Zero { get { return new Vector3I(0,0,0); } }
        public static Vector3I GetRotatedValue(this Vector3I subject, Vector3I rotation)
        {
            int tmp;

            if (rotation.X != 0 || rotation.Y != 0 || rotation.Z != 0)
            {
                if (rotation.X == 1)
                {
                    tmp = subject.Y;
                    subject.Y = subject.Z;
                    subject.Z = -tmp;
                }
                else if (rotation.X == -1 || rotation.X == 3)
                {
                    tmp = subject.Y;
                    subject.Y = -subject.Z;
                    subject.Z = tmp;
                }
                else if (rotation.X == 2)
                {
                    subject.Y = -subject.Y;
                    subject.Z = -subject.Z;
                }

                if (rotation.Y == 1)
                {
                    tmp = subject.X;
                    subject.X = subject.Z;
                    subject.Z = -tmp;
                }
                else if (rotation.Y == -1 || rotation.X == 3)
                {
                    tmp = subject.X;
                    subject.X = -subject.Z;
                    subject.Z = tmp;
                }
                else if (rotation.Y == 2)
                {
                    subject.X = -subject.X;
                    subject.Z = -subject.Z;
                }

                if (rotation.Z == 1)
                {
                    tmp = subject.X;
                    subject.X = subject.Y;
                    subject.Y = -tmp;
                }
                else if (rotation.Z == -1 || rotation.X == 3)
                {
                    tmp = subject.X;
                    subject.X = -subject.Y;
                    subject.Y = tmp;
                }
                else if (rotation.Z == 2)
                {
                    subject.X = -subject.X;
                    subject.Y = -subject.Y;
                }
            }
            return subject;
        }
    }
}