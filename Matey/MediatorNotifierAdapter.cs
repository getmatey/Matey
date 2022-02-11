namespace Matey
{
    using Common;

    internal class MediatorNotifierAdapter : INotifier
    {
        public MediatR.IMediator mediator;

        public MediatorNotifierAdapter(MediatR.IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task NotifyAsync<TNotification>(TNotification message) where TNotification : INotification
        {
            await mediator.Publish(message).ConfigureAwait(false);
        }
    }
}
