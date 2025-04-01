public class TerrainLighting
{
    public readonly int Width;
    public readonly int Height;
    public readonly byte[] Result = null;

    public TerrainLighting(int w, int h)
    {
        Width = w;
        Height = h;
        Result = new byte[w * h];
    }

    public void Calculate(sbyte[] heights, double solarAngle)
    {
        double sunang = solarAngle * (Math.PI / 180.0);

        double sunx = Math.Cos(sunang) * 1.0;
        double suny = Math.Sin(sunang) * 1.0;
        double sunz = -0.75;

        //
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if ((x <= 1) || (y <= 1) ||
                    (x >= Width - 2) || (y >= Height - 2))
                {
                    Result[y * Width + x] = 0;
                    continue;
                }

                // 
                double p1x = x * 32.0;
                double p1y = y * 32.0;
                double p1z = heights[y * Width + x];

                double p2x = x * 32.0 + 32.0;
                double p2y = y * 32.0;
                double p2z = heights[y * Width + (x + 1)];

                double p3x = x * 32.0;
                double p3y = y * 32.0 + 32.0;
                double p3z = heights[(y + 1) * Width + x];

                //
                double ux = p2x - p1x;
                double uy = p2y - p1y;
                double uz = p2z - p1z;

                double vx = p3x - p1x;
                double vy = p3y - p1y;
                double vz = p3z - p1z;

                //
                double nx = (uy * vz) - (uz * vy);
                double ny = (uz * vx) - (ux * vz);
                double nz = (ux * vy) - (uy * vx);

                //
                double nl = Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz));
                nx /= nl;
                ny /= nl;
                nz /= nl;

                //
                //double dot = Math.Abs(nx * sunx + ny * suny + nz * sunz) * 64.0 + 128.0;
                double dot = Math.Abs(nx * sunx + ny * suny + nz * sunz) * 64.0 + 96.0;
                /*double dot = Math.Abs(nx * sunx + ny * suny + nz * sunz) * 62.0;
                int dot_i = (int)dot;
                uint dot_ab = (uint)Math.Abs(dot_i-31);
                // pow dot_ab
                dot_ab = (uint)(1 << (int)dot_ab);
                dot_ab = Math.Min(31, dot_ab / 16777216 / 8);
                if (dot_i < 32)
                    dot = 128.0 - ((double)dot_ab * 2);
                else dot = 128.0 + ((double)dot_ab * 2);*/

                //double dot = Math.Abs(nx * sunx + ny * suny + nz * sunz) * 128.0 + 64.0;
                Result[y * Width + x] = (byte)dot;
                //Console.WriteLine("dot = {0}", dot);
            }
        }
    }
}
