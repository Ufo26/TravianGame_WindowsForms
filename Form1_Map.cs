using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Windows.Forms;
using UFO;
using static UFO.Convert;
using static GameLogica.Enums_and_structs;
using static GameLogica.TGame;
using System.Threading.Tasks;

namespace WindowsInterface {
    interface IForm1_Map {
        /// <summary> Метод открывает диалоговое окно с информацией о выбранной ячейке правым кликом мыши по карте. </summary>
        void pb_MAP_MouseClick(object sender, MouseEventArgs e);
        /// <summary> Метод обрабатывает двойной левый клик по карте. Результат кода: центрирование выбранной ячейки. </summary>
        void pb_MAP_MouseDoubleClick(object sender, MouseEventArgs e);
        /// <summary> Метод обрабатывает движение курсора мыши по видимой части карты. </summary>
        void pb_MAP_MouseMove(object sender, MouseEventArgs e);
        /// <summary> Метод делает из элементов контекстного меню аля RadioButton. Запрет на мульти checked. </summary>
        /// <remarks> шаг на 1 ячейку по карте. </remarks>
        void toolStripMenuItem3_Click(object sender, EventArgs e);
        /// <summary> <inheritdoc cref="toolStripMenuItem3_Click"/> </summary>
        /// <remarks> шаг на пол листа. </remarks>
        void toolStripMenuItem4_Click(object sender, EventArgs e);
        /// <summary> <inheritdoc cref="toolStripMenuItem3_Click"/> </summary>
        /// <remarks> шаг на лист по карте. </remarks>
        void toolStripMenuItem5_Click(object sender, EventArgs e);
        /// <summary> Метод двигает карту вдоль осей X/Y при нажатии на кнопки с зелёными стрелочками. </summary>
        void Shift_XY_Map(object sender, EventArgs e);
        /// <summary> Метод центрирует карту с указанными координатами. </summary>
        void btn_Map_OK_Click(object sender, EventArgs e);
        /// <summary> Метод обрабатывает некорректный ввод в поля координат центрирования карты. </summary>
        void tb_Coord_X_TextChanged(object sender, EventArgs e);
        /// <summary> Метод обрабатывает изменение <b>value</b> значения в nud_value_size_map. </summary>
        void nud_value_size_map_ValueChanged(object sender, EventArgs e);
        /// <summary> Метод обрабатывает левый клик по кнопки для вызова метода <b>FULL_SCREEN();</b> </summary>
        void btn_Map_Full_Screen_Click(object sender, EventArgs e);
        /// <summary> Метод меняет размеры и локации интерфейса карты для двух режимов: <b>FULL SCREEN</b> / <b>DEFAULT SIZE</b>. </summary>
        void FULL_SCREEN();

        /// <summary> Метод обновляет всю информацию и интерфейс на вкладке "КАРТА / MAP". </summary>
        void Update_Panels_Map();
    }

    public partial class Form1 : Form, IForm1_Map {
        //========================================== МЕТОДЫ ДЛЯ ВКЛАДКИ "КАРТА / MAP" ==========================================
        /// <summary> Метод обрабатывает клик по кнопке, запускающей альтернативную карту (GetterTools.com) </summary>
        private void btn_Map_GetterTools_Click(object sender, EventArgs e) {
            WinDlg_Map_GetterTools();
        }
        /// <summary> Окно альтернативной карты (GetterTools.com). </summary>
        private Form Form_Map_GetterTools = null;
        /// <summary> Таблица-картинка альтернативной карты (GetterTools.com). </summary>
        private PictureBox pb_Map_GetterTools = null;
        /// <summary> Панелька-картинка с миникартой (GetterTools.com). </summary>
        private PictureBox pb_MiniMap_GetterTools = null;
        /// <summary> Табличка списка альянсов. (GetterTools.com). Столбцы: 1.checkbox; 2.pic; 3.NameAlliance; </summary>
        private DataGridView dgv_GetterTools_Alliances = null;
        /// <summary> Табличка списка аккаунтов - основателей альянсов. (GetterTools.com). Столбцы: 1.checkbox; 2.pic; 3.NameAccount; </summary>
        private DataGridView dgv_GetterTools_Accounts = null;
        /// <summary> Полоска с информацией на альтернативной карте (GetterTools.com). </summary>
        private StatusStrip SS_GetterTools_Info = null;
        /// <summary> Размер ячейки-картинки (GetterTools.com) в текущем разрешении экрана без учёта масштаба. </summary>
        private int GTC_Size = 0;
        /// <summary> Минимальный размер ячейки-картинки (GetterTools.com). </summary>
        private int GTC_MIN = 4;
        /// <summary> Максимальный размер ячейки-картинки (GetterTools.com). </summary>
        private int GTC_MAX = 150;
        /// <summary> Масштаб альтернативной карты (GetterTools.com). </summary>
        private double MGT_Scale = 1;
        /// <summary> Шаг увеличения/уменьшения масштаба альтернативной карты (GetterTools.com). </summary>
        private double MGT_Step = 0.1;
        /// <summary> Текущие координаты отрисовки картинки на <b>pb_Map_GetterTools</b> по осям <b>X/Y.</b> </summary>
        private int _cx = 0, _cy = 0;
        /// <summary> Предыдущие координаты отрисовки картинки на <b>pb_Map_GetterTools</b> по осям <b>Y/Y.</b> </summary>
        private int _lx = 0, _ly = 0;
        /// <summary> Ссылка на кадр <b>pb_Map_GetterTools</b> альтернативной карты (GetterTools.com). </summary>
        private Bitmap MGT_bmp = null;
        /// <summary> Флаг отрисовки. <b>true</b> = отрисовать кадр полностью. <b>false</b> = только сместить картинку по осям <b>_cx/_cy</b> без перерисовки кадра. </summary>
        private bool MGT_Draw_Lock = true;
        /// <summary> Двумерный массив фильтров отрисовки ячеек. <br/> Подробнее о фильтре: <br/> <inheritdoc cref="MGT_DrawCell"/> </summary>
        private MGT_DrawCell[,] MGT_Filter = null;

        /// <summary> Метод срабатывает после закрытия формы альтернативной карты (GetterTools.com). </summary>
        private void Form_Map_GetterTools_FormClosed(object sender, FormClosedEventArgs e) {
            pnl_Map_Info_Village_Population.VisibleChanged -= (s, a) => { };
            //Form_Map_GetterTools.Controls.Remove(pnl_Map_Info_Village_Population);
            //tabPage4.Controls.Add(pnl_Map_Info_Village_Population);
            pnl_Map_Info_Village_Population.Parent = bg_Map;//перецепляем всплывающую подсказку
            pnl_Map_Info_Village_Population.BackColor = Color.FromArgb(255, 165, 145, 125);//фон на панели
            pnl_Map_Info_Village_Population.Visible = false;
            pnl_Map_Info_Village_Population.BringToFront();
        }

        /// <summary> Метод получает цвет ячейки с ботом на альтернативной карте (GetterTools.com) в зависимости от пренадлежности к альянсу или "беспартийности", если бота в ячейке нет, возвращает фоновый цвет (по умолчанию тёмно-зелёный). </summary>
        /// <returns> Возвращает цвет ячейки бота выраженный через его личный ранг и ранг его альянса. </returns>
        private Color GetColorForPaintAltMap(TPlayer Account) {
            HSV hsv = new HSV(360, 0.5, 0.8);
            if (Account != null) { int RANK = Account.LinkOnAlliance != null ? Account.LinkOnAlliance.Rank : -1;
                if (RANK > -1) { hsv.Hue = 360.0 / (RANK + 1); hsv.Saturation = 1.0 * (1.0 / (RANK + 1) + 0.5); hsv.Value = 1.0; }//в альянсе
                else { hsv.Hue = 360.0 / (Account.Rank + 1); hsv.Saturation = 0.6; hsv.Value = 0.6; }//беспартийный
                return hsv.HSVToColor();
            } else return Color.FromArgb(25, 75, 25);/*цвет фоновых ячеек*/
        }

