using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ACDCTestSystemPart1.Services
{
    public class PasswordBindingAttachedProperties : DependencyObject
    {
        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBindingAttachedProperties), new PropertyMetadata(string.Empty, (s, e) =>
            {
                if (s is PasswordBox p)
                {
                    if (p.Password != (string)e.NewValue)
                    {
                        p.Password = (string)e.NewValue;
                    }
                    //if (!string.IsNullOrEmpty((string)s.GetValue(PasswordBindingProperty)))
                    //{

                    //}
                    //else
                    //{

                    //}
                }
            }));
    }



    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += OnPasswordChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= OnPasswordChanged;
        }

        private void OnPasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is PasswordBox p)
            {
                if (p.Password != PasswordBindingAttachedProperties.GetPassword(p))
                {
                    PasswordBindingAttachedProperties.SetPassword(p, p.Password);
                }
            }
        }
    }
}
