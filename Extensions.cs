using GameLogica;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using static UFO.Convert;

namespace UFO
{
    //======================================================= Extensions =======================================================
    /// <summary> <code>
    ///     <b>Extensions</b> - это Extension расширение какого-либо класса. <br/>
    ///     <b>Extension</b> расширения служат например для дописания чужого класса без изменения кода самого чужого класса.
    ///     Таким образом у чужого класса появляются методы и поля ранее не существовавшие. Они будут доступны всем кто
    ///     использует пространство имён и файл в котором описано это расширение класса (Extensions).
    ///     Например первый параметр Transparent(this Button btn, ...) - указывает для какого класса написано это расширение
    ///     и оно не указывается в качестве первого параметра при вызове метода Transparent у Button. <br/>
    ///     <b>Extensions</b> расширение класса обязательно должно иметь модификатор static !!!
    /// </code> </summary>
    static class Extensions
    {
        /// <summary>
        ///     Метод настраивает прозрачность и цвет фона, текста, контура кнопки Button при отображении/наведении/нажатии на неё.
        /// </summary>
        /// <param name="btn"> бла бла бла. </param>
        /// <param name="BackColor"> бла бла бла. </param>
        /// <param name="MouseDownBackColor"> бла бла бла. </param>
        /// <param name="MouseOverBackColor"> бла бла бла. </param>
        /// <param name="BorderColor"> бла бла бла. </param>
        /// <param name="TextColor"> бла бла бла. </param>
        /// <param name="BorderSize"> бла бла бла. </param>
        /// <remarks> 
        ///     <b> BackColor: </b> Задаёт цвет фона Button. <br/>
        ///     <b> MouseDownBackColor: </b> Задаёт цвет фона Button при нажатии левой кнопкой мыши на Button. <br/>
        ///     <b> MouseOverBackColor: </b> Задаёт цвет фона Button при наведении курсора на Button. <br/>
        ///     <b> BorderColor: </b> Задаёт цвет границы (рамки) вокруг Button. <br/>
        ///     <b> TextColor: </b> Задаёт цвет текста Button. <br/>
        ///     <b> BorderSize: </b> Задаёт толщину границы (рамки) вокруг Button. <br/>
        /// </remarks>
        public static void Button_Styles(this Button btn, Color BackColor, Color MouseDownBackColor, Color MouseOverBackColor,
                                         Color BorderColor, Color TextColor, int BorderSize)
        {
            btn.FlatStyle = FlatStyle.Flat; btn.FlatAppearance.BorderSize = BorderSize; btn.ForeColor = TextColor;
            btn.BackColor = BackColor; btn.FlatAppearance.MouseDownBackColor = MouseDownBackColor;
            btn.FlatAppearance.MouseOverBackColor = MouseOverBackColor; btn.FlatAppearance.BorderColor = BorderColor;
        }

        /// <summary>
        ///     Метод устанавливает значение свойства <b>SelectedIndex</b> у объекта <b>ComboBox</b> и запрещает срабатывание
        ///     вызова окна повышения уровня постройки в <b>SelectedIndexChanged(...);</b> <br/> посредством установки флага <b>Tag</b> = -1; 
        ///     Обработчик события <b>SelectedIndexChanged</b> считывает это значение и не срабатывает вызов окна. <br/>
        ///     После изменения <b>SelectedIndex</b> поле <b>Tag</b> принимает прежнее значение. <br/>
        ///     Если происходит изменение <b>SelectedIndex</b> на то же значение, вызов метода <b>SelectedIndexChanged(...);</b> не случится. <br/>
        ///     Если <b>Index</b> больше <b>Items.Count</b>, SelectedIndex не изменится и вызова метода <b>SelectedIndexChanged(...);</b> не случится.
        /// </summary>
        /// <value> <b> <paramref name="Index"/>: </b> индекс для SelectedIndex. <br/> </value>
        public static void Set_SelectedIndex_And_Changed_Ignore(this ComboBox obj, int Index) {
            if (obj.Items.Count > Index) { int tag = (int)obj.Tag; obj.Tag = -1; obj.SelectedIndex = Index; obj.Tag = tag; }
        }

        /// <summary> Метод принудительно перезванивает в событие <b>SelectedIndexChanged(...);</b> чтобы оно вызвало окно повышения уровня постройки. <br/> Если <b>Items.Count </b> = 0; перезвон всё равно случится. </summary>
        public static void CallBack_SelectedIndexChanged(this ComboBox obj) {
            if (obj.Items.Count == 0) { obj.Items.Add("-"); }
            int Index = obj.SelectedIndex;
            if (obj.SelectedIndex <= -1) { obj.SelectedIndex = obj.Items.Count - 1; obj.SelectedIndex = Index; }
            else { obj.SelectedIndex = -1; obj.SelectedIndex = Index; }
            obj.Items.RemoveAt(0);
        }

        //========================================================== BEGIN ==========================================================
        //================================== Control::Size / Control::Location / Control::SizeFont ==================================
        //===========================================================================================================================
        /// <summary>
        ///     Метод вычисляет позицию Control по центру относительно переданного <b>(other)</b> контрола по высоте и ширине. <br/>
        ///     Контрол <b>other</b> не должен быть родительским! Для манипульций с предком, используйте перегрузку.
        /// </summary>
        /// <remarks> После отработки кода, <b>Control.Location</b> получает новую позицию. <br/> Преобразования позиции под текущее разрешение не происходит! </remarks>
        /// <returns> Устанавливает новые координаты свойству <b>Location</b> и возвращает позицию контрола. <br/> Если <b>other</b> = null, Location контрола будет равен (0, 0); </returns>
        public static Point Centering(this Control sender, Control other) {
            if (other == null) sender.Location = new Point(0, 0);
            else sender.Location = new Point((other.Width - sender.Width) / 2 + other.Left, (other.Height - sender.Height) / 2 + other.Top);
            return sender.Location;
        }

        /// <summary> Метод вычисляет позицию Control по центру относительно его <b>Parent</b> по высоте и ширине. </summary>
        /// <remarks> После отработки кода, <b>Control.Location</b> получает новую позицию. <br/> Преобразования позиции под текущее разрешение не происходит! </remarks>
        /// <returns> Устанавливает новые координаты свойству <b>Location</b> и возвращает позицию контрола. <br/> Если <b>Parent = null</b>, Location контрола будет по центру относительно экрана монитора. </returns>
        public static Point Centering(this Control ctrl) {
            if (ctrl.Parent == null) ctrl.Location = new Point((ScreenBounds_Size().Width - ctrl.Width) / 2, (ScreenBounds_Size().Height - ctrl.Height) / 2);
            else ctrl.Location = new Point((ctrl.Parent.Width - ctrl.Width) / 2, (ctrl.Parent.Height - ctrl.Height) / 2);
            return ctrl.Location;
        }

        /// <summary> Метод вычисляет позицию Control по центру относительно его <b>Parent</b> по оси <b>X.</b> </summary>
        /// <remarks> После отработки кода, <b>Control.Location</b> получает новую позицию. <br/> Преобразования позиции под текущее разрешение не происходит! </remarks>
        /// <returns> Устанавливает новые координаты свойству <b>Location</b> и возвращает позицию контрола. </returns>
        public static Point Centering_X(this Control ctrl) {
            if (ctrl == null || ctrl.Parent == null) { ctrl.Location = new Point(0, 0); return ctrl.Location; }
            else ctrl.Location = new Point((ctrl.Parent.Width - ctrl.Width) / 2, ctrl.Height); return ctrl.Location;
        }

        /// <summary> Метод вычисляет позицию Control по центру относительно его <b>Parent</b> по оси <b>Y.</b> </summary>
        /// <remarks> После отработки кода, <b>Control.Location</b> получает новую позицию. <br/> Преобразования позиции под текущее разрешение не происходит! </remarks>
        /// <returns> Устанавливает новые координаты свойству <b>Location</b> и возвращает позицию контрола. </returns>
        public static Point Centering_Y(this Control ctrl) {
            if (ctrl == null || ctrl.Parent == null) { ctrl.Location = new Point(0, 0); return ctrl.Location; }
            else ctrl.Location = new Point(ctrl.Width, (ctrl.Parent.Height - ctrl.Height) / 2); return ctrl.Location;
        }

