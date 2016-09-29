using System;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Accordion
{
	public class ExpendableView : ContentView
	{
		private bool _isExpended;

		public ExpendableView(ScrollView parent)
			: this(async (view) => { await parent.ScrollToAsync(0, view.Y, true); }) 
		{ }


		public ExpendableView(Action<ContentView> onSelected)
		{
			var header = new AbsoluteLayout {
					BackgroundColor = Color.FromHex("0067B7")
			};
			var icon =
				new Image
				{
					Source = ImageSource.FromFile("ic_keyboard_arrow_right_white_24dp.png"),
					VerticalOptions = LayoutOptions.Center
				};
			header.Children.Add(icon, new Rectangle(0, 1, .1, 1), AbsoluteLayoutFlags.All);
			header.Children.Add(
				new Label
				{
					Text = "October",
					TextColor = Color.White,
					BackgroundColor = Color.FromHex("0067B7"),
					VerticalTextAlignment = TextAlignment.Center,
					HeightRequest = 50
				},
				new Rectangle(1, 1, .9, 1),
				AbsoluteLayoutFlags.All);

			var list =
				new StackLayout
				{
					Children = {
						new Label { Text = "Hello", HeightRequest = 30 },
						new Label { Text = "Hello", HeightRequest = 30 },
						new Label { Text = "Hello", HeightRequest = 30 },
						new Label { Text = "Hello", HeightRequest = 30 },
						new Label { Text = "Hello", HeightRequest = 30 },
						new Label { Text = "Hello", HeightRequest = 30 },
						new Label { Text = "Hello", HeightRequest = 30 },
						new Label { Text = "Hello", HeightRequest = 30 },
						new Label { Text = "Hello", HeightRequest = 30 }
					},
					HeightRequest = 0,
					
				};

			var layout =
				new StackLayout
				{
					Spacing = 0,
					Children = { 
						header,
						list
					}
				};

			header.GestureRecognizers.Add(
				new TapGestureRecognizer
				{
					Command = new Command(() =>
					{
						if (_isExpended)
						{
							list.HeightRequest = 0;
							icon.Source = ImageSource.FromFile("ic_keyboard_arrow_right_white_24dp.png");
							_isExpended = false;
						}
						else 
						{
							list.HeightRequest = list.Children.Count * 30;
							icon.Source = ImageSource.FromFile("ic_keyboard_arrow_down_white_24dp.png");
							onSelected(this);
							_isExpended = true;
						}
					})
				}
			);

			this.Content = layout;
		}
	}

	public class AccordionViewPage : ContentPage
	{
		public AccordionViewPage()
		{
			var scrollView = new ScrollView();

			var layout =
				new StackLayout
				{
					Spacing = 1,
					Children = {
						new ExpendableView(scrollView),
						new ExpendableView(scrollView),
						new ExpendableView(scrollView),
						new ExpendableView(scrollView),
						new ExpendableView(scrollView),
						new ExpendableView(scrollView),
						new ExpendableView(scrollView),
						new ExpendableView(scrollView),
						new ExpendableView(scrollView),
						new ExpendableView(scrollView),
						new ExpendableView(scrollView),
						new ExpendableView(scrollView),
						new ExpendableView(scrollView)
					}
				};

			scrollView.Content = layout;

			this.Title = "Accordion";
			this.Content = scrollView;
		}
	}

	public class App : Application
	{
		public App()
		{
			var page = 
				new TabbedPage
				{
					Children = { 
						new AccordionViewPage(),
						new CustomViewTestPage()
					}
				};
			MainPage = page;
		}
	}
}
