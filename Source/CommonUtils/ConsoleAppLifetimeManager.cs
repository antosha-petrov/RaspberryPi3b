namespace CommonUtils
{
    /// <summary>
    /// Управляет временем жизни консольного приложения.
    /// </summary>
    public sealed class ConsoleAppLifetimeManager : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private bool isDisposed;

        /// <summary>
        /// Инициализирует экземпляр <see cref="ConsoleAppLifetimeManager"/>.
        /// </summary>
        public ConsoleAppLifetimeManager()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Ожидает команду на завершение работы консольного приложения.
        /// </summary>
        /// <returns>
        /// Асинхронная операция, не возвращающая результат.
        /// </returns>
        public async Task WaitForShutdownAsync()
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => ShutDown();

                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    ShutDown();
                    eventArgs.Cancel = true;
                };

                await WaitForCancellationAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Дает команду на завершение работы консольного приложения.
        /// </summary>
        public void ShutDown()
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Освобождает управляемые ресурсы, используемые этим экземпляром <see cref="ConsoleAppLifetimeManager"/>.
        /// </summary>
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            cancellationTokenSource.Dispose();
            isDisposed = true;
        }

        private Task WaitForCancellationAsync()
        {
            var waitForCancellationTcs = new TaskCompletionSource<object>();

            cancellationTokenSource.Token.Register(_ => waitForCancellationTcs.TrySetResult(null), null);

            return waitForCancellationTcs.Task;
        }
    }
}
