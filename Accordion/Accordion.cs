using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Accordion
{
	public interface ISelectable
	{
		bool IsSelected { get; set; }
	}

	public class ItemsView : ScrollView
	{
		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create(
				propertyName: "ItemsSource",
				returnType: typeof(IList),
				declaringType: typeof(ItemsView),
				defaultBindingMode: BindingMode.TwoWay,
				defaultValue: default(IList),
				propertyChanged: ItemsView.OnItemsSourceChanged);

		public IList ItemsSource
		{
			get { return (IList)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly BindableProperty SelectedItemProperty =
			BindableProperty.Create(
				propertyName: "SelectedItem",
				returnType: typeof(object),
				declaringType: typeof(ItemsView),
				defaultBindingMode: BindingMode.TwoWay,
				defaultValue: default(object),
				propertyChanged: ItemsView.OnSelectedItemChanged);

		public object SelectedItem
		{
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public static readonly BindableProperty ItemTemplateProperty =
			BindableProperty.Create(
				propertyName: "ItemTemplate",
				returnType: typeof(DataTemplate),
				declaringType: typeof(ItemsView),
				defaultValue: default(DataTemplate),
				defaultBindingMode: BindingMode.TwoWay);

		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		readonly ICommand selectedCommand;
		readonly StackLayout stackLayout;

		public ItemsView(DataTemplate template): base()
		{
			selectedCommand = new Command<object>(item => this.SelectedItem = item);
			stackLayout =
				new StackLayout()
				{
					Orientation = StackOrientation.Horizontal,
					Padding = 0,
					Spacing = 0,
					HorizontalOptions = LayoutOptions.FillAndExpand

				};
			this.ItemTemplate = template;
			this.Content = stackLayout;
		}

		void Init()
		{
			stackLayout.Children.Clear();

			if (ItemsSource == null)
				return;

			foreach (var item in ItemsSource)
			{
				var content = ItemTemplate.CreateContent();
				var itemView = content as View;
				itemView.BindingContext = item;

				itemView.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = selectedCommand,
					CommandParameter = itemView as object
				});

				stackLayout.Children.Add(itemView);
			}
		}

		static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((ItemsView)bindable).Init();
		}

		static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var itemsView = (ItemsView)bindable;

			if (newValue == oldValue)
				return;

			var items = itemsView.ItemsSource.OfType<ISelectable>();

			foreach (var item in items)
				item.IsSelected = item == newValue;
		}
	}

	public class ItemsViewModel
	{
		public IEnumerable<Item> Items
		{
			get;
			set;
		}

		public ItemsViewModel ()
		{
			this.Items = new List<Item> {
				new Item { Title = "Something" },
				new Item { Title = "Test" },
				new Item { Title = "Hello" }
			};
		}
	}

	public class Item: ISelectable
	{
		public bool IsSelected
		{
			get; set;
		}

		public string Title { get; set; }
	}

	public class App : Application
	{
		public App()
		{
			var template =
				new DataTemplate(() =>
				{
					var label = new Label();
					var layout = 
						new StackLayout
						{
							Children = 
							{
								label		
							}
						};
					label.SetBinding(Label.TextProperty, "Title");
				return (object)layout;
				});

			var list = new ItemsView(template);
			var vm = new ItemsViewModel();
			list.SetBinding(ItemsView.ItemsSourceProperty, "Items");
			list.BindingContext = vm;

			var content = new ContentPage
			{
				Title = "Accordion",
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children = {
						list
					}
				}
			};

			MainPage = new NavigationPage(content);
		}
	}
}
