namespace FCFS_Lab.Models;

public class Process
{
    public string Id { get; set; } = string.Empty;
    public int BurstTime { get; set; }
    public int WaitingTime { get; set; }
    public int TurnaroundTime { get; set; }
}