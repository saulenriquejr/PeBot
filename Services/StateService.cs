using Microsoft.Bot.Builder;
using System;
using WelcomeUser.Models;

namespace WelcomeUser.Services
{
    public class StateService
    {
        #region Variables

        public ConversationState ConversationState { get; }
        //state Variables
        public UserState UserState { get; }

        //IDs

        public static string UserProfileId { get; } = $"{nameof(StateService)}.UserProfile";

        public static string ConversationDataId { get; } = $"{nameof(StateService)}.ConversationData";

        public static string DialogStateId { get; } = $"{nameof(StateService)}.DialogState";

        //Accessors
        public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }

        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }

        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }

        #endregion
        public StateService(UserState userState, ConversationState conversationState)
        {
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            InitializaAccessors();
        }

        private void InitializaAccessors()
        {
            //Initialize User state
            UserProfileAccessor = UserState.CreateProperty<UserProfile>(UserProfileId);
            DialogStateAccessor = ConversationState.CreateProperty<DialogState>(DialogStateId);
            ConversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationDataId);
        }
    }
}