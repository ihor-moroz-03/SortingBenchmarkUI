using Algorithms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace SortingBenchmarkUI
{
    public partial class MainWindow : Window
    {
        SortingBenchmark _benchmark;
        DataGridIndexerAdapter _results;

        Regex _textBoxValidator = new(@"^[\d ]*$");

        public MainWindow()
        {
            InitializeComponent();

            _benchmark = new SortingBenchmark
            {
                Sorters = new Dictionary<string, SortingBenchmark.DSorter>
                {
                    ["Bubble"] = Sorting.Bubble,
                    ["Selection"] = Sorting.Selection,
                    ["Shell"] = Sorting.Shell,
                    ["Merge"] = Sorting.Merge,
                    ["Quick"] = Sorting.Quick,
                    ["Counting"] = Sorting.Counting
                }
            };

            _results = new(_benchmark.Sorters.Keys, new[] { "Algorithm" });

            _benchmark.OnNewResult += (obj, args) =>
                _results[args.Sorter, args.ArrSize] = args.ElapsedMilliseconds;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_textBoxValidator.IsMatch(textBox.Text))
            {
                textBox.Clear();
                MessageBox.Show("Please put arr sizes divided by spaces", "Invalid input");
                return;
            }

            if (!int.TryParse(rangeTextBox.Text, out int range))
            {
                rangeTextBox.Clear();
                MessageBox.Show("Please input range as an integer < 2147483647", "Invalid input");
                return;
            }

            string[] sizes = textBox.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            _results.AddColumns(sizes);
            grid.ItemsSource = _results.DataView;
            _benchmark.Run(sizes.Select(s => int.Parse(s)), range);

            (sender as Button).IsEnabled = false;
        }
    }
}
