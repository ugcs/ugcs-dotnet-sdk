using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UgCS.SDK.Examples.Common
{
    internal static class ConsoleEx
    {
        public static T Select<T>(IEnumerable<T> values, 
            Func<T, int> identityGetter, Func<T, string> displayNameGetter)
        {
            if (!values.Any())
                throw new ArgumentException("At lease one value must be in the list.");

            Console.WriteLine($"Select {typeof(T).Name}:");
            foreach (var v in values)
            {
                Console.WriteLine($"\tid: {identityGetter(v)}; name: {displayNameGetter(v)}");
            }

            while (true)
            {
                int selectedId = readInt("Input id");
                T? selectedObject = values.FirstOrDefault(v => identityGetter(v) == selectedId);
                if (selectedObject != null)
                    return selectedObject;

                Console.WriteLine("Wrong value.");
            }
        }

        private static int readInt(string message)
        {
            Console.Write(message + ": ");
            string? input = Console.ReadLine();
            int result;
            while (input == null || !Int32.TryParse(input, out result))
            {
                Console.WriteLine("Wrong value.");
                Console.Write(message + ": ");
                input = Console.ReadLine();
            }
            return result;
        }
    }
}
