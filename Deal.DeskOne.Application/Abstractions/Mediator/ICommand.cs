namespace Deal.DeskOne.Application.Abstractions.Mediator
{
    public interface ICommand { }

    public interface ICommand<TResult> : ICommand { }
}
