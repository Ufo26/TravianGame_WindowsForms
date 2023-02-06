using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using UFO;
using static GameLogica.Enums_and_structs;
using static GameLogica.TGame;
using static GameLogica.TGame.TPlayer;
using static UFO.Convert;

namespace WindowsInterface {
    interface IForm1_DialogWindows {
        /// <summary> Метод обрабатывает отпускание клавиши для форм. В ччастности закрывает форму <b>sender.</b> </summary>
        /// <remarks> Код отработает если у формы в свойствах стоит <b>KeyPreview</b> = true. </remarks>
        void KeyUp_EscapeFormClose(object sender, KeyEventArgs e);
        /// <summary> Метод убирает фокус ввода на RichTextBox. </summary>
        void rich_Info_Res_SelectionChanged(object sender, EventArgs e);
        /// <summary> Метод обрабатывает выбор постройки из списка построек. </summary>
        void Build_List_SelectedIndexChanged(object sender, EventArgs e);

        /// <summary> Метод создаёт диалоговое окно повышения уровня выбранной постройки. </summary>
        /// <value> <b>NumberSlot:</b> номер слота. </value>
        void winDlg_LevelUp_Builds(int NumberSlot);

        /// <summary> Метод создаёт диалоговое окно с информацией о выбранной ячейке. </summary>
        /// <value> <b>cx / cy:</b> координаты ячейки в массиве <b>Map.Cell[][].</b> </value>
        void winDlg_InfoCell(int cx, int cy);

        /// <summary> Метод создаёт диалоговое окно с профилем игрока. </summary>
        /// <value> <b>account:</b> ссылка на игрока или бота из которого вытаскивается нужная информация. </value>
        void WinDlg_AccountProfile(TPlayer account);

        /// <summary> Метод создаёт диалоговое окно с конкретным отчётом и заполняет свою таблицу значениями <b>GAME.Reports.LIST[n];</b> </summary>
        /// <value>
        ///     <b>index:</b> номер строки в таблице <b>grid_Reports</b> <br/>
        ///     <b>report:</b> ссылка на отчёт в GAME.Reports.LIST[n]; <br/>
        /// </value>
        void WinDlg_Multi_Reports(int index, TReport.TData report);

        /// <summary> Метод создаёт диалоговое окно для написания/чтения сообщения. </summary>
        /// <value>
        ///     <b>index:</b> номер строки в таблице <b>grid_Messages</b> <br/>
        ///     <b>report:</b> ссылка на отчёт в GAME.Messages.LIST[n]; <br/>
        ///     <b>WindowMessage:</b> режим открытия окна сообщения: [Read/Чтение, Write/Запись] <br/>
        /// </value>
        void WinDlg_Message(int index, TMessage.TData message, Window_Message WindowMessage);

        /// <summary> Метод создаёт диалоговое окно с информацией "о программе". </summary>
        void WinDlg_AboutTheProgram();

        /// <summary> Метод создаёт диалоговое окно для переименования выделенной деревни в списке деревень. </summary>
        void btn_Rename_Village_Click(object sender, EventArgs e);
        /// <summary> Метод переименовывет выделенную деревню в списке деревень если новое название не совпадает с другой деревней. </summary>
        void Btn_Rename_Click(object sender, EventArgs e);
        /// <summary> Метод проверяет количество введённых символов и отрубает всё лишнее. </summary>
        void Tb_Rename_TextChanged(object sender, EventArgs e);
    }

    public partial class Form1 : Form, IForm1_DialogWindows {
        public void KeyUp_EscapeFormClose(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) { ((Form)sender).Close(); }
        }

        /// <summary> Метод обрабатывает клик по кнопкам диалоговых окон. </summary>
        public void Buttons_winDlg_Click(object sender, EventArgs e) {
            if (sender != null && sender is Button btn) {
                //центрировать карту
                if (btn.Name == "Buttons_InfoCell_0") {
                    Form_InfoCell.Close(); GAME.Map.Location = GAME.Map.Coord_MapToWorld(((Point)btn.Tag).X, ((Point)btn.Tag).Y);
                    Update_Panels_Map();//обновляет всю информацию на вкладке "КАРТА / MAP"
                }
                //основать поселение
                else if (btn.Name == "Buttons_InfoCell_1") {

                }
                //закрыть диалоговое окно "о программе"
                else if (btn.Name == "About_btn_Close") { Form_About.Close(); }
            }
        }

        //==================================== ДИАЛОГОВОЕ ОКНО :: ЗАПУСКА РЕСУРСНОЙ ПОСТРОЙКИ ====================================
        //private float SaveFontSize = 0;
        /// <summary> Метод применяет выбранный цвет для ссылки в тексте RichTextBox: <b>rich_Info_Res</b>. <br/> Важно! Список параметров цвета по сути чекбоксы, только один из этих параметров может содержать цвет, остальные должны быть равны <b>default</b>. Если ни один параметр не передан, по умолчанию ссылка покрасится в <b>чёрный</b> цвет. </summary>
        private void _LinkColor(float FontSize, Color LinkColor = default, Color ActiveLinkColor = default,
            Color DisabledLinkColor = default, Color VisitedLinkColor = default) {
            Color Cl = LinkColor != default ? LinkColor : ActiveLinkColor != default ? ActiveLinkColor
                : DisabledLinkColor != default ? DisabledLinkColor : VisitedLinkColor != default ? VisitedLinkColor
                    : Color.Black;
            rich_Info_Res.Find_and_PaintLink(Cl, SelectionFontStyle: FontStyle.Bold, FontSize: FontSize);
        }

        /// <summary> Метод обрабатывает клик по ссылке <b>RichTextBox</b> в окне "ЗАПУСКА РЕСУРСНОЙ ПОСТРОЙКИ". </summary>
        private void rich_Info_Res_LinkClicked(object sender, LinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(e.LinkText);//открываем ссылку в веб-браузере по умолчанию
            //_LinkColor(SaveFontSize, VisitedLinkColor: Color.DarkViolet);
        }
        private void rich_Info_Res_MouseEnter(object sender, EventArgs e) {
            //if (Cursor.Current == Cursors.Hand) _LinkColor(SaveFontSize + 2, ActiveLinkColor: Color.Red);
            
        }
        private void rich_Info_Res_MouseMove(object sender, EventArgs e) {
            //if (Cursor.Current == Cursors.Hand) _LinkColor(SaveFontSize + 2, ActiveLinkColor: Color.Red);
            //else _LinkColor(SaveFontSize, LinkColor: Color.Blue);
        }

        /// <summary> Флаг авто ресайза. <b>true</b> = авто ресайз разрешён, <b>false</b> = запрещён. </summary>
        private bool Flag_RichResize = false;
        /// <summary> Хранит фактический размер <b>RichTextBox</b> в который может поместиться текст без полосы прокрутки. </summary>
        private Size ContentsResized = Size.Empty;
        /// <summary> <inheritdoc cref="RichTextBox.ContentsResized"/> </summary>
        private void rich_Info_Res_ContentsResized(object sender, ContentsResizedEventArgs e) {
            if (!Flag_RichResize) return; else Flag_RichResize = false;
            ContentsResized = new Size(e.NewRectangle.Width, e.NewRectangle.Height + 30);
        }
        public void rich_Info_Res_SelectionChanged(object sender, EventArgs e) { form_LevelUp.ActiveControl = null; }
        public void Build_List_SelectedIndexChanged(object sender, EventArgs e) {
            var ComboBox = sender as ComboBox; if ((int)ComboBox.Tag < 0) return;
            BL_SelectedIndex_Save = ComboBox.SelectedIndex;
            winDlg_LevelUp_Builds((int)ComboBox.Tag);//открываем диалоговое окно и передаём в него номер слота
        }

        private void form_LevelUp_Resource_Close(object sender, FormClosingEventArgs e) {
            form_LevelUp.Visible = Build_List.Visible = false; ActiveSlot_in_winDlg_LevelUp_Builds = -1;
        }

        /// <summary>
        ///     Метод получает число вначале строки из списка <b>Build_List</b> с проверкой всех недопустимых значений. <br/>
        ///     Если в списке ничего не выбрано, метод вернёт или первую строку, или покрасит кнопки <b>btn_LevelUp</b>, <b>btn_LevelDown_Destroy</b> и сделает их недоступными. <br/>
        ///     Этот номер может быть или <b>ID</b> постройки или <b>номером слота</b>. Всё зависит от того каким методом создавался список: "для построек" или "для сноса".
        /// </summary>
        /// <returns> Возвращает или <b>ID</b> постройки или <b>номер слота</b>, в случае неудачи вернёт <b>-1.</b> </returns>
        private int get_Number() { 
            return Build_List.SelectedIndex > -1 ? Get_Number_From_ComboBox_Items(Build_List.Items[Build_List.SelectedIndex].ToString())
                : Build_List.Items.Count > 0 ? Get_Number_From_ComboBox_Items(Build_List.Items[0].ToString())
                    : (int)Buildings.ПУСТО;
        }

        /// <summary> Хранит варианты состояний <b>ComboBox</b> Build_List. <br/> <b>Строительство </b> = в Items список доступных построек. <br/> <b>Снос</b> = в Items список построек на снос. <br/> <b>ПУСТО = </b> Items пустой. </summary>
        private enum What_In_Items { Строительство, Снос, ПУСТО }
        /// <summary> Экземпляр enum перечисления. <inheritdoc cref="_Items"/> <br/> Значение меняется вместе с изменением списка на другой список. </summary>
        private What_In_Items This_List = What_In_Items.ПУСТО;
        /// <summary> Переменная связанная с диалоговым окном повышения уровня выбранной постройки. </summary>
        private Form form_LevelUp = null;
        /// <summary> <inheritdoc cref="form_LevelUp"/> </summary>
        private RichTextBox rich_Info_Res = null;
        /// <summary> <inheritdoc cref="form_LevelUp"/> </summary>
        private PictureBox pb_Image_Build = null;
        /// <summary> <inheritdoc cref="form_LevelUp"/> </summary>
        private PictureBox[] pb_pic_res = null;
        /// <summary> <inheritdoc cref="form_LevelUp"/> </summary>
        private Label[] lb_pic_res = null;
        /// <summary> <inheritdoc cref="form_LevelUp"/> </summary>
        private CustomControls.RoundAPIButton btn_LevelUp = null;
        /// <summary> <inheritdoc cref="form_LevelUp"/> </summary>
        private CustomControls.RoundAPIButton btn_LevelDown_Destroy = null;
        /// <summary> <inheritdoc cref="form_LevelUp"/> </summary>
        private ComboBox Build_List = null;
        /// <summary> <inheritdoc cref="form_LevelUp"/> </summary>
        private StatusStrip stStrip = null;
        /// <summary> Поле запоминает <b>Build_List.SelectedIndex</b> чтобы после завершения строительства вернуть в отформатированный выпадающий список выбранную ранее постройку. </summary>
        private int BL_SelectedIndex_Save = 0;
        /// <summary> Поле хранит активный слот в открытом окне повышения уровня выбранной постройки. </summary>
        private int ActiveSlot_in_winDlg_LevelUp_Builds = -1;
        public void winDlg_LevelUp_Builds(int NumberSlot) {
            if (NumberSlot < 0) return; else ActiveSlot_in_winDlg_LevelUp_Builds = NumberSlot;
            Color BgCl_Active = Color.FromArgb(100, 150, 0);//цвет фона кнопок добавления/сноса постройки
            Color BgCl_Deactive = Color.FromArgb(128, 128, 128);//цвет фона кнопок добавления/сноса постройки
            Color TextColor = Color.Black;//цвет текста лэйблов
            int tabX = ToCSR(40);//отступ от левого края
            int Max_lvl = 20;/*default :: если слот пустой или строимся в деревне*/
            Buildings ID = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID;//что в слоте в который кликнули
            if (ID == Buildings.ЧУДО_СВЕТА) Max_lvl = 100;
            else if (ID >= Buildings.Лесопильный_завод && ID <= Buildings.Пекарня) Max_lvl = 5;
            else if (ID == Buildings.Тайник) Max_lvl = 10;/*тайник 10 ур.*/
            else if (Player.ActiveIndex == Player.NumberOfCapital) Max_lvl = 20;
            else if (ID >= Buildings.Лесопилка && ID <= Buildings.Ферма) Max_lvl = (int)Player.Limit_Lvl;
            int Уровень = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].Level;
            int Сносимый_Уровень = -1;//это уровень для выбранной постройки которую можно снести
            bool Строится = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ProcessOfConstruction;
            Buildings ID_Строящегося = Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID_ProcessOfConstruction;
            int lvl = Актуальный_Уровень_С_Учётом_Строительства_И_Сноса(NumberSlot);
            int lvl_Next; if (lvl + 1 <= Max_lvl) lvl_Next = lvl + 1; else lvl_Next = Max_lvl;
            int Бонус_Главного_Здания = Player.VillageList[Player.ActiveIndex].BonusOfTime_Construction;

            //ТЕСТ ОТСУТСТВИЯ ГЛАВНОГО ЗДАНИЯ
            //Player.VillageList[Player.ActiveIndex].Slot[27].Level = 0;
            if (Player.VillageList[Player.ActiveIndex].Slot[27].Level <= 0) if (NumberSlot >= 18 && NumberSlot != 27)
                { MessageBox.Show(LANGUAGES.RESOURSES[99]); return; }//Строительство заблокировано. Постройте главное здание до 1 уровня.

            if (form_LevelUp == null) {
                //форма
                form_LevelUp = Extensions.CreateForm("form_LevelUp", new Icon("DATA_BASE/IMG/logotip.ico"),
                    new Font(Font.FontFamily, ToCSR(10), FontStyle.Bold),
                    StartPosition: FormStartPosition.CenterScreen, FormBorderStyle: FormBorderStyle.Fixed3D,
                    ControlBox: true, KeyPreview: true);
                form_LevelUp.FormClosing += form_LevelUp_Resource_Close;
                form_LevelUp.KeyUp += new KeyEventHandler(KeyUp_EscapeFormClose);
                //текстовая информация
                rich_Info_Res = new RichTextBox {
                    Parent = form_LevelUp, Location = new Point(ToCSR(20), ToCSR(20)),
                    ReadOnly = true, WordWrap = true, ScrollBars = RichTextBoxScrollBars.Vertical, Cursor = Cursors.Default,
                    BorderStyle = BorderStyle.None, BackColor = form_LevelUp.BackColor,
                    MinimumSize = new Size(500, 160), MaximumSize = new Size(1200, 600),
                };
                rich_Info_Res.LinkClicked += rich_Info_Res_LinkClicked; 
                rich_Info_Res.MouseEnter += rich_Info_Res_MouseEnter;
                rich_Info_Res.MouseMove += rich_Info_Res_MouseMove;
                rich_Info_Res.ContentsResized += rich_Info_Res_ContentsResized;
                rich_Info_Res.GotFocus += rich_Info_Res_SelectionChanged; rich_Info_Res.BringToFront();
                //картинка постройки
                pb_Image_Build = new PictureBox { Parent = form_LevelUp, BackgroundImageLayout = ImageLayout.Stretch, };
                pb_Image_Build.BringToFront();
                //Массив ресурсных пиктограмм 
                //[0] = wood    [1] = clay      [2] = iron      [3] = crop      [4] = cons_crop     [5] = CLOCK     [6] = culturePoints
                pb_pic_res = new PictureBox[7];
                for (int i = 0; i < pb_pic_res.Length; i++) {
                    Size SZ; if (i == 5) SZ = new Size(22, 22); else if (i == 6) SZ = new Size(24, 22); else SZ = new Size(32, 22);
                    pb_pic_res[i] = new PictureBox { Parent = form_LevelUp, Size = SZ, BackgroundImageLayout = ImageLayout.Stretch, };
                    if (i < 5) pb_pic_res[i].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/0" + i + ".png");
                    else if (i == 5) pb_pic_res[i].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/CLOCK.png");
                    else if (i == 6) pb_pic_res[i].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/culturePoints.png");
                    pb_pic_res[i].BringToFront();
                }

                //массив величин строительства постройки {wood, clay, iron, crop, cons_crop, gold}
                lb_pic_res = new Label[pb_pic_res.Length + 2];
                for (int i = 0; i < lb_pic_res.Length; i++) {
                    FontStyle fs; float SZ; if (i >= 5) { fs = FontStyle.Bold; SZ = 12; } else { fs = FontStyle.Bold; SZ = 10; }
                    lb_pic_res[i] = new Label {
                        Parent = form_LevelUp, Font = new Font(Font.FontFamily, ToCSR(SZ), fs),
                        ForeColor = TextColor, AutoSize = true,
                    }; lb_pic_res[i].BringToFront(); if (i == 8) lb_pic_res[i].ForeColor = Color.Red;
                }
            }

