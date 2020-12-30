
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WelcomeUser.Common;
using WelcomeUser.Mail;
using WelcomeUser.Models;
using WelcomeUser.Services;
using Attachment = Microsoft.Bot.Schema.Attachment;

namespace Microsoft.BotBuilderSamples
{
    public class WelcomeUserBot : ActivityHandler
    {
        
            
          
        private const string Name = "id_name";
        private const string Description = "id_description";
        private const string Phone = "id_phone";
        private const string Mail = "id_mail";
    


        private readonly BotState _userState;
        private readonly AppSettings _appSetting;
        public readonly IMailService mailService;
        //private readonly IAppServices _appServices;
        private readonly StateService _stateService;
        private readonly BotServices _botServices;


        public static bool DidBotWelcomeUser { get; private set; }

        // Initializes a new instance of the "WelcomeUserBot" class.

        public WelcomeUserBot(UserState userState, IMailService mailService, AppSettings appSetting, StateService stateService, BotServices botServices)//, IAppServices appServices
        { 
            _userState = userState;
            this.mailService = mailService;
            //_appServices = appServices;
            _appSetting = appSetting;
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));
            _botServices = botServices ?? throw new System.ArgumentNullException(nameof(stateService)); 
        
        }

        readonly string[] _cards =
        {
            Path.Combine(".", "Resources", "FormCotizar.json"),
            Path.Combine(".", "Resources", "FormTrabajaConNosotr.json"),
        };
        public int inConversation;

        public static IFormFile remoteFileUrl { get; private set; }
        public static IList<Attachment> Attachments { get; private set; }

        
