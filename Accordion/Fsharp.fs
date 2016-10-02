type DefaultAccordionItemTemplate() as self =
    inherit AbsoluteLayout()

    do
        self.Padding <- new Thickness(5.)
        self.HeightRequest <- 50.
        let title = 
            new Label(HorizontalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.StartAndExpand)
        let price = 
            new Label(HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.End)

        title.SetBinding(Label.TextProperty, "Date", stringFormat = "{0:dd MMM yyyy}")
        price.SetBinding(Label.TextProperty, "Amount", stringFormat = "{0:C2}")

        self.Children.Add(title, new Rectangle(0., 0.5, 0.5, 1.), AbsoluteLayoutFlags.All)
        self.Children.Add(price, new Rectangle(1., 0.5, 0.5, 1.), AbsoluteLayoutFlags.All)
        
type AccordionSectionView(template: DataTemplate, parent: ScrollView) as self =
    inherit StackLayout()

    let content = new StackLayout(HeightRequest = 0.)
    let headerColor = Color.FromHex "0067B7"
    let arrowRight = FileImageSource.op_Implicit "ic_keyboard_arrow_right_white_24dp.png"
    let arrowDown = FileImageSource.op_Implicit "ic_keyboard_arrow_down_white_24dp.png"
    let header = new AbsoluteLayout(BackgroundColor = headerColor)
    let headerIcon = new Image(VerticalOptions = LayoutOptions.Center, Source = arrowRight)
    let headerTitle = new Label(TextColor = Color.White, VerticalTextAlignment = TextAlignment.Center, HeightRequest = 50., BackgroundColor = headerColor)

    do
        header.Children.Add(headerIcon, new Rectangle(0., 1., 0.1, 1.), AbsoluteLayoutFlags.All)
        header.Children.Add(headerTitle, new Rectangle(1., 1., 0.9, 1.), AbsoluteLayoutFlags.All)
        self.Spacing <- 0.
        self.Children.Add(header)
        self.Children.Add(content)
        
        header.GestureRecognizers.Add(
            new TapGestureRecognizer(Command = new Command(fun () ->
                if (content.IsVisible && content.HeightRequest > 0.) then
                    headerIcon.Source <- arrowRight
                    content.HeightRequest <- 0.
                    content.IsVisible <- false
                else
                    headerIcon.Source <- arrowDown
                    content.HeightRequest <- (float)(content.Children.Count * 50)
                    content.IsVisible <- true

                    // Scroll top by the current Y position of the section
                    parent.ScrollToAsync(0., self.Y, true) 
                    |> Async.AwaitTask 
                    |> Async.StartImmediate
                )))

    member val HeaderTitle = headerTitle
    member val Content = content
    member val Template = template
    
    static member ChangeTitle bindable oldValue newValue =
        if (oldValue <> newValue) then (unbox<AccordionSectionView> bindable).HeaderTitle.Text <- string newValue

    static member PopulateList (bindable: BindableObject) oldValue newValue =
        if (oldValue <> newValue) then 
            let view = (unbox<AccordionSectionView> bindable)
            view.Content.Children.Clear()
            
            for item in view.ItemsSource do
                let template = unbox<View> (view.Template.CreateContent())
                template.BindingContext <- item
                view.Content.Children.Add(template)
        
    static member ItemsSourceProperty = 
        BindableProperty.Create(
            propertyName = "ItemsSource", 
            returnType = typeof<IList>, 
            declaringType = typeof<AccordionSectionView>, 
            defaultValue = Unchecked.defaultof<IList>, 
            propertyChanged = new BindableProperty.BindingPropertyChangedDelegate(AccordionSectionView.PopulateList))

    member self.ItemsSource
        with get (): IList = unbox<IList> <| self.GetValue(AccordionSectionView.ItemsSourceProperty)
        and set (value: IList) = self.SetValue(AccordionSectionView.ItemsSourceProperty, value)

    static member TitleProperty = 
        BindableProperty.Create(
            propertyName = "Title", 
            returnType = typeof<string>, 
            declaringType = typeof<AccordionSectionView>, 
            propertyChanged = new BindableProperty.BindingPropertyChangedDelegate(AccordionSectionView.ChangeTitle))
    
    member self.Title
        with get () = unbox<string> <| self.GetValue(AccordionSectionView.TitleProperty)
        and set (value: string) = self.SetValue(AccordionSectionView.TitleProperty, value) 

type AccordionView(itemTemplate) as self = 
    inherit ScrollView()
    
    do self.Content <- new StackLayout(Spacing = 1.)
    
    member val Template = new DataTemplate(fun () -> box (new AccordionSectionView(itemTemplate, self))) with get, set
    member val SubTemplate = itemTemplate with get, set
    
    static member PopulateList bindable oldValue newValue =
        if (oldValue <> newValue) then
            let view = (unbox<AccordionView> bindable)
            let content = unbox<Layout<View>> view.Content
            content.Children.Clear()
            
            for item in view.ItemsSource do
                let template = unbox<View> (view.Template.CreateContent())
                template.BindingContext <- item
                content.Children.Add(template)

    static member ItemsSourceProperty =
        BindableProperty.Create(
            propertyName = "ItemsSource",
            returnType = typeof<IList>,
            declaringType = typeof<AccordionSectionView>,
            defaultValue = Unchecked.defaultof<IList>,
            propertyChanged = new BindableProperty.BindingPropertyChangedDelegate(AccordionView.PopulateList))
            
    member self.ItemsSource
        with get (): IList = unbox<IList> <| self.GetValue(AccordionSectionView.ItemsSourceProperty)
        and set (value: IList) = self.SetValue(AccordionSectionView.ItemsSourceProperty, value)