        /// <summary> Метод вычисляет позицию Control по центру относительно его <b>Parent</b> по оси <b>X</b>, а в качестве высоты берёт параметр <b>Top.</b> </summary>
        /// <remarks> После отработки кода, <b>Control.Location</b> получает новую позицию. <br/> Преобразования позиции под текущее разрешение не происходит! </remarks>
        /// <returns> Устанавливает новые координаты свойству <b>Location</b> и возвращает позицию контрола. </returns>
        public static Point Centering_X(this Control ctrl, int Top) {
            if (ctrl == null || ctrl.Parent == null) { ctrl.Location = new Point(0, 0); return ctrl.Location; }
            else ctrl.Location = new Point((ctrl.Parent.Width - ctrl.Width) / 2, Top); return ctrl.Location;
        }

        /// <summary> Метод вычисляет позицию Control по центру относительно его <b>Parent</b> по оси <b>Y</b>, а в качестве горизонтали берёт параметр <b>Left.</b> </summary>
        /// <remarks> После отработки кода, <b>Control.Location</b> получает новую позицию. <br/> Преобразования позиции под текущее разрешение не происходит! </remarks>
        /// <returns> Устанавливает новые координаты свойству <b>Location</b> и возвращает позицию контрола. </returns>
        public static Point Centering_Y(this Control ctrl, int Left) {
            if (ctrl == null || ctrl.Parent == null) { ctrl.Location = new Point(0, 0); return ctrl.Location; }
            else ctrl.Location = new Point(Left, (ctrl.Parent.Height - ctrl.Height) / 2); return ctrl.Location;
        }

        /// <summary> 
        ///     Функция вычисляет итоговый размер с учётом текущего разрешения экрана из переданных параметров. <br/>
        ///     <b> Size(...) </b> вызывается тогда когда нужно установить размер сразу.
        /// </summary>
        /// <remarks>
        ///     Параметры передаются из расчёта, что приложение пишется в разрешении 1920х1080 <br/>
        ///     и вычисляются заново с учётом текущего разрешения экрана. <br/>
        /// </remarks>
        /// <value> <b> Width, Height: </b> размеры, которые требуется преобразовать под текущее разрешение экрана. </value>
        /// <returns> Возвращает структуру <b> System.Drawing.Size </b>. Новый итоговый размер под текущее разрешение экрана. </returns>
        public static Size Size(int Width, int Height) {
            //1920x1080 - потому что я разрабатываю приложение в этом разрешении экрана!
            return new Size((int)(Width / 1920.0 * Screen.PrimaryScreen.Bounds.Size.Width), /*разрешение экрана по горизонтали */
                            (int)(Height / 1080.0 * Screen.PrimaryScreen.Bounds.Size.Height));/*разрешение экрана по вертикали*/
        }

        /// <summary> 
        ///     Функция устанавливает размер визуального элемента управления. <br/>
        ///     <b> Size(...) </b> вызывается тогда когда нужно установить размер сразу.
        /// </summary>
        /// <remarks> <inheritdoc cref="Size"/> </remarks>
        /// <value>
        ///     <b> Control.Width / Control.Height: </b> эти поля меняют своё значение. <br/>
        ///     <b> Width, Height: </b> устанавливаемые размеры для этого визуального элемента управления .
        /// </value>
        public static void Size(this Control sender, int Width, int Height) { sender.Size = Size(Width, Height); }
        /// <summary> 
        ///     Функция переустанавливает новый размер визуального элемента управления с учётом текущего разрешения экрана. <br/>
        ///     <b> ReSize(...) </b> вызывается после того как в поля <b> Control.Width / Control.Height </b> установлены значения.
        /// </summary>
        /// <value> <b> Control.Width / Control.Height: </b> эти поля меняют своё значение. </value>
        public static void ReSize(this Control ctrl) { ctrl.Size = Size(ctrl.Width, ctrl.Height); }

        /// <summary>
        ///     Метод вычисляет итоговое местоположение контрола с учётом текущего разрешения экрана. <br/>
        ///     Параметры <b>LocX </b> и <b>LocY </b> задаются в диапазоне [0..1], где: 0 - левый край, 0.5 - середина, 1 - правый край. <br/>
        ///     <b> Location(...) </b> вызывается тогда когда нужно установить местоположение сразу. <br/>
        ///     После отработки метода, <b>Control.Location</b> изменит свои координаты.
        /// </summary>
        /// <value>
        ///     <b> <paramref name="LocX"/>: </b> местоположение, которое требуется преобразовать под текущее разрешение экрана по оси Х. <br/>
        ///     <b> <paramref name="LocY"/>: </b> местоположение, которое требуется преобразовать под текущее разрешение экрана по оси Y. <br/>
        /// </value>
        /// <returns> Возвращает структуру <b> System.Drawing.Point </b>. Новое итоговое местоположение под текущее разрешение экрана. </returns>
        public static Point Location(this Control sender, double LocX, double LocY) {
            sender.Location = new Point((int)(LocX * sender.Parent.Width), (int)(LocY * sender.Parent.Height));
            return sender.Location;
        }
        /// <summary>
        ///     Метод корректирует местоположение контрола с учётом текущего разрешения экрана. <br/>
        ///     Если перед вызовом этого метода разрешение экрана или размер формы не менялся, координаты контрола <b>Left/Top</b> останутся неизменными. <br/>
        ///     После отработки метода, <b>Control.Location</b> изменит свои координаты.
        /// </summary>
        public static void ReLocation(this Control sender) {
            double LocX = (double)sender.Left / sender.Parent.Width;
            double LocY = (double)sender.Top / sender.Parent.Height;
            sender.Location(LocX, LocY);
        }

        /// <summary>
        ///     Функция устанавливает размер шрифта визуального элемента управления. <br/>
        ///     Введённый размер шрифта, в разрешении <b>1920х1080</b>, будет вычеслен заново <br/>
        ///     с учётом текущего разрешения экрана. <br/>
        ///     <b> SizeFont(...) </b> вызывается тогда когда нужно установить размер шрифта сразу.
        /// </summary>
        /// <remarks>
        ///     Не использовать для <b>RichTextBox</b>, такое изменение шрифта убивает все теги <b>RTF</b> документа. <br/>
        ///     Вместо этого следует использовать <b>RichTextBox.ZoomFactor</b> типа <b>float</b>, <br/>
        ///     где <b>1.0F</b> = 100% размера шрифта (при 24 будет 24), а <b>0.5F</b> = 50% размера шрифта (при 24 будет 12).
        /// </remarks>
        /// <value>
        ///     <b> Control.Font.Size: </b> это поле меняет своё значение. <br/>
        ///     <b> FontSize: </b> размер шрифта заданного для разрешения экрана <b>1920х1080. (16:9)</b> <br/>
        ///     <b> FS: </b> сведения о стиле, применяемые к тексту. [bold, regular, italic и т.д.]
        /// </value>
        public static void SizeFont(this Control sender, float FontSize, FontStyle FS = FontStyle.Regular, string FontName = "Time New Roman") {
            sender.Font = new Font(sender.Font.FontFamily, (float)(FontSize / 1080.0 * Screen.PrimaryScreen.Bounds.Size.Height), FS);
        }
        /// <summary>
        ///     Функция переустанавливает новый размер шрифта визуального элемента управления с учётом текущего разрешения экрана. <br/>
        ///     <b> ReSizeFont(...) </b> вызывается после того как в поле <b> Control.Font.Size</b> установлено значение.
        /// </summary>
        /// <remarks> <inheritdoc cref="SizeFont"/> </remarks>
        /// <value> 
        ///     <b> Control.Font.Size: </b> это поле меняет своё значение. <br/>
        ///     <b> FS: </b> сведения о стиле, применяемые к тексту. [bold, regular, italic и т.д.]
        /// </value>
        public static void ReSizeFont(this Control sender, FontStyle FS, string FontName = "Time New Roman") {
            SizeFont(sender, sender.Font.Size, FS, FontName);
        }