        /// <summary> Перечисление фильтров рисования на альтернативной карте (GetterTools.com). <br/> <b>Standart</b> = рисовать то, что фактически находится в ячейке <b>x/y</b>; <br/> <b>Empty</b> = рисовать пустую, никем не занятую ячейку, даже если в ней есть деревня. <br/> <b>Illumination</b> = нарисовать подсвеченную ячейку (по умолчанию белый круг). </summary>
        private enum MGT_DrawCell { Standart, Empty, Illumination };
        /// <summary> Метод рисует на <b>Graphics</b>, связанным с <b>Bitmap</b> ячейку в заданных координатах в системе координат Map.Cell[x][y]; </summary>
        /// <value>
        ///     <b> <paramref name="g"/>: </b> поверхность рисования связанная с кадром <b>Bitmap</b> MGT_bmp. <br/>
        ///     <b> <paramref name="x"/>/<paramref name="y"/>: </b> координаты ячейки которую требуется перерисовать в системе координат Map.Cell[x][y]; <br/>
        ///     <b> <paramref name="DrawCell"/>: </b> фильтр, режим рисования: Standart, Empty, Illumination. <br/>
        /// </value>
        /// <returns> Возвращает объект <b>Graphics</b> с нарисованной ячейкой. </returns>
        private Graphics MGT_DrawBitmap(Graphics g, int x, int y, MGT_DrawCell DrawCell = MGT_DrawCell.Standart) {
            double szc = GTC_Size * MGT_Scale;//размер ячейки в масштабе
            Color bg = Color.FromArgb(25, 75, 25);/*цвет фоновых ячеек*/ Color cl = bg;//цвет ячейки
            float sh = (float)(szc / 16); sh = sh > 0 ? sh : 1;//толщина тени
            float th = 2;//толщина рамки
            float L1 = (float)(szc * 0.03); L1 = L1 > 0 ? L1 : 1; float L2 = (float)(szc * 0.03); L2 = L2 > 0 ? L2 : 1;
            //рисуем ячейку карты
            var Cell = GAME.Map.Cell[x, y]; var Account = Cell.LinkAccount; Pen pen = null; bool Float = false;
            if (Cell.ID < Cell_ID.Village_Tiny || Cell.ID > Cell_ID.Village_Large ||
                DrawCell == MGT_DrawCell.Empty) { //в ячейке нет деревни
                cl = Color.FromArgb(25, 75, 25); int step = Random.RND(-1, 1);
                cl = Color.FromArgb(cl.R + step, cl.G + step, cl.B + step); Float = true;
            } else if (Account != null) { 
                if (DrawCell != MGT_DrawCell.Illumination) { 
                    //в альянсе
                    if (Account.LinkOnAlliance != null) { pen = new Pen(Color.Black, 1); cl = GetColorForPaintAltMap(Account); }
                    //беспартийный
                    else { pen = new Pen(Color.Black, 1) { DashPattern = new float[] { L1, L2 }, }; cl = GetColorForPaintAltMap(Account); }
                } else { float w = (float)(szc / 100 * 5);/* % */ w = w < 1 ? 1 : w;
                    pen = new Pen(Color.Black, w); cl = Color.White; }
            }
            if (DrawCell != MGT_DrawCell.Illumination) {
                if (Float) g.FillRectangle(new SolidBrush(cl), new RectangleF((float)(szc * x), (float)(szc * y), (float)szc, (float)szc));
                else { g.FillRectangle(new SolidBrush(cl), new Rectangle((int)(szc * x), (int)(szc * y), (int)szc, (int)szc)); 
                       g.DrawRectangle(pen, new Rectangle((int)(szc * x + sh * 0.5 + th * 0.5), (int)(szc * y + sh * 0.5 + th * 0.5), (int)(szc - sh * 2 - th), (int)(szc - sh * 2 - th))); 
                }
                if (pen != null) {
                    //фоновый горизонт
                    g.DrawLine(new Pen(bg, sh), (int)(szc * x), (int)(szc * y + szc - sh * 0.5), (int)(szc * x + szc), (int)(szc * y + szc - sh * 0.5));
                    //фоновая вертикаль
                    g.DrawLine(new Pen(bg, sh), (int)(szc * x + szc - sh * 0.5), (int)(szc * y), (int)(szc * x + szc - sh * 0.5), (int)(szc * y + szc - sh * 0.5));
                    g.DrawLine(new Pen(Color.FromArgb(150, 0, 0, 0), sh), //горизонт тени
                        (int)(szc * x), (int)(szc * y + szc - sh * 0.5), (int)(szc * x + szc), (int)(szc * y + szc - sh * 0.5));
                    g.DrawLine(new Pen(Color.FromArgb(150, 0, 0, 0), sh), //вертикаль тени
                        (int)(szc * x + szc - sh * 0.5), (int)(szc * y), (int)(szc * x + szc - sh * 0.5), (int)(szc * y + szc - sh * 0.5));
                }
            } else { //подсветка ячейки
                g.FillRectangle(new SolidBrush(bg), new RectangleF((float)(szc * x), (float)(szc * y), (float)szc, (float)szc));
                g.FillEllipse(new SolidBrush(cl), new Rectangle((int)(szc * x + pen.Width), (int)(szc * y + pen.Width), (int)(szc - pen.Width * 2), (int)(szc - pen.Width * 2)));
                //g.DrawEllipse(pen, new Rectangle((int)(szc * x + th * 0.5), (int)(szc * y + th * 0.5), (int)(szc - th), (int)(szc - th)));
                g.DrawArc(pen, new Rectangle((int)(szc * x + pen.Width), (int)(szc * y + pen.Width), (int)(szc - pen.Width * 2), (int)(szc - pen.Width * 2)), -45, 180);
            }
            return g;
        }

        /// <summary> Метод обрабатывает перерисовку <b>pb_Map_GetterTools</b> и применяет ко всем ячейкам <b>x/y</b> закраску согласно условиям метода. Альтернативная карта (GetterTools.com). </summary>
        private void pb_Map_GetterTools_Paint(object sender, PaintEventArgs e) {
            if (MGT_bmp != null) { //коррекция координат картинки
                if (MGT_bmp.Width <= pb_Map_GetterTools.Width) { //проверка Left/Right
                    _cx = _cx > 0 ? _cx + MGT_bmp.Width < pb_Map_GetterTools.Width ? _cx : pb_Map_GetterTools.Width - MGT_bmp.Width : 0; }
                else { _cx = _cx < 0 ? _cx + MGT_bmp.Width > pb_Map_GetterTools.Width ? _cx : pb_Map_GetterTools.Width - MGT_bmp.Width : 0; }
                if (MGT_bmp.Height <= pb_Map_GetterTools.Height) { //проверка Top/Bottom
                    _cy = _cy > 0 ? _cy + MGT_bmp.Height < pb_Map_GetterTools.Height ? _cy : pb_Map_GetterTools.Height - MGT_bmp.Height : 0; } 
                else { _cy = _cy < 0 ? _cy + MGT_bmp.Height > pb_Map_GetterTools.Height ? _cy : pb_Map_GetterTools.Height - MGT_bmp.Height : 0; }
            }
            //отрисовка картинки в контейнере PictureBox с заданной позиции
            if (!MGT_Draw_Lock) { e.Graphics.DrawImageUnscaled(MGT_bmp, _cx, _cy); return; }
            double szc = GTC_Size * MGT_Scale;//размер ячейки в масштабе
            MGT_bmp = new Bitmap((int)(GAME.Map.Length_X() * szc), (int)(GAME.Map.Length_Y() * szc));
            Graphics g = Graphics.FromImage(MGT_bmp); g.Clear(Color.Black);
            //рисуем ячейки карты
            for (int y = 0; y < GAME.Map.Length_Y(); y++) for (int x = 0; x < GAME.Map.Length_X(); x++) {
                MGT_DrawBitmap(g, x, y, MGT_Filter[x, y]);
            }
            //рисуем разлиновку
            Color cl_line = Color.FromArgb(50, 150, 50);
            int sz = 10;//10x10 размер чанка в ячейках
            Size chunk = new Size((GAME.Map.Length_X() - 1) / sz, (GAME.Map.Length_Y() - 1) / sz);//кол-во чанков
            Pen PEN = new Pen(cl_line, 1) { DashPattern = new float[] { 1, 1 }, };
            double size_chunk = szc * sz;//размер чанка в пикселях
            for (int y = 0; y < chunk.Height; y++) for (int x = 0; x < chunk.Width; x++) {
                g.DrawRectangle(PEN, (int)(size_chunk * x + 1), (int)(size_chunk * y + 1), 
                    (int)(size_chunk * x + size_chunk - 2), (int)(size_chunk * y + size_chunk - 2));
            }
            //ось +/- (0, 0)
            int X = (int)(GAME.Map.Width * szc); int Y = (int)(GAME.Map.Height * szc);
            int XX = (int)(GAME.Map.Length_X() * szc); int YY = (int)(GAME.Map.Length_Y() * szc);
            g.DrawRectangle(new Pen(Color.FromArgb(75, 100, 50), 1), X + 1, 1, (int)(szc - 2), YY - 2);// |
            g.DrawRectangle(new Pen(Color.FromArgb(75, 100, 50), 1), 1, Y + 1, XX - 2, (int)(szc - 2));// --
            Font font = new Font("Arial", (float)(size_chunk / 20), FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.FromArgb(10, 100, 255, 100));
            SizeF label = g.MeasureString(LANGUAGES.RESOURSES[154]/*СЕВЕРО-ЗАПАД*/, font);
            g.DrawString(LANGUAGES.RESOURSES[154]/*СЕВЕРО-ЗАПАД*/, font, brush, (int)(X - label.Width) / 2, (int)(Y - label.Height) / 2);
            label = g.MeasureString(LANGUAGES.RESOURSES[155]/*СЕВЕРО-ВОСТОК*/, font);
            g.DrawString(LANGUAGES.RESOURSES[155]/*СЕВЕРО-ВОСТОК*/, font, brush, (int)((X - label.Width) / 2 + X + szc), (int)(Y - label.Height) / 2);
            label = g.MeasureString(LANGUAGES.RESOURSES[156]/*ЮГО-ЗАПАД*/, font);
            g.DrawString(LANGUAGES.RESOURSES[156]/*ЮГО-ЗАПАД*/, font, brush, (int)(X - label.Width) / 2, (int)((Y - label.Height) / 2 + Y + szc));
            label = g.MeasureString(LANGUAGES.RESOURSES[157]/*ЮГО-ВОСТОК*/, font);
            g.DrawString(LANGUAGES.RESOURSES[157]/*ЮГО-ВОСТОК*/, font, brush, (int)((X - label.Width) / 2 + X + szc), (int)((Y - label.Height) / 2 + Y + szc));

            e.Graphics.DrawImageUnscaled(MGT_bmp, _cx, _cy);//отрисовка картинки в контейнере PictureBox с заданной позиции
            pb_MiniMap_GetterTools.BackgroundImage = MGT_bmp;

            SS_GetterTools_Info.Items[0].Text = $"{LANGUAGES.RESOURSES[153]/*Масштаб*/}: {(MGT_Scale * 100).ToString("#.##")}%";
            /*ТЕСТ*/SS_GetterTools_Info.Items[0].Text += $"   _cx = {_cx}  |  _cy = {_cy}";
        }
        /// <summary> Метод обрабатывает перерисовку миникарты <b>pb_MiniMap_GetterTools</b> (GetterTools.com). </summary>
        private void pb_MiniMap_GetterTools_Paint(object sender, PaintEventArgs e) {
            if (MGT_bmp == null) return;
            var G = e.Graphics; float th = 1; Pen pen = new Pen(Color.White, th);
            Size size = new Size((int)(pb_MiniMap_GetterTools.Width / ((double)MGT_bmp.Width / pb_Map_GetterTools.Width)),
                                 (int)(pb_MiniMap_GetterTools.Height / ((double)MGT_bmp.Height / pb_Map_GetterTools.Height)));
            int X = -(int)((double)_cx / MGT_bmp.Width * pb_MiniMap_GetterTools.Width);
            int Y = -(int)((double)_cy / MGT_bmp.Height * pb_MiniMap_GetterTools.Height);
            G.FillRectangle(new SolidBrush(Color.FromArgb(100, 255, 255, 255)), new Rectangle(X, Y, size.Width, size.Height));
            G.DrawRectangle(pen, new Rectangle((int)(X + th * 0.5f), (int)(Y + th * 0.5f), (int)(size.Width - th), (int)(size.Height - th)));
        }
        /// <summary> Метод обрабатывает клик по <b>pb_Map_GetterTools</b> ячейкам таблицы альтернативной карты (GetterTools.com). </summary>
        private void pb_Map_GetterTools_MouseClick(object sender, MouseEventArgs e) {
            var pb = sender as PictureBox;
            double szc = GTC_Size * MGT_Scale;//размер ячейки в масштабе
            int x = (int)((pb.Left + e.X - _cx) / szc); int y = (int)((pb.Top + e.Y - _cy) / szc);
            if (x < 0 || x >= GAME.Map.Length_X() || y < 0 || y >= GAME.Map.Length_Y()) return;
            if (GAME.Map.Cell[x, y].LinkAccount != null && MGT_Filter[x, y] != MGT_DrawCell.Empty) {
                //открыть окно аккаунта, который находится в данной ячейке
                if (Form_AccountProfile != null && Form_AccountProfile.Visible) Form_AccountProfile.Close();
                WinDlg_AccountProfile(GAME.Map.Cell[x, y].LinkAccount);
            }
        }
        /// <summary> Метод обрабатывает движение курсора мыши по <b>pb_Map_GetterTools</b> ячейкам таблицы альтернативной карты (GetterTools.com). </summary>
        private void pb_Map_GetterTools_MouseMove(object sender, MouseEventArgs e) {
            var pb = sender as PictureBox;
            double szc = GTC_Size * MGT_Scale;//размер ячейки в масштабе
            int x = (int)((pb.Left + e.X - _cx) / szc); int y = (int)((pb.Top + e.Y - _cy) / szc);
            if (x < 0 || x >= GAME.Map.Length_X() || y < 0 || y >= GAME.Map.Length_Y()) return;
            var Cell = GAME.Map.Cell[x, y]; var Account = Cell.LinkAccount; var Village = Cell.LinkVillage;
            if (Account != null && MGT_Filter[x, y] != MGT_DrawCell.Empty) { 
                Cursor.Current = Cursors.Hand;
                string Alliance_Name = Account.LinkOnAlliance != null ? Account.LinkOnAlliance.AllianceNameAbbreviated : "-";
                lb_Map_Info_Village_Population.Text = $"({Village.Coordinates_World_Travian.X}|{Village.Coordinates_World_Travian.Y}) {Village.Village_Name}\n" +
                    $"{LANGUAGES.RESOURSES[15]/*Игрок:*/} {Account.Nick_Name}\n" +
                    $"{LANGUAGES.RESOURSES[17]/*Альянс:*/} {Alliance_Name}\n\n{LANGUAGES.RESOURSES[152]/*Подробности по щелчку*/}.";
                pnl_Map_Info_Village_Population.Visible = true;
            } else { Cursor.Current = Cursors.Cross; pnl_Map_Info_Village_Population.Visible = false; }
            pnl_Map_Info_Village_Population.Location = new Point(pb.Left + e.X + ToCSR(8), pb.Top + e.Y + ToCSR(18));

            if (e.Button == MouseButtons.Left && pb.Capture) {
                int dx = e.X - _lx; int dy = e.Y - _ly; _cx += dx; _cy += dy; _lx = e.X; _ly = e.Y;
                pb_Map_GetterTools.Invalidate();//отрисовка контрола в порядке живой очереди
            }
        }
        /// <summary> Метод обрабатывает выход курсора мыши за границы <b>pb_Map_GetterTools</b>. </summary>
        private void pb_Map_GetterTools_MouseLeave(object sender, EventArgs e) { pnl_Map_Info_Village_Population.Visible = false; }
        /// <summary> Метод обрабатывает нажатие кнопки мыши на <b>pb_Map_GetterTools</b>. </summary>
        private void pb_Map_GetterTools_MouseDown(object sender, MouseEventArgs e) { _lx = e.X; _ly = e.Y; }
        /// <summary> Метод обрабатывает прокрутку <b>pb_Map_GetterTools</b> таблицы альтернативной карты (GetterTools.com). <br/> Происходит изменение масштаба посредством скроллинга колёсиком мыши. </summary>
        private void pb_Map_GetterTools_MouseWheel(object sender, MouseEventArgs e) {
            Size sz1 = MGT_bmp.Size;//размер ДО применения масштаба
            //изменение масштаба
            if (e.Delta < 0) { double scale = MGT_Scale - MGT_Step; MGT_Scale = GTC_Size * scale > GTC_MIN ? scale : MGT_Scale; }//-
            else { MGT_Scale = GTC_Size * MGT_Scale < GTC_MAX ? MGT_Scale + MGT_Step : MGT_Scale; }//+
            pb_Map_GetterTools.Refresh();//немедленная перерисовка контрола
            Size sz2 = MGT_bmp.Size;//размер ПОСЛЕ применения масштаба
            Size delta = sz1 - sz2; _cx += delta.Width / 2; _cy += delta.Height / 2;
            MGT_Draw_Lock = false; pb_Map_GetterTools.Refresh(); MGT_Draw_Lock = true;
        }

