using System;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Accordion
{
	public class AccordionView : ScrollView
	{
		private StackLayout _layout = new StackLayout();

		public DataTemplate Template { get; set; }
		public DataTemplate SubTemplate { get; set; }

		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create(
				propertyName: "ItemsSource",
				returnType: typeof(IList),
				declaringType: typeof(AccordionSectionView),
				defaultValue: default(IList),
				propertyChanged: AccordionView.PopulateList);

		public IList ItemsSource
		{
			get { return (IList)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public AccordionView(DataTemplate itemTemplate)
		{
			this.SubTemplate = itemTemplate;
			this.Template = new DataTemplate(() => (object)(new AccordionSectionView(itemTemplate, this)));
			this.Content = _layout;
		}

		void PopulateList()
		{
			_layout.Children.Clear();

			var template = (View)this.Template.CreateContent();
			foreach (object item in this.ItemsSource)
			{
				template.BindingContext = item;
				_layout.Children.Add(template);
			}
		}

		static void PopulateList(BindableObject bindable, object oldValue, object newValue)
		{
			if (oldValue == newValue) return;
			((AccordionView)bindable).PopulateList();
		}
	}

	public class AccordionSectionView : ContentView
	{
		private bool _isExpended = false;
		private Color _headerColor = Color.FromHex("0067B7");
		private ImageSource _arrowRight = ImageSource.FromFile("ic_keyboard_arrow_right_white_24dp.png");
		private ImageSource _arrowDown = ImageSource.FromFile("ic_keyboard_arrow_down_white_24dp.png");
		private StackLayout _list = new StackLayout { HeightRequest = 0 };
		private AbsoluteLayout _header = new AbsoluteLayout();
		private Image _headerIcon = new Image { VerticalOptions = LayoutOptions.Center };
		private Label _headerTitle = new Label { TextColor = Color.White, VerticalTextAlignment = TextAlignment.Center, HeightRequest = 50 };
		private DataTemplate _template;

		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create(
				propertyName: "ItemsSource",
				returnType: typeof(IList),
				declaringType: typeof(AccordionSectionView),
				defaultValue: default(IList),
				propertyChanged: AccordionSectionView.PopulateList);

		public IList ItemsSource
		{
			get { return (IList)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly BindableProperty TitleProperty =
			BindableProperty.Create(
				propertyName: "Title",
				returnType: typeof(string),
				declaringType: typeof(AccordionSectionView),
				propertyChanged: AccordionSectionView.ChangeTitle);

		public string Title
		{
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		public AccordionSectionView(DataTemplate itemTemplate, ScrollView parent)
		{
			_template = itemTemplate;
			_headerTitle.BackgroundColor = _headerColor;
			_headerIcon.Source = _arrowRight;
			_header.BackgroundColor = _headerColor;

			_header.Children.Add(_headerIcon, new Rectangle(0, 1, .1, 1), AbsoluteLayoutFlags.All);
			_header.Children.Add(_headerTitle, new Rectangle(1, 1, .9, 1), AbsoluteLayoutFlags.All);

			var layout =
				new StackLayout
				{
					Spacing = 0,
					Children = {
						_header,
						_list
					}
				};

			_header.GestureRecognizers.Add(
				new TapGestureRecognizer
				{
					Command = new Command(async () =>
					{
						if (_isExpended)
						{
							_headerIcon.Source = _arrowRight;
							_list.HeightRequest = 0;
							_isExpended = false;
						}
						else
						{
							_headerIcon.Source = _arrowDown;
							_list.HeightRequest = _list.Children.Count * 30;
							_isExpended = true;
							await parent.ScrollToAsync(0, layout.Y, true);
						}
					})
				}
			);

			this.Content = layout;
		}

		void ChangeTitle()
		{
			_headerTitle.Text = this.Title;
		}

		void PopulateList()
		{
			_list.Children.Clear();

			var template = (View)_template.CreateContent();
			foreach (object item in this.ItemsSource)
			{
				template.BindingContext = item;
				_list.Children.Add(template);
			}
		}

		static void ChangeTitle(BindableObject bindable, object oldValue, object newValue)
		{
			if (oldValue == newValue) return;
			((AccordionSectionView)bindable).ChangeTitle();
		}

		static void PopulateList(BindableObject bindable, object oldValue, object newValue)
		{
			if (oldValue == newValue) return;
			((AccordionSectionView)bindable).PopulateList();
		}
	}
}
