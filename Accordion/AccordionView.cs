using System;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Accordion
{
	public class DefaultTemplate : AbsoluteLayout
	{
		public DefaultTemplate()
		{
			this.Padding = 5;
			this.HeightRequest = 50;
			var title = new Label { HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.StartAndExpand };
			var price = new Label { HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.End };
			this.Children.Add(title, new Rectangle(0, 0.5, 0.5, 1), AbsoluteLayoutFlags.All);
			this.Children.Add(price, new Rectangle(1, 0.5, 0.5, 1), AbsoluteLayoutFlags.All);
			title.SetBinding(Label.TextProperty, "Date", stringFormat: "{0:dd MMM yyyy}");
			price.SetBinding(Label.TextProperty, "Amount", stringFormat: "{0:C2}");
		}
	}

	public class AccordionView : ScrollView
	{
		private StackLayout _layout = new StackLayout { Spacing = 1 };

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

			foreach (object item in this.ItemsSource)
			{
				var template = (View)this.Template.CreateContent();
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

	public class AccordionSectionView : StackLayout
	{
		private bool _isExpanded = false;
		private StackLayout _content = new StackLayout { HeightRequest = 0 };
		private Color _headerColor = Color.FromHex("0067B7");
		private ImageSource _arrowRight = ImageSource.FromFile("ic_keyboard_arrow_right_white_24dp.png");
		private ImageSource _arrowDown = ImageSource.FromFile("ic_keyboard_arrow_down_white_24dp.png");
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

			this.Spacing = 0;
			this.Children.Add(_header);
			this.Children.Add(_content);

			_header.GestureRecognizers.Add(
				new TapGestureRecognizer
				{
					Command = new Command(async () =>
					{
						if (_isExpanded)
						{
							_headerIcon.Source = _arrowRight;
							_content.HeightRequest = 0;
							_content.IsVisible = false;
							_isExpanded = false;
						}
						else
						{
							_headerIcon.Source = _arrowDown;
							_content.HeightRequest = _content.Children.Count * 50;
							_content.IsVisible = true;
							_isExpanded = true;

							// Scroll top by the current Y position of the section
							if (parent.Parent is VisualElement)
							{
								await parent.ScrollToAsync(0, this.Y, true);
							}
						}
					})
				}
			);
		}

		void ChangeTitle()
		{
			_headerTitle.Text = this.Title;
		}

		void PopulateList()
		{
			_content.Children.Clear();

			foreach (object item in this.ItemsSource)
			{
				var template = (View)_template.CreateContent();
				template.BindingContext = item;
				_content.Children.Add(template);
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