// Greet when users are added to the conversation. Note that all channels do not send the conversation update activity. If you find that this bot works in the emulator, but does not in
// another channel the reason is most likely that the channel does not send this activity.

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await SendHiCardAsync(turnContext, cancellationToken);
                    await SendIntroCardAsync(turnContext, cancellationToken);
                }
            }
        }

        //protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    if (turnContext.Activity.Name == "webchat/join")
        //    {
        //        await turnContext.SendActivityAsync("Got webchat/join event.");
        //    }
        //}


        private async Task SendHiCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
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

        private static async Task SendEmailCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
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

        private static async Task SmileCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard
            { 
                Images = new List<CardImage>() { new CardImage("https://i.picasion.com/pic90/8958dd1ff9761598e7198b38f3b26c19.gif") },
            };

            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {   
            
            var welcomeUserStateAccessor = _userState.CreateProperty<WelcomeUserState>(nameof(WelcomeUserState));
            var didBotWelcomeUser = await welcomeUserStateAccessor.GetAsync(turnContext, () => new WelcomeUserState(), cancellationToken);

            if (didBotWelcomeUser.DidBotWelcomeUser == false)
            {

                if (turnContext.Activity.Text == null) {
                    Attachments = turnContext.Activity.Attachments;
                    await FormActivityAsync(turnContext, cancellationToken, 1);
                }
                didBotWelcomeUser.DidBotWelcomeUser = true;
                if (turnContext.Activity.Text != null)
                {
                    int r;
                    var text = turnContext.Activity.Text.ToLowerInvariant();
                    switch (text)
                    {
                        case "información general":
                            await SendInfGenCardAsync(turnContext, cancellationToken);
                            break;
                        case "contactenos":
                            await SendKnowlCardAsync(turnContext, cancellationToken);
                            break;
                        case "conversemos un rato":
                            inConversation = 1;
                            await GetName(turnContext, cancellationToken);
                            break;
                        case "finalizar conversación":
                            await SendByeCardAsync(turnContext, cancellationToken);
                            break;
                        case "cotizaciones":
                            r = 0;
                            await FormActivityAsync(turnContext, cancellationToken, r);
                            break;
                        case "trabaje con nosotros":
                            r = 1;
                            await RecruitingSteps(turnContext, cancellationToken);
                            break;
                    }
                }
            
            }


            if (string.IsNullOrEmpty(turnContext.Activity.Text) && turnContext.Activity.Value!=null)
            {
                await validationsAssert(turnContext, cancellationToken);
                if (turnContext.Responded == false) 
                {
                    dynamic value = turnContext.Activity.Value;
                    string comercialEmail = _appSetting.ComercialEmail;
                    string textPlain = $"Nombre del Solicitante = {value[Name]}" + "  \n \n \n" + $"Descripción de la solicitud = {value[Description]}" + " \n \n \n" + $"Telefono de contacto = {value[Phone]}" + " \n \n \n" + $"Correo Electronico de contacto = {value[Mail]}";
                    if (null!=(value[Description])) 
                    {    
 
                        var card = new HeroCard
                        {
                            Title = "Resumen de la información Suministrada: \n \n \n",
                            Subtitle = " Una copia del correo será enviado al correo de contacto suministrado.  \n \n \n",
                            Text = "Nombre del Solicitante ="+ value[Name] +" \n \n \n" +
                                   "Descripción de la solicitud =" + value[Description] + " \n \n \n" +
                                   "Telefono de contacto =" + value[Phone] + " \n \n \n" +
                                   "Correo Electronico de contacto =" + value[Mail] + " \n \n \n" +
                                   "¿Ahora que deseas hacer?",
                            Images = new List<CardImage> { new CardImage("https://www.freepik.es/vector-premium/icono-telefono-boton-miniatura_3220716.htm") },
                        };
                
                        var response = MessageFactory.Attachment(card.ToAttachment());
                        didBotWelcomeUser.DidBotWelcomeUser = false;
                        await SendEmailCardAsync(turnContext, cancellationToken);
                        await turnContext.SendActivityAsync(response, cancellationToken); 
                    }
                    else if (string.IsNullOrEmpty(value[Description]))
                    {
                        var card = new HeroCard
                        {
                            Title = "Resumen de la información Suministrada: \n \n \n",
                            Subtitle = " Una copia del correo será enviado al correo de contacto suministrado.  \n \n \n",
                            Text = "Nombre del Solicitante =" + value[Name] + " \n \n \n" +
                                   "Telefono de contacto =" + value[Phone] + " \n \n \n" +
                                   "Correo Electronico de contacto =" + value[Mail] + " \n \n \n" +
                                   "¿Ahora que deseas hacer?",
                            Images = new List<CardImage> { new CardImage("https://www.freepik.es/vector-premium/icono-telefono-boton-miniatura_3220716.htm") },
                        };

                        var response = MessageFactory.Attachment(card.ToAttachment());

                        await SendEmailCardAsync(turnContext, cancellationToken);
                        await turnContext.SendActivityAsync(response, cancellationToken);
                    };
                    var EmailAttachments = Attachments;
                    await SendIntroCardAsync(turnContext, cancellationToken);
                    string eMail = value[Mail];
                    string description = value[Description];

                    List<string> ToMailing = new List<string> { eMail };
                    if (null != (value[Description])) { ToMailing.Add("info@pevaar.com"); }
                       
                    else if (string.IsNullOrEmpty(value[Description])){ToMailing.Add("recruiting@pevaarmails.com"); }
                        

                    foreach (var mailto in ToMailing) {

                        var request = new MailRequest {

                            ToEmail = mailto,
                            Subject = "Cotización",
                            Name = value[Name],
                            Email = value[Mail],
                            Description = value[Description],
                            Phone = value[Phone],
                            BotAttachments = Attachments
                        };
                        if (string.IsNullOrEmpty(description)) { request.Subject = "Trabaje con nosotros"; };
                        await mailService.SendEmailAsync(request);
                        didBotWelcomeUser.DidBotWelcomeUser = false;
                    } 
             
                };

                //else if (turnContext.Responded == true) 
                //    {

                //    turnContext.Activity.Text = ;
                //};
            }
            else//if (turnContext.Activity.Value == "Si, deseo finalizar la conversación")
            {
                // This example hardcodes specific utterances. You should use LUIS or QnA for more advance language understanding.

                
                if (turnContext.Activity.Text != null) 
                {
                    

                    if (turnContext.Responded==false && turnContext.TurnState.Count == 6) //inConversation == 1) 
                    {
                        
                        var recognizerResult = await _botServices.Dispatch.RecognizeAsync(turnContext, cancellationToken);
                        var topIntent = recognizerResult.GetTopScoringIntent();
                        if (topIntent.score >= 0.7) {
                            switch (topIntent.intent)
                            {
                                case "Cotización":
                                    await turnContext.SendActivityAsync(MessageFactory.Text($"Te enviaré un formulario para que puedas exponer los requerimentos del proyecto."), cancellationToken);
                                    await FormActivityAsync(turnContext, cancellationToken, 0);
                                    break;
                                case "Horarios":
                                    await turnContext.SendActivityAsync(MessageFactory.Text($"Será un gusto conocerte y atenderte personalmente."), cancellationToken);
                                    didBotWelcomeUser.DidBotWelcomeUser = false;
                                    await SendHorarCardAsync(turnContext, cancellationToken);
                                    break;
                                case "Misión":
                                    await turnContext.SendActivityAsync(MessageFactory.Text($"Es una pregunta muy interesante."), cancellationToken);
                                    turnContext.Activity.Text = "¿Quienes Somos?";
                                    break;
                                case "Reclutamiento":
                                    await turnContext.SendActivityAsync(MessageFactory.Text($"Que emoción que quieras hacer parte de nuestro equipo!!!"), cancellationToken);
                                    RecruitingSteps(turnContext, cancellationToken);
                                    await FormActivityAsync(turnContext, cancellationToken, 1);
                                    break;
                                case "Salir":
                                    await SendByeCardAsync(turnContext, cancellationToken);
                                    break;
                                case "Menú_principal":
                                    turnContext.Activity.Text = "ir al menú principal";
                                    break;
                                case "Telefonos":
                                    await turnContext.SendActivityAsync(MessageFactory.Text($"Será un gusto atenderte."), cancellationToken);
                                    didBotWelcomeUser.DidBotWelcomeUser = false;
                                    await SendPhoneCardAsync(turnContext, cancellationToken);
                                    break;
                                case "Visión":
                                    await turnContext.SendActivityAsync(MessageFactory.Text($"Es una pregunta muy interesante."), cancellationToken);
                                    turnContext.Activity.Text = "Visión";
                                    break;
                                default:
                                    await turnContext.SendActivityAsync(MessageFactory.Text($"Espero una de estas opciones pueda ayudarte."), cancellationToken);
                                    didBotWelcomeUser.DidBotWelcomeUser = false;
                                    await SendIntroCardAsync(turnContext, cancellationToken);
                                    break;
                            }
                        }
                        
                    }
                    
                    
                    var text = turnContext.Activity.Text.ToLowerInvariant();
                    switch (text)
                    {
                        case "¿quienes somos?":
                        case "visión":
                        case "nuestros activos":
                        case "¿que hacemos?":
                            await SendQnACardAsync(turnContext, cancellationToken);
                            break;
                        case "si, deseo finalizar la conversación":
                            await turnContext.SendActivityAsync($"Ha sido un placer atenderte, Hasta pronto!!!.", cancellationToken: cancellationToken);
                            await SmileCardAsync(turnContext, cancellationToken); 
                            break;
                        case "ir al menú principal":
                        case "no, deseo volver al menú principal.":
                            didBotWelcomeUser.DidBotWelcomeUser = false;
                            await SendIntroCardAsync(turnContext, cancellationToken);
                            turnContext.Activity.Text = "";
                            break;
                        case "horario de atención":
                            didBotWelcomeUser.DidBotWelcomeUser = false;
                            await SendHorarCardAsync(turnContext, cancellationToken);
                            break;
                        case "telefonos":
                            didBotWelcomeUser.DidBotWelcomeUser = false;
                            await SendPhoneCardAsync(turnContext, cancellationToken);
                            break;
                        case "seguir conociendo pevaar":
                            await SendInfGenCardAsync(turnContext, cancellationToken);
                            didBotWelcomeUser.DidBotWelcomeUser = false;
                            break;

                    }

                    
                }
                if (turnContext.Activity.Text != null && turnContext.TurnState.Count == 6 && turnContext.Responded == false) 
                {
                    await GetName(turnContext, cancellationToken);
                    inConversation=1;
                }
            }
            // Save any state changes.
            await _userState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
        }

        private async Task RecruitingSteps(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var card = new HeroCard
            {
                Title = "Bien, Iniciemos tu proceso de postulación!!!",
                Subtitle = "El proceso consta de dos pasos, pero antes de iniciar debes tener tu  \n" + "Hoja de Vida actualizada.",
                Text = @"1. Sube tu hoja de vida como anexo haciendo click en el icono del clip en la parte" + " \n  inferior izquierda y selecciona tu archivo.  \n \n \n",
                Images = new List<CardImage>() { new CardImage("https://i.picasion.com/pic90/8958dd1ff9761598e7198b38f3b26c19.gif") },
               
            };
           
            var response = MessageFactory.Attachment(card.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);
            while (null == turnContext.Activity.Attachments)
            { if (null != turnContext.Activity.Attachments) { Attachments = turnContext.Activity.Attachments; } };
        }

        private static async Task SendIntroCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
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
                    new CardAction(ActionTypes.MessageBack, "Finalizar Conversación", null, "Finalizar Conversación", "Finalizar Conversación", "Finalizar Conversación"),
                }
            };

            var response2 = MessageFactory.Attachment(newcard.ToAttachment());
            await turnContext.SendActivityAsync(response2, cancellationToken);
        }

        private static async Task SendByeCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
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

        private static async Task SendPhoneCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
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

        private static async Task SendHorarCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
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

        private static async Task SendKnowlCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
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

        protected async Task FormActivityAsync(ITurnContext turnContext, CancellationToken cancellationToken, int r)
        {
            var cardAttachment = CreateAdaptiveCardAttachment(_cards[r]);
            var txt = turnContext.Activity.Text;
            dynamic val = turnContext.Activity.Value;
            await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment), cancellationToken);
        }

        private async Task validationsAssert(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            List<string> FailValid = new List<string>();
            dynamic value = turnContext.Activity.Value;
            string EMail =  value[Mail] ;
            string name =  value[Name];
            string phone = value[Phone];

            bool IsValidEmail(string EMail)
            {
                return Regex.IsMatch(EMail, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            }
            if (!IsValidEmail(EMail)) { FailValid.Add($"Por favor verifica que el correo ingresado sea válido."); };

            bool IsValidName(string name)
            {
                return Regex.IsMatch(name, @"^[\p{L}'][ \p{L}'-]*[\p{L}]$");
            }
            if (!IsValidName(name)) { FailValid.Add($"Por favor verifica que tu nombre este correctamente escrito."); };

            bool IsValidPhone(string Phone)
            {
                return Regex.IsMatch(phone, @"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$"); //((?:\(?[2-9](?(?=1)1[02-9]|(?(?=0)0[1-9]|\d{2}))\)?\D{0,3})(?:\(?[2-9](?(?=1)1[02-9]|\d{2})\)?\D{0,3})\d{4})
            }
            if (!IsValidPhone(phone)) { FailValid.Add($"Por favor verifica que el numero de contacto ingresado sea válido."); };

            foreach (string Fail in FailValid) {
            var reply = MessageFactory.Text(Fail);
            await turnContext.SendActivityAsync(reply, cancellationToken); }
        }

        private static Attachment CreateAdaptiveCardAttachment(string filePath)
        {
            var adaptiveCardJson = File.ReadAllText(filePath);
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
        }

        private static async Task SendInfGenCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
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

        private static async Task SendQnACardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
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

        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(turnContext, () => new UserProfile());
            ConversationData conversationData = await _stateService.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData());
            if (!string.IsNullOrEmpty(userProfile.Name))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(String.Format(" Hola {0}. Como puedo ayudarte ahora?", userProfile.Name)), cancellationToken);
            }
            else
            {
                if (conversationData.PromptedUserForName)
                {
                    // Set the name to what the User provided
                    userProfile.Name = turnContext.Activity.Text?.Trim();
                    // Acknowledge that we got their name
                    await turnContext.SendActivityAsync(MessageFactory.Text(String.Format("Encantada de conocerte {0}. Como puedo ayudarte?", userProfile.Name)), cancellationToken);
                    // Reset the flag to allow the bot to go through the cycle again
                    conversationData.PromptedUserForName = false;
                    inConversation = 1;

                }
                else
                {
                    // Prompt the user for their name
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hola de nuevo, Bienvenido a Pevaar Software Factory, mi nombre es PeBot y seré tu asistente virtual; Cuál es tu nombre?"), cancellationToken);
                    // Set the flag to true, so we don´t prompt in the next turn
                    conversationData.PromptedUserForName = true;
                }
                //save any state changes that might have ocurred during the turn
                await _stateService.UserProfileAccessor.SetAsync(turnContext, userProfile);
                await _stateService.ConversationDataAccessor.SetAsync(turnContext, conversationData);

                await _stateService.UserState.SaveChangesAsync(turnContext);
                await _stateService.ConversationState.SaveChangesAsync(turnContext);

            }
        }


    }
}
