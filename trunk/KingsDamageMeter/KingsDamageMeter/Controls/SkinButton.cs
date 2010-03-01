/**************************************************************************\
 * 
    This file is part of KingsDamageMeter.

    KingsDamageMeter is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    KingsDamageMeter is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with KingsDamageMeter. If not, see <http://www.gnu.org/licenses/>.
 * 
\**************************************************************************/

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KingsDamageMeter.Controls
{
    /// <summary>
    /// A class that represents a skinnable button.
    /// </summary>
    public class SkinButton : Button
    {
        #region MouseUpImage Property
        
        public static DependencyProperty MouseUpImageProperty = DependencyProperty.Register(
            "MouseUpImage",
            typeof (ImageSource),
            typeof (SkinButton),
            new PropertyMetadata(null, MouseUpImageChanged));

        /// <summary>
        /// Gets or sets the image to draw when the MouseUp event occurs.
        /// </summary>
        public ImageSource MouseUpImage
        {
            get { return (ImageSource)GetValue(MouseUpImageProperty); }
            set { SetValue(MouseUpImageProperty, value); }
        }

        private static void MouseUpImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (SkinButton) d;
            ctrl.Content = new Image {Source = ctrl.MouseUpImage};
        }

        #endregion

        #region MouseOverImage Property

        public static DependencyProperty MouseOverImageProperty = DependencyProperty.Register(
            "MouseOverImage",
            typeof(ImageSource),
            typeof(SkinButton));

        /// <summary>
        /// Gets or sets the image to draw when the MouseOver event occurs.
        /// </summary>
        public ImageSource MouseOverImage
        {
            get { return (ImageSource)GetValue(MouseOverImageProperty); }
            set { SetValue(MouseOverImageProperty, value); }
        }

        #endregion

        #region MouseDownImage Property

        public static DependencyProperty MouseDownImageProperty = DependencyProperty.Register(
            "MouseDownImage",
            typeof(ImageSource),
            typeof(SkinButton));

        /// <summary>
        /// Gets or sets the image to draw when the MouseDown event occurs.
        /// </summary>
        public ImageSource MouseDownImage
        {
            get { return (ImageSource)GetValue(MouseDownImageProperty); }
            set { SetValue(MouseDownImageProperty, value); }
        }

        #endregion
        
        /// <summary>
        /// Gets the status of images.
        /// </summary>
        public bool ImagesLoaded
        {
            get
            {
                bool result = true;
                result = (MouseUpImage == null) ? false : result;
                result = (MouseOverImage == null) ? false : result;
                result = (MouseDownImage == null) ? false : result;
                return result;
            }
        }

        /// <summary>
        /// A class that represents a skinnable button.
        /// </summary>
        public SkinButton()
        {
            Width = 20;
            Height = 20;

            Focusable = true;

            var presenter = new FrameworkElementFactory(typeof (ContentPresenter));
            presenter.SetValue(ContentProperty, new TemplateBindingExtension(ContentProperty));
            Template = new ControlTemplate {VisualTree = presenter};
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (ImagesLoaded)
            {
                Content = new Image {Source = MouseOverImage};
            }
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (ImagesLoaded)
            {
                Content = new Image { Source = MouseUpImage };
            }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (ImagesLoaded)
            {
                Content = new Image { Source = MouseDownImage };
            }
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (ImagesLoaded)
            {
                Content = new Image { Source = MouseOverImage };
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (ImagesLoaded)
            {
                Content = new Image { Source = MouseOverImage };
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (ImagesLoaded)
            {
                Content = new Image { Source = MouseUpImage };
            }
        }
    }
}
