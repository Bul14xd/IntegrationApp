using Npgsql;
using IntegralCalculator.Models;

namespace IntegralCalculator.Services
{
    public static class DatabaseService
    {
        private static string connectionString = "Host=localhost;Username=postgres;Password=your_password;Database=integraldb";

        public static void InitializeDatabase()
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS calculations (
                    id SERIAL PRIMARY KEY,
                    lower_bound DOUBLE PRECISION,
                    upper_bound DOUBLE PRECISION,
                    intervals INTEGER,
                    result DOUBLE PRECISION,
                    calculation_time TIMESTAMP
                )", conn);

            cmd.ExecuteNonQuery();
        }

        public static void SaveResult(IntegralData data, CalculationResult result)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO calculations (lower_bound, upper_bound, intervals, result, calculation_time)
                VALUES (@lower, @upper, @intervals, @result, @time)",
                conn);

            cmd.Parameters.AddWithValue("lower", data.LowerBound);
            cmd.Parameters.AddWithValue("upper", data.UpperBound);
            cmd.Parameters.AddWithValue("intervals", data.NumberOfIntervals);
            cmd.Parameters.AddWithValue("result", result.Result);
            cmd.Parameters.AddWithValue("time", result.CalculationTime);

            cmd.ExecuteNonQuery();
        }
    }
}