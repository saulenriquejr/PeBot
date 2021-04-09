
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow.Bindings;
using WelcomeUser.Bots;
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
        private const string Cedula = "id_cedula";
        private const string Cargo = "id_cargo";
        private const string FechaI = "id_dateI";
        private const string Formulario = "form";
        private const string Lideres = "Lideres";
        private const string FechaF = "id_dateF";

        private readonly BotState _userState;
        private readonly AppSettings _appSetting;
        public readonly IMailService mailService;
        private readonly StateService _stateService;
        private readonly BotServices _botServices;

        public static bool DidBotWelcomeUser { get; private set; }

        // Initializes a new instance of the "WelcomeUserBot" class.

        public WelcomeUserBot(UserState userState, IMailService mailService, AppSettings appSetting, StateService stateService, BotServices botServices)//, IAppServices appServices
        { 
            _userState = userState;
            this.mailService = mailService;
            _appSetting = appSetting;
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));
            _botServices = botServices ?? throw new System.ArgumentNullException(nameof(stateService)); 
        
        }

        readonly string[] _cards =
        {
            Path.Combine(".", "Resources", "FormCotizar.json"),
            Path.Combine(".", "Resources", "FormTrabajaConNosotr.json"),
            Path.Combine(".", "Resources", "FormCertificado.json"),
            Path.Combine(".", "Resources", "FormInasistenciaJustif.json"),
        };
        public int inConversation;

        public static IFormFile remoteFileUrl { get; private set; }
        public static IList<Attachment> Attachments { get; private set; }

        Saludo saludo = new Saludo();
        Despedida despedida = new Despedida();
        Emociones emociones = new Emociones();
        MenuPevaar menuPevaar = new MenuPevaar();
        QnA qna = new QnA();
        Información información = new Información();