        /// <summary> Перечисление режима работы метода <b>MGT_UpdateFilter(...);</b> <br/> <b>Alliance</b> = обработать альянс аккаунта; <br/> <b>Account</b> = обработать сам аккаунт; </summary>
        private enum MGT_UpdateFilterFor { Alliance, Account };
        /// <summary> Метод обновляет фильр ячеек на альтернативной карте (GetterTools.com). </summary>
        /// <value>
        ///     <b> <paramref name="Alliance"/>: </b> альянс, для которого требуется обновить фильтр. <br/>
        ///     <b> <paramref name="DrawCell"/>: </b> фильтр, режим рисования: Standart, Empty, Illumination. <br/>
        ///     <b> <paramref name="UpdateFilterFor"/>: </b> режим работы этого метода. <br/>
        /// </value>
        private void MGT_UpdateFilter(TPlayer Account, MGT_DrawCell DrawCell, MGT_UpdateFilterFor UpdateFilterFor) {
            if (UpdateFilterFor == MGT_UpdateFilterFor.Alliance) {
                var Alliance = Account.LinkOnAlliance;
                for (int i = 0; i < Alliance.ListAlly.Count; i++) { Point p = Alliance.ListAlly[i].CoordinatesCell;
                    for (int j = 0; j < GAME.Map.Cell[p.X, p.Y].LinkAccount.VillageList.Count; j++) {
                        Point member = GAME.Map.Cell[p.X, p.Y].LinkAccount.VillageList[j].Coordinates_Cell;
                        MGT_Filter[member.X, member.Y] = DrawCell;
                    }
                }
            } else { //MGT_UpdateFilterFor.Account
                for (int i = 0; i < Account.VillageList.Count; i++) MGT_Filter[Account.VillageList[i].Coordinates_Cell.X, Account.VillageList[i].Coordinates_Cell.Y] = DrawCell;
            }
        }

