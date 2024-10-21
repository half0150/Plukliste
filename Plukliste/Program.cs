namespace Plukliste;

class PluklisteProgram
{
    static void Main()
    {
        char userInput = ' ';
        List<string> fileList;
        int fileIndex = -1;
        ConsoleColor defaultColor = Console.ForegroundColor;

        Directory.CreateDirectory("import");

        if (!Directory.Exists("export"))
        {
            Console.WriteLine("Directory \"export\" not found");
            Console.ReadLine();
            return;
        }

        fileList = Directory.EnumerateFiles("export").ToList();

        while (userInput != 'Q')
        {
            if (fileList.Count == 0)
            {
                Console.WriteLine("No files found.");
            }
            else
            {
                fileIndex = ValidateFileIndex(fileIndex, fileList.Count);
                DisplayPlukliste(fileList[fileIndex]);
            }

            PrintOptions(fileIndex, fileList.Count, defaultColor);
            userInput = char.ToUpper(Console.ReadKey().KeyChar);
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red;
            HandleUserInput(userInput, ref fileList, ref fileIndex);
            Console.ForegroundColor = defaultColor;
        }
    }

    private static int ValidateFileIndex(int index, int fileCount)
    {
        return index == -1 ? 0 : index;
    }

    private static void DisplayPlukliste(string filePath)
    {
        Console.WriteLine($"File: {filePath}");

        using FileStream fileStream = File.OpenRead(filePath);
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Pluklist));
        var plukliste = (Pluklist?)xmlSerializer.Deserialize(fileStream);

        if (plukliste != null && plukliste.Items != null)  
        {
            Console.WriteLine("\n{0, -13}{1}", "Name:", plukliste.Name);
            Console.WriteLine("{0, -13}{1}", "Shipment:", plukliste.Shipment);  

            Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Amount", "Type", "Product ID", "Title");
            foreach (var item in plukliste.Items)  
            {
                Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
            }
        }
    }

    private static void PrintOptions(int index, int fileCount, ConsoleColor defaultColor)
    {
        Console.WriteLine("\n\nOptions:");
        PrintOption("Q", "Quit", defaultColor);
        if (index >= 0) PrintOption("A", "Complete Plukliste", defaultColor);
        if (index > 0) PrintOption("F", "Previous Plukliste", defaultColor);
        if (index < fileCount - 1) PrintOption("N", "Next Plukliste", defaultColor);
        PrintOption("G", "Reload Plukliste", defaultColor);
    }

    private static void PrintOption(string key, string description, ConsoleColor defaultColor)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(key);
        Console.ForegroundColor = defaultColor;
        Console.WriteLine(description);
    }

    private static void HandleUserInput(char userInput, ref List<string> fileList, ref int fileIndex)
    {
        switch (userInput)
        {
            case 'G':
                fileList = Directory.EnumerateFiles("export").ToList();
                fileIndex = -1;
                Console.WriteLine("Pluklister reloaded");
                break;
            case 'F':
                if (fileIndex > 0) fileIndex--;
                break;
            case 'N':
                if (fileIndex < fileList.Count - 1) fileIndex++;
                break;
            case 'A':
                MoveFileToImportDirectory(fileList, ref fileIndex);
                break;
        }
    }

    private static void MoveFileToImportDirectory(List<string> files, ref int index)
    {
        string fileName = Path.GetFileName(files[index]);
        string destinationPath = Path.Combine("import", fileName);
        File.Move(files[index], destinationPath);
        Console.WriteLine($"Plukliste {files[index]} completed.");
        files.RemoveAt(index);
        if (index == files.Count) index--;
    }
}
