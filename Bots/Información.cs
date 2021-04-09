using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WelcomeUser.Bots
{
    public class Información
    {
        public async Task SendPhoneCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new ThumbnailCard
            {
                Title = "Telefonos de Contacto e Informacion",
                Subtitle = "Contactenos para tener el gusto de atenderlos",
                Text = "PBX: +57 (1) 717-298 \n \n \n" +
                       "Móvil: +57(301) 336 - 1362 \n \n \n" +
                       "¿Ahora que deseas hacer?",
                Images = new List<CardImage> { new CardImage("https://holatelcel.com/wp-content/uploads/2017/11/celular.gif") },
                Buttons = new List<CardAction> {
                        new CardAction(ActionTypes.MessageBack,"Ir al menú principal",null,"Ir al menú principal","Ir al menú principal","Ir al menú principal"),
                        new CardAction(ActionTypes.MessageBack, "Finalizar conversación", null, "Finalizar conversación", "Finalizar conversación", "Finalizar conversación"),
                    },
            };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);


        }

        public async Task SendHorarCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new ThumbnailCard
            {
                Title = "Información de Nuestras Oficinas",
                Subtitle = "Visitenos para tener el gusto de atenderlos",
                Text = "Visitanos en: \n \n \n" +
                           "Bogotá: Calle 68 # 12 – 32  \n \n \n" +
                           "Barranquilla: Calle 75 C # 39 – 143, oficina 01 \n \n \n" +
                           "Horarios de Atención: \n \n \n" +
                           "Lunes a Viernes: 8:00 am – 12:00 am  y de  2:00 pm - 6:00 pm  \n \n \n" +
                           "Sábados y Domingos: Cerrado \n \n \n" +
                           "¿Ahora que deseas hacer?",
                Images = new List<CardImage> { new CardImage("https://www.yorokobu.es/src/uploads/2013/06/tumblr_inline_mm3bmbPITb1qz4rgp.gif") },
                Buttons = new List<CardAction> {
                        new CardAction(ActionTypes.MessageBack,"Ir al menú principal",null,"Ir al menú principal","Ir al menú principal","Ir al menú principal"),
                        new CardAction(ActionTypes.MessageBack, "Finalizar conversación", null, "Finalizar conversación", "Finalizar conversación", "Finalizar conversación"),
                    },
            };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }

        public async Task SendKnowlCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var attachments = new List<Attachment>();

            var card = new HeroCard
            {
                Text = @"¿Que te gustaría saber?",
                Images = new List<CardImage>() { new CardImage("https://aka.ms/bf-welcome-card-image") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(ActionTypes.MessageBack,"Mostrar telefonos",null,"Telefonos","Mostrar telefonos"),
                    new CardAction(ActionTypes.MessageBack, "Mostrar horario", null, "Horario de atención", "Mostrar horario")
                }
            };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }
        public async Task SendInfGenCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("Que te gustaría saber acerca de PEVAAR Software Factory?");

            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "¿Quienes Somos?", Type = ActionTypes.ImBack, Value = "¿Quienes Somos?", Image = null, ImageAltText = null },
                    new CardAction() { Title = "Visión", Type = ActionTypes.ImBack, Value = "Visión", Image = null, ImageAltText = null },
                    new CardAction() { Title = "Nuestros Activos", Type = ActionTypes.ImBack, Value = "Nuestros Activos", Image = null, ImageAltText = null},
                    new CardAction() { Title = "¿Que Hacemos?", Type = ActionTypes.ImBack, Value = "¿Que Hacemos?", Image = null, ImageAltText = null},
                },
            };
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}
