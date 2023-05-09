using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MFDictionary.MVVM.View
{
    /// <summary>
    /// Interaction logic for DictionaryView.xaml
    /// </summary>
    public partial class DictionaryView : System.Windows.Controls.UserControl
    {
        private const string POSITION_FILE = "add_button_position.txt";

        private bool _isMoving;
        private Point? _buttonPosition;
        private double deltaX;
        private double deltaY;
        private TranslateTransform _currentTT;

        public DictionaryView()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(POSITION_FILE))
            {
                string[] position = File.ReadAllLines(POSITION_FILE);
                if (position.Length == 2 && double.TryParse(position[0], out double x) && double.TryParse(position[1], out double y))
                {
                    AddButton.RenderTransform = new TranslateTransform(x, y);

                    if (_buttonPosition == null)
                        _buttonPosition = new Point(x, y);
                }
            }
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            TranslateTransform transform = AddButton.RenderTransform as TranslateTransform;
            string positionString = $"{transform.X}\n{transform.Y}";
            File.WriteAllText(POSITION_FILE, positionString);
        }

        private void AddButton_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_buttonPosition == null)
                _buttonPosition = AddButton.TransformToAncestor(MainGrid).Transform(new Point(0, 0));
            var mousePosition = Mouse.GetPosition(MainGrid);
            deltaX = mousePosition.X - _buttonPosition.Value.X;
            deltaY = mousePosition.Y - _buttonPosition.Value.Y;
            _isMoving = true;
        }

        private void AddButton_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _currentTT = AddButton.RenderTransform as TranslateTransform;
            _buttonPosition = new Point(_currentTT.X, _currentTT.Y);
            _isMoving = false;
        }

        private void AddButton_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMoving) return;

            var mousePoint = Mouse.GetPosition(MainGrid);

            if (_buttonPosition == null)
                _buttonPosition = AddButton.TransformToAncestor(MainGrid).Transform(new Point(0, 0));

            var offsetX = mousePoint.X - deltaX;
            var offsetY = mousePoint.Y - deltaY;

            this.AddButton.RenderTransform = new TranslateTransform(offsetX, offsetY);

            _buttonPosition = AddButton.TransformToAncestor(MainGrid).Transform(new Point(0, 0));
        }
    }
}
