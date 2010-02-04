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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

namespace KingsDamageMeter.Controls
{
    /// <summary>
    /// A class that represents a skinnable button.
    /// </summary>
    public class SkinButton : Image
    {
        private string _MouseUpImageLocation;
        private string _MouseOverImageLocation;
        private string _MouseDownImageLocation;

        private BitmapImage _MouseUpImage;
        private BitmapImage _MouseOverImage;
        private BitmapImage _MouseDownImage;

        private event EventHandler MouseUpImageLoaded;
        private event EventHandler MouseOverImageLoaded;
        private event EventHandler MouseDownImageLoaded;

        /// <summary>
        /// Gets or sets the image to draw when the MouseUp event occurs.
        /// </summary>
        public string MouseUpImage
        {
            get
            {
                return _MouseUpImageLocation;
            }

            set
            {
                _MouseUpImageLocation = value;

                if (MouseUpImageLoaded != null)
                {
                    MouseUpImageLoaded(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the image to draw when the MouseOver event occurs.
        /// </summary>
        public string MouseOverImage
        {
            get
            {
                return _MouseOverImageLocation;
            }

            set
            {
                _MouseOverImageLocation = value;

                if (MouseOverImageLoaded != null)
                {
                    MouseOverImageLoaded(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the image to draw when the MouseDown event occurs.
        /// </summary>
        public string MouseDownImage
        {
            get
            {
                return _MouseDownImageLocation;
            }

            set
            {
                _MouseDownImageLocation = value;

                if (MouseDownImageLoaded != null)
                {
                    MouseDownImageLoaded(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the status of images.
        /// </summary>
        public bool ImagesLoaded
        {
            get
            {
                bool result = true;
                result = (_MouseUpImage == null) ? false : result;
                result = (_MouseOverImage == null) ? false : result;
                result = (_MouseDownImage == null) ? false : result;
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

            MouseUpImageLoaded += OnMouseUpImageLoaded;
            MouseOverImageLoaded += OnMouseOverImageLoaded;
            MouseDownImageLoaded += OnMouseDownImageLoaded;
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (ImagesLoaded)
            {
                Source = _MouseOverImage;
            }
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (ImagesLoaded)
            {
                Source = _MouseUpImage;
            }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            
            base.OnMouseDown(e);

            if (ImagesLoaded)
            {
                Source = _MouseDownImage;
            }

            e.Handled = true;
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (ImagesLoaded)
            {
                Source = _MouseOverImage;
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (ImagesLoaded)
            {
                Source = _MouseOverImage;
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (ImagesLoaded)
            {
                Source = _MouseUpImage;
            }
        }

        private void OnMouseUpImageLoaded(object sender, EventArgs e)
        {
            _MouseUpImage = new BitmapImage(new Uri(@_MouseUpImageLocation));
            Source = _MouseUpImage;
        }

        private void OnMouseOverImageLoaded(object sender, EventArgs e)
        {
            _MouseOverImage = new BitmapImage(new Uri(@_MouseOverImageLocation));
        }

        private void OnMouseDownImageLoaded(object sender, EventArgs e)
        {
            _MouseDownImage = new BitmapImage(new Uri(@_MouseDownImageLocation));
        }
    }
}
