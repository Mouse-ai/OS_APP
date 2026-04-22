using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using FCFS_Lab.Models;
using System.Collections.Generic;

namespace FCFS_Lab.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Process> Queue { get; } = new();
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    public ObservableCollection<Process> Processes { get; } = new();
    public ObservableCollection<GanttBlock> GanttBlocks { get; } = new();

    private string _avgWaitTime = "—";
    public string AvgWaitTime { get => _avgWaitTime; set => SetField(ref _avgWaitTime, value); }

    private string _avgTotalTime = "—";
    public string AvgTotalTime { get => _avgTotalTime; set => SetField(ref _avgTotalTime, value); }

    public void CalculateFCFS()
    {
        if (Processes.Count == 0) return;

        int currentTime = 0;
        GanttBlocks.Clear();

        foreach (var proc in Processes)
        {
            proc.WaitingTime = currentTime;
            currentTime += proc.BurstTime;
            proc.TurnaroundTime = currentTime;

            var color = proc.Id switch
            {
                "p0" => Color.Parse("#4A90E2"),
                "p1" => Color.Parse("#50C878"),
                "p2" => Color.Parse("#E25B5B"),
                _ => Color.Parse("#9B9B9B")
            };

            GanttBlocks.Add(new GanttBlock
            {
                ProcessId = proc.Id,
                BurstTime = proc.BurstTime,
                BlockColor = new SolidColorBrush(color)
            });
        }

        var avgWait = Processes.Average(p => p.WaitingTime);
        var avgTotal = Processes.Average(p => p.TurnaroundTime);

        AvgWaitTime = $"Среднее время ожидания: {avgWait:F2}";
        AvgTotalTime = $"Среднее полное время: {avgTotal:F2}";
    }

    public void LoadTest1()
    {
        Processes.Clear();
        Processes.Add(new Process { Id = "p0", BurstTime = 13 });
        Processes.Add(new Process { Id = "p1", BurstTime = 4 });
        Processes.Add(new Process { Id = "p2", BurstTime = 1 });
        CalculateFCFS();
    }

    public void LoadTest2()
    {
        Processes.Clear();
        Processes.Add(new Process { Id = "p2", BurstTime = 1 });
        Processes.Add(new Process { Id = "p1", BurstTime = 4 });
        Processes.Add(new Process { Id = "p0", BurstTime = 13 });
        CalculateFCFS();
    }

    public void ClearAll()
    {
        Processes.Clear();
        GanttBlocks.Clear();
        AvgWaitTime = "—";
        AvgTotalTime = "—";
    }
}

public class GanttBlock
{
    public string ProcessId { get; set; } = string.Empty;
    public int BurstTime { get; set; }
    public IBrush BlockColor { get; set; } = Brushes.Gray;
}