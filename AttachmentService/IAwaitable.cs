namespace WelcomeUser.AttachmentService
{
    public interface IAwaitable<out T>
    {
        public IAwaitable<T>
            GetAwaiter();
            //public Microsoft.Bot.Builder.Internals.Fibers.IAwaiter<out T> 
        
    }
}