        /// <summary> Метод обрабатывает наведение курсора на ячейку таблиц альтернативной карты (GetterTools.com). </summary>
        private void dgv_GetterTools_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex <= -1) return; var grid = sender as DataGridView; var Account = grid[0, e.RowIndex].Tag as TPlayer;
            if (e.ColumnIndex == 1 || e.ColumnIndex == 2) {//наведение на пиктограмму "око" или на название - [1, 2] столбцы
                if (grid.Name == "dgv_GetterTools_Alliances") 
                    MGT_UpdateFilter(Account, MGT_DrawCell.Illumination, MGT_UpdateFilterFor.Alliance);
                else MGT_UpdateFilter(Account, MGT_DrawCell.Illumination, MGT_UpdateFilterFor.Account);//dgv_GetterTools_Accounts
                pb_Map_GetterTools.Refresh(); pb_Map_GetterTools.Focus();
            }
        }
        /// <summary> Метод обрабатывает покидание курсором границ ячейки в таблице альтернативной карты (GetterTools.com). </summary>
        private void dgv_GetterTools_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            var grid = sender as DataGridView; var Account = grid[0, e.RowIndex].Tag as TPlayer;
            //покидание пиктограммы "око" и названия - [1, 2] столбцы
            if (grid.Name == "dgv_GetterTools_Alliances") 
                MGT_UpdateFilter(Account, MGT_DrawCell.Standart, MGT_UpdateFilterFor.Alliance);
            else MGT_UpdateFilter(Account, MGT_DrawCell.Standart, MGT_UpdateFilterFor.Account);//dgv_GetterTools_Accounts
            pb_Map_GetterTools.Refresh(); pb_Map_GetterTools.Focus();
        }

        /// <summary> Метод обрабатывает клик по содержимому ячеек таблиц альтернативной карты (GetterTools.com). </summary>
        private void dgv_GetterTools_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex <= -1) return; var grid = sender as DataGridView;
            if (e.ColumnIndex == 0) grid[0, e.RowIndex].Value = !(bool)(grid[0, e.RowIndex].Value);//check
            var Account = grid[0, e.RowIndex].Tag as TPlayer;
            if (grid.Name == "dgv_GetterTools_Alliances" || grid.Name == "dgv_GetterTools_Accounts") {
                if (e.ColumnIndex == 0) { //клик по чекбоксу - [0] столбец
                    MGT_DrawCell DrawFilterCell; MGT_UpdateFilterFor UpdateFilterFor;
                    if ((bool)grid[0, e.RowIndex].Value) DrawFilterCell = MGT_DrawCell.Standart; else DrawFilterCell = MGT_DrawCell.Empty;
                    if (grid.Name == "dgv_GetterTools_Alliances") UpdateFilterFor = MGT_UpdateFilterFor.Alliance;
                    else UpdateFilterFor = MGT_UpdateFilterFor.Account;//dgv_GetterTools_Accounts
                    MGT_UpdateFilter(Account, DrawFilterCell, UpdateFilterFor);
                } else if (e.ColumnIndex == 1) { //клик по пиктограмме "око/eye" - [1] столбец
                    var Alliance = Account.LinkOnAlliance;
                    int X1 = int.MaxValue, Y1 = int.MaxValue, X2 = int.MinValue, Y2 = int.MinValue; 
                    if (grid.Name == "dgv_GetterTools_Alliances") {
                        for (int i = 0; i < Alliance.ListAlly.Count; i++) { Point p = Alliance.ListAlly[i].CoordinatesCell;
                            for (int j = 0; j < GAME.Map.Cell[p.X, p.Y].LinkAccount.VillageList.Count; j++) {
                                Point m = GAME.Map.Cell[p.X, p.Y].LinkAccount.VillageList[j].Coordinates_Cell;
                                if (m.X < X1) X1 = m.X; if (m.Y < Y1) Y1 = m.Y; if (m.X > X2) X2 = m.X; if (m.Y > Y2) Y2 = m.Y;
                            }
                        }
                    } else { //dgv_GetterTools_Accounts
                        for (int i = 0; i < Account.VillageList.Count; i++) {
                            Point a = Account.VillageList[i].Coordinates_Cell;
                            if (a.X < X1) X1 = a.X; if (a.Y < Y1) Y1 = a.Y; if (a.X > X2) X2 = a.X; if (a.Y > Y2) Y2 = a.Y;
                        }
                    }
                    int oldCX = _cx; int oldCY = _cy; double oldScale = MGT_Scale;//запоминаем старые переменные
                    PointF AbsoluteCenter = new PointF((X1 + X2) / 2, (Y1 + Y2) / 2);//абсолютный центр
                    //PointF RelativeCenter = new PointF((X2 - X1) / 2, (Y2 - Y1) / 2);//относительный центр
                    Point CellCount = new Point(X2 - X1, Y2 - Y1);//дистанция между крайними точками альянса (кол-во ячеек)
                    double scaleX = (double)pb_Map_GetterTools.Width / (CellCount.X + 6) / GTC_Size;
                    double scaleY = (double)pb_Map_GetterTools.Height / (CellCount.Y + 6) / GTC_Size;
                    MGT_Scale = Math.Round(Math.Min(scaleX, scaleY), 1);
                    if (MGT_Scale * GTC_Size > GTC_MAX) MGT_Scale = Math.Round((double)GTC_MAX / GTC_Size, 1);
                    if (MGT_Scale * GTC_Size < GTC_MIN) MGT_Scale = 1;
                    double szc = GTC_Size * MGT_Scale;//размер ячейки в масштабе
                    //_cx = _lx = -(int)(AbsoluteCenter.X * szc) + (int)((pb_Map_GetterTools.Width - szc) / 2.0);
                    //_cy = _ly = -(int)(AbsoluteCenter.Y * szc) + (int)((pb_Map_GetterTools.Height - szc) / 2.0);
                    int w = (int)(szc * GAME.Map.Length_X()); int h = (int)(szc * GAME.Map.Length_Y());
                    //запоминаем новые переменные
                    double newScale = MGT_Scale;
                    int newCX = -(int)(AbsoluteCenter.X * szc) + (int)((pb_Map_GetterTools.Width - szc) / 2.0);
                    int newCY = -(int)(AbsoluteCenter.Y * szc) + (int)((pb_Map_GetterTools.Height - szc) / 2.0);
                    newCX = w < pb_Map_GetterTools.Width ? (int)((pb_Map_GetterTools.Width - w) / 2.0) : newCX;
                    newCY = h < pb_Map_GetterTools.Height ? (int)((pb_Map_GetterTools.Height - h) / 2.0) : newCY;
                    pb_Map_GetterTools.Enabled = pb_MiniMap_GetterTools.Enabled = dgv_GetterTools_Alliances.Enabled = false;
                    AnimateEye(30, oldCX, oldCY, newCX, newCY, oldScale, newScale);
                    pb_Map_GetterTools.Enabled = pb_MiniMap_GetterTools.Enabled = dgv_GetterTools_Alliances.Enabled = true;
                } else if (e.ColumnIndex == 2) { //клик по названию альянса - [2] столбец
                    if (grid.Name == "dgv_GetterTools_Alliances") {
                        //тут будет код открытия окна альянса

                    } else { //dgv_GetterTools_Accounts
                        //открыть окно аккаунта, который находится в данной ячейке
                        if (Form_AccountProfile != null && Form_AccountProfile.Visible) Form_AccountProfile.Close();
                        WinDlg_AccountProfile(Account);
                    }
                }
                pb_Map_GetterTools.Refresh(); pb_Map_GetterTools.Focus();
            } 
        }
        /// <summary> Метод анимирует центрирование выбранной группы игроков. </summary>
        private async void AnimateEye(int CountSteps, int oldCX, int oldCY, int newCX, int newCY, double oldScale, double newScale) {
            double inter = 0; 
            for (int i = 0; i < CountSteps; i++) { await Task.Delay(1);
                _cx = _lx = (int)Interpolate(newCX, oldCX, inter);
                _cy = _lx = (int)Interpolate(newCY, oldCY, inter);
                MGT_Scale = Interpolate(newScale, oldScale, inter);
                pb_Map_GetterTools.Refresh();
                inter = i * CountSteps / ((CountSteps * (CountSteps / 2.0) - i * (CountSteps / 2.0)) + CountSteps) / (i / 2 + 1);
                inter = inter / (inter + 1) / CountSteps * (i * 2); inter = inter <= 0 ? 0.1 : inter;
                Form_Map_GetterTools.Text = "inter = " + inter;//ТЕСТ
            }
            _cx = _lx = newCX; _cy = _lx = newCY; MGT_Scale = newScale; pb_Map_GetterTools.Refresh();
            Form_Map_GetterTools.Text = "inter = " + inter + " | _cx=" + _cx + "  _cy=" + _cy +//ТЕСТ
                " | oldCX=" + oldCX + "  oldCY=" + oldCY + " | newCX=" + newCX + "  newCY=" + newCY;//ТЕСТ
        }

        /// <summary> Метод запускает окно альтернативной карты (GetterTools.com). </summary>
        private void WinDlg_Map_GetterTools() {
            float FSize = ToCSR(10);/*размер шрифта текста*/ string FName = "Arial";/*имя шрифта текста*/
            GTC_Size = //(pb_Map_GetterTools.Height / GAME.Map.Length_Y());
                (int)(ScreenBounds_Size().Width * 0.85 / GAME.Map.Length_X());
            double szc = GTC_Size * MGT_Scale;//размер ячейки в масштабе
            Size Size_bmp = new Size((int)(GAME.Map.Length_X() * szc), (int)(GAME.Map.Length_Y() * szc));
            if (Form_Map_GetterTools == null) {
                MGT_Filter = new MGT_DrawCell[GAME.Map.Length_X(), GAME.Map.Length_Y()];
                //ФОРМА
                Form_Map_GetterTools = Extensions.CreateForm("Form_Map_GetterTools", new Icon("DATA_BASE/IMG/logotip.ico"),
                    new Font(FName, FSize, FontStyle.Regular), Text = LANGUAGES.tool_tip_TEXT[34]/*Карта альянсов (GetterTools.com)*/,
                    FormBorderStyle: FormBorderStyle.Fixed3D, WindowState: FormWindowState.Maximized, 
                    ControlBox: true, KeyPreview: true, AutoScroll: false);
                Form_Map_GetterTools.HorizontalScroll.Visible = false;
                Form_Map_GetterTools.KeyUp += new KeyEventHandler(KeyUp_EscapeFormClose);
                Form_Map_GetterTools.FormClosed += Form_Map_GetterTools_FormClosed;

                //АЛЬТЕРНАТИВНАЯ КАРТА
                pb_Map_GetterTools = new PictureBox { Parent = Form_Map_GetterTools, //BackgroundImageLayout = ImageLayout.Stretch,
                    Location = new Point(ToCSR(0), ToCSR(0)), BackColor = Color.FromArgb(230, 230, 230) };
                pb_Map_GetterTools.Paint += pb_Map_GetterTools_Paint;
                pb_Map_GetterTools.MouseClick += pb_Map_GetterTools_MouseClick;
                pb_Map_GetterTools.MouseMove += pb_Map_GetterTools_MouseMove;
                pb_Map_GetterTools.MouseLeave += pb_Map_GetterTools_MouseLeave;
                pb_Map_GetterTools.MouseDown += pb_Map_GetterTools_MouseDown;
                pb_Map_GetterTools.MouseWheel += pb_Map_GetterTools_MouseWheel;
                pb_Map_GetterTools.BringToFront();

                //ПАНЕЛЬКА С МИНИ-КАРТОЙ
                pb_MiniMap_GetterTools = new PictureBox { Parent = Form_Map_GetterTools, BorderStyle = BorderStyle.FixedSingle, 
                    BackgroundImageLayout = ImageLayout.Stretch };
                tool_tip[35] = Extensions.CreateHint(pb_MiniMap_GetterTools, 0, 30000, LANGUAGES.tool_tip_TITLE[35], LANGUAGES.tool_tip_TEXT[35], Color.GreenYellow, Color.DarkGreen, ToolTipIcon.Info, false);
                pb_MiniMap_GetterTools.Paint += pb_MiniMap_GetterTools_Paint;

                //Табличка списка альянсов и аккаунтов
                dgv_GetterTools_Alliances = Extensions.CreateGrid(Form_Map_GetterTools, "dgv_GetterTools_Alliances", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Color.FromArgb(35, 105, 40), GridColor: Color.FromArgb(35, 105, 35), 
                    BorderStyle: BorderStyle.FixedSingle, ColumnHeadersVisible: true, ReadOnly: true, Enabled: true);
                dgv_GetterTools_Accounts = Extensions.CreateGrid(Form_Map_GetterTools, "dgv_GetterTools_Accounts", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Color.FromArgb(35, 105, 40), GridColor: Color.FromArgb(35, 105, 35),
                    BorderStyle: BorderStyle.FixedSingle, ColumnHeadersVisible: true, ReadOnly: true, Enabled: true);
                var __1 = dgv_GetterTools_Alliances; var __2 = dgv_GetterTools_Accounts;
                __1.Columns.AddRange(new DataGridViewColumn[] {
                        new DataGridViewCheckBoxColumn { ReadOnly = false, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewImageColumn { ReadOnly = false, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                });
                __2.Columns.AddRange(new DataGridViewColumn[] {
                        new DataGridViewCheckBoxColumn { ReadOnly = false, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewImageColumn { ReadOnly = false, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                });
                __1.DefaultCellStyle.BackColor = __2.DefaultCellStyle.BackColor = Color.FromArgb(25, 75, 25);//цвет фона каждой ячейки
                __1.DefaultCellStyle.SelectionBackColor = __2.DefaultCellStyle.SelectionBackColor = __1.DefaultCellStyle.BackColor;
                __1.DefaultCellStyle.SelectionForeColor = __2.DefaultCellStyle.SelectionForeColor = __1.DefaultCellStyle.ForeColor;
                for (int i = 0; i < __1.Columns.Count; i++) {
                    __1.Columns[i].HeaderCell.Style.Font = __2.Columns[i].HeaderCell.Style.Font = new Font(FName, FSize + 2, FontStyle.Bold);
                    __1.Columns[i].DefaultCellStyle.Font = __2.Columns[i].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Regular);
                    //__1.Columns[i].AutoSizeMode = __2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    __1.Columns[i].DefaultCellStyle.Alignment = __2.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                __1.Columns[2].AutoSizeMode = __2.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                __1.Columns[2].DefaultCellStyle.Alignment = __2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                __1.AutoSizeColumnsMode = __2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                __1.AutoSizeRowsMode = __2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                __1.ColumnHeadersHeightSizeMode = __2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                //__1.DefaultCellStyle.Padding = __2.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
                __1.Cursor = __2.Cursor = Cursors.Hand;
                __1.CellContentClick += dgv_GetterTools_CellContentClick;
                __1.CellMouseEnter += dgv_GetterTools_CellMouseEnter;
                __1.CellMouseLeave += dgv_GetterTools_CellMouseLeave;
                __2.CellContentClick += dgv_GetterTools_CellContentClick;
                __2.CellMouseEnter += dgv_GetterTools_CellMouseEnter;
                __2.CellMouseLeave += dgv_GetterTools_CellMouseLeave;

                //StatusStrip. Полоска с инфой.
                SS_GetterTools_Info = new StatusStrip { Parent = Form_Map_GetterTools, ForeColor = Color.Black, 
                    Font = new Font(FName, FSize, FontStyle.Regular), Dock = DockStyle.Bottom,
                }; SS_GetterTools_Info.Items.Add("");
            }
            for (int y = 0; y < MGT_Filter.GetLength(0); y++) for (int x = 0; x < MGT_Filter.GetLength(1); x++) MGT_Filter[x, y] = MGT_DrawCell.Standart;
            if (!Form_Map_GetterTools.Visible) Form_Map_GetterTools.Visible = true;
            pb_Map_GetterTools.Size = new Size((int)(ScreenBounds_Size().Width * 0.85),
                                                 Form_Map_GetterTools.ClientSize.Height - SS_GetterTools_Info.Height - 5);

            _cx = (pb_Map_GetterTools.Width - Size_bmp.Width) / 2; _lx = _cx;
            _cy = (pb_Map_GetterTools.Height - Size_bmp.Height) / 2; _ly = _cy;

            pb_Map_GetterTools.Focus();
            Form_Map_GetterTools.Visible = false;

            pb_MiniMap_GetterTools.Size = new Size(Form_Map_GetterTools.ClientSize.Width - pb_Map_GetterTools.Right - 10,
                                                    Form_Map_GetterTools.ClientSize.Width - pb_Map_GetterTools.Right - 10);
            pb_MiniMap_GetterTools.Location = new Point(pb_Map_GetterTools.Right + 5, 0);

            //табличка альянсов
            var _1 = dgv_GetterTools_Alliances; _1.Rows.Clear(); var _2 = dgv_GetterTools_Accounts; _2.Rows.Clear();
            _1.Columns[2].HeaderText = LANGUAGES.Statistics[2];/*Альянсы*/
            _2.Columns[2].HeaderText = LANGUAGES.Statistics[1];/*Игроки*/
            int sz = (int)_1.Columns[0].HeaderCell.Style.Font.Size;
            for (int i = 0; i < GAME.Alliances.LIST.Count; i++) {
                _1.Rows.Add(true, GetICO("DATA_BASE/IMG/pictograms/ico/", "eye.png", sz, sz), GAME.Alliances.LIST[i].AllianceNameAbbreviated);
                _1[2, i].ToolTipText = GAME.Alliances.LIST[i].AllianceNameAbbreviated + "\n" + GAME.Alliances.LIST[i].AllianceNameFull;
                _1[2, i].Style.ForeColor = GetColorForPaintAltMap(GAME.Map.Cell[GAME.Alliances.LIST[i].Owner.X, GAME.Alliances.LIST[i].Owner.Y].LinkAccount);
                _1[0, i].Tag = GAME.Map.Cell[GAME.Alliances.LIST[i].Owner.X, GAME.Alliances.LIST[i].Owner.Y].LinkAccount;//вешаем на чек бокс ссылку на владельца альянса
            }
            for (int i = 0; i < BotList.Count; i++) {
                _2.Rows.Add(true, GetICO("DATA_BASE/IMG/pictograms/ico/", "eye.png", sz, sz), BotList[i].Nick_Name);
                string AllianceName = BotList[i].LinkOnAlliance == null ? "-" : BotList[i].LinkOnAlliance.AllianceNameAbbreviated;
                _2[2, i].ToolTipText = $"[{AllianceName}] {BotList[i].Nick_Name}";
                _2[2, i].Style.ForeColor = GetColorForPaintAltMap(BotList[i]);
                _2[0, i].Tag = BotList[i];//вешаем на чек бокс ссылку на аккаунт
            }

            _1.Size = new Size(pb_MiniMap_GetterTools.Width, (pb_Map_GetterTools.Height - pb_MiniMap_GetterTools.Height) / 2 - 5);
            _1.Location = new Point(pb_MiniMap_GetterTools.Left, pb_MiniMap_GetterTools.Bottom + 5);
            _2.Size = _1.Size; _2.Location = new Point(_1.Left, _1.Bottom + 5);

            //Parent панельки не клеится на форму Form_Map_GetterTools, панель на ней не отображается
            pnl_Map_Info_Village_Population.Parent = pb_Map_GetterTools;//Form_Map_GetterTools;//перецепляем всплывающую подсказку
            pnl_Map_Info_Village_Population.BackColor = Color.FromArgb(100, 150, 0);//фон на панели
            pnl_Map_Info_Village_Population.BringToFront();
            pnl_Map_Info_Village_Population.VisibleChanged += (s, e) => { pb_Map_GetterTools.Refresh(); };

            if (!Form_Map_GetterTools.Visible) Form_Map_GetterTools.ShowDialog();
        }

        public void pb_MAP_MouseMove(object sender, MouseEventArgs e) {
            //КОД МОЕЙ ВСПЛЫВАЮЩЕЙ ПОДСКАЗКИ ДЛЯ КАРТЫ + ПАНЕЛЬ С ИНФОРМАЦИЕЙ:
            //наведение на ячейку карты и вывод информации о ячейке в pnl_Map_DATA
            Point C = pb_MAP.CursorToCell(GAME.Map, new Point(e.X, e.Y), (int)nud_value_size_map.Value, FULL_SCREEN_MAP);
            string[] t = new string[6];
            if (C != new Point(-1, -1)) { //курсор на видимой части карты
                txt_Map_DATA.Text = LANGUAGES.RESOURSES[7]/*Данные*/ + " (X:" + (C.X - GAME.Map.Width) + " Y:" + (C.Y - GAME.Map.Height) + ")";
                txt_Map_DATA.Left = (pnl_Map_DATA.Width - txt_Map_DATA.Width) / 2;
                string Village; string Population; string Folk;
                //ОАЗИС
                if (GAME.Map.Cell[C.X, C.Y].TypeResoueces == TypeCell._0_0_0_0 &&
                    GAME.Map.Cell[C.X, C.Y].ID != Cell_ID.Water && GAME.Map.Cell[C.X, C.Y].ID != Cell_ID.Mountains &&
                    GAME.Map.Cell[C.X, C.Y].ID != Cell_ID.Forest) { 
                    t[0] = LANGUAGES.RESOURSES[8];/*Ячейка:*/ t[1] = LANGUAGES.RESOURSES[9];/*Оазис*/ t[4] = "-"; t[5] = "-";
                    switch (GAME.Map.Cell[C.X, C.Y].ID) {
                        case Cell_ID.Oasis_wood25: t[2] = LANGUAGES.RESOURSES[10];/*Древесина:*/ t[3] = GAME.Bonuses_Oasis[0, 0] + "%"; break;
                        case Cell_ID.Oasis_wood50: t[2] = LANGUAGES.RESOURSES[10];/*Древесина:*/ t[3] = GAME.Bonuses_Oasis[0, 1] + "%"; break;
                        case Cell_ID.Oasis_wood25_crop25: 
                                t[2] = LANGUAGES.RESOURSES[10];/*Древесина:*/ t[3] = GAME.Bonuses_Oasis[0, 2] + "%";
                                t[4] = LANGUAGES.RESOURSES[11];/*Зерно:*/ t[5] = GAME.Bonuses_Oasis[1, 2] + "%"; break;
                        case Cell_ID.Oasis_clay25: t[2] = LANGUAGES.RESOURSES[12];/*Глина:*/ t[3] = GAME.Bonuses_Oasis[0, 3] + "%"; break;
                        case Cell_ID.Oasis_clay50: t[2] = LANGUAGES.RESOURSES[12];/*Глина:*/ t[3] = GAME.Bonuses_Oasis[0, 4] + "%"; break;
                        case Cell_ID.Oasis_clay25_crop25: 
                                t[2] = LANGUAGES.RESOURSES[12];/*Глина:*/ t[3] = GAME.Bonuses_Oasis[0, 5] + "%";
                                t[4] = LANGUAGES.RESOURSES[11];/*Зерно:*/ t[5] = GAME.Bonuses_Oasis[1, 5] + "%"; break;
                        case Cell_ID.Oasis_iron25: t[2] = LANGUAGES.RESOURSES[13];/*Железо:*/ t[3] = GAME.Bonuses_Oasis[0, 6] + "%"; break;
                        case Cell_ID.Oasis_iron50: t[2] = LANGUAGES.RESOURSES[13];/*Железо:*/ t[3] = GAME.Bonuses_Oasis[0, 7] + "%"; break;
                        case Cell_ID.Oasis_iron25_crop25: t[2] = LANGUAGES.RESOURSES[13];/*Железо:*/ t[3] = GAME.Bonuses_Oasis[0, 8] + "%";
                                t[4] = LANGUAGES.RESOURSES[11];/*Зерно:*/ t[5] = GAME.Bonuses_Oasis[1, 8] + "%"; break;
                        case Cell_ID.Oasis_crop25: t[2] = LANGUAGES.RESOURSES[11];/*Зерно:*/ t[3] = GAME.Bonuses_Oasis[0, 9] + "%"; break;
                        case Cell_ID.Oasis_crop50: t[2] = LANGUAGES.RESOURSES[11];/*Зерно:*/ t[3] = GAME.Bonuses_Oasis[0, 10] + "%"; break;
                        default: for (int i = 0; i < t.Length; i++) t[i] = "null"; break;
                    }
                }//свободная ячейка
                else if (GAME.Map.Cell[C.X, C.Y].LinkAccount == null) { 
                    pnl_Map_Info_Village_Population.Visible = false;
                    t[0] = LANGUAGES.RESOURSES[8];/*Ячейка:*/
                    if (GAME.Map.Cell[C.X, C.Y].ID == Cell_ID.Water) t[1] = LANGUAGES.RESOURSES[131];/*Озеро*/
                    else if (GAME.Map.Cell[C.X, C.Y].ID == Cell_ID.Mountains) t[1] = LANGUAGES.RESOURSES[133];/*Горы*/
                    else if (GAME.Map.Cell[C.X, C.Y].ID == Cell_ID.Forest) t[1] = LANGUAGES.RESOURSES[134];/*Лес*/
                    else t[1] = LANGUAGES.RESOURSES[14];/*Покинутая долина*/
                    t[2] = t[3] = t[4] = t[5] = "-";
                } else { //ячейка принадлежит игроку Player или ИИ / AI / ботам
                    Village = GAME.Map.Cell[C.X, C.Y].LinkVillage.Village_Name;
                    Population = toTABString(GAME.Map.Cell[C.X, C.Y].LinkVillage.Population.ToString(), "'");
                    Folk = LANGUAGES.RESOURSES[21 + (int)GAME.Map.Cell[C.X, C.Y].LinkAccount.Folk_Name];

                    t[0] = LANGUAGES.RESOURSES[15];/*Игрок:*/
                    t[1] = GAME.Map.Cell[C.X, C.Y].LinkAccount.Nick_Name;
                    t[2] = LANGUAGES.RESOURSES[16];/*Население:*/
                    t[3] = toTABString(GAME.Map.Cell[C.X, C.Y].LinkAccount.Total_Population.ToString(), "'");
                    t[4] = LANGUAGES.RESOURSES[17];/*Альянс:*/
                    if (GAME.Map.Cell[C.X, C.Y].LinkAccount.Alliance_Name != "") t[5] = GAME.Map.Cell[C.X, C.Y].LinkAccount.Alliance_Name; else t[5] = "-";
                    lb_Map_Info_Village_Population.Text =
                            LANGUAGES.RESOURSES[18]/*Деревня*/ + ": " + Village + "\n" +
                            LANGUAGES.RESOURSES[19]/*Население*/ + ": " + Population + "\n" +
                            LANGUAGES.RESOURSES[20]/*Народ:*/ + " " + Folk;
                    pnl_Map_Info_Village_Population.Location = new Point(pb_MAP.Left + e.X + ToCSR(8), pb_MAP.Top + e.Y + ToCSR(18));
                    pnl_Map_Info_Village_Population.Visible = true;
                }
            } else { //курсор за пределами видимой части карты
                txt_Map_DATA.Text = LANGUAGES.RESOURSES[7];//Данные
                txt_Map_DATA.Left = (pnl_Map_DATA.Width - txt_Map_DATA.Width) / 2;
                t[0] = t[1] = t[2] = t[3] = t[4] = t[5] = "-";
                pnl_Map_Info_Village_Population.Visible = false;
            }
            Left1_Map_DATA.Text = t[0];  Right1_Map_DATA.Text = t[1]; Left2_Map_DATA.Text = t[2];
            Right2_Map_DATA.Text = t[3]; Left3_Map_DATA.Text = t[4];  Right3_Map_DATA.Text = t[5];

            //подгон габаритов под содержимое (атрибуты Font у всех текстбоксов должны быть одинаковыми!)
            var L1 = Left1_Map_DATA; var L2 = Left2_Map_DATA; var L3 = Left3_Map_DATA;
            var R1 = Right1_Map_DATA; var R2 = Right2_Map_DATA; var R3 = Right3_Map_DATA;
            string txt; Font f = L1.Font;
            if (L1.Text.Length > L2.Text.Length) txt = L1.Text; else txt = L2.Text;
            if (txt.Length < L3.Text.Length) txt = L3.Text;
            int w = TextRenderer.MeasureText(txt, f).Width + ToCSR(5); L1.Width = L2.Width = L3.Width = w;
            if (R1.Text.Length > R2.Text.Length) txt = R1.Text; else txt = R2.Text;
            if (txt.Length < R3.Text.Length) txt = R3.Text;
            w = TextRenderer.MeasureText(txt, f).Width + ToCSR(5); R1.Width = R2.Width = R3.Width = w;
            R1.Left = R2.Left = R3.Left = L1.Right;
        }

        public void pb_MAP_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle) return;
            Point C = pb_MAP.CursorToCell(GAME.Map, new Point(e.X, e.Y), (int)nud_value_size_map.Value, FULL_SCREEN_MAP);
            if (C != new Point(-1, -1)) winDlg_InfoCell(C.X, C.Y);
        }
        public void pb_MAP_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle) return;
            Point C = pb_MAP.CursorToCell(GAME.Map, new Point(e.X, e.Y), (int)nud_value_size_map.Value, FULL_SCREEN_MAP);
            if (C != new Point(-1, -1)) { GAME.Map.Location = new Point(C.X - GAME.Map.Width, C.Y - GAME.Map.Height); Update_Panels_Map(); }
        }

        /// <summary> Метод обрабатывает нажатие клавишь в <b> tabControl </b> <br/> Реализованы клавиши: W, A, S, D. Они двигают карту на заданный интервал. </summary>
        private void tabControl_KeyDown(object sender, KeyEventArgs e) {
            if (tabControl.SelectedIndex == 3) switch (e.KeyCode) {
                case Keys.A: Shift_XY_Map(btn_Map_Up_Y, new EventArgs()); break;//влево
                case Keys.D: Shift_XY_Map(btn_Map_Down_Y, new EventArgs()); break;//вправо
                case Keys.W: Shift_XY_Map(btn_Map_Right_X, new EventArgs()); break;//вверх
                case Keys.S: Shift_XY_Map(btn_Map_Left_X, new EventArgs()); break;//вниз
            }
        }

        public void toolStripMenuItem3_Click(object sender, EventArgs e) {
            toolStripMenuItem3.Checked = true; toolStripMenuItem4.Checked = false; toolStripMenuItem5.Checked = false; }
        public void toolStripMenuItem4_Click(object sender, EventArgs e) {
            toolStripMenuItem3.Checked = false; toolStripMenuItem4.Checked = true; toolStripMenuItem5.Checked = false; }
        public void toolStripMenuItem5_Click(object sender, EventArgs e) {
            toolStripMenuItem3.Checked = false; toolStripMenuItem4.Checked = false; toolStripMenuItem5.Checked = true; }

        public void Shift_XY_Map(object sender, EventArgs e) {
            var btn = sender as Button; int step = 1;//шаг по карте на +1 или на размер карты (на лист)
            if (toolStripMenuItem3.Checked) step = 1; 
            else if (toolStripMenuItem4.Checked) step = (int)nud_value_size_map.Value / 2;
            else if (toolStripMenuItem5.Checked) step = (int)nud_value_size_map.Value;
            switch (btn.Name) {
                case "btn_Map_Left_X": GAME.Map.Location = new Point(GAME.Map.Location.X - step, GAME.Map.Location.Y); break;
                case "btn_Map_Right_X": GAME.Map.Location = new Point(GAME.Map.Location.X + step, GAME.Map.Location.Y); break;
                case "btn_Map_Up_Y": GAME.Map.Location = new Point(GAME.Map.Location.X, GAME.Map.Location.Y + step); break;
                case "btn_Map_Down_Y": GAME.Map.Location = new Point(GAME.Map.Location.X, GAME.Map.Location.Y - step); break;
            } Update_Panels_Map();//вызываем построение карты с новыми вбитыми координатами
        }

        public void btn_Map_OK_Click(object sender, EventArgs e) {
            if (tb_Coord_X.Text == "" || tb_Coord_Y.Text == "") return;
            if (tb_Coord_X.Text.Length == 1 && tb_Coord_X.Text == "-") return;
            if (tb_Coord_Y.Text.Length == 1 && tb_Coord_Y.Text == "-") return;
            GAME.Map.Location = new Point(System.Convert.ToInt32(tb_Coord_X.Text), System.Convert.ToInt32(tb_Coord_Y.Text));
            Update_Panels_Map();//вызываем построение карты с новыми вбитыми координатами
        }

        public void tb_Coord_X_TextChanged(object sender, EventArgs e) {
            var tb = sender as TextBox; if (tb.Text == "") return; string New = ""; bool flag = false;
            int start; if (tb.Text[0] == '-') { start = 1; New = "-"; } else start = 0;
            for (int i = start; i < tb.Text.Length; i++) { char ch = tb.Text[i];
                if (ch=='0' || ch=='1' || ch=='2' || ch=='3' || ch=='4' || ch=='5' || ch=='6' || ch=='7' || ch=='8' || ch=='9')
                    { New += ch; } else flag = true;//Обнаружен некорректный ввод символа.
            } if (flag) { SystemSounds.Beep.Play(); tb.Text = New; tb.SelectionStart = tb.Text.Length; }//Поставить каретку в конец строки и пропищать
        }

        /// <summary> Массив текстовых меток с координатами по периметру картинки аля морсокй бой. </summary>
        Label[] lb_Map_Coord_X = null;
        /// <summary> Массив текстовых меток с координатами по периметру картинки аля морсокй бой. </summary>
        Label[] lb_Map_Coord_Y = null;
        public void nud_value_size_map_ValueChanged(object sender, EventArgs e) {
            if (nud_value_size_map.Value % 2 == 0) nud_value_size_map.Value++;
            if (nud_value_size_map.Value > GAME.Map.Length_X()) nud_value_size_map.Value = GAME.Map.Length_X();
            lb_text_size_map.Text = nud_value_size_map.Value + " x " + nud_value_size_map.Value;
            pnl_Map_Size.Size = new Size(lb_text_size_map.Left + lb_text_size_map.Width + ToCSR(3),
                                         nud_value_size_map.Top + nud_value_size_map.Height + ToCSR(3));
            toolStripMenuItem3.Text = LANGUAGES.RESOURSES[0];/*  "на единицу (+/- 1)";  */
            toolStripMenuItem4.Text = LANGUAGES.RESOURSES[1]/*  на пол страницы (+/-  */ + " " +
                                      (int)(nud_value_size_map.Value / 2) + ")";
            toolStripMenuItem5.Text = LANGUAGES.RESOURSES[2]/*  на страницу (+/-  */ + " " +
                                      nud_value_size_map.Value + ")";
            Update_Panels_Map();
        }

        /// <summary> Массив сохранённых позиций с вкладки карты для интерфейса. Нужен для того чтобы после возвращения стандартного размера, вернуть прежние локации. </summary>
        private Point[] Locations = new Point[6];
        ///<summary> Флаг увеличенной карты: <b>true</b> = full screen / <b>false</b> = default size. </summary>
        private bool FULL_SCREEN_MAP = false;
        public void btn_Map_Full_Screen_Click(object sender, EventArgs e) {
            if (FULL_SCREEN_MAP == true) { FULL_SCREEN_MAP = false; FULL_SCREEN(); Update_Panels_Map(); }//FULL SCREEN
            else { FULL_SCREEN_MAP = true; Update_Panels_Map(); FULL_SCREEN(); }                         //DEFAULT SIZE
        }
        public void FULL_SCREEN() {
            if (FULL_SCREEN_MAP == true) { //FULL SCREEN
                //запоминаем прежние локации интерфейса карты
                Locations[0] = btn_Map_Full_Screen.Location; Locations[1] = pnl_Map_Size.Location;        Locations[2] = pnl_Map_Coord.Location;
                Locations[3] = pnl_Map_DATA.Location;        Locations[4] = pnl_Map_Input_Coord.Location; Locations[5] = pb_XXXX_Map.Location;
                //ставим новые локации интерфейса карты
                pnl_Map_Coord.Location = new Point(pb_MAP.Left + (int)(pb_MAP.Width * 0.68), pb_MAP.Top - ToCSR(5));
                pnl_Map_DATA.Location = new Point(pnl_Map_Coord.Left + pnl_Map_Coord.Width - pnl_Map_DATA.Width, pnl_Map_Coord.Top + pnl_Map_Coord.Height + ToCSR(15));
                pnl_Map_Input_Coord.Location = new Point(pb_MAP.Left + pb_XXXX_Map.Left - pnl_Map_Input_Coord.Width - ToCSR(10), pb_MAP.Top + pb_MAP.Height - pnl_Map_Input_Coord.Height);
            }
            else { //DEFAULT
                //возвращаем прежние локации интерфейса карты
                btn_Map_Full_Screen.Location = Locations[0]; pnl_Map_Size.Location = Locations[1];        pnl_Map_Coord.Location = Locations[2];
                pnl_Map_DATA.Location = Locations[3];        pnl_Map_Input_Coord.Location = Locations[4]; pb_XXXX_Map.Location = Locations[5];
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Update_Panels_Map() {
            Cursor.Current = Cursors.WaitCursor;
            int CoordX = GAME.Map.Location.X; int CoordY = GAME.Map.Location.Y;//коры относительно игровой карты
            txt_Map_Coord.Text = LANGUAGES.RESOURSES[89]/*Центр карты*/ + " (" + CoordX + " | " + CoordY + ")";
            pnl_Map_Coord.Size = new Size(txt_Map_Coord.Left * 2 + txt_Map_Coord.Width, txt_Map_Coord.Top * 2 + txt_Map_Coord.Height);
            tb_Coord_X.Text = CoordX.ToString(); tb_Coord_Y.Text = CoordY.ToString();
            
            //рисуем карту на BitMap PictureBox pb_MAP из данных <b>Map.Cell[][].</b>
            int size = (int)nud_value_size_map.Value;//размер карты 7х7     размер должен быть не чётным !!!!!!!!!!!!!
            double ScaleMap = GAME.Map.ScaleMap(size, FULL_SCREEN_MAP);//масштаб карты (коэффициент размера pb_MAP.Size)
            if (FULL_SCREEN_MAP) { int LocX = 0, LocY = 0;
                /*1080*/if (SCREEN(max:1080)) { LocX = 50; LocY = 150; }
                /*900*/else if (SCREEN(900, 1080)) { LocX = 50; LocY = 160; }
                /*x768*/else if (SCREEN(768, 900)) { LocX = 50; LocY = 165; }
                /*x720*/else if (SCREEN(min: 768)) { LocX = 50; LocY = 170; }
                pb_MAP.Location = new Point(ToCSR(LocX), ToCSR(LocY));
            } else { pb_MAP.Location(LC.pm_X, LC.pm_Y); }//default size map

            Bitmap bmp_Cell = new Bitmap(102, 58);//размер ячейки
            Bitmap bmp_Border;//new Bitmap(1, 1);
            Bitmap bmp_Map = new Bitmap(bmp_Cell.Width * size, bmp_Cell.Height * size);//размер карты 7x7
            pb_MAP.Size((int)(bmp_Map.Width * ScaleMap), (int)(bmp_Map.Height * ScaleMap));
                //bmp.setPixels() bmp.getPixels() - долго. создаём массивы битмапов и работаем с ними (как в SFML DISP[]) 
                BitmapData MapData = bmp_Map.LockBits(new Rectangle(0, 0, bmp_Map.Width, bmp_Map.Height), ImageLockMode.ReadWrite, bmp_Map.PixelFormat);
                // создаём массив для хранения байтов растрового изображения и копируем в него значения RGB
                int LengthMap = Math.Abs(MapData.Stride) * bmp_Map.Height; byte[] RGBA_MAP = new byte[LengthMap];
                System.Runtime.InteropServices.Marshal.Copy(MapData.Scan0, RGBA_MAP, 0, LengthMap);

            Size SizeMapCells = new Size(size, size);//размер отображаемой карты
            float sz = (float)ScaleMap * 9;//размер шрифта меток
            //создаём-удаляем метки
            if (lb_Map_Coord_X != null && size != lb_Map_Coord_X.Length) { 
                for (int i = 0; i < lb_Map_Coord_X.Length; i++) { lb_Map_Coord_X[i].Dispose(); lb_Map_Coord_X[i] = null; }
                for (int i = 0; i < lb_Map_Coord_Y.Length; i++) { lb_Map_Coord_Y[i].Dispose(); lb_Map_Coord_Y[i] = null; }
                lb_Map_Coord_X = lb_Map_Coord_Y = null;
            }
            if (lb_Map_Coord_X == null) { 
                lb_Map_Coord_X = new Label[SizeMapCells.Width]; lb_Map_Coord_Y = new Label[SizeMapCells.Height];
            }

            //коэффециент центрирования карты. Делаем выделенную деревню по центру.
            //размер должен быть не чётным !!!!!!!!!!!!!
            int CenterX = (size / 2), CenterY = (size / 2);
            int Пол_Ширины = (int)(0.5 * bmp_Cell.Width); int Пол_Высоты = (int)(0.5 * bmp_Cell.Height);
            for (int y = 0; y < SizeMapCells.Height; y++) {
                int W = y * Пол_Ширины; int H = Пол_Высоты * (SizeMapCells.Height - y - 1);
                //наносим текстовую разметку координат аля морской бой по оси Y
                if (lb_Map_Coord_Y[y] == null) {
                    lb_Map_Coord_Y[y] = new Label { Parent = bg_Map, AutoSize = true, ForeColor = Color.Black, };
                    lb_Map_Coord_Y[y].SizeFont(sz, FontStyle.Bold); lb_Map_Coord_Y[y].BringToFront();
                }
                if (y == CenterY) lb_Map_Coord_Y[y].ForeColor = Color.Red;
                int VY = y + CoordY - CenterY; VY = GAME.Map.Y_to_Tor(VY);
                lb_Map_Coord_Y[y].Text = (VY).ToString();
                lb_Map_Coord_Y[y].Location = new Point(pb_MAP.Left + ToCSR((int)Math.Round(W * ScaleMap)) - (lb_Map_Coord_Y[y].Width / 2),
                                             pb_MAP.Top + ToCSR((int)Math.Round(H * ScaleMap)) - (lb_Map_Coord_Y[y].Height / 2));
                for (int x = 0; x < SizeMapCells.Width; x++) {
                    //наносим текстовую разметку координат аля морской бой по оси Х
                    if (y == 0) {
                        if (lb_Map_Coord_X[x] == null) { 
                            lb_Map_Coord_X[x] = new Label { Parent = bg_Map, AutoSize = true, ForeColor = Color.Black, };
                            lb_Map_Coord_X[x].SizeFont(sz, FontStyle.Bold); lb_Map_Coord_X[x].BringToFront();
                        }
                        if (x == CenterX) lb_Map_Coord_X[x].ForeColor = Color.Red;
                        int VX = x + CoordX - CenterX; VX = GAME.Map.X_to_Tor(VX);
                        lb_Map_Coord_X[x].Text = (VX).ToString();
                        lb_Map_Coord_X[x].Location = new Point(pb_MAP.Left + ToCSR((int)Math.Round(W * ScaleMap)) - (lb_Map_Coord_X[x].Width / 2),
                                          pb_MAP.Top + ToCSR((int)Math.Round(H * ScaleMap)) + ToCSR((int)Math.Round(bmp_Cell.Height * ScaleMap)) - (lb_Map_Coord_X[x].Height / 2));
                    }
                    //замыкаем мир травиан в ТОР (бублик)
                    int CX = x + GAME.Map.Width + CoordX - CenterX; int CY = y + GAME.Map.Height + CoordY - CenterY;
                    Point XY = GAME.Map.Cell_to_Tor(CX, CY);         CX = XY.X; CY = XY.Y;
                    Cell_ID Cell_id = GAME.Map.Cell[CX, CY].ID;      int PIC_ID = GAME.Map.Cell[CX, CY].pic_ID;
                    if (x == 0 && y == 0) GAME.Map.Location_X0_Y0 = new Point(CX, CY);
                    string path = "DATA_BASE/IMG/map/Cells/";
                    //рисуем рамку нужного цвета поверх ячейки если требуется
                    bmp_Border = new Bitmap(1, 1);
                    if (GAME.Map.Cell[CX, CY].LinkVillage != null) {
                        if (GAME.Map.Cell[CX, CY].LinkAccount.Nick_Name == Player.Nick_Name)
                            bmp_Border = new Bitmap(path + "Border_Yellow.png");//Yellow: цвет игрока
                        else if (GAME.Map.Cell[CX, CY].LinkAccount.Alliance_Name == Player.Alliance_Name && Player.Alliance_Name != "")
                            bmp_Border = new Bitmap(path + "Border_Blue.png");//Blue: цвет альянса игрока
                        //else if () Green: цвет союзного альянса.
                        //else if () Aqua (Светло-синий): цвет альянса, с которым подписан пакт о ненападени.
                        else bmp_Border = new Bitmap(path + "Border_Red.png");//Red: цвет войны или чужой цвет

                        //тест цветных рамок
                        int RR = Random.RND(0, 4); if (RR <= 0) bmp_Border = new Bitmap(path + "Border_Aqua.png");
                        else if (RR <= 1) bmp_Border = new Bitmap(path + "Border_Blue.png");
                        else if (RR <= 2) bmp_Border = new Bitmap(path + "Border_Green.png");
                        else if (RR <= 3) bmp_Border = new Bitmap(path + "Border_Red.png");
                        else if (RR <= 4) bmp_Border = new Bitmap(path + "Border_Yellow.png");
                    }
                    //рисуем ячейку
                    if (Cell_id != Cell_ID.Null) bmp_Cell = new Bitmap(path + Cell_id + "_" + PIC_ID + ".png");
                    else bmp_Cell = new Bitmap(path + "null.png");

                    //тест размеров деревень
                    if (Cell_id >= Cell_ID.Village_Tiny && Cell_id <= Cell_ID.Village_Large) { int R = Random.RND(0, 3);
                        if (R <= 0) bmp_Cell = new Bitmap(path + "Village_Tiny_0.png");
                        else if (R == 1) bmp_Cell = new Bitmap(path + "Village_Small_0.png");
                        else if (R == 2) bmp_Cell = new Bitmap(path + "Village_Medium_0.png");
                        else if (R == 3) bmp_Cell = new Bitmap(path + "Village_Large_0.png");
                    }

                    //Проецируем bmp_Cell на bmp_Map
                        //bmp.setPixels() bmp.getPixels() - долго. создаём массивы битмапов и работаем с ними (как в SFML DISP[]) 
                        //прирост производительности: карта 25х25 Get/Set-Pixel: 7 сек загрузка, 5 сек сдвиг на 1 ячейку
                        //прирост производительности: карта 25х25 BitmapData:    3 сек загрузка, 3 сек сдвиг на 1 ячейку
                        BitmapData CellData = bmp_Cell.LockBits(new Rectangle(0, 0, bmp_Cell.Width, bmp_Cell.Height), ImageLockMode.ReadWrite, bmp_Cell.PixelFormat);
                        // создаём массив для хранения байтов растрового изображения и копируем в него значения RGBA
                        int LengthCell = bmp_Cell.Width * bmp_Cell.Height * 4; byte[] RGBA_Cell = new byte[LengthCell];
                        System.Runtime.InteropServices.Marshal.Copy(CellData.Scan0, RGBA_Cell, 0, LengthCell);

                        byte[] RGBA_BorderCell = null;
                        if (bmp_Border.Width > 1) {
                            BitmapData CellData2 = bmp_Border.LockBits(new Rectangle(0, 0, bmp_Border.Width, bmp_Border.Height), ImageLockMode.ReadWrite, bmp_Border.PixelFormat);
                            // создаём массив для хранения байтов растрового изображения и копируем в него значения RGBA
                            int LengthCell2 = bmp_Border.Width * bmp_Border.Height * 4; RGBA_BorderCell = new byte[LengthCell2];
                            System.Runtime.InteropServices.Marshal.Copy(CellData2.Scan0, RGBA_BorderCell, 0, LengthCell2);
                        }

                        for (int yy = 0; yy < bmp_Cell.Height; yy++) for (int xx = 0; xx < bmp_Cell.Width; xx++) { 
                            int i = (xx + yy * bmp_Cell.Width) * 4; int j = ((W + xx) + (H + yy) * bmp_Map.Width) * 4;
                            //RGBA: [j + 0] = blue, [j + 1] = green, [j + 2] = red, [j + 3] = alpha
                            //переносим пиксели ячейки на карту
                            if (RGBA_Cell[i + 3] != 0) {//если Alpha не Transparent
                                RGBA_MAP[j + 0] = RGBA_Cell[i + 0]; RGBA_MAP[j + 1] = RGBA_Cell[i + 1];
                                RGBA_MAP[j + 2] = RGBA_Cell[i + 2]; RGBA_MAP[j + 3] = RGBA_Cell[i + 3];
                                //переносим пиксели рамки загруженного цвета на карту, если она есть
                                if (bmp_Border.Width > 1) if (RGBA_BorderCell[i + 3] != 0) {//если Alpha не Transparent
                                    RGBA_MAP[j + 0] = RGBA_BorderCell[i + 0]; RGBA_MAP[j + 1] = RGBA_BorderCell[i + 1];
                                    RGBA_MAP[j + 2] = RGBA_BorderCell[i + 2]; RGBA_MAP[j + 3] = RGBA_BorderCell[i + 3];
                                }
                            }
                        } bmp_Cell.UnlockBits(CellData); //bmp_Map.UnlockBits(MapData);
                    W += Пол_Ширины; H += Пол_Высоты;
            }}
            //копируем значения RGB из массива обратно в растровое изображение и разблокировываемся
            System.Runtime.InteropServices.Marshal.Copy(RGBA_MAP, 0, MapData.Scan0, LengthMap); bmp_Map.UnlockBits(MapData);
            pb_MAP.BackgroundImage = bmp_Map;
            pb_XXXX_Map.Location = new Point(pb_MAP.Width - pb_XXXX_Map.Width, pb_MAP.Height - pb_XXXX_Map.Height);
            Cursor.Current = Cursors.Default;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //========================================== МЕТОДЫ ДЛЯ ВКЛАДКИ "КАРТА / MAP" ==========================================
    }
}
