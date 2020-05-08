using System;
using System.Threading;
using System.Threading.Tasks;

namespace Treads
{
    class Program
    {
        //
        // There were two options how to proceed periodical text writing from each tread - task continuation or while loop.
        // Task continuation has been chosen instead while loop.
        // There were two options how to proceed stopping writing text treads after key pressing - ManualResetEvent or CancellationToken.
        // Both options have been shown. With ManualResetEvent approach writing text treads will be halted and with CancellationToken those treads will be stopped.  
        // Locker object has been introduced to avoid simultaneous access WriteText() method by two treads.
        // If there was no such, text color in the beginning would be the same.
        // There is also an option to make treads sleep with task delay functionality, but it was not implemented.
        //

        private static readonly TimeSpan TimeBeforeAppShutDown = TimeSpan.FromMilliseconds(1000);//1 second
        private static readonly TimeSpan FirstTreadInterval = TimeSpan.FromMilliseconds(100);//0.1 second
        private static readonly TimeSpan SecondTreadInterval = TimeSpan.FromMilliseconds(200);//0.2 second
        
        private static readonly object _locker = new object();
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private static readonly CancellationToken _cancellationToken = _cancellationTokenSource.Token;
        //private static readonly ManualResetEvent _resetEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Handler(RunFirstTextWriter);
            Handler(RunSecondTextWriter);

            //_resetEvent.Set();
            Console.ReadKey();
            _cancellationTokenSource.Cancel();
            //_resetEvent.Reset();

            Thread.Sleep(TimeBeforeAppShutDown);
            Environment.Exit(0);
        }

        private static void Handler(Action<Task> tread) =>
            Task.Factory
                .StartNew(() => tread, _cancellationToken)
                .ContinueWith(tread);

        private static void RunFirstTextWriter(Task task = null)
        {
            if (_cancellationToken.IsCancellationRequested)
                return;

            WriteText("Tread 1", ConsoleColor.Cyan);

            Thread.Sleep(FirstTreadInterval);

            //_resetEvent.WaitOne();

            task?.ContinueWith(RunFirstTextWriter);
        }

        private static void RunSecondTextWriter(Task task = null)
        {
            if (_cancellationToken.IsCancellationRequested)
                return;

            WriteText("Tread 2", ConsoleColor.Gray);

            Thread.Sleep(SecondTreadInterval);

            //_resetEvent.WaitOne();

            task?.ContinueWith(RunSecondTextWriter);
        }

        private static void WriteText(string text, ConsoleColor color)
        {
            lock (_locker)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ResetColor();
            }
        }
    }
}
