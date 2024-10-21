// Example of functional coding where only a model layer is used
namespace Plukliste;

class PluklisteProgram
{
    static void Main()
    {
        // Arrange
        char readKey = ' ';
        List<string> files;
        int index = -1;
        ConsoleColor standardColor = Console.ForegroundColor;
        Directory.CreateDirectory("import");

        if (!Directory.Exists("export"))
        {
            Console.WriteLine("Directory \"export\" not found");
            Console.ReadLine();
            return;
        }
        files = Directory.EnumerateFiles("export").ToList();

        // Act
        while (readKey != 'Q')
        {
            if (files.Count == 0)
            {
                Console.WriteLine("No files found.");
            }
            else
            {
                if (index == -1) index = 0;

                Console.WriteLine($"Plukliste {index + 1} of {files.Count}");
                Console.WriteLine($"\nFile: {files[index]}");

                // Read file
                using FileStream fileStream = File.OpenRead(files[index]);
                var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Pluklist));
                var plukliste = (Pluklist?)xmlSerializer.Deserialize(fileStream);

                // Print plukliste
                if (plukliste != null && plukliste.Lines != null)
                {
                    Console.WriteLine("\n{0, -13}{1}", "Name:", plukliste.Name);
                    Console.WriteLine("{0, -13}{1}", "Forsendelse:", plukliste.Forsendelse);
                    // TODO: Add address to screen print

                    Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Amount", "Type", "Product ID", "Title");
                    foreach (var item in plukliste.Lines)
                    {
                        Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
                    }
                }
            }

            // Print options
            PrintOptions(index, files.Count, standardColor);

            readKey = Console.ReadKey().KeyChar;
            readKey = char.ToUpper(readKey);
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red; // Status in red
            switch (readKey)
            {
                case 'G':
                    files = Directory.EnumerateFiles("export").ToList();
                    index = -1;
                    Console.WriteLine("Pluklister reloaded");
                    break;
                case 'F':
                    if (index > 0) index--;
                    break;
                case 'N':
                    if (index < files.Count - 1) index++;
                    break;
                case 'A':
                    MoveFileToImportDirectory(files, ref index);
                    break;
            }
            Console.ForegroundColor = standardColor; // Reset color
        }
    }

    private static void PrintOptions(int index, int fileCount, ConsoleColor standardColor)
    {
        Console.WriteLine("\n\nOptions:");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Q");
        Console.ForegroundColor = standardColor;
        Console.WriteLine("uit");
        if (index >= 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("A");
            Console.ForegroundColor = standardColor;
            Console.WriteLine("fslut plukseddel");
        }
        if (index > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("F");
            Console.ForegroundColor = standardColor;
            Console.WriteLine("orrige plukseddel");
        }
        if (index < fileCount - 1)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("N");
            Console.ForegroundColor = standardColor;
            Console.WriteLine("æste plukseddel");
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("G");
        Console.ForegroundColor = standardColor;
        Console.WriteLine("enindlæs pluksedler");
    }

    private static void MoveFileToImportDirectory(List<string> files, ref int index)
    {
        string fileWithoutPath = Path.GetFileName(files[index]);
        string destinationPath = Path.Combine("import", fileWithoutPath);
        File.Move(files[index], destinationPath);
        Console.WriteLine($"Plukseddel {files[index]} completed.");
        files.RemoveAt(index);
        if (index == files.Count) index--;
    }
}
