﻿using Plukliste;
//Eksempel på funktionel kodning hvor der kun bliver brugt et model lag

//Arrange
System.Xml.Serialization.XmlSerializer xmlSerializer = 
    new System.Xml.Serialization.XmlSerializer(typeof(Pluklist));
char readKey = ' ';
List<string> files;
var index = -1;
var standardColor = Console.ForegroundColor;
Directory.CreateDirectory("import");

if (!Directory.Exists("export"))
{
    Console.WriteLine("Directory \"export\" not found");
    Console.ReadLine();
    return;
}

files = Directory.EnumerateFiles("export").ToList();
while (readKey != 'Q')
{
    //get files
    if(files.Count == 0)
    {
        Console.WriteLine("No new files found.");
        
    } else
    {
        if (index == -1) index = 0;

        Console.WriteLine($"Plukliste {index+1} af {files.Count}");
        Console.WriteLine($"\nfile: {files[index]}");
        FileStream file = File.OpenRead(files[index]);
        var plukliste = (Pluklist)xmlSerializer.Deserialize(file);
        Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");
        foreach(var item in plukliste.items)
        {
            Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.amount, item.type, item.productid, item.title);
        }
        file.Close();
    }

    //Print options
    Console.WriteLine("\n\nOptions:");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("Q");
    Console.ForegroundColor = standardColor;
    Console.WriteLine("uit");
    if( index >= 0)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("A");
        Console.ForegroundColor = standardColor;
        Console.WriteLine("fslut plukseddel");
    }
    if( index > 0)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("F");
        Console.ForegroundColor = standardColor;
        Console.WriteLine("orrige plukseddel");
    }
    if (index < files.Count - 1)
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

    readKey = Console.ReadKey().KeyChar;
    if (readKey >= 'a') readKey -= (char)('a' - 'A'); //To upper hack
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Red;
    switch (readKey)
    {
        case 'G':
            files = Directory.EnumerateFiles("export").ToList();
            index = -1;
            Console.WriteLine("Pluklister genindlæst");
            break;
        case 'F':
            if (index > 0) index -= 1;
            break;
        case 'N':
            if (index < files.Count - 1) index += 1;
            break;
        case 'A':
            File.Move(files[index], "import");
            break;

    }
    Console.ForegroundColor = standardColor;

}


