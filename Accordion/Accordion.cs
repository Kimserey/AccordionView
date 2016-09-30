using System;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace Accordion
{
	public class ShoppingCart
	{
		public DateTime Date { get; set; }
		public double Amount { get; set; }
	}

	public class Section
	{ 
		public string Title { get; set; }
		public IEnumerable<ShoppingCart> List { get; set; }
	}

	public class ViewModel
	{
		public IEnumerable<Section> List { get; set; }
	}

	public class AccordionViewPage : ContentPage
	{
		public AccordionViewPage()
		{
			this.Title = "Accordion";

			var template = new DataTemplate(() =>
			{
				var layout = new AbsoluteLayout { Padding = 5, HeightRequest = 50 };
				var title = new Label { HorizontalTextAlignment = TextAlignment.Start };
				var price = new Label { HorizontalTextAlignment = TextAlignment.End };
				layout.Children.Add(title, new Rectangle(0, 1, 0.5, 1), AbsoluteLayoutFlags.All);
				layout.Children.Add(price, new Rectangle(0, 1, 0.5, 1), AbsoluteLayoutFlags.All);
				title.SetBinding(Label.TextProperty, "Date", stringFormat: "{0:dd MMM yyyy}");
				price.SetBinding(Label.TextProperty, "Amount", stringFormat: "{0:C2}");
				return (object)layout;
			});

			var view = new AccordionView(template);
			view.SetBinding(AccordionView.ItemsSourceProperty, "List");
			view.Template.SetBinding(AccordionSectionView.TitleProperty, "Title");
			view.Template.SetBinding(AccordionSectionView.ItemsSourceProperty, "List");

			view.BindingContext =
				new ViewModel
				{ 
					List = new List<Section> {
						new Section
						{
							Title = "October",
							List = new List<ShoppingCart> {
								new ShoppingCart { Date = DateTime.UtcNow, Amount = 10.05 }
							}
						},
						new Section
						{
							Title = "September",
							List = new List<ShoppingCart> {
								new ShoppingCart { Date = DateTime.UtcNow, Amount = 10.05 }
							}
						},
						new Section
						{
							Title = "August",
							List = new List<ShoppingCart> {
								new ShoppingCart { Date = DateTime.UtcNow, Amount = 10.05 }
							}
						}
					}
				};
			this.Content = view;
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
