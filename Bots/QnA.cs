using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WelcomeUser.Bots
{
    public class QnA
    {
        public async Task SendQnACardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            //string title,string subtitle, string image,
            string Concepto = "Somos una nueva generación de proveedores de servicios " +
                                       " de tecnología, enfocados en ofrecer soluciones innovadoras de " +
                                       "software mediantemediante el aprovechamiento de las tecnologías y" +
                                       "tendencias emergentes . Combinamos la ingeniería y el rigor técnico" +
                                       "de los proveedores de servicios de TI con el enfoque creativo y " +
                                       "cultural de las agencias digitales. \n \n \n \n ";

            string Visión = "Nuestra visión es convertirnos en la mejor compañía " +
                                                "de desarrollo de soluciones, combinando lo mejor" +
                                                "de la ingeniería, la innovación y el diseño. Nuestro " +
                                                "objetivo es ser el líder en la creación de productos" +
                                                "de software innovadores atractivos para las audiencias globales. \n \n \n \n ";

            string Activos = "Además de nuestros clientes y las relaciones con ellos establecidas," +
                                                "contamos con 4 activos fundamentales para nosotros." +
                                                "\n \n Equipo: Un equipo joven, proactivo, especializado e" +
                                                "implicado en los proyectos y con los clientes." +
                                                "\n \n Metodología: Tanto a nivel técnico como de gestión" +
                                                "de proyectos que nos aporta calidad y eficiencia." +
                                                "\n \n Knowhow: Nuestra experiencia y conocimiento a lo largo" +
                                                "de los años es clave para ofrecer soluciones de valor." +
                                                "\n \n Tecnología: El conocimiento de la tecnología y la capacidad" +
                                                "de reciclarnos y evolucionar tecnológicamente. \n \n \n \n ";

            string Hacemos = "Ayudamos a nuestros clientes en su estrategia y desarrollo " +
                                                 "e-business integrándonos como Partner Tecnológico y aportando " +
                                                 "nuestra capacidad, experiencia y conocimiento como valor añadido." +
                                                 "Desarrollamos proyectos modulares y escalables – integrados con " +
                                                 "los procesos de negocio – para que nuestros clientes puedan ofrecer" +
                                                 "servicios de valor añadido en entornos web y móvil a nivel B2C y B2B. \n \n \n \n ";

            var title = turnContext.Activity.Text.ToUpper();
            switch (turnContext.Activity.Text)
            {
                case "¿Quienes Somos?":
                    await SendInfoCardAsync(turnContext, cancellationToken, title, null, Concepto, "https://pevaaar.azurewebsites.net/wp-content/uploads/2017/03/logo.png");
                    break;
                case "Visión":
                    await SendInfoCardAsync(turnContext, cancellationToken, title, null, Visión, "https://pevaaar.azurewebsites.net/wp-content/uploads/2017/03/idea.png");
                    break;
                case "Nuestros Activos":
                    await SendInfoCardAsync(turnContext, cancellationToken, title, null, Activos, "https://pevaaar.azurewebsites.net/wp-content/uploads/2017/03/cloud1.png");
                    break;
                case "¿Que Hacemos?":
                    await SendInfoCardAsync(turnContext, cancellationToken, title, null, Hacemos, "https://pevaaar.azurewebsites.net/wp-content/uploads/2017/03/diagrama.png");
                    break;
            }
        }

        private static async Task SendInfoCardAsync(ITurnContext turnContext, CancellationToken cancellationToken, string title, string subtitle, string text, string image)
        {
            var card = new ThumbnailCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage> { new CardImage(image) },
                Buttons = new List<CardAction> {
                        new CardAction(ActionTypes.MessageBack,"Ir al menú principal",null,"Ir al menú principal","Ir al menú principal","Ir al menú principal"),
                        new CardAction(ActionTypes.MessageBack,"Seguir conociendo PEVAAR",null,"Seguir conociendo PEVAAR","Seguir conociendo PEVAAR","Seguir conociendo PEVAAR"),
                        new CardAction(ActionTypes.MessageBack, "Finalizar conversación", null, "Finalizar conversación", "Finalizar conversación", "Finalizar conversación"),
                },
            };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }
    }
}
