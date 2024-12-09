namespace Octree
{
    public static class Utils
    {
        public static Godot.Vector3 Vector3Convert(System.Numerics.Vector3 NumericsVector3)
        {
            return new Godot.Vector3(
                NumericsVector3.X,
                NumericsVector3.Y,
                NumericsVector3.Y
            );
        }

        public static System.Numerics.Vector3 Vector3Convert(Godot.Vector3 GodotVector3)
        {
            return new System.Numerics.Vector3(
                GodotVector3.X,
                GodotVector3.Y,
                GodotVector3.Z
            );
        }
    }
}