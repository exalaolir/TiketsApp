using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TiketsApp.Core.Servises;
using TiketsApp.Models;
using TiketsApp.res;
using TiketsApp.ViewModels.Base;

namespace TiketsApp.ViewModels.Admin
{

    internal class RootCategoryAddVM : CategoryEditVM
    {
        public override ICommand SaveCommand { get; }
        private readonly CategoriesVM _categoriesVM;
        private readonly ObservableCollection<Models.Category>? _collection;

        public RootCategoryAddVM ( CategoriesVM categoriesVM, ObservableCollection<Models.Category>? collecton )
        {
            _categoriesVM = categoriesVM;
            _collection = collecton;

            _validFields = new bool[1];

            SaveCommand = new Command(AddTooRoot);
        }

        private async void AddTooRoot ()
        {
            _categoriesVM.DataLoaded = false;

            await Task.Run(() =>
            {
                var newRoot = new Models.Category()
                {
                    Name = Name,
                    IsBlocked = false,
                };

                using AppContext context = new();

                if (context.Categories.Where(c => c.Name == newRoot.Name).FirstOrDefault() != null)
                {
                    SetValidationResults(false, nameof(Name), Consts.CategoryExistsMsg);
                    _categoriesVM.DataLoaded = true;
                    return;
                }

                context.Categories.Add(newRoot);
                context.SaveChanges();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _collection!.Add(newRoot);
                    _categoriesVM.DataLoaded = true;
                });
            });
        }
    }


    internal class ChildCategoryAddVM : CategoryEditVM
    {
        public override ICommand SaveCommand { get; }
        private readonly CategoriesVM _categoriesVM;
        private readonly ObservableCollection<Models.Category>? _collection;
        private readonly int _parentId;

        public event EventHandler? CategoryAdd;

        public ChildCategoryAddVM ( CategoriesVM categoriesVM, ObservableCollection<Models.Category>? collecton, int parentId )
        {
            _categoriesVM = categoriesVM;
            _collection = collecton;
            _parentId = parentId;

            _validFields = new bool[1];

            SaveCommand = new Command(AddTooRoot);
        }

        private async void AddTooRoot ()
        {
            _categoriesVM.DataLoaded = false;

            await Task.Run(() =>
            {
                var newChild = new Models.Category()
                {
                    Name = Name,
                    IsBlocked = false,
                    ParentId = _parentId,
                };

                using AppContext context = new();

                if (context.Categories.Where(c => c.Name == newChild.Name).FirstOrDefault() != null)
                {
                    SetValidationResults(false, nameof(Name), Consts.CategoryExistsMsg);
                    _categoriesVM.DataLoaded = true;
                    return;
                }

                var parent = context.Categories
                    .Include(c => c.ChildCategories)
                    .FirstOrDefault(c => c.Id == _parentId);


                parent!.ChildCategories.Add(newChild);
                context.SaveChanges();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    CategoryAdd?.Invoke(this, EventArgs.Empty);
                    _categoriesVM.DataLoaded = true;
                });
            });
        }
    }

    internal class CategoryEditingVM : CategoryEditVM
    {
        public override ICommand SaveCommand { get; }
        private readonly CategoriesVM _categoriesVM;
        private readonly ObservableCollection<Models.Category>? _collection;
        private readonly int _id;

        public event EventHandler? CategoryEditing;

        public CategoryEditingVM ( CategoriesVM categoriesVM, ObservableCollection<Models.Category>? collecton, Category category )
        {
            _categoriesVM = categoriesVM;
            _collection = collecton;
            this._name = category.Name;
            _id = category.Id;

            _validFields = new bool[1];
            _validFields[0] = true;

            SaveCommand = new Command(AddTooRoot);
        }

        private async void AddTooRoot ()
        {
            _categoriesVM.DataLoaded = false;

            await Task.Run(() =>
            {

                using AppContext context = new();

                if (context.Categories.Where(c => c.Name == Name).FirstOrDefault() != null)
                {
                    SetValidationResults(false, nameof(Name), Consts.CategoryExistsMsg);
                    _categoriesVM.DataLoaded = true;
                    return;
                }

                context.Categories.Find(_id)!.Name = Name;
                context.SaveChanges();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    CategoryEditing?.Invoke(this, EventArgs.Empty);
                    _categoriesVM.DataLoaded = true;
                });
            });
        }
    }


    internal class ChartVM : ViewModel
    {
        public IEnumerable<ISeries> Series { get; set; }

        public string Title { get; }

        public ChartVM ( Category parent, int currentElId )
        {
            if (parent.Id == currentElId)
                Title = parent.Name;
            else Title = parent.ChildCategories.Find(c => c.Id == currentElId)!.Name;

            Series = parent.ChildCategories.Select(child => new PieSeries<int>
            {
                Name = child.Name,
                Values = new[] { child.ElementsInCategory ?? 0 },
                Pushout = child.Id == currentElId ? 25 : 0,
            }).ToList();
        }
    }

    internal class EmptyChart ( bool isRoot ) : ViewModel
    {
        public string Message => isRoot ? "Дочерние категории не содержат элементов" : "Категория не содержит элементов";
    }

    internal class CategoriesVM : ViewModel
    {
        private readonly Navigation _navigator;
        private readonly Models.Admin? _admin;
        private bool _isDataLoaded;
        private dynamic _currentEditVM;
        private Category? _selectedCategory;
        private bool _canEdit;
        private bool _canAdd;
        private bool _canBlock;
        private bool _canUnblock;
        private dynamic _currentChart;

        public ObservableCollection<Models.Category>? Categories { get; private set; }

        public bool DataLoaded
        {
            get => _isDataLoaded;
            set => this.SetValue(ref _isDataLoaded, value);
        }

        public dynamic CurrentChart
        {
            get => _currentChart;
            set
            {
                SetValue(ref _currentChart, value);
            }
        }

        public bool CanUnblock
        {
            get => _canUnblock;
            set => this.SetValue(ref _canUnblock, value);
        }

        public dynamic CurrentEditVM
        {
            get => _currentEditVM;
            set => this.SetValue(ref _currentEditVM, value);
        }

        public bool CanEdit
        {
            get => _canEdit;
            set => this.SetValue(ref _canEdit, value);
        }

        public bool CanBlock
        {
            get => _canBlock;
            set => this.SetValue(ref _canBlock, value);
        }

        public bool CanAdd
        {
            get => _canAdd;
            set => this.SetValue(ref _canAdd, value);
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                this.SetValue(ref _selectedCategory, value);
                if (value != null) CanEdit = true;
                else CanEdit = false;

                if (value != null && value.Parent == null) CanAdd = true;
                else CanAdd = false;

                if (value != null && !value.IsBlocked)
                {
                    CanBlock = true;
                    CanUnblock = false;
                }
                else if (value != null)
                {
                    CanBlock = false;
                    if (value.Parent == null) CanUnblock = true;
                    else CanUnblock = !value.Parent.IsBlocked;
                }
                else
                {
                    CanBlock = false;
                    CanUnblock = false;
                }

                CurrentChart = value switch
                {
                    null => new Default(),
                    _ when IsRootWithNoChildren(value) => new EmptyChart(true),
                    _ when IsChildWithNoElements(value) => new EmptyChart(false),
                    _ => new ChartVM(value.Parent ?? value, value.Id)
                };

                bool IsRootWithNoChildren ( Category category ) => category.Parent == null &&
                    category.ChildCategories.All(c => c.ElementsInCategory <= 0 || c.ElementsInCategory == null);

                bool IsChildWithNoElements ( Category category ) => category.Parent != null &&
                    (category.ElementsInCategory == 0 || category.ElementsInCategory == null);
            }
        }

        public ICommand NewRootCommand { get; }
        public ICommand EditCommand { get; }

        public ICommand NewChildCommand { get; }

        public ICommand BlockCommand { get; }

        public ICommand UnblockCommand { get; }

        public CategoriesVM ( object? param, Navigation navigator )
        {
            _navigator = navigator;
            _admin = param as Models.Admin;
            _isDataLoaded = true;
            _currentEditVM = new Default();
            _currentChart = new Default();
            _param = param;

            LoadData().ConfigureAwait(false);

            NewRootCommand = new Command(() => CurrentEditVM = new RootCategoryAddVM(this, Categories));

            EditCommand = new Command(() =>
            {
                var editVM = new CategoryEditingVM(this, Categories, SelectedCategory!);
                editVM.CategoryEditing += ( s, e ) => LoadData().ConfigureAwait(false);

                CurrentEditVM = editVM;
            });

            NewChildCommand = new Command(() =>
            {
                var addVM = new ChildCategoryAddVM(this, Categories, SelectedCategory!.Id);

                addVM.CategoryAdd += ( s, e ) => LoadData().ConfigureAwait(false);

                CurrentEditVM = addVM;
            });

            BlockCommand = new Command(Block);
            UnblockCommand = new Command(Unblock);
        }

        private async void Block ()
        {
            DataLoaded = false;

            await Task.Run(() =>
            {
                using AppContext context = new();

                var category = context.Categories.Where(c => c.Id == SelectedCategory!.Id).Include(c => c.ChildCategories).First();

                if (category!.Parent == null)
                {
                    category.ChildCategories.ForEach(c => c.IsBlocked = true);
                }
                category.IsBlocked = true;

                context.SaveChanges();
            });

            await LoadData().ConfigureAwait(false);
        }

        private async void Unblock ()
        {
            DataLoaded = false;

            await Task.Run(() =>
            {
                using AppContext context = new();

                var category = context.Categories.Where(c => c.Id == SelectedCategory!.Id).Include(c => c.ChildCategories)
                .Include(c => c.Parent).First();

                if (category!.Parent == null)
                {
                    category.ChildCategories.ForEach(c => c.IsBlocked = false);
                }
                category.IsBlocked = false;

                context.SaveChanges();
            });

            await LoadData().ConfigureAwait(false);
        }

        private async Task LoadData ()
        {
            DataLoaded = false;

            await Task.Run(() =>
            {
                using AppContext appContext = new();

                Categories = new(appContext.Categories
                    .Where(c => c.Parent == null)
                    .Include(c => c.ChildCategories)
                    .Include(c => c.Parent));
            });

            OnPropertyChanged(nameof(Categories));
            DataLoaded = true;
        }
    }
}
