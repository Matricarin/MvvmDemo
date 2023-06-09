﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using ST = System.Timers;

namespace ConsoleMvvm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ///Возможно реализовать внедрение зависимостей с помощью Autofac
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
    /// <summary>
    /// Класс для связывания Model и View.
    /// </summary>
    internal class ViewModel : INotifyPropertyChanged
    {
        private string _time;
        /// <summary>
        /// Свойство Time соотвествует свойству из View.
        /// </summary>
        public string Time 
        { 
            get => _time; 
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// В конструкторе подписываемся на событие из Model 
        /// для отображения изменений в модели.
        /// </summary>
        /// <param name="model">Передаем в конструктор экземпляр Model.</param>
        public ViewModel(Model model)
        {
            Time = "00:00:00";
            model.TimeChanged += ModelOnTimeChanged;
        }
        /// <summary>
        /// Событие для оповещения об изменения для View
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        /// <summary>
        /// Метод для изменения свойства
        /// </summary>
        /// <param name="propertyName">Имя изменяемого свойства</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Метод для записи текущего времени в свойство Time.
        /// </summary>
        /// <param name="obj">Время на таймере.</param>
        private void ModelOnTimeChanged(DateTime obj)
        {
            Time = obj.ToLongTimeString();
        }
    }
    /// <summary>
    /// Класс для отображения View на консоли.
    /// </summary>
    internal class View
    {
        /// <summary>
        /// Хранит экземпляр ViewModel для связывания.
        /// </summary>
        public object DataContext { get; }
        /// <summary>
        /// Хранит данные из свойства Text.
        /// </summary>
        private string _text;
        /// <summary>
        /// Имитация части UI в консоли.
        /// </summary>
        public string Text 
        { 
            get => _text;
            set
            {
                _text = value;
                Update();
            }
        }
        public View(ViewModel dataContext)
        {
            DataContext = dataContext;
            var binding = new Binding("Time");
            SetBinding(nameof(Text), binding);
        }
        /// <summary>
        /// Данный метод отображает UI в консоль. 
        /// </summary>
        public void Show()
        {
            Update();
            Console.ReadKey();
        }
        /// <summary>
        /// Метод для изменения свойства.
        /// </summary>
        private void Update()
        {
            Console.Clear();
            foreach(var text in Text)
            {
                Console.ForegroundColor = text == ':' ?
                    ConsoleColor.Green :
                    ConsoleColor.Red;
                Console.Write(text);
            }
        }
        /// <summary>
        /// Метод устанавливающий связь между свойствами View и свойствами ViewModel/
        /// </summary>
        /// <param name="dependencyPropertyName">Наименование свойства из View.</param>
        /// <param name="binding">Наименование  свойства из ViewModel, 
        /// полученное с помощью Binding.</param>
        private void SetBinding(string dependencyPropertyName, Binding binding)
        {
            GetProperties(dependencyPropertyName, binding);
            if(DataContext is INotifyPropertyChanged notify)
            {
                notify.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == binding.DataContextPropertyName)
                    {
                        GetProperties(dependencyPropertyName, binding);
                    }
                };
            }
        }
        /// <summary>
        /// Назначение исходного и целевого свойств.
        /// </summary>
        /// <param name="dependencyPropertyName">Наименование свойства из View.</param>
        /// <param name="binding">Наименование  свойства из ViewModel, 
        /// полученное с помощью Binding.</param>
        private void GetProperties(string dependencyPropertyName, Binding binding)
        {
            ///Получаем PropertyInfo из ViewModel.
            var sourceProperty = DataContext.GetType().GetProperty(binding.DataContextPropertyName);
            ///Получаем PropertyInfo is View.
            var targetProperty = this.GetType().GetProperty(dependencyPropertyName);
            ///Назначаем в targetValue значение из свойства наименование, 
            ///которого находится в sourceProperty
            targetProperty.SetValue(this, sourceProperty.GetValue(DataContext));
        }
    }
    /// <summary>
    /// <remarks> Класс для связывания View и ViewModel.</remarks>
    /// </summary>
    internal class Binding
    {
        /// <summary>
        /// Хранит наименование связываемого свойства из ViewModel.
        /// </summary>
        public string DataContextPropertyName { get; }
        public Binding(string dataContextPropertyName)
        {
            DataContextPropertyName = dataContextPropertyName;
        }
    }
}