using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core
{
    public abstract class BaseController
    {
        protected ControllerState CurrentState = ControllerState.Pending;

        public ControllerState CurrentControllerState => CurrentState;

        public virtual UniTask Run(CancellationToken cancellationToken)
        {
            if (CurrentState == ControllerState.Run)
                throw new InvalidOperationException($"{GetType().Name}: controller already running");

            CurrentState = ControllerState.Run;
            return UniTask.CompletedTask;
        }

        public virtual UniTask Stop()
        {
            if (CurrentState != ControllerState.Run)
                throw new InvalidOperationException($"{GetType().Name}: try to stop not active controller");

            CurrentState = ControllerState.Stop;
            return UniTask.CompletedTask;
        }
    }

    public abstract class BaseController<T> : BaseController where T : class
    {
        public sealed override UniTask Run(CancellationToken cancellationToken)
        {
            return base.Run(cancellationToken);
        }

        public virtual UniTask Run(T data, CancellationToken cancellationToken)
        {
            base.Run(cancellationToken);

            return UniTask.CompletedTask;
        }
    }

    public enum ControllerState
    {
        Pending,
        Run,
        Stop,
        Failed,
        Complete
    }
}