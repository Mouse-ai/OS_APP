using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Layout;
using FCFS_Lab.Models;
using System.Collections.ObjectModel;
using FCFS_Lab.ViewModels;

namespace FCFS_Lab.Views;

public partial class MainWindow : Window
{
    public ObservableCollection<Process> Queue { get; } = [];

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void AddProcess_Click(object? sender, RoutedEventArgs e)
    {
        string id = NewIdBox.Text?.Trim();
        string burstStr = NewBurstBox.Text?.Trim();

        if (string.IsNullOrEmpty(id))
        {
            NewIdBox.BorderBrush = new SolidColorBrush(Colors.Red);
            return;
        }

        if (!int.TryParse(burstStr, out int burst) || burst <= 0)
        {
            NewBurstBox.BorderBrush = new SolidColorBrush(Colors.Red);
            return;
        }

        Queue.Add(new Process { Id = id, BurstTime = burst });
        
        NewIdBox.BorderBrush = Brushes.Gray;
        NewBurstBox.BorderBrush = Brushes.Gray;
        NewBurstBox.Text = "";
        
        RunAlgorithm();
    }

    private void Calculate_Click(object? sender, RoutedEventArgs e) => RunAlgorithm();

    private void Test1_Click(object? sender, RoutedEventArgs e)
    {
        Queue.Clear();
        Queue.Add(new Process { Id = "p0", BurstTime = 13 });
        Queue.Add(new Process { Id = "p1", BurstTime = 4 });
        Queue.Add(new Process { Id = "p2", BurstTime = 1 });
        RunAlgorithm();
    }

    private void Test2_Click(object? sender, RoutedEventArgs e)
    {
        Queue.Clear();
        Queue.Add(new Process { Id = "p2", BurstTime = 1 });
        Queue.Add(new Process { Id = "p1", BurstTime = 4 });
        Queue.Add(new Process { Id = "p0", BurstTime = 13 });
        RunAlgorithm();
    }

    private void Clear_Click(object? sender, RoutedEventArgs e)
    {
        Queue.Clear();
        AvgWaitText.Text = "Среднее ожидание: —";
        AvgTotalText.Text = "Среднее полное: —";
        GanttPanel.Children.Clear();
    }

    private void RunAlgorithm()
    {
        if (Queue.Count == 0) return;

        int currentTime = 0;
        foreach (ref var p in Queue.ToArray().AsSpan())
        {
            p.WaitingTime = currentTime;
            currentTime += p.BurstTime;
            p.TurnaroundTime = currentTime;
        }

        var avgWait = Queue.Average(p => p.WaitingTime);
        var avgTotal = Queue.Average(p => p.TurnaroundTime);
        AvgWaitText.Text = $"Среднее ожидание: {avgWait:F2}";
        AvgTotalText.Text = $"Среднее полное: {avgTotal:F2}";
        
        ProcessesGrid.ItemsSource = null;
        ProcessesGrid.ItemsSource = Queue;
        ProcessesGrid.UpdateLayout();
        DrawGanttChart();
    }

    private void DrawGanttChart()
    {
        GanttPanel.Children.Clear();
        
        foreach (var p in Queue)
        {
            var width = Math.Max(p.BurstTime * 15, 50);
            
            var color = p.Id switch
            {
                "p0" => Color.Parse("#4A90E2"),
                "p1" => Color.Parse("#50C878"),
                "p2" => Color.Parse("#E25B5B"),
                _ => Color.Parse("#9B59B6")
            };

            var border = new Border
            {
                Width = width,
                Background = new SolidColorBrush(color),
                Margin = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Child = new TextBlock
                {
                    Text = $"{p.Id}\n{p.BurstTime}",
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.White,
                    FontWeight = FontWeight.Bold
                }
            };
            
            GanttPanel.Children.Add(border);
        }
    }
}