﻿using System;
using System.Diagnostics;
using System.Windows.Input;

namespace KingsDamageMeter.Controls
{
    /// <summary>
    /// A command whose sole purpose is to 
    /// relay its functionality to other
    /// objects by invoking delegates. The
    /// default return value for the CanExecute
    /// method is 'true'.
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        #region Fields

        private Action<T> _execute;
        private Predicate<T> _canExecute;        

        #endregion // Fields

        #region Constructors

        public RelayCommand()
        {
            
        }

        public void Initialize(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;           
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        #endregion // ICommand Members
    }

    public class ObjectRelayCommand : RelayCommand<object>
    {
        public ObjectRelayCommand()
        {
	        
        }

        public ObjectRelayCommand(Action<object> execute)
            : base(execute, null)
        {
        }

        public ObjectRelayCommand(Action<object> execute, Predicate<object> canExecute)
            : base(execute, canExecute)
        {        
        }
    }
}