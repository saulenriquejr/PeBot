using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.Text;
using PdfSharpCore.Drawing.Layout;
using Newtonsoft.Json;

namespace WelcomeUser.Bots
{
    public class MenuPevaar
    {
        public async Task SendPevaarCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var newcard = new HeroCard
            {

                Text = @"¿Como puedo ayudarte?",
                Buttons = new List<CardAction>()
                {
                    new CardAction(ActionTypes.MessageBack,"Generar Certificado Laboral",null,"Certificado","Certificado","Certificado"),
                    new CardAction(ActionTypes.MessageBack, "Solicitar autorización de ausencias",null,"Ausencias","Ausencias","Ausencias"),
                    new CardAction(ActionTypes.MessageBack, "Inspección rápida de Equipo de computo", null, "Inspección Equipo","Inspección Equipo","Inspección Equipo"),
                    new CardAction(ActionTypes.MessageBack, "Conversemos un rato", null, "Conversemos un rato","Conversemos un rato","Conversemos un rato"),
                    new CardAction(ActionTypes.MessageBack, "Volver al menú Principal", null, "Volver al menú Principal", "Volver al menú Principal", "Volver al menú Principal"),
                    new CardAction(ActionTypes.MessageBack, "Finalizar Conversación", null, "Finalizar Conversación", "Finalizar Conversación", "Finalizar Conversación"),
                }
            };
            var response2 = MessageFactory.Attachment(newcard.ToAttachment());
            await turnContext.SendActivityAsync(response2, cancellationToken);
        }