        /// <summary>
        ///     Метод устанавливает новый размер контрола по ширине и высоте в соответствии с его содержимым. <br/>
        ///     У контролов есть свойство <b>AutoSizeMode</b>, которое определяет как вычисляется авторазмер. <br/>
        ///     Этот метод умеет как увеличивать, так и уменьшать контрол под содержимое с возможностью изменения размеров в будущем. <br/>
        ///     <b>Control.AutoSizeMode.GrowAndShirink</b> делает то же самое, но после его применения, размеры контрола вручную уже не поменять.
        /// </summary>
        /// <remarks> Не учитывается разрешение экрана. </remarks>
        public static void AutoSize(this Control sender) {
            sender.AutoSize = false; sender.Width = sender.Height = 1; sender.AutoSize = true;
        }
        /// <summary> Метод масштабирует текст под разное разрешение экрана для <b>RichTextBox</b>. </summary>
        public static void ZoomFactor(this RichTextBox sender) {
            float ZoomFactor = //1F;
            /*1080*/SCREEN(max: 1080) ? 1F :
            /*900*/SCREEN(900, 1080) ? 1.05F :
            /*x768*/SCREEN(768, 900) ? 1.10F :
            /*x720*/SCREEN(min: 768) ? 1.15F : 1F;
            sender.ZoomFactor = ToCSR(ZoomFactor);
        }
        //===========================================================================================================================
        //================================== Control::Size / Control::Location / Control::SizeFont ==================================
        //=========================================================== END ===========================================================

