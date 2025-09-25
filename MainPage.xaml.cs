using Assignment_12._3._2.Data;
using Assignment_12._3._2.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Assignment_12._3._2
{
    public partial class MainPage : ContentPage
    {
        private readonly TaskDataContext _context;
        public ObservableCollection<TaskToDo> Tasks { get; set; }

        public MainPage(TaskDataContext context)
        {
            InitializeComponent();
            _context = context;
            Tasks = new();
            BindingContext = this;
            LoadTasks();
        }

        private async void LoadTasks()
        {
            try
            {
                // Check that database exists: if not, create it
                await _context.Database.EnsureCreatedAsync();

                // Move all entries into a temp list
                var tasks = await _context.Tasks.ToListAsync();

                // Copy from temp list to Tasks list
                foreach (var task in tasks)
                {
                    Tasks.Add(task);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txtTask.Text))
            {
                // Create new TaskToDo object
                var newTask = new TaskToDo { TaskDescription = txtTask.Text };
                
                // Add task to database
                _context.Tasks.Add(newTask);

                // Save database
                await _context.SaveChangesAsync();
                
                // Add task to Tasks list
                Tasks.Add(newTask);

                // Clear textbox
                txtTask.Text = String.Empty;
            }
        }

        private async void OnEditTaskClicked(object sender, EventArgs e)
        {
            // If the sender is a button and its CommandParameter is a TaskToDo object
            if (sender is Button button && button.CommandParameter is TaskToDo task)
            {
                // Display prompt asking the user to enter a new task description, giving the original description as the initial value
                string newTask = await DisplayPromptAsync("Edit Task", "Enter new task description:", initialValue: task.TaskDescription);

                // If the newTask string is not empty and not the same as the original task description
                if (!String.IsNullOrWhiteSpace(newTask) && newTask != task.TaskDescription)
                {
                    try
                    {
                        task.TaskDescription = newTask;     // Replace task description
                        _context.Tasks.Update(task);        // Update the database
                        await _context.SaveChangesAsync();  // Save database

                        // Refresh the ObservableCollection
                        var index = Tasks.IndexOf(task);
                        if (index >= 0)
                        {
                            Tasks[index] = task;
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.Message, "OK");
                    }
                }
            }
        }

        private async void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            // If the sender is a button and its CommandParameter is a TaskToDo object
            if (sender is Button button && button.CommandParameter is TaskToDo task)
            {
                bool confirm = await DisplayAlert("Confirm Delete", $"Confirm deletion of task: {task.TaskDescription}?", "Yes", "No");

                // If "No", do nothing
                if (!confirm) return;

                // If "Yes"
                try
                {
                    _context.Tasks.Remove(task);        // Remove task from database
                    await _context.SaveChangesAsync();  // Save database
                    Tasks.Remove(task);                 // Remove task from Tasks list
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }
    }
}
