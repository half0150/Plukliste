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
                    Console.WriteLine("\n{0, -13}{1}", "Adresse:", plukliste.Adresse);

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

            Console.ForegroundColor = ConsoleColor.Red;
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
                case 'P':
                    if (index >= 0)
                    {
                        using FileStream fileStream = File.OpenRead(files[index]);
                        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Pluklist));
                        var plukliste = (Pluklist?)xmlSerializer.Deserialize(fileStream);
                        if (plukliste != null)
                        {
                            PrintPickSlip(plukliste);
                        }
                    }
                    break;
            }
            Console.ForegroundColor = standardColor; // Reset color
        }
    }

    private static void PrintOptions(int index, int fileCount, ConsoleColor standardColor)
    {
        Console.WriteLine("\n\nOptions:");

        PrintOption("Q", "uit", standardColor);

        if (index >= 0)
        {
            PrintOption("A", "fslut plukseddel", standardColor);
        }

        if (index > 0)
        {
            PrintOption("F", "orrige plukseddel", standardColor);
        }

        if (index < fileCount - 1)
        {
            PrintOption("N", "æste plukseddel", standardColor);
        }

        PrintOption("G", "enindlæs pluksedler", standardColor);

        PrintOption("P", "rint plukseddel", standardColor);
    }

    private static void PrintPickSlip(Pluklist plukliste)
    {
        Console.WriteLine("\nChoose a template for printing:");
        Console.WriteLine("OPGRADE");
        Console.WriteLine("OPSIGELSE");
        Console.WriteLine("WELCOME");

        char templateChoice = Console.ReadKey().KeyChar;
        string selectedTemplate = "/PRINT-WELCOME.html"; 

        switch (templateChoice)
        {
            case '1':
                selectedTemplate = "/PRINT-OPGRADE.html";
                break;
            case '2':
                selectedTemplate = "/PRINT-OPSIGELSE.html";
                break;
            case '3':
                selectedTemplate = "/PRINT-WELCOME.html";
                break;
            default:
                Console.WriteLine("\nInvalid choice, using default template.");
                break;
        }

        string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates", selectedTemplate);

        if (!File.Exists(templatePath))
        {
            Console.WriteLine($"Template file \"{selectedTemplate}\" not found.");
            return;
        }

        string html = File.ReadAllText(templatePath);
        html = html.Replace("[Name]", plukliste.Name);
        html = html.Replace("[Forsendelse]", plukliste.Forsendelse);
        html = html.Replace("[Adresse]", plukliste.Adresse);

        string items = "";
        foreach (var item in plukliste.Lines)
        {
            items += $"<tr><td>{item.Amount}</td><td>{item.Type}</td><td>{item.ProductID}</td><td>{item.Title}</td></tr>";
        }
        html = html.Replace("[Plukliste]", items);

        string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "print", "print.html");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.WriteAllText(outputPath, html);

        Console.WriteLine($"Printed plukseddel to \"{outputPath}\"");
    }

    private static void PrintOption(string key, string message, ConsoleColor standardColor)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(key);
        Console.ForegroundColor = standardColor;
        Console.WriteLine(message);
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
