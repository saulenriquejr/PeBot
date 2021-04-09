using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WelcomeUser.Bots
{
    public class Saludo
    {
        public async Task SendHiCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard
            {
                Title = "Bienvenido a PEVAAR SOFTWARE FACTORY!!!",
                Text = @"Soy PeBot tu asistente Virtual y estoy aquí para ayudarte.",
                Images = new List<CardImage>() { new CardImage("https://i.picasion.com/pic90/68978a4edf1400af271f05adad9f3ce7.gif") },

            };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }

        public async Task SendIntroCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {

            var newcard = new HeroCard
            {

                Text = @"¿Que deseas Hacer?",
                Buttons = new List<CardAction>()
                {
                    new CardAction(ActionTypes.MessageBack,"Información General",null,"Información General","Información General","Información General"),
                    new CardAction(ActionTypes.MessageBack,"Contactenos",null,"Contactenos","Contactenos","Contactenos"),
                    new CardAction(ActionTypes.MessageBack, "Cotizaciones", null, "Cotizaciones", "Cotizaciones", "Cotizaciones"),
                    new CardAction(ActionTypes.MessageBack, "Trabaje con nosotros", null, "Trabaje con nosotros","Trabaje con nosotros","Trabaje con nosotros"),
                    new CardAction(ActionTypes.MessageBack, "Conversemos un rato", null, "Conversemos un rato","Conversemos un rato","Conversemos un rato"),
                    new CardAction(ActionTypes.MessageBack, "Soy PEVAAR", null, "Soy PEVAAR", "Soy PEVAAR", "Soy PEVAAR"),
                    new CardAction(ActionTypes.MessageBack, "Finalizar Conversación", null, "Finalizar Conversación", "Finalizar Conversación", "Finalizar Conversación"),
                }
            };

            var response2 = MessageFactory.Attachment(newcard.ToAttachment());
            await turnContext.SendActivityAsync(response2, cancellationToken);
        }
    }
}
