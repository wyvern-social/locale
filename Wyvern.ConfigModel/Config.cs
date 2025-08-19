using System;
using System.IO;
using Tommy;

namespace Wyvern.ConfigModel
{
    public static class Config
    {
        private static readonly TomlTable Root;

        static Config()
        {
            var path = Path.GetFullPath(Path.Combine("..", "config.toml"));
            if (!File.Exists(path))
                throw new FileNotFoundException($"config.toml not found at: {path}");

            try
            {
                using var reader = File.OpenText(path);
                Root = TOML.Parse(reader);
                Console.WriteLine($"Loaded config.toml from: {path}");
            }
            catch (TomlParseException ex)
            {
                throw new Exception($"Error parsing TOML: {ex.Message}", ex);
            }
        }


        /// <summary>
        /// Get a value of type T. Throws if missing or invalid.
        /// </summary>
public static T GetKey<T>(params string[] path)
{
    var value = TraversePath(path);

    try
    {
        if (value == null)
            throw new Exception();

        if (typeof(T) == typeof(bool) && value is Tommy.TomlBoolean tb)
            return (T)(object)tb.Value;

        if (typeof(T) == typeof(string) && value is Tommy.TomlString ts)
            return (T)(object)ts.Value;

        if (typeof(T) == typeof(int) && value is Tommy.TomlInteger ti)
            return (T)(object)(int)ti.Value;

        if (typeof(T) == typeof(long) && value is Tommy.TomlInteger tli)
            return (T)(object)tli.Value;

        if (typeof(T) == typeof(double) && value is Tommy.TomlFloat td)
            return (T)(object)(double)td.Value;

        if (typeof(T) == typeof(float) && value is Tommy.TomlFloat tdf)
            return (T)(object)(float)tdf.Value;

        return (T)Convert.ChangeType(value, typeof(T));
    }
    catch (Exception)
    {
        throw new Exception($"Required key '{string.Join('.', path)}' missing or invalid in config.toml!");
    }
}

        private static object TraversePath(string[] path)
        {
            var current = Root;

            for (int i = 0; i < path.Length - 1; i++)
            {
                if (current[path[i]] is not TomlTable next || next == null)
                    throw new Exception($"Required table '{string.Join('.', path[..(i + 1)])}' missing in config.toml!");

                current = next;
            }

            string lastKey = path[^1];
            var finalValue = current[lastKey];

            if (finalValue == null)
                throw new Exception($"Required key '{string.Join('.', path)}' missing in config.toml!");

            return finalValue;
        }
    }
}
