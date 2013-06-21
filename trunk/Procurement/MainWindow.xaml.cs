﻿using System.Windows;
using System.Windows.Input;
using Procurement.ViewModel;
using Procurement.Controls;

namespace Procurement
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Title = ApplicationState.Version;
            this.DataContext = new ScreenController(this);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(MainWindow_MouseLeftButtonDown);
        }

        void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ItemDisplay.ClosePopups();
            DragMove();
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}