        //========================================================== BEGIN ==========================================================
        //=================================================== Control.Transparent ===================================================
        //===========================================================================================================================
        /// <summary> 
        ///     Resize the image to the specified width and height. <br/>
        ///     Метод создаёт новую копию картинки с размером <b>width / height</b>. <br/> Высококачественное изменение размера.
        /// </summary>
        /// <value>
        ///     <b>image:</b> изображение для изменения размера. <br/>
        ///     <b>width:</b> ширина нового размера. <br/>
        ///     <b>height:</b> высота нового размера. <br/>
        /// </value>
        /// <returns> Возвращает копию изображения с новым размером. <br/> Если <b>image = null</b>, метод тоже вернёт <b>null.</b> </returns>
        public static Bitmap ResizeImage(Image image, int width, int height) {
            if (image == null) return null;
            var destRect = new Rectangle(0, 0, width, height); var destImage = new Bitmap(width, height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (var graphics = Graphics.FromImage(destImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            } return destImage;
        }

        /// <summary>
        ///     Функция получает System.Drawing.Bitmap любого визуального элемента управления унаследованного от базового класса Control.
        /// </summary>
        /// <remarks> 
        ///     <b>ctrl:</b> Любой визуальный элемент управления унаследованный от базового класса Control у которого есть свойство BackgroundImage. <br/>
        ///     <b>TransparentColor:</b> хромокей переданного <b>ctrl.</b> <br/>
        /// </remarks>
        /// <returns> Точечный рисунок System.Drawing.Bitmap. При неудачной попытке получить Bitmap, вернёт null. </returns>
        public static Bitmap getBitmap(Control ctrl, Color TransparentColor = default) {
            try {
                if (ctrl.BackgroundImage != null) {
                    Bitmap BMP = new Bitmap(ctrl.Width, ctrl.Height); ctrl.DrawToBitmap(BMP, ctrl.ClientRectangle); return BMP;
                } else {
                    Bitmap BMP = new Bitmap(ctrl.Width, ctrl.Height); for (int x = 0; x < ctrl.Width; x++)
                        for (int y = 0; y < ctrl.Height; y++) BMP.SetPixel(x, y, TransparentColor);//BMP.SetPixel(x, y, ctrl.BackColor);
                    return BMP;
                }
            } catch (Exception) {
                if (ctrl == null) MessageBox.Show("Error 15.\nОшибка в class UFO.Extensions.getBitmap(...);\nctrl = null. ctrl.Name = '" + ctrl.Name + "'.");
                else if (ctrl.BackgroundImage == null) MessageBox.Show("Error 16.\nОшибка в class UFO.Extensions.getBitmap(...);\n" +
                    "Не удалось получить Bitmap этого элемента управления. ctrl.Name = '" + ctrl.Name + "'.");
                return null;
            }
        }

        /// <summary>
        ///     Метод задаёт степень прозрачности для любого визуального элемента управления унаследованного от Control. <br/>
        ///     После отработки кода, <b>Control.BackgroundImage</b> получает новый <b>Bitmap</b>. <br/>
        /// </summary>
        /// <remarks> 
        ///     <b> <paramref name="Transparent"/>: </b> цвет "хромокея". Alpha канал контрола с этим цветом заменятся на значение <b>Alpha.</b> <br/>
        ///     <b> <paramref name="Alpha"/>: </b> цвет альфа канала. [255] = 100% не прозрачный, [127] = полупрозрачный на 50%, [0] = 100% прозрачный. <br/>
        /// </remarks>
        public static void Transparent(this Control sender, Color Transparent, byte Alpha = 255) {
            Bitmap bmp = getBitmap(sender, Transparent);
            BitmapData Data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            int length = bmp.Width * bmp.Height * 4; byte[] RGBA = new byte[length];//создать массив
            System.Runtime.InteropServices.Marshal.Copy(Data.Scan0, RGBA, 0, length);//скопировать RGB bmp в массив 
            for (int i = 0; i < length; i += 4) { Color Cl = Color.FromArgb(RGBA[i + 3], RGBA[i + 0], RGBA[i + 1], RGBA[i + 2]);
                if (Cl == Transparent) {
                    RGBA[i + 3] = Alpha; RGBA[i + 2] = Transparent.R;
                    RGBA[i + 1] = Transparent.G; RGBA[i + 0] = Transparent.B;
                }
            } System.Runtime.InteropServices.Marshal.Copy(RGBA, 0, Data.Scan0, length); bmp.UnlockBits(Data);
            sender.BackgroundImage = bmp;
        }
        //===========================================================================================================================
        //=================================================== Control.Transparent ===================================================
        //=========================================================== END ===========================================================

        //========================================================== BEGIN ==========================================================
        //===================================================== Control.Contour =====================================================
        //===========================================================================================================================
        /// <summary>
        ///     Функция очищает рамку в любом визуальном элементе управления унаследованного от Control
        /// </summary>
        /// <remarks> 
        ///     <b> this Control: </b> элемент управления верхнего слоя, в который вставляются пиксели для замены его "хромокея". <br/>
        ///     <b> DownLayer: </b> элемент управления нижнего слоя, из которого берутся пиксели для
        ///                         замены фона верхнего слоя (this Control). <br/>
        ///     <b> Thickness: </b> толщина рамки в пикселях. <br/>
        ///     <b> ThicknessColor: </b> цвет рамки. Если ThicknessColor не совпадёт с фактическим цветом рамки,
        ///                              очищение не получится и рамка останется. <br/>
        ///     <br/> Код работает только в случае если контрол полностью помещается на своём Parent, <br/>
        ///     в противном случае, если например контрол лежит на стыке между двумя другими контролами, <br/>
        ///     то при замене уголков в местах где под уголком не его Parent, замена не происходит и часть рамки остаётся. <br/>
        ///     В таких случаях рамка снимается так: <br/>
        ///     грузим картинку в контрол снова или пишем Control.BackgroundImage = null; если картинка не нужна.
        /// </remarks>
        public static void Contour_Clear(this Control sender, byte Thickness, Color ThicknessColor) {
            if (Thickness <= 0) return;
            var bmp = getBitmap(sender, ThicknessColor); var BMP = getBitmap(sender.Parent, ThicknessColor);
            //верхняя горизонталь
            for (int y = 0; y < Thickness; y++) for (int x = 0; x < sender.Width; x++) {
                    int X = x + sender.Left, Y = y + sender.Top;
                    if (bmp.GetPixel(x, y) == ThicknessColor) bmp.SetPixel(x, y, BMP.GetPixel(X, Y));
                }
            //нижняя горизонталь
            for (int y = sender.Height - Thickness; y < sender.Height; y++) for (int x = 0; x < sender.Width; x++) {
                    int X = x + sender.Left, Y = y + sender.Top;
                    if (bmp.GetPixel(x, y) == ThicknessColor) bmp.SetPixel(x, y, BMP.GetPixel(X, Y));
                }
            //левая вертикаль
            for (int x = 0; x < Thickness; x++) for (int y = 0; y < sender.Height; y++) {
                    int X = x + sender.Left, Y = y + sender.Top;
                    if (bmp.GetPixel(x, y) == ThicknessColor) bmp.SetPixel(x, y, BMP.GetPixel(X, Y));
                }
            //правая вертикаль
            for (int x = sender.Width - Thickness; x < sender.Width; x++) for (int y = 0; y < sender.Height; y++) {
                    int X = x + sender.Left, Y = y + sender.Top;
                    if (bmp.GetPixel(x, y) == ThicknessColor) bmp.SetPixel(x, y, BMP.GetPixel(X, Y));
                }
            sender.BackgroundImage = bmp;
        }

        /// <summary>
        ///     Функция рисует рамку в виде уголков у любого визуального элемента управления унаследованного от Control
        /// </summary>
        /// <remarks> 
        ///     <b> this Control: </b> элемент управления в котором рисуются уголки. <br/>
        ///     <b> Thickness: </b> толщина уголков в пикселях. <br/>
        ///     <b> Percent: </b> размер уголка в процентах от общего размера this Control в пикселях. <br/>
        ///     при Percent = 100% уголки нарисуются сплошной линией, всё равно что вызвать Extensions.Contour_Solid(...); <br/>
        ///     <b> ThicknessColor: </b> цвет уголков. <br/>
        /// </remarks>
        public static void Contour_Angles(this Control sender, byte Thickness, byte Percent, Color ThicknessColor) {
            if (Thickness <= 0) return; else if (Percent > 100) Percent = 100;
            var bmp = new Bitmap(sender.Width, sender.Height); sender.DrawToBitmap(bmp, sender.ClientRectangle);
            BitmapData Data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            int length = bmp.Width * bmp.Height * 4; byte[] RGBA = new byte[length];//создать массив
            System.Runtime.InteropServices.Marshal.Copy(Data.Scan0, RGBA, 0, length);//скопировать RGB bmp в массив 
            //верхняя горизонталь
            for (int y = 0; y < Thickness; y++) {
                for (int x = 0; x < sender.Width * (Percent / 100.0); x++) { int i = (y * bmp.Width + x) * 4;
                    RGBA[i + 3] = ThicknessColor.A; RGBA[i + 2] = ThicknessColor.R;
                    RGBA[i + 1] = ThicknessColor.G; RGBA[i + 0] = ThicknessColor.B;
                }
                for (int x = (int)(sender.Width * (1.0 - (Percent / 100.0))); x < sender.Width; x++) { int i = (y * bmp.Width + x) * 4;
                    RGBA[i + 3] = ThicknessColor.A; RGBA[i + 2] = ThicknessColor.R;
                    RGBA[i + 1] = ThicknessColor.G; RGBA[i + 0] = ThicknessColor.B;
                }
            }
            //нижняя горизонталь
            for (int y = sender.Height - Thickness; y < sender.Height; y++) {
                for (int x = 0; x < sender.Width * (Percent / 100.0); x++) { int i = (y * bmp.Width + x) * 4;
                    RGBA[i + 3] = ThicknessColor.A; RGBA[i + 2] = ThicknessColor.R;
                    RGBA[i + 1] = ThicknessColor.G; RGBA[i + 0] = ThicknessColor.B;
                }
                for (int x = (int)(sender.Width * (1.0 - (Percent / 100.0))); x < sender.Width; x++) { int i = (y * bmp.Width + x) * 4;
                    RGBA[i + 3] = ThicknessColor.A; RGBA[i + 2] = ThicknessColor.R;
                    RGBA[i + 1] = ThicknessColor.G; RGBA[i + 0] = ThicknessColor.B;
                }
            }
            //левая вертикаль
            for (int x = 0; x < Thickness; x++) {
                for (int y = 0; y < sender.Height * (Percent / 100.0); y++) { int i = (y * bmp.Width + x) * 4;
                    RGBA[i + 3] = ThicknessColor.A; RGBA[i + 2] = ThicknessColor.R;
                    RGBA[i + 1] = ThicknessColor.G; RGBA[i + 0] = ThicknessColor.B;
                }
                for (int y = (int)(sender.Height * (1.0 - (Percent / 100.0))); y < sender.Height; y++) { int i = (y * bmp.Width + x) * 4;
                    RGBA[i + 3] = ThicknessColor.A; RGBA[i + 2] = ThicknessColor.R;
                    RGBA[i + 1] = ThicknessColor.G; RGBA[i + 0] = ThicknessColor.B;
                }
            }
            //правая вертикаль
            for (int x = sender.Width - Thickness; x < sender.Width; x++) {
                for (int y = 0; y < sender.Height * (Percent / 100.0); y++) { int i = (y * bmp.Width + x) * 4;
                    RGBA[i + 3] = ThicknessColor.A; RGBA[i + 2] = ThicknessColor.R;
                    RGBA[i + 1] = ThicknessColor.G; RGBA[i + 0] = ThicknessColor.B;
                }
                for (int y = (int)(sender.Height * (1.0 - (Percent / 100.0))); y < sender.Height; y++) { int i = (y * bmp.Width + x) * 4;
                    RGBA[i + 3] = ThicknessColor.A; RGBA[i + 2] = ThicknessColor.R;
                    RGBA[i + 1] = ThicknessColor.G; RGBA[i + 0] = ThicknessColor.B;
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(RGBA, 0, Data.Scan0, length); bmp.UnlockBits(Data);
            sender.BackgroundImage = bmp;
        }

        /// <summary>
        ///     Функция рисует сплошную рамку у любого визуального элемента управления унаследованного от Control
        /// </summary>
        /// <remarks> 
        ///     <b> this Control: </b> элемент управления в котором рисуется рамка. <br/>
        ///     <b> Thickness: </b> толщина рамки в пикселях. <br/>
        ///     <b> ThicknessColor: </b> цвет рамки. <br/>
        /// </remarks>
        public static void Contour_Solid(this Control sender, byte Thickness, Color ThicknessColor) {
            if (Thickness <= 0) return; var bmp = new Bitmap(sender.Width, sender.Height);
            sender.DrawToBitmap(bmp, sender.ClientRectangle);
            BitmapData Data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            int length = bmp.Width * bmp.Height * 4; byte[] RGBA = new byte[length];//создать массив
            System.Runtime.InteropServices.Marshal.Copy(Data.Scan0, RGBA, 0, length);//скопировать RGB bmp в массив 
            //верхняя горизонталь
            for (int y = 0; y < Thickness; y++) for (int x = 0; x < bmp.Width; x++) {
                    int i = (y * bmp.Width + x) * 4;
                    RGBA[i + 3] = ThicknessColor.A; RGBA[i + 2] = ThicknessColor.R;
                    RGBA[i + 1] = ThicknessColor.G; RGBA[i + 0] = ThicknessColor.B;
                }
            //нижняя горизонталь
            for (int y = bmp.Height - Thickness; y < bmp.Height; y++) for (int x = 0; x < bmp.Width; x++) {
                    int i = (y * bmp.Width + x) * 4;
                    RGBA[i + 3] = ThicknessColor.A; RGBA[i + 2] = ThicknessColor.R;
                    RGBA[i + 1] = ThicknessColor.G; RGBA[i + 0] = ThicknessColor.B;
                }
            //левая вертикаль
            for (int x = 0; x < Thickness; x++) for (int y = 0; y < bmp.Height; y++) {
                    int i = (y * bmp.Width + x) * 4;
                    RGBA[i + 3] = ThicknessColor.A; RGBA[i + 2] = ThicknessColor.R;
                    RGBA[i + 1] = ThicknessColor.G; RGBA[i + 0] = ThicknessColor.B;
                }
            //правая вертикаль
            for (int x = bmp.Width - Thickness; x < bmp.Width; x++) for (int y = 0; y < bmp.Height; y++) {
                    int i = (y * bmp.Width + x) * 4;
                    RGBA[i + 3] = ThicknessColor.A; RGBA[i + 2] = ThicknessColor.R;
                    RGBA[i + 1] = ThicknessColor.G; RGBA[i + 0] = ThicknessColor.B;
                }
            System.Runtime.InteropServices.Marshal.Copy(RGBA, 0, Data.Scan0, length); bmp.UnlockBits(Data);
            sender.BackgroundImage = bmp;
        }
        //===========================================================================================================================
        //===================================================== Control.Contour =====================================================
        //=========================================================== END ===========================================================

        public struct fVec2 { public float X, Y; public fVec2(float x, float y) { X = x; Y = y; } }
        /// <summary>
        ///     Метод преобразовывает координаты мыши относительно контрола в координаты относительно сетки карты и <br/>
        ///     переворачивает карту в изометрию. (0, 0) находистя слева Bitmap-а. <b>pb_Map.Left = 0; pb_Map.Top = pb_Map.Height / 2;</b>
        /// </summary>
        /// <value>
        ///     <b> <paramref name="MouseLocation"/>: </b> Координаты мыши относительно контрола. <br/>
        ///     <b> <paramref name="ScaleMap"/>: </b> масштаб карты (Bitmap контрола pb_Map). <br/>
        /// </value>
        /// <returns>
        ///     Возвращает преобразованные в изометрию координаты ячейки в пределах размеров карты. <br/>
        ///     Если координаты находятся за пределами границ видимой карты, метод вернёт <b>Point(-1, -1);</b>
        /// </returns>
        public static Point mouse2romb(Point MouseLocation, double ScaleMap) {
            fVec2 m = new fVec2(MouseLocation.X, MouseLocation.Y);
            Size size = new Size((int)(102 * ScaleMap), (int)(58 * ScaleMap));//размер спрайта одного ромба
            m.X /= size.Width; m.Y /= size.Height;
            fVec2 r = new fVec2(m.X + m.Y, m.X - m.Y);
            if (r.X > 0F && r.Y > 0F) return new Point((int)r.X, (int)r.Y);
            else return new Point(-1, -1);
        }

        /// <summary> Метод преобразовывает координаты курсора на карте в координаты относительно массива <b>Map.Cell[][];</b> </summary>
        /// <value> 
        ///     <b><paramref name="map"/>:</b> ссылка на карту <b>TPlayer.TMap</b> <br/>
        ///     <b><paramref name="cursor"/>:</b> координаты курсора на карте. <br/>
        ///     <b><paramref name="size"/>:</b> размер карты в ячейках. (default = 7x7) <br/>
        ///     <b><paramref name="FULL_SCREEN_MAP"/>:</b> режим отображения карты. true = увеличенный, false = обычный. (default = false) <br/>
        /// </value>
        /// <returns> 
        ///     Возвращает <b>Point</b> структуру. Координаты в массиве <b>Map.Cell[ <paramref name="Point.X"/> ][ <paramref name="Point.Y"/> ];</b> <br/>
        ///     Если координаты вычислились некорректно (за пределами карты), метод вернёт <b>Point(-1, -1);</b>
        /// </returns>
        public static Point CursorToCell(this Control sender, TGame.TMap map, Point cursor, int size = 7,
                                         bool FULL_SCREEN_MAP = false) {
            double _ScaleMap = map.ScaleMap(size, FULL_SCREEN_MAP);
            Point XY = mouse2romb(new Point(cursor.X, cursor.Y - sender.Height / 2), _ScaleMap);
            //если клик курсора в пределах размеров видимой карты, например x[0..6], y[0..6] при карте 7х7
            if (XY.X >= 0 && XY.X < size && XY.Y >= 0 && XY.Y < size) {
                return map.Cell_to_Tor(XY.X + map.Location_X0_Y0.X, XY.Y + map.Location_X0_Y0.Y);//коры в массиве
            } else return new Point(-1, -1);
        }

        /// <summary>
        ///     Метод ищет подстроку <paramref name="FindText"/> в тексте <b>RichTextBox</b> с точным соответствием регистра
        ///     и вставляет в него строку <paramref name="ReplaceText"/> если подстрока найдена. <br/><br/>
        ///     Если требуется вставить подстроку перед найденным фрагментом текста сохранив искомый текст,
        ///     в параметр <paramref name="SelectionLength"/> следует установить <b>0.</b> <br/>
        ///     Если нужно заменить найденную подстроку, тогда в параметр <paramref name="SelectionLength"/> следует установить 
        ///     значение равное количеству затираемых символов, <br/>
        ///     например <paramref name="SelectionLength"/> = <paramref name="FindText"/>.Length;
        /// </summary>
        /// <remarks> Если не задавать название, размер и стиль шрифта, то к выделенному фрагменту применятся свойства шрифта, которые установлены для выделенного фрагмента, т.е. всё останется как было. </remarks>
        /// <value>
        ///     <b> <paramref name="FindText"/>: </b> искомая текстовая подстрока в <b>RichTextBox</b>. <br/>
        ///     <b> <paramref name="ReplaceText"/>: </b> вставляемая текстовая строка в <b>RichTextBox</b>. <br/>
        ///     <b> <paramref name="ReplaceColor"/>: </b> цвет шрифта вставляемого текста в <b>RichTextBox</b>. <br/>
        ///     <b> <paramref name="SelectionLength"/>: </b> число символов выделенных в <b>RichTextBox</b>. <br/>
        ///     <b> <paramref name="FamilyName"/>: </b> название шрифта в <b>RichTextBox</b>. <br/>
        ///     <b> <paramref name="FontSize"/>: </b> размер шрифта вставляемого текста заданного для разрешения экрана <b>1920х1080.</b> (размер подстроится под текущее разрешение экрана автоматически) <br/>
        ///     <b> <paramref name="SelectionFontStyle"/>: </b> сведения о стиле, применяемые к вставляемому тексту в <b>RichTextBox</b>. [bold, regular, italic и т.д.] <br/>
        /// </value>
        public static void Find_and_Replace(this RichTextBox sender, string FindText, string ReplaceText,
                           Color ReplaceColor, int SelectionLength,
                           string FamilyName = default, float FontSize = default, FontStyle SelectionFontStyle = default)
        {
            int pos = sender.Find(FindText, RichTextBoxFinds.MatchCase);
            if (pos > -1) {
                string FN = FamilyName == default ? sender.SelectionFont.FontFamily.Name/*"Courier New"*/ : FamilyName;
                float FS = FontSize == default ? sender.SelectionFont.Size : FontSize;
                FontStyle SFS = SelectionFontStyle == default ? sender.SelectionFont.Style : SelectionFontStyle;
                sender.SelectionFont = new Font(FN, ToCSR(FS), SFS);
                sender.SelectionColor = ReplaceColor; sender.SelectionStart = pos;
                sender.SelectionLength = SelectionLength;
                if (ReplaceText != "") sender.SelectedText = ReplaceText;
                else {
                    sender.SelectionLength = SelectionLength + 1;
                    sender.SelectedText = sender.SelectedText[sender.SelectedText.Length - 1].ToString();
                }
            }
        }

        /// <summary> Метод находит ссылку в тексте RichTextBox: <b>rich_Info_Res</b> и применяет к ней атрибуты шрифта. <br/> Важно! Ссылка должна начинаться с <b>'https://'</b> и в тексте не должно быть больше одной ссылки, т.к. метод находит только первую ссылку и игнорирует все остальные. </summary>
        /// <remarks> <inheritdoc cref="Find_and_Replace"/> <br/> Не работает если <b>sender.DetectUrls = true</b>, а при <b>false</b> контрол читает ссылку как обычный текст. Если переключать туда-сюда, то весь текст пропадает. </remarks>
        /// <value>
        ///     <b> <paramref name="SelectionColor"/>: </b> цвет выделенного текста в <b>RichTextBox</b>. <br/>
        ///     <b> <paramref name="FamilyName"/>: </b> название шрифта в <b>RichTextBox</b>. <br/>
        ///     <b> <paramref name="FontSize"/>: </b> размер шрифта вставляемого текста заданного для разрешения экрана <b>1920х1080.</b> (размер подстроится под текущее разрешение экрана автоматически) <br/>
        ///     <b> <paramref name="SelectionFontStyle"/>: </b> сведения о стиле, применяемые к вставляемому тексту в <b>RichTextBox</b>. [bold, regular, italic и т.д.] <br/>
        /// </value>
        public static void Find_and_PaintLink(this RichTextBox sender, Color SelectionColor,
            string FamilyName = default, float FontSize = default, FontStyle SelectionFontStyle = default) {
            int start = sender.Find("<https://"); int end = sender.Find(">") - 1;
            if (start < 0 || end < 0) return; //ссылка не найдена
            string FN = FamilyName == default ? sender.SelectionFont.FontFamily.Name/*"Courier New"*/ : FamilyName;
            float FS = FontSize == default ? sender.SelectionFont.Size : FontSize;
            FontStyle SFS = SelectionFontStyle == default ? sender.SelectionFont.Style : SelectionFontStyle;
            sender.SelectionFont = new Font(FN, ToCSR(FS), SFS);
            sender.SelectionColor = SelectionColor; 
            sender.SelectionStart = start; sender.SelectionLength = end - start;
            //string text = sender.SelectedText;
        }

        /// <summary> 
        ///     Метод заливает в ToolStripMenu все <b>языки</b> для которых существует соответствующая папка в <b>DATA_BASE/LANGUAGES/</b>, <br/>
        ///     чтобы можно было выбрать какой <b>язык</b> грузить. <br/> <br/>
        ///     <b> this <paramref name="ToolStripMenuItem"/>: </b> вершинное меню от которого построятся подменю. <br/>
        ///     <b> <paramref name="index"/>: </b> поле <b>GAME.Language</b>. Номер папки-языка в пути <b>"DATA_BASE/LANGUAGES/"</b>. Номера соответствуют отсортированному списку в алфавитном порядке. <br/>
        ///     <b> <paramref name="Event_onClick"/>: </b> Событие EventHandler обрабатывающее клик левой кнопкой мыши по элементу меню. <br/>
        ///     <b> <paramref name="Languages"/>: </b> <inheritdoc cref="WindowsInterface.Form1.TLANGUAGES"/> <br/>
        /// </summary>
        public static void Load_SubMenu_Language_FromFolder(this ToolStripMenuItem TSMI, int index,
                            EventHandler Event_onClick, WindowsInterface.Form1.TLANGUAGES Languages)
        {
            TSMI.DropDownItems.Clear();
            string[] tmp = Directory.GetDirectories("DATA_BASE/LANGUAGES/");
            if (tmp == null) { MessageBox.Show("Error 17.\nОшибка в class UFO.Extensions.Load_SubMenu_Language_FromFolder(...)\n" +
                "Каталоги языков в: 'DATA_BASE/LANGUAGES/...' не найдены."); return; }
            else if (index >= tmp.Length) { MessageBox.Show("Error 18.\nОшибка в class UFO.Extensions.Load_SubMenu_Language_FromFolder(...)\n" +
                "значение 'index' (" + index + ") больше фактического количества языков (" + tmp.Length + ")."); return; }
            for (int i = 0; i < tmp.Length; i++) {
                var menu = new ToolStripMenuItem() { Name = "Language " + i, Text = Languages.languages[i], Tag = i };
                if (i == index) menu.Checked = true; else menu.Checked = false;
                menu.Click += new EventHandler(Event_onClick);
                TSMI.DropDownItems.Add(menu);
            }
        }

        /// <summary> Метод заливает в ToolStripMenu все созданные ранее аккаунты списком, чтобы можно было выбрать какой аккаунт грузить. </summary>
        /// <remarks> 
        ///     <b> this ToolStripMenuItem: </b> вершинное меню от которого построятся подменю. <br/>
        ///     <b> Event_onClick: </b> событие EventHandler обрабатывающее клик левой кнопкой мыши по элементу меню. <br/>
        /// </remarks>
        public static void Load_SubMenu_Account_FromFolder(this ToolStripMenuItem TSMI, EventHandler Event_onClick) {
            TSMI.DropDownItems.Clear();
            string[] tmp = Directory.GetDirectories("DATA_BASE/ACCOUNTS/");
            int CountAccounts = 0;//количество аккаунтов Players
            for (int i = 0; i < tmp.Length; i++) if (tmp[i][tmp[i].Length - 1] != ']') CountAccounts++;
            int N = 0; string[] path = new string[CountAccounts];
            for (int i = 0; i < tmp.Length; i++) if (tmp[i][tmp[i].Length - 1] != ']') { path[N] = tmp[i]; N++; }

            if (path == null) { MessageBox.Show("Error 19.\nОшибка в class UFO.Extensions.Load_SubMenu_Account_FromFolder(...)\n" +
                "Каталоги в: 'DATA_BASE/ACCOUNTS/...' не найдены."); return; }
            for (int i = 0; i < path.Length; i++) {
                string account = ""; for (int j = path[i].Length - 1; j > 0; j--) if (path[i][j] == '/') {
                        for (int k = j + 1; k < path[i].Length; k++) account += path[i][k]; break;
                    }
                ToolStripMenuItem menu = new ToolStripMenuItem() {
                    Name = "account " + i,
                    Text = account,
                };
                menu.Click += new EventHandler(Event_onClick);
                TSMI.DropDownItems.Add(menu);
            }
        }

        /// <summary>
        ///     Метод создаёт настраиваемую всплывающую подсказку классом <paramref name="ToolTip"/> библиотеки <b>C#</b> и возвращает её для построения коллекции подсказок.
        /// </summary>
        /// <remarks> 
        ///     <b> Control: </b> Control для которого нужно создать подсказку. <br/>
        ///     <b> ReshowDelay: </b> интервал времени, который должен пройти перед появлением окна очередной всплывающей подсказки в милисекундах. <br/>
        ///     <b> AutoPopDelay: </b> интервал времени, в течении которого всплывающая подсказка отображается,
        ///                            когда указатель мыши останавливается в границах Control в милисекундах. <br/>
        ///     <b> HintTitle: </b> заголовок окна всплывающей подсказки. <br/>
        ///     <b> HintText: </b> текст всплывающей подсказки. <br/>
        ///     <b> TextColor: </b> цвет текста всплывающей подсказки. <br/>
        ///     <b> BackColor: </b> цвет фона всплывающей подсказки. <br/>
        ///     <b> ToolTipIcon: </b> (enum перечисление) тип значка, отображаемого вместе с текстом всплывающей подсказки. <br/>
        ///     <b> IsBalloon: </b> true - использовать всплывающее окно, false - стандартное прямоугольное окно. <br/>
        ///     Варианты enum: <br/>
        ///     None = 0 (Значок сведений), Info = 1 (Значок предупреждения), Warning = 2 (Значок ошибки), Error = 3 (ошибка). <br/>
        /// </remarks>
        /// <returns> Возвращает ToolTip подсказку. </returns>
        public static ToolTip CreateHint(Control Control, int ReshowDelay, int AutoPopDelay, string HintTitle, string HintText,
                                         Color TextColor, Color BackColor, ToolTipIcon ToolTipIcon, bool IsBalloon) {
            ToolTip hint = new ToolTip() {
                InitialDelay = 0, ReshowDelay = ReshowDelay, AutoPopDelay = AutoPopDelay, ToolTipTitle = HintTitle,
                BackColor = BackColor, ForeColor = TextColor, IsBalloon = IsBalloon, ToolTipIcon = ToolTipIcon,
            };
            hint.SetToolTip(Control, HintText);
            return hint;
        }

        /// <summary> Метод автоматически изменяет цвет и атрибуты шрифта фрагментов текста для каждой ячейки, которая удовлетворяет критериям алгоритма. </summary>
        /// <remarks>
        ///     Берётся текст ячейки => <br/> парсится сплитом по сепаратору <b>"/"</b> => <br/> каждый чётный фрагмент текста оставляется как есть => <br/> к каждому не чётному применяется алгоритм форматирования (цвет и жирность). <br/>
        ///     Пример: <br/> .Value = "[0] Земляной вал: разрушен с уровня / [1] 15 / [2] до уровня / [3] 7". <br/>
        ///     В квадратных скобках [] фрагменты, которые попадут в массив. <br/> [0, 2] - останутся как есть, [1, 3] - изменятся.
        /// </remarks>
        /// <value> <b> <paramref name="Draw"/>: </b> <b>true</b> = включить авто раскраску, <b>false</b> = выключить. <br/> </value>
        public static void TextMultiColorCell(this DataGridView sender, bool Draw) {
            if (Draw) { sender.CellPainting -= grid_CellPainting; sender.CellPainting += grid_CellPainting; }
            else sender.CellPainting -= grid_CellPainting;
        }
        private static void grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e) {
            var grid = sender as DataGridView;
            if (grid.Rows.Count <= 0 || grid.Columns.Count <= 0 ||
                e.Value.GetType() != typeof(string) || e.Value == null || e.Value.ToString() == "" ||
                e.Value.ToString().Split('/') == null || e.Value.ToString().Split('/').Length <= 1) return;
            if (!e.Handled) { e.Handled = true; e.PaintBackground(e.CellBounds, false); }
            if ((e.PaintParts & DataGridViewPaintParts.ContentForeground) != DataGridViewPaintParts.None) {
                string text = e.Value.ToString(); var SplitText = text.Split('/');
                Size[] SplitSize = new Size[SplitText.Length];
                for (int i = 0; i < SplitText.Length; i++) SplitSize[i] = TextRenderer.MeasureText(SplitText[i], e.CellStyle.Font);
                Rectangle rect = new Rectangle(e.CellBounds.Location, e.CellBounds.Size);
                rect.X += grid.DefaultCellStyle.Padding.Left;//поправка на ветер
                rect.Y += grid.DefaultCellStyle.Padding.Top;//поправка на ветер
                for (int i = 0; i < SplitText.Length; i++) {
                    if (i % 2 == 0) e.Graphics.DrawString(SplitText[i], e.CellStyle.Font, new SolidBrush(e.CellStyle.ForeColor), rect);
                    else {
                        Font FontItalic = new Font(e.CellStyle.Font.Name, e.CellStyle.Font.Size, FontStyle.Italic);
                        Font FontBold = new Font(e.CellStyle.Font.Name, e.CellStyle.Font.Size, FontStyle.Bold);
                        if (SplitText[i][0] == '[') e.Graphics.DrawString(SplitText[i], FontItalic, new SolidBrush(e.CellStyle.ForeColor), rect);
                        else e.Graphics.DrawString(SplitText[i], FontBold, new SolidBrush(e.CellStyle.ForeColor)/*Brushes.Navy*/, rect);
                    }
                    rect.X += SplitSize[i].Width;
                }
            }
        }

        /// <summary> Метод сниамет выделение всех ячеек таблицы <b>DataGridView</b> с помощью костыля и такой-то матери. </summary>
        public static void CancelSelection(this DataGridView sender) {
            if (sender.Rows.Count > 0) { sender[0, 0].Selected = true; sender[0, 0].Selected = false; }
        }

        /// <summary> Метод подгоняет габариты всей таблицы DataGridView под её содержимое с учётом заголовков, если они отображаются и с учётом свойства <b>grid.DefaultCellStyle.Padding;</b> </summary>
        /// <remarks> <b>sender.Width / sender.Height</b> изменяют свои значения. </remarks>
        public static void UpdateSize(this DataGridView sender) {
            if (sender.Columns[0] == null) return;
            var padding = sender.DefaultCellStyle.Padding;
            int Width = 0; int Height = ToCSR(5);
            if (sender.ColumnHeadersVisible) Height += sender.Columns[0].HeaderCell.PreferredSize.Height;//ColumnHeadersHeight;//строка сверху
            if (sender.RowHeadersVisible) Width += sender.Columns[0].HeaderCell.PreferredSize.Width;//RowHeadersWidth;//столбец слева
            for (int x = 0; x < sender.Columns.Count; x++) { int MAX_W;
                MAX_W = !sender.ColumnHeadersVisible ? 0 : sender.Columns[x].HeaderCell.PreferredSize.Width;//ширина заголовков
                    //TextRenderer.MeasureText(sender.Columns[x].HeaderCell.Value.ToString(), sender.Columns[x].HeaderCell.Style.Font).Width; 
                for (int y = 0; y < sender.Rows.Count; y++) { int w;
                  sender.Columns[x].Width = sender.Rows[y].Cells[x].Size.Width + padding.Left + padding.Right;//+ padding
                  w = sender[x, y].PreferredSize.Width;//ширина ячеек
                  //w = TextRenderer.MeasureText(sender.Rows[y].Cells[x].Value.ToString(), sender.Columns[x].DefaultCellStyle.Font).Width;
                  //w = (int)(sender.Rows[y].Cells[x].Value.ToString().Length * sender.Columns[x].DefaultCellStyle.Font.Size);
                  //w = (new Label() { Text = sender.Rows[y].Cells[x].Value.ToString(), Font = sender.Columns[x].DefaultCellStyle.Font, AutoSize = true }).Width;
                    if (MAX_W < w) MAX_W = w;
                    if (x == 0) sender.Rows[y].Height = sender.Rows[y].Height + padding.Top + padding.Bottom;//+ padding
                    if (x == 0) Height += sender[x, y].PreferredSize.Height;
                } Width += MAX_W + 2;//+2 с запасом для текстов Bold
            } sender.ClientSize = new Size(Width, Height);
        }

        /// <summary> Метод форматирует таблицу и её ячейки вторичными значениями. <br/> Длины всех массивов должны совпадать с кол-вом колонок <b>Columns</b>. </summary>
        /// <remarks>
        ///     <b> <paramref name="DefaultCellStyle_BackColor"/>: </b> цвет фона каждой ячейки. <br/>
        ///     <b> <paramref name="ColumnsDefaultCellStyleFont"/>[]: </b> массив атрибутов шрифта <b>ЯЧЕЕК</b> для каждого столбца. <br/>
        ///     <b> <paramref name="AutoSizeMode"/>[]: </b> массив автоизменения ширины каждого столбца под содержимое. <br/>
        ///     <b> <paramref name="DefaultCellStyle_Alignment"/>[]: </b> массив выравнивания содержимого <b>ЯЧЕЕК</b> относительно верх/низ, лево/право для каждого столбца. <br/>
        ///     <b> <paramref name="AutoSizeRowsMode"/>: </b> автоизменение высоты всех строк под содержимое. <br/>
        ///     <b> <paramref name="ScrollBars"/>: </b> указывает, какие полосы прокрутки будут видны в таблице (по умолчанию - все). <br/>
        ///     <b> <paramref name="ColumnHeadersHeightSizeMode"/>: </b> автоизменение высоты <b>ЗАГОЛОВКОВ</b>. <br/>
        ///     <b> <paramref name="RowHeadersWidthSizeMode"/>: </b> автоизменение ширины <b>ЗАГОЛОВКОВ</b>. <br/>
        ///     <b> <paramref name="ColumnsDefaultCellStyleFont_Header"/>[]: </b> массив атрибутов шрифта <b>ЗАГОЛОВКОВ</b> для каждого столбца. <br/>
        ///     <b> <paramref name="DefaultCellStyle_Alignment_Header"/>[]: </b> массив выравнивания содержимого <b>ЗАГОЛОВКОВ</b> относительно верх/низ, лево/право для каждого столбца. <br/>
        ///     <b> <paramref name="DefaultCellStyle_SelectionBackColor"/>: </b> цвет фона выбранной ячейки <b>[Select]</b>. <br/>
        ///     <b> <paramref name="DefaultCellStyle_SelectionForeColor"/>: </b> цвет текста выбранной ячейки <b>[Select]</b>. <br/>
        /// </remarks>
        public static void Settings(this DataGridView sender, Color DefaultCellStyle_BackColor,
                        Font[] ColumnsDefaultCellStyleFont,
                        DataGridViewAutoSizeColumnMode[] AutoSizeMode,
                        DataGridViewContentAlignment[] DefaultCellStyle_Alignment,
                        DataGridViewAutoSizeRowsMode AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None,
                        ScrollBars ScrollBars = ScrollBars.Both,
                        DataGridViewColumnHeadersHeightSizeMode ColumnHeadersHeightSizeMode = default,
                        DataGridViewRowHeadersWidthSizeMode RowHeadersWidthSizeMode = default,
                        Font[] ColumnsDefaultCellStyleFont_Header = default,
                        DataGridViewContentAlignment[] DefaultCellStyle_Alignment_Header = default,
                        Color DefaultCellStyle_SelectionBackColor = default,
                        Color DefaultCellStyle_SelectionForeColor = default
                        ) {
            if (DefaultCellStyle_SelectionBackColor == default) DefaultCellStyle_SelectionBackColor = Color.Blue;
            if (DefaultCellStyle_SelectionForeColor == default) DefaultCellStyle_SelectionForeColor = Color.White;

            sender.DefaultCellStyle.BackColor = DefaultCellStyle_BackColor;//цвет фона каждой ячейки
            //скрыть выделение ячеек
            sender.DefaultCellStyle.SelectionBackColor = DefaultCellStyle_SelectionBackColor;
            sender.DefaultCellStyle.SelectionForeColor = DefaultCellStyle_SelectionForeColor;
            //автоматическое изменение ширины столбцов чтобы влезли заголовки
            sender.ColumnHeadersHeightSizeMode = ColumnHeadersHeightSizeMode;
            sender.RowHeadersWidthSizeMode = RowHeadersWidthSizeMode;
            sender.AutoSizeRowsMode = AutoSizeRowsMode;//авто изменение высоты строк под содержимое
            sender.ScrollBars = ScrollBars;
            for (int i = 0; i < ColumnsDefaultCellStyleFont.Length; i++) {
                //заголовки
                if (ColumnsDefaultCellStyleFont_Header != null)
                    sender.Columns[i].HeaderCell.Style.Font = ColumnsDefaultCellStyleFont_Header[i];
                if (DefaultCellStyle_Alignment_Header != null)
                    sender.Columns[i].HeaderCell.Style.Alignment = DefaultCellStyle_Alignment_Header[i];
                //ячейки 
                sender.Columns[i].DefaultCellStyle.Font = ColumnsDefaultCellStyleFont[i];//шрифт
                sender.Columns[i].DefaultCellStyle.Alignment = DefaultCellStyle_Alignment[i];//выравнивание содержимого ячейки относительно верх/низ, лево/право
                //ячейки + заголовки
                //sender.AutoSizeColumnsMode
                sender.Columns[i].AutoSizeMode = AutoSizeMode[i];//авто изменение ширины столбца под содержимое
            }
        }

        /// <summary> 
        ///    Метод Создаёт таблицу <b>DataGridView</b> и заполняет объект основными значениями. <br/> <br/>
        ///    <b> <paramref name="Parent"/>: </b> <inheritdoc cref="Control.Parent"/> <br/>
        ///    <b> <paramref name="Name"/>: </b> <inheritdoc cref="Control.Name"/> <br/>
        ///    <b> <paramref name="Font"/>: </b> <inheritdoc cref="DataGridView.Font"/> <br/>
        ///    <b> <paramref name="Location"/>: </b> <inheritdoc cref="Control.Location"/> <br/>
        ///    <b> <paramref name="BackgroundColor"/>: </b> <inheritdoc cref="DataGridView.BackgroundColor"/> <br/>
        ///    <b> <paramref name="GridColor"/>: </b> <inheritdoc cref="DataGridView.GridColor"/> <br/>
        ///    <b> <paramref name="ForeColor"/>: </b> <inheritdoc cref="DataGridView.ForeColor"/> <br/>
        ///    <b> <paramref name="BorderStyle"/>: </b> <inheritdoc cref="DataGridView.BorderStyle"/> (default = <inheritdoc cref="BorderStyle.None"/>) <br/>
        ///    <b> <paramref name="AllowUserToAddRows"/>: </b> <inheritdoc cref="DataGridView.AllowUserToAddRows"/> <br/>
        ///    <b> <paramref name="AllowUserToDeleteRows"/>: </b> <inheritdoc cref="DataGridView.AllowUserToDeleteRows"/> <br/>
        ///    <b> <paramref name="AllowUserToResizeRows"/>: </b> <inheritdoc cref="DataGridView.AllowUserToResizeRows"/> <br/>
        ///    <b> <paramref name="AllowUserToResizeColumns"/>: </b> <inheritdoc cref="DataGridView.AllowUserToResizeColumns"/> <br/>
        ///    <b> <paramref name="RowHeadersVisible"/>: </b> <inheritdoc cref="DataGridView.RowHeadersVisible"/> <br/>
        ///    <b> <paramref name="ColumnHeadersVisible"/>: </b> <inheritdoc cref="DataGridView.ColumnHeadersVisible"/> <br/>
        ///    <b> <paramref name="MultiSelect"/>: </b> <inheritdoc cref="DataGridView.MultiSelect"/> <br/>
        ///    <b> <paramref name="ReadOnly"/>: </b> <inheritdoc cref="DataGridView.ReadOnly"/> <br/>
        ///    <b> <paramref name="AutoSize"/>: </b> <inheritdoc cref="DataGridView.AutoSize"/> <br/>
        ///    <b> <paramref name="Enabled"/>: </b> Возвращает или задаёт значение, может ли элемент управления отвечать на действия пользователя. <br/>
        /// </summary>
        /// <returns> Возвращает таблицу. Экземпляр класса <b>DataGridView</b>. </returns>
        public static DataGridView CreateGrid(Control Parent, string Name, Font Font, Point Location = default, 
                        Color BackgroundColor = default, Color GridColor = default, Color ForeColor = default,
                        BorderStyle BorderStyle = BorderStyle.None, bool AllowUserToAddRows = false,
                        bool AllowUserToDeleteRows = false, bool AllowUserToResizeRows = false,
                        bool AllowUserToResizeColumns = false, bool RowHeadersVisible = false,
                        bool ColumnHeadersVisible = false, bool MultiSelect = false, bool ReadOnly = false,
                        bool AutoSize = false, bool Enabled = false) {
            //программная инициализация стандартной таблицы
            if (Location == default) Location = new Point(0, 0);
            if (BackgroundColor == default) BackgroundColor = Color.White;
            if (GridColor == default) GridColor = Color.Black;
            if (ForeColor == default) ForeColor = Color.Black;
            return new DataGridView { Parent = Parent, Name = Name, Location = Location,
                //Font
                Font = Font,
                //Color
                BackgroundColor = BackgroundColor, GridColor = GridColor,
                ForeColor = ForeColor,
                //enum
                BorderStyle = BorderStyle,
                //bool
                AllowUserToAddRows = AllowUserToAddRows,/*убирает пустую строку*/
                AllowUserToDeleteRows = AllowUserToDeleteRows, AllowUserToResizeRows = AllowUserToResizeRows,
                AllowUserToResizeColumns = AllowUserToResizeColumns, RowHeadersVisible = RowHeadersVisible,
                ColumnHeadersVisible = ColumnHeadersVisible, MultiSelect = MultiSelect,
                ReadOnly = ReadOnly, AutoSize = AutoSize, Enabled = Enabled,
            };
        }

        /// <summary>
        ///     Метод Создаёт окно <b>Form</b> и заполняет объект основными значениями. <br/> <br/>
        ///     <b> <paramref name="Name"/>: </b> <inheritdoc cref="Control.Name"/> <br/>
        ///     <b> <paramref name="icon"/>: </b> <inheritdoc cref="Form.Icon"/> <br/>
        ///     <b> <paramref name="font"/>: </b> <inheritdoc cref="Font"/> <br/>
        ///     <b> <paramref name="Text"/>: </b> <inheritdoc cref="Form.Text"/> <br/>
        ///     <b> <paramref name="size"/>: </b> <inheritdoc cref="Form.Size"/> <br/>
        ///     <b> <paramref name="StartPosition"/>: </b> <inheritdoc cref="Form.StartPosition"/> (default = <inheritdoc cref="FormStartPosition.Manual"/>) <br/>
        ///     <b> <paramref name="FormBorderStyle"/>: </b> <inheritdoc cref="Form.FormBorderStyle"/> (default = <inheritdoc cref="FormBorderStyle.None"/>) <br/>
        ///     <b> <paramref name="AutoSizeMode"/>: </b> <inheritdoc cref="Form.AutoSizeMode"/> (default = <inheritdoc cref="AutoSizeMode.GrowAndShrink"/>) <br/>
        ///     <b> <paramref name="WindowState"/>: </b> <inheritdoc cref="Form.WindowState"/> (default = <inheritdoc cref="FormWindowState.Normal"/>) <br/>
        ///     <b> <paramref name="Padding"/>: </b> <inheritdoc cref="Control.Padding"/> (отступы от краёв внутри формы) <br/>
        ///     <b> <paramref name="MinimizeBox"/>: </b> <inheritdoc cref="Form.MinimizeBox"/> <br/>
        ///     <b> <paramref name="MaximizeBox"/>: </b> <inheritdoc cref="Form.MaximizeBox"/> <br/>
        ///     <b> <paramref name="ControlBox"/>: </b> <inheritdoc cref="Form.ControlBox"/> <br/>
        ///     <b> <paramref name="KeyPreview"/>: </b> <inheritdoc cref="Form.KeyPreview"/> <br/>
        ///     <b> <paramref name="AutoSize"/>: </b> <inheritdoc cref="Form.AutoSize"/> <br/>
        ///     <b> <paramref name="AutoScroll"/>: </b> <inheritdoc cref="Form.AutoScroll"/> <br/>
        /// </summary>
        /// <returns> Возвращает окно. Экземпляр класса <b>Form</b>. </returns>
        public static Form CreateForm(string Name, Icon icon, Font font, string Text = default, Size size = default,
                        FormStartPosition StartPosition = FormStartPosition.Manual,
                        FormBorderStyle FormBorderStyle = FormBorderStyle.None,
                        AutoSizeMode AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        FormWindowState WindowState = FormWindowState.Normal,
                        Padding Padding = default,
                        bool MinimizeBox = false, bool MaximizeBox = false, bool ControlBox = false,
                        bool KeyPreview = false, bool AutoSize = false, bool AutoScroll = false) {
            return new Form {
                Name = Name, Icon = icon, Font = font, Text = Text, Size = size,
                StartPosition = StartPosition, FormBorderStyle = FormBorderStyle, AutoSizeMode = AutoSizeMode,
                WindowState = WindowState,
                Padding = Padding/*отступы от краёв*/,
                MinimizeBox = MinimizeBox, MaximizeBox = MaximizeBox, ControlBox = ControlBox,
                KeyPreview = KeyPreview, AutoSize = AutoSize, AutoScroll = AutoScroll
            };
        }
    }
    //======================================================= Extensions =======================================================
}
