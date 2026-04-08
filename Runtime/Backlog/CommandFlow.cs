/*using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public interface ICommand
    {
        UniTask Execute(CancellationToken token = default);
    }

public interface ICommandFlow : ICommand
    {
        int CurrentCommandIndex { get; }
        int CommandCount { get; }
        ICommandFlow Add<TCommand>() where TCommand : ICommand;
        ICommandFlow AddParallel<TCommand>() where TCommand : ICommand;
        ICommandFlow AddFlow(Action<ICommandFlow> configureFlow = null);
        ICommandFlow AddParallelFlow(Action<ICommandFlow> configureFlow = null);
    }

public class InitializationCommand<T> : ICommand where T : IInitialize
    {
        private readonly T _target;

        public InitializationCommand(T target)
        {
            _target = target;
        }

        public UniTask Execute(CancellationToken token = default)
        {
            _target?.Initialize();
            return UniTask.CompletedTask;
        }
    }

public class CommandFlow : ICommandFlow, IDisposable
    {
        private readonly IInstantiator _instantiator;
        private readonly List<(ICommand command, bool waitIt)> _commands = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        
        public int CurrentCommandIndex { get; private set; }
        public int CommandCount => _commands.Count;
        private bool _isExecuting;
        protected readonly Logger logger;

        public CommandFlow(IInstantiator instantiator, Logger logger)
        {
            _instantiator = instantiator;
            this.logger = logger;
        }
        
        public ICommandFlow Add<TCommand>() where TCommand : ICommand
        {
            var command = _instantiator.Instantiate<TCommand>();
            AddCommandInternal(command, true);
            return this;
        }
        
        public ICommandFlow AddParallel<TCommand>() where TCommand : ICommand
        {
            var command = _instantiator.Instantiate<TCommand>();
            AddCommandInternal(command, false);
            return this;
        }

        public ICommandFlow AddFlow(Action<ICommandFlow> configureFlow = null)
        {
            var flow = _instantiator.Instantiate<CommandFlow>();
            AddCommandInternal(flow, true);
            configureFlow?.Invoke(flow);
            return this;
        }
        
        public ICommandFlow AddParallelFlow(Action<ICommandFlow> configureFlow = null)
        {
            var flow = _instantiator.Instantiate<CommandFlow>();
            AddCommandInternal(flow, false);
            configureFlow?.Invoke(flow);
            return this;
        }

        private void AddCommandInternal(ICommand command, bool waitIt)
        {
            _commands.Add((command, waitIt));
        }
        
        public virtual async UniTask Execute(CancellationToken token)
        {
            if (_isExecuting) return;
            _isExecuting = true;
            logger.Trace($"Start execution of {CommandCount} commands");
            
            foreach (var entry in _commands)
            {
                if (entry.waitIt)
                {
                    await entry.command.Execute(token);
                }
                else
                {
                    entry.command.Execute(token).Forget();
                }
                
                CurrentCommandIndex++;
                logger.Trace($"{CurrentCommandIndex}/{CommandCount} {entry.command.GetType().Name} <color=green>PASSED</color>");
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }*/