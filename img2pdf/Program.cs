using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

if (args.Length == 0)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("No file paths have been supplied. Exiting...");
    Console.ForegroundColor = ConsoleColor.White;
    return;
}

if (args.Length < 2)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Either no input or output files have been supplied. The last file path must be the result file. Exiting...");
    Console.ForegroundColor = ConsoleColor.White;
    return;
}

var inputFilePaths = args[0..^1];
var outputFilePath = args.Last();

var allowedInputExtensions = new[] { ".jpg", ".jpeg", ".png" };
var allowedOutputExtensions = new[] { ".pdf" };

foreach (var filePath in inputFilePaths)
{
    var inputFileInfo = new FileInfo(filePath);

    if (!allowedInputExtensions.Contains(inputFileInfo.Extension, StringComparer.OrdinalIgnoreCase))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"The file {inputFileInfo.Name} is not an image. Exiting...");
        Console.ForegroundColor = ConsoleColor.White;
        return;
    }

    if (!inputFileInfo.Exists)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"The file {inputFileInfo.Name} does not exist. Exiting...");
        Console.ForegroundColor = ConsoleColor.White;
        return;
    }
}

var outputFileInfo = new FileInfo(outputFilePath);

if (!allowedOutputExtensions.Contains(outputFileInfo.Extension, StringComparer.OrdinalIgnoreCase))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"The file {outputFileInfo.Name} is not a PDF document. Exiting...");
    Console.ForegroundColor = ConsoleColor.White;
    return;
}

if (outputFileInfo.Exists)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"The file {outputFileInfo.Name} already exists. Exiting...");
    Console.ForegroundColor = ConsoleColor.White;
    return;
}

using var stream = new FileStream(outputFilePath, FileMode.CreateNew);
using var writer = new PdfWriter(stream);
using var pdf = new PdfDocument(writer);
using var document = new Document(pdf);
document.SetMargins(0, 0, 0, 0);

for (var pageNumber = 1; pageNumber <= inputFilePaths.Length; pageNumber++)
{
    var filePath = inputFilePaths[pageNumber - 1];
    var fileInfo = new FileInfo(filePath);

    Console.WriteLine($"Adding file {fileInfo.Name} to document...");

    using var imageStream = new MemoryStream();
    fileInfo.OpenRead().CopyTo(imageStream);
    var imageBytes = imageStream.ToArray();

    var imageData = ImageDataFactory.Create(imageBytes);
    var image = new Image(imageData);

    pdf.AddNewPage(new PageSize(image.GetImageWidth(), image.GetImageHeight()));

    image.SetFixedPosition(pageNumber, 0, 0);
    document.Add(image);
}

pdf.Close();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"Document {outputFileInfo.Name} was created successfully! Exiting...");
Console.ForegroundColor = ConsoleColor.White;
