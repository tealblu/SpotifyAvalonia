﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace SpotivyAvalonia.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
#pragma warning disable CA1822 // Mark members as static
        public string Greeting => "Welcome to Avalonia!";

        /// <summary>
        /// Gets a collection of <see cref="ToDoItem"/> which allows adding and removing items
        /// </summary>
        public ObservableCollection<ToDoItemViewModel> ToDoItems { get; } = new ObservableCollection<ToDoItemViewModel>();

        /// <summary>
        /// Gets or set the content for new Items to add. If this string is not empty, the AddItemCommand will be enabled automatically
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddItemCommand))] // This attribute will invalidate the command each time this property changes
        private string? _newItemContent;

        /// <summary>
        /// Returns if a new Item can be added. We require to have the NewItem some Text
        /// </summary>
        private bool CanAddItem() => !string.IsNullOrWhiteSpace(NewItemContent);

        /// <summary>
        /// This command is used to add a new Item to the List
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanAddItem))]
        private void AddItem()
        {
            // Add a new item to the list
            ToDoItems.Add(new ToDoItemViewModel() { Content = NewItemContent });

            // reset the NewItemContent
            NewItemContent = null;
        }

        /// <summary>
        /// Removes the given Item from the list
        /// </summary>
        /// <param name="item">the item to remove</param>
        [RelayCommand]
        private void RemoveItem(ToDoItemViewModel item)
        {
            // Remove the given item from the list
            ToDoItems.Remove(item);
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
