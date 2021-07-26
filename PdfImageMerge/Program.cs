using System;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;

namespace PdfImageMerge
{
    class Program
    {
        /// <summary>
        /// Merge images and pdfs into a single pdf
        /// </summary>
        /// <param name="files">All the files</param>
        /// <param name="date">Transaction</param>
        /// <param name="description">Short description</param>
        ///
        static void Main(FileInfo[] files, DateTime date, string description)
        {
            var outputFileName = $"{date:yyyy-MM-dd} - utlägg - {description}.pdf";
            using var pdfOutput = new PdfDocument(new PdfWriter(outputFileName));

            var merger = new PdfMerger(pdfOutput);

            merger.SetCloseSourceDocuments(true);

            foreach (var file in files)
            {
                Console.WriteLine($"{file.Extension}");
                if (file.Extension == ".png")
                {
                    using var imageDocument = new PdfDocument(new PdfWriter($"temp_{file.Name}.pdf"));
                    var imageData = ImageDataFactory.Create(file.FullName);
                    var image = new Image(imageData);
                    imageDocument.AddNewPage(new PageSize(
                        PageSize.A4.GetWidth(),
                        image.GetImageHeight() * PageSize.A4.GetWidth() / image.GetImageWidth()
                    ));

                    using var document = new Document(imageDocument);
                    document.SetMargins(0,0,0,0);
                    document.Add(image);
                    imageDocument.Close();
                    document.Close();
                    using var writtenImagePdf = new PdfDocument(new PdfReader($"temp_{file.Name}.pdf"));
                    merger.Merge(writtenImagePdf, 1, 1);
                }

                if (file.Extension == ".pdf")
                {
                    using var pdfDocument = new PdfDocument(new PdfReader(file));
                    merger.Merge(pdfDocument, 1, pdfDocument.GetNumberOfPages());
                }
                // file.Extension switch
                // {
                //     "pdf" =>
                //     ;
                //     _ => ;
                // };
                //
                // if ("pdf".Equals(file.Extension, StringComparison.OrdinalIgnoreCase))
                // {
                //
                // }
            }
            merger.Close();
            Console.WriteLine(outputFileName);
        }

        private PdfDocument GetPdfDocument(FileInfo fi) =>
            fi.Extension switch
            {
                "pdf" => new PdfDocument(new PdfReader(fi)),
                "png" => new PdfDocument(new PdfReader(fi))
            };
    }
}