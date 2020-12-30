using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    // This ASP Controller is created to handle a request. Dependency Injection will provide the Adapter and IBot
    // implementation at runtime. Multiple different IBot implementations running at different endpoints can be
    // achieved by specifying a more specific type for the bot constructor argument.
    
    [Route("api/messages")]
    [AllowAnonymous]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            
            //var conversationUpdateActivity = new Bot.Schema.Activity
            //{
            //    Type = ActivityTypes.ConversationUpdate,
            //    MembersAdded = new List<ChannelAccount>
            //    {
            //        new ChannelAccount { Id = "theUser" },
            //    },
            //    Recipient = new ChannelAccount { Id = "theBot" },
            //};
            //var _adapter = new TestAdapter(Channels.Test);

            //// Act
            //// Send the conversation update activity to the bot.
            //await _adapter.ProcessActivityAsync(conversationUpdateActivity, _bot.OnTurnAsync, CancellationToken.None);
            await _adapter.ProcessAsync(Request, Response, _bot);
        }
    }
}
