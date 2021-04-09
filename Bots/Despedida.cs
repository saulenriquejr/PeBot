using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WelcomeUser.Bots
{
    public class Despedida
    {
        public async Task SendByeCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard
            {
                Text = @"¿Estas seguro que deseas salir?",
                Images = new List<CardImage>() { new CardImage("https://aka.ms/bf-welcome-card-image") },
                Buttons = new List<CardAction>()
                {
                    new CardAction(ActionTypes.MessageBack,"Si, deseo finalizar la conversación.",null,"Si, deseo finalizar la conversación","Si, deseo finalizar la conversación","Si, deseo finalizar la conversación"),
                    new CardAction(ActionTypes.MessageBack, "No, deseo volver al menú principal.", null, "No, deseo volver al menú principal.", "No, deseo volver al menú principal.", "No, deseo volver al menú principal.")
                }
            };
            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }
    }
}