// Greet when users are added to the conversation. Note that all channels do not send the conversation update activity. If you find that this bot works in the emulator, but does not in
// another channel the reason is most likely that the channel does not send this activity.

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    
                    await saludo.SendHiCardAsync(turnContext, cancellationToken);
                    await saludo.SendIntroCardAsync(turnContext, cancellationToken);
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
                            await información.SendInfGenCardAsync(turnContext, cancellationToken);
                            break;
                        case "contactenos":
                            await información.SendKnowlCardAsync(turnContext, cancellationToken);
                            break;
                        case "soy pevaar":
                            ////var magicCode = await OauthCardAsync(turnContext, cancellationToken);
                            
                            //var token = await GetUserTokenAsync(turnContext, cancellationToken).ConfigureAwait(false);
                            //if (token != null)"OauthPeBotConnection", magicCode
                            //{
                            //    // use the token to do exciting things!
                            //}
                            //else
                            //{
                                // If Bot Service does not have a token, send an OAuth card to sign in
                                //var magicCode = 
                            await OauthCardAsync(turnContext, cancellationToken);
                            //}
                            //
                            break;
                        case "conversemos un rato":
                            inConversation = 1;
                            await GetName(turnContext, cancellationToken);
                            break;
                        case "finalizar conversación":
                            await despedida.SendByeCardAsync(turnContext, cancellationToken);
                            break;
                        case "cotizaciones":
                            r = 0;
                            await FormActivityAsync(turnContext, cancellationToken, r);
                            break;
                        case "trabaje con nosotros":
                            r = 1;
                            await RecruitingSteps(turnContext, cancellationToken);
                            break;
                        case "certificado":
                            r = 2;
                            await FormActivityAsync(turnContext, cancellationToken, r);
                            //await menuPevaar.GenerarCertificado(turnContext, cancellationToken);
                            break;
                        case "ausencias":
                            r = 3;
                            await FormActivityAsync(turnContext, cancellationToken, r);
                            //await menuPevaar.PresentarAusencia(turnContext, cancellationToken);
                            break;
                        case "inspección equipo":
                            await menuPevaar.InspeccionEquipo(turnContext, cancellationToken);
                            break;
                        case "volver al menú principal":
                            await turnContext.SendActivityAsync(MessageFactory.Text($"Primero cerraremos la sesión y podras elegir que hacer ahora."), cancellationToken);
                            var botAdapter = (BotFrameworkAdapter)turnContext.Adapter;
                            await botAdapter.SignOutUserAsync(turnContext, "OauthPeBotConnection", null, cancellationToken);
                            didBotWelcomeUser.DidBotWelcomeUser = false;
                            await saludo.SendIntroCardAsync(turnContext, cancellationToken);
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
                    string form = value[Formulario];
                    string cedula = value[Cedula];
                    string cargo = value[Cargo];
                    string nombre = value[Name];
                    string fechaInicio = value[FechaI];
                    string fechaFinal = value[FechaF];
                    string descripcion = value[Description];
                    string lideres = value[Lideres];

                    if ("Certificado" == (form)) 
                    { 
                        await menuPevaar.GenerarCertificado(turnContext, cancellationToken, cedula, cargo, nombre, fechaInicio, fechaFinal, descripcion ); 
                    };
                    if ("Inasistencia" == (form)) 
                    { 
                        await menuPevaar.PresentarAusencia(turnContext, cancellationToken, nombre, fechaInicio, fechaFinal, descripcion, lideres); 
                    };
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
                        await emociones.SendEmailCardAsync(turnContext, cancellationToken);
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

                        await emociones.SendEmailCardAsync(turnContext, cancellationToken);
                        await turnContext.SendActivityAsync(response, cancellationToken);
                    };
                    var EmailAttachments = Attachments;
                    await saludo.SendIntroCardAsync(turnContext, cancellationToken);
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
                                    await información.SendHorarCardAsync(turnContext, cancellationToken);
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
                                    await despedida.SendByeCardAsync(turnContext, cancellationToken);
                                    break;
                                case "Menú_principal":
                                    turnContext.Activity.Text = "ir al menú principal";
                                    break;
                                case "Telefonos":
                                    await turnContext.SendActivityAsync(MessageFactory.Text($"Será un gusto atenderte."), cancellationToken);
                                    didBotWelcomeUser.DidBotWelcomeUser = false;
                                    await información.SendPhoneCardAsync(turnContext, cancellationToken);
                                    break;
                                case "Visión":
                                    await turnContext.SendActivityAsync(MessageFactory.Text($"Es una pregunta muy interesante."), cancellationToken);
                                    turnContext.Activity.Text = "Visión";
                                    break;
                                case "CodigoMagico":
                                    await turnContext.SendActivityAsync(MessageFactory.Text($"Estas autenticado compañero, Que deseas hacer ahora estimado Pevaariano?"), cancellationToken);
                                    await emociones.SmileCardAsync(turnContext, cancellationToken);
                                    turnContext.Activity.Text = "MenuPevaar";
                                    break;
                                default:
                                    await turnContext.SendActivityAsync(MessageFactory.Text($"Espero una de estas opciones pueda ayudarte."), cancellationToken);
                                    didBotWelcomeUser.DidBotWelcomeUser = false;
                                    await saludo.SendIntroCardAsync(turnContext, cancellationToken);
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
                            await qna.SendQnACardAsync(turnContext, cancellationToken);
                            break;
                        case "si, deseo finalizar la conversación":
                            await turnContext.SendActivityAsync($"Ha sido un placer atenderte, Hasta pronto!!!.", cancellationToken: cancellationToken);
                            await emociones.SmileCardAsync(turnContext, cancellationToken); 
                            break;
                        case "ir al menú principal":
                        case "no, deseo volver al menú principal.":
                            didBotWelcomeUser.DidBotWelcomeUser = false;
                            await saludo.SendIntroCardAsync(turnContext, cancellationToken);
                            turnContext.Activity.Text = "";
                            break;
                        case "horario de atención":
                            didBotWelcomeUser.DidBotWelcomeUser = false;
                            await información.SendHorarCardAsync(turnContext, cancellationToken);
                            break;
                        case "telefonos":
                            didBotWelcomeUser.DidBotWelcomeUser = false;
                            await información.SendPhoneCardAsync(turnContext, cancellationToken);
                            break;
                        case "seguir conociendo pevaar":
                            await información.SendInfGenCardAsync(turnContext, cancellationToken);
                            didBotWelcomeUser.DidBotWelcomeUser = false;
                            break;
                        case "soy pevaar":
                            didBotWelcomeUser.DidBotWelcomeUser = false;
                            
                            turnContext.Activity.Text = "Soy PEVAAR";
                            break;
                        case "menupevaar":
                            await menuPevaar.SendPevaarCardAsync(turnContext, cancellationToken);
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

        private static async Task OauthCardAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var oauthCard = new OAuthCard
            {
                Text = "BotFramework OAuth Card",
                ConnectionName = "OauthPeBotConnection",//_appSetting.OAuthConnection Replace with the name of your Azure AD connection.

                Buttons = new List<CardAction> { new CardAction(ActionTypes.Signin, "Sign In", "Pevaar SignIn", "https://login.microsoftonline.com/organizations/oauth2/v2.0/token") },
                
            };
            var response = MessageFactory.Attachment(oauthCard.ToAttachment());
            await turnContext.SendActivityAsync(response, cancellationToken);

            //new OAuthPrompt(
            //nameof(OAuthPrompt),
            //new OAuthPromptSettings
            //{
            //    ConnectionName = "OauthPeBotConnection",
            //    Text = "Please Sign In",
            //    Title = "Sign In",
            //    Timeout = 300000, // User has 5 minutes to login (1000 * 60 * 5)
            //}); 

            //var tokenResponse = (TokenResponse)turnContext;
            //return tokenResponse;

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
        ////private async Task SendOAuthCardAsync(IDialogContext context, Activity activity)
        ////{
        ////    await context.PostAsync($"To do this, you'll first need to sign in.");

        ////    var reply = await context.Activity.CreateOAuthReplyAsync(_connectionName, _signInMessage, _buttonLabel).ConfigureAwait(false);
        ////    await context.PostAsync(reply);

        ////    context.Wait(WaitForToken);
        ////}

        ////public virtual System.Threading.Tasks.Task<Microsoft.Bot.Schema.TokenResponse> GetUserTokenAsync(Microsoft.Bot.Builder.ITurnContext turnContext, string connectionName, string magicCode, System.Threading.CancellationToken cancellationToken = default) { return null; }
        //public Task<TokenResponse> GetUserTokenAsync(ITurnContext turnContext, CancellationToken cancellationToken = default);

    }
}
