using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace SemaphoreTask;

public partial class MainWindow : Window
{
	Thread Cthread;
	public Semaphore semaphore { get; set; }
	public int countSemaphore { get; set; }
	public int count = 0;

	public MainWindow()
	{
		InitializeComponent();
	}

	private void btn_create_Click(object sender, RoutedEventArgs e)
	{
		semaphore = new Semaphore(countSemaphore, countSemaphore, "SEMAPHORE");

		for (int i = 0; i < 3; i++)
		{
			Cthread = new Thread(() => Simulation(semaphore));
			++count;
			Cthread.Name = $"Thread -> {count}";
			Dispatcher.Invoke(() => listbox_created.Items.Add(Cthread));
		}
	}

	private void Simulation(Semaphore semaphore)
	{
		bool st = false;
		while (!st)
		{
			if (semaphore.WaitOne(1))
			{
				try
				{
					var t = Thread.CurrentThread;
					Dispatcher.Invoke(() => listbox_working.Items.Add(t));
					Dispatcher.Invoke(() => listbox_waiting.Items.Remove(t));
					Thread.Sleep(3000);
				}
				finally
				{
					st = true;
					var t = Thread.CurrentThread;

					Dispatcher.Invoke(() =>
					{
						listbox_working.Items.Remove(t);
						listbox_waiting.Items.Remove(t);
					});

					semaphore.Release();
				}
			}
			else
			{
				var t = Thread.CurrentThread;

				Dispatcher.Invoke(() =>
				{
					if (!listbox_waiting.Items.Contains(t)) 
						listbox_waiting.Items.Add(t);
				});
			}
		}
	}

	private void listbox_created_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		try
		{
			var t = listbox_created.SelectedItem as Thread;
			t?.Start();
			listbox_created.Items.Remove(t);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}
	}

	private void nuSemaphore_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		if (nuSemaphore.Value <= 0)
		{
			MessageBox.Show("Value cannot be than less 1...", "", MessageBoxButton.OK, MessageBoxImage.Warning);
			nuSemaphore.Value = 1;
		}
		else
			countSemaphore = Convert.ToInt32(nuSemaphore.Value);
	}
}