            //блок всплывающих подсказок. прописаны здесь, потому что контролы создаются и уничтожаются здесь
            string[] TITLE = new string[4], TXT = new string[4];
            for (int i = 0; i < 4; i++) {
                string Title = LANGUAGES.tool_tip_TITLE[17 + i];    string Text = LANGUAGES.tool_tip_TEXT[17 + i];
                string[] title = Title.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries); string[] txt = Text.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                for (int it = 0; it < title.Length; it++) TITLE[i] += title[it] + "\n"; for (int it = 0; it < txt.Length; it++) TXT[i] += txt[it] + "\n";
            }
            tool_tip[17] = Extensions.CreateHint(pb_pic_res[5], 0, 30000, TITLE[0], TXT[0], Color.GreenYellow, Color.DarkGreen, ToolTipIcon.Info, false);
            tool_tip[18] = Extensions.CreateHint(pb_pic_res[6], 0, 30000, TITLE[1], TXT[1], Color.GreenYellow, Color.DarkGreen, ToolTipIcon.Info, false);
            tool_tip[19] = Extensions.CreateHint(lb_pic_res[5], 0, 30000, TITLE[2], TXT[2], Color.GreenYellow, Color.DarkGreen, ToolTipIcon.Info, false);
            tool_tip[20] = Extensions.CreateHint(lb_pic_res[6], 0, 30000, TITLE[3], TXT[3], Color.GreenYellow, Color.DarkGreen, ToolTipIcon.Info, false);

            //ComboBox. выпадающий список доступных построек
            if (Build_List == null) {
                Build_List = new ComboBox {
                    Parent = form_LevelUp, Font = new Font(Font.FontFamily, ToCSR(12), FontStyle.Regular),
                    ForeColor = Color.Black, Width = ToCSR(320), Visible = false, DropDownStyle = ComboBoxStyle.DropDownList,
                    Tag = -1,
                };
                Build_List.SelectedIndexChanged += Build_List_SelectedIndexChanged; Build_List.BringToFront();
                Build_List.SelectedIndex = Build_List.Items.Count > 0 ? 0 : -1;
                //выясняем какой список грузим в Items ComboBox
                if (ID == Buildings.Главное_здание) {
                    Build_List.Items.Clear(); var _Items = Get_ComboBox_Items_Destruction();
                    if (_Items != null) Build_List.Items.AddRange(_Items);
                    This_List = What_In_Items.Снос; //Build_List.CallBack_SelectedIndexChanged();
                } else {
                    Build_List.Items.Clear(); var _Items = Get_ComboBox_Items();
                    if (_Items != null) Build_List.Items.AddRange(_Items);
                    This_List = What_In_Items.Строительство; //Build_List.CallBack_SelectedIndexChanged();
                }
                Build_List.Set_SelectedIndex_And_Changed_Ignore(0);
            }
            Build_List.Tag = NumberSlot;/*номер слота*/
            //подгон ширины выпадающего списка по максимальной ширине текста
            for (int i = 0; i < Build_List.Items.Count; i++) {
                int w = TextRenderer.MeasureText(Build_List.Items[i].ToString(), Build_List.Font).Width + ToCSR(20);
                Build_List.Width = Build_List.Width < w ? w : Build_List.Width;
            }

            //кнопка запуска следующего уровня постройки
            float sz = 10;
            if (btn_LevelUp == null) {
                btn_LevelUp = new CustomControls.RoundAPIButton() {
                    Parent = form_LevelUp, Name = "btn_LevelUp",
                    Font = new Font(Font.FontFamily, ToCSR(sz), FontStyle.Bold),
                    Text = "текст ниже", ForeColor = Color.White, BackColor = BgCl_Active,
                    Ellipse_Percent = 30
                };
                btn_LevelUp.Button_Styles(BgCl_Active, Color.Black,
                    Color.FromArgb(BgCl_Active.R / 3, BgCl_Active.G / 3, BgCl_Active.B / 3), Color.Black, Color.White, 0);
                btn_LevelUp.BringToFront(); btn_LevelUp.Click += Control_Click;
            }
            btn_LevelUp.Tag = NumberSlot;
            //кнопка сноса уровня постройки на один уровень ниже
            if (btn_LevelDown_Destroy == null) {
                btn_LevelDown_Destroy = new CustomControls.RoundAPIButton() {
                    Parent = form_LevelUp, Name = "btn_LevelDown_Destroy",
                    Font = new Font(Font.FontFamily, ToCSR(sz), FontStyle.Bold),
                    Text = "текст ниже", ForeColor = Color.White, BackColor = BgCl_Active,
                    Ellipse_Percent = 30
                };
                btn_LevelDown_Destroy.Button_Styles(BgCl_Active, Color.Black, Color.DarkRed, Color.Black, Color.White, 0);
                btn_LevelDown_Destroy.BringToFront(); btn_LevelDown_Destroy.Click += Control_Click;
            }
            btn_LevelDown_Destroy.Tag = NumberSlot;

            //StatusStrip. Полоска с инфой.
            if (stStrip == null) {
                stStrip = new StatusStrip { Parent = form_LevelUp, ForeColor = Color.Black, 
                    Font = new Font(Font.FontFamily, ToCSR(10), FontStyle.Regular), Dock = DockStyle.Bottom,
                }; stStrip.Items.Add("");
            }

            //СПИСКИ МЕНЯЮТСЯ В ЗАВИСИМОСТИ ОТ ВЫБРАННОГО СЛОТА В ТАЙМЕРЕ И В КЛИКЕ form1
            //показать выпадающий список Build_List и загрузить в него все постройки которые можно снести
            if (ID == Buildings.Главное_здание) { 
                Build_List.Visible = true; btn_LevelDown_Destroy.Visible = true;
                btn_LevelDown_Destroy.Tag = get_Number();//номер слота
                if ((int)btn_LevelDown_Destroy.Tag >= 0 && (int)btn_LevelDown_Destroy.Tag < (int)Buildings.ПУСТО)
                    Сносимый_Уровень = Актуальный_Уровень_С_Учётом_Строительства_И_Сноса((int)btn_LevelDown_Destroy.Tag);
            } else {
                //показать-скрыть выпадающий список Build_List и загрузить в ID нужный номер из списка
                if (ID == Buildings.ПУСТО) { 
                    if (Строится) { Build_List.Visible = false; ID = ID_Строящегося; } 
                    else { Build_List.Visible = true; ID = (Buildings)get_Number(); }
                }
                btn_LevelDown_Destroy.Visible = false;
            }
            rich_Info_Res.SelectionStart = 0; rich_Info_Res.ScrollToCaret();//смещаем полосу прокрутки наверх

            //тест отсутствия выбора постройки и отсутствия ID
            //ID = Buildings.ПУСТО; Build_List.Set_SelectedIndex_And_Changed_Ignore(-1);

            string Имя = LANGUAGES.buildings[(int)Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID];//номер в поле ".ID" совпадает с номером строки в массиве buildings[] включая пустой слот (42)
            form_LevelUp.Text = "id_" + NumberSlot + ". " + Имя + ". " + LANGUAGES.RESOURSES[3]/*Level*/ + " " +
                Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].Level;
            //загружаем rtf файл с описанием постройки и вычисляем подходящий размер rich_Info_Res для rtf текста
            rich_Info_Res.LoadFile(Directory.GetDirectories("DATA_BASE/LANGUAGES/")[GAME.Language] + "/buildings_RTF/g" + (int)ID + "-ltr.rtf");
            //запоминаем дефолтный размер шрифта ссылки
            //int pos = rich_Info_Res.Find("https://"); if (pos > -1) { rich_Info_Res.SelectionStart = pos; SaveFontSize = rich_Info_Res.SelectionFont.Size; }
            rich_Info_Res.Size = Size.Empty;//КОСТЫЛЬ! авторесайз не срабатывает если применить тот же размер
            Flag_RichResize = true;//однократное автоизменение высоты, флаг автоматом встаёт на false после ресайза
            //- вкладка ресурсы
            if (ID > 0 && ID <= Buildings.Ферма) rich_Info_Res.Width = 580;
            //- вкладка деревня + Buildings.ПУСТО
            else if (ID >= 0 && ID <= Buildings.ПУСТО) rich_Info_Res.Width = rich_Info_Res.Lines.Length < 15 ? 700 : 900;
            else rich_Info_Res.Width = 666;//- Buildings вне диапазона (деревня)
            Flag_RichResize = false; rich_Info_Res.ClientSize = ContentsResized; rich_Info_Res.ReSize();

            var _ = pb_Image_Build;
            if (ID < 0 || ID >= Buildings.ПУСТО) {//грузим логотип вместо постройки
                _.BackgroundImage = Image.FromFile("DATA_BASE/IMG/building/logo.jpg"); //_.Size(304, 394);
            } else {//грузим постройку
                try { _.BackgroundImage = Image.FromFile($"DATA_BASE/IMG/building/g{(int)ID}-ltr2.png"); }
                catch (FileNotFoundException) { _.BackgroundImage = Image.FromFile($"DATA_BASE/IMG/building/g{(int)ID}-ltr.png"); }
            }
            _.Size(_.BackgroundImage.Width, _.BackgroundImage.Height);
            pb_Image_Build.Location = new Point(rich_Info_Res.Left + rich_Info_Res.Width + 10, ToCSR(40));
            Build_List.Location = new Point(tabX, rich_Info_Res.Top + rich_Info_Res.Height + ToCSR(20));

            //постройки с уникальным параметром 1 в файле g[n]-ltr.txt
            //(все кроме [0] ЧУДА, [13] КУЗНИЦЫ, [22] АКАДЕМИИ, [24] РАТУШИ, [25] РЕЗИДЕНЦИИ, [26] ДВОРЦА, [38] ТАВЕРНЫ)
            //ID = 24 РАТУША - теперь имеет 2 уникальных параметра хранящих время, но тут отображать не надо
            string str1 = "[str1]"; string str2 = "[str2]";
            string str3 = "[str3]"; string str4 = "[str4]"; bool UNIKUM_2 = false;
            if (ID >= Buildings.ЧУДО_СВЕТА && ID < Buildings.ПУСТО ) {
                if (ID != 0 && ID != Buildings.Кузница && ID != Buildings.Академия && ID != Buildings.Ратуша && 
                    ID != Buildings.Резиденция && ID != Buildings.Дворец && ID != Buildings.Таверна) {
                    double множитель = 1.0;
                    if (ID == Buildings.Тайник) if (Player.Folk_Name == Folk.Gaul) множитель = 2.0;
                    str1 = (GAME.Build[(int)ID].Information[Уровень].Unique_Parameter_1 * множитель).ToString();
                    str2 = (GAME.Build[(int)ID].Information[lvl_Next].Unique_Parameter_1 * множитель).ToString();
                } else { str1 = Уровень.ToString(); str2 = (lvl_Next).ToString(); }

                //постройки с уникальным параметром 2 в файле g[n]-ltr.txt
                if (ID == Buildings.Госпиталь || ID >= Buildings.Городская_стена_Римляне || 
                    ID == Buildings.Земляной_вал_Германцы || ID <= Buildings.Изгородь_Галлы || ID == Buildings.Стена_Натары) {
                    UNIKUM_2 = true;
                    str3 = GAME.Build[(int)ID].Information[lvl].Unique_Parameter_2.ToString();
                    str4 = GAME.Build[(int)ID].Information[lvl_Next].Unique_Parameter_2.ToString();
                } 
            }

            rich_Info_Res.Find_and_Replace("<>", str1, Color.Black, "<>".Length, FontSize: 11F, SelectionFontStyle: FontStyle.Bold);
            if (Уровень < Max_lvl) {
                //информация о следующем уровне постройки
                if (ID != 0) rich_Info_Res.Find_and_Replace("[]", " " + (lvl_Next).ToString() + ":\t", Color.Red, "[]".Length, FontSize: 11F, SelectionFontStyle: FontStyle.Bold);
                rich_Info_Res.Find_and_Replace("<>", str2, Color.Black, "<>".Length, FontSize: 11F, SelectionFontStyle: FontStyle.Bold);
                if (UNIKUM_2) {
                    rich_Info_Res.Find_and_Replace("<>", str3, Color.Black, "<>".Length, FontSize: 11F, SelectionFontStyle: FontStyle.Bold);
                    rich_Info_Res.Find_and_Replace("[]", " " + (lvl_Next).ToString() + ":\t", Color.Red, "[]".Length, FontSize: 11F, SelectionFontStyle: FontStyle.Bold);
                    rich_Info_Res.Find_and_Replace("<>", str4, Color.Black, "<>".Length, FontSize: 11F, SelectionFontStyle: FontStyle.Bold);
                }
                rich_Info_Res.Find_and_Replace("@", "", Color.Black, "@".Length, FontSize: 11F, SelectionFontStyle: FontStyle.Regular);//удаляем спец. символ

                //расходы на строительство до следующего уровня
                var PRIS = Player.VillageList[Player.ActiveIndex].ResourcesInStorages;
                Res CC = new Res(); lb_pic_res[8].Text = "--";
                if (ID >= 0 && ID < Buildings.ПУСТО) {
                    CC = GAME.Build[(int)ID].Information[lvl_Next].Construction_Costs;
                    if (PRIS.wood < CC.wood) lb_pic_res[0].ForeColor = Color.Red; else lb_pic_res[0].ForeColor = TextColor;
                    if (PRIS.clay < CC.clay) lb_pic_res[1].ForeColor = Color.Red; else lb_pic_res[1].ForeColor = TextColor;
                    if (PRIS.iron < CC.iron) lb_pic_res[2].ForeColor = Color.Red; else lb_pic_res[2].ForeColor = TextColor;
                    if (PRIS.crop < CC.crop) lb_pic_res[3].ForeColor = Color.Red; else lb_pic_res[3].ForeColor = TextColor;
                    lb_pic_res[7].Text = LANGUAGES.RESOURSES[47] + " ";/*Расходы на строительство до уровня*/
                    lb_pic_res[8].Text = (lvl_Next) + ":";
                }

                if (Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID == Buildings.ПУСТО ||
                    Player.VillageList[Player.ActiveIndex].Slot[NumberSlot].ID == Buildings.Главное_здание)
                    lb_pic_res[7].Location = new Point(tabX, Build_List.Top + Build_List.Height + ToCSR(20));
                else lb_pic_res[7].Location = new Point(tabX, rich_Info_Res.Top + rich_Info_Res.Height + ToCSR(20));
                lb_pic_res[8].Location = new Point(lb_pic_res[7].Left + lb_pic_res[7].Width, lb_pic_res[7].Top);
                pb_pic_res[0].Location = new Point(tabX, lb_pic_res[7].Top + lb_pic_res[7].Height + ToCSR(10));
                lb_pic_res[0].Text = toTABString(CC.wood.ToString());
                lb_pic_res[0].Location = new Point(pb_pic_res[0].Left + pb_pic_res[0].Width + 5, pb_pic_res[0].Top);
                pb_pic_res[1].Location = new Point(lb_pic_res[0].Left + lb_pic_res[0].Width + ToCSR(10), pb_pic_res[0].Top);
                lb_pic_res[1].Text = toTABString(CC.clay.ToString());
                lb_pic_res[1].Location = new Point(pb_pic_res[1].Left + pb_pic_res[1].Width + 5, pb_pic_res[0].Top);
                pb_pic_res[2].Location = new Point(lb_pic_res[1].Left + lb_pic_res[1].Width + ToCSR(10), pb_pic_res[0].Top);
                lb_pic_res[2].Text = toTABString(CC.iron.ToString());
                lb_pic_res[2].Location = new Point(pb_pic_res[2].Left + pb_pic_res[2].Width + 5, pb_pic_res[0].Top);
                pb_pic_res[3].Location = new Point(lb_pic_res[2].Left + lb_pic_res[2].Width + ToCSR(10), pb_pic_res[0].Top);
                lb_pic_res[3].Text = toTABString(CC.crop.ToString());
                lb_pic_res[3].Location = new Point(pb_pic_res[3].Left + pb_pic_res[3].Width + 5, pb_pic_res[0].Top);
                pb_pic_res[4].Location = new Point(lb_pic_res[3].Left + lb_pic_res[3].Width + ToCSR(10), pb_pic_res[0].Top);
                lb_pic_res[4].Text = "--";
                if (ID >= 0 && ID < Buildings.ПУСТО) lb_pic_res[4].Text = toTABString(GAME.Build[(int)ID].Information[lvl_Next].Consumption.ToString());
                lb_pic_res[4].Location = new Point(pb_pic_res[4].Left + pb_pic_res[4].Width + 5, pb_pic_res[0].Top);
                pb_pic_res[5].Location = new Point(tabX, pb_pic_res[0].Top + pb_pic_res[0].Height + ToCSR(20));
                lb_pic_res[5].Text = "??:??:??";
                if (ID >= 0 && ID < Buildings.ПУСТО) lb_pic_res[5].Text = ToTimeString((int)(GAME.Build[(int)ID].Information[lvl_Next].Time_Construction / 100.0 * Бонус_Главного_Здания),
                    LANGUAGES.Time[0], LANGUAGES.Time[1], LANGUAGES.Time[2]//часы, минуты, секунды: ч:м:с
                );
                lb_pic_res[5].Location = new Point(pb_pic_res[5].Left + pb_pic_res[5].Width + 5, pb_pic_res[5].Top);
                pb_pic_res[6].Location = new Point(lb_pic_res[5].Left + lb_pic_res[5].Width + ToCSR(50), pb_pic_res[5].Top);
                lb_pic_res[6].Text = "--";
                if (ID >= 0 && ID < Buildings.ПУСТО) lb_pic_res[6].Text = toTABString(GAME.Build[(int)ID].Information[lvl_Next].ProductionCulture.ToString());
                lb_pic_res[6].Location = new Point(pb_pic_res[6].Left + pb_pic_res[6].Width + 5, pb_pic_res[5].Top);
                for (int i = 0; i < pb_pic_res.Length; i++) { pb_pic_res[i].Visible = true; lb_pic_res[i].Visible = true; }
                lb_pic_res[7].Visible = true; lb_pic_res[8].Visible = true;
                btn_LevelUp.Text = LANGUAGES.RESOURSES[48] + " "/*Улучшить до уровня*/ + (lvl_Next); btn_LevelUp.Enabled = true;
                string tmp = LANGUAGES.RESOURSES[49] + " ";/*Снести до уровня*/
                if (Сносимый_Уровень - 1 > 0) tmp += (Сносимый_Уровень - 1);
                else if (Сносимый_Уровень >= 0) tmp = LANGUAGES.RESOURSES[50];//Снести полностью
                else tmp += " ?";
                btn_LevelDown_Destroy.Text = tmp;
                btn_LevelUp.Location = new Point(tabX, pb_pic_res[5].Top + pb_pic_res[5].Height + ToCSR(15));
            } else {
                //это здание отстроено полностью
                string tmp1 = "@"; int length = rich_Info_Res.Text.Length - rich_Info_Res.Find(tmp1, RichTextBoxFinds.MatchCase);
                rich_Info_Res.Find_and_Replace(tmp1, LANGUAGES.RESOURSES[51]/*Это здание отстроено полностью.*/,
                    Color.Blue, length, FontSize: 11F, SelectionFontStyle: FontStyle.Bold);//стираем с текущей позиции до конца
                for (int i = 0; i < pb_pic_res.Length; i++) { pb_pic_res[i].Visible = false; lb_pic_res[i].Visible = false; }
                lb_pic_res[7].Visible = false; lb_pic_res[8].Visible = false;
                btn_LevelUp.Text = LANGUAGES.RESOURSES[48] + " ?";/*Улучшить до уровня*/
                btn_LevelDown_Destroy.Text = LANGUAGES.RESOURSES[49]/*Снести до уровня*/ + " " + (Сносимый_Уровень - 1);
                btn_LevelUp.Location = new Point(tabX, rich_Info_Res.Top + rich_Info_Res.Height + ToCSR(15));
            }
            //красим кнопки в зависимости от уровня
            btn_LevelUp.ForeColor = btn_LevelDown_Destroy.ForeColor = Color.White;
            if (ID < Buildings.ЧУДО_СВЕТА || ID > Buildings.ПУСТО) {
                btn_LevelUp.BackColor = BgCl_Deactive; btn_LevelUp.Enabled = false;
                btn_LevelDown_Destroy.BackColor = BgCl_Deactive; btn_LevelDown_Destroy.Enabled = false;
            } else if (lvl > 0 && lvl < Max_lvl) {
                btn_LevelUp.BackColor = BgCl_Active; btn_LevelUp.Enabled = true;
                btn_LevelDown_Destroy.BackColor = BgCl_Active; btn_LevelDown_Destroy.Enabled = true;
            } else if (lvl <= 0) { 
                btn_LevelDown_Destroy.BackColor = BgCl_Deactive; btn_LevelDown_Destroy.Enabled = false;
                btn_LevelUp.BackColor = BgCl_Active; btn_LevelUp.Enabled = true;
            } else if (lvl >= Max_lvl) {
                btn_LevelDown_Destroy.BackColor = BgCl_Active; btn_LevelDown_Destroy.Enabled = true;
                btn_LevelUp.BackColor = BgCl_Deactive; btn_LevelUp.Enabled = false;
            }

            //если Index выпадающего списка не выбран, блокируем эти кнопки и разблокируем если Index выбран
            if (ID > Buildings.Ферма) if (Build_List.SelectedIndex < 0)
                if (This_List == What_In_Items.Строительство) { 
                if (ID != Buildings.Главное_здание && ID != Buildings.Пункт_сбора && 
                    ID != Buildings.Городская_стена_Римляне && ID != Buildings.Земляной_вал_Германцы &&
                    ID != Buildings.Изгородь_Галлы && ID != Buildings.Стена_Натары) {
                    btn_LevelUp.Enabled = false;
                    btn_LevelUp.Text = LANGUAGES.RESOURSES[109];//Выберите постройку
                    btn_LevelUp.BackColor = Color.FromArgb(200, 200, 200); btn_LevelUp.ForeColor = Color.FromArgb(0, 0, 0);
                }
                } else if (This_List == What_In_Items.Снос) { 
                    btn_LevelDown_Destroy.Enabled = false;
                    btn_LevelDown_Destroy.Text = LANGUAGES.RESOURSES[109];//Выберите постройку
                    btn_LevelDown_Destroy.BackColor = Color.FromArgb(200, 200, 200); btn_LevelDown_Destroy.ForeColor = Color.FromArgb(0, 0, 0);
                }

            btn_LevelDown_Destroy.Location = new Point(Build_List.Left + Build_List.Width + ToCSR(10), Build_List.Top);
            
            rich_Info_Res.ZoomFactor();//масштабирвоание текста под разное разрешение экрана

            btn_LevelUp.AutoSize(); btn_LevelDown_Destroy.AutoSize();//устанавливаем авторазмер по тексту кнопок
            //добавляем к размеру отступы
            btn_LevelUp.Width += ToCSR(20); btn_LevelUp.Height += ToCSR(10);
            btn_LevelDown_Destroy.Width += ToCSR(20); btn_LevelDown_Destroy.Height += ToCSR(10);

            //размер формы
            int H = pb_Image_Build.Top + pb_Image_Build.Height > btn_LevelUp.Top + btn_LevelUp.Height ?
                pb_Image_Build.Top + pb_Image_Build.Height + stStrip.Height + ToCSR(75)
                : btn_LevelUp.Top + btn_LevelUp.Height + stStrip.Height + ToCSR(75);
            form_LevelUp.Size = new Size(pb_Image_Build.Left + pb_Image_Build.Width + ToCSR(30), H);
            stStrip.Items[0].Text = $"Window: {form_LevelUp.Width}x{form_LevelUp.Height} px." +
                $"     RichTextBox: {rich_Info_Res.Size.Width}x{rich_Info_Res.Size.Height} px." +
                $"     ID = [{(int)ID}]({ID})     Lines = {rich_Info_Res.Lines.Length}";
            form_LevelUp.Centering();//потому что!
            if (!form_LevelUp.Visible) { form_LevelUp.ShowDialog(); }
        }
        //==================================== ДИАЛОГОВОЕ ОКНО :: ЗАПУСКА РЕСУРСНОЙ ПОСТРОЙКИ ====================================

        //======================================== ДИАЛОГОВОЕ ОКНО :: ИНФОРМАЦИЯ О ЯЧЕЙКЕ ========================================
        /// <summary> Метод обрабатывает закрытие формы "Информация о ячейке". </summary>
        void Form_InfoCell_Closing(object sender, EventArgs e) {
            //вызываем обработчик в котором скрываем панельку краткой информации о деревне и затираем общую информацию
            Control_MouseMove(new PictureBox { Name = "pb_MAP" }, new MouseEventArgs(MouseButtons.None, -1, 0, 0, 0));
        }

        /// <summary> Ссылка на диалоговое окно с информацией о выбранной ячейке. </summary>
        private Form Form_InfoCell = null;
        /// <summary> Заголовок с названием ячейки и координатами. </summary>
        private Label HeadText_InfoCell = null;
        /// <summary> Большая картинка типа выбранного поля: 1-1-1-15, 3-3-3-9, 4-4-4-6 и т.д. и типа оазиса. </summary>
        private PictureBox img_Field_InfoCell = null;
        /// <summary> Панель с информацией справа от картинки. </summary>
        private Panel Panel_InfoCell = null;
        ///<summary> 4 кнопки под картинкой: 1. центрировать карту, 2. основать поселение, 3. отправить войска, 4. отправить торговцев. </summary>
        private Button[] Buttons_InfoCell = null;
        //ОАЗИС
        /// <summary> Текстовые метки для вкладки "ОАЗИС". </summary>
        private Label[] lb_Oasis_Text = new Label[4];
        /// <summary> Отображение войск: Картинка + величина + название. 10 строк, если войск больше, то появляется полоса прокрутки. </summary>
        private DataGridView grid_Troops = null;
        /// <summary> Хранит 2 пиктограммы с ресурсами для вкладки "ОАЗИС". </summary>
        private PictureBox[] pic2_Oasis = new PictureBox[2];
        //ОСТАЛЬНЫЕ ВКЛАДКИ
        /// <summary> Текстовая информация о кол-ве ресурсных полей для остальных вкладкок <br/> 4 - величина, 4 - название. </summary>
        private Label[] lb_Field_Res = new Label[8];
        /// <summary> Текстовая информация остальных вкладок. </summary>
        private Label[] lb_Field = new Label[4];
        /// <summary> Хранит все пиктограммы с ресурсами для остальных вкладок. </summary>
        private PictureBox[] pic5_Field = new PictureBox[5];
        /// <summary> Информация о деревне. </summary>
        private Label[] lb_Village = new Label[10];
        //ЭЛЕМЕНТЫ ОТЧЁТА (СПРАВА ВНИЗУ)
        private Label HeadText_Report = null;
        //Это окно имеет 3 типа визуалиации: 1) дикое поле, 2) оазис, 3) занятое поле игроком
        public void winDlg_InfoCell(int cx, int cy) {
            float FSize = ToCSR(10);//размер шрифта текста
            string FName = "Arial";//имя шрифта текста
            if (Form_InfoCell == null) { //создание контролов один раз
                //ОБЩИЕ ЭЛЕМЕНТЫ
                Form_InfoCell = Extensions.CreateForm("Form_InfoCell", new Icon("DATA_BASE/IMG/logotip.ico"),
                    new Font(FName, FSize, FontStyle.Bold), StartPosition: FormStartPosition.CenterParent,
                    FormBorderStyle: FormBorderStyle.Fixed3D, Padding: new Padding(0, 0, 10, 10),/*отступы от краёв*/
                    ControlBox: true, KeyPreview: true, AutoSize: true);
                Form_InfoCell.FormClosing += Form_InfoCell_Closing;
                Form_InfoCell.KeyUp += new KeyEventHandler(KeyUp_EscapeFormClose);
                HeadText_InfoCell = new Label {
                    Parent = Form_InfoCell, AutoSize = true, ForeColor = Color.Black,
                    Font = new Font(FName, FSize + 2, FontStyle.Bold),
                    Location = new Point(ToCSR(10), ToCSR(20)),
                };
                img_Field_InfoCell = new PictureBox { Parent = Form_InfoCell, BackgroundImageLayout = ImageLayout.Stretch,
                    Location = new Point(HeadText_InfoCell.Left, HeadText_InfoCell.Top + HeadText_InfoCell.Height + ToCSR(10)),
                    Size = new Size((int)ToCSR(380 * 1.463303F)/*W*/, ToCSR(380)/*H*/),
                };
                Panel_InfoCell = new Panel {
                    Parent = Form_InfoCell, Size = new Size(ToCSR(100)/*W*/, ToCSR(380)/*H*/),
                    Location = new Point(img_Field_InfoCell.Left + img_Field_InfoCell.Width + ToCSR(10), img_Field_InfoCell.Top),
                }; 
                Buttons_InfoCell = new Button[4];
                for (int i = 0; i < Buttons_InfoCell.Length; i++) {
                    Buttons_InfoCell[i] = new Button {
                        Parent = Form_InfoCell, Font = new Font(FName, FSize, FontStyle.Bold),
                        AutoSize = true, ForeColor = Color.FromArgb(50, 75, 0), BackColor = Form_InfoCell.BackColor,
                        Name = "Buttons_InfoCell_" + i, Padding = new Padding(10, 5, 10, 5),
                    };
                    Buttons_InfoCell[i].Button_Styles(Form_InfoCell.BackColor, Color.FromArgb(100, 150, 0),
                        Color.FromArgb(128, 200, 0), Color.FromArgb(50, 75, 0), Color.FromArgb(50, 75, 0), 1);
                    Buttons_InfoCell[i].Click += Buttons_winDlg_Click;
                }
                for (int i = 0; i < lb_Field.Length; i++) {
                    lb_Field[i] = new Label { Parent = Form_InfoCell, AutoSize = true, ForeColor = Color.Black,
                        Font = new Font(FName, FSize, FontStyle.Bold), Location = new Point(0, 0),
                    }; lb_Field[i].BringToFront();
                } lb_Field[2].Font = new Font(FName, FSize, FontStyle.Regular);
                //ЭЛЕМЕНТЫ ВКЛАДКИ ОАЗИС
                for (int i = 0; i < lb_Oasis_Text.Length; i++) {
                    lb_Oasis_Text[i] = new Label { Parent = Panel_InfoCell, AutoSize = true, ForeColor = Color.Black,
                        Font = new Font(FName, FSize, FontStyle.Regular), Location = new Point(0, 0),
                    };
                }
                for (int i = 0; i < pic2_Oasis.Length; i++) {
                        pic2_Oasis[i] = new PictureBox { Parent = Panel_InfoCell, BackgroundImageLayout = ImageLayout.Stretch,
                        Size = new Size(ToCSR(32), ToCSR(22))
                    };
                }
                //ЭЛЕМЕНТЫ ВКЛАДКИ ДИКОЕ ПОЛЕ
                bool b = true; FontStyle FS = FontStyle.Bold;
                for (int i = 0; i < lb_Field_Res.Length; i++) {
                    lb_Field_Res[i] = new Label { Parent = Panel_InfoCell, AutoSize = true, ForeColor = Color.Black,
                        Font = new Font(FName, FSize, FS),
                    }; if (b) { b = false; FS = FontStyle.Regular; } else { b = true; FS = FontStyle.Bold; }
                }
                for (int i = 0; i < pic5_Field.Length; i++) {
                        pic5_Field[i] = new PictureBox { Parent = Panel_InfoCell, BackgroundImageLayout = ImageLayout.Stretch,
                        Size = new Size(ToCSR(32), ToCSR(22)), Visible = false,
                    };
                    string str = "DATA_BASE/IMG/pictograms/resources/0" + i + ".png";
                    if (i == 4 ) str = "DATA_BASE/IMG/pictograms/resources/0" + (i + 1) + ".png";
                    pic5_Field[i].BackgroundImage = Image.FromFile(str);
                }
                //ЭЛЕМЕНТЫ ВКЛАДКИ "ДЕРЕВНЯ"
                b = true; FS = FontStyle.Bold;
                for (int i = 0; i < lb_Village.Length; i++) {
                    lb_Village[i] = new Label { Parent = Panel_InfoCell, Name = "lb_Village" + i,
                        AutoSize = true, ForeColor = Color.Black,
                        Font = new Font(FName, FSize, FS), Location = new Point(0, 0),
                    }; if (i >= 1) if (b) { b = false; FS = FontStyle.Regular; } else { b = true; FS = FontStyle.Bold; }
                }
                    lb_Village[0].Font = lb_Village[1].Font = lb_Village[3].Font = lb_Village[9].Font =
                        new Font(FName, FSize, FontStyle.Bold);
                    //метка-ссылка. клик ведёт в профиль игрока
                    lb_Village[7].ForeColor = Color.FromArgb(60, 130, 40);//зелёный
                    lb_Village[7].Cursor = Cursors.Hand;//рука при наведении-стрелка при выходе за границы контрола
                    lb_Village[7].Click += Control_Click; lb_Village[7].MouseEnter += Control_MouseEnter;
                    lb_Village[7].MouseLeave += Control_MouseLeave;
                //ЭЛЕМЕНТЫ ОТЧЁТА (СПРАВА ВНИЗУ)
                HeadText_Report = new Label {
                    Parent = Form_InfoCell, Text = LANGUAGES.RESOURSES[68],/*Отчёт:*/ AutoSize = true, ForeColor = Color.Black,
                    Font = new Font(FName, FSize + 2, FontStyle.Bold),
                };
                //таблица войск в оазисе. природа если оазис дикий, и подкрепы, если оазис под контролем
                grid_Troops = Extensions.CreateGrid(Panel_InfoCell, "grid_Troops", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_InfoCell.BackColor, GridColor: Form_InfoCell.BackColor,
                    BorderStyle: BorderStyle.FixedSingle, ReadOnly: true);
                //создание типов колонок
                grid_Troops.Columns.AddRange(new DataGridViewColumn[] {
                    new DataGridViewImageColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                    new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable }, 
                    new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                });
                var FC = new Font[grid_Troops.Columns.Count]; 
                var ASM = new DataGridViewAutoSizeColumnMode[grid_Troops.Columns.Count]; var DCSA_Cell = new DataGridViewContentAlignment[grid_Troops.Columns.Count];
                var Regular = new Font(FName, FSize, FontStyle.Regular); var Bold = new Font(FName, FSize, FontStyle.Bold);
                for (int i = 0; i < grid_Troops.Columns.Count; i++) {
                    ASM[i] = DataGridViewAutoSizeColumnMode.AllCells;
                    if (i == 0) DCSA_Cell[i] = DataGridViewContentAlignment.MiddleCenter;//pic
                    else if (i == 1) { FC[i] = Bold; DCSA_Cell[i] = DataGridViewContentAlignment.MiddleRight; }//count troops
                    else { FC[i] = Regular; DCSA_Cell[i] = DataGridViewContentAlignment.MiddleLeft;
                        ASM[i] = DataGridViewAutoSizeColumnMode.Fill; }//name troops
                }
                grid_Troops.Settings(Form_InfoCell.BackColor, FC, ASM, DCSA_Cell,
                    DefaultCellStyle_SelectionBackColor: Form_InfoCell.BackColor,
                    DefaultCellStyle_SelectionForeColor: grid_Troops.DefaultCellStyle.ForeColor);
            }

            Buttons_InfoCell[0].Tag = new Point(cx, cy);//прикрепляем координаты на кнопку центрирования карты
            lb_Village[7].Tag = GAME.Map.Cell[cx, cy].LinkAccount;//ссылка на аккаунт-владельца ячейкой
            lb_Field[0].Text = LANGUAGES.RESOURSES[60];/*Распределение:*/
            lb_Field[1].Text = LANGUAGES.RESOURSES[61];/*Другая информация:*/
            lb_Field[2].Text = LANGUAGES.RESOURSES[62];/*Дистанция:*/
            //4 зелёные кнопки под картинкой
            Buttons_InfoCell[0].Text = LANGUAGES.RESOURSES[56];/*Центрировать карту*/
            Buttons_InfoCell[1].Text = LANGUAGES.RESOURSES[57];/*Основать поселение*/
            Buttons_InfoCell[2].Text = LANGUAGES.RESOURSES[58];/*Отправить войска*/
            Buttons_InfoCell[3].Text = LANGUAGES.RESOURSES[59];/*Отправить торговцев*/
            int w = Buttons_InfoCell[0].Width; if (Buttons_InfoCell[1].Width > w) w = Buttons_InfoCell[1].Width; if (Buttons_InfoCell[2].Width > w) w = Buttons_InfoCell[2].Width; if (Buttons_InfoCell[3].Width > w) w = Buttons_InfoCell[3].Width;
            for (int i = 0; i < Buttons_InfoCell.Length; i++) Buttons_InfoCell[i].Width = w;
            Buttons_InfoCell[0].Location = new Point(img_Field_InfoCell.Left, img_Field_InfoCell.Top + img_Field_InfoCell.Height + ToCSR(10));
            Buttons_InfoCell[1].Location = new Point(Buttons_InfoCell[0].Left, Buttons_InfoCell[0].Top + Buttons_InfoCell[0].Height + ToCSR(5));
            Buttons_InfoCell[2].Location = new Point(Buttons_InfoCell[1].Left, Buttons_InfoCell[1].Top + Buttons_InfoCell[1].Height + ToCSR(5));
            Buttons_InfoCell[3].Location = new Point(Buttons_InfoCell[2].Left, Buttons_InfoCell[2].Top + Buttons_InfoCell[2].Height + ToCSR(5));
            //ЭЛЕМЕНТЫ ВКЛАДКИ ОАЗИС
            pic2_Oasis[0].Location = new Point(ToCSR(5), lb_Oasis_Text[0].Top + lb_Oasis_Text[0].Height + ToCSR(10));
            pic2_Oasis[1].Location = new Point(ToCSR(5), pic2_Oasis[0].Top + pic2_Oasis[0].Height + ToCSR(5));
            lb_Oasis_Text[1].Location = new Point(pic2_Oasis[0].Left + pic2_Oasis[0].Width + ToCSR(5), pic2_Oasis[0].Top);
            lb_Oasis_Text[2].Location = new Point(pic2_Oasis[1].Left + pic2_Oasis[1].Width + ToCSR(5), pic2_Oasis[1].Top);
            //ЭЛЕМЕНТЫ ВКЛАДКИ ДИКОЕ ПОЛЕ
            lb_Field_Res[1].Text = LANGUAGES.RESOURSES[63];/*Лесопилок*/
            lb_Field_Res[3].Text = LANGUAGES.RESOURSES[64];/*Глиняных карьеров*/
            lb_Field_Res[5].Text = LANGUAGES.RESOURSES[65];/*Железных рудников*/
            lb_Field_Res[7].Text = LANGUAGES.RESOURSES[66];/*Ферм*/
            //ЭЛЕМЕНТЫ ВКЛАДКИ "ДЕРЕВНЯ"
            lb_Village[0].Text = LANGUAGES.RESOURSES[60];/*Распределение:*/
            lb_Village[1].Text = LANGUAGES.RESOURSES[67];/*Информация об игроке:*/
            lb_Village[2].Text = LANGUAGES.RESOURSES[20];/*Народ:*/
            lb_Village[4].Text = LANGUAGES.RESOURSES[17];/*Альянс:*/
            lb_Village[6].Text = LANGUAGES.RESOURSES[15];/*Игрок:*/
            lb_Village[8].Text = LANGUAGES.RESOURSES[16];/*Население:*/
            //ЭЛЕМЕНТЫ ОТЧЁТА (СПРАВА ВНИЗУ)
            //дописать код потом по мере появления таблицы отчёта
            HeadText_Report.Location = new Point(Panel_InfoCell.Left + lb_Field[0].Left, Buttons_InfoCell[0].Top);

            //дистанция между двумя точками
            Point VillageLoc = new Point(Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian.X + GAME.Map.Width, Player.VillageList[Player.ActiveIndex].Coordinates_World_Travian.Y + GAME.Map.Height);
            double dist = GAME.Map.Dist(new Point(VillageLoc.X, VillageLoc.Y), new Point(cx, cy));

            lb_Field[3].Text = string.Format("{0:0.00}", dist) + " " + LANGUAGES.RESOURSES[69];/*поля*/
            HeadText_InfoCell.Text = GAME.Map.Cell[cx, cy].ID_ToText(LANGUAGES.RESOURSES) + " (" + (cx - GAME.Map.Width) + " | " + (cy - GAME.Map.Height) + ")";
            Cell_ID Cell_id = GAME.Map.Cell[cx, cy].ID; string path = "DATA_BASE/IMG/map/"; int MAX_W = 0;//ширина панели
            switch (GAME.Map.Cell[cx, cy].TypeResoueces) {
                //ОАЗИС
                //Если оазис принадлежит кому-то, то вешаем на эту ячейку ссылку на аккаунт и деревню
                case TypeCell._0_0_0_0:
                    Form_InfoCell.Text = LANGUAGES.RESOURSES[124];/*Информация об оазисе*/
                    lb_Field_Res[0].Text = "0"; lb_Field_Res[2].Text = "0"; lb_Field_Res[4].Text = "0"; lb_Field_Res[6].Text = "0";
                    /*ОЗЕРО*/if (Cell_id == Cell_ID.Water) { img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "Fields/Water.png"); break; }
                    /*ГОРЫ*/if (Cell_id == Cell_ID.Mountains) { img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "Fields/Mountains.png"); break; }
                    /*ЛЕС*/if (Cell_id == Cell_ID.Forest) { img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "Fields/Forest.png"); break; }
                    //скрываем всё что не относится к оазису
                    lb_Field[0].Visible = false;
                    for (int i = 0; i < lb_Field_Res.Length; i++) lb_Field_Res[i].Visible = false;
                    for (int i = 0; i < pic5_Field.Length; i++) pic5_Field[i].Visible = false;

                    int count = 0;//1 = один бонус, 2 = два бонуса
                    lb_Oasis_Text[0].Visible = lb_Oasis_Text[3].Visible = true;
                    lb_Oasis_Text[0].Text = LANGUAGES.RESOURSES[70];/*Бонус:*/
                    switch (Cell_id) { 
                        case Cell_ID.Oasis_wood25:
                            pic2_Oasis[0].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/00.png");
                            lb_Oasis_Text[1].Text = "25% " + LANGUAGES.RESOURSES[71];/*древесина*/ count = 1;
                            img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "oasis/wood1.png"); break;
                        case Cell_ID.Oasis_wood50:
                            pic2_Oasis[0].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/00.png");
                            lb_Oasis_Text[1].Text = "50% " + LANGUAGES.RESOURSES[71];/*древесина*/ count = 1;
                            img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "oasis/wood2.png"); break;
                        case Cell_ID.Oasis_wood25_crop25:
                            pic2_Oasis[0].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/00.png");
                            pic2_Oasis[1].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/03.png");
                            lb_Oasis_Text[1].Text = "25% " + LANGUAGES.RESOURSES[71];/*древесина*/
                            lb_Oasis_Text[2].Text = "25% " + LANGUAGES.RESOURSES[72];/*зерно*/ count = 2;
                            img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "oasis/wood_crop.png"); break;
                        case Cell_ID.Oasis_clay25:
                            pic2_Oasis[0].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/01.png");
                            lb_Oasis_Text[1].Text = "25% " + LANGUAGES.RESOURSES[73];/*глина*/ count = 1;
                            img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "oasis/clay.png"); break;
                        case Cell_ID.Oasis_clay50:
                            pic2_Oasis[0].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/01.png");
                            lb_Oasis_Text[1].Text = "50% " + LANGUAGES.RESOURSES[73];/*глина*/ count = 1;
                            img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "oasis/clay.png"); break;
                        case Cell_ID.Oasis_clay25_crop25:
                            pic2_Oasis[0].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/01.png");
                            pic2_Oasis[1].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/03.png");
                            lb_Oasis_Text[1].Text = "25% " + LANGUAGES.RESOURSES[73];/*глина*/
                            lb_Oasis_Text[2].Text = "25% " + LANGUAGES.RESOURSES[72];/*зерно*/ count = 2;
                            img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "oasis/clay_crop.png"); break;
                        case Cell_ID.Oasis_iron25:
                            pic2_Oasis[0].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/02.png");
                            lb_Oasis_Text[1].Text = "25% " + LANGUAGES.RESOURSES[74];/*железо*/ count = 1;
                            img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "oasis/iron.png"); break;
                        case Cell_ID.Oasis_iron50:
                            pic2_Oasis[0].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/02.png");
                            lb_Oasis_Text[1].Text = "50% " + LANGUAGES.RESOURSES[74];/*железо*/ count = 1;
                            img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "oasis/iron.png"); break;
                        case Cell_ID.Oasis_iron25_crop25:
                            pic2_Oasis[0].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/02.png");
                            pic2_Oasis[1].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/03.png");
                            lb_Oasis_Text[1].Text = "25% " + LANGUAGES.RESOURSES[74];/*железо*/
                            lb_Oasis_Text[2].Text = "25% " + LANGUAGES.RESOURSES[72];/*зерно*/ count = 2;
                            img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "oasis/iron_crop.png"); break;
                        case Cell_ID.Oasis_crop25:
                            pic2_Oasis[0].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/03.png");
                            lb_Oasis_Text[1].Text = "25% " + LANGUAGES.RESOURSES[72];/*зерно*/ count = 1;
                            img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "oasis/crop.png"); break;
                        case Cell_ID.Oasis_crop50:
                            pic2_Oasis[0].BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/resources/03.png");
                            lb_Oasis_Text[1].Text = "50% " + LANGUAGES.RESOURSES[72];/*зерно*/ count = 1;
                            img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "oasis/crop.png"); break;
                    }
                    //количество отображаемых бонусов
                    if (count == 2) { //2 бонуса
                        pic2_Oasis[0].Visible = true; pic2_Oasis[1].Visible = true;//пиктограммы ресурсов
                        lb_Oasis_Text[1].Visible = true; lb_Oasis_Text[2].Visible = true;//текст для пиктограмм
                    } else { //1 бонус
                        pic2_Oasis[0].Visible = true; pic2_Oasis[1].Visible = false;//пиктограммы ресурсов
                        lb_Oasis_Text[1].Visible = true; lb_Oasis_Text[2].Visible = false;//текст для пиктограмм
                    }

                    lb_Oasis_Text[3].Text = LANGUAGES.RESOURSES[54];/*Войска:*/
                    grid_Troops.Rows.Clear(); grid_Troops.Visible = true;
                    grid_Troops.Columns[1].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Bold);
                    for (int i = 0; i < lb_Village.Length; i++) lb_Village[i].Visible = false;//скрываем метки
                    //ЕСТЬ АРМИЯ - отображаем войска (дикий оазис или оазис принадлежащий игроку)
                    for (int i = 0; i < GAME.Map.Cell[cx, cy].AllTroops.Length; i++) {
                        if (GAME.Map.Cell[cx, cy].AllTroops[i] > 0) { 
                            string PATH = "DATA_BASE/IMG/pictograms/unit/"; string PATH2 = "Rome/"; string extension = ".gif";
                            if (i / 10 == 0) PATH2 = "Rome/"; else if (i / 10 == 1) PATH2 = "German/"; else
                            if (i / 10 == 2) PATH2 = "Gaul/"; else if (i / 10 == 3) PATH2 = "Nature/"; else
                            if (i / 10 == 4) PATH2 = "Natar/"; //if (path2 == "Natar/") extension = ".png";
                            //добавляем строки войск
                            grid_Troops.Rows.Add(Image.FromFile(PATH + PATH2 + (i + 1) + extension),/*pic*/
                                GAME.Map.Cell[cx, cy].AllTroops[i]/*value*/, GAME.Troops[i / 10].Information[i % 10].Name);
                        } 
                    }
                    //НЕТ АРМИИ
                    if (grid_Troops.Rows.Count <= 0) {
                        grid_Troops.Columns[1].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Regular);
                        grid_Troops.Rows.Add(new Bitmap(1, 1), LANGUAGES.RESOURSES[55]/*Нет.*/ + ".", "");
                    }
                    
                    //занятый оазис - выводим информацию о владельце
                    if (GAME.Map.Cell[cx, cy].LinkAccount != null) { 
                        for (int i = 1; i < lb_Village.Length; i++) lb_Village[i].Visible = true;//показываем метки
                        lb_Village[3].Text = LANGUAGES.RESOURSES[21 + (int)GAME.Map.Cell[cx, cy].LinkAccount.Folk_Name].ToUpper();
                        if (GAME.Map.Cell[cx, cy].LinkAccount.Alliance_Name != "")
                            lb_Village[5].Text = GAME.Map.Cell[cx, cy].LinkAccount.Alliance_Name; else lb_Village[5].Text = "-";
                        lb_Village[7].Text = GAME.Map.Cell[cx, cy].LinkAccount.Nick_Name;
                        lb_Village[9].Text = UFO.Convert.toTABString(GAME.Map.Cell[cx, cy].LinkVillage.Population.ToString(), "'");
                    }
                    //подгон габаритов Grid 
                    grid_Troops.Height = 10 * grid_Troops.Rows[0].Height;
                    //локация таблицы
                    if (GAME.Map.Cell[cx, cy].LinkVillage == null) { //дикий оазис
                        grid_Troops.Visible = true; lb_Oasis_Text[3].Visible = true;
                        lb_Oasis_Text[3].Location = new Point(0, pic2_Oasis[1].Top + pic2_Oasis[1].Height + UFO.Convert.ToCSR(10));
                        grid_Troops.Location = new Point(lb_Oasis_Text[3].Left, lb_Oasis_Text[3].Top + lb_Oasis_Text[3].Height + UFO.Convert.ToCSR(5));
                    } else if (GAME.Map.Cell[cx, cy].LinkAccount != null &&
                        GAME.Map.Cell[cx, cy].LinkAccount.Nick_Name == Player.Nick_Name) { //оазис игрока
                        grid_Troops.Visible = true; lb_Oasis_Text[3].Visible = true;
                        grid_Troops.Height = 5 * grid_Troops.Rows[0].Height;
                        grid_Troops.Location = new Point(lb_Village[1].Left, img_Field_InfoCell.Height - grid_Troops.Height + UFO.Convert.ToCSR(10));
                        lb_Oasis_Text[3].Location = new Point(lb_Village[1].Left, grid_Troops.Top - lb_Oasis_Text[3].Height - UFO.Convert.ToCSR(5));
                    }
                    else { grid_Troops.Visible = false; lb_Oasis_Text[3].Visible = false; }//оазис бота

                    //авто подгон ширины панели
                    for (int i = 0; i < lb_Oasis_Text.Length; i++) if (lb_Oasis_Text[i].Left + lb_Oasis_Text[i].Width > MAX_W) MAX_W = lb_Oasis_Text[i].Left + lb_Oasis_Text[i].Width;
                    for (int i = 1; i < lb_Field.Length; i++) if (lb_Field[i].Left + lb_Field[i].Width > MAX_W) MAX_W = lb_Field[i].Left + lb_Field[i].Width;
                    if (grid_Troops.Left + grid_Troops.Width > MAX_W) MAX_W = grid_Troops.Left + grid_Troops.Width;
                break;
                case TypeCell._4_4_4_6: lb_Field_Res[0].Text = "4"; lb_Field_Res[2].Text = "4"; lb_Field_Res[4].Text = "4"; lb_Field_Res[6].Text = "6"; break;
                case TypeCell._3_4_5_6: lb_Field_Res[0].Text = "3"; lb_Field_Res[2].Text = "4"; lb_Field_Res[4].Text = "5"; lb_Field_Res[6].Text = "6"; break;
                case TypeCell._3_5_4_6: lb_Field_Res[0].Text = "3"; lb_Field_Res[2].Text = "5"; lb_Field_Res[4].Text = "4"; lb_Field_Res[6].Text = "6"; break;
                case TypeCell._5_4_3_6: lb_Field_Res[0].Text = "5"; lb_Field_Res[2].Text = "4"; lb_Field_Res[4].Text = "3"; lb_Field_Res[6].Text = "6"; break;
                case TypeCell._4_3_5_6: lb_Field_Res[0].Text = "4"; lb_Field_Res[2].Text = "3"; lb_Field_Res[4].Text = "5"; lb_Field_Res[6].Text = "6"; break;
                case TypeCell._3_4_4_7: lb_Field_Res[0].Text = "3"; lb_Field_Res[2].Text = "4"; lb_Field_Res[4].Text = "4"; lb_Field_Res[6].Text = "7"; break;
                case TypeCell._4_3_4_7: lb_Field_Res[0].Text = "4"; lb_Field_Res[2].Text = "3"; lb_Field_Res[4].Text = "4"; lb_Field_Res[6].Text = "7"; break;
                case TypeCell._4_4_3_7: lb_Field_Res[0].Text = "4"; lb_Field_Res[2].Text = "4"; lb_Field_Res[4].Text = "3"; lb_Field_Res[6].Text = "7"; break;
                case TypeCell._3_3_3_9: lb_Field_Res[0].Text = "3"; lb_Field_Res[2].Text = "3"; lb_Field_Res[4].Text = "3"; lb_Field_Res[6].Text = "9"; break;
                case TypeCell._1_1_1_15: lb_Field_Res[0].Text = "1"; lb_Field_Res[2].Text = "1"; lb_Field_Res[4].Text = "1"; lb_Field_Res[6].Text = "15"; break;
                default: break;
            }
            if (GAME.Map.Cell[cx, cy].TypeResoueces != TypeCell._0_0_0_0)
                img_Field_InfoCell.BackgroundImage = Image.FromFile(path + "Fields/" + GAME.Map.Cell[cx, cy].TypeResoueces + ".png");
            //это не оазис
            if (GAME.Map.Cell[cx, cy].TypeResoueces != TypeCell._0_0_0_0 || //дикое поле
                Cell_id == Cell_ID.Water || Cell_id == Cell_ID.Mountains || Cell_id == Cell_ID.Forest) {
                for (int i = 0; i < lb_Oasis_Text.Length; i++) lb_Oasis_Text[i].Visible = false;
                for (int i = 0; i < pic2_Oasis.Length; i++) pic2_Oasis[i].Visible = false;
                grid_Troops.Visible = false;

                for (int i = 0; i < pic5_Field.Length - 1; i++) pic5_Field[i].Visible = true;
                //ячейка свободна (для дикого поля)
                if (GAME.Map.Cell[cx, cy].LinkAccount == null) {
                    Form_InfoCell.Text = LANGUAGES.RESOURSES[125];/*Информация о диком поле*/
                    lb_Field[0].Visible = true;
                    for (int i = 0; i < lb_Village.Length; i++) lb_Village[i].Visible = false;
                    for (int i = 0; i < lb_Field_Res.Length; i++) lb_Field_Res[i].Visible = true;
                    lb_Field[0].Top = img_Field_InfoCell.Top; lb_Field[0].Left = img_Field_InfoCell.Left + img_Field_InfoCell.Width + UFO.Convert.ToCSR(5);
                    pic5_Field[0].Location = new Point(0, lb_Field[0].Height + ToCSR(15));//wood
                    pic5_Field[1].Location = new Point(0, pic5_Field[0].Top + pic5_Field[0].Height + ToCSR(15));//clay
                    pic5_Field[2].Location = new Point(0, pic5_Field[1].Top + pic5_Field[1].Height + ToCSR(15));//iron
                    pic5_Field[3].Location = new Point(0, pic5_Field[2].Top + pic5_Field[2].Height + ToCSR(15));//iron
                    lb_Field_Res[0].Location = new Point(pic5_Field[0].Left + pic5_Field[0].Width + ToCSR(15), pic5_Field[0].Top);
                    lb_Field_Res[1].Location = new Point(lb_Field_Res[0].Left + lb_Field_Res[0].Width + ToCSR(5), pic5_Field[0].Top);
                    lb_Field_Res[2].Location = new Point(pic5_Field[1].Left + pic5_Field[1].Width + ToCSR(15), pic5_Field[1].Top);
                    lb_Field_Res[3].Location = new Point(lb_Field_Res[2].Left + lb_Field_Res[2].Width + ToCSR(5), pic5_Field[1].Top);
                    lb_Field_Res[4].Location = new Point(pic5_Field[2].Left + pic5_Field[2].Width + ToCSR(15), pic5_Field[2].Top);
                    lb_Field_Res[5].Location = new Point(lb_Field_Res[4].Left + lb_Field_Res[4].Width + ToCSR(5), pic5_Field[2].Top);
                    lb_Field_Res[6].Location = new Point(pic5_Field[3].Left + pic5_Field[3].Width + ToCSR(15), pic5_Field[3].Top);
                    lb_Field_Res[7].Location = new Point(lb_Field_Res[6].Left + lb_Field_Res[6].Width + ToCSR(5), pic5_Field[3].Top);
                    //авто подгон ширины панели
                    for (int i = 0; i < lb_Field.Length; i++) if (lb_Field[i].Left + lb_Field[i].Width > MAX_W) MAX_W = lb_Field[i].Left + lb_Field[i].Width;
                    for (int i = 0; i < lb_Field_Res.Length; i++) if (lb_Field_Res[i].Left + lb_Field_Res[i].Width > MAX_W) MAX_W = lb_Field_Res[i].Left + lb_Field_Res[i].Width;
                } else {
                    //ячейка принадлежит игроку Player или ИИ / AI / ботам 
                    Form_InfoCell.Text = LANGUAGES.RESOURSES[126];/*Информация о деревне*/
                    HeadText_InfoCell.Text = LANGUAGES.RESOURSES[18]/*Деревня*/ + " " + GAME.Map.Cell[cx, cy].LinkVillage.Village_Name
                                             + " (" + (cx - GAME.Map.Width) + " | " + (cy - GAME.Map.Height) + ")";
                    lb_Village[3].Text = LANGUAGES.RESOURSES[21 + (int)GAME.Map.Cell[cx, cy].LinkAccount.Folk_Name].ToUpper();
                    if (GAME.Map.Cell[cx, cy].LinkAccount.Alliance_Name != "")
                        lb_Village[5].Text = GAME.Map.Cell[cx, cy].LinkAccount.Alliance_Name; else lb_Village[5].Text = "-";
                    lb_Village[7].Text = GAME.Map.Cell[cx, cy].LinkAccount.Nick_Name;
                    lb_Village[9].Text = UFO.Convert.toTABString(GAME.Map.Cell[cx, cy].LinkVillage.Population.ToString(), "'");
                    for (int i = 0; i < lb_Village.Length; i++) lb_Village[i].Visible = true;
                    for (int i = 0; i < lb_Field_Res.Length; i++) lb_Field_Res[i].Visible = false;
                    lb_Field[0].Visible = false;
                    lb_Field_Res[0].Visible = lb_Field_Res[2].Visible = lb_Field_Res[4].Visible = lb_Field_Res[6].Visible = true;
                    pic5_Field[0].Location = new Point(0, lb_Village[0].Top + lb_Village[0].Height + ToCSR(15));
                    lb_Field_Res[0].Location = new Point(pic5_Field[0].Left + pic5_Field[0].Width + ToCSR(5), pic5_Field[0].Top);
                    pic5_Field[1].Location = new Point(lb_Field_Res[0].Left + lb_Field_Res[0].Width + ToCSR(15), pic5_Field[0].Top);
                    lb_Field_Res[2].Location = new Point(pic5_Field[1].Left + pic5_Field[1].Width + ToCSR(5), pic5_Field[0].Top);
                    pic5_Field[2].Location = new Point(lb_Field_Res[2].Left + lb_Field_Res[2].Width + ToCSR(15), pic5_Field[0].Top);
                    lb_Field_Res[4].Location = new Point(pic5_Field[2].Left + pic5_Field[2].Width + ToCSR(5), pic5_Field[0].Top);
                    pic5_Field[3].Location = new Point(lb_Field_Res[4].Left + lb_Field_Res[4].Width + ToCSR(15), pic5_Field[0].Top);
                    lb_Field_Res[6].Location = new Point(pic5_Field[3].Left + pic5_Field[3].Width, pic5_Field[0].Top);
                }
            }

            if (GAME.Map.Cell[cx, cy].LinkVillage == null)
                lb_Village[1].Location = new Point(0, pic5_Field[0].Top + pic5_Field[0].Height + ToCSR(40));
            else lb_Village[1].Location = new Point(0, pic2_Oasis[1].Top + pic2_Oasis[1].Height + ToCSR(10));
            lb_Village[2].Location = new Point(0, lb_Village[1].Top + lb_Village[1].Height + ToCSR(10));
            lb_Village[3].Location = new Point(ToCSR(120), lb_Village[2].Top);
            lb_Village[4].Location = new Point(0, lb_Village[2].Top + lb_Village[2].Height + ToCSR(10));
            lb_Village[5].Location = new Point(ToCSR(120), lb_Village[4].Top);
            lb_Village[6].Location = new Point(0, lb_Village[4].Top + lb_Village[4].Height + ToCSR(10));
            lb_Village[7].Location = new Point(ToCSR(120), lb_Village[6].Top);
            lb_Village[8].Location = new Point(0, lb_Village[6].Top + lb_Village[6].Height + ToCSR(10));
            lb_Village[9].Location = new Point(ToCSR(120), lb_Village[8].Top);

            Panel_InfoCell.AutoSize = true;
            /*Другая информация:*/lb_Field[1].Location = new Point(Panel_InfoCell.Left, Panel_InfoCell.Top + Panel_InfoCell.Height + ToCSR(5));
            /*Дистанция:*/lb_Field[2].Location = new Point(Panel_InfoCell.Left, lb_Field[1].Top + lb_Field[1].Height + ToCSR(5));
            /*dist value*/lb_Field[3].Location = new Point(lb_Field[2].Left + lb_Field[2].Width + ToCSR(20), lb_Field[2].Top);
            //отчёт
            HeadText_Report.Location = new Point(Panel_InfoCell.Left, lb_Field[3].Top + lb_Field[3].Height + ToCSR(20));

            if (!Form_InfoCell.Visible) Form_InfoCell.ShowDialog();
        }
        //======================================== ДИАЛОГОВОЕ ОКНО :: ИНФОРМАЦИЯ О ЯЧЕЙКЕ ========================================

        //============================== ДИАЛОГОВОЕ ОКНО :: ПЕРЕИМЕНОВАНИЕ ДЕРЕВНИ В СПИСКЕ ДЕРЕВЕНЬ =============================
        private Form FormRename = null; private TextBox tb_Rename = null; private Button btn_Rename = null; private Label lb_Rename = null;
        public void btn_Rename_Village_Click(object sender, EventArgs e) {
            if (FormRename == null) { 
                FormRename = Extensions.CreateForm("FormRename", new Icon("DATA_BASE/IMG/logotip.ico"),
                    new Font(Font.FontFamily, ToCSR(10), FontStyle.Bold), 
                    Text: LANGUAGES.RESOURSES[75],/*Переименовать выделенную деревню.*/
                    StartPosition: FormStartPosition.CenterParent, FormBorderStyle: FormBorderStyle.Fixed3D, 
                    Padding: new Padding(0, 0, 10, 20), ControlBox: true, KeyPreview: true, AutoSize: true);
                FormRename.KeyUp += new KeyEventHandler(KeyUp_EscapeFormClose);
            }
            if (lb_Rename == null) { lb_Rename = new Label { 
                    Parent = FormRename, Left = 10, Top = 20, AutoSize = true,
                    Text = LANGUAGES.RESOURSES[76]/*Максмум разрешённых символов:*/ + " " + GAME.Name_MaxLength + LANGUAGES.RESOURSES[95],/*шт.*/
               }; lb_Rename.SizeFont(10F, FontStyle.Bold);
            } 
            if (tb_Rename == null) { tb_Rename = new TextBox { Parent = FormRename, 
                    Width = 320, Left = 10, Top = lb_Rename.Top + lb_Rename.Height + 5, MaxLength = GAME.Name_MaxLength,
               }; tb_Rename.SizeFont(10F, FontStyle.Bold);
                tb_Rename.TextChanged += Tb_Rename_TextChanged;
            } 
            if (btn_Rename == null) { btn_Rename = new Button { 
                    Parent = FormRename, //Font = new Font(Font.FontFamily, UFO.Convert.ToCSR(10), FontStyle.Bold),
                    Width = 130, Height = tb_Rename.Height + 8,
                    Left = tb_Rename.Left + tb_Rename.Width + 10, Top = tb_Rename.Top - 4,
                    Text = LANGUAGES.RESOURSES[77]/*Применить*/,
               }; btn_Rename.SizeFont(10F, FontStyle.Bold);
                btn_Rename.Click += Btn_Rename_Click;
            }
            tb_Rename.Text = Player.VillageList[Player.ActiveIndex].Village_Name;

            tb_Rename.Select();/*автовыделение текста у TextBox при открытии окна*/
                //if (!tb_Rename.Focused) tb_Rename.SelectAll();/*альтернативное автовыделение текста у TextBox при открытии окна*/
                btn_Rename.Enabled = false;
            tb_Rename.BringToFront(); lb_Rename.BringToFront(); btn_Rename.BringToFront(); FormRename.ShowDialog();
        }

        public void Btn_Rename_Click(object sender, EventArgs e) {
            for (int i = 0; i < Player.VillageList.Count; i++) if (tb_Rename.Text == Player.VillageList[i].Village_Name) {
                MessageBox.Show(LANGUAGES.RESOURSES[78]/*Ошибка. Название:*/ + " '" + tb_Rename.Text +
                    LANGUAGES.RESOURSES[79]/*не уникально.*/ + "'\n" + LANGUAGES.RESOURSES[80]);/*Деревня с таким именем уже существует в списке деревень.*/
                btn_Rename.Enabled = false; return;
            }
            if (Player.Hero.Registration_Village == Player.VillageList[Player.ActiveIndex].Village_Name)
                Player.Hero.Registration_Village = tb_Rename.Text;//перепрописываем героя в этой деревне с новым названием
            Player.VillageList[Player.ActiveIndex].Village_Name = tb_Rename.Text;//переименовываем в VillageList
            ListBox1.Items[ListBox1.SelectedIndex] = tb_Rename.Text;//переименовываем в списке ListBox1
            Update_Panel_VillageList();//ещё раз обновляем панель списка деревень
            Update_Panel_VillageInfo();
            //Выделяем переименованную строку в ListBox
            for (int i = 0; i < ListBox1.Items.Count; i++) if (tb_Rename.Text == ListBox1.Items[i].ToString())
                { ListBox1.SelectedIndex = ListBox2.SelectedIndex = i; break; }
            TabControl_SelectedIndexChanged();
            FormRename.Close();
        }

        public void Tb_Rename_TextChanged(object sender, EventArgs e) {
            if (tb_Rename.Text == "") { btn_Rename.Enabled = false; return; } else btn_Rename.Enabled = true;
            bool flag = false; string New = "", t = tb_Rename.Text;
            for (int i = 0; i < t.Length; i++) //копируем только разрешённые символы
                if (t[i] != '/' && t[i] != '\\' && t[i] != ':' && t[i] != '*' && t[i] != '?' && t[i] != '"' && t[i] != '<'
                    && t[i] != '>' && t[i] != '[' && t[i] != ']' && t[i] != '.') New += t[i]; else flag = true;
            if (flag) { SystemSounds.Beep.Play(); tb_Rename.Text = New; tb_Rename.SelectionStart = tb_Rename.Text.Length; } 
        }
        //============================== ДИАЛОГОВОЕ ОКНО :: ПЕРЕИМЕНОВАНИЕ ДЕРЕВНИ В СПИСКЕ ДЕРЕВЕНЬ =============================

        //=========================================== ДИАЛОГОВОЕ ОКНО :: ПРОФИЛЬ ИГРОКА ==========================================
        //_______________________________________________________________________________________________________
        private void btn_ShowHide_Click(object sender, EventArgs e) { btn_ShowHide_Animate((Button)sender); }
        /// <summary> Метод запускает анимацию кнопок "показать/скрыть" информацию. </summary>
        private async void btn_ShowHide_Animate(Button Ctrl) {
            Ctrl.Visible = false; string txt = Ctrl.Text; int N, C0L1, C0R1, C0L2, C0R2, C1L1, C1R1, C1L2, C1R2;
            int value; if (Ctrl.Name == "0") value = 400; else value = 200; int div = 16; int shift = value / div;
            btn_ShowHide_IncDec(Ctrl, out C0L1, out C0R1, out C0L2, out C0R2, out C1L1, out C1R1, out C1L2, out C1R2);
            if (txt == "...") { txt = "+"; N = -1; } else { txt = "..."; N = 1; } Ctrl.Text = txt;
            if (txt == "+")
                while (C0L1 > C0R1 || C1L1 > C1R1) { btn_ShowHide_Animate_Step(Ctrl, shift * N);
                    btn_ShowHide_IncDec(Ctrl, out C0L1, out C0R1, out C0L2, out C0R2, out C1L1, out C1R1, out C1L2, out C1R2);
                    await Task.Delay(GAME.SpeedAnimateProfileWindow);
                    value -= shift; div += 2; shift = value / div; if (shift < 1) shift = 1; } 
                else while (C0L2 < C0R2 || C1L2 < C1R2) { btn_ShowHide_Animate_Step(Ctrl, shift * N);
                    btn_ShowHide_IncDec(Ctrl, out C0L1, out C0R1, out C0L2, out C0R2, out C1L1, out C1R1, out C1L2, out C1R2);
                    await Task.Delay(GAME.SpeedAnimateProfileWindow);
                    value -= shift; div += 2; shift = value / div; if (shift < 1) shift = 1; }
            //подгон габаритов Grid и обрезка высоты таблицы
            var _ = TableVillages; _.UpdateSize(); if (_.Top + _.Height > ToCSR(800)) _.Height = ToCSR(800 - _.Top);
            if (_.Width < Panel_Header[2].Width) _.Width = Panel_Header[2].Width;
            Ctrl.Visible = true; //await System.Threading.Tasks.Task.Run(() => { });
        }
        private void btn_ShowHide_Animate_Step(Button Ctrl, int shift) {
            if (Ctrl.Name == "0") { TableInfo.Top += shift; pb_Avatar.Top += shift; btn_WriteMessage.Top += shift; Panel_Header[1].Top += shift; }
            Panel_Header[2].Top += shift; TableVillages.Top += shift;
            if (TableVillages.DisplayedRowCount(false) < TableVillages.Rows.Count ||
                TableVillages.DisplayedRowCount(false) > GAME.MaxVillageTabelRowsVisibleProfileWindow)
                TableVillages.Height += -shift;
        }
        private void btn_ShowHide_IncDec(Button Ctrl, out int C0L1, out int C0R1, out int C0L2, out int C0R2,
                                         out int C1L1, out int C1R1, out int C1L2, out int C1R2) {
            if (Ctrl.Name == "0") { C1L1 = C1L2 = 0; C1R1 = C1R2 = 0;
                C0L1 = Panel_Header[1].Top; C0R1 = Panel_Header[0].Bottom + ToCSR(5);
                C0L2 = pb_Avatar.Top;       C0R2 = Panel_Header[0].Bottom + ToCSR(5);
            } else { C0L1 = C0L2 = 0; C0R1 = C0R2 = 0;
                C1L1 = Panel_Header[2].Top; C1R1 = Panel_Header[1].Bottom + ToCSR(5);
                C1L2 = Panel_Header[2].Top; C1R2 = Panel_Header[1].Bottom + ToCSR(100);
            }
        }
        //```````````````````````````````````````````````````````````````````````````````````````````````````````

        /// <summary> Метод обрабатывает клик по ячейкам таблицы <b>TableVillages.</b> </summary>
        private void grid_TableVillages_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex <= -1 || e.ColumnIndex <= -1) return;//был клик по заголовку
            var Village = (TVillage)TableVillages.Rows[e.RowIndex].Cells[0].Tag;
            //обработка клика по деревням
            if (e.RowIndex >= 0) if (e.ColumnIndex == 0) {  
                if (Form_InfoCell != null && Form_InfoCell.Visible) Form_InfoCell.Close();
                winDlg_InfoCell(Village.Coordinates_Cell.X, Village.Coordinates_Cell.Y);//открыть окно "о ячейке"
            }//обработка клика по координатам
            else if (e.ColumnIndex == 5) {
                Form_AccountProfile.Close(); if (Form_InfoCell != null && Form_InfoCell.Visible) Form_InfoCell.Close();
                GAME.Map.Location = Village.Coordinates_World_Travian;
                tabControl.SelectedIndex = 3;/*к карте*/ _Update();
            }//обработка клика по оазису (если он есть)
            else if (e.ColumnIndex >= 1 && e.ColumnIndex <= 3) if (e.ColumnIndex - 1 < Village.OasisList.Count) {
                Form_AccountProfile.Close(); if (Form_InfoCell != null && Form_InfoCell.Visible) Form_InfoCell.Close();
                GAME.Map.Location = GAME.Map.Coord_MapToWorld(Village.OasisList[e.ColumnIndex - 1]);
                tabControl.SelectedIndex = 3;/*к карте*/ _Update();
            }
            //подсвечиваем строку с деревней в которой находится игрок
            TableVillages.Rows[Player.ActiveIndex].Cells[0].Selected = true;
        }
        /// <summary> Метод обрабатывает наведение на ячейку <b>grid_Statistics.</b> </summary>
        private void grid_TableVillages_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 0 || e.ColumnIndex == 5) if (e.RowIndex >= 0) {
                TableVillages.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
            }
        }
        /// <summary> Метод обрабатывает выход за пределы ячейки <b>grid_Statistics.</b> </summary>
        private void grid_TableVillages_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 0 || e.ColumnIndex == 5) if (e.RowIndex >= 0) {
                TableVillages.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Green;
            }
        }

        /// <summary> Окно профиля аккаунта. </summary>
        private Form Form_AccountProfile = null;
        /// <summary> Таблица общей информации об аккаунтах. </summary>
        private DataGridView TableInfo = null;
        /// <summary> Таблица деревень аккаунта. </summary>
        private DataGridView TableVillages = null;
        /// <summary> Массив панелей-плашек (3 шт.) </summary>
        private Panel[] Panel_Header = null;
        /// <summary> Массив заголовков для панелей (3 шт.) </summary>
        private Label[] lb_Header = null;
        /// <summary> Аватарка нации аккаунта. </summary>
        private PictureBox pb_Avatar = null;
        /// <summary> Кнопка "Написать сообщение". </summary>
        private Button btn_WriteMessage = null;
        /// <summary> Массив кнопок "показать/скрыть" </summary>
        private CustomControls.RoundAPIButton[] btn_ShowHide = null;
        public void WinDlg_AccountProfile(TPlayer account) {
            float FSize = ToCSR(10);//размер шрифта текста
            string FName = "Arial";//имя шрифта текста
            if (Form_AccountProfile == null) {
                Form_AccountProfile = Extensions.CreateForm("Form_AccountProfile", new Icon("DATA_BASE/IMG/logotip.ico"),
                    new Font(Font.FontFamily, FSize, FontStyle.Bold),
                    StartPosition: FormStartPosition.CenterParent, FormBorderStyle: FormBorderStyle.Fixed3D,
                    Padding: new Padding(0, 0, 20, 10), ControlBox: true, KeyPreview: true, AutoSize: true);
                Form_AccountProfile.MaximumSize = new Size(ScreenBounds_Size().Width, (int)(ScreenBounds_Size().Height * 0.925));
                Form_AccountProfile.StartPosition = FormStartPosition.CenterScreen;
                Form_AccountProfile.KeyUp += new KeyEventHandler(KeyUp_EscapeFormClose);
                Panel_Header = new Panel[3]; lb_Header = new Label[3];
                for (int i = 0; i < Panel_Header.Length; i++) {
                    Panel_Header[i] = new Panel { Parent = Form_AccountProfile, BackColor = Color.FromArgb(220, 230, 220), }; 
                    lb_Header[i] = new Label { Parent = Panel_Header[i], AutoSize = true, ForeColor = Color.Black,
                        Font = new Font(FName, FSize + 2, FontStyle.Bold),
                    }; 
                }
                pb_Avatar = new PictureBox { Parent = Form_AccountProfile, BackgroundImageLayout = ImageLayout.Stretch, };
                btn_ShowHide = new CustomControls.RoundAPIButton[2];
                for (int i = 0; i < btn_ShowHide.Length; i++) {
                    btn_ShowHide[i] = new CustomControls.RoundAPIButton {
                        Parent = Panel_Header[i], Name = i.ToString(), Text = "...",
                        BackColor = SystemColors.Highlight, ForeColor = Color.White, AutoSize = true, 
                        Font = new Font(FName, FSize + 2, FontStyle.Bold), Ellipse_Percent = 10, 
                    };
                    btn_ShowHide[i].Click += btn_ShowHide_Click; 
                }
                btn_WriteMessage = new Button { Parent = Form_AccountProfile, Name = "btn_WriteMessage", Font = new Font(FName, FSize, FontStyle.Bold),
                    BackColor = pb_Avatar.BackColor, ForeColor = Color.FromArgb(100, 150, 0), Padding = new Padding(5, 0, 5, 0), AutoSize = true,
                    ImageAlign = ContentAlignment.TopLeft, TextAlign = ContentAlignment.MiddleRight, FlatStyle = FlatStyle.Flat,
                };
                btn_WriteMessage.FlatAppearance.BorderSize = 0; btn_WriteMessage.FlatAppearance.MouseOverBackColor = pb_Avatar.BackColor;
                int w = (int)((FSize + 2) * 1.7); int h = (int)((FSize + 2) * 1.4);
                btn_WriteMessage.Image = Extensions.ResizeImage(Image.FromFile(@"DATA_BASE\IMG\pictograms\ico\read_false.png"), w, h);
                btn_WriteMessage.Click += Control_Click; btn_WriteMessage.MouseEnter += Control_MouseEnter; btn_WriteMessage.MouseLeave += Control_MouseLeave;

                //программное создание таблицы с общей информацией профиля
                TableInfo = Extensions.CreateGrid(Form_AccountProfile, "TableInfo", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_AccountProfile.BackColor, GridColor: Form_AccountProfile.BackColor, AutoSize: true);
                    //создание типов колонок
                    TableInfo.Columns.AddRange(new DataGridViewColumn[] {
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                    });
                    TableInfo.DefaultCellStyle.BackColor = Form_AccountProfile.BackColor;//цвет фона каждой ячейки
                    //скрыть выделение ячеек
                    TableInfo.DefaultCellStyle.SelectionBackColor = TableInfo.DefaultCellStyle.BackColor;
                    TableInfo.DefaultCellStyle.SelectionForeColor = TableInfo.DefaultCellStyle.ForeColor;
                    //шрифт
                    TableInfo.Columns[0].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Bold);
                    TableInfo.Columns[1].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Regular);
                    //автоматическое изменение ширины столбца чтобы поместился весь текст
                    TableInfo.Columns[0].AutoSizeMode = TableInfo.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    //выравнивание содержимого ячейки относительно верх/низ, лево/право
                    TableInfo.Columns[0].DefaultCellStyle.Alignment = TableInfo.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                //программное создание таблицы со списком деревень
                TableVillages = Extensions.CreateGrid(Form_AccountProfile, "TableVillages", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_AccountProfile.BackColor, GridColor: Form_AccountProfile.BackColor,
                    BorderStyle: BorderStyle.FixedSingle, ColumnHeadersVisible: true, ReadOnly: true, Enabled: true);
                TableVillages.CellClick += grid_TableVillages_CellClick;
                TableVillages.CellMouseEnter += grid_TableVillages_CellMouseEnter;
                TableVillages.CellMouseLeave += grid_TableVillages_CellMouseLeave;
                    //создание типов колонок
                    TableVillages.Columns.AddRange(new DataGridViewColumn[] {
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewImageColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewImageColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewImageColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable }, 
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                    });
                var FC = new Font[TableVillages.Columns.Count]; var FH = new Font[TableVillages.Columns.Count];
                var ASM = new DataGridViewAutoSizeColumnMode[TableVillages.Columns.Count]; 
                var DCSA_Cell = new DataGridViewContentAlignment[TableVillages.Columns.Count];
                var DCSA_Header = new DataGridViewContentAlignment[TableVillages.Columns.Count];
                var Regular = new Font(FName, FSize, FontStyle.Regular);
                var Bold = new Font(FName, FSize, FontStyle.Bold);
                for (int i = 0; i < TableVillages.Columns.Count; i++) {
                    FH[i] = Bold;
                    DCSA_Header[i] = DataGridViewContentAlignment.MiddleCenter;
                    DCSA_Cell[i] = DataGridViewContentAlignment.MiddleCenter;
                    ASM[i] = DataGridViewAutoSizeColumnMode.AllCells;
                    if (i == 0) { FC[i] = Bold; DCSA_Cell[i] = DataGridViewContentAlignment.MiddleLeft;
                        ASM[i] = DataGridViewAutoSizeColumnMode.Fill;
                    } else if (i == 5) { FC[i] = Bold; } else { FC[i] = Regular; }
                }
                TableVillages.Settings(Color.White, FC, ASM, DCSA_Cell,
                    ColumnHeadersHeightSizeMode: DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                    RowHeadersWidthSizeMode: DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders,
                    ColumnsDefaultCellStyleFont_Header: FH,
                    DefaultCellStyle_Alignment_Header: DCSA_Header,
                    DefaultCellStyle_SelectionBackColor: Color.Black, DefaultCellStyle_SelectionForeColor: Color.Yellow);
                TableVillages.Columns[0].DefaultCellStyle.ForeColor = TableVillages.Columns[5].DefaultCellStyle.ForeColor =
                    Color.FromArgb(60, 130, 40);//зелёный
                //TableVillages.DefaultCellStyle.Padding = new Padding(0, 2, 0, 2);
                //TableVillages.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                //подсвечиваем строку с деревней в которой находится игрок
                TableVillages.CellMouseMove += (s, e) => { if (e.RowIndex <= -1 || e.ColumnIndex <= -1) return; TableVillages.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = false; TableVillages.Rows[Player.ActiveIndex].Cells[0].Selected = true; };
            }

            TableVillages.Columns[0].HeaderText = LANGUAGES.RESOURSES[128];/*Название деревни*/
            //развбиваем название на 3 части (потому что каждая картинка оазиса в своей ячейке)
            string Header = LANGUAGES.RESOURSES[129];/*Оазисы*/ string[] str = new string[3];
            if (Header.Length <= 3) { str[0] = str[1] = str[2] = Header; }
            else { int len = (int)Math.Round(Header.Length / 3.0);
                str[0] = Header.Substring(0, len); str[1] = Header.Substring(len, len);
                str[2] = Header.Substring(len * 2, Header.Length - len * 2);
            }
            TableVillages.Columns[1].HeaderText = str[0];/*Оазисы - part 1*/
            TableVillages.Columns[2].HeaderText = str[1];/*Оазисы - part 2*/
            TableVillages.Columns[3].HeaderText = str[2];/*Оазисы - part 3*/
            TableVillages.Columns[4].HeaderText = LANGUAGES.RESOURSES[19];/*Население*/
            TableVillages.Columns[5].HeaderText = LANGUAGES.RESOURSES[130];/*Координаты*/

                //ТЕСТ создания деревень
                string path_json1 = "DATA_BASE/JSON/Default_Village.json"; string path_json2; int RR = Random.RND(0, 3);
                if (RR == 0) path_json2 = "DATA_BASE/JSON/Default_Village_slots_4_4_4_6.json";
                else if (RR == 1) path_json2 = "DATA_BASE/JSON/Default_Village_slots_1_1_1_15.json";
                else if (RR == 2) path_json2 = "DATA_BASE/JSON/Default_Village_slots_4_3_5_6.json";
                else path_json2 = "DATA_BASE/JSON/Default_Village_slots_3_4_4_7.json";//RR = 3
                for (int i = 0; i < 4; i++) {
                    var Village = Newtonsoft.Json.JsonConvert.DeserializeObject<TVillage>(File.ReadAllText(path_json1));
                    Village.Slot = Newtonsoft.Json.JsonConvert.DeserializeObject<TVillage.TSlot[]>(File.ReadAllText(path_json2));
                    Player.CreateVillage(Village, GAME, LANGUAGES);
                } Update_Panel_VillageList();

            //заполнение контролов данными
            Form_AccountProfile.Text = LANGUAGES.RESOURSES[123]/*Профиль игрока*/ + " '" + account.Nick_Name + "'";
            int tabX = ToCSR(20);//отступ от левого края формы
            int tabY = ToCSR(20);//отступ между разделами
                //раздел ДАННЫЕ 
                lb_Header[0].Text = LANGUAGES.RESOURSES[7];/*Данные*/ 
                lb_Header[0].Location = new Point(ToCSR(10), ToCSR(10));
                Panel_Header[0].Height = Panel_Header[1].Height  = Panel_Header[2].Height = lb_Header[0].Height + lb_Header[0].Top * 2;
                Panel_Header[0].Location = new Point(tabX, 0);
                string Alliance; if (account.Alliance_Name != "") Alliance = account.Alliance_Name; else Alliance = "-";
                //заполнение таблицы анкетными данными
                SortRank(TypeStatistics.Players);//сортировка игроков в списке ListBox по населению
                TableInfo.Rows.Clear();
                TableInfo.Rows.Add(LANGUAGES.Statistics[11] + ": "/*Ранг*/, account.Rank + " ");
                //TableInfo.Rows.Add(LANGUAGES.RESOURSES[24] + " "/*Игровой ник:*/, account.Nick_Name + " ");
                TableInfo.Rows.Add(LANGUAGES.RESOURSES[20] + " "/*Народ:*/, LANGUAGES.RESOURSES[21 + (int)account.Folk_Name] + " ");
                TableInfo.Rows.Add(LANGUAGES.RESOURSES[17] + " "/*Альянс:*/, Alliance + " ");
                TableInfo.Rows.Add(LANGUAGES.RESOURSES[46] + ": "/*Деревни*/, account.VillageList.Count + " ");
                TableInfo.Rows.Add(LANGUAGES.RESOURSES[16] + " "/*Население:*/, account.Total_Population + " ");
                TableInfo.Rows.Add(LANGUAGES.RESOURSES[121] + " "/*пол:*/, LANGUAGES.RESOURSES[122]/*Не реализовано*/ + " ");
                TableInfo.Location = new Point(tabX, Panel_Header[0].Top + Panel_Header[0].Height + ToCSR(5));

                Location_Recipient = account.VillageList[0].Coordinates_Cell;
                btn_WriteMessage.Text = "     " + LANGUAGES.Messages[19];/*Написать сообщение*/
                btn_WriteMessage.Location = new Point(tabX, TableInfo.Bottom - tabY);
                //диактивируем кнопку в зависимости от условий
                if ((Location_Recipient == Player.VillageList[0].Coordinates_Cell) || (Form_Message != null && Form_Message.Visible))
                    { btn_WriteMessage.Enabled = false; btn_WriteMessage.ForeColor = Color.Gray; }
                else { btn_WriteMessage.Enabled = true; btn_WriteMessage.ForeColor = Color.FromArgb(100, 150, 0); }

                //изменяем размеры аватарки пропорционально под высоту таблицы
                pb_Avatar.Size = new Size((int)(TableInfo.Height * 1.84), TableInfo.Height);
                string FileName;
                if ((int)account.Folk_Name <= -1 || account.Folk_Name == Folk.NULL || account.Folk_Name > Folk.NULL)
                    FileName = "NULL.png"; else FileName = $"{account.Folk_Name}.png";
                pb_Avatar.BackgroundImage = Image.FromFile("DATA_BASE/IMG/natiions/Profile/" + FileName);
                pb_Avatar.Location = new Point(TableInfo.Right, TableInfo.Top);

                //раздел ОПИСАНИЕ / НАГРАДЫ
                lb_Header[1].Text = LANGUAGES.RESOURSES[127];/*Описание*/
                lb_Header[1].Location = new Point(ToCSR(10), ToCSR(10));
                Panel_Header[1].Location = new Point(tabX, btn_WriteMessage.Bottom + tabY);
                //раздел ДЕРЕВНИ
                lb_Header[2].Text = LANGUAGES.RESOURSES[46];/*Деревни*/
                lb_Header[2].Location = new Point(ToCSR(10), ToCSR(10));
                Panel_Header[2].Location = new Point(tabX, Panel_Header[1].Bottom + ToCSR(100));

                //заполнение таблицы деревнями
                TableVillages.Rows.Clear();
                for (int i = 0; i < account.VillageList.Count; i++) {
                    string VillageName = account.VillageList[i].Village_Name;
                    if (i == account.NumberOfCapital) VillageName += " " + LANGUAGES.RESOURSES[42];/*(Столица)*/
                    //построение пиктограмм оазисов и отображение их в ячейках ссылок-оазисов, если их нет то грузим bmp
                    Image[] img = new Image[3];/*картинки в оригинальном размере*/
                    img[0] = new Bitmap(1, 1); img[1] = new Bitmap(1, 1); img[2] = new Bitmap(1, 1);//default нет оазисов
                    
                    //::ТЕСТЫ ОАЗИСОВ:: ::ТЕСТЫ ОАЗИСОВ::
                    account.VillageList[i].OasisList.Clear();
                    int count_in_oasislist = Random.RND(0, 2); int limit = 255;
                    while (true) { if (count_in_oasislist < 0 || limit <= 0) break;
                        int x = Random.RND(0, GAME.Map.Length_X() - 1); int y = Random.RND(0, GAME.Map.Length_Y() - 1);
                        var _ = GAME.Map.Cell[x, y];
                        if (_.ID >= (Cell_ID)9 && _.ID <= (Cell_ID)19 && _.LinkAccount == null) { //все оазисы
                            account.VillageList[i].OasisList.Add(new Point(x, y));
                            _.LinkAccount = account; _.LinkVillage = account.VillageList[i]; int j = account.VillageList[i].OasisList.Count - 1;
                            count_in_oasislist--;
                        } limit--;
                    }

                    //нацепляем картинки оазисов в img[] для таблицы
                    for (int j = 0; j < account.VillageList[i].OasisList.Count; j++) {
                        var oases = GAME.Map.Cell[account.VillageList[i].OasisList[j].X, account.VillageList[i].OasisList[j].Y].ID;
                        img[j] = Image.FromFile($"DATA_BASE/IMG/pictograms/oases/{oases}.png");
                    }

                //ресайз картинок с учётом разрешения экрана
                Image[] IMG = new Image[3]; int size = 25;
                    IMG[0] = Extensions.ResizeImage(img[0], ToCSR(size), ToCSR(size));
                    IMG[1] = Extensions.ResizeImage(img[1], ToCSR(size), ToCSR(size));
                    IMG[2] = Extensions.ResizeImage(img[2], ToCSR(size), ToCSR(size));
                    //добавление строки с данными о деревне
                    TableVillages.Rows.Add(" " + VillageName + " ", IMG[0], IMG[1], IMG[2],
                        " " + account.VillageList[i].Population + " ",
                        " [" + account.VillageList[i].Coordinates_World_Travian.X + " | " + account.VillageList[i].Coordinates_World_Travian.Y + "] ");
                    TableVillages.Rows[TableVillages.Rows.Count - 1].Height = IMG[0].Height;
                    TableVillages.Rows[TableVillages.Rows.Count - 1].Cells[0].Tag = account.VillageList[i];//крепим ссылку
                }
            //сортировка списка деревень в таблице чтобы он совпадал с панелью деревень
            if (TableVillages.RowCount > 1) TableVillages.Sort(TableVillages.Columns[0], System.ComponentModel.ListSortDirection.Ascending);

            //подгон габаритов Grid
            TableVillages.UpdateSize();
            //костыль! свойство таблицы Visible всегда = false до тех пор, пока его Parent (форма) не появится на экране,
            //на экране она появляется только если: 1) Visible формы = true; 2) отрабатывает Show(); ShowDialog();
            //а метод DisplayedRowCount() работает только если свойство Visible таблицы = true
            //по этому мы временно показываем форму здесь
            if (!Form_AccountProfile.Visible) Form_AccountProfile.Visible = true;
            //обрезка высоты таблицы под заданное отображаемое кол-во деревень
            if (TableVillages.DisplayedRowCount(false) > GAME.MaxVillageTabelRowsVisibleProfileWindow)
                TableVillages.Height = GAME.MaxVillageTabelRowsVisibleProfileWindow *
                        TableVillages.Rows[0].Cells[0].PreferredSize.Height +
                        TableVillages.ColumnHeadersHeight; //Columns[0].HeaderCell.PreferredSize.Height;
            Form_AccountProfile.Visible = false;

            TableVillages.Location = new Point(tabX, Panel_Header[2].Top + Panel_Header[2].Height + ToCSR(5));
            //лимит ширины таблицы с деревнями
            if (TableVillages.Width > ToCSR(1600)) TableVillages.Width = ToCSR(1600);
            //выравнивание ширины панелей, картинки и таблиц
            if (TableInfo.Width + pb_Avatar.Width > TableVillages.Width)
                Panel_Header[0].Width = Panel_Header[1].Width = Panel_Header[2].Width = TableInfo.Width + pb_Avatar.Width;
            else Panel_Header[0].Width = Panel_Header[1].Width = Panel_Header[2].Width = TableVillages.Width;
            //выравнивание ширины таблицы с деревнями
            if (TableVillages.Width < Panel_Header[0].Width) TableVillages.Width = Panel_Header[0].Width;
            //лепим кнопки на панели
            for (int i = 0; i < btn_ShowHide.Length; i++) {
                btn_ShowHide[i].Size = new Size(Panel_Header[i].Height - 10, Panel_Header[i].Height - 10);
                btn_ShowHide[i].Location = new Point(Panel_Header[i].Width - btn_ShowHide[i].Width - ToCSR(5), (Panel_Header[i].Height - btn_ShowHide[i].Height) / 2);
            }

            //подсвечиваем строку с деревней в которой находится игрок
            TableVillages.Rows[Player.ActiveIndex].Cells[0].Selected = true;

            if (!Form_AccountProfile.Visible) Form_AccountProfile.ShowDialog();
        }
        //=========================================== ДИАЛОГОВОЕ ОКНО :: ПРОФИЛЬ ИГРОКА ==========================================

        //=============================================== ДИАЛОГОВОЕ ОКНО :: ОТЧЁТЫ ==============================================
        /// <summary> Метод обрабатывает клик по ячейкам таблиц окна отчётов. </summary>
        private void grid_Multi_Reports_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex <= -1 || e.ColumnIndex <= -1) return;//был клик по заголовку
            if (!(sender is DataGridView grid)) return;
            if (grid.Name == "grid_AccountStart" || grid.Name == "grid_AccountFinish") {
                var Cell = grid.Rows[0].Cells[0].Tag as TMap.TCell;
                if ((string)grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != "[?]") {
                    //обработка клика по [альянсу]
                    if (e.ColumnIndex == 1) {
                        //код появится когда будет готова вкладка или окно альянса
                    }
                    //обработка клика по нику
                    if (e.ColumnIndex == 2) { 
                        if (Form_AccountProfile != null && Form_AccountProfile.Visible) Form_AccountProfile.Close();
                        if (Cell.LinkAccount != null) WinDlg_AccountProfile(Cell.LinkAccount);
                    }//обработка клика по деревне
                    else if (e.ColumnIndex == 4) {
                        if (Form_InfoCell != null && Form_InfoCell.Visible) Form_InfoCell.Close();
                            winDlg_InfoCell(Cell.Location.X, Cell.Location.Y);
                            //winDlg_InfoCell(Cell.LinkVillage.Coordinates_Cell.X, Cell.LinkVillage.Coordinates_Cell.Y);
                    }
                }
            }
            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = false;//отмена выделения ячейки
        }
        /// <summary> Метод обрабатывает наведение на ячейку таблиц окна отчётов. </summary>
        private void grid_Multi_Reports_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (!(sender is DataGridView grid)) return;
            var Cell = grid.Rows[0].Cells[0].Tag as TMap.TCell;
            if (grid.Name == "grid_AccountStart" || grid.Name == "grid_AccountFinish") {
                if ((string)grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != "[?]") {
                    if (e.ColumnIndex == 2) { //ник
                        if (Cell.LinkAccount != null)
                            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
                    } else if (e.ColumnIndex == 4) { //деревня
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
                    } else if (e.ColumnIndex == 1) { //[альянс]
                        if (Cell.LinkAccount != null && Cell.LinkAccount.IsAlliance())
                            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red;
                    }
                }
            }
        }
        /// <summary> Метод обрабатывает выход за пределы ячейки таблиц окна отчётов. </summary>
        private void grid_Multi_Reports_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            if (!(sender is DataGridView grid)) return;
            var Cell = grid.Rows[0].Cells[0].Tag as TMap.TCell; 
            if (grid.Name == "grid_AccountStart" || grid.Name == "grid_AccountFinish") {
                if ((string)grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != "[?]") {
                    if (e.ColumnIndex == 2) { //ник
                        if (Cell.LinkAccount != null)
                            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.FromArgb(100, 150, 0);
                    } else if (e.ColumnIndex == 4) { //деревня
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.FromArgb(100, 150, 0);
                    } else if (e.ColumnIndex == 1) { //[альянс]
                        if (Cell.LinkAccount != null && Cell.LinkAccount.IsAlliance())
                            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.FromArgb(100, 150, 0);
                    }
                }
            }
        }

        /// <summary> Окно подробных мульти отчётов. </summary>
        private Form Form_Multi_Reports = null;
        /// <summary> Таблица информации "шапка" [событие, дата]. </summary>
        private DataGridView grid_Head = null;
        /// <summary> Массив-таблица информации об аккаунте из координат [0]: "СТАРТ"; [1] "ФИНИШ". </summary>
        private DataGridView[] grid_Account = null;
        /// <summary> Массив-таблица пиктограмм войск аккаунта из координат: [0] "СТАРТ"; [1] "ФИНИШ"; [2..5] "Остальные нации". </summary>
        private DataGridView[] grid_Tabel_Army_PIC = null;
        /// <summary> Массив-таблица величин войск аккаунта из координат: [0] "СТАРТ"; [1] "ФИНИШ"; [2..5] "Остальные нации". </summary>
        private DataGridView[] grid_Tabel_Army_TXT = null;
        /// <summary> Массив-таблица информации атаки/разведки/торговли аккаунтов [0] "СТАРТ"; [1] "ФИНИШ"; - тараны, катапульты, уровни сноса, ни один ваш солдат не вернулся, торговля и т.д. </summary>
        private DataGridView[] grid_Tabel_Information = null;
        /// <summary> Массив-таблица заголовка подкреплений 5 наций включая природу и натар, если они стоят в подкрепе. <br/> [0] Римляне; [1] Германцы; [2] Галлы; [3] Природа; [4] Натары. </summary>
        private DataGridView[] grid_Head_Reinforcements = null;
        /// <summary> Массив пиктограмм ресурсов на 6 элементов. </summary>
        private PictureBox[] img_Resources = null;
        /// <summary> Массив текстовых величин ресурсов на 7 элементов. </summary>
        private Label[] txt_Resources = null;

        public void WinDlg_Multi_Reports(int index, TReport.TData report) {
            float FSize = 14;/*размер шрифта*/ int SizePic = 21;/*размер пиктограмм*/ string FName = "Arial";
            Color[] bgCell = new Color[] { default, default };
            var TypeEvent = TypeReport_To_LanguagesReports(report.TypeReport, out bgCell[0], out bgCell[1]);
            int Tab_X = ToCSR(25); int Tab_Y = ToCSR(25);//стандартные отступы
            int Tab_x = ToCSR(5); int Tab_y = ToCSR(5);//минимальные отступы
            var GridColor = Color.FromArgb(200, 200, 200);//цвет сетки таблиц

            //коррекция размеров под количество подкрепов
            for (int i = 0; i < 5; i++) if (GAME.IsNotZero_Array(report.Defense_Troops, i * 11, 11)) { FSize--; SizePic--; }
            if (FSize > 12) FSize = ToCSR(12); if (SizePic > 20) SizePic = ToCSR(20);
            if (Form_Multi_Reports != null) { Form_Multi_Reports.Dispose(); Form_Multi_Reports = null; }

            if (Form_Multi_Reports == null) {
                Form_Multi_Reports = Extensions.CreateForm("Form_Multi_Reports", new Icon("DATA_BASE/IMG/logotip.ico"),
                    new Font(Font.FontFamily, FSize, FontStyle.Bold), LANGUAGES.Reports[0]/*Отчёты*/,
                    StartPosition: FormStartPosition.CenterParent, FormBorderStyle: FormBorderStyle.FixedSingle,
                    Padding: new Padding(5, 5, 5, 5), ControlBox: true, KeyPreview: true, AutoSize: true, AutoScroll: true);
                Form_Multi_Reports.HorizontalScroll.Visible = false;
                Form_Multi_Reports.MaximumSize = new Size(ScreenBounds_Size().Width, (int)(ScreenBounds_Size().Height * 0.925));
                Form_Multi_Reports.KeyUp += new KeyEventHandler(KeyUp_EscapeFormClose);

                //::программное создание таблиц::
                //grid_Head
                grid_Head = Extensions.CreateGrid(Form_Multi_Reports, "grid_Head", new Font(FName, FSize, FontStyle.Regular), 
                    new Point(ToCSR(10), ToCSR(10)), Color.FromArgb(200, 200, 200), Color.FromArgb(200, 200, 200),
                    BorderStyle: BorderStyle.FixedSingle);
                var __ = grid_Head;
                __.Columns.AddRange(new DataGridViewColumn[] {
                    new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                    new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                });
                var FC = new Font[__.Columns.Count];
                var ASM = new DataGridViewAutoSizeColumnMode[__.Columns.Count]; var DCSA_Cell = new DataGridViewContentAlignment[__.Columns.Count];
                var Regular = new Font(FName, FSize, FontStyle.Regular); var Bold = new Font(FName, FSize, FontStyle.Bold);
                FC[0] = Regular; ASM[0] = DataGridViewAutoSizeColumnMode.AllCells;
                FC[1] = Bold; ASM[1] = DataGridViewAutoSizeColumnMode.Fill;
                __.Settings(__.BackgroundColor, FC, ASM, DCSA_Cell, DataGridViewAutoSizeRowsMode.AllCells, ScrollBars.None,
                    DefaultCellStyle_SelectionBackColor: __.BackgroundColor,
                    DefaultCellStyle_SelectionForeColor: __.DefaultCellStyle.ForeColor);
                //grid_Account
                grid_Account = new DataGridView[] {
                    /*[0] START*/Extensions.CreateGrid(Form_Multi_Reports, "grid_AccountStart", Regular,
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: Form_Multi_Reports.BackColor, ReadOnly: true, Enabled: true),
                    /*[1] FINISH*/Extensions.CreateGrid(Form_Multi_Reports, "grid_AccountFinish", Regular,
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: Form_Multi_Reports.BackColor, ReadOnly: true, Enabled: true)
                };
                var ___ = grid_Account; int ColumnCount = 5;
                FC = new Font[ColumnCount]; ASM = new DataGridViewAutoSizeColumnMode[ColumnCount]; DCSA_Cell = new DataGridViewContentAlignment[ColumnCount];
                Regular = new Font(FName, FSize, FontStyle.Regular); Bold = new Font(FName, FSize, FontStyle.Bold);
                for (int x = 0; x < ColumnCount; x++) {
                    FC[x] = Bold; ASM[x] = DataGridViewAutoSizeColumnMode.AllCells; DCSA_Cell[x] = DataGridViewContentAlignment.MiddleCenter;
                }
                FC[3] = Regular; ASM[4] = DataGridViewAutoSizeColumnMode.Fill; DCSA_Cell[4] = DataGridViewContentAlignment.MiddleLeft;
                for (int i = 0; i < ___.Length; i++) {
                    for (int x = 0; x < ColumnCount; x++) ___[i].Columns.Add(new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable });
                    ___[i].Settings(Form_Multi_Reports.BackColor, FC, ASM, DCSA_Cell, DataGridViewAutoSizeRowsMode.AllCells, ScrollBars.None);
                    ___[i].CellClick += grid_Multi_Reports_CellClick;
                    ___[i].CellMouseEnter += grid_Multi_Reports_CellMouseEnter;
                    ___[i].CellMouseLeave += grid_Multi_Reports_CellMouseLeave;
                }
                //grid_Tabel_Army_PIC
                grid_Tabel_Army_PIC = new  DataGridView[] {
                    /*[0] START*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_ArmyStart_PIC", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true, Enabled: true),
                    /*[1] FINISH*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_ArmyFinish_PIC", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true, Enabled: true),
                    /*[2] ROMES*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_Reinforcements_Rome_PIC", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true, Enabled: true),
                    /*[3] TEUTONS*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_Reinforcements_German_PIC", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true, Enabled: true),
                    /*[4] GAULS*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_Reinforcements_Gaul_PIC", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true, Enabled: true),
                    /*[5] NATURES*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_Reinforcements_Nature_PIC", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true, Enabled: true),
                    /*[6] NATARS*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_Reinforcements_Natar_PIC", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true, Enabled: true),
                };
                ___ = grid_Tabel_Army_PIC;
                for (int i = 0; i < ___.Length; i++) {
                    //скрыть выделение ячеек
                    ___[i].DefaultCellStyle.SelectionBackColor = Color.White; ___[i].DefaultCellStyle.SelectionForeColor = Color.White;
                    for (int x = 0; x < 12; x++) { //добавление однотипных колонок
                        ___[i].Columns.Add(new DataGridViewImageColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable });
                        ___[i].Columns[x].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        ___[i].Columns[x].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        ___[i].Columns[x].DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
                    }
                    ___[i].Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    ___[i].AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    ___[i].DefaultCellStyle.Padding = new Padding(0, 4, 0, 4);
                    ___[i].ScrollBars = ScrollBars.None;
                }
                //grid_Tabel_Army_TXT
                grid_Tabel_Army_TXT = new DataGridView[] {
                    /*[0] START*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_ArmyStart_TXT", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true),
                    /*[1] FINISH*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_ArmyFinish_TXT", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true),
                    /*[2] ROMES*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_Reinforcements_Rome_TXT", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true), 
                    /*[3] TEUTONS*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_Reinforcements_German_TXT", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true),
                    /*[4] GAULS*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_Reinforcements_Gaul_TXT", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true),
                    /*[5] NATURES*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_Reinforcements_Nature_TXT", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true),
                    /*[6] NATARS*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Tabel_Reinforcements_Natar_TXT", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: GridColor, ReadOnly: true),
                };
                ___ = grid_Tabel_Army_TXT;
                for (int i = 0; i < ___.Length; i++) {
                    for (int x = 0; x < 12; x++) { //добавление однотипных колонок
                        ___[i].Columns.Add(new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable });
                        ___[i].Columns[x].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Regular);
                        ___[i].Columns[x].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        ___[i].Columns[x].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        ___[i].Columns[x].DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
                    }
                    ___[i].Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    ___[i].AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    ___[i].ScrollBars = ScrollBars.None;
                }
                //grid_Tabel_Information
                grid_Tabel_Information = new DataGridView[] {
                    /*[0] START*/Extensions.CreateGrid( Form_Multi_Reports, "grid_Tabel_ArmyStart_Information",
                    new Font(FName, FSize, FontStyle.Regular), BackgroundColor: Form_Multi_Reports.BackColor,
                    GridColor: Color.FromArgb(200, 200, 200), ReadOnly: true),
                    /*[1] FINISH*/Extensions.CreateGrid( Form_Multi_Reports, "grid_Tabel_ArmyStart_Information",
                    new Font(FName, FSize, FontStyle.Regular), BackgroundColor: Form_Multi_Reports.BackColor,
                    GridColor: Color.FromArgb(200, 200, 200), ReadOnly: true),
                };
                ___ = grid_Tabel_Information;
                for (int i = 0; i < ___.Length; i++) {
                    ___[i].Columns.AddRange(new DataGridViewColumn[] {
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewImageColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                    });
                    for (int j = 0; j < ___[i].Columns.Count; j++) {
                        ___[i].Columns[j].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Regular);
                        ___[i].Columns[j].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        ___[i].Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        ___[i].Columns[j].DefaultCellStyle.BackColor = Color.FromArgb(200, 200, 200);
                    }
                    ___[i].Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    ___[i].Columns[0].DefaultCellStyle.Alignment = __.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    ___[i].AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    ___[i].DefaultCellStyle.Padding = new Padding(0, 2, 0, 2);
                    ___[i].ScrollBars = ScrollBars.None;
                    ___[i].TextMultiColorCell(true);//включить раскраску текста каждой ячейки в разный цвет с заданным алгоритмом
                }
                //grid_Head_Reinforcements [5]
                grid_Head_Reinforcements = new DataGridView[] {
                    /*[0] ROMES*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Head_Reinforcements_Rome", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: Form_Multi_Reports.BackColor, ReadOnly: true),
                    /*[1] TEUTONS*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Head_Reinforcements_German", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: Form_Multi_Reports.BackColor, ReadOnly: true),
                    /*[2] GAULS*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Head_Reinforcements_Gaul", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: Form_Multi_Reports.BackColor, ReadOnly: true),
                    /*[3] NATURES*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Head_Reinforcements_Nature", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: Form_Multi_Reports.BackColor, ReadOnly: true),
                    /*[4] NATARS*/Extensions.CreateGrid(Form_Multi_Reports, "grid_Head_Reinforcements_Natar", new Font(FName, FSize, FontStyle.Regular),
                    BackgroundColor: Form_Multi_Reports.BackColor, GridColor: Form_Multi_Reports.BackColor, ReadOnly: true),
                };
                for (int i = 0; i < grid_Head_Reinforcements.Length; i++) {
                    __ = grid_Head_Reinforcements[i];
                    for (int x = 0; x < 2; x++) {
                        __.Columns.Add(new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable });
                        __.Columns[x].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Regular);
                        __.Columns[x].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        __.Columns[x].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        __.Columns[x].DefaultCellStyle.BackColor = Form_Multi_Reports.BackColor;
                    }
                    __.Columns[0].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Bold);
                    __.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    __.Columns[0].DefaultCellStyle.BackColor = bgCell[1];
                    __.Columns[0].DefaultCellStyle.ForeColor = Color.White;
                }
                __.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                __.ScrollBars = ScrollBars.None;

                //img_Resources[] - txt_Resources[]
                img_Resources = new PictureBox[6]; txt_Resources = new Label[7];
                for (int i = 0; i < 7; i++) {
                    if (i < 6) {
                        img_Resources[i] = new PictureBox() { Parent = Form_Multi_Reports,
                            BackgroundImageLayout = ImageLayout.Stretch,
                            BackgroundImage = Image.FromFile($"DATA_BASE/IMG/pictograms/resources/0{i}.png")
                        }; img_Resources[i].Size = img_Resources[i].BackgroundImage.Size;
                    }
                    //картинка №4 "farm _0/_1/_2.png" грузится ниже
                    txt_Resources[i] = new Label { Parent = Form_Multi_Reports, AutoSize = true, ForeColor = Color.Black,
                        Font = new Font(FName, FSize + 1, FontStyle.Regular),
                    };
                } img_Resources[4].BackgroundImage = Image.FromFile($"DATA_BASE/IMG/pictograms/resources/05.png");
            }

            //Во всех массивах локальных переменных и контролов в первом измерении: [0] START; [1] FINISH;
            //ТЕСТ [?] в деревне и аккаунте
            //GAME.Map.Cell[report.Cell_Finish.X, report.Cell_Finish.Y].LinkAccount = null;
            //GAME.Map.Cell[report.Cell_Finish.X, report.Cell_Finish.Y].LinkVillage = null;

            var ReportPoint = new Point[] { report.Cell_Start, report.Cell_Finish };
            var ReportCell = new TMap.TCell[] {
                GAME.Map.Cell[report.Cell_Start.X, report.Cell_Start.Y],
                GAME.Map.Cell[report.Cell_Finish.X, report.Cell_Finish.Y]
            };
            var LinkAccount = new TPlayer[] { ReportCell[0].LinkAccount, ReportCell[1].LinkAccount };
            var LinkVillage = new TVillage[] { ReportCell[0].LinkVillage, ReportCell[1].LinkVillage };
            Folk tmp = LinkAccount[1] != null ? LinkAccount[1].Folk_Name : Folk.Nature;
            var folk = new Folk[] { LinkAccount[0].Folk_Name, tmp };
            string[] AccountName = new string[] {
                LinkAccount[0] == null ? "[?]" : LinkAccount[0].Nick_Name,
                LinkAccount[1] == null ? "[?]" : LinkAccount[1].Nick_Name
            };
            string[] VillageName = new string[] { 
                LinkVillage[0] == null ? "[?]" : LinkVillage[0].Village_Name,
                LinkVillage[1] == null ? "[?]" : LinkVillage[1].Village_Name
            };

            //- чисти вех, Бог узнает своих! (С)
            grid_Head.Rows.Clear(); grid_Account[0].Rows.Clear(); grid_Account[1].Rows.Clear();
            grid_Tabel_Information[0].Rows.Clear(); grid_Tabel_Information[1].Rows.Clear();
            for (int i = 0; i < grid_Head_Reinforcements.Length; i++) grid_Head_Reinforcements[i].Rows.Clear();
            for (int i = 0; i < grid_Tabel_Army_PIC.Length; i++) { grid_Tabel_Army_PIC[i].Rows.Clear(); grid_Tabel_Army_TXT[i].Rows.Clear(); }

            //ШАПКА
            var _ = grid_Head;
            _.Rows.Add(" " + LANGUAGES.Reports[7]/*Событие:*/ + " ", grid_Reports.Rows[index].Cells[3].Value);
            _.Rows.Add(" " + LANGUAGES.Reports[8] + " "/*Дата:*/, grid_Reports.Rows[index].Cells[5].Value);
            //ЗАГОЛОВОК ТАБЛИЦЫ С ВОЙСКАМИ
            for (int i = 0; i < grid_Account.Length; i++) {
                if (TypeEvent[i].Contains("Error")) continue;//TypeEvent[1] содержит "Error" если не надо отображдать блок обороны
                _ = grid_Account[i];//[0] START; [1] FINISH;
                string[] str = new string[5];
                str[0] = TypeEvent[i];
                str[1] = ReportCell[i].IsFree() ? "[]" : $"[{LinkAccount[i].Alliance_Name}]";
                str[2] = ReportCell[i].IsFree() ? ReportCell[i].IsOasis() ?
                    LANGUAGES.RESOURSES[143]/*Природа*/ : AccountName[i] : AccountName[i];
                str[3] = LinkVillage[i] != null ? LANGUAGES.Reports[60]/*из деревни*/ : LANGUAGES.Reports[87];/*из*/
                str[4] = ReportCell[i].IsFree() ? ReportCell[i].IsOasis() ?
                    $"{LANGUAGES.Reports[86]/*Дикий оазис*/} ({GAME.Map.Coord_MapToWorld(ReportPoint[i]).X} | {GAME.Map.Coord_MapToWorld(ReportPoint[i]).Y})"
                    : VillageName[i]
                    : ReportCell[i].IsOasis() ? $"{LANGUAGES.Reports[85]/*Захваченный оазис*/} ({LinkVillage[i].Coordinates_World_Travian.X} | {LinkVillage[i].Coordinates_World_Travian.Y})"
                    : VillageName[i];

                //ТЕСТ заливаем в оборону войска природы из оазиса на карте
                if (str[2] == LANGUAGES.RESOURSES[143]/*Природа*/)
                    for (int n = 0; n < report.Defense_Village_Troops.Length - 1; n++)
                        report.Defense_Village_Troops[n] = ReportCell[i].AllTroops[n + 30];

                _.Rows.Add(str[0], str[1], str[2], str[3], str[4]);
                _.Rows[0].Cells[0].Style.BackColor = bgCell[i]; _.Rows[0].Cells[0].Style.ForeColor = Color.White;
                _.Rows[0].Cells[2].Style.ForeColor = str[2] == "[?]" ? Color.FromArgb(128, 128, 128) : Color.FromArgb(100, 150, 0);
                _.Rows[0].Cells[4].Style.ForeColor = str[4] == "[?]" ? Color.FromArgb(128, 128, 128) : Color.FromArgb(100, 150, 0);
                if (str[2] == LANGUAGES.RESOURSES[143]/*Природа*/) _.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                //всплывающая подсказка об альянсе
                if (LinkAccount[i] != null) { //если не дикое поле или свободный оазис
                    if (LinkAccount[i].IsAlliance()) { var C = _.Rows[0].Cells[1]; C.Style.ForeColor = Color.FromArgb(100, 150, 0); C.ToolTipText = $"{LANGUAGES.RESOURSES[17]/*Альянс:*/} {ReportCell[i].LinkAccount.Alliance_Name}"; }
                    else { var C = _.Rows[0].Cells[1]; C.Style.ForeColor = _.ForeColor; C.ToolTipText = $"{LANGUAGES.RESOURSES[17]/*Альянс:*/} {LANGUAGES.Reports[62]/*'беспартийный'*/}"; }
                }
                _.Rows[0].Cells[0].Tag = ReportCell[i];//ссылка на ячейку TCell
            }
            //FARM visible
            for (int i = 0; i < img_Resources.Length; i++) img_Resources[i].Visible = false;
            for (int i = 0; i < txt_Resources.Length; i++) txt_Resources[i].Visible = false;
            img_Resources[img_Resources.Length - 1].Tag = false;//костыльный флаг false = объект не видим

            //отображаем войска в таблицах для двух сторон и информацию о таранах, катапультах и т.д.
            if (GAME.IsNotZero_Array(report.Attack_Troops) || GAME.IsNotZero_Array(report.Defense_Village_Troops) ||
                GAME.IsNotZero_Array(report.Defense_Troops) || GAME.IsNotZero_Array(report.Units)) { 
                string path = "DATA_BASE/IMG/pictograms/unit"; int PicCount = 11; 
                Image[][] IMG = new Image[][] { new Image[PicCount], new Image[PicCount],
                                                new Image[PicCount], new Image[PicCount], new Image[PicCount],
                                                new Image[PicCount], new Image[PicCount] };//пиктограммы войск в таблицах
                //подсчёт процента убитых подкрепов и обороны
                int summ_deff_troops = 0, summ_deff_losses = 0; double percent_deff = 0;
                for (int j = 0; j < report.Defense_Village_Troops.Length; j++) { summ_deff_troops += report.Defense_Village_Troops[j]; summ_deff_losses += report.Defense_Village_Losses[j]; }
                for (int j = 0; j < report.Defense_Troops.Length; j++) { summ_deff_troops += report.Defense_Troops[j]; summ_deff_losses += report.Defense_Losses[j]; }
                if (summ_deff_troops <= 0) percent_deff = 0; else percent_deff = (double)summ_deff_losses / summ_deff_troops * 100.0;

                //ТЕСТ процентов
                //percent_deff = 26;

                for (int i = 0; i < grid_Tabel_Army_PIC.Length; i++) {
                    //проверка перед заполнением таблиц подкрепов 
                    if (i >= 2 && Player.Nick_Name == AccountName[0]) {
                        //выход из цикла перед заполнением таблиц подкрепов если никто не выжил у Player
                        if (!GAME.IsNotZero_Array(report.Units)) if (percent_deff < 25) break;
                    }

                    _ = grid_Tabel_Army_PIC[i];//[0] START [1] FINISH [2..5] остальные нации
                    if (i < 2) for (int x = 0; x < PicCount - 1; x++) IMG[i][x] = Extensions.ResizeImage(Image.FromFile($"{path}/{folk[i]}/{((int)folk[i] * (IMG[i].Length - 1)) + x + 1}.gif"), SizePic, SizePic);
                    else for (int x = 0; x < PicCount - 1; x++) IMG[i][x] = Extensions.ResizeImage(Image.FromFile($"{path}/{(Folk)(i - 2)}/{(i - 2) * (IMG[i].Length - 1) + x + 1}.gif"), SizePic, SizePic);
                    string ext = "gif"; if (i < 2) if (AccountName[i] == Player.Nick_Name) ext = "png";
                    IMG[i][10] = Extensions.ResizeImage(Image.FromFile($"{path}/Hero/51.{ext}"), SizePic, SizePic);
                    if (i >= 5) IMG[i][10] = new Bitmap(1, 1);
                    _.Rows.Add(new Bitmap(1, 1), IMG[i][0], IMG[i][1], IMG[i][2], IMG[i][3], IMG[i][4],
                                                 IMG[i][5], IMG[i][6], IMG[i][7], IMG[i][8], IMG[i][9], IMG[i][10]);
                    //_.Rows[0].Height = ToCSR(SizePic + 6); //аля Padding внутри ячейки
                    string[][] Units = new string[][] { LANGUAGES.Rome_Name, LANGUAGES.German_Name,
                        LANGUAGES.Gaul_Name, LANGUAGES.Animal_Name, LANGUAGES.Natar_Name };
                    if (i < 2) for (int x = 1; x < _.Rows[0].Cells.Count - 1; x++) _.Rows[0].Cells[x].ToolTipText = Units[(int)folk[i]][x - 1];//юниты
                    else for (int x = 1; x < _.Rows[0].Cells.Count - 1; x++) _.Rows[0].Cells[x].ToolTipText = Units[i - 2][x - 1];//юниты
                    if (i != 5) _.Rows[0].Cells[11].ToolTipText = LANGUAGES.RESOURSES[141];/*Герой*/

                    _ = grid_Tabel_Army_TXT[i];//[0] START [1] FINISH [2..5] нации подкрепа
                    _.Rows.Add(LANGUAGES.Reports[2]/*Войска*/); _.Rows.Add(LANGUAGES.Reports[61]/*Потери*/);
                    //грузим нужный массив в каждый блок
                    int[][] Arr_Troops = new int[][] { report.Attack_Troops, report.Defense_Village_Troops };
                    int[][] Arr_Losses = new int[][] { report.Attack_Losses, report.Defense_Village_Losses };
                    int[][] Arr_Reinf = new int[][] { report.Defense_Troops, report.Defense_Losses };

                    for (int x = 1; x < _.Rows[0].Cells.Count; x++) {
                        //ставим "?" у обороны если:
                        // - ни один солдат не вернулся у Player ("?" у 2 аккаунта, подкрепы не выводим)
                        // - вернулся хотя бы один солдат у Player, но у обороны убито менее 25% ("?" у всех подкрепов)
                        if (i < 2) {
                            _.Rows[0].Cells[x].Value = Arr_Troops[i][x - 1].ToString();
                            _.Rows[1].Cells[x].Value = Arr_Losses[i][x - 1].ToString();
                        } else { 
                            int pos = (i - 2) * report.Attack_Troops.Length;
                            _.Rows[0].Cells[x].Value = Arr_Reinf[0][pos + x - 1].ToString();
                            _.Rows[1].Cells[x].Value = Arr_Reinf[1][pos + x - 1].ToString();
                        }
                        if (i >= 1) if ((report.TypeReport == Type_Report.Reinforcements_Attacked) ||
                            (!GAME.IsNotZero_Array(report.Units) && Player.Nick_Name == AccountName[0])) {
                                if (percent_deff < 25) { _.Rows[0].Cells[x].Value = "?"; _.Rows[1].Cells[x].Value = "?"; }
                        }
                        //если у обороны убито менее 25% личного состава
                        if (i >= 2) if ((report.TypeReport < Type_Report.Won_Scout_Attacker_GREEN ||
                                        report.TypeReport > Type_Report.Lost_Scout_Defender_YELLOW) &&
                                        (Player.Nick_Name == AccountName[0]))
                            if (percent_deff < 25) { _.Rows[0].Cells[x].Value = "?"; _.Rows[1].Cells[x].Value = "?"; }
                    }
                    for (int y = 0; y < _.Rows.Count; y++) for (int x = 0; x < _.Rows[y].Cells.Count; x++) {
                        _.Rows[y].Cells[x].Style.ForeColor = (string)_.Rows[y].Cells[x].Value == "0" ? _.GridColor :
                                (string)_.Rows[y].Cells[x].Value == "?" ? _.GridColor : _.Rows[y].Cells[x].Style.ForeColor;
                    }
                }

                //доп. информация о таранах, катапультах, капканах и т.д.
                _ = grid_Tabel_Information[0]; string cell_2 = ""; var LR = LANGUAGES.Reports;
                if (report.Attack_Troops[6] > 0) { //таранов больше нуля
                    if (report.TypeEvent == Type_Event.ATTACK || report.TypeEvent == Type_Event.ADVENTURE_ATTACK) {
                        cell_2 = $"{LANGUAGES.buildings[(int)folk[1] + 31]}: ";//название стены обороняющегося
                        if (report.Wall_BeforeLevel <= 0) cell_2 += $"/{LR[64]/*строение отсутствует*/}";
                        else if (report.Wall_BeforeLevel > 0) {
                            if (report.Wall_BeforeLevel == report.Wall_AfterLevel) cell_2 += $"/{LR[65]/*не повреждено*/}";
                            else if (report.Wall_AfterLevel == 0) cell_2 += $"/{LR[66]/*разрушено полностью*/}";
                            else if (report.Wall_AfterLevel > 0)  {
                                cell_2 += $"{LR[67]/*разрушено с уровня*/}/{report.Wall_BeforeLevel}/";/*lvl*/
                                cell_2 += $"{LR[68]/*до уровня*/}/{report.Wall_AfterLevel}";/*lvl*/
                            }
                        }
                        _.Rows.Add($"   {LR[63]}   "/*Информация*/, IMG[0][6]/*тараны атакующего*/, cell_2);
                    } else if (report.TypeEvent == Type_Event.RAID || report.TypeEvent == Type_Event.ADVENTURE_RAID) { 
                        _.Rows.Add($"   {LR[63]}   "/*Информация*/, IMG[0][6]/*тараны атакующего*/, LR[70]/*Осадные орудия бездействовали в этом набеге*/);
                        _.Rows[_.Rows.Count - 1].Cells[2].Style.Font = new Font(_.DefaultCellStyle.Font.Name, _.DefaultCellStyle.Font.Size, FontStyle.Italic);
                    }
                }
                if (report.Attack_Troops[7] > 0) { //катапульт больше нуля
                    if (report.TypeEvent == Type_Event.ATTACK || report.TypeEvent == Type_Event.ADVENTURE_ATTACK) {
                        //первая цель для катапульт атакующего
                        if (report.Construction_Name_1 != Buildings.ПУСТО) {
                            cell_2 = $" {LANGUAGES.buildings[(int)report.Construction_Name_1]}";//название постройки
                            if (report.Construction_TargetRandom_1) cell_2 += $"/{LR[69]/*[Случайная цель]*/}:/ "; else cell_2 += ": ";
                            if (report.Construction_BeforeLevel_1 > 0) {
                                if (report.Construction_BeforeLevel_1 == report.Construction_AfterLevel_1) cell_2 += $"/{LR[65]/*не повреждено*/}";
                                else if (report.Construction_AfterLevel_1 == 0) cell_2 += $"/{LR[66]/*разрушено полностью*/}";
                                else if (report.Construction_AfterLevel_1 > 0) {
                                    cell_2 += $"{LR[67]/*разрушено с уровня*/}/{report.Construction_BeforeLevel_1}/";/*lvl*/
                                    cell_2 += $"{LR[68]/*до уровня*/}/{report.Construction_AfterLevel_1}";/*lvl*/
                                }
                                _.Rows.Add($"   {LR[63]}   "/*Информация*/, IMG[0][7]/*каты атакующего*/, cell_2);
                            } 
                        }
                        //вторая цель для катапульт атакующего
                        if (report.Construction_Name_2 != Buildings.ПУСТО) {
                            cell_2 = $" {LANGUAGES.buildings[(int)report.Construction_Name_2]}";//название постройки
                            if (report.Construction_TargetRandom_2) cell_2 += $"/{LR[69]/*[Случайная цель]*/}:/ "; else cell_2 += ": ";
                            if (report.Construction_BeforeLevel_2 > 0) {
                                if (report.Construction_BeforeLevel_2 == report.Construction_AfterLevel_2) cell_2 += $"/{LR[65]/*не повреждено*/}";
                                else if (report.Construction_AfterLevel_2 == 0) cell_2 += $"/{LR[66]/*разрушено полностью*/}";
                                else if (report.Construction_AfterLevel_2 > 0) {
                                    cell_2 += $"{LR[67]/*разрушено с уровня*/}/{report.Construction_BeforeLevel_2}/";/*lvl*/
                                    cell_2 += $"{LR[68]/*до уровня*/}/{report.Construction_AfterLevel_2}";/*lvl*/
                                }
                                _.Rows.Add($"   {LR[63]}   "/*Информация*/, IMG[0][7]/*каты атакующего*/, cell_2);
                            } 
                        }
                    } else if (report.TypeEvent == Type_Event.RAID || report.TypeEvent == Type_Event.ADVENTURE_RAID) { 
                        _.Rows.Add($"   {LR[63]}   "/*Информация*/, IMG[0][7]/*каты атакующего*/, LR[70]/*Осадные орудия бездействовали в этом набеге*/);
                        _.Rows[_.Rows.Count - 1].Cells[2].Style.Font = new Font(_.DefaultCellStyle.Font.Name, _.DefaultCellStyle.Font.Size, FontStyle.Italic);
                    }
                }

                //шаманим капканщика
                if (report.WarriorsFreedFromTraps_Count > -1 && report.WarriorsFreedFromTraps != Traps.None) {
                    cell_2 = $"{LR[73]/*Ты освободил*/}/{report.WarriorsFreedFromTraps_Count}/";
                    if (report.WarriorsFreedFromTraps == Traps.My_Troops) cell_2 += LR[74];/*своих солдат*/
                    else if (report.WarriorsFreedFromTraps == Traps.Ally_Troops) cell_2 += LR[75];/*солдат союзников*/
                    else if (report.WarriorsFreedFromTraps == Traps.My_And_Ally_Troops) cell_2 += LR[76];/*солдат (своих и союзников)*/
                    _.Rows.Add($"   {LR[63]}   "/*Информация*/, new Bitmap(1, 1), cell_2);
                }

                if ((report.TypeEvent == Type_Event.RAID || report.TypeEvent == Type_Event.ADVENTURE_RAID ||
                    report.TypeEvent == Type_Event.ATTACK || report.TypeEvent == Type_Event.ADVENTURE_ATTACK) //&&
                    //(report.TypeReport != Type_Report.Animals_Caught && )
                    ) {
                    //не разведка
                    if (report.TypeReport < Type_Report.Won_Scout_Attacker_GREEN ||
                        report.TypeReport > Type_Report.Lost_Scout_Defender_YELLOW) {
                        //добыча-фарм
                        for (int i = 0; i < img_Resources.Length; i++) img_Resources[i].Visible = true;
                        for (int i = 0; i < txt_Resources.Length; i++) txt_Resources[i].Visible = true;
                        img_Resources[img_Resources.Length - 1].Tag = true;//костыльный флаг true = объект видим
                        var _IS = img_Resources; var _TS = txt_Resources;
                        _IS[5].BackgroundImage = Farm(report, out string farm);//farm: Добыто: 12345/12345
                        _IS[5].Size = _IS[5].BackgroundImage.Size;
                        for (int i = 0; i < _TS.Length - 2; i++) _TS[i].Text = report.Resources[i].ToString();
                        _TS[5].Text = farm; _TS[6].Text = LANGUAGES.Reports[15]/*Добыча*/;
                    }
                    //% истребления армии атакующего/обороняющегося
                    cell_2 = ""; int TroopCount = 0; int TroopLosses = 0; double percent_attack = 0;
                    for (int i = 0; i < report.Attack_Troops.Length; i++) { TroopCount += report.Attack_Troops[i]; TroopLosses += report.Attack_Losses[i]; }
                    if (TroopCount <= 0) percent_attack = 0; else percent_attack = (double)TroopLosses / TroopCount * 100;
                    if (percent_attack >= 100) cell_2 = LR[72];/*Ни один ваш солдат не вернулся*/
                    else if (percent_attack > 0) cell_2 = $"{LR[71]/*Атакующие избиты на*/}/{(int)percent_attack}%";
                    if (percent_attack > 0) _.Rows.Add($"   {LR[63]}   "/*Информация*/, new Bitmap(1, 1), cell_2);
                    if (percent_deff >= 100) _.Rows.Add($"   {LR[63]}   "/*Информация*/, new Bitmap(1, 1), LR[84]/*Атакующий помножил всю оборону на ноль!*/);
                    else if (percent_deff > 0) _.Rows.Add($"   {LR[63]}   "/*Информация*/, new Bitmap(1, 1), $"{LR[83]/*Оборона обескровлена на*/}/{(int)percent_deff}%");

                    //ТЕСТ сноса деревни
                    //if (LinkVillage[1] != null) LinkVillage[1].Population = 0;

                    //удаление самой деревни происходит в потоке таймера событий в момент генерации отчёта
                    if (report.Attack_Troops[7]/*катапульты атакующего*/ > 0 && report.TypeEvent == Type_Event.ATTACK) 
                        if (LinkVillage[1] == null || LinkVillage[1].Population <= 0) {
                            //если условия соблюдены, значит это атакующий виновник уничтожения деревни,
                            //в противном случае войска поцелуют руины, развернутся домой, а отчёт не сгенерируется
                            _.Rows.Add($"   {LR[63]}   "/*Информация*/, new Bitmap(1, 1), LR[88]/*Деревня уничтожена полностью*/);
                    }

                    //заполняем только те таблицы, где есть подкрепы
                    for (int i = 0; i < grid_Head_Reinforcements.Length; i++) {
                        _ = grid_Head_Reinforcements[i];
                        //проверка перед заполнением таблиц подкрепов 
                        if (Player.Nick_Name == AccountName[0]) {
                            //выход из цикла перед заполнением таблиц подкрепов если никто не выжил у Player
                            if (!GAME.IsNotZero_Array(report.Units)) if (percent_deff < 25) break;
                        }
                        if (GAME.IsNotZero_Array(report.Defense_Troops, i * 11, 11)) {
                            _.Rows.Add($" {LANGUAGES.Reports[49]/*Оборона*/} ", $" {LANGUAGES.Reports[50]/*Подкрепление*/}");
                            if (i <= 2) _.Rows[0].Cells[1].ToolTipText = LANGUAGES.RESOURSES[i + 21];//названия нации в подкрепе [игровые]
                            else _.Rows[0].Cells[1].ToolTipText = LANGUAGES.RESOURSES[i + 140];//названия нации в подкрепе [природа/натары]
                        }
                    }
                }

                //Информация о присланном подкрепе
                if (report.TypeReport == Type_Report.Reinforcement_Arrived) {
                    string Name; if (LinkVillage[1] != null) Name = VillageName[1]; else Name = LANGUAGES.RESOURSES[9]/*Оазис*/;
                    grid_Tabel_Information[0].Rows.Add($"   {LR[63]}   "/*Информация*/, new Bitmap(1, 1),
                        $"{Name} ({GAME.Map.Coord_MapToWorld(report.Cell_Finish).X} | {GAME.Map.Coord_MapToWorld(report.Cell_Finish).Y}) " +
                        $"{LR[77]/*получил подкрепление от*/} {VillageName[0]} " +
                        $"({LinkVillage[0].Coordinates_World_Travian.X} | {LinkVillage[0].Coordinates_World_Travian.Y})");
                    grid_Tabel_Army_TXT[0].Rows.RemoveAt(1); 
                }
                //Информация об основании нового поселения
                if (report.TypeReport == Type_Report.SettlersCreateNewVillage) {
                    grid_Tabel_Army_TXT[0].Rows.RemoveAt(1);
                    string coordinatesNewVillage = LinkVillage[1] == null ? "(? | ?)" : $"{LinkVillage[1].Coordinates_World_Travian.X} | {LinkVillage[1].Coordinates_World_Travian.Y}";
                    grid_Tabel_Information[0].Rows.Add($"   {LR[63]}   "/*Информация*/, IMG[0][9]/*поселенцы*/,
                        $"{VillageName[0]} ({LinkVillage[0].Coordinates_World_Travian.X} | {LinkVillage[0].Coordinates_World_Travian.Y}) " +
                        $"{LR[58]/*основал поселение*/} '{VillageName[1]}' ({coordinatesNewVillage})");
                    grid_Tabel_Information[0].Rows.Add($"   {LR[63]}   "/*Информация*/, new Bitmap(1, 1),
                        $"{LR[82]/*Тип ресурсных полей деревни*/}: {GAME.TypeCellToString(ReportCell[1].TypeResoueces, LANGUAGES)}");
                    grid_Tabel_Information[0].Rows.Add($"   {LR[63]}   "/*Информация*/, new Bitmap(1, 1),
                        $"{VillageName[1]} ({coordinatesNewVillage}): {LR[80]/*в хранилища поступили ресурсы*/}");
                    for (int i = 0; i < img_Resources.Length; i++) img_Resources[i].Visible = true;
                    for (int i = 0; i < txt_Resources.Length; i++) txt_Resources[i].Visible = true;
                    img_Resources[img_Resources.Length - 1].Tag = true;//костыльный флаг true = объект видим
                    var _IS = img_Resources; var _TS = txt_Resources;
                    _IS[5].BackgroundImage = Farm(report, out string farm); _IS[5].Size = _IS[5].BackgroundImage.Size;
                    for (int i = 0; i < _TS.Length - 2; i++) _TS[i].Text = report.Resources[i].ToString();
                    _TS[5].Text = farm; _TS[6].Text = LANGUAGES.Reports[78]/*Доставлено*/;
                }
            }

            //Информация о торговле
            int sizeX = ToCSR(32); int sizeY = ToCSR(22);
            if (report.TypeReport >= Type_Report.Mostly_wood && report.TypeReport <= Type_Report.Mostly_gold) {
                var path = "DATA_BASE/IMG/pictograms/resources";
                int N; if (report.TypeReport != Type_Report.Mostly_gold) N = (int)report.TypeReport - 16; else N = (int)report.TypeReport - 15;
                grid_Tabel_Information[0].Rows.Add($"   {LANGUAGES.Reports[63]}   "/*Информация*/,
                    Extensions.ResizeImage(Image.FromFile($"{path}/0{N}.png"), sizeX, sizeY),
                    $"{LANGUAGES.Reports[17]/*Торговец доставил в основном*/} {LANGUAGES.Reports[(int)report.TypeReport + 2]/*ресурс*/}");
                for (int i = 0; i < img_Resources.Length; i++) img_Resources[i].Visible = true;
                for (int i = 0; i < txt_Resources.Length; i++) txt_Resources[i].Visible = true;
                img_Resources[5].Visible = txt_Resources[6].Visible = false;
                img_Resources[img_Resources.Length - 1].Tag = true;//костыльный флаг true = объект видим
                var _IS = img_Resources; var _TS = txt_Resources;
                _IS[5].BackgroundImage = new Bitmap(1, 1); _IS[5].Size = _IS[5].BackgroundImage.Size;
                for (int i = 0; i < _TS.Length - 2; i++) _TS[i].Text = report.Resources[i].ToString();
                _TS[5].Text = ""; _TS[6].Text = LANGUAGES.Reports[78]/*Доставлено*/;

                _ = grid_Tabel_Information[1];

            }

            //блок подгона габаритов Grid
            grid_Head.UpdateSize();
            grid_Account[0].UpdateSize(); grid_Account[1].UpdateSize();
            for (int i = 0; i < grid_Tabel_Army_PIC.Length; i++) grid_Tabel_Army_PIC[i].UpdateSize();
            for (int i = 0; i < grid_Tabel_Army_TXT.Length; i++) grid_Tabel_Army_TXT[i].UpdateSize();
            grid_Tabel_Information[0].UpdateSize(); grid_Tabel_Information[1].UpdateSize();
            for (int i = 0; i < grid_Head_Reinforcements.Length; i++) grid_Head_Reinforcements[i].UpdateSize();

            //поиск максимальной ширины ячейки всех таблиц с текстом величин войск Start + Finish + подкрепы
            int maxW = 0; int max_w0 = 0;
            if (grid_Tabel_Army_PIC[0].Rows.Count > 0 && grid_Tabel_Army_TXT[0].Rows.Count > 0) {
                int MAX_W = 0; int MAX_W0 = 0; var _PIC = grid_Tabel_Army_PIC; var _TXT = grid_Tabel_Army_TXT;
                for (int i = 0; i < _TXT.Length; i++) {
                    if (_TXT[i].Rows.Count <= 0) continue;
                    if (MAX_W0 < _TXT[i].Rows[0].Cells[0].PreferredSize.Width) MAX_W0 = _TXT[i].Rows[0].Cells[0].PreferredSize.Width;
                    for (int y = 0; y < _TXT[0].Rows.Count; y++) for (int x = 0; x < _TXT[0].Rows[y].Cells.Count; x++) {
                        int width = _TXT[i].Rows[y].Cells[x].PreferredSize.Width; if (MAX_W < width) MAX_W = width;
                    }
                    int summ_w = MAX_W0 + MAX_W * _PIC[0].Columns.Count + 4;
                    for (int y = 0; y < _PIC[0].Rows.Count; y++) for (int x = 0; x < _PIC[0].Rows[y].Cells.Count; x++) {
                        _PIC[i].Columns[x].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;//0 = Fill
                        _TXT[i].Columns[x].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;//0 = Fill
                        _PIC[i].Columns[x].Width = MAX_W; _PIC[i].Columns[0].Width = MAX_W0 + MAX_W;
                        _TXT[i].Columns[x].Width = MAX_W; _TXT[i].Columns[0].Width = MAX_W0 + MAX_W;
                    }
                    _PIC[i].Size = new Size(summ_w, _PIC[i].Height); _TXT[i].Size = new Size(summ_w, _TXT[i].Height);
                }

                //удаляем пустые таблицы
                var _1 = grid_Tabel_Army_PIC; var _2 = grid_Tabel_Army_TXT;
                for (int i = 2; i < 7; i++) { int pos = (i - 2) * report.Attack_Troops.Length;
                    if (!GAME.IsNotZero_Array(report.Defense_Troops, pos, 11)) { _1[i].Rows.Clear(); _2[i].Rows.Clear(); }
                }
                maxW = MAX_W; max_w0 = MAX_W0;
            }

            //локации таблиц атакующей стороны
            int Left = grid_Head.Left; int MinHeight = 0;
            var img = img_Resources[img_Resources.Length - 1]; var inf = grid_Tabel_Information;
            var TXT = grid_Tabel_Army_TXT; var PIC = grid_Tabel_Army_PIC; var Acc = grid_Account;
            var Reinf = grid_Head_Reinforcements;
            //grid_Head.Location = описан вверху при создании таблички
            Acc[0].Location = new Point(Left, grid_Head.Top + grid_Head.Height + Tab_Y);
            PIC[0].Location = new Point(Acc[0].Left, Acc[0].Top + Acc[0].Height);
            if ((PIC[0].Rows.Count <= 0) || (report.TypeReport >= Type_Report.Mostly_wood && report.TypeReport <= Type_Report.Mostly_gold))
                PIC[0].Height = MinHeight; else PIC[0].Top += Tab_y;
            TXT[0].Location = new Point(PIC[0].Left, PIC[0].Top + PIC[0].Height);
            if ((TXT[0].Rows.Count <= 0) || (report.TypeReport >= Type_Report.Mostly_wood && report.TypeReport <= Type_Report.Mostly_gold)) 
                TXT[0].Height = MinHeight; else TXT[0].Top += 0;
            inf[0].Location = new Point(TXT[0].Left, TXT[0].Top + TXT[0].Height);

            //локации контролов
            var IS = img_Resources; var TS = txt_Resources;
            var grid = grid_Tabel_Information[0];//таблица от которой высчитывается локация
            int left = grid.Left + max_w0 + maxW + Tab_x;    int Itop = grid.Top + grid.Height + Tab_Y;
            int delta = (IS[0].Height - TS[0].Height) / 2;    int Ttop = Itop + delta;
            for (int i = 0; i < IS.Length - 1; i++) {
                IS[i].Location = new Point(left, Itop); TS[i].Location = new Point(left + IS[i].Width + Tab_x, Ttop);//пара
                left = TS[i].Left + TS[i].Width + Tab_X;
            }
            delta = (IS[5].Height - TS[5].Height) / 2;
            IS[5].Location = new Point(IS[0].Left, IS[0].Top + IS[0].Height + Tab_Y / 2);
            TS[5].Location = new Point(IS[5].Left + IS[5].Width + Tab_x, IS[0].Top + IS[0].Height + Tab_Y / 2 + delta);
            left = grid.Left + ((max_w0 + maxW) - TS[6].Width) / 2;
            delta = ((IS[5].Top + IS[5].Height) - (grid.Top + grid.Height));
            var top = (grid.Top + grid.Height) + (delta - TS[6].Height) / 2;
            TS[6].Location = new Point(left, top);

            //локации таблиц обороняющейся стороны
            if ((bool)img.Tag) Acc[1].Location = new Point(Left, img.Top + img.Height + Tab_Y);
            else if (inf[0].Rows.Count > 0) Acc[1].Location = new Point(Left, inf[0].Top + inf[0].Height + Tab_Y);
            else /*if (TXT[0].Rows.Count > 0)*/Acc[1].Location = new Point(Left, TXT[0].Top + TXT[0].Height + Tab_Y);
            PIC[1].Location = new Point(Left, Acc[1].Top + Acc[1].Height);
            if (PIC[1].Rows.Count <= 0 || report.TypeReport == Type_Report.Reinforcement_Arrived ||
                report.TypeReport == Type_Report.SettlersCreateNewVillage)
                PIC[1].Height = MinHeight; else PIC[1].Top += Tab_y;
            TXT[1].Location = new Point(Left, PIC[1].Top + PIC[1].Height);
            if (TXT[1].Rows.Count <= 0 || report.TypeReport == Type_Report.Reinforcement_Arrived ||
                report.TypeReport == Type_Report.SettlersCreateNewVillage)
                TXT[1].Height = MinHeight; else TXT[1].Top += 0;
                inf[1].Location = new Point(Left, TXT[1].Top + TXT[1].Height);
            if (inf[1].Rows.Count <= 0) inf[1].Height = MinHeight; else inf[1].Top += Tab_y;
            for (int i = 2; i < TXT.Length; i++) {
                if (inf[1].Rows.Count > 0 && i == 2) Reinf[i - 2].Location = new Point(Left, inf[1].Top + inf[1].Height);
                else Reinf[i - 2].Location = new Point(Left, TXT[i - 1].Top + TXT[i - 1].Height);
                if (TXT[i].Rows.Count <= 0) Reinf[i - 2].Height = MinHeight; else Reinf[i - 2].Top += Tab_Y;
                PIC[i].Location = new Point(Left, Reinf[i - 2].Top + Reinf[i - 2].Height);
                if (PIC[i].Rows.Count <= 0) PIC[i].Height = MinHeight; else PIC[i].Top += Tab_y; 
                TXT[i].Location = new Point(Left, PIC[i].Top + PIC[i].Height);
                if (TXT[i].Rows.Count <= 0) TXT[i].Height = MinHeight; else TXT[i].Top += 0;
            }

            //выравнивание таблиц по ширине 
            int Max_W = grid_Head.Width;
            if (Max_W < grid_Account[0].Width) Max_W = grid_Account[0].Width;
            for (int i = 0; i < grid_Tabel_Army_PIC.Length; i++) {
                if (Max_W < grid_Tabel_Army_PIC[i].Width) Max_W = grid_Tabel_Army_PIC[i].Width;
                if (Max_W < grid_Tabel_Army_TXT[i].Width) Max_W = grid_Tabel_Army_TXT[i].Width;
            }
            if (Max_W < grid_Tabel_Information[0].Width) Max_W = grid_Tabel_Information[0].Width;
            if (Max_W < grid_Tabel_Information[1].Width) Max_W = grid_Tabel_Information[1].Width;
            if (Max_W < grid_Account[1].Width) Max_W = grid_Account[1].Width;
            for (int i = 0; i < grid_Head_Reinforcements.Length; i++)
                if (Max_W < grid_Head_Reinforcements[i].Width) Max_W = grid_Head_Reinforcements[i].Width;
            grid_Head.Width = grid_Account[0].Width = grid_Tabel_Information[0].Width = grid_Tabel_Information[1].Width =
                grid_Account[1].Width = Max_W;
            for (int i = 0; i < grid_Tabel_Army_PIC.Length; i++) {
                grid_Tabel_Army_PIC[i].Width = grid_Tabel_Army_TXT[i].Width = Max_W;
            }
            for (int i = 0; i < grid_Head_Reinforcements.Length; i++) grid_Head_Reinforcements[i].Width = Max_W;

            //выделение снимается моим методом, дефолтный .ClearSelection(); не работает. КОСТЫЛЬ!
            grid_Head.CancelSelection();                 grid_Account[0].CancelSelection();
            grid_Tabel_Information[0].CancelSelection(); grid_Tabel_Information[1].CancelSelection();
            grid_Account[1].CancelSelection();
            for (int i = 0; i < grid_Head_Reinforcements.Length; i++) grid_Head_Reinforcements[i].CancelSelection();
            for (int i = 0; i < grid_Tabel_Army_PIC.Length; i++) {
                grid_Tabel_Army_PIC[i].CancelSelection(); grid_Tabel_Army_TXT[i].CancelSelection();
            }
            //возврат габаритов
            grid_Account[0].UpdateSize(); grid_Account[1].UpdateSize();
            for (int i = 0; i < grid_Head_Reinforcements.Length; i++) grid_Head_Reinforcements[i].UpdateSize();
            
            if (Form_Multi_Reports.VerticalScroll.Visible) {
                Form_Multi_Reports.Padding = new Padding(5, 5, Tab_Y, Tab_Y); }

            report.Read = true;//помечаем отчёт как прочитанный
            Form_Multi_Reports.Focus();
            if (!Form_Multi_Reports.Visible) Form_Multi_Reports.ShowDialog();
        }
        //=============================================== ДИАЛОГОВОЕ ОКНО :: ОТЧЁТЫ ==============================================

        //============================================== ДИАЛОГОВОЕ ОКНО :: СООБЩЕНИЯ ============================================
        /// <summary> Окно "Чтения/Написания сообщения" </summary>
        private Form Form_Message = null;
        /// <summary> Задний фон, не котором расположен заголовок, сообщение и кнопки. </summary>
        private PictureBox BackGround_Message = null;
        /// <summary> Иконка удаления сообщения. Картинка имеет форму: "линия + иконка удаления" - полоска. </summary>
        private PictureBox ICO_DeleteMessage = null;
        /// <summary> Кнопка-пиктограмма над иконкой удаления сообщения. </summary>
        private Button btn_DeleteMessage = null;
        /// <summary> Таблица заголовка сообщения (Отправитель:, Тема:, Дата:). </summary>
        private DataGridView TableHead_Message = null;
        private Label Text_Message_Read = null;
        /// <summary> Кнопка "Пометить непрочитанным". </summary>
        private Button btn_MarkAsNotRead_Message = null;
        /// <summary> Кнопка "Ответить". </summary>
        private Button btn_Answer_Message = null;
        /// <summary> Таблица заголовка для написания сообщения (Получатель:, Тема:). </summary>
        private DataGridView TableHead_MessageWrite = null;
        /// <summary> Многострочный редактор ввода текста сообщения. </summary>
        private TextBox Text_Message_Write = null;
        /// <summary> Кнопка "Отправить" сообщение. </summary>
        private Button btn_Send = null;
        /// <summary> Флаг кнопки "Пометить сообщение непрочитанным". <br/> <b>true</b> = сообщение помечено как прочитанное, <br/> <b>false</b> = сообщение помечено как не прочитанное. <br/> Значение по умолчанию = <b>true</b>. </summary>
        private bool MarkAsRead = true;
        /// <summary> Номер строки в таблице <b>grid_Messages.</b> Переменная нужна для нахождения строки в таблице и <b>GAME.Messages.LIST[n];</b> при удалении сообщения. </summary>
        private int MessageIndex = -1;
        /// <summary> Координаты стартовой деревни получателя сообщения в системе координат <b>Map.Cell[x][y];</b> </summary>
        private Point Location_Recipient;
        /// <summary> Массив координат стартовых деревень всех ботов из листа <b>BotList[n]</b> в системе координат <b>Map.Cell[x][y];</b> </summary>
        private Point[] Array_LocationRecepients = null;
        /// <summary> Глубина переписки текущего сообщения. </summary>
        private int DepthMessage = -1;
        /// <summary> Тема текущего сообщения. </summary>
        private string TopicMessage = "";

        /// <summary> Метод создаёт многострочный редактор ввода текста сообщения. <br/> Этот метод предварительного создания контрола понадобился для клика по кнопке "Написать сообщение" в окне профиля, чтобы была возможность передать в него заготовку: "кто пишет". </summary>
        private void Create_Text_Message_Write(string FName = "Arial", float FSize = 10) {
            if (Text_Message_Write == null) {
                Text_Message_Write = new TextBox { Name = "Text_Message_Write", Font = new Font(FName, ToCSR(FSize), FontStyle.Regular),
                    ForeColor = Color.Black, BackColor = Color.FromArgb(248, 248, 248), ScrollBars = ScrollBars.Both,
                    BorderStyle = BorderStyle.None, Multiline = true,
                };
            }
        }

        /// <summary> Метод редактирует выпадающий список в таблице заголовка написания сообщения с именами всех игроков из списка <b>ListBox[n];</b>. </summary>
        private void TableHead_MessageWrite_EditingComboBoxText(object sender, DataGridViewEditingControlShowingEventArgs e) {
            if (!(sender is DataGridView grid)) return; if (grid.CurrentCell.ReadOnly) return;
            if (grid.CurrentCell == grid[1, 0]) { var cb = (DataGridViewComboBoxEditingControl)e.Control;
                cb.MaxLength = GAME.Name_MaxLength; cb.FlatStyle = FlatStyle.Popup; cb.DropDownStyle = ComboBoxStyle.DropDown;
                //фильтрация автозаполения поля Text
                cb.AutoCompleteSource = AutoCompleteSource.ListItems; cb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            }
        }

        /// <summary> <inheritdoc cref="DataGridView.CellValueChanged"/> <br/> Метод нужен для смены координат столицы аккаунта-получателя сообщения в системе координат <b>Map.Cell[x][y];</b> </summary>
        private void TableHead_MessageWrite_CellValueChanged(object sender, EventArgs e) {
            if (!(sender is DataGridView grid)) return; if (!(grid[1, 0] is DataGridViewComboBoxCell ComboBox)) return;
            if (grid.CurrentCell.ReadOnly || ComboBox.ReadOnly) return;
            int index = ComboBox.Items.IndexOf(grid[1, 0].Value.ToString());
            Location_Recipient = index > -1 ? Array_LocationRecepients[index] : new Point(-1, -1);
        }

        /// <summary> Метод обрабатывает клик по содержимому ячеек таблицы <b>TableHead_Message.</b> </summary>
        private void TableHead_Message_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex < 0) return; if (!(sender is DataGridView grid)) return;
            var Cell = grid[1, 0].Tag as TMap.TCell;
            if (e.ColumnIndex == 1) {
                //открыть окно аккаунта отправителя сообщения
                if (Form_AccountProfile != null && Form_AccountProfile.Visible) Form_AccountProfile.Close();
                if (Cell.LinkAccount != null) WinDlg_AccountProfile(Cell.LinkAccount);
            }
        }
        /// <summary> Метод обрабатывает наведение на ячейку <b>TableHead_Message.</b> </summary>
        private void TableHead_Message_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex == 0 && e.ColumnIndex == 1) {
                ((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Red;
            }
        }
        /// <summary> Метод обрабатывает выход за пределы ячейки <b>TableHead_Message.</b> </summary>
        private void TableHead_Message_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex == 0 && e.ColumnIndex == 1) {
                ((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Style.ForeColor = grid_Messages_Cell_3_Color_Read_true;
            }
        }

        public void WinDlg_Message(int index = -1, TMessage.TData message = null, Window_Message WindowMessage = Window_Message.Read) {
            float FSize = ToCSR(10);/*размер шрифта текста*/ string FName = "Arial";/*имя шрифта текста*/
            if (Form_Message == null) {
                if (Array_LocationRecepients != null) Array_LocationRecepients = null;
                Array_LocationRecepients = new Point[BotList.Count];
                //ФОРМА
                Form_Message = Extensions.CreateForm("Form_Message", new Icon("DATA_BASE/IMG/logotip.ico"),
                    new Font(Font.FontFamily, FSize, FontStyle.Bold), LANGUAGES.Messages[0]/*Сообщения*/,
                    StartPosition: FormStartPosition.CenterParent, FormBorderStyle: FormBorderStyle.FixedSingle,
                    ControlBox: true, KeyPreview: true, AutoSize: true, AutoScroll: true);
                Form_Message.BackColor = Color.FromArgb(248, 248, 248);
                Form_Message.MaximumSize = new Size(ScreenBounds_Size().Width, (int)(ScreenBounds_Size().Height * 0.8));//y:864
                Form_Message.HorizontalScroll.Visible = false;
                Form_Message.KeyUp += new KeyEventHandler(KeyUp_EscapeFormClose);
                //КАРТИНКА-ФОН
                BackGround_Message = new PictureBox { Parent = Form_Message, BackgroundImageLayout = ImageLayout.Stretch,
                    //Size.Height должен быть МЕНЬШЕ ЧЕМ MinimumSize.Height !!!!!!!!!!!!!!!!!!!!!!!!!!
                    Location = new Point(ToCSR(10), ToCSR(10)), Size = new Size(ToCSR(600), ToCSR(750)), MinimumSize = new Size(ToCSR(600), ToCSR(800)),
                };
                BackGround_Message.BackgroundImage = Image.FromFile("DATA_BASE/IMG/Messages/bg_Message.png");
                //ИКОНКА УДАЛЕНИЯ СООБЩЕНИЯ
                ICO_DeleteMessage = new PictureBox { Parent = BackGround_Message, BackgroundImageLayout = ImageLayout.Stretch, };
                ICO_DeleteMessage.BackgroundImage = Image.FromFile("DATA_BASE/IMG/Messages/delete_line.png");
                //КНОПКА УДАЛЕНИЯ СООБЩЕНИЯ
                btn_DeleteMessage = new Button { Parent = ICO_DeleteMessage, Name = "btn_DeleteMessage",
                    Font = new Font(FName, FSize, FontStyle.Bold), ForeColor = Color.Black, BackColor = Color.Transparent, AutoSize = false,
                    BackgroundImageLayout = ImageLayout.Stretch, FlatStyle = FlatStyle.Flat, 
                };
                btn_DeleteMessage.FlatAppearance.BorderSize = 0;
                btn_DeleteMessage.BackgroundImage = Image.FromFile("DATA_BASE/IMG/pictograms/ico/delete_message.png");
                btn_DeleteMessage.Click += Control_Click;
                btn_DeleteMessage.MouseEnter += Control_MouseEnter; btn_DeleteMessage.MouseLeave += Control_MouseLeave;

                //программное создание таблицы заголовка сообщения
                TableHead_Message = Extensions.CreateGrid(BackGround_Message, "TableHead_Message", new Font(FName, FSize + 2, FontStyle.Regular),
                    new Point(ToCSR(40), ToCSR(60)), Color.FromArgb(248, 248, 248), Color.FromArgb(248, 248, 248), Enabled: true);
                TableHead_Message.Columns.AddRange(new DataGridViewColumn[] {
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                });
                TableHead_Message.DefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);//цвет фона каждой ячейки
                TableHead_Message.DefaultCellStyle.SelectionBackColor = TableHead_Message.DefaultCellStyle.BackColor;
                TableHead_Message.DefaultCellStyle.SelectionForeColor = TableHead_Message.DefaultCellStyle.ForeColor;
                TableHead_Message.Columns[0].DefaultCellStyle.Font = new Font(FName, FSize + 2, FontStyle.Regular);
                TableHead_Message.Columns[1].DefaultCellStyle.Font = new Font(FName, FSize + 2, FontStyle.Regular);
                TableHead_Message.Columns[0].AutoSizeMode = TableHead_Message.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                TableHead_Message.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                TableHead_Message.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                TableHead_Message.CellContentClick += TableHead_Message_CellContentClick;
                TableHead_Message.CellMouseEnter += TableHead_Message_CellMouseEnter;
                TableHead_Message.CellMouseLeave += TableHead_Message_CellMouseLeave;

                //ТЕКСТ СООБЩЕНИЯ
                Text_Message_Read = new Label { Parent = BackGround_Message, AutoSize = false, Font = new Font(FName, FSize, FontStyle.Regular),
                    ForeColor = Color.Black, BackColor = Color.Transparent,
                };
                //КНОПКА "ПОМЕТИТЬ НЕПРОЧИТАННЫМ"
                btn_MarkAsNotRead_Message = new Button { Parent = BackGround_Message, Name = "btn_MarkAsNotRead_Message",
                    Font = new Font(FName, FSize, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.OliveDrab,
                    AutoSize = true, Padding = new Padding(ToCSR(20), 5, ToCSR(20), 5),
                }; btn_MarkAsNotRead_Message.Click += Control_Click;
                //КНОПКА "ОТВЕТИТЬ"
                btn_Answer_Message = new Button { Parent = BackGround_Message, Name = "btn_Answer_Message",
                    Font = new Font(FName, FSize, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.OliveDrab,
                    AutoSize = true, Padding = new Padding(ToCSR(20), 5, ToCSR(20), 5),
                }; btn_Answer_Message.Click += Control_Click;

                //программное создание таблицы заголовка написания сообщения
                TableHead_MessageWrite = Extensions.CreateGrid(BackGround_Message, "TableHead_MessageWrite", new Font(FName, FSize + 2, FontStyle.Regular),
                    new Point(ToCSR(40), ToCSR(80)), Color.FromArgb(248, 248, 248), Color.FromArgb(223, 223, 223), ReadOnly: false, Enabled: true);
                TableHead_MessageWrite.Columns.AddRange(new DataGridViewColumn[] {
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable, MaxInputLength = GAME.Name_MaxLength, },
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable, MaxInputLength = GAME.Topic_MaxLength, },
                });
                TableHead_MessageWrite.DefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);//цвет фона каждой ячейки
                TableHead_MessageWrite.DefaultCellStyle.SelectionBackColor = TableHead_MessageWrite.DefaultCellStyle.BackColor;
                TableHead_MessageWrite.DefaultCellStyle.SelectionForeColor = TableHead_MessageWrite.DefaultCellStyle.ForeColor;
                TableHead_MessageWrite.Columns[0].DefaultCellStyle.Font = new Font(FName, FSize + 2, FontStyle.Regular);
                TableHead_MessageWrite.Columns[1].DefaultCellStyle.Font = new Font(FName, FSize + 2, FontStyle.Regular);
                TableHead_MessageWrite.Columns[1].DefaultCellStyle.ForeColor = Color.Blue;
                TableHead_MessageWrite.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                TableHead_MessageWrite.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                TableHead_MessageWrite.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                TableHead_MessageWrite.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                TableHead_MessageWrite.MinimumSize = new Size(ToCSR(500), ToCSR(50));
                TableHead_MessageWrite.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
                TableHead_MessageWrite.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                TableHead_MessageWrite.EditingControlShowing += TableHead_MessageWrite_EditingComboBoxText;
                TableHead_MessageWrite.CellValueChanged += TableHead_MessageWrite_CellValueChanged;

                //МНОГОСТРОЧНЫЙ РЕДАКТОР ВВОДА ТЕКСТА СООБЩЕНИЯ
                Create_Text_Message_Write(FName, FSize); Text_Message_Write.Parent = BackGround_Message;

                //КНОПКА "ОТПРАВИТЬ" СООБЩЕНИЕ
                btn_Send = new Button { Parent = BackGround_Message, Name = "btn_Send", Font = new Font(FName, FSize, FontStyle.Bold),
                    ForeColor = Color.White, BackColor = Color.OliveDrab, AutoSize = true, Padding = new Padding(ToCSR(20), 5, ToCSR(20), 5),
                }; btn_Send.Click += Control_Click;
            }
            //default
            MarkAsRead = true; MessageIndex = index;

            if (WindowMessage == Window_Message.Read) { //показать окно в режиме ЧТЕНИЯ сообещния
                //показываем контролы чтения сообщения
                TableHead_Message.Visible = Text_Message_Read.Visible = ICO_DeleteMessage.Visible = btn_DeleteMessage.Visible =
                btn_MarkAsNotRead_Message.Visible = btn_Answer_Message.Visible = true;
                //скрываем контролы записи сообщения
                TableHead_MessageWrite.Visible = Text_Message_Write.Visible = btn_Send.Visible = false;
                //запрет написания сообщения самому себе
                var btn = btn_Answer_Message;
                if (Location_Recipient == Player.VillageList[0].Coordinates_Cell) { btn.Enabled = false; btn.BackColor = Color.FromArgb(128, 128, 128); btn.ForeColor = Color.Black; }
                else { btn.Enabled = true; btn.BackColor = Color.OliveDrab; btn.ForeColor = Color.White; }
                /*кнопка "удалить сообщение"*/tool_tip[33] = Extensions.CreateHint(btn_DeleteMessage, 0, 30000, LANGUAGES.tool_tip_TITLE[33], LANGUAGES.tool_tip_TEXT[33], Color.GreenYellow, Color.DarkGreen, ToolTipIcon.Info, false);
                btn_MarkAsNotRead_Message.Text = LANGUAGES.Messages[15];/*Отметить непрочитанным*/
                btn_Answer_Message.Text = LANGUAGES.Messages[18];/*Ответить*/

                //Добавление заголовка в таблицу
                var _ = TableHead_Message; _.Rows.Clear();
                string NickName = GAME.Map.Cell[message.Cell_Start.X, message.Cell_Start.Y].LinkAccount.Nick_Name;
                string DateTime = message.Date + " " + LANGUAGES.Reports[47]/*в*/ + " " + message.Time;
                Location_Recipient = message.Cell_Start;
                _.Rows.Add(LANGUAGES.Messages[14]/*Отправитель:*/, NickName);
                _.Rows.Add(LANGUAGES.Messages[9]/*Тема:*/, $"{message.GetRE()} {message.Topic}");
                _.Rows.Add(LANGUAGES.Messages[10]/*Дата:*/, DateTime);
                _[1, 0].Tag = GAME.Map.Cell[message.Cell_Start.X, message.Cell_Start.Y];//записываем ячейку (источник генерации сообщения)
                Text_Message_Read.Text = message.Text;//Добавление текста сообщения
                //форматирвоание строк таблицы (зелёный никнэйм отправителя)
                var grid_Style_01 = _.Rows[0].Cells[1].Style; var grid_Style_11 = _.Rows[1].Cells[1].Style;
                grid_Style_01.ForeColor = grid_Messages_Cell_3_Color_Read_true; grid_Style_01.Font = new Font(FName, FSize + 2, FontStyle.Bold);
                grid_Style_11.ForeColor = Color.Black;                          grid_Style_11.Font = new Font(FName, FSize + 2, FontStyle.Bold);

                TableHead_Message.UpdateSize();//подгон габаритов таблицы под её содержимое
                Text_Message_Read.Location = new Point(ToCSR(50), TableHead_Message.Bottom + ToCSR(60));
                Text_Message_Read.Size = TextRenderer.MeasureText(message.Text, Text_Message_Read.Font);
                int MAX_W = TableHead_Message.Right + ToCSR(100) > Text_Message_Read.Right + Text_Message_Read.Left ? 
                    TableHead_Message.Right + ToCSR(100) : Text_Message_Read.Right + Text_Message_Read.Left;
                BackGround_Message.Width = MAX_W > BackGround_Message.MinimumSize.Width ? MAX_W : BackGround_Message.MinimumSize.Width;
                BackGround_Message.Height = Text_Message_Read.Bottom + ToCSR(110);
                int H = (int)(BackGround_Message.Width / 20.0); int tab_y = (int)(BackGround_Message.Height / 16.5);
                //ICO_DeleteMessage.BackgroundImage = Image.FromFile("DATA_BASE/IMG/Messages/delete_line.png");
                ICO_DeleteMessage.Size = new Size((int)(BackGround_Message.Right / 1.146), H);
                ICO_DeleteMessage.Location = new Point((int)(BackGround_Message.Right / 18.58), TableHead_Message.Bottom);
                btn_DeleteMessage.Size = new Size(ICO_DeleteMessage.Height - 4, ICO_DeleteMessage.Height - 4);
                btn_DeleteMessage.Location = new Point(ICO_DeleteMessage.Width - btn_DeleteMessage.Width, 0);
                btn_MarkAsNotRead_Message.Location = new Point(ICO_DeleteMessage.Left,
                        BackGround_Message.Left + BackGround_Message.Height - btn_MarkAsNotRead_Message.Height - tab_y);
                btn_Answer_Message.Location = new Point(ICO_DeleteMessage.Right - btn_Answer_Message.Width, btn_MarkAsNotRead_Message.Top);
                
                Form_Message.AutoSize = true;
                Messages_ReadNotRead(MarkAsRead, index);//помечаем выбранное сообщение как прочитанное/не прочитанное
                message.Read = MarkAsRead;//помечаем сообщение как прочитанное/не прочитанное

                Form_Message.Padding = Form_Message.VerticalScroll.Visible ? 
                    new Padding(10, 10, SystemInformation.VerticalScrollBarWidth, 10) : new Padding(10, 10, 10, 10);
            } else { //показать окно в режиме ЗАПИСИ сообещния
                //показываем контролы записи сообщения
                TableHead_MessageWrite.Visible = Text_Message_Write.Visible = btn_Send.Visible = ICO_DeleteMessage.Visible = true;
                //скрываем контролы чтения сообщения
                TableHead_Message.Visible = Text_Message_Read.Visible = btn_DeleteMessage.Visible =
                btn_MarkAsNotRead_Message.Visible = btn_Answer_Message.Visible = false;

                btn_Send.Text = LANGUAGES.Messages[21];/*Отправить*/

                //Добавление заголовка в таблицу
                var _ = TableHead_MessageWrite; _.Rows.Clear(); 
                DataGridViewComboBoxCell cbCell = new DataGridViewComboBoxCell { FlatStyle = FlatStyle.Popup, ReadOnly = false,
                    AutoComplete = true,
                };
                for (int i = 0; i < BotList.Count; i++) {
                    cbCell.Items.Add(BotList[i].Nick_Name);
                    Array_LocationRecepients[i] = BotList[i].VillageList[0].Coordinates_Cell;
                }
                cbCell.Value = Location_Recipient.X > -1 ? 
                    GAME.Map.Cell[Location_Recipient.X, Location_Recipient.Y].LinkAccount.Nick_Name : "";
                _.Rows.Add(LANGUAGES.Messages[20]/*Получатель:*/);
                string RE = message != null ? message.GetRE(message.Depth + 1) : ""; 
                TopicMessage = message != null ? message.Topic : "";
                DepthMessage = message != null ? message.Depth : -1;
                _.Rows.Add(LANGUAGES.Messages[9]/*Тема:*/, $"{RE} {TopicMessage}");
                _[1, 0] = cbCell;

                //при написании сообщения по кнопке "написать", можно редактировать все поля этой таблицы
                if (message == null && Location_Recipient.X <= -1) { _[1, 0].ReadOnly = false; _[1, 1].ReadOnly = false; }
                //при ответе нельзя редактировать эту таблицу
                else if (message != null) { _[1, 0].ReadOnly = true; _[1, 1].ReadOnly = true;
                    _.CurrentCell = _.Rows[1].Cells[1];/*ставим фокус ввода на ячейку с вводом темы сообщения*/ }
                //при написании сообщения из окна профиля, нельзя редактировать поле таблицы "ПОЛУЧАТЕЛЬ"
                else if (message == null && Location_Recipient.X >= 0) { _[1, 0].ReadOnly = true; _[1, 1].ReadOnly = false; }

                TableHead_MessageWrite.UpdateSize();//подгон габаритов таблицы под её содержимое
                BackGround_Message.Width = TableHead_MessageWrite.Right + ToCSR(120);
                BackGround_Message.Height = BackGround_Message.MinimumSize.Height;
                int H = (int)(BackGround_Message.Width / 20.0); int tab_y = (int)(BackGround_Message.Height / 16.5);
                ICO_DeleteMessage.Size = new Size((int)(BackGround_Message.Right / 1.146), H);
                ICO_DeleteMessage.Location = new Point((int)(BackGround_Message.Right / 18.58), TableHead_MessageWrite.Bottom);
                btn_Send.Centering_X(BackGround_Message.Left + BackGround_Message.Height - btn_Send.Height - tab_y);
                Text_Message_Write.Location = new Point((int)(BackGround_Message.Right / 18.58) + ToCSR(20), ICO_DeleteMessage.Bottom + ToCSR(40));
                Text_Message_Write.Size = new Size((int)(BackGround_Message.Right / 1.146) - ToCSR(40), btn_Send.Top - Text_Message_Write.Top - ToCSR(40));

                Form_Message.AutoSize = true; int w = Form_Message.Width; Form_Message.AutoSize = false;
                Form_Message.Size = new Size(w, Form_Message.MaximumSize.Height);
                Form_Message.Padding = new Padding(10, 10, 10, 10);
            }

            if (!Form_Message.Visible) Form_Message.ShowDialog();
        }
        //============================================== ДИАЛОГОВОЕ ОКНО :: СООБЩЕНИЯ ============================================

        //======================================== ДИАЛОГОВОЕ ОКНО :: О ПРОГРАММЕ / About ========================================
        /// <summary> Метод обрабатывает клик по ссылке в окне "о программе". </summary>
        private void About_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(About_LinkLabel_Link);//открываем ссылку в веб-браузере по умолчанию
        }

        /// <summary> Окно "о программе". </summary>
        private Form Form_About = null;
        /// <summary> Логотип игры. </summary>
        private PictureBox About_Logo = null;
        /// <summary> Кнопка закрытия окна "о программе". </summary>
        private Button About_btn_Close = null;
        /// <summary> Текстовая метка-заголовок. </summary>
        private Label About_Label_Header = null;
        ///<summary> Массив текстовых меток в окне "о программе". </summary>
        private Label[] About_Label_Card = null;
        /// <summary> Текстовая метка-ссылка ведущая на сайт в окне "о программе". </summary>
        private LinkLabel About_LinkLabel = null;
        /// <summary> Строка хранящая ссылку для метки-ссылки <b>About_LinkLabel</b>. </summary>
        private string About_LinkLabel_Link = "http://www.youtube.com/channel/UCowq7mi0EDPuDICouQ0UHdw/videos";
        /// <summary> Текстовый контейнер с информацией. </summary>
        private RichTextBox About_Rich_Information = null;
        /// <summary> Таблица информации в окне "о программе". </summary>
        private DataGridView About_TableInfo = null;
        public void WinDlg_AboutTheProgram() {
            if (Form_About == null) {
                float FSize = ToCSR(10);//размер шрифта текста
                string FName = "Arial";//имя шрифта текста
                Form_About = Extensions.CreateForm("Form_About", new Icon("DATA_BASE/IMG/logotip.ico"),
                    new Font(Font.FontFamily, FSize, FontStyle.Bold), 
                    StartPosition: FormStartPosition.CenterParent, FormBorderStyle: FormBorderStyle.FixedSingle,
                    Padding: new Padding(0, 0, 10, 10), ControlBox: true, KeyPreview: true, AutoSize: true);
                Form_About.KeyUp += new KeyEventHandler(KeyUp_EscapeFormClose);
                About_Logo = new PictureBox { Parent = Form_About, BackgroundImageLayout = ImageLayout.Stretch,
                    Size = new Size(ToCSR(256), ToCSR(256)),
                };
                About_btn_Close = new Button {
                    Parent = Form_About, Font = new Font(FName, FSize, FontStyle.Bold), AutoSize = true,
                    ForeColor = Color.Black, BackColor = Form_About.BackColor, Name = "About_btn_Close",
                };
                About_btn_Close.Click += Buttons_winDlg_Click;
                About_Label_Header = new Label { Parent = Form_About, AutoSize = true, ForeColor = Color.Black,
                    Font = new Font(FName, FSize + 6, FontStyle.Bold),
                };
                About_Label_Card = new Label[3];
                for (int i = 0; i < About_Label_Card.Length; i++) {
                    About_Label_Card[i] = new Label { Parent = Form_About, AutoSize = true, ForeColor = Color.Black,
                        Font = new Font(FName, FSize, FontStyle.Bold),
                    };
                }
                var _ = About_Label_Card[2];
                _.Font = new Font(FName, FSize - 2, FontStyle.Bold);
                _.ForeColor = Color.FromArgb(200, 0, 0);

                About_LinkLabel = new LinkLabel { Parent = Form_About, AutoSize = true, 
                    ActiveLinkColor = Color.Red, DisabledLinkColor = Color.SkyBlue, VisitedLinkColor = Color.Silver,
                    LinkColor = Color.Blue, LinkBehavior = LinkBehavior.HoverUnderline,
                    Font = new Font(FName, FSize, FontStyle.Bold),
                };
                About_LinkLabel.LinkClicked += About_LinkLabel_LinkClicked;
                About_Rich_Information = new RichTextBox {
                    Parent = Form_About, ReadOnly = true, WordWrap = true, ScrollBars = RichTextBoxScrollBars.Vertical,
                    BorderStyle = BorderStyle.FixedSingle, BackColor = Form_About.BackColor, Cursor = Cursors.Default,
                };
                //программное создание таблицы с общей информацией профиля
                About_TableInfo = Extensions.CreateGrid(Form_About, "About_TableInfo", new Font(FName, FSize, FontStyle.Regular),
                    new Point(10, 10), Form_About.BackColor, Form_About.BackColor);
                    //создание типов колонок
                    About_TableInfo.Columns.AddRange(new DataGridViewColumn[] {
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                        new DataGridViewTextBoxColumn { ReadOnly = true, Resizable = DataGridViewTriState.False, SortMode = DataGridViewColumnSortMode.NotSortable },
                    });
                    About_TableInfo.DefaultCellStyle.BackColor = Form_About.BackColor;//цвет фона каждой ячейки
                    //скрыть выделение ячеек
                    About_TableInfo.DefaultCellStyle.SelectionBackColor = About_TableInfo.DefaultCellStyle.BackColor;
                    About_TableInfo.DefaultCellStyle.SelectionForeColor = About_TableInfo.DefaultCellStyle.ForeColor;
                    //шрифт
                    About_TableInfo.Columns[0].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Bold);
                    About_TableInfo.Columns[1].DefaultCellStyle.Font = new Font(FName, FSize, FontStyle.Regular);
                    //автоматическое изменение ширины столбца чтобы поместился весь текст
                    About_TableInfo.AutoSize = true;
                    About_TableInfo.Columns[0].AutoSizeMode = About_TableInfo.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    //выравнивание содержимого ячейки относительно верх/низ, лево/право
                    About_TableInfo.Columns[0].DefaultCellStyle.Alignment = About_TableInfo.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
            //заполнение данными
            Form_About.Text = LANGUAGES.menu[8];/*О программе*/
            About_LinkLabel.Text = LANGUAGES.About[1]/*Посетить сайт*/ + " YouTube: " + About_LinkLabel_Link.Substring(0, About_LinkLabel_Link.Length / 2) + "...";
            About_LinkLabel.LinkArea = new LinkArea(About_LinkLabel.Text.Length - (About_LinkLabel_Link.Length / 2) - 1 - 3, (About_LinkLabel_Link.Length / 2) + 1 + 3);
                About_Label_Card[0].Text = LANGUAGES.About[2]/*Помочь проекту*/ + ":";
                About_Label_Card[1].Text = "Bank VTB: 4893 4704 7715 8775";
                About_Label_Card[2].Text = LANGUAGES.About[9];/*(Проверяйте номер на сайте YouTube под роликом)*/
            About_Label_Header.Text = LANGUAGES.About[8];/*Оффлайн версия игры Travian*/
            About_Logo.BackgroundImage = Image.FromFile("DATA_BASE/IMG/logotip.ico");
            About_Rich_Information.LoadFile(Directory.GetDirectories("DATA_BASE/LANGUAGES/")[GAME.Language] + "/AboutTheProgram.rtf");
            About_btn_Close.Text = LANGUAGES.About[0];/*закрыть*/
            About_TableInfo.Rows.Clear();
            About_TableInfo.Rows.Add(LANGUAGES.About[3]/*Версия:*/, "0.2.0");
            About_TableInfo.Rows.Add(LANGUAGES.About[4]/*Дата:*/, "31.12.2022");
            About_TableInfo.Rows.Add(LANGUAGES.About[5]/*Автор:*/, "Пивсаев Виктор / Pivsaev Victor");
            About_TableInfo.Rows.Add(LANGUAGES.About[6]/*Почта:*/, "workdelphi@rambler.ru / rusufo27@gmail.com");
            About_TableInfo.Rows.Add(LANGUAGES.About[7]/*Язык:*/, "C#");
            //размеры
            //About_Logo.BackgroundImage = Extensions.ResizeImage(About_Logo.BackgroundImage, UFO.Convert.ToCSR(512), UFO.Convert.ToCSR(512));
            About_Rich_Information.Size = new Size(About_Logo.Width + ToCSR(10) + About_LinkLabel.Width, ToCSR(300));
            About_Rich_Information.ZoomFactor();//масштабирвоание текста под разное разрешение экрана
            //разметка
            About_Logo.Location = new Point(ToCSR(10), ToCSR(10));
            About_Label_Header.Location = new Point(About_Logo.Left + About_Logo.Width + ToCSR(10), About_Logo.Top);
            About_TableInfo.Location = new Point(About_Label_Header.Left, About_Label_Header.Top + About_Label_Header.Height + ToCSR(10));
            About_LinkLabel.Location = new Point(About_TableInfo.Left, About_TableInfo.Top + About_TableInfo.Height);
            About_Label_Card[0].Location = new Point(About_LinkLabel.Left, About_LinkLabel.Top + About_LinkLabel.Height);
            About_Label_Card[1].Location = new Point(About_Label_Card[0].Left + ToCSR(20), About_Label_Card[0].Top + About_Label_Card[0].Height + ToCSR(5));
            About_Label_Card[2].Location = new Point(About_Label_Card[1].Left, About_Label_Card[1].Top + About_Label_Card[1].Height);
            About_Rich_Information.Location = new Point(About_Logo.Left, About_Label_Card[2].Top + About_Label_Card[2].Height + ToCSR(20));
            About_btn_Close.Location = new Point(About_Rich_Information.Width - About_btn_Close.Width, About_Rich_Information.Top + About_Rich_Information.Height + ToCSR(20));
            if (!Form_About.Visible) Form_About.ShowDialog();
        }
        //======================================== ДИАЛОГОВОЕ ОКНО :: О ПРОГРАММЕ / About ========================================
    }
}
