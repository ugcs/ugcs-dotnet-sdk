using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Commands
{
    /// <summary>
    /// This class implements the Decorator pattern to provide
    /// the ability to handle command execution errors.
    /// </summary>
    public sealed class CommandWithErrorHandling : ICommand, IDisposable
    {
        public delegate void CommandErrorHandler(ICommand source, Exception error, out bool wasErrorHandled);


        private readonly ICommand _baseCommand;
        private readonly CommandErrorHandler _errrorHandler;


        public CommandWithErrorHandling(ICommand baseCommand, CommandErrorHandler errorHandler)
        {
            _baseCommand = baseCommand ?? throw new ArgumentNullException();
            _errrorHandler = errorHandler ?? throw new ArgumentNullException();

            baseCommand.CanExecuteChanged += baseCommand_CanExecuteChanged;
        }

        private void baseCommand_CanExecuteChanged(object sender, EventArgs e)
        {
            CanExecuteChanged?.Invoke(sender, e);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _baseCommand.CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            try
            {
                _baseCommand.Execute(parameter);
            }
            catch (Exception err)
            {
                _errrorHandler(_baseCommand, err, out bool wasErrorHandled);
                if (!wasErrorHandled)
                    throw;
            }
        }

        public void Dispose()
        {
            _baseCommand.CanExecuteChanged -= baseCommand_CanExecuteChanged;
        }
    }
}