        public async Task GenerarCertificado(ITurnContext turnContext, CancellationToken cancellationToken, string cedula, string cargo, string name, string fechaI, string fechaF, string descripcion)
        {
            //PdfDocument document = new PdfDocument();
            //PdfPage page = document.AddPage();
            //XGraphics gfx = XGraphics.FromPdfPage(page);
            //XFont font = new XFont("Verdana", 20, XFontStyle.Bold);
            //gfx.DrawString(DateTime.UtcNow.AddDays, font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
            //gfx.DrawString("Pevaar Software Factory certifica que Pepe Perez trabajó para la compañia desde el dia 01 de Enero del 2022 hasta el 23 de Diciembre del 2025 desempeñando el cargo de desarrollador de aplicaciones web.", font, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
            string filename = "CertificadoLaboralPevaar";
            //document.Save(filename);
            ////Process.Start(filename);
            var title = DateTime.Now.ToString("MM-dd-yyyy-hh-m-ss") + "-" +filename+ "-" +name;
            var now = DateTime.Now;
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("EL SUSCRITO JAVIER PEREZ DE PEVAAR");
            sb.AppendLine("NIT: 23Y4321T236T4I71T24E823 ");
            sb.AppendLine("");
            sb.AppendLine("CERTIFICA:");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Que " + name + ", identificado(a) con cédula de ciudadanía No."+cedula+" , [está o estuvo] vinculado a PEVAAR SOFTWARE FACTORY bajo un contrato de prestación de servicios, desempeñando el/los siguientes cargos:"); 
            sb.AppendLine("");
            sb.AppendLine("•	"+cargo);
            sb.AppendLine("");
            sb.AppendLine("Desde el "+fechaI+" Hasta el "+fechaF);
            sb.AppendLine("");
            sb.AppendLine("Funciones Desempeñadas:");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine(descripcion);
            sb.AppendLine("");
            sb.AppendLine("Expedido en Bogota el " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("FIRMA");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("RFID Port: " + "Linea1");
            sb.AppendLine("TFI IP Address: " + "Linea1");
            sb.AppendLine("QR Port: " + "Linea1");

            //sb.AppendLine("Passed Tests").AppendLine();
            //foreach (var p in passedList)
            //{
            //    sb.AppendLine("\t").Append(p.Trim()).AppendLine();
            //}
            //sb.AppendLine("Failed Tests").AppendLine();
            //foreach (var f in failedList)
            //{
            //    sb.AppendLine("\f").Append(f.Trim()).AppendLine();
            //}

            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = title;
            PdfPage pdfPage = pdf.AddPage();
            XImage header = XImage.FromFile("C:\\Users\\Pevaar 07\\Documents\\PeBot\\RecursosGraficos\\cabezote.jpg");
            XGraphics graph = XGraphics.FromPdfPage(pdfPage);
            graph.DrawImage(header, 20, 20, 260, 60) ;
            //XGraphics graph2 = XGraphics.FromPdfPage(pdfPage);
            XImage footer = XImage.FromFile("C:\\Users\\Pevaar 07\\Documents\\PeBot\\RecursosGraficos\\footer.png");
            graph.DrawImage(footer, 20, 700, 550, 60);
            XImage firma = XImage.FromFile("C:\\Users\\Pevaar 07\\Documents\\PeBot\\RecursosGraficos\\firma_falsa.jpg");
            graph.DrawImage(firma, 60, 450, 100, 50);
            var formatter = new XTextFormatter(graph);
            var layoutRectangle = new XRect(50, 100, 600, 420);
            XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
            formatter.DrawString(sb.ToString(), font, XBrushes.Black, layoutRectangle);
            //graph.DrawString(sb.ToString(), font, XBrushes.Black, new XRect(0, 0, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);
            
            string pdfFilename = title + ".pdf";
            pdf.Save(pdfFilename);
            var response2 = MessageFactory.Text("Tu documento "+ pdfFilename + " ha sido generado exitosamente.");
            await turnContext.SendActivityAsync(response2, cancellationToken);
        }

        public async Task PresentarAusencia(ITurnContext turnContext, CancellationToken cancellationToken, string name, string fechaI, string fechaF, string description, string lideres)
        {
            string filename = "PresentarAusenciaPevaar";
            var title = DateTime.Now.ToString("MM-dd-yyyy-hh-m-ss") + "-" + filename + "-" + name;
            var now = DateTime.Now;
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("SOLICITUD DE AUSENCIA JUSTIFICADA");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("El contratista " + name +" , cuyos lider(es) son "+lideres+ ", solicita permiso de ausencia desde el "+fechaI+" hasta el "+fechaF+", bajo la siguiente justificación: "+description);
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Expedido en Bogota el" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("FIRMA");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("RFID Port: " + "Linea1");
            sb.AppendLine("TFI IP Address: " + "Linea1");
            sb.AppendLine("QR Port: " + "Linea1");

            //sb.AppendLine("Passed Tests").AppendLine();
            //foreach (var p in passedList)
            //{
            //    sb.AppendLine("\t").Append(p.Trim()).AppendLine();
            //}
            //sb.AppendLine("Failed Tests").AppendLine();
            //foreach (var f in failedList)
            //{
            //    sb.AppendLine("\f").Append(f.Trim()).AppendLine();
            //}

            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = title;
            PdfPage pdfPage = pdf.AddPage();
            XImage header = XImage.FromFile("C:\\Users\\Pevaar 07\\Documents\\PeBot\\RecursosGraficos\\cabezote.jpg");
            XGraphics graph = XGraphics.FromPdfPage(pdfPage);
            graph.DrawImage(header, 20, 20, 260, 60);
            //XGraphics graph2 = XGraphics.FromPdfPage(pdfPage);
            XImage footer = XImage.FromFile("C:\\Users\\Pevaar 07\\Documents\\PeBot\\RecursosGraficos\\footer.png");
            graph.DrawImage(footer, 20, 700, 550, 60);
            XImage firma = XImage.FromFile("C:\\Users\\Pevaar 07\\Documents\\PeBot\\RecursosGraficos\\firma_falsa.jpg");
            graph.DrawImage(firma, 60, 450, 100, 50);
            var formatter = new XTextFormatter(graph);
            var layoutRectangle = new XRect(50, 100, 500, 420);
            XFont font = new XFont("Verdana", 12, XFontStyle.Regular);
            formatter.DrawString(sb.ToString(), font, XBrushes.Black, layoutRectangle);
            //graph.DrawString(sb.ToString(), font, XBrushes.Black, new XRect(0, 0, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.TopLeft);

            string pdfFilename = title + ".pdf";
            pdf.Save(pdfFilename);
            var response2 = MessageFactory.Text("Tu documento " + pdfFilename + " ha sido generado exitosamente.");
            await turnContext.SendActivityAsync(response2, cancellationToken);
        }
        
        public async Task InspeccionEquipo(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var newcard = new HeroCard
            {

                Text = @"¿Como puedo ayudarte?",
                Buttons = new List<CardAction>()
                {
                    new CardAction(ActionTypes.MessageBack,"Generar Certificado Laboral",null,"Certificado","Certificado","Certificado"),
                    new CardAction(ActionTypes.MessageBack, "Solicitar autorización de ausencias",null,"Ausencias","Ausencias","Ausencias"),
                    new CardAction(ActionTypes.MessageBack, "Inspección rápida de Equipo de computo", null, "Inspección Equipo","Inspección Equipo","Inspección Equipo"),
                    new CardAction(ActionTypes.MessageBack, "Conversemos un rato", null, "Conversemos un rato","Conversemos un rato","Conversemos un rato"),
                    new CardAction(ActionTypes.MessageBack, "Volver al menú Principal", null, "Volver al menú Principal", "Volver al menú Principal", "Volver al menú Principal"),
                    new CardAction(ActionTypes.MessageBack, "Finalizar Conversación", null, "Finalizar Conversación", "Finalizar Conversación", "Finalizar Conversación"),
                }
            };
            var response2 = MessageFactory.Attachment(newcard.ToAttachment());
            await turnContext.SendActivityAsync(response2, cancellationToken);
        }
    }
}
