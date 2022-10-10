namespace Monte_Carlo_Simulation;

class RandomNumberGenerator
{
    public static double[,] Generate(Int64 N, double T)
    {
        double[,] randomNumberArray = new double[N,(int) (T * 252)];
        var row = 1;
        var rand = new Random();

        while (row <= N)
        {
            var column = 1;
            while (column <= (int) (T * 252))
            {
                var u1 = 1.0 - rand.NextDouble();
                var u2 = 1.0 - rand.NextDouble();

                randomNumberArray[row - 1, column - 1] =
                    Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

                column++;
            }

            row++;

        }

        return randomNumberArray;
    }
}