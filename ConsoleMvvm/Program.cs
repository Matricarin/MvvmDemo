using System.Timers;
using ST = System.Timers;

namespace ConsoleMvvm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var view = new View(new ViewModel(new Model()));
            view.Show();
        }
    }
    /// <summary>
    /// <remarks>Класс Model для хранения данных.</remarks>
    /// </summary>
    internal class Model
    {
        /// <summary>
        /// <remarks>Хранит счетчик времени.</remarks>
        /// </summary>
        private ST.Timer _timer;
        /// <summary>
        /// <remarks>Событие для оповещения об изменении времени.</remarks>
        /// </summary>
        public event Action<DateTime> TimeChanged;
        /// <summary>
        /// <remarks>В конструкторе создается экземпляр таймера.
        /// Подписываемся на изменение таймера.
        /// Запускаем таймер.</remarks>
        /// </summary>
        public Model()
        {
            _timer = new ST.Timer(1000);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }
        /// <summary>
        /// Метод изменяющий значение таймера.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
            => TimeChanged?.Invoke(e.SignalTime);
    }

    internal class ViewModel
    {
        public ViewModel(Model model)
        {
            
        }
    }

    internal class View
    {
        public View(ViewModel viewModel)
        {

        }

        public void Show()
        {

        }
    }
}