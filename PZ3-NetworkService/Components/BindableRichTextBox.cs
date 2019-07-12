using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PZ3_NetworkService.Components
{
    /// <summary> Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project. Add
    /// this XmlNamespace attribute to the root element of the markup file where it is to be used:
    ///
    /// xmlns:MyNamespace="clr-namespace:PZ3_NetworkService.Components"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project. Add
    /// this XmlNamespace attribute to the root element of the markup file where it is to be used:
    ///
    /// xmlns:MyNamespace="clr-namespace:PZ3_NetworkService.Components;assembly=PZ3_NetworkService.Components"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives to
    /// this project and Rebuild to avoid compilation errors:
    ///
    /// Right click on the target project in the Solution Explorer and "Add
    /// Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2) Go ahead and use your control in the XAML file.
    ///
    /// <MyNamespace:BindableRichTextBox/>
    ///
    /// </summary>
    public class BindableRichTextBox : RichTextBox
    {
        public static readonly DependencyProperty DocumentProperty =
       DependencyProperty.Register("Document", typeof(FlowDocument),
       typeof(BindableRichTextBox), new FrameworkPropertyMetadata
       (null, new PropertyChangedCallback(OnDocumentChanged)));

        public new FlowDocument Document
        {
            get
            {
                return (FlowDocument)this.GetValue(DocumentProperty);
            }

            set
            {
                this.SetValue(DocumentProperty, value);
            }
        }

        public static void OnDocumentChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs args)
        {
            var rtb = (RichTextBox)obj;
            rtb.Document = (FlowDocument)args.NewValue;
        }
    }
}