using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WelcomeUser.Bots
{
    public class Emociones
    {
        public async Task SendEmailCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard
            {
                Title = "He recibido toda la información que necesito!!!",
                Text = @"En pocos segundos pondre tu correo en la bandeja de entrada y enviare una copia a tu direccion de E-mail.",
                Images = new List<CardImage>() { new CardImage("https://i.picasion.com/pic90/c9c5a326c946c688bde7b82244ffa6ec.gif") },

            };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }

        public async Task SmileCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard
            {
                Images = new List<CardImage>() { new CardImage("https://i.picasion.com/pic90/8958dd1ff9761598e7198b38f3b26c19.gif") },
            };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }
    }